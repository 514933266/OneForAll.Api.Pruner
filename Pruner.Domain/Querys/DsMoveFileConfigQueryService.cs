using OneForAll.Core;
using Pruner.Domain.Entities;
using Pruner.Domain.Interfaces;
using Pruner.Domain.Repositorys;
using System.Threading.Tasks;

namespace Pruner.Domain.Querys
{
    /// <summary>
    /// 文件迁移配置查询服务（只读，返回实体，不含DTO）
    /// </summary>
    public class DsMoveFileConfigQueryService : IDsMoveFileConfigQueryService
    {
        private readonly IDsMoveFileConfigRepository _repository;

        public DsMoveFileConfigQueryService(IDsMoveFileConfigRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// 分页查询文件迁移配置
        /// </summary>
        public async Task<PageList<DsMoveFileConfig>> GetPageAsync(int pageIndex, int pageSize, string key = "")
        {
            return await _repository.GetPageListAsync(pageIndex, pageSize, key);
        }

        /// <summary>
        /// 获取文件迁移配置详情
        /// </summary>
        public async Task<DsMoveFileConfig> GetAsync(int id)
        {
            return await _repository.FindAsync(id);
        }
    }
}
