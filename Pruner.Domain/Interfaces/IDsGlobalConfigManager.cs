using OneForAll.Core;
using Pruner.Domain.Entities;
using System.Threading.Tasks;

namespace Pruner.Domain.Interfaces
{
    /// <summary>
    /// 全局配置命令服务
    /// </summary>
    public interface IDsGlobalConfigManager
    {
        /// <summary>
        /// 新增全局配置
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>结果类型</returns>
        Task<BaseErrType> AddAsync(DsGlobalConfig entity);

        /// <summary>
        /// 保存已修改的全局配置（存在性由编排层校验）
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>结果类型</returns>
        Task<BaseErrType> UpdateAsync(DsGlobalConfig entity);

        /// <summary>
        /// 启用/禁用全局配置
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="isEnabled">是否启用</param>
        /// <returns>结果类型</returns>
        Task<BaseErrType> UpdateEnabledAsync(int id, bool isEnabled);

        /// <summary>
        /// 删除全局配置
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>结果类型</returns>
        Task<BaseErrType> DeleteAsync(int id);
    }
}
