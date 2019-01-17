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
    /// 用户实体
    /// </summary>
    public class User
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Key]
        [StringLength(36, MinimumLength = 36)]
        [RegularExpression(Constants.GUID_REG, ErrorMessage = Constants.GUID_ERR)]
        public string Id { get; set; }

        /// <summary>
        /// 用户签名
        /// </summary>
        [Required(ErrorMessage ="用户名不能为空")]
        [MaxLength(15)]
        [RegularExpression(Constants.SIGNNAME_REG, ErrorMessage = Constants.SIGNNAME_ERR)]
        public string SignName { get; set; }

        /// <summary>
        /// 用户密码
        /// </summary>
        [MaxLength(63)]
        [RegularExpression(Constants.PASSWORD_REG, ErrorMessage = Constants.PASSWORD_ERR)]
        public string PassWord { get; set; }

        /// <summary>
        /// 用户角色
        /// </summary>
        [NotMapped]
        public List<UserRole> UserRoles { get; set; }
    }
}
