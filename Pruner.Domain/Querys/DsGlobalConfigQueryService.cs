using OneForAll.Core;
using Pruner.Domain.Entities;
using Pruner.Domain.Interfaces;
using Pruner.Domain.Repositorys;
using System.Threading.Tasks;

namespace Pruner.Domain.Querys
{
    /// <summary>
    /// 全局配置查询服务（只读，返回实体，不含DTO）
    /// </summary>
    public class DsGlobalConfigQueryService : IDsGlobalConfigQueryService
    {
        private readonly IDsGlobalConfigRepository _repository;

        public DsGlobalConfigQueryService(IDsGlobalConfigRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// 分页查询全局配置
        /// </summary>
        public async Task<PageList<DsGlobalConfig>> GetPageAsync(int pageIndex, int pageSize, string key = "")
        {
            return await _repository.GetPageListAsync(pageIndex, pageSize, key);
        }

        /// <summary>
        /// 获取全局配置详情
        /// </summary>
        public async Task<DsGlobalConfig> GetAsync(int id)
        {
            return await _repository.FindAsync(id);
        }
    }
}
