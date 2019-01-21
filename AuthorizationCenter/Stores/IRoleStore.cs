using AuthorizationCenter.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Stores
{
    /// <summary>
    /// 角色存储
    /// </summary>
    public interface IRoleStore : INameStore<Role>
    {
        /// <summary>
        /// 通过用户ID查询角色
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        IQueryable<Role> FindByUserId(string userId);
        
        /// <summary>
        /// 查询通过组织ID
        /// </summary>
        /// <param name="orgId">组织ID</param>
        /// <returns></returns>
        IQueryable<Role> FindByOrgId(string orgId);

        /// <summary>
        /// 用户(userId)删除角色(id)
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="id">被删除角色ID</param>
        /// <returns></returns>
        Task DeleteByUserId(string userId, string id);

        /// <summary>
        /// 用户(userId)条件(predicate)删除角色
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="predicate">条件表达式</param>
        /// <returns></returns>
        Task DeleteByUserId(string userId, Func<Role, bool> predicate);

        /// <summary>
        /// 用户(userId)删除符合条件(predicate)的组织下的所有角色
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="predicate">条件表达式</param>
        /// <returns></returns>
        Task DeleteByUserIdOrgId(string userId, Func<Organization, bool> predicate);
    }
}
