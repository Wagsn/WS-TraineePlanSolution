using AuthorizationCenter.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WS.Log;

namespace AuthorizationCenter.Stores
{
    /// <summary>
    /// 角色组织存储
    /// </summary>
    public class RoleOrgStore: StoreBase<RoleOrg>,  IRoleOrgStore
    {
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="context"></param>
        public RoleOrgStore(ApplicationDbContext context):base(context){ }
    }
}
