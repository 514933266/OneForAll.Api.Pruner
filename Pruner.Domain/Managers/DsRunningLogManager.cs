using OneForAll.Core;
using Pruner.Domain.Entities;
using Pruner.Domain.Interfaces;
using Pruner.Domain.Repositorys;
using System.Linq;
using System.Threading.Tasks;

namespace Pruner.Domain.Managers
{
    /// <summary>
    /// 运行日志（命令）
    /// </summary>
    public class DsRunningLogManager : PrunerDomainManager, IDsRunningLogManager
    {
        private readonly IDsRunningLogRepository _repository;

        public DsRunningLogManager(
            ICurrentUser currentUser,
            IDsRunningLogRepository repository) : base(currentUser)
        {
            _repository = repository;
        }

        /// <summary>
        /// 删除指定运行日志
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>结果类型</returns>
        public async Task<BaseErrType> DeleteAsync(long id)
        {
            var entity = await _repository.FindAsync(id);
            if (entity == null)
                return BaseErrType.DataNotFound;

            return await ResultAsync(() => _repository.DeleteAsync(entity));
        }

        /// <summary>
        /// 清空运行日志
        /// </summary>
        /// <returns>结果类型与清空条数（Count=0 表示本就为空或删除失败）</returns>
        public async Task<(BaseErrType Result, int Count)> ClearAsync()
        {
            var list = (await _repository.GetListAsync()).ToList();
            if (!list.Any())
                return (BaseErrType.Success, 0);

            var count = await _repository.DeleteRangeAsync(list);
            return count > 0 ? (BaseErrType.Success, count) : (BaseErrType.Fail, 0);
        }
    }
}
