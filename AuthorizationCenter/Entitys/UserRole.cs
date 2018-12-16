using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Entitys
{
    /// <summary>
    /// 用户与角色关联
    /// </summary>
    public class UserRole
    {
        /// <summary>
        /// 关联ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public string RoleId { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        [NotMapped]
        public UserBase User { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        [NotMapped]
        public Role Role { get; set; }
    }
}
