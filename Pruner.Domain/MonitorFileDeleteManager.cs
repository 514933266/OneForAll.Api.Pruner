using Microsoft.AspNetCore.Http;
using OneForAll.Core;
using Pruner.Domain.Entities;
using Pruner.Domain.Interfaces;
using Pruner.Domain.Repositorys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pruner.Domain
{
    /// <summary>
    /// 监控文件删除
    /// </summary>
    public class MonitorFileDeleteManager : BaseManager, IMonitorFileDeleteManager
    {
        private readonly IDsDeleteFileConfigRepository _configRepository;
        private readonly IDsRunningLogRepository _runningLogRepository;

        /// <summary>
        /// 单批删除的文件数量，控制删除过程中的内存占用
        /// </summary>
        private const int BatchSize = 1000;

        public MonitorFileDeleteManager(
            IHttpContextAccessor httpContextAccessor,
            IDsDeleteFileConfigRepository configRepository,
            IDsRunningLogRepository runningLogRepository) : base(httpContextAccessor)
        {
            _configRepository = configRepository;
            _runningLogRepository = runningLogRepository;
        }

        /// <summary>
        /// 执行文件删除任务
        /// </summary>
        /// <param name="clientCode">客户端代码</param>
        /// <returns>删除结果描述</returns>
        public async Task<string> HandleDeleteAsync(string clientCode)
        {
            var configs = (await _configRepository.GetListAsync(w => w.IsEnabled && w.ClientCode == clientCode)).ToList();
            if (!configs.Any())
                return "文件删除任务未配置任何目录，跳过执行";

            var totalDeleted = 0;
            var sb = new StringBuilder();

            foreach (var config in configs)
            {
                try
                {
                    var (deleted, failed, removedDirs, warning) = DeleteExpiredFiles(config.Path, config.KeepDays, config.MaxDeleteCount, config.IncludeSubDirectories);
                    totalDeleted += deleted;

                    // 每处理完一个目录记录一次本地运行日志
                    var dirSummary = removedDirs > 0 ? $"，清理{removedDirs}个空目录" : "";
                    if (warning != null)
                    {
                        var desc = $"目录[{config.Path}]{warning}";
                        sb.AppendLine(desc);
                        await AddRunningLogAsync(config.Path, desc, config.ClientCode);
                    }
                    else if (failed > 0)
                    {
                        var desc = $"目录[{config.Path}]删除{deleted}个过期文件，{failed}个文件因权限不足或被占用删除失败（保留{config.KeepDays}天）{dirSummary}";
                        sb.AppendLine(desc);
                        await AddRunningLogAsync(config.Path, desc, config.ClientCode);
                    }
                    else
                    {
                        var desc = $"目录[{config.Path}]删除{deleted}个过期文件（保留{config.KeepDays}天）{dirSummary}";
                        sb.AppendLine(desc);
                        await AddRunningLogAsync(config.Path, desc, config.ClientCode);
                    }
                }
                catch (Exception ex)
                {
                    var desc = $"目录[{config.Path}]处理失败：{ex.Message}";
                    sb.AppendLine(desc);
                    await AddRunningLogAsync(config.Path, desc, config.ClientCode);
                }
            }

            sb.Insert(0, $"文件删除任务执行完成，共删除{totalDeleted}个文件。\n");
            return sb.ToString();
        }

        /// <summary>
        /// 记录本地运行日志
        /// </summary>
        private async Task AddRunningLogAsync(string tableName, string description, string clientCode)
        {
            await _runningLogRepository.AddAsync(new DsRunningLog()
            {
                TypeName = "MonitorFileDeleteJob",
                TableName = tableName,
                Description = description,
                ClientCode = clientCode,
                CreateTime = DateTime.UtcNow
            });
        }

        /// <summary>
        /// 删除指定目录下的过期文件，文件删除后若所在子目录为空则一并删除该子目录
        /// </summary>
        /// <returns>(成功删除数, 失败数, 清理空目录数, 警告信息)</returns>
        private (int deleted, int failed, int removedDirs, string warning) DeleteExpiredFiles(string path, int keepDays, int maxDeleteCount, bool includeSubDirs)
        {
            if (!Directory.Exists(path))
                return (0, 0, 0, "目录不存在，跳过执行");

            var cutoffDate = DateTime.Now.AddDays(-keepDays);
            var deleted = 0;
            var failed = 0;
            var removedDirs = 0;
            var unlimited = maxDeleteCount <= 0;
            var remaining = maxDeleteCount;

            // 待检查的空目录集合：删除文件后延迟统一清理，避免每个文件删除后都枚举一次父目录
            var pendingDirs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // 待处理目录：根目录自身 + 递归子目录（仅目录路径，量级远小于文件数量）
            var dirsToProcess = new List<string> { path };
            if (includeSubDirs)
            {
                try
                {
                    var enumOptions = new EnumerationOptions
                    {
                        RecurseSubdirectories = true,
                        IgnoreInaccessible = true
                    };
                    dirsToProcess.AddRange(Directory.EnumerateDirectories(path, "*", enumOptions));
                }
                catch (UnauthorizedAccessException)
                {
                    return (0, 0, 0, "无目录访问权限，跳过执行");
                }
            }

            foreach (var dirPath in dirsToProcess)
            {
                // 剩余额度耗尽，任务完成
                if (!unlimited && remaining <= 0)
                    break;

                // 处理单个目录：若该目录过期文件数超过剩余额度，则只删除剩余额度个并结束任务；
                // 若不超过，则全部删除后继续下一个目录（递归时进入下一层级子目录）
                var (dirDeleted, dirFailed, accessible) = DeleteExpiredFilesInDir(dirPath, cutoffDate, unlimited ? 0 : remaining, pendingDirs);
                if (!accessible && dirPath == path)
                    return (0, 0, 0, "无目录访问权限，跳过执行");
                deleted += dirDeleted;
                failed += dirFailed;

                // 尝试处理的文件数消耗额度（与 Take 截断候选语义一致，删除失败也算消耗）
                if (!unlimited)
                    remaining -= dirDeleted + dirFailed;
            }

            // 统一清理删除后变空的目录（按深度从深到浅，可顺带向上清理；不删除配置根目录）
            var rootPath = Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            foreach (var dirPath in pendingDirs
                .Where(d => !string.Equals(Path.GetFullPath(d).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar), rootPath, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(d => d.Count(c => c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar)))
            {
                if (IsDirectoryEmpty(dirPath))
                {
                    try
                    {
                        Directory.Delete(dirPath, false);
                        removedDirs++;
                    }
                    catch
                    {
                        // 空目录删除失败不影响文件删除结果
                    }
                }
            }

            return (deleted, failed, removedDirs, null);
        }

        /// <summary>
        /// 删除单个目录下的过期文件
        /// </summary>
        /// <param name="dirPath">目录路径</param>
        /// <param name="cutoffDate">过期时间界限</param>
        /// <param name="maxCount">该目录最多删除数量（0表示不限制）</param>
        /// <param name="pendingDirs">待检查的空目录集合</param>
        /// <returns>(成功删除数, 失败数, 是否成功访问该目录)</returns>
        private (int deleted, int failed, bool accessible) DeleteExpiredFilesInDir(string dirPath, DateTime cutoffDate, int maxCount, HashSet<string> pendingDirs)
        {
            var deleted = 0;
            var failed = 0;
            var unlimited = maxCount <= 0;

            try
            {
                // EnumerateFiles 枚举时即带回文件时间，无需再逐文件调用 GetLastWriteTime；
                // 小批量物化后删除，避免文件过多时一次性加载全部路径占用大量内存
                var batch = new List<FileInfo>(BatchSize);
                foreach (var fileInfo in new DirectoryInfo(dirPath).EnumerateFiles())
                {
                    // 达到该目录的最大删除数量后停止（含已加入批次尚未删除的文件）
                    if (!unlimited && deleted + failed + batch.Count >= maxCount)
                        break;
                    if (fileInfo.LastWriteTime >= cutoffDate)
                        continue;

                    batch.Add(fileInfo);
                    if (batch.Count >= BatchSize)
                    {
                        DeleteBatch(batch, pendingDirs, ref deleted, ref failed);
                        batch.Clear();
                    }
                }
                if (batch.Count > 0)
                    DeleteBatch(batch, pendingDirs, ref deleted, ref failed);
                return (deleted, failed, true);
            }
            catch (UnauthorizedAccessException)
            {
                // 目录无权限时跳过，不影响其他目录处理
                return (deleted, failed, false);
            }
        }

        /// <summary>
        /// 批量删除文件，并记录文件所在目录供最后统一检查空目录
        /// </summary>
        private void DeleteBatch(List<FileInfo> batch, HashSet<string> pendingDirs, ref int deleted, ref int failed)
        {
            foreach (var fileInfo in batch)
            {
                try
                {
                    File.Delete(fileInfo.FullName);
                    deleted++;

                    var parentDir = fileInfo.DirectoryName;
                    if (!string.IsNullOrEmpty(parentDir))
                        pendingDirs.Add(parentDir);
                }
                catch
                {
                    failed++;
                }
            }
        }

        /// <summary>
        /// 判断目录是否为空（无文件且无子目录）
        /// </summary>
        private static bool IsDirectoryEmpty(string dirPath)
        {
            try
            {
                return !Directory.EnumerateFileSystemEntries(dirPath).Any();
            }
            catch
            {
                return false;
            }
        }
    }
}
