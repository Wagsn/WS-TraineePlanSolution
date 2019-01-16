using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Dto.Jsons
{
    /// <summary>
    /// 登陆用户信息
    /// </summary>
    public class SignUserInfo
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 组织ID -用户登陆到哪个组织的
        /// </summary>
        public string OrgId { get; set; }

        /// <summary>
        /// 角色ID -用户以什么角色登陆的
        /// </summary>
        public string RoleId { get; set; }
    }
}
