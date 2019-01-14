using AuthorizationCenter.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WS.Log;

namespace AuthorizationCenter.Stores
{
    /// <summary>
    /// 用户组织存储
    /// </summary>
    public class UserOrgStore: StoreBase<UserOrg>, IUserOrgStore
    {
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="context"></param>
        public UserOrgStore(ApplicationDbContext context):base(context){}

    }
}
