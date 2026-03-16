using System.Threading.Tasks;

namespace Pruner.Domain.Interfaces
{
    /// <summary>
    /// 监控文件删除
    /// </summary>
    public interface IMonitorFileDeleteManager
    {
        /// <summary>
        /// 执行文件删除任务
        /// </summary>
        /// <param name="clientCode">客户端代码</param>
        /// <returns>删除结果描述</returns>
        Task<string> HandleDeleteAsync(string clientCode);
    }
}
