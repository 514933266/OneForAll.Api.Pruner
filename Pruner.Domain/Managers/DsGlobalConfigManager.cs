using OneForAll.Core;
using Pruner.Domain.Entities;
using Pruner.Domain.Interfaces;
using Pruner.Domain.Repositorys;
using System.Threading.Tasks;

namespace Pruner.Domain.Managers
{
    /// <summary>
    /// 全局配置（命令）
    /// </summary>
    public class DsGlobalConfigManager : PrunerDomainManager, IDsGlobalConfigManager
    {
        private readonly IDsGlobalConfigRepository _repository;

        public DsGlobalConfigManager(
            ICurrentUser currentUser,
            IDsGlobalConfigRepository repository) : base(currentUser)
        {
            _repository = repository;
        }

        /// <summary>
        /// 新增全局配置
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>结果类型</returns>
        public async Task<BaseErrType> AddAsync(DsGlobalConfig entity)
        {
            if (entity == null || string.IsNullOrWhiteSpace(entity.SourceDbName))
                return BaseErrType.InvalidParameter;

            return await ResultAsync(() => _repository.AddAsync(entity));
        }

        /// <summary>
        /// 保存已修改的全局配置（存在性由编排层校验）
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>结果类型</returns>
        public async Task<BaseErrType> UpdateAsync(DsGlobalConfig entity)
        {
            return await ResultAsync(() => _repository.UpdateAsync(entity));
        }

        /// <summary>
        /// 启用/禁用全局配置
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="isEnabled">是否启用</param>
        /// <returns>结果类型</returns>
        public async Task<BaseErrType> UpdateEnabledAsync(int id, bool isEnabled)
        {
            var entity = await _repository.FindAsync(id);
            if (entity == null)
                return BaseErrType.DataNotFound;

            entity.IsEnabled = isEnabled;
            return await ResultAsync(() => _repository.UpdateAsync(entity));
        }

        /// <summary>
        /// 删除全局配置
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>结果类型</returns>
        public async Task<BaseErrType> DeleteAsync(int id)
        {
            var entity = await _repository.FindAsync(id);
            if (entity == null)
                return BaseErrType.DataNotFound;

            return await ResultAsync(() => _repository.DeleteAsync(entity));
        }
    }
}
