using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Entitys
{
    /// <summary>
    /// 用户角色组织权限扩展表
    /// 扩展表不包含业务逻辑，主要用于查询
    /// </summary>
    public class UserPermissionExpansion
    {
        /// <summary>
        /// 关系主键
        /// </summary>
        [Key]
        [StringLength(36)]
        public string Id { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        public User User { get; set; }

        ///// <summary>
        ///// 角色ID
        ///// </summary>
        //public string RoleId { get; set; }

        ///// <summary>
        ///// 角色
        ///// </summary>
        //public Role Role { get; set; }

        /// <summary>
        /// 组织ID
        /// </summary>
        public string OrganizationId { get; set; }

        /// <summary>
        /// 组织
        /// </summary>
        public Organization Organization { get; set; }

        /// <summary>
        /// 权限ID
        /// </summary>
        public string PermissionId { get; set; }

        /// <summary>
        /// 权限
        /// </summary>
        public Permission Permission { get; set; }
    }
}
