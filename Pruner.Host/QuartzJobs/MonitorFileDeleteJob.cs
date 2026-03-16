using Pruner.Domain.Interfaces;
using Pruner.Host.Models;
using Pruner.HttpService.Interfaces;
using Quartz;
using System;
using System.Threading.Tasks;

namespace Pruner.Host.QuartzJobs
{
    /// <summary>
    /// 监控文件删除任务
    /// </summary>
    [DisallowConcurrentExecution]
    public class MonitorFileDeleteJob : IJob
    {
        private readonly AuthConfig _authConfig;
        private readonly IScheduleJobHttpService _service;
        private readonly IMonitorFileDeleteManager _manager;

        public MonitorFileDeleteJob(
            AuthConfig authConfig,
            IScheduleJobHttpService service,
            IMonitorFileDeleteManager manager)
        {
            _authConfig = authConfig;
            _service = service;
            _manager = manager;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var result = await _manager.HandleDeleteAsync(_authConfig.ClientCode);
                await AddLogAsync(result);
            }
            catch (Exception ex)
            {
                await AddLogAsync($"文件删除任务执行失败：{ex.Message}\n{ex.StackTrace}", true);
            }
        }

        /// <summary>
        /// 记录定时任务日志
        /// </summary>
        private async Task AddLogAsync(string log, bool isException = false)
        {
            await _service.LogAsync(_authConfig.ClientCode, typeof(MonitorFileDeleteJob).Name, log, isException);
        }
    }
}
