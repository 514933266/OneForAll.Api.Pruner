using Microsoft.EntityFrameworkCore;
using OneForAll.EFCore;
using Pruner.Domain.Entities;
using Pruner.Domain.Repositorys;

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
    }
}
