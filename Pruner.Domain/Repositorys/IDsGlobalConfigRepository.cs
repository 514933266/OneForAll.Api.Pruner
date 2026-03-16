using OneForAll.EFCore;
using Pruner.Domain.Entities;

namespace Pruner.Domain.Repositorys
{
    /// <summary>
    /// 全局配置仓储
    /// </summary>
    public interface IDsGlobalConfigRepository : IEFCoreRepository<DsGlobalConfig>
    {
    }
}
