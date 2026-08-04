using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneForAll.Core;
using Pruner.Host.Models;
using Quartz;
using Quartz.Impl;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pruner.Host.Controllers
{
    /// <summary>
    /// 定时任务管理
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class JobsController : Controller
    {
        private readonly QuartzScheduleJobConfig _config;
        private readonly ISchedulerFactory _schedulerFactory;

        public JobsController(QuartzScheduleJobConfig config, ISchedulerFactory schedulerFactory)
        {
            _config = config;
            _schedulerFactory = schedulerFactory;
        }

        /// <summary>
        /// 获取定时任务列表（来自appsettings.json配置）
        /// </summary>
        [HttpGet]
        public async Task<List<object>> GetListAsync()
        {
            var scheduler = await _schedulerFactory.GetScheduler();

            var list = new List<object>();
            foreach (var job in _config.ScheduleJobs)
            {
                var running = false;
                var nextFireTime = (string)null;
                if (job.JobType != null && scheduler.IsStarted)
                {
                    var trigger = await scheduler.GetTrigger(new TriggerKey($"{job.JobType.FullName}.Trigger"));
                    if (trigger != null)
                    {
                        running = true;
                        var time = trigger.GetNextFireTimeUtc();
                        if (time.HasValue)
                            nextFireTime = time.Value.LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                }

                list.Add(new
                {
                    TypeName = job.TypeName,
                    Cron = job.Cron,
                    Remark = job.Remark,
                    Running = running,
                    NextFireTime = nextFireTime
                });
            }

            return list;
        }

        /// <summary>
        /// 手动触发定时任务
        /// </summary>
        /// <param name="typeName">任务类型名（如 MonitorDbDeleteJob）</param>
        [HttpPost("{typeName}/Trigger")]
        public async Task<BaseMessage> TriggerAsync(string typeName)
        {
            var msg = new BaseMessage();
            var job = _config.ScheduleJobs.FirstOrDefault(e => e.TypeName == typeName);
            if (job == null || job.JobType == null)
                return msg.Fail(BaseErrType.DataNotFound, "任务不存在或未注册");

            var scheduler = await _schedulerFactory.GetScheduler();
            if (!scheduler.IsStarted)
                return msg.Fail(BaseErrType.StateConflict, "调度器未启动");

            var jobKey = new JobKey(job.JobType.FullName);
            if (!await scheduler.CheckExists(jobKey))
                return msg.Fail(BaseErrType.DataNotFound, "任务未调度");

            await scheduler.TriggerJob(jobKey);
            return msg.Success($"任务[{typeName}]已触发");
        }
    }
}
