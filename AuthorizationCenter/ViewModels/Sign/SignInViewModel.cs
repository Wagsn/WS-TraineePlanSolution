using AuthorizationCenter.Define;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.ViewModels.Sign
{
    /// <summary>
    /// 登陆模型 
    /// </summary>
    public class SignInViewModel
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Key]
        [MaxLength(36)]
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
        [Required(ErrorMessage = "密码不能为空")]
        [MaxLength(63)]
        [RegularExpression(Constants.PASSWORD_REG, ErrorMessage = Constants.PASSWORD_ERR)]
        public string PassWord { get; set; }
    }
}
