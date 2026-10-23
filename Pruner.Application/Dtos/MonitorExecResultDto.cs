namespace Pruner.Application.Dtos
{
    /// <summary>
    /// 监控类任务执行结果
    /// </summary>
    public class MonitorExecResultDto
    {
        /// <summary>
        /// 删除/迁移的数据条数（文件类任务以消息为准）
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 执行结果说明
        /// </summary>
        public string Message { get; set; }
    }
}
