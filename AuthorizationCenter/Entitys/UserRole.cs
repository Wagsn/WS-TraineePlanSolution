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
    /// 用户与角色关联
    /// </summary>
    public class UserRole
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
        [ForeignKey("UserId")]
        public string UserId { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        [ForeignKey("RoleId")]
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
