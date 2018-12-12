using AuthorizationCenter.Define;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Dto.Jsons
{
    /// <summary>
    /// 用户基础信息 -- UserBase
    /// </summary>
    public class UserBaseJson
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
        [MaxLength(15)]
        [RegularExpression(Constants.SIGNNAME_REG, ErrorMessage = Constants.SIGNNAME_ERR)]
        public string SignName { get; set; }

        /// <summary>
        /// 用户密码
        /// </summary>
        [MaxLength(63)]
        [RegularExpression(Constants.PASSWORD_REG, ErrorMessage = Constants.PASSWORD_ERR)]
        public string PassWord { get; set; }
    }
}
