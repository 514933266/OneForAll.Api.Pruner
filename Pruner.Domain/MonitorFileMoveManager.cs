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

            // 使用 EnumerationOptions 跳过无权限的子目录，避免枚举中断
            var enumOptions = new EnumerationOptions
            {
                RecurseSubdirectories = includeSubDirs,
                IgnoreInaccessible = true
            };

            IEnumerable<string> files;
            try
            {
                files = Directory.EnumerateFiles(sourcePath, "*", enumOptions);
            }
            catch (UnauthorizedAccessException)
            {
                return (0, 0, "无目录访问权限，跳过执行");
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
            }).ToList();

            // 限制最大读取数量，避免文件过多时占用大量内存
            if (maxMoveCount > 0)
                candidates = candidates.Take(maxMoveCount).ToList();

            foreach (var filePath in candidates)
            {
                try
                {
                    var fileName = Path.GetFileName(filePath);
                    var destPath = Path.Combine(targetPath, fileName);

                    // 如果目标文件已存在，先删除目标文件
                    if (File.Exists(destPath))
                    {
                        File.Delete(destPath);
                    }

                    File.Move(filePath, destPath);
                    moved++;
                }
                catch
                {
                    failed++;
                }
            }

            return (moved, failed, null);
        }
    }
}
