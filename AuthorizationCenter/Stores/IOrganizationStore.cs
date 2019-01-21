using AuthorizationCenter.Entitys;
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
        /// 用户(userId)创建组织(organization)
        /// 添加一个组织会在组织扩展表中添加数据
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="organization">组织</param>
        /// <returns></returns>
        Task CreateByUserId(string userId, Organization organization);

        /// <summary>
        /// 用户(userId)更新组织(organization)
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="organization">组织</param>
        /// <returns></returns>
        Task UpdateByUserId(string userId, Organization organization);

        /// <summary>
        /// 用户(userId)删除组织(orgId)
        /// 删除关联表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="orgId">组织ID</param>
        /// <returns></returns>
        Task DeleteByUserId(string userId, string orgId);

        /// <summary>
        /// [组织扩展表] 删除与组织(orgId)有关的关联
        /// </summary>
        /// <param name="orgId">组织ID</param>
        /// <returns></returns>
        Task DeleteRelById(string orgId);

        /// <summary>
        /// 删除递归删除组织，先删叶子组织
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="orgId">组织ID</param>
        /// <returns></returns>
        Task DeleteRecursionByUserId(string userId, string orgId);

        /// <summary>
        /// 通过组织ID找到所有子组织（包含自身）
        /// </summary>
        /// <param name="orgId">组织ID</param>
        /// <returns></returns>
        Task<IEnumerable<Organization>> FindChildrenById(string orgId);

        /// <summary>
        /// 查询组织ID(orgId)下的所有子组织（包括自身）
        /// </summary>
        /// <param name="orgId">组织ID</param>
        /// <returns></returns>
        Task<List<Organization>> FindChildrenFromOrgById(string orgId);

        /// <summary>
        /// 通过组织ID找到所有子组织（包括间接子组织，包括自身）
        /// </summary>
        /// <param name="orgId">组织ID</param>
        /// <returns></returns>
        IQueryable<Organization> FindChildrenFromOrgRelById(string orgId);

        /// <summary>
        /// [组织关系表] 找到组织ID集合的所有子组织（包括自身）
        /// </summary>
        /// <param name="orgIds">组织ID集合</param>
        /// <returns></returns>
        IQueryable<Organization> FindChildrenFromOrgRelById(List<string> orgIds);

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
        /// 查询组织(orgId)的所有父组织包含自身
        /// </summary>
        /// <param name="orgId">组织ID</param>
        /// <returns></returns>
        IQueryable<Organization> FindParentFromRelById(string orgId);

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

        ///// <summary>
        ///// 用户(userId)查询包含权限(perName)的组织
        ///// </summary>
        ///// <param name="userId">用户ID</param>
        ///// <param name="perName">权限名</param>
        ///// <returns></returns>
        //Task<IEnumerable<Organization>> FindPerOrgByUserIdPerName(string userId, string perName);
    }
}
