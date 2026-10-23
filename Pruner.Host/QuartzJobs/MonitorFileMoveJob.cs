using Pruner.Application.Interfaces;
using Pruner.Host.Models;
using Pruner.HttpService.Interfaces;
using Pruner.Public.Models;
using Quartz;
using System.Threading.Tasks;

namespace Pruner.Host.QuartzJobs
{
    /// <summary>
    /// 监控文件迁移任务
    /// </summary>
    [DisallowConcurrentExecution]
    public class MonitorFileMoveJob : BaseLockJob
    {
        private readonly IMonitorExecService _execService;

        public MonitorFileMoveJob(
            IMonitorExecService execService,
            QuartzScheduleJobConfig jobConfig,
            AuthConfig authConfig,
            JobLockConfig lockConfig,
            IScheduleJobHttpService jobHttpService,
            ISysGlobalExceptionLogHttpService logHttpService)
            : base(jobConfig, authConfig, lockConfig, jobHttpService, logHttpService)
        {
            _execService = execService;
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        protected override async Task ExecuteInternalAsync(IJobExecutionContext context)
        {
            var result = await _execService.HandleFileMoveAsync(_authConfig.ClientCode);
            await AddLogAsync(result.Message);
        }
    }
}
