using System.Threading.Tasks;

namespace Pruner.Domain.Interfaces
{
    /// <summary>
    /// 监控数据库数据删除
    /// </summary>
    public interface IMonitorDbDeleteManager
    {
        /// <summary>
        /// 执行数据删除任务
        /// </summary>
        /// <returns>删除的总记录数</returns>
        Task<int> HandleDeleteAsync();
    }
}
