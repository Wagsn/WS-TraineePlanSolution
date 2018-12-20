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
        /// <param name="userId"></param>
        /// <returns></returns>
        IQueryable<Role> FindByUserId(string userId);
    }
}
