using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneForAll.Core;
using Pruner.Domain.Entities;
using Pruner.Domain.Models;
using Pruner.Domain.Repositorys;
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
        private readonly IDsMoveFileConfigRepository _repository;
        private readonly IMapper _mapper;

        public MoveFileConfigsController(IDsMoveFileConfigRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
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
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 10;

            return await _repository.GetPageListAsync(pageIndex, pageSize, key);
        }

        /// <summary>
        /// 获取文件迁移配置详情
        /// </summary>
        [HttpGet("{id}")]
        public async Task<DsMoveFileConfig> GetAsync(int id)
        {
            return await _repository.FindAsync(id);
        }

        /// <summary>
        /// 新增文件迁移配置
        /// </summary>
        [HttpPost]
        public async Task<BaseMessage> AddAsync([FromBody] DsMoveFileConfigForm form)
        {
            var msg = new BaseMessage();
            if (form == null || string.IsNullOrEmpty(form.SourcePath))
                return msg.Fail(BaseErrType.InvalidParameter, "源目录路径不能为空");

            var entity = _mapper.Map<DsMoveFileConfig>(form);
            entity.Id = 0;
            var count = await _repository.AddAsync(entity);
            if (count > 0)
                return BaseMessage.Success("新增成功", entity);

            return msg.Fail("新增失败");
        }

        /// <summary>
        /// 修改文件迁移配置
        /// </summary>
        [HttpPut("{id}")]
        public async Task<BaseMessage> UpdateAsync(int id, [FromBody] DsMoveFileConfigForm form)
        {
            var msg = new BaseMessage();
            if (form == null || form.Id != id)
                return msg.Fail(BaseErrType.InvalidParameter, "参数无效");

            var exists = await _repository.FindAsync(id);
            if (exists == null)
                return msg.Fail(BaseErrType.DataNotFound, "配置不存在");

            _mapper.Map(form, exists);
            var count = await _repository.UpdateAsync(exists);
            if (count > 0)
                return BaseMessage.Success("修改成功", exists);

            return msg.Fail("修改失败");
        }

        /// <summary>
        /// 启用/禁用文件迁移配置
        /// </summary>
        [HttpPut("{id}/Enabled/{isEnabled}")]
        public async Task<BaseMessage> UpdateEnabledAsync(int id, bool isEnabled)
        {
            var msg = new BaseMessage();
            var exists = await _repository.FindAsync(id);
            if (exists == null)
                return msg.Fail(BaseErrType.DataNotFound, "配置不存在");

            exists.IsEnabled = isEnabled;
            var count = await _repository.UpdateAsync(exists);
            if (count > 0)
                return msg.Success(isEnabled ? "已启用" : "已禁用");

            return msg.Fail("操作失败");
        }

        /// <summary>
        /// 删除文件迁移配置
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<BaseMessage> DeleteAsync(int id)
        {
            var msg = new BaseMessage();
            var entity = await _repository.FindAsync(id);
            if (entity == null)
                return msg.Fail(BaseErrType.DataNotFound, "配置不存在");

            var count = await _repository.DeleteAsync(entity);
            if (count > 0)
                return msg.Success("删除成功");

            return msg.Fail("删除失败");
        }
    }
}
