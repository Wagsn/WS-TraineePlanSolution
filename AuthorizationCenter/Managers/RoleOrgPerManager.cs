using AuthorizationCenter.Define;
using AuthorizationCenter.Dto.Jsons;
using AuthorizationCenter.Entitys;
using AuthorizationCenter.Stores;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Managers
{
    /// <summary>
    /// 角色组织权限管理实现
    /// </summary>
    public class RoleOrgPerManager : IRoleOrgPerManager<RoleOrgPerJson>
    {

        /// <summary>
        /// 用户角色存储
        /// </summary>
        protected IUserRoleStore UserRoleStore { get; set; }

        /// <summary>
        /// 角色组织权限关联存储
        /// </summary>
        protected IRoleOrgPerStore RoleOrgPerStore { get; set; }

        /// <summary>
        /// 组织存储
        /// </summary>
        protected IOrganizationStore OrganizationStore { get; set; }

        /// <summary>
        /// 通过ID删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IQueryable<RoleOrgPerJson> DeleteById(string id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 通过ID查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IQueryable<RoleOrgPerJson> FindById(string id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 通过角色ID查询
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public IQueryable<RoleOrgPerJson> FindByRoleId(string roleId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 通过用户ID查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IQueryable<RoleOrgPerJson> FindByUserId(string id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 是否有权限 用户在某个组织下是否具有某项权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="orgId">操作组织ID-前端传入、表示数据范围</param>
        /// <param name="perName">权限ID</param>
        /// <returns></returns>
        public async Task<bool> HasPermission(string userId, string orgId, string perName)
        {
            // 1. 通过用户ID和权限名查询有权组织ID集合
            var perOrgIds = (await RoleOrgPerStore.FindOrgByUserIdPerName(userId, perName)).Select(org => org.Id);
            // 4. 判断传入的组织ID在这些权限组织ID集合中
            return perOrgIds.Contains(orgId);
        }

        /// <summary>
        /// 是否有权限 用户是否在这些组织具有某项权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="orgIds">操作组织集合-前端传入、表示数据范围</param>
        /// <param name="perId">权限ID</param>
        /// <returns></returns>
        public async Task<bool> HasPermission(string userId, List<string> orgIds, string perId)
        {
            // 0.参数检查
            if (userId == null)
            {
                throw new ArgumentNullException(nameof(userId));
            }
            if (orgIds == null)
            {
                throw new ArgumentNullException(nameof(orgIds));
            }
            // 2. 通过角色ID集合和权限ID查询组织ID集合
            var rootOrgIds = await (from rop in RoleOrgPerStore.Context.Set<RoleOrgPer>()
                                    where (from ur in UserRoleStore.Context.Set<UserRole>()  // 1. 通过用户ID查询角色ID集合
                                           where ur.UserId == userId
                                           select ur.RoleId).Contains(rop.RoleId) && rop.PerId == perId
                                    select rop.Id).ToListAsync();
            // 3. 通过找到的组织ID集合递归查询所有子组织ID集合构成权限组织ID集合
            var perOrgIds = new List<string>();
            foreach(var orgId in rootOrgIds)
            {
                // 递归
                perOrgIds.AddRange((await OrganizationStore.FindChildrenById(orgId)).Select(org => org.Id));
            }
            perOrgIds.AddRange(rootOrgIds);
            // 4. 判断传入的组织ID列表是有权限组织ID列表的子集
            return perOrgIds.ContainsAll(orgIds);
        }

        // 根据用户名和权限名查询权限组织

    }
}
