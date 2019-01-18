using AuthorizationCenter.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Stores
{
    /// <summary>
    /// 用户权限扩展
    /// </summary>
    public interface IUserPermissionExpansionStore: IStore<UserPermissionExpansion>
    {
        /// <summary>
        /// 添加用户组织权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="expansions">用户组织权限</param>
        /// <returns></returns>
        Task Create(string userId, IEnumerable<UserPermissionExpansion> expansions);
    }
}
