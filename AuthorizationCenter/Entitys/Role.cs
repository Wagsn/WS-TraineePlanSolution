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
    /// 角色实体
    /// </summary>
    public class Role
    {
        /// <summary>
        /// 角色GUID
        /// </summary>
        [Key]
        //[MaxLength(36)]
        [StringLength(36, MinimumLength =36)]
        [RegularExpression(Constants.GUID_REG, ErrorMessage =Constants.GUID_ERR)]
        public string Id { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        //[MaxLength(15)]
        [Required(ErrorMessage ="角色名称不能为空")]
        [StringLength(15, MinimumLength = 2)]
        [RegularExpression(Constants.VISIBLE_REG, ErrorMessage =Constants.VISIBLE_ERR)]
        public string Name { get; set; }

        /// <summary>
        /// 角色描述
        /// </summary>
        [MaxLength(255)]
        public string Decription { get; set; }
        
        /// <summary>
        /// 用户角色
        /// </summary>
        [NotMapped]
        public List<UserRole> UserRoles { get; set; }
    }
}