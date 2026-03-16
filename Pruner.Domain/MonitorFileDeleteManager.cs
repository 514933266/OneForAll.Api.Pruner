using Microsoft.AspNetCore.Http;
using OneForAll.Core;
using Pruner.Domain.Interfaces;
using Pruner.Domain.Repositorys;
using System;
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

        public MonitorFileDeleteManager(
            IHttpContextAccessor httpContextAccessor,
            IDsDeleteFileConfigRepository configRepository) : base(httpContextAccessor)
        {
            _configRepository = configRepository;
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
                    var deleted = DeleteExpiredFiles(config.Path, config.KeepDays, config.MaxDeleteCount, config.IncludeSubDirectories);
                    totalDeleted += deleted;
                    sb.AppendLine($"目录[{config.Path}]删除{deleted}个过期文件（保留{config.KeepDays}天）");
                }
                catch (Exception ex)
                {
                    sb.AppendLine($"目录[{config.Path}]处理失败：{ex.Message}");
                }
            }

            sb.Insert(0, $"文件删除任务执行完成，共删除{totalDeleted}个文件。\n");
            return sb.ToString();
        }

        /// <summary>
        /// 删除指定目录下的过期文件
        /// </summary>
        private int DeleteExpiredFiles(string path, int keepDays, int maxDeleteCount, bool includeSubDirs)
        {
            if (!Directory.Exists(path))
                return 0;

            var cutoffDate = DateTime.Now.AddDays(-keepDays);
            var deleted = 0;
            var searchOption = includeSubDirs ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            var files = Directory.EnumerateFiles(path, "*", searchOption)
                .Where(f => File.GetLastWriteTime(f) < cutoffDate);

            // 限制最大读取数量，避免文件过多时占用大量内存
            var candidates = maxDeleteCount > 0 ? files.Take(maxDeleteCount) : files;

            foreach (var filePath in candidates)
            {
                try
                {
                    File.Delete(filePath);
                    deleted++;
                }
                catch
                {
                    // 跳过无法删除的文件（可能被占用）
                }
            }

            return deleted;
        }
    }
}
