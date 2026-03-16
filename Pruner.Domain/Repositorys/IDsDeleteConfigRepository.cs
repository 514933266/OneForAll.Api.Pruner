using OneForAll.EFCore;
using Pruner.Domain.Entities;

namespace Pruner.Domain.Repositorys
{
    /// <summary>
    /// 删除配置仓储
    /// </summary>
    public interface IDsDeleteConfigRepository : IEFCoreRepository<DsDeleteConfig>
    {
    }
}
