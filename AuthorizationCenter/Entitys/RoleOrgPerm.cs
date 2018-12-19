using AuthorizationCenter.Define;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Entitys
{
    /// <summary>
    /// 角色组织权限关联
    /// </summary>
    public class RoleOrgPerm
    {
        /// <summary>
        /// 关联ID - 便于修改
        /// </summary>
        [Key]
        //[MaxLength(36)]
        [StringLength(36, MinimumLength = 36)]
        [RegularExpression(Constants.GUID_REG, ErrorMessage = Constants.GUID_ERR)]
        public string Id { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [ForeignKey("UserId")]
        public string UserId { get; set; }

        //public

        /// <summary>
        /// 组织ID
        /// </summary>
        [ForeignKey("OrgId")]
        public string OrgId { get; set; }

        /// <summary>
        /// 权限ID
        /// </summary>
        [ForeignKey("PemId")]
        public string PemId { get; set; }
    }
}
