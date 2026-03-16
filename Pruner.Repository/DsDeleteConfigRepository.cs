using Microsoft.EntityFrameworkCore;
using OneForAll.EFCore;
using Pruner.Domain.Entities;
using Pruner.Domain.Repositorys;

namespace Pruner.Repository
{
    /// <summary>
    /// 删除配置仓储
    /// </summary>
    public class DsDeleteConfigRepository : Repository<DsDeleteConfig>, IDsDeleteConfigRepository
    {
        public DsDeleteConfigRepository(DbContext context)
            : base(context)
        {
        }
    }
}
