using OneForAll.Core;
using Pruner.Domain.Entities;
using Pruner.Domain.Models;
using System.Threading.Tasks;

namespace Pruner.Application.Interfaces
{
    /// <summary>
    /// 数据库数据删除配置
    /// </summary>
    public interface IDsDeleteConfigService
    {
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pageIndex">页码（从1开始）</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="key">关键字（表名）</param>
        /// <returns>分页列表</returns>
        Task<PageList<DsDeleteConfig>> GetPageAsync(int pageIndex, int pageSize, string key);

        /// <summary>
        /// 查询详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>实体</returns>
        Task<DsDeleteConfig> GetAsync(int id);

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="form">表单</param>
        /// <returns>结果消息</returns>
        Task<BaseMessage> AddAsync(DsDeleteConfigForm form);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="form">表单</param>
        /// <returns>结果消息</returns>
        Task<BaseMessage> UpdateAsync(int id, DsDeleteConfigForm form);

        /// <summary>
        /// 启用/禁用
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="isEnabled">是否启用</param>
        /// <returns>结果消息</returns>
        Task<BaseMessage> UpdateEnabledAsync(int id, bool isEnabled);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>结果消息</returns>
        Task<BaseMessage> DeleteAsync(int id);
    }
}
