using Microsoft.EntityFrameworkCore;
using OneForAll.EFCore;
using Pruner.Domain.Entities;
using Pruner.Domain.Repositorys;

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
    }
}
