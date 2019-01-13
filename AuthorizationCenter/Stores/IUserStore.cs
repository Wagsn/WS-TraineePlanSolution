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
        /// <param name="id">被删除用户ID</param>
        /// <returns></returns>
        Task DeleteByUserId(string userId, string id);
    }
}
