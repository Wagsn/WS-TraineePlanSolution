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
        /// 是否有权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="orgId">操作组织ID-前端传入、表示数据范围</param>
        /// <param name="perId">权限ID</param>
        /// <returns></returns>
        public async Task<bool> HasPermission(string userId, string orgId, string perId)
        {
            // 2. 通过角色ID集合和权限ID查询组织ID集合
            var orgIds = await (from rop in RoleOrgPerStore.Context.Set<RoleOrgPer>()
                         where (from ur in UserRoleStore.Context.Set<UserRole>()  // 1. 通过用户ID查询角色ID集合
                                where ur.UserId == userId
                                select ur.RoleId).Contains(rop.RoleId) && rop.PerId == perId
                         select rop.Id).ToListAsync();
            // 3. 找到操作组织ID的所有父组织
            var parents = (await OrganizationStore.FindParentById(orgId)).Select(org => org.Id);
            // 4. 判断这些父组织是否在查询出来的组织ID集合中存在??
            foreach (var id in parents)
            {
                if (orgIds.Contains(id))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 是否有权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="orgIds2">操作组织集合-前端传入、表示数据范围</param>
        /// <param name="perId">权限ID</param>
        /// <returns></returns>
        public async Task<bool> HasPermission(string userId, List<string> orgIds2, string perId)
        {
            // 2. 通过角色ID集合和权限ID查询组织ID集合
            var orgIds = await (from rop in RoleOrgPerStore.Context.Set<RoleOrgPer>()
                                where (from ur in UserRoleStore.Context.Set<UserRole>()  // 1. 通过用户ID查询角色ID集合
                                       where ur.UserId == userId
                                       select ur.RoleId).Contains(rop.RoleId) && rop.PerId == perId
                                select rop.Id).ToListAsync();
            // 3. 找到所有有权限的组织ID集合
            var perOrgIds = new List<string>();
            // 4. 判断传入的组织ID列表是有权限组织ID列表的子集
            //if(perOrgIds.ContainsAll())
            if (orgIds2.Count <= 0)
            {
                return false;
            }
            foreach(var id in orgIds2)
            {
                if (!perOrgIds.Contains(id))
                {
                    return false;
                }
            }
            return true;
            //// 3. 找到操作组织ID的所有父组织
            //List<string> parents = new List<string>();
            //foreach(var id in orgIds2)
            //{
            //    parents.AddRange((await OrganizationStore.FindParentById(id)).Select(org => org.Id));
            //}
            //// 4. 判断这些父组织是否在查询出来的组织ID集合中存在??
            //foreach (var id in parents)
            //{
            //    if (orgIds.Contains(id))
            //    {
            //        return true;
            //    }
            //}
            //return false;
            //throw new NotImplementedException();
        }

    }
}
