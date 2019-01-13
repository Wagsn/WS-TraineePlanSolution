using AuthorizationCenter.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Managers
{
    /// <summary>
    /// 角色组织权限关联管理
    /// </summary>
    public interface IRoleOrgPerManager
    {
        /// <summary>
        /// 某用户在某组织下是否具有某项权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="orgId">组织ID</param>
        /// <param name="perName">权限名</param>
        /// <returns></returns>
        Task<bool> HasPermission(string userId, string perName, string orgId);

        /// <summary>
        /// 某用户在某组织下是否具有某项权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="perName">权限名</param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> HasPermissionForUser(string userId, string perName, string id);

        /// <summary>
        /// 用户(userId)在自身的组织下是否具有权限(perName)
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="perName">权限名</param>
        /// <returns></returns>
        Task<bool> HasPermissionInSelfOrg(string userId, string perName);

        /// <summary>
        /// 用户(userId)是否具有权限(perName)
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="perName">权限名</param>
        /// <returns></returns>
        Task<bool> HasPermission(string userId, string perName);

        /// <summary>
        /// 判断用户(userId)有没权限(perName)操作资源(resourceId)
        /// 新增的有组织资源类型需要在<see cref="Stores.OrganizationStore.FindByUserIdSrcId{TResource}(string, string)"/>中添加获取组织方法
        /// </summary>
        /// <typeparam name="TResource">资源类型</typeparam>
        /// <param name="userId">用户ID</param>
        /// <param name="perName">权限名称</param>
        /// <param name="resourceId">资源ID</param>
        /// <returns></returns>
        Task<bool> HasPermission<TResource>(string userId, string perName, string resourceId) where TResource : class;

        /// <summary>
        /// 查询用户(userId)有权限(perName)的组织
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="perName">权限名</param>
        /// <returns></returns>
        Task<IEnumerable<Organization>> FindOrgByUserIdPerName(string userId, string perName);

        /// <summary>
        /// 查询角色的权限
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <returns></returns>
        IQueryable<RoleOrgPer> FindByRoleId(string roleId);

        /// <summary>
        /// 查询一个用户的权限
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IQueryable<RoleOrgPer> FindByUserId(string id);

        /// <summary>
        /// 查询通过ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IQueryable<RoleOrgPer> FindById(string id);

        /// <summary>
        /// 删除通过ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IQueryable<RoleOrgPer> DeleteById(string id);
    }
}
