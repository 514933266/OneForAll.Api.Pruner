using Microsoft.EntityFrameworkCore;
using OneForAll.Core;
using OneForAll.EFCore;
using Pruner.Domain.Entities;
using Pruner.Domain.Repositorys;
using System.Linq;
using System.Threading.Tasks;

namespace Pruner.Repository
{
    /// <summary>
    /// 删除配置仓储
    /// </summary>
    public class DsDeleteConfigRepository : Repository<DsDeleteConfig>, IDsDeleteConfigRepository
    {
        private readonly DbContext _dbContext;

        public DsDeleteConfigRepository(DbContext context)
            : base(context)
        {
            _dbContext = context;
        }

        /// <summary>
        /// 分页查询数据库删除配置
        /// </summary>
        public async Task<PageList<DsDeleteConfig>> GetPageListAsync(int pageIndex, int pageSize, string tableName = "")
        {
            var query = DbSet.AsQueryable();
            if (!string.IsNullOrWhiteSpace(tableName))
                query = query.Where(e => e.TableName.Contains(tableName));

            var total = await query.CountAsync();
            // 按全局配置Id排序，同配置内新配置在前
            var list = await query
                .OrderBy(e => e.GlobalConfigId)
                .ThenByDescending(e => e.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // 填充所属全局配置对应的数据库名称，供列表展示
            // 全局配置表数据量小：直接全量取出后在内存中匹配，避免参数化集合 Contains 在旧版数据库上的语法兼容问题
            var globalConfigs = await _dbContext.Set<DsGlobalConfig>().ToListAsync();
            var dbNames = globalConfigs.ToDictionary(g => g.Id, g => g.SourceDbName);
            list.ForEach(e => e.SourceDbName = dbNames.TryGetValue(e.GlobalConfigId, out var name) ? name : "");

            return new PageList<DsDeleteConfig>(total, pageIndex, pageSize, list);
        }
    }
}
