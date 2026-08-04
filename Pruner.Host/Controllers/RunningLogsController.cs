using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneForAll.Core;
using Pruner.Domain.Entities;
using Pruner.Domain.Repositorys;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Pruner.Host.Controllers
{
    /// <summary>
    /// 运行日志管理
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class RunningLogsController : Controller
    {
        private readonly IDsRunningLogRepository _repository;

        public RunningLogsController(IDsRunningLogRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// 分页查询运行日志
        /// </summary>
        /// <param name="pageIndex">页码（从1开始）</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="typeName">日志类型</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="key">关键字（表名/描述）</param>
        [HttpGet]
        public async Task<PageList<DsRunningLog>> GetPageAsync(
            int pageIndex = 1,
            int pageSize = 10,
            string typeName = "",
            DateTime? startTime = null,
            DateTime? endTime = null,
            string key = "")
        {
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 10;

            return await _repository.GetPageListAsync(pageIndex, pageSize, typeName, key, startTime, endTime);
        }

        /// <summary>
        /// 删除指定运行日志
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<BaseMessage> DeleteAsync(long id)
        {
            var msg = new BaseMessage();
            var entity = await _repository.FindAsync(id);
            if (entity == null)
                return msg.Fail(BaseErrType.DataNotFound, "日志不存在");

            var count = await _repository.DeleteAsync(entity);
            if (count > 0)
                return msg.Success("删除成功");

            return msg.Fail("删除失败");
        }

        /// <summary>
        /// 清空运行日志
        /// </summary>
        [HttpDelete]
        public async Task<BaseMessage> ClearAsync()
        {
            var msg = new BaseMessage();
            var list = (await _repository.GetListAsync()).ToList();
            if (!list.Any())
                return msg.Success("日志已为空");

            var count = await _repository.DeleteRangeAsync(list);
            if (count > 0)
                return msg.Success($"已清空{count}条日志");

            return msg.Fail("清空失败");
        }
    }
}
