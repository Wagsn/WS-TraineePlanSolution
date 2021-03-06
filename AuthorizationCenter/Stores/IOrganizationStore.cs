﻿using AuthorizationCenter.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Stores
{
    /// <summary>
    /// 组织存储
    /// </summary>
    public interface IOrganizationStore : INameStore<Organization>
    {

        /// <summary>
        /// 通过组织ID找到所有子组织（包括间接子组织，但是不包括自身）
        /// </summary>
        /// <param name="orgId">组织ID</param>
        /// <returns></returns>
        Task<List<Organization>> FindChildrenById(string orgId);

        /// <summary>
        /// 通过组织找到所有子组织（不包含自身）
        /// </summary>
        /// <param name="organization">组织</param>
        /// <returns></returns>
        Task<List<Organization>> FindChildren(Organization organization);

        /// <summary>
        /// 查询所有父组织
        /// </summary>
        /// <param name="orgId">组织ID</param>
        /// <returns></returns>
        Task<List<Organization>> FindParentById(string orgId);

        /// <summary>
        /// 递归查询所有节点，构成一棵树返回
        /// </summary>
        /// <param name="orgId">组织</param>
        /// <returns></returns>
        Organization FindTreeById(string orgId);

        /// <summary>
        /// 查询通过用户ID在UserOrg表中
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        IQueryable<Organization> FindByUserId(string userId);

        /// <summary>
        /// 查询资源所在组织
        /// </summary>
        /// <typeparam name="TResource">资源类型</typeparam>
        /// <param name="userId">用户ID</param>
        /// <param name="resourceId">资源ID</param>
        /// <returns></returns>
        Task<IEnumerable<Organization>> FindByUserIdSrcId<TResource>(string userId, string resourceId) where TResource : class;
    }
}
