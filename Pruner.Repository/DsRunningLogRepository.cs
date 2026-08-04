using Microsoft.EntityFrameworkCore;
using OneForAll.Core;
using OneForAll.EFCore;
using Pruner.Domain.Entities;
using Pruner.Domain.Repositorys;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Pruner.Repository
{
    /// <summary>
    /// 运行日志仓储
    /// </summary>
    public class DsRunningLogRepository : Repository<DsRunningLog>, IDsRunningLogRepository
    {
        public DsRunningLogRepository(DbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// 分页查询运行日志
        /// </summary>
        public async Task<PageList<DsRunningLog>> GetPageListAsync(int pageIndex, int pageSize, string typeName = "", string key = "", DateTime? startTime = null, DateTime? endTime = null)
        {
            var query = DbSet.AsQueryable();
            if (!string.IsNullOrWhiteSpace(typeName))
                query = query.Where(e => e.TypeName.Contains(typeName));
            if (!string.IsNullOrWhiteSpace(key))
                query = query.Where(e => e.TableName.Contains(key) || e.Description.Contains(key));
            if (startTime.HasValue)
                query = query.Where(e => e.CreateTime >= startTime.Value);
            if (endTime.HasValue)
                query = query.Where(e => e.CreateTime <= endTime.Value);

            var total = await query.CountAsync();
            var list = await query
                .OrderByDescending(e => e.CreateTime)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PageList<DsRunningLog>(total, pageIndex, pageSize, list);
        }
    }
}
