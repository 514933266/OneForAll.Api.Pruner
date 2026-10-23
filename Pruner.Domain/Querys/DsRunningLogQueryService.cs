using OneForAll.Core;
using Pruner.Domain.Entities;
using Pruner.Domain.Interfaces;
using Pruner.Domain.Repositorys;
using System;
using System.Threading.Tasks;

namespace Pruner.Domain.Querys
{
    /// <summary>
    /// 运行日志查询服务（只读，返回实体，不含DTO）
    /// </summary>
    public class DsRunningLogQueryService : IDsRunningLogQueryService
    {
        private readonly IDsRunningLogRepository _repository;

        public DsRunningLogQueryService(IDsRunningLogRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// 分页查询运行日志
        /// </summary>
        public async Task<PageList<DsRunningLog>> GetPageAsync(int pageIndex, int pageSize, string typeName = "", string key = "", DateTime? startTime = null, DateTime? endTime = null)
        {
            return await _repository.GetPageListAsync(pageIndex, pageSize, typeName, key, startTime, endTime);
        }

        /// <summary>
        /// 获取运行日志详情
        /// </summary>
        public async Task<DsRunningLog> GetAsync(long id)
        {
            return await _repository.FindAsync(id);
        }
    }
}
