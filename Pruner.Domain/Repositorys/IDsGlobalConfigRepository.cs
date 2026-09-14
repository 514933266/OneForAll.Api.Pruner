using OneForAll.Core;
using OneForAll.EFCore;
using Pruner.Domain.Entities;
using System.Threading.Tasks;

namespace Pruner.Domain.Repositorys
{
    /// <summary>
    /// 全局配置仓储
    /// </summary>
    public interface IDsGlobalConfigRepository : IEFCoreRepository<DsGlobalConfig>
    {
        /// <summary>
        /// 分页查询全局配置
        /// </summary>
        /// <param name="pageIndex">页码（从1开始）</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="key">关键字（源数据库名）</param>
        Task<PageList<DsGlobalConfig>> GetPageListAsync(int pageIndex, int pageSize, string key = "");
    }
}
