using Pruner.Application.Dtos;
using System.Threading.Tasks;

namespace Pruner.Application.Interfaces
{
    /// <summary>
    /// 监控类任务手动执行（数据库删除 / 文件删除 / 文件迁移）
    /// </summary>
    public interface IMonitorExecService
    {
        /// <summary>
        /// 执行数据库数据删除任务
        /// </summary>
        /// <param name="clientCode">客户端代码</param>
        /// <returns>执行结果</returns>
        Task<MonitorExecResultDto> HandleDbDeleteAsync(string clientCode);

        /// <summary>
        /// 执行文件删除任务
        /// </summary>
        /// <param name="clientCode">客户端代码</param>
        /// <returns>执行结果</returns>
        Task<MonitorExecResultDto> HandleFileDeleteAsync(string clientCode);

        /// <summary>
        /// 执行文件迁移任务
        /// </summary>
        /// <param name="clientCode">客户端代码</param>
        /// <returns>执行结果</returns>
        Task<MonitorExecResultDto> HandleFileMoveAsync(string clientCode);
    }
}
