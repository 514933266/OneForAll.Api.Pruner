using System;
using System.ComponentModel.DataAnnotations;

namespace Pruner.Domain.Models
{
    /// <summary>
    /// 数据库删除配置表单（新增/修改）
    /// </summary>
    public class DsDeleteConfigForm
    {
        /// <summary>
        /// 主键（修改时必填）
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 全局配置id
        /// </summary>
        [Required]
        public int GlobalConfigId { get; set; }

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
        [StringLength(500)]
        public string CustonWhere { get; set; } = "";

        /// <summary>
        /// 数据保留Id（删除到该Id时将停止）
        /// </summary>
        public long KeepId { get; set; }

        /// <summary>
        /// 每次最大删除数量
        /// </summary>
        [Required]
        public int MaxDelCount { get; set; }

        /// <summary>
        /// 延迟执行时间
        /// </summary>
        public DateTime? WaitingTime { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; }
    }
}
