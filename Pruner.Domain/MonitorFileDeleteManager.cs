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
                        await AddRunningLogAsync(config.Path, desc);
                    }
                    else if (failed > 0)
                    {
                        var desc = $"目录[{config.Path}]删除{deleted}个过期文件，{failed}个文件因权限不足或被占用删除失败（保留{config.KeepDays}天）{dirSummary}";
                        sb.AppendLine(desc);
                        await AddRunningLogAsync(config.Path, desc);
                    }
                    else
                    {
                        var desc = $"目录[{config.Path}]删除{deleted}个过期文件（保留{config.KeepDays}天）{dirSummary}";
                        sb.AppendLine(desc);
                        await AddRunningLogAsync(config.Path, desc);
                    }
                }
                catch (Exception ex)
                {
                    var desc = $"目录[{config.Path}]处理失败：{ex.Message}";
                    sb.AppendLine(desc);
                    await AddRunningLogAsync(config.Path, desc);
                }
            }

            sb.Insert(0, $"文件删除任务执行完成，共删除{totalDeleted}个文件。\n");
            return sb.ToString();
        }

        /// <summary>
        /// 记录本地运行日志
        /// </summary>
        private async Task AddRunningLogAsync(string tableName, string description)
        {
            await _runningLogRepository.AddAsync(new DsRunningLog()
            {
                TypeName = "MonitorFileDeleteJob",
                TableName = tableName,
                Description = description,
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

            // 使用 EnumerationOptions 跳过无权限的子目录，避免枚举中断
            var enumOptions = new EnumerationOptions
            {
                RecurseSubdirectories = includeSubDirs,
                IgnoreInaccessible = true
            };

            IEnumerable<string> files;
            try
            {
                files = Directory.EnumerateFiles(path, "*", enumOptions);
            }
            catch (UnauthorizedAccessException)
            {
                return (0, 0, 0, "无目录访问权限，跳过执行");
            }

            // 过滤过期文件，安全获取文件时间以防权限问题
            var candidates = files.Where(f =>
            {
                try
                {
                    return File.GetLastWriteTime(f) < cutoffDate;
                }
                catch
                {
                    return false;
                }
            });

            // 限制最大读取数量，避免文件过多时占用大量内存
            if (maxDeleteCount > 0)
                candidates = candidates.Take(maxDeleteCount);

            // 先物化候选列表，避免删除文件/目录时枚举器失效
            var candidateList = candidates.ToList();

            var rootPath = Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var removedDirs = 0;

            foreach (var filePath in candidateList)
            {
                try
                {
                    File.Delete(filePath);
                    deleted++;

                    // 文件删除成功后，若其所在子目录为空则一并删除（不删除配置根目录）
                    var dirPath = Path.GetDirectoryName(filePath);
                    if (!string.IsNullOrEmpty(dirPath)
                        && !string.Equals(Path.GetFullPath(dirPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar), rootPath, StringComparison.OrdinalIgnoreCase)
                        && IsDirectoryEmpty(dirPath))
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
                catch
                {
                    failed++;
                }
            }

            return (deleted, failed, removedDirs, null);
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
