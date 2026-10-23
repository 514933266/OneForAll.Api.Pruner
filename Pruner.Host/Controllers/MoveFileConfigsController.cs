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
    /// 文件迁移配置管理
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class MoveFileConfigsController : Controller
    {
        private readonly IDsMoveFileConfigService _service;

        public MoveFileConfigsController(IDsMoveFileConfigService service)
        {
            _service = service;
        }

        /// <summary>
        /// 分页查询文件迁移配置
        /// </summary>
        /// <param name="pageIndex">页码（从1开始）</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="key">关键字（目录路径）</param>
        [HttpGet]
        public async Task<PageList<DsMoveFileConfig>> GetPageAsync(int pageIndex = 1, int pageSize = 10, string key = "")
        {
            return await _service.GetPageAsync(pageIndex, pageSize, key);
        }

        /// <summary>
        /// 获取文件迁移配置详情
        /// </summary>
        [HttpGet("{id}")]
        public async Task<DsMoveFileConfig> GetAsync(int id)
        {
            return await _service.GetAsync(id);
        }

        /// <summary>
        /// 新增文件迁移配置
        /// </summary>
        [HttpPost]
        public async Task<BaseMessage> AddAsync([FromBody] DsMoveFileConfigForm form)
        {
            return await _service.AddAsync(form);
        }

        /// <summary>
        /// 修改文件迁移配置
        /// </summary>
        [HttpPut("{id}")]
        public async Task<BaseMessage> UpdateAsync(int id, [FromBody] DsMoveFileConfigForm form)
        {
            return await _service.UpdateAsync(id, form);
        }

        /// <summary>
        /// 启用/禁用文件迁移配置
        /// </summary>
        [HttpPut("{id}/Enabled/{isEnabled}")]
        public async Task<BaseMessage> UpdateEnabledAsync(int id, bool isEnabled)
        {
            return await _service.UpdateEnabledAsync(id, isEnabled);
        }

        /// <summary>
        /// 删除文件迁移配置
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<BaseMessage> DeleteAsync(int id)
        {
            return await _service.DeleteAsync(id);
        }
    }
}
