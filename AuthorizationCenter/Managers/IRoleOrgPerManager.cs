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
        Task<bool> HasPermission(string userId, string orgId, string perName);

        /// <summary>
        /// 某用户在某组织下是否具有某项权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="perName">权限名</param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> HasPermissionForUser(string userId, string perName, string id);

        /// <summary>
        /// 某用户在其组织下是否具有某项权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="perName">权限名</param>
        /// <returns></returns>
        Task<bool> HasPermission(string userId, string perName);

        /// <summary>
        /// 查询有权组织
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
