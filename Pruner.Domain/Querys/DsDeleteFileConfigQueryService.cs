using OneForAll.Core;
using Pruner.Domain.Entities;
using Pruner.Domain.Interfaces;
using Pruner.Domain.Repositorys;
using System.Threading.Tasks;

namespace Pruner.Domain.Querys
{
    /// <summary>
    /// 文件删除配置查询服务（只读，返回实体，不含DTO）
    /// </summary>
    public class DsDeleteFileConfigQueryService : IDsDeleteFileConfigQueryService
    {
        private readonly IDsDeleteFileConfigRepository _repository;

        public DsDeleteFileConfigQueryService(IDsDeleteFileConfigRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// 分页查询文件删除配置
        /// </summary>
        public async Task<PageList<DsDeleteFileConfig>> GetPageAsync(int pageIndex, int pageSize, string key = "")
        {
            return await _repository.GetPageListAsync(pageIndex, pageSize, key);
        }

        /// <summary>
        /// 获取文件删除配置详情
        /// </summary>
        public async Task<DsDeleteFileConfig> GetAsync(int id)
        {
            return await _repository.FindAsync(id);
        }
    }
}
