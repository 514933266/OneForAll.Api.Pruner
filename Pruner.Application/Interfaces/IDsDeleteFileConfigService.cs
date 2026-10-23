using OneForAll.Core;
using Pruner.Domain.Entities;
using Pruner.Domain.Models;
using System.Threading.Tasks;

namespace Pruner.Application.Interfaces
{
    /// <summary>
    /// 文件删除配置
    /// </summary>
    public interface IDsDeleteFileConfigService
    {
        /// <summary>
        /// 分页查询
        /// </summary>
        Task<PageList<DsDeleteFileConfig>> GetPageAsync(int pageIndex, int pageSize, string key);

        /// <summary>
        /// 查询详情
        /// </summary>
        Task<DsDeleteFileConfig> GetAsync(int id);

        /// <summary>
        /// 新增
        /// </summary>
        Task<BaseMessage> AddAsync(DsDeleteFileConfigForm form);

        /// <summary>
        /// 修改
        /// </summary>
        Task<BaseMessage> UpdateAsync(int id, DsDeleteFileConfigForm form);

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
