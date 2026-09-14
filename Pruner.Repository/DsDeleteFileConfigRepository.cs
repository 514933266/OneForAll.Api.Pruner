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
    /// 文件删除配置仓储
    /// </summary>
    public class DsDeleteFileConfigRepository : Repository<DsDeleteFileConfig>, IDsDeleteFileConfigRepository
    {
        public DsDeleteFileConfigRepository(DbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// 分页查询文件删除配置
        /// </summary>
        public async Task<PageList<DsDeleteFileConfig>> GetPageListAsync(int pageIndex, int pageSize, string key = "")
        {
            var query = DbSet.AsQueryable();
            if (!string.IsNullOrWhiteSpace(key))
                query = query.Where(e => e.Path.Contains(key) || e.ClientCode.Contains(key));

            var total = await query.CountAsync();
            // 按客户端代码排序，同客户端内新配置在前
            var list = await query
                .OrderBy(e => e.ClientCode)
                .ThenByDescending(e => e.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PageList<DsDeleteFileConfig>(total, pageIndex, pageSize, list);
        }
    }
}
