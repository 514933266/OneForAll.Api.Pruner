using System.Threading.Tasks;

namespace Pruner.Domain.Interfaces
{
    /// <summary>
    /// 监控文件迁移
    /// </summary>
    public interface IMonitorFileMoveManager
    {
        /// <summary>
        /// 执行文件迁移任务
        /// </summary>
        /// <param name="clientCode">客户端代码</param>
        /// <returns>迁移结果描述</returns>
        Task<string> HandleMoveAsync(string clientCode);
    }
}
