using Pruner.Application.Dtos;
using Pruner.Application.Interfaces;
using Pruner.Domain.Interfaces;
using System.Threading.Tasks;

namespace Pruner.Application
{
    /// <summary>
    /// 监控类任务手动执行（数据库删除 / 文件删除 / 文件迁移）
    /// </summary>
    public class MonitorExecService : IMonitorExecService
    {
        private readonly IMonitorDbDeleteManager _dbDeleteManager;
        private readonly IMonitorFileDeleteManager _fileDeleteManager;
        private readonly IMonitorFileMoveManager _fileMoveManager;

        public MonitorExecService(
            IMonitorDbDeleteManager dbDeleteManager,
            IMonitorFileDeleteManager fileDeleteManager,
            IMonitorFileMoveManager fileMoveManager)
        {
            _dbDeleteManager = dbDeleteManager;
            _fileDeleteManager = fileDeleteManager;
            _fileMoveManager = fileMoveManager;
        }

        /// <summary>
        /// 执行数据库数据删除任务
        /// </summary>
        /// <param name="clientCode">客户端代码</param>
        /// <returns>执行结果</returns>
        public async Task<MonitorExecResultDto> HandleDbDeleteAsync(string clientCode)
        {
            var count = await _dbDeleteManager.HandleDeleteAsync(clientCode);
            return new MonitorExecResultDto
            {
                TotalCount = count,
                Message = $"数据维护删除任务执行完成，共删除{count}条数据"
            };
        }

        /// <summary>
        /// 执行文件删除任务
        /// </summary>
        /// <param name="clientCode">客户端代码</param>
        /// <returns>执行结果</returns>
        public async Task<MonitorExecResultDto> HandleFileDeleteAsync(string clientCode)
        {
            var message = await _fileDeleteManager.HandleDeleteAsync(clientCode);
            return new MonitorExecResultDto { Message = message };
        }

        /// <summary>
        /// 执行文件迁移任务
        /// </summary>
        /// <param name="clientCode">客户端代码</param>
        /// <returns>执行结果</returns>
        public async Task<MonitorExecResultDto> HandleFileMoveAsync(string clientCode)
        {
            var message = await _fileMoveManager.HandleMoveAsync(clientCode);
            return new MonitorExecResultDto { Message = message };
        }
    }
}
