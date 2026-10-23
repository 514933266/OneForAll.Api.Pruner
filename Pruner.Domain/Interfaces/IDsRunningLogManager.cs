using OneForAll.Core;
using System.Threading.Tasks;

namespace Pruner.Domain.Interfaces
{
    /// <summary>
    /// 运行日志命令服务
    /// </summary>
    public interface IDsRunningLogManager
    {
        /// <summary>
        /// 删除指定运行日志
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>结果类型</returns>
        Task<BaseErrType> DeleteAsync(long id);

        /// <summary>
        /// 清空运行日志
        /// </summary>
        /// <returns>结果类型与清空条数（Count=0 表示本就为空或删除失败）</returns>
        Task<(BaseErrType Result, int Count)> ClearAsync();
    }
}
