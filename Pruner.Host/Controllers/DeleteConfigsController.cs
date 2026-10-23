using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneForAll.Core;
using Pruner.Application.Interfaces;
using Pruner.Domain.Entities;
using Pruner.Domain.Models;
using System.Threading.Tasks;

namespace Pruner.Host.Controllers
{
    /// <summary>
    /// 数据库数据删除配置管理
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class DeleteConfigsController : Controller
    {
        private readonly IDsDeleteConfigService _service;

        public DeleteConfigsController(IDsDeleteConfigService service)
        {
            _service = service;
        }

        /// <summary>
        /// 分页查询数据库删除配置
        /// </summary>
        /// <param name="pageIndex">页码（从1开始）</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="key">关键字（表名）</param>
        [HttpGet]
        public async Task<PageList<DsDeleteConfig>> GetPageAsync(int pageIndex = 1, int pageSize = 10, string key = "")
        {
            return await _service.GetPageAsync(pageIndex, pageSize, key);
        }

        /// <summary>
        /// 获取数据库删除配置详情
        /// </summary>
        [HttpGet("{id}")]
        public async Task<DsDeleteConfig> GetAsync(int id)
        {
            return await _service.GetAsync(id);
        }

        /// <summary>
        /// 新增数据库删除配置
        /// </summary>
        [HttpPost]
        public async Task<BaseMessage> AddAsync([FromBody] DsDeleteConfigForm form)
        {
            return await _service.AddAsync(form);
        }

        /// <summary>
        /// 修改数据库删除配置
        /// </summary>
        [HttpPut("{id}")]
        public async Task<BaseMessage> UpdateAsync(int id, [FromBody] DsDeleteConfigForm form)
        {
            return await _service.UpdateAsync(id, form);
        }

        /// <summary>
        /// 启用/禁用数据库删除配置
        /// </summary>
        [HttpPut("{id}/Enabled/{isEnabled}")]
        public async Task<BaseMessage> UpdateEnabledAsync(int id, bool isEnabled)
        {
            return await _service.UpdateEnabledAsync(id, isEnabled);
        }

        /// <summary>
        /// 删除数据库删除配置
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<BaseMessage> DeleteAsync(int id)
        {
            return await _service.DeleteAsync(id);
        }
    }
}
