using System.ComponentModel.DataAnnotations;

namespace Pruner.Domain.Entities
{
    /// <summary>
    /// 文件删除配置
    /// </summary>
    public class DsDeleteFileConfig
    {
        [Key]
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// 客户端代码（用于区分不同节点）
        /// </summary>
        [Required]
        [StringLength(200)]
        public string ClientCode { get; set; }

        /// <summary>
        /// 目录路径
        /// </summary>
        [Required]
        [StringLength(500)]
        public string Path { get; set; }

        /// <summary>
        /// 文件保留天数
        /// </summary>
        [Required]
        public int KeepDays { get; set; } = 30;

        /// <summary>
        /// 每次最大删除数量（0表示不限制）
        /// </summary>
        [Required]
        public int MaxDeleteCount { get; set; }

        /// <summary>
        /// 是否递归子目录
        /// </summary>
        [Required]
        public bool IncludeSubDirectories { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        [Required]
        public bool IsEnabled { get; set; }
    }
}
