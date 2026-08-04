using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneForAll.Core;
using Pruner.Domain.Interfaces;
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
        private readonly IMonitorFileDeleteManager _deleteManager;
        private readonly IMonitorFileMoveManager _moveManager;

        public MonitorFileController(
            AuthConfig authConfig,
            IMonitorFileDeleteManager deleteManager,
            IMonitorFileMoveManager moveManager)
        {
            _authConfig = authConfig;
            _deleteManager = deleteManager;
            _moveManager = moveManager;
        }

        /// <summary>
        /// 手动执行文件删除任务
        /// </summary>
        [HttpPost("Delete")]
        public async Task<BaseMessage> ExecuteDeleteAsync()
        {
            var msg = new BaseMessage();
            var result = await _deleteManager.HandleDeleteAsync(_authConfig.ClientCode);
            return msg.Success(result);
        }

        /// <summary>
        /// 手动执行文件迁移任务
        /// </summary>
        [HttpPost("Move")]
        public async Task<BaseMessage> ExecuteMoveAsync()
        {
            var msg = new BaseMessage();
            var result = await _moveManager.HandleMoveAsync(_authConfig.ClientCode);
            return msg.Success(result);
        }
    }
}
