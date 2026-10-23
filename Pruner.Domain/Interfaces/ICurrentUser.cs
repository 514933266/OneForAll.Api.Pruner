using OneForAll.Core.OAuth;

namespace Pruner.Domain.Interfaces
{
    /// <summary>
    /// 当前请求上下文端口：领域层通过该端口获取登录态，不直接依赖 HttpContext
    /// </summary>
    public interface ICurrentUser
    {
        /// <summary>
        /// 当前登录用户信息
        /// </summary>
        LoginUser LoginUser { get; }
    }
}
