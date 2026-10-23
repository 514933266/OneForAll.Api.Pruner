using OneForAll.Core;
using OneForAll.EFCore;
using Pruner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pruner.Domain.Repositorys
{
    /// <summary>
    /// 删除配置仓储
    /// </summary>
    public interface IDsDeleteConfigRepository : IEFCoreRepository<DsDeleteConfig>
    {
        /// <summary>
        /// 分页查询数据库删除配置
        /// </summary>
        /// <param name="pageIndex">页码（从1开始）</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="tableName">表名关键字</param>
        /// <returns>分页结果</returns>
        Task<PageList<DsDeleteConfig>> GetPageListAsync(int pageIndex, int pageSize, string tableName = "");

        /// <summary>
        /// 获取已启用且等待时间已到（或无等待时间）的删除配置列表
        /// </summary>
        /// <param name="maxWaitingDate">等待时间上限</param>
        /// <returns>列表</returns>
        Task<IEnumerable<DsDeleteConfig>> GetListReadyAsync(DateTime maxWaitingDate);
    }
}
