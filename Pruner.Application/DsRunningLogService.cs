using OneForAll.Core;
using Pruner.Application.Interfaces;
using Pruner.Domain.Entities;
using Pruner.Domain.Interfaces;
using System;
using System.Threading.Tasks;

namespace Pruner.Application
{
    /// <summary>
    /// 运行日志
    /// </summary>
    public class DsRunningLogService : IDsRunningLogService
    {
        private readonly IDsRunningLogQueryService _queryService;
        private readonly IDsRunningLogManager _manager;

        public DsRunningLogService(
            IDsRunningLogQueryService queryService,
            IDsRunningLogManager manager)
        {
            _queryService = queryService;
            _manager = manager;
        }

        /// <summary>
        /// 分页查询运行日志
        /// </summary>
        public async Task<PageList<DsRunningLog>> GetPageAsync(
            int pageIndex,
            int pageSize,
            string typeName,
            string key,
            DateTime? startTime,
            DateTime? endTime)
        {
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 10;

            return await _queryService.GetPageAsync(pageIndex, pageSize, typeName, key, startTime, endTime);
        }

        /// <summary>
        /// 查询详情
        /// </summary>
        public async Task<DsRunningLog> GetAsync(long id)
        {
            return await _queryService.GetAsync(id);
        }

        /// <summary>
        /// 删除指定日志
        /// </summary>
        public async Task<BaseMessage> DeleteAsync(long id)
        {
            var msg = new BaseMessage();
            var entity = await _queryService.GetAsync(id);
            if (entity == null)
                return msg.Fail(BaseErrType.DataNotFound, "日志不存在");

            var result = await _manager.DeleteAsync(id);
            if (result == BaseErrType.Success)
                return msg.Success("删除成功");

            return result == BaseErrType.DataNotFound
                ? msg.Fail(BaseErrType.DataNotFound, "日志不存在")
                : msg.Fail("删除失败");
        }

        /// <summary>
        /// 清空日志
        /// </summary>
        public async Task<BaseMessage> ClearAsync()
        {
            var msg = new BaseMessage();
            var (result, count) = await _manager.ClearAsync();
            if (result == BaseErrType.Success)
                return count > 0
                    ? msg.Success($"已清空{count}条日志")
                    : msg.Success("日志已为空");

            return msg.Fail("清空失败");
        }
    }
}
