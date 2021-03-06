﻿using AuthorizationCenter.Entitys;
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
        IEnumerable<Role> FindByOrgId(string orgId);

        /// <summary>
        /// 用户(userId)删除角色(id)
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="id">被删除角色ID</param>
        /// <returns></returns>
        Task DeleteByUserId(string userId, string id);
    }
}
