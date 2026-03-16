using Pruner.Domain.Interfaces;
using Pruner.Host.Models;
using Pruner.HttpService.Interfaces;
using Quartz;
using System;
using System.Threading.Tasks;

namespace Pruner.Host.QuartzJobs
{
    /// <summary>
    /// 监控数据库数据删除任务
    /// </summary>
    [DisallowConcurrentExecution]
    public class MonitorDbDeleteJob : IJob
    {
        private readonly AuthConfig _config;
        private readonly IScheduleJobHttpService _service;
        private readonly IMonitorDbDeleteManager _manager;

        public MonitorDbDeleteJob(
            AuthConfig config,
            IScheduleJobHttpService service,
            IMonitorDbDeleteManager manager)
        {
            _config = config;
            _service = service;
            _manager = manager;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var result = await _manager.HandleDeleteAsync();
                await AddLogAsync($"数据维护删除任务执行完成，共删除{result}条数据");
            }
            catch (Exception ex)
            {
                await AddLogAsync($"数据维护删除任务执行失败：{ex.Message}\n{ex.StackTrace}", true);
            }
        }

        /// <summary>
        /// 记录定时任务日志
        /// </summary>
        private async Task AddLogAsync(string log, bool isException = false)
        {
            await _service.LogAsync(_config.ClientCode, typeof(MonitorDbDeleteJob).Name, log, isException);
        }
    }
}
