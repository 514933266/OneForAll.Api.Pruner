using OneForAll.Core;
using OneForAll.EFCore;
using Pruner.Domain.Entities;
using System.Threading.Tasks;

namespace Pruner.Domain.Repositorys
{
    /// <summary>
    /// 文件迁移配置仓储
    /// </summary>
    public interface IDsMoveFileConfigRepository : IEFCoreRepository<DsMoveFileConfig>
    {
        /// <summary>
        /// 分页查询文件迁移配置
        /// </summary>
        /// <param name="pageIndex">页码（从1开始）</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="key">关键字（源/目标目录路径、客户端代码）</param>
        /// <returns>分页结果</returns>
        Task<PageList<DsMoveFileConfig>> GetPageListAsync(int pageIndex, int pageSize, string key = "");
    }
}
