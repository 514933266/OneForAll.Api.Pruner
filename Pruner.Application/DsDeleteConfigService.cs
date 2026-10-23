using AutoMapper;
using OneForAll.Core;
using Pruner.Application.Interfaces;
using Pruner.Domain.Entities;
using Pruner.Domain.Interfaces;
using Pruner.Domain.Models;
using System.Threading.Tasks;

namespace Pruner.Application
{
    /// <summary>
    /// 数据库数据删除配置
    /// </summary>
    public class DsDeleteConfigService : IDsDeleteConfigService
    {
        private readonly IDsDeleteConfigQueryService _queryService;
        private readonly IDsDeleteConfigManager _manager;
        private readonly IMapper _mapper;

        public DsDeleteConfigService(
            IDsDeleteConfigQueryService queryService,
            IDsDeleteConfigManager manager,
            IMapper mapper)
        {
            _queryService = queryService;
            _manager = manager;
            _mapper = mapper;
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        public async Task<PageList<DsDeleteConfig>> GetPageAsync(int pageIndex, int pageSize, string key)
        {
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 10;

            return await _queryService.GetPageAsync(pageIndex, pageSize, key);
        }

        /// <summary>
        /// 查询详情
        /// </summary>
        public async Task<DsDeleteConfig> GetAsync(int id)
        {
            return await _queryService.GetAsync(id);
        }

        /// <summary>
        /// 新增
        /// </summary>
        public async Task<BaseMessage> AddAsync(DsDeleteConfigForm form)
        {
            var msg = new BaseMessage();
            if (form == null || string.IsNullOrEmpty(form.TableName))
                return msg.Fail(BaseErrType.InvalidParameter, "表名不能为空");

            var entity = _mapper.Map<DsDeleteConfig>(form);
            entity.Id = 0;
            var result = await _manager.AddAsync(entity);
            if (result == BaseErrType.Success)
                return BaseMessage.Success("新增成功", entity);

            return ResultMessage.ToFailMessage(result);
        }

        /// <summary>
        /// 修改
        /// </summary>
        public async Task<BaseMessage> UpdateAsync(int id, DsDeleteConfigForm form)
        {
            var msg = new BaseMessage();
            if (form == null || form.Id != id)
                return msg.Fail(BaseErrType.InvalidParameter, "参数无效");

            var exists = await _queryService.GetAsync(id);
            if (exists == null)
                return msg.Fail(BaseErrType.DataNotFound, "配置不存在");

            _mapper.Map(form, exists);
            var result = await _manager.UpdateAsync(exists);
            if (result == BaseErrType.Success)
                return BaseMessage.Success("修改成功", exists);

            return ResultMessage.ToFailMessage(result);
        }

        /// <summary>
        /// 启用/禁用
        /// </summary>
        public async Task<BaseMessage> UpdateEnabledAsync(int id, bool isEnabled)
        {
            var msg = new BaseMessage();
            var exists = await _queryService.GetAsync(id);
            if (exists == null)
                return msg.Fail(BaseErrType.DataNotFound, "配置不存在");

            exists.IsEnabled = isEnabled;
            var result = await _manager.UpdateAsync(exists);
            if (result == BaseErrType.Success)
                return msg.Success(isEnabled ? "已启用" : "已禁用");

            return ResultMessage.ToFailMessage(result);
        }

        /// <summary>
        /// 删除
        /// </summary>
        public async Task<BaseMessage> DeleteAsync(int id)
        {
            var msg = new BaseMessage();
            var entity = await _queryService.GetAsync(id);
            if (entity == null)
                return msg.Fail(BaseErrType.DataNotFound, "配置不存在");

            var result = await _manager.DeleteAsync(id);
            if (result == BaseErrType.Success)
                return msg.Success("删除成功");

            return ResultMessage.ToFailMessage(result);
        }
    }
}
