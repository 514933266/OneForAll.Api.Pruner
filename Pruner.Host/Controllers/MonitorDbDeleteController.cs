using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneForAll.Core;
using Pruner.Domain.Interfaces;
using Pruner.Host.Models;
using Pruner.Host.QuartzJobs;
using Pruner.HttpService.Interfaces;
using Pruner.Public.Models;
using System;
using System.Threading.Tasks;

namespace Pruner.Host.Controllers
{
    /// <summary>
    /// 监控数据库数据删除
    /// </summary>
    [Route("api/Monitor/DbData")]
    [Authorize(Roles = UserRoleType.Admin)]
    public class MonitorDbDeleteController : Controller
    {
        private readonly IMonitorDbDeleteManager _manager;

        public MonitorDbDeleteController(IMonitorDbDeleteManager manager)
        {
            _manager = manager;
        }

        /// <summary>
        /// 手动执行监控数据库数据删除任务
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public async Task<BaseMessage> ExecuteAsync()
        {
            var msg = new BaseMessage();
            var result = await _manager.HandleDeleteAsync();
            return msg.Success($"数据维护删除任务执行完成，共删除{result}条数据");
        }
    }
}
