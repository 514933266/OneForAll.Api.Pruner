using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneForAll.Core;
using Pruner.Application.Interfaces;
using Pruner.Public.Models;
using System.Threading.Tasks;

namespace Pruner.Host.Controllers
{
    /// <summary>
    /// 监控文件类任务手动执行
    /// </summary>
    [Route("api/Monitor/File")]
    [ApiController]
    [AllowAnonymous]
    public class MonitorFileController : Controller
    {
        private readonly AuthConfig _authConfig;
        private readonly IMonitorExecService _execService;

        public MonitorFileController(
            AuthConfig authConfig,
            IMonitorExecService execService)
        {
            _authConfig = authConfig;
            _execService = execService;
        }

        /// <summary>
        /// 手动执行文件删除任务
        /// </summary>
        [HttpPost("Delete")]
        public async Task<BaseMessage> ExecuteDeleteAsync()
        {
            var result = await _execService.HandleFileDeleteAsync(_authConfig.ClientCode);
            return new BaseMessage().Success(result.Message);
        }

        /// <summary>
        /// 手动执行文件迁移任务
        /// </summary>
        [HttpPost("Move")]
        public async Task<BaseMessage> ExecuteMoveAsync()
        {
            var result = await _execService.HandleFileMoveAsync(_authConfig.ClientCode);
            return new BaseMessage().Success(result.Message);
        }
    }
}
