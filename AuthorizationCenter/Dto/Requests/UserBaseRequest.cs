using AuthorizationCenter.Dto.Jsons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Dto.Requests
{
    /// <summary>
    /// 用户请求
    /// </summary>
    public class UserBaseRequest
    {
        /// <summary>
        /// 用户基础信息
        /// </summary>
        public UserJson User { get; set; }
    }
}
