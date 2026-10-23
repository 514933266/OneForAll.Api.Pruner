using OneForAll.Core;
using Pruner.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Pruner.Application.Interfaces
{
    /// <summary>
    /// 运行日志
    /// </summary>
    public interface IDsRunningLogService
    {
        /// <summary>
        /// 分页查询运行日志
        /// </summary>
        Task<PageList<DsRunningLog>> GetPageAsync(
            int pageIndex,
            int pageSize,
            string typeName,
            string key,
            DateTime? startTime,
            DateTime? endTime);

        /// <summary>
        /// 查询详情
        /// </summary>
        Task<DsRunningLog> GetAsync(long id);

        /// <summary>
        /// 删除指定日志
        /// </summary>
        Task<BaseMessage> DeleteAsync(long id);

        /// <summary>
        /// 清空日志
        /// </summary>
        Task<BaseMessage> ClearAsync();
    }
}
