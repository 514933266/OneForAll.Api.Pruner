using Microsoft.EntityFrameworkCore;
using OneForAll.EFCore;
using Pruner.Domain.Entities;
using Pruner.Domain.Repositorys;

namespace Pruner.Repository
{
    /// <summary>
    /// 全局配置仓储
    /// </summary>
    public class DsGlobalConfigRepository : Repository<DsGlobalConfig>, IDsGlobalConfigRepository
    {
        public DsGlobalConfigRepository(DbContext context)
            : base(context)
        {
        }
    }
}
