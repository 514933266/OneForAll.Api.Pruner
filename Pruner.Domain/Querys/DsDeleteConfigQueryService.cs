using OneForAll.Core;
using Pruner.Domain.Entities;
using Pruner.Domain.Interfaces;
using Pruner.Domain.Repositorys;
using System.Threading.Tasks;

namespace Pruner.Domain.Querys
{
    /// <summary>
    /// 数据库删除配置查询服务（只读，返回实体，不含DTO）
    /// </summary>
    public class DsDeleteConfigQueryService : IDsDeleteConfigQueryService
    {
        private readonly IDsDeleteConfigRepository _repository;

        public DsDeleteConfigQueryService(IDsDeleteConfigRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// 分页查询数据库删除配置
        /// </summary>
        public async Task<PageList<DsDeleteConfig>> GetPageAsync(int pageIndex, int pageSize, string key = "")
        {
            return await _repository.GetPageListAsync(pageIndex, pageSize, key);
        }

        /// <summary>
        /// 获取数据库删除配置详情
        /// </summary>
        public async Task<DsDeleteConfig> GetAsync(int id)
        {
            return await _repository.FindAsync(id);
        }
    }
}
