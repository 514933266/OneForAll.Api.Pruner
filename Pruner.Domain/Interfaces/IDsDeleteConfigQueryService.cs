using OneForAll.Core;
using Pruner.Domain.Entities;
using System.Threading.Tasks;

namespace Pruner.Domain.Interfaces
{
    /// <summary>
    /// 数据库删除配置查询服务（只读）
    /// </summary>
    public interface IDsDeleteConfigQueryService
    {
        /// <summary>
        /// 分页查询数据库删除配置
        /// </summary>
        /// <param name="pageIndex">页码（从1开始）</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="key">关键字（表名）</param>
        /// <returns>分页列表</returns>
        Task<PageList<DsDeleteConfig>> GetPageAsync(int pageIndex, int pageSize, string key = "");

        /// <summary>
        /// 获取数据库删除配置详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>实体（可能为null）</returns>
        Task<DsDeleteConfig> GetAsync(int id);
    }
}
