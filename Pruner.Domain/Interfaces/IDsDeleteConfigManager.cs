using OneForAll.Core;
using Pruner.Domain.Entities;
using System.Threading.Tasks;

namespace Pruner.Domain.Interfaces
{
    /// <summary>
    /// 数据库删除配置命令服务
    /// </summary>
    public interface IDsDeleteConfigManager
    {
        /// <summary>
        /// 新增删除配置（校验表名必填并落库）
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>结果类型</returns>
        Task<BaseErrType> AddAsync(DsDeleteConfig entity);

        /// <summary>
        /// 保存已修改的删除配置（存在性由编排层校验）
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>结果类型</returns>
        Task<BaseErrType> UpdateAsync(DsDeleteConfig entity);

        /// <summary>
        /// 启用/禁用删除配置
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="isEnabled">是否启用</param>
        /// <returns>结果类型</returns>
        Task<BaseErrType> UpdateEnabledAsync(int id, bool isEnabled);

        /// <summary>
        /// 删除配置
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>结果类型</returns>
        Task<BaseErrType> DeleteAsync(int id);
    }
}
