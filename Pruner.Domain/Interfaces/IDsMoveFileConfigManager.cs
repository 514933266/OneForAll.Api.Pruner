using OneForAll.Core;
using Pruner.Domain.Entities;
using System.Threading.Tasks;

namespace Pruner.Domain.Interfaces
{
    /// <summary>
    /// 文件迁移配置命令服务
    /// </summary>
    public interface IDsMoveFileConfigManager
    {
        /// <summary>
        /// 新增文件迁移配置（校验源目录路径必填并落库）
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>结果类型</returns>
        Task<BaseErrType> AddAsync(DsMoveFileConfig entity);

        /// <summary>
        /// 保存已修改的文件迁移配置（存在性由编排层校验）
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>结果类型</returns>
        Task<BaseErrType> UpdateAsync(DsMoveFileConfig entity);

        /// <summary>
        /// 启用/禁用文件迁移配置
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="isEnabled">是否启用</param>
        /// <returns>结果类型</returns>
        Task<BaseErrType> UpdateEnabledAsync(int id, bool isEnabled);

        /// <summary>
        /// 删除文件迁移配置
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>结果类型</returns>
        Task<BaseErrType> DeleteAsync(int id);
    }
}
