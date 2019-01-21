using AuthorizationCenter.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Stores
{
    /// <summary>
    /// 用户角色存储
    /// </summary>
    public interface IUserRoleStore : IStore<UserRole>
    {
        /// <summary>
        /// 创建用户角色关联
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="userRole">用户角色</param>
        /// <returns></returns>
        Task Create(string userId, UserRole userRole);

        /// <summary>
        /// 用户添加用户角色关联
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="uId"></param>
        /// <param name="rId"></param>
        /// <returns></returns>
        Task CreateByUserId(string userId, string uId, string rId);

        /// <summary>
        /// 用户删除用户角色关联
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="urId"></param>
        /// <returns></returns>
        Task DeleteByUserId(string userId, string urId);

        /// <summary>
        /// 用户删除用户角色关联
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="uId"></param>
        /// <param name="rId"></param>
        /// <returns></returns>
        Task DeleteByUserId(string userId, string uId, string rId);

        /// <summary>
        /// 用户条件删除用户角色关联
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task DeleteByUserId(string userId, Func<UserRole, bool> predicate);
    }
}
