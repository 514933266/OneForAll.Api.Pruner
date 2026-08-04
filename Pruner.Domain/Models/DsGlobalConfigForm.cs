using System.ComponentModel.DataAnnotations;

namespace Pruner.Domain.Models
{
    /// <summary>
    /// 全局配置表单（新增/修改）
    /// </summary>
    public class DsGlobalConfigForm
    {
        /// <summary>
        /// 主键（修改时必填）
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 源数据库名
        /// </summary>
        [Required]
        [StringLength(200)]
        public string SourceDbName { get; set; } = "";

        /// <summary>
        /// 最大删除数量
        /// </summary>
        [Required]
        public int MaxDelCount { get; set; } = 50;

        /// <summary>
        /// 最大同步数量
        /// </summary>
        [Required]
        public int MaxSyncCount { get; set; } = 50;

        /// <summary>
        /// 源库连接字符串
        /// </summary>
        [Required]
        [StringLength(500)]
        public string SourceConn { get; set; } = "";

        /// <summary>
        /// 目标库连接字符串
        /// </summary>
        [Required]
        [StringLength(500)]
        public string TargetConn { get; set; } = "";

        /// <summary>
        /// 是否启用删除服务
        /// </summary>
        public bool IsEnabledDel { get; set; }

        /// <summary>
        /// 是否启用同步服务
        /// </summary>
        public bool IsEnabledSync { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; }
    }
}
