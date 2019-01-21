using AuthorizationCenter.Entitys;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AuthorizationCenter.Stores
{
    /// <summary>
    /// 用户存储
    /// </summary>
    public interface IUserStore : INameStore<User>
    {
        /// <summary>
        /// 通过组织ID查询
        /// </summary>
        /// <param name="orgId">组织ID</param>
        /// <returns></returns>
        IQueryable<User> FindByOrgId(string orgId);

        /// <summary>
        /// 查询所有组织下的用户
        /// </summary>
        /// <param name="orgIds">组织ID集合</param>
        /// <returns></returns>
        IQueryable<User> FindByOrgId(IEnumerable<string> orgIds);

        ///// <summary>
        ///// 查询所有符合条件(predicate)的组织下的用户
        ///// </summary>
        ///// <param name="predicate">条件</param>
        ///// <returns></returns>
        //IQueryable<User> FindByOrg(Func<Organization, bool> predicate);

        /// <summary>
        /// 用户在其组织下创建用户
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="user">用户</param>
        /// <returns></returns>
        Task<User> CreateForOrgByUserId(string userId, User user);


        /// <summary>
        /// 删除通过用户ID
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="uId">被删除用户ID</param>
        /// <returns></returns>
        Task DeleteByUserId(string userId, string uId);

        /// <summary>
        /// 删除通过用户ID
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="uIds">被删除用户ID</param>
        /// <returns></returns>
        Task DeleteByUserId(string userId, IEnumerable<string> uIds);

        /// <summary>
        /// 删除符合条件的组织的所有用户
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="predicate">条件表达式</param>
        /// <returns></returns>
        Task DeleteByUserIdOrgId(string userId, Func<Organization, bool> predicate);
    }
}
