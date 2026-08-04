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
    /// 文件迁移配置仓储
    /// </summary>
    public class DsMoveFileConfigRepository : Repository<DsMoveFileConfig>, IDsMoveFileConfigRepository
    {
        public DsMoveFileConfigRepository(DbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// 分页查询文件迁移配置
        /// </summary>
        public async Task<PageList<DsMoveFileConfig>> GetPageListAsync(int pageIndex, int pageSize, string key = "")
        {
            var query = DbSet.AsQueryable();
            if (!string.IsNullOrWhiteSpace(key))
                query = query.Where(e => e.SourcePath.Contains(key) || e.TargetPath.Contains(key) || e.ClientCode.Contains(key));

            var total = await query.CountAsync();
            var list = await query
                .OrderByDescending(e => e.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PageList<DsMoveFileConfig>(total, pageIndex, pageSize, list);
        }
    }
}
