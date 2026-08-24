using System.ComponentModel.DataAnnotations;

namespace Pruner.Domain.Models
{
    /// <summary>
    /// 文件迁移配置表单（新增/修改）
    /// </summary>
    public class DsMoveFileConfigForm
    {
        /// <summary>
        /// 主键（修改时必填）
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 客户端代码（用于区分不同节点）
        /// </summary>
        [Required]
        [StringLength(200)]
        public string ClientCode { get; set; }

        /// <summary>
        /// 源目录路径
        /// </summary>
        [Required]
        [StringLength(500)]
        public string SourcePath { get; set; }

        /// <summary>
        /// 目标目录路径
        /// </summary>
        [Required]
        [StringLength(500)]
        public string TargetPath { get; set; }

        /// <summary>
        /// 文件保留天数
        /// </summary>
        [Required]
        public int KeepDays { get; set; } = 30;

        /// <summary>
        /// 每次最大迁移数量（0表示不限制）
        /// </summary>
        [Required]
        public int MaxMoveCount { get; set; }

        /// <summary>
        /// 是否递归迁移（递归迁移时，会按照原文件所在目录层级迁移）
        /// </summary>
        public bool IncludeSubDirectories { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; }
    }
}
