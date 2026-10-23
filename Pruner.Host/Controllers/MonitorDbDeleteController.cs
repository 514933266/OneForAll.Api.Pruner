using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneForAll.Core;
using Pruner.Application.Interfaces;
using Pruner.Public.Models;
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
        private readonly AuthConfig _authConfig;
        private readonly IMonitorExecService _execService;

        public MonitorDbDeleteController(
            AuthConfig authConfig,
            IMonitorExecService execService)
        {
            _authConfig = authConfig;
            _execService = execService;
        }

        /// <summary>
        /// 手动执行监控数据库数据删除任务
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public async Task<BaseMessage> ExecuteAsync()
        {
            var result = await _execService.HandleDbDeleteAsync(_authConfig.ClientCode);
            return new BaseMessage().Success(result.Message);
        }
    }
}
