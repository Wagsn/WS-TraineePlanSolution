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
    /// 用户组织关联表
    /// </summary>
    public class UserOrg
    {
        /// <summary>
        /// 关联ID
        /// </summary>
        [Key]
        [StringLength(36, MinimumLength = 36)]
        [RegularExpression(Constants.GUID_REG, ErrorMessage = Constants.GUID_ERR)]
        public string Id { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        [ForeignKey("UserId")]
        public User User { get; set; }

        /// <summary>
        /// 组织ID
        /// </summary>
        public string OrgId { get; set; }

        /// <summary>
        /// 组织
        /// </summary>
        [ForeignKey("OrgId")]
        public Organization Org { get; set; }

        /// <summary>
        /// 关系类型 -所有、从属、管理、访客 
        /// </summary>
        [StringLength(31, MinimumLength =3, ErrorMessage ="类型格式错误，长度在3-15之间")]
        public string Type { get; set; }
    }
}
