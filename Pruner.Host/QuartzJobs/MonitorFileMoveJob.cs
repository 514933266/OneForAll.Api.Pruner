using Pruner.Domain.Interfaces;
using Pruner.Host.Models;
using Pruner.HttpService.Interfaces;
using Pruner.Public.Models;
using Quartz;
using System;
using System.Threading.Tasks;

namespace Pruner.Host.QuartzJobs
{
    /// <summary>
    /// 监控文件迁移任务
    /// </summary>
    [DisallowConcurrentExecution]
    public class MonitorFileMoveJob : IJob
    {
        private readonly AuthConfig _authConfig;
        private readonly IScheduleJobHttpService _service;
        private readonly IMonitorFileMoveManager _manager;

        public MonitorFileMoveJob(
            AuthConfig authConfig,
            IScheduleJobHttpService service,
            IMonitorFileMoveManager manager)
        {
            _authConfig = authConfig;
            _service = service;
            _manager = manager;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var result = await _manager.HandleMoveAsync(_authConfig.ClientCode);
                await AddLogAsync(result);
            }
            catch (Exception ex)
            {
                await AddLogAsync($"文件迁移任务执行失败：{ex.Message}\n{ex.StackTrace}", true);
            }
        }

        /// <summary>
        /// 记录定时任务日志
        /// </summary>
        private async Task AddLogAsync(string log, bool isException = false)
        {
            await _service.LogAsync(_authConfig.ClientCode, typeof(MonitorFileMoveJob).Name, log, isException);
        }
    }
}
