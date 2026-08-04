using OneForAll.Core;
using OneForAll.EFCore;
using Pruner.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Pruner.Domain.Repositorys
{
    /// <summary>
    /// 运行日志仓储
    /// </summary>
    public interface IDsRunningLogRepository : IEFCoreRepository<DsRunningLog>
    {
        /// <summary>
        /// 分页查询运行日志
        /// </summary>
        /// <param name="pageIndex">页码（从1开始）</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="typeName">日志类型关键字</param>
        /// <param name="key">关键字（表名/描述）</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>分页结果</returns>
        Task<PageList<DsRunningLog>> GetPageListAsync(int pageIndex, int pageSize, string typeName = "", string key = "", DateTime? startTime = null, DateTime? endTime = null);
    }
}
