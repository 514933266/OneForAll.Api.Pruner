using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pruner.Domain.Entities
{
    /// <summary>
    /// 删除配置
    /// </summary>
    public class DsDeleteConfig
    {
        [Key]
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// 全局配置id
        /// </summary>
        [Required]
        public int GlobalConfigId { get; set; }

        /// <summary>
        /// 数据库名称（取自所属全局配置，非持久化字段）
        /// </summary>
        [NotMapped]
        public string SourceDbName { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        [Required]
        [StringLength(200)]
        public string TableName { get; set; }

        /// <summary>
        /// 数据保留天数
        /// </summary>
        [Required]
        public int KeepDays { get; set; } = 120;

        /// <summary>
        /// 日期比较字段
        /// </summary>
        [Required]
        [StringLength(100)]
        public string DateFieldName { get; set; } = "";

        /// <summary>
        /// 自定义比较条件
        /// </summary>
        [Required]
        [StringLength(500)]
        public string CustonWhere { get; set; } = "";

        /// <summary>
        /// 数据保留Id（删除到该Id时将停止）
        /// </summary>
        [Required]
        public long KeepId { get; set; }

        /// <summary>
        /// 每次最大删除数量
        /// </summary>
        [Required]
        public int MaxDelCount { get; set; }

        /// <summary>
        /// 是否启用LastDelId限制（启用时按最后删除Id逐批向后推进删除；停用时每次直接按查询条件删除）
        /// </summary>
        [Required]
        public bool IsLastDelIdEnabled { get; set; } = true;

        /// <summary>
        /// 最后删除的数据id
        /// </summary>
        [Required]
        public long LastDelId { get; set; }

        /// <summary>
        /// 最后一次删除时间
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? LastDelTime { get; set; }

        /// <summary>
        /// 延迟执行时间
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? WaitingTime { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        [Required]
        public bool IsEnabled { get; set; }
    }
}
