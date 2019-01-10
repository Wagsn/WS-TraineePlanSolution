using System;
using System.ComponentModel.DataAnnotations;

namespace MVCDemo.Entitys
{
    public class Student
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Key] // 表示主键
        public string Id { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [StringLength(63, MinimumLength =0)]  // 长度范围 最大长度映射到数据库的字段长度
        public string Password { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        [StringLength(63, MinimumLength = 0)]  // 长度范围 最大长度映射到数据库的字段长度
        public string Name { get; set; }

        /// <summary>
        /// 邮箱地址
        /// </summary>
        [StringLength(63, MinimumLength = 0)]  // 长度范围 最大长度映射到数据库的字段长度
        public string Email { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [StringLength(63, MinimumLength = 0)]  // 长度范围 最大长度映射到数据库的字段长度
        public string MobileNumber { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(63, MinimumLength = 0)]  // 长度范围 最大长度映射到数据库的字段长度
        public string Remarks { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public Guid CreateUserId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 上次登录时间
        /// </summary>
        public DateTime LastLoginTime { get; set; }

        /// <summary>
        /// 登录次数
        /// </summary>
        public int LoginTimes { get; set; }
    }
}
