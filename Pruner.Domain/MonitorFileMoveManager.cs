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
    /// 监控文件迁移
    /// </summary>
    public class MonitorFileMoveManager : BaseManager, IMonitorFileMoveManager
    {
        private readonly IDsMoveFileConfigRepository _configRepository;
        private readonly IDsRunningLogRepository _runningLogRepository;

        /// <summary>
        /// 单批迁移的文件数量，控制迁移过程中的内存占用
        /// </summary>
        private const int BatchSize = 1000;

        public MonitorFileMoveManager(
            IHttpContextAccessor httpContextAccessor,
            IDsMoveFileConfigRepository configRepository,
            IDsRunningLogRepository runningLogRepository) : base(httpContextAccessor)
        {
            _configRepository = configRepository;
            _runningLogRepository = runningLogRepository;
        }

        /// <summary>
        /// 执行文件迁移任务
        /// </summary>
        /// <param name="clientCode">客户端代码</param>
        /// <returns>迁移结果描述</returns>
        public async Task<string> HandleMoveAsync(string clientCode)
        {
            var configs = await _configRepository.GetListAsync(w => w.IsEnabled && w.ClientCode == clientCode);
            if (!configs.Any())
                return "文件迁移任务未配置任何目录，跳过执行";

            var totalMoved = 0;
            var sb = new StringBuilder();

            foreach (var config in configs)
            {
                try
                {
                    var (moved, failed, warning) = MoveExpiredFiles(config.SourcePath, config.TargetPath, config.KeepDays, config.MaxMoveCount, config.IncludeSubDirectories);
                    totalMoved += moved;

                    if (warning != null)
                    {
                        sb.AppendLine($"目录[{config.SourcePath}]{warning}");
                    }
                    else if (failed > 0)
                    {
                        sb.AppendLine($"目录[{config.SourcePath}]迁移{moved}个过期文件，{failed}个文件因权限不足或被占用迁移失败（保留{config.KeepDays}天）");
                    }
                    else
                    {
                        sb.AppendLine($"目录[{config.SourcePath}]迁移{moved}个过期文件（保留{config.KeepDays}天）");
                    }
                }
                catch (Exception ex)
                {
                    sb.AppendLine($"目录[{config.SourcePath}]处理失败：{ex.Message}");
                }
            }

            sb.Insert(0, $"文件迁移任务执行完成，共迁移{totalMoved}个文件。\n");

            // 记录本地运行日志
            await AddRunningLogAsync(sb.ToString());
            return sb.ToString();
        }

        /// <summary>
        /// 记录本地运行日志
        /// </summary>
        private async Task AddRunningLogAsync(string description)
        {
            await _runningLogRepository.AddAsync(new DsRunningLog()
            {
                TypeName = "MonitorFileMoveJob",
                TableName = "文件迁移",
                Description = description,
                CreateTime = DateTime.UtcNow
            });
        }

        /// <summary>
        /// 迁移指定目录下的过期文件
        /// </summary>
        /// <param name="sourcePath">源目录路径</param>
        /// <param name="targetPath">目标目录路径</param>
        /// <param name="keepDays">文件保留天数</param>
        /// <param name="maxMoveCount">每次最大迁移数量（0表示不限制）</param>
        /// <param name="includeSubDirs">是否递归迁移（递归迁移时，会按照原文件所在目录层级迁移）</param>
        /// <returns>(成功迁移数, 失败数, 警告信息)</returns>
        private (int moved, int failed, string warning) MoveExpiredFiles(string sourcePath, string targetPath, int keepDays, int maxMoveCount, bool includeSubDirs)
        {
            if (!Directory.Exists(sourcePath))
                return (0, 0, "源目录不存在，跳过执行");

            // 确保目标目录存在
            if (!Directory.Exists(targetPath))
            {
                try
                {
                    Directory.CreateDirectory(targetPath);
                }
                catch
                {
                    return (0, 0, "目标目录无法创建，跳过执行");
                }
            }

            var cutoffDate = DateTime.Now.AddDays(-keepDays);
            var moved = 0;
            var failed = 0;
            var unlimited = maxMoveCount <= 0;
            var remaining = maxMoveCount;

            // 已确认存在的目标子目录缓存，避免每个文件迁移前都重复检查目标目录
            var ensuredDirs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            ensuredDirs.Add(Path.GetFullPath(targetPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

            // 待处理目录：源根目录自身 + 递归子目录（仅目录路径，量级远小于文件数量）
            var dirsToProcess = new List<string> { sourcePath };
            if (includeSubDirs)
            {
                try
                {
                    var enumOptions = new EnumerationOptions
                    {
                        RecurseSubdirectories = true,
                        IgnoreInaccessible = true
                    };
                    dirsToProcess.AddRange(Directory.EnumerateDirectories(sourcePath, "*", enumOptions));
                }
                catch (UnauthorizedAccessException)
                {
                    return (0, 0, "无目录访问权限，跳过执行");
                }
            }

            foreach (var dirPath in dirsToProcess)
            {
                // 剩余额度耗尽，任务完成
                if (!unlimited && remaining <= 0)
                    break;

                // 处理单个目录：若该目录过期文件数超过剩余额度，则只迁移剩余额度个并结束任务；
                // 若不超过，则全部迁移后继续下一个目录（递归时进入下一层级子目录）
                var (dirMoved, dirFailed, accessible) = MoveExpiredFilesInDir(dirPath, cutoffDate, unlimited ? 0 : remaining, sourcePath, targetPath, includeSubDirs, ensuredDirs);
                if (!accessible && dirPath == sourcePath)
                    return (0, 0, "无目录访问权限，跳过执行");
                moved += dirMoved;
                failed += dirFailed;

                // 尝试处理的文件数消耗额度（与 Take 截断候选语义一致，迁移失败也算消耗）
                if (!unlimited)
                    remaining -= dirMoved + dirFailed;
            }

            return (moved, failed, null);
        }

        /// <summary>
        /// 迁移单个目录下的过期文件
        /// </summary>
        /// <param name="dirPath">源目录路径</param>
        /// <param name="cutoffDate">过期时间界限</param>
        /// <param name="maxCount">该目录最多迁移数量（0表示不限制）</param>
        /// <param name="sourcePath">源根目录路径（递归迁移时用于计算相对层级）</param>
        /// <param name="targetPath">目标目录路径</param>
        /// <param name="includeSubDirs">是否递归迁移</param>
        /// <param name="ensuredDirs">已确认存在的目标子目录缓存</param>
        /// <returns>(成功迁移数, 失败数, 是否成功访问该目录)</returns>
        private (int moved, int failed, bool accessible) MoveExpiredFilesInDir(string dirPath, DateTime cutoffDate, int maxCount, string sourcePath, string targetPath, bool includeSubDirs, HashSet<string> ensuredDirs)
        {
            var moved = 0;
            var failed = 0;
            var unlimited = maxCount <= 0;

            try
            {
                // EnumerateFiles 枚举时即带回文件时间，无需再逐文件调用 GetLastWriteTime；
                // 小批量物化后迁移，避免文件过多时一次性加载全部路径占用大量内存
                var batch = new List<FileInfo>(BatchSize);
                foreach (var fileInfo in new DirectoryInfo(dirPath).EnumerateFiles())
                {
                    // 达到该目录的最大迁移数量后停止（含已加入批次尚未迁移的文件）
                    if (!unlimited && moved + failed + batch.Count >= maxCount)
                        break;
                    if (fileInfo.LastWriteTime >= cutoffDate)
                        continue;

                    batch.Add(fileInfo);
                    if (batch.Count >= BatchSize)
                    {
                        MoveBatch(batch, sourcePath, targetPath, includeSubDirs, ensuredDirs, ref moved, ref failed);
                        batch.Clear();
                    }
                }
                if (batch.Count > 0)
                    MoveBatch(batch, sourcePath, targetPath, includeSubDirs, ensuredDirs, ref moved, ref failed);
                return (moved, failed, true);
            }
            catch (UnauthorizedAccessException)
            {
                // 目录无权限时跳过，不影响其他目录处理
                return (moved, failed, false);
            }
        }

        /// <summary>
        /// 批量迁移文件
        /// </summary>
        private void MoveBatch(List<FileInfo> batch, string sourcePath, string targetPath, bool includeSubDirs, HashSet<string> ensuredDirs, ref int moved, ref int failed)
        {
            foreach (var fileInfo in batch)
            {
                try
                {
                    string destPath;
                    if (includeSubDirs)
                    {
                        // 递归迁移：按原文件相对源目录的层级迁移，并确保目标子目录存在
                        destPath = Path.Combine(targetPath, Path.GetRelativePath(sourcePath, fileInfo.FullName));
                        var destDir = Path.GetDirectoryName(destPath);
                        if (!string.IsNullOrEmpty(destDir) && !ensuredDirs.Contains(destDir))
                        {
                            Directory.CreateDirectory(destDir);
                            ensuredDirs.Add(destDir);
                        }
                    }
                    else
                    {
                        destPath = Path.Combine(targetPath, fileInfo.Name);
                    }

                    // 目标文件已存在时直接覆盖，避免先查再删的额外系统调用
                    File.Move(fileInfo.FullName, destPath, true);
                    moved++;
                }
                catch
                {
                    failed++;
                }
            }
        }
    }
}
