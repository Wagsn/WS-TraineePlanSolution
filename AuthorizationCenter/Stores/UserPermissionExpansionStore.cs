using AuthorizationCenter.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Stores
{
    /// <summary>
    /// 用户权限存储
    /// </summary>
    public class UserPermissionExpansionStore : StoreBase<UserPermissionExpansion>, IUserPermissionExpansionStore
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public UserPermissionExpansionStore(ApplicationDbContext context) : base(context){}

        /// <summary>
        /// 添加用户组织权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="expansions">用户组织权限</param>
        /// <returns></returns>
        public async Task Create(string userId, IEnumerable<UserPermissionExpansion> expansions)
        {
            try
            {
                Context.AddRange(expansions);
                await Context.SaveChangesAsync();
            }
            catch(Exception e)
            {
                Logger.Error($"[{nameof(Create)}] 用户({userId})添加失败:\r\n{e}");
                throw new Exception($" 用户({userId})添加用户组织权限失败", e);
            }
        }
    }
}
