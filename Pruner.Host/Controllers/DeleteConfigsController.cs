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
    /// 数据库数据删除配置管理
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class DeleteConfigsController : Controller
    {
        private readonly IDsDeleteConfigRepository _repository;
        private readonly IMapper _mapper;

        public DeleteConfigsController(IDsDeleteConfigRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
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
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 10;

            return await _repository.GetPageListAsync(pageIndex, pageSize, key);
        }

        /// <summary>
        /// 获取数据库删除配置详情
        /// </summary>
        [HttpGet("{id}")]
        public async Task<DsDeleteConfig> GetAsync(int id)
        {
            return await _repository.FindAsync(id);
        }

        /// <summary>
        /// 新增数据库删除配置
        /// </summary>
        [HttpPost]
        public async Task<BaseMessage> AddAsync([FromBody] DsDeleteConfigForm form)
        {
            var msg = new BaseMessage();
            if (form == null || string.IsNullOrEmpty(form.TableName))
                return msg.Fail(BaseErrType.InvalidParameter, "表名不能为空");

            var entity = _mapper.Map<DsDeleteConfig>(form);
            entity.Id = 0;
            var count = await _repository.AddAsync(entity);
            if (count > 0)
                return BaseMessage.Success("新增成功", entity);

            return msg.Fail("新增失败");
        }

        /// <summary>
        /// 修改数据库删除配置
        /// </summary>
        [HttpPut("{id}")]
        public async Task<BaseMessage> UpdateAsync(int id, [FromBody] DsDeleteConfigForm form)
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
        /// 启用/禁用数据库删除配置
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
        /// 删除数据库删除配置
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
