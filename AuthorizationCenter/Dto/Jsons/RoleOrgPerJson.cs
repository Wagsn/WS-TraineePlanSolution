using AuthorizationCenter.Define;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Dto.Jsons
{
    /// <summary>
    /// 角色组织权限关联
    /// </summary>
    public class RoleOrgPerJson
    {
        /// <summary>
        /// 关联ID - 便于修改
        /// </summary>
        [StringLength(36, MinimumLength = 36)]
        [RegularExpression(Constants.GUID_REG, ErrorMessage = Constants.GUID_ERR)]
        public string Id { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string RoleId { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        public RoleJson Role { get; set; }

        /// <summary>
        /// 组织ID
        /// </summary>
        public string OrgId { get; set; }

        /// <summary>
        /// 组织
        /// </summary>
        public OrganizationJson Org { get; set; }

        /// <summary>
        /// 权限ID
        /// </summary>
        public string PerId { get; set; }

        /// <summary>
        /// 权限
        /// </summary>
        public PermissionJson Per { get; set; }
    }
}
