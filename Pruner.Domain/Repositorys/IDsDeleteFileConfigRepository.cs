using OneForAll.Core;
using OneForAll.EFCore;
using Pruner.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pruner.Domain.Repositorys
{
    /// <summary>
    /// 文件删除配置仓储
    /// </summary>
    public interface IDsDeleteFileConfigRepository : IEFCoreRepository<DsDeleteFileConfig>
    {
        /// <summary>
        /// 分页查询文件删除配置
        /// </summary>
        /// <param name="pageIndex">页码（从1开始）</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="key">关键字（目录路径/客户端代码）</param>
        /// <returns>分页结果</returns>
        Task<PageList<DsDeleteFileConfig>> GetPageListAsync(int pageIndex, int pageSize, string key = "");

        /// <summary>
        /// 获取指定客户端启用的文件删除配置列表
        /// </summary>
        /// <param name="clientCode">客户端代码</param>
        /// <returns>列表</returns>
        Task<IEnumerable<DsDeleteFileConfig>> GetListEnabledByClientAsync(string clientCode);
    }
}
