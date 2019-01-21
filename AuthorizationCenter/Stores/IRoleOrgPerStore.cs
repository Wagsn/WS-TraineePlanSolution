using AuthorizationCenter.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Stores
{
    /// <summary>
    /// 角色组织权限关联存储接口
    /// </summary>
    public interface IRoleOrgPerStore: IStore<RoleOrgPer>
    {
        /// <summary>
        /// 用户(userId)更新角色组织权限(roleOrgPer)
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="roleOrgPer">角色组织权限</param>
        /// <returns></returns>
        Task UpdateByUserId(string userId, RoleOrgPer roleOrgPer);

        #region << 查询操作 >>

        /// <summary>
        /// 查询用户有权根组织ID集合
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="perName">权限名称</param>
        /// <returns></returns>
        Task<IEnumerable<Organization>> FindOrgByUserIdPerName(string userId, string perName);

        /// <summary>
        /// 查询用户拥有某项权限（用户可能拥有其父级权限）的所有组织
        /// 如果用户拥有的权限是在该操作权限之上 ROOT > USER_MANAGE > USER_QUERY
        /// 有权组织列表获取，通过用户ID和权限名称获取组织列表(U.ID-[UR]->R.ID, P.N-[P]->P.ID-[P]->P.ID)-[ROP]->O.ID-[O]->O.ID
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="perName">权限名称</param>
        /// <returns></returns>
        Task<IEnumerable<Organization>> FindOrgFromURAndROPByUserIdPerName(string userId, string perName);

        /// <summary>
        /// 查询用户拥有某项权限（用户可能拥有其父级权限）的所有组织
        /// 如果用户拥有的权限是在该操作权限之上 ROOT > USER_MANAGE > USER_QUERY
        /// 有权组织列表获取，通过用户ID和权限名称获取组织列表(U.ID,(P.N-[P]->P.ID-[P]->P.ID))-[UOP]->O.ID-[OR]->O.ID
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="perName">权限名称</param>
        /// <returns></returns>
        Task<IEnumerable<Organization>> FindOrgFromUOPByUserIdPerName(string userId, string perName);

        #endregion

        ///// <summary>
        ///// 用户(userId)创建角色组织权限(roleOrgPer)
        ///// </summary>
        ///// <param name="userId">用户ID</param>
        ///// <param name="roleOrgPer">角色组织权限</param>
        ///// <returns></returns>
        //Task CreateByUserId(string userId, RoleOrgPer roleOrgPer);

        /// <summary>
        /// 用户(userId)创建角色(rId)组织(oId)权限(pId)关联
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="rId"></param>
        /// <param name="oId"></param>
        /// <param name="pId"></param>
        /// <returns></returns>
        Task CreateByUserId(string userId, string rId, string oId, string pId);

        /// <summary>
        /// 用户(userId)删除角色组织权限(ropId)
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="ropId">角色组织权限ID</param>
        /// <returns></returns>
        Task DeleteByUserId(string userId, string ropId);

        /// <summary>
        /// 用户(userId)删除角色组织权限(ropId)
        /// 当删除角色时要删除用户角色关联和角色组织关联和角色权限关联（当用户角色关联删除完后角色生成的用户权限也被删除完了）
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task DeleteByUserId(string userId, Func<RoleOrgPer, bool> predicate);

        ///// <summary>
        ///// 用户(userId)删除角色组织权限(ropId)
        ///// </summary>
        ///// <param name="userId">用户ID</param>
        ///// <param name="predicate">角色组织权限ID</param>
        ///// <returns></returns>
        //Task DeleteByUserId(string userId, Func<RoleOrgPer> predicate);

        /// <summary>
        /// 用户组织权限扩展
        /// </summary>
        /// <returns></returns>
        Task ReExpansion();

        ///// <summary>
        ///// 生成用户权限扩展表
        ///// </summary>
        ///// <returns></returns>
        //Task<IEnumerable<UserPermissionExpansion>> GenUserPermissionExpansion();

        ///// <summary>
        ///// 根据角色组织权限(roleOrgPer)生成用户组织权限
        ///// </summary>
        ///// <param name="rId"></param>
        ///// <param name="oId"></param>
        ///// <param name="pId"></param>
        ///// <returns></returns>
        //Task<IEnumerable<UserPermissionExpansion>> GenUserPermissionExpansion(string rId, string oId, string pId);

        ///// <summary>
        ///// 根据用户角色(userRole)生成用户组织权限
        ///// </summary>
        ///// <param name="userRole">用户角色</param>
        ///// <returns></returns>
        //Task<IEnumerable<UserPermissionExpansion>> GenUserPermissionExpansion(UserRole userRole);
    }
}
