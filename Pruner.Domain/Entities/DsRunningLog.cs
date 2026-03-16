using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pruner.Domain.Entities
{
    /// <summary>
    /// 运行日志
    /// </summary>
    public class DsRunningLog
    {
        [Key]
        [Required]
        public long Id { get; set; }

        /// <summary>
        /// 日志类型
        /// </summary>
        [Required]
        [Column(TypeName = "varchar(100)")]
        public string TypeName { get; set; }

        /// <summary>
        /// 日志类型
        /// </summary>
        [Required]
        [Column(TypeName = "varchar(100)")]
        public string TableName { get; set; }


        /// <summary>
        /// 描述
        /// </summary>
        [Required]
        [Column(TypeName = "varchar(1000)")]
        public string Description { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Required]
        [Column(TypeName = "datetime")]
        public DateTime CreateTime { get; set; } = DateTime.UtcNow;
    }
}