using OneForAll.EFCore;
using Pruner.Domain.Entities;

namespace Pruner.Domain.Repositorys
{
    /// <summary>
    /// 文件删除配置仓储
    /// </summary>
    public interface IDsDeleteFileConfigRepository : IEFCoreRepository<DsDeleteFileConfig>
    {
    }
}
