using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneForAll.Core;
using Pruner.Application.Interfaces;
using Pruner.Domain.Entities;
using System;
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
        private readonly IDsRunningLogService _service;

        public RunningLogsController(IDsRunningLogService service)
        {
            _service = service;
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
            return await _service.GetPageAsync(pageIndex, pageSize, typeName, key, startTime, endTime);
        }

        /// <summary>
        /// 删除指定运行日志
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<BaseMessage> DeleteAsync(long id)
        {
            return await _service.DeleteAsync(id);
        }

        /// <summary>
        /// 清空运行日志
        /// </summary>
        [HttpDelete]
        public async Task<BaseMessage> ClearAsync()
        {
            return await _service.ClearAsync();
        }
    }
}
