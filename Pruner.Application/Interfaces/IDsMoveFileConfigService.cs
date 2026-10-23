using OneForAll.Core;
using Pruner.Domain.Entities;
using Pruner.Domain.Models;
using System.Threading.Tasks;

namespace Pruner.Application.Interfaces
{
    /// <summary>
    /// 文件迁移配置
    /// </summary>
    public interface IDsMoveFileConfigService
    {
        /// <summary>
        /// 分页查询
        /// </summary>
        Task<PageList<DsMoveFileConfig>> GetPageAsync(int pageIndex, int pageSize, string key);

        /// <summary>
        /// 查询详情
        /// </summary>
        Task<DsMoveFileConfig> GetAsync(int id);

        /// <summary>
        /// 新增
        /// </summary>
        Task<BaseMessage> AddAsync(DsMoveFileConfigForm form);

        /// <summary>
        /// 修改
        /// </summary>
        Task<BaseMessage> UpdateAsync(int id, DsMoveFileConfigForm form);

        /// <summary>
        /// 启用/禁用
        /// </summary>
        Task<BaseMessage> UpdateEnabledAsync(int id, bool isEnabled);

        /// <summary>
        /// 删除
        /// </summary>
        Task<BaseMessage> DeleteAsync(int id);
    }
}
