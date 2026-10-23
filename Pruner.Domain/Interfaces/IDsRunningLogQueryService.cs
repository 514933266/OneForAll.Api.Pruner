using OneForAll.Core;
using Pruner.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Pruner.Domain.Interfaces
{
    /// <summary>
    /// 运行日志查询服务（只读）
    /// </summary>
    public interface IDsRunningLogQueryService
    {
        /// <summary>
        /// 分页查询运行日志
        /// </summary>
        /// <param name="pageIndex">页码（从1开始）</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="typeName">日志类型</param>
        /// <param name="key">关键字（表名/描述）</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>分页列表</returns>
        Task<PageList<DsRunningLog>> GetPageAsync(int pageIndex, int pageSize, string typeName = "", string key = "", DateTime? startTime = null, DateTime? endTime = null);

        /// <summary>
        /// 获取运行日志详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>实体（可能为null）</returns>
        Task<DsRunningLog> GetAsync(long id);
    }
}
