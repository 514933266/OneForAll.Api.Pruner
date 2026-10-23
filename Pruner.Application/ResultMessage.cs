using OneForAll.Core;

namespace Pruner.Application
{
    /// <summary>
    /// 领域结果类型 → 接口消息的通用转换
    /// </summary>
    internal static class ResultMessage
    {
        /// <summary>
        /// 将命令层结果类型转换为失败消息
        /// </summary>
        /// <param name="result">结果类型</param>
        /// <returns>失败消息</returns>
        public static BaseMessage ToFailMessage(BaseErrType result)
        {
            var msg = new BaseMessage();
            if (result == BaseErrType.InvalidParameter)
                return msg.Fail(BaseErrType.InvalidParameter, "参数无效");
            if (result == BaseErrType.DataNotFound)
                return msg.Fail(BaseErrType.DataNotFound, "数据不存在");
            return msg.Fail("操作失败");
        }
    }
}
