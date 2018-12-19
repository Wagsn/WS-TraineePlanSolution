using AuthorizationCenter.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WS.Log;

namespace AuthorizationCenter.Stores
{
    /// <summary>
    /// 用户角色存储
    /// </summary>
    public class UserRoleStore : StoreBase<UserRole>, IUserRoleStore
    {
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="context"></param>
        public UserRoleStore(ApplicationDbContext context)
        {
            Logger = LoggerManager.GetLogger(GetType().Name);
            Context = context;
        }
    }
}
