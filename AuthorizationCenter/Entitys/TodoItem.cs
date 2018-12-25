using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Entitys
{
    /// <summary>
    /// 待办项
    /// </summary>
    public class TodoItem
    {
        /// <summary>
        /// 待办ID
        /// </summary>
        [Key]
        [MaxLength(36, ErrorMessage = Define.Constants.IdLengthErrMsg)]
        public string Id { get; set; }

        /// <summary>
        /// 待办名
        /// </summary>
        [MaxLength(31, ErrorMessage = "待办名不能超过31个字符")]
        public string Name { get; set; }

        /// <summary>
        /// 待办内容
        /// </summary>
        [MaxLength(255, ErrorMessage = "待办内容不能超过255个字符")]
        public string Content { get; set; }

        /// <summary>
        /// 是否完成
        /// </summary>
        [Required]
        public bool IsComplete { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? BuildTime { get; set; }

        /// <summary>
        /// 预期完成时间
        /// </summary>
        public DateTime? ExpectTime { get; set; }

        /// <summary>
        /// 实际完成时间
        /// </summary>
        public DateTime? ActualTime { get; set; }
    }
}
