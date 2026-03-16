using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pruner.Domain.Entities
{
    /// <summary>
    /// 全局配置
    /// </summary>
    public class DsGlobalConfig
    {
        [Key]
        [Required]
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
        [Required]
        public bool IsEnabledDel { get; set; }

        /// <summary>
        /// 是否启用同步服务
        /// </summary>
        [Required]
        public bool IsEnabledSync { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        [Required]
        public bool IsEnabled { get; set; }
    }
}
