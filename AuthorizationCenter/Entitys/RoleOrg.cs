using AuthorizationCenter.Define;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Entitys
{
    /// <summary>
    /// 角色组织关联
    /// </summary>
    public class RoleOrg
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        [StringLength(36, MinimumLength = 36)]
        [RegularExpression(Constants.GUID_REG, ErrorMessage = Constants.GUID_ERR)]
        public string Id { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public string RoleId { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public Role Role { get; set; }

        /// <summary>
        /// 组织ID
        /// </summary>
        public string OrgId { get; set; }

        /// <summary>
        /// 组织
        /// </summary>
        public Organization Org { get; set; }
    }
}
