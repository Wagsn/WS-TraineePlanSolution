using AuthorizationCenter.Define;
using AuthorizationCenter.Dto.Jsons;
using AuthorizationCenter.Entitys;
using AuthorizationCenter.Stores;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WS.Log;

namespace AuthorizationCenter.Managers
{
    /// <summary>
    /// 角色组织权限管理实现
    /// </summary>
    public class RoleOrgPerManager : IRoleOrgPerManager
    {

        /// <summary>
        /// 用户角色存储
        /// </summary>
        IUserRoleStore UserRoleStore { get; set; }

        /// <summary>
        /// 角色组织权限关联存储
        /// </summary>
        IRoleOrgPerStore RoleOrgPerStore { get; set; }

        /// <summary>
        /// 组织存储
        /// </summary>
        IOrganizationStore OrganizationStore { get; set; }

        /// <summary>
        /// 日志器
        /// </summary>
        readonly ILogger Logger = LoggerManager.GetLogger<RoleOrgPerManager>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userRoleStore"></param>
        /// <param name="roleOrgPerStore"></param>
        /// <param name="organizationStore"></param>
        public RoleOrgPerManager(IUserRoleStore userRoleStore, IRoleOrgPerStore roleOrgPerStore, IOrganizationStore organizationStore)
        {
            UserRoleStore = userRoleStore ?? throw new ArgumentNullException(nameof(userRoleStore));
            RoleOrgPerStore = roleOrgPerStore ?? throw new ArgumentNullException(nameof(roleOrgPerStore));
            OrganizationStore = organizationStore ?? throw new ArgumentNullException(nameof(organizationStore));
        }
        /// <summary>
        /// 通过ID删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IQueryable<RoleOrgPer> DeleteById(string id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 用户(userId)条件(predicate)删除角色权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="predicate">条件</param>
        /// <returns></returns>
        public async Task DeleteByUserId(string userId, Func<RoleOrgPer, bool> predicate)
        {
            Logger.Trace($"[{nameof(DeleteByUserId)}] 用户{userId}条件删除角色权限");
            var roleOrgPers = await RoleOrgPerStore.Find(predicate).AsNoTracking().ToListAsync();
            foreach(var rop in roleOrgPers)
            {
                await RoleOrgPerStore.DeleteByUserId(userId, rop.Id);
            }
        }

        /// <summary>
        /// 通过ID查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IQueryable<RoleOrgPer> FindById(string id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 通过角色ID查询
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public IQueryable<RoleOrgPer> FindByRoleId(string roleId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 查询用户(userId)的角色组织权限列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public IQueryable<RoleOrgPer> FindByUserId(string userId)
        {
            return from rop in RoleOrgPerStore.Context.Set<RoleOrgPer>()
                   where (from ur in RoleOrgPerStore.Context.Set<UserRole>()
                          where ur.UserId == userId  
                          select ur.RoleId).Contains(rop.RoleId) // 1. 查询用户的角色
                   select rop; // 2. 查询角色组织权限关联
        }

        /// <summary>
        /// 查询用户(userId)可见的角色组织权限列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public async Task<IEnumerable<RoleOrgPer>> FindForPerByUserId(string userId)
        {
            // 1. 查询用户的有权组织
            // 2. 
            return from rop in RoleOrgPerStore.Context.Set<RoleOrgPer>()
                   where (from ur in RoleOrgPerStore.Context.Set<UserRole>()
                          where ur.UserId == userId
                          select ur.RoleId).Contains(rop.RoleId) // 1. 查询用户的角色
                   select rop; // 2. 查询角色组织权限关联
        }

        /// <summary>
        /// 用户(userId)查询可见的角色组织权限列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public async Task<IEnumerable<RoleOrgPer>> FindFromOrgByUserId(string userId)
        {
            // 查询用户包含权限的组织
            var perOrgIds = (await RoleOrgPerStore.FindOrgByUserIdPerName(userId, Constants.AUTH_MANAGE)).Select(org => org.Id);
            // 查询组织相关的角色权限关联
            var roleOrgPers = await RoleOrgPerStore.Find(rop => perOrgIds.Contains(rop.OrgId)).Include(r => r.Org).Include(r => r.Per).Include(r => r.Role).AsNoTracking().ToListAsync();
            return roleOrgPers;
        }

        /// <summary>
        /// 是否有权限 用户在某个组织下是否具有某项权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="orgId">操作组织ID-前端传入、表示数据范围</param>
        /// <param name="perName">权限ID</param>
        /// <returns></returns>
        public async Task<bool> HasPermission(string userId, string perName, string orgId)
        {
            // 1. 通过用户ID和权限名查询有权组织ID集合
            var perOrgIds = (await RoleOrgPerStore.FindOrgByUserIdPerName(userId, perName)).Select(org => org.Id);
            // 2. 判断传入的组织ID在这些权限组织ID集合中
            return perOrgIds.Contains(orgId);
        }

        /// <summary>
        /// 某用户在某组织下是否具有某项权限
        /// </summary>
        /// <param name="userId">登陆用户ID</param>
        /// <param name="perName">权限名</param>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        public async Task<bool> HasPermissionForUser(string userId, string perName, string id)
        {
            // 1. 通过用户ID和权限名查询有权组织ID集合
            var perOrgIds = (await RoleOrgPerStore.FindOrgByUserIdPerName(userId, perName)).Select(org => org.Id);
            // 2. 查询用户所在组织ID集合
            var orgIds = await OrganizationStore.FindByUserId(id).Select(org => org.Id).AsNoTracking().ToListAsync();
            return perOrgIds.ContainsAll(orgIds);
        }

        /// <summary>
        /// 用户(userId)在自身的组织下是否具有权限(perName)
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="perName">权限名</param>
        /// <returns></returns>
        public async Task<bool> HasPermissionInSelfOrg(string userId, string perName)
        {
            // 1. 通过用户ID和权限名查询有权组织ID集合
            var perOrgIds = (await RoleOrgPerStore.FindOrgByUserIdPerName(userId, perName)).Select(org => org.Id);
            // 2. 查询用户所在组织
            var userOrgIds = await OrganizationStore.FindByUserId(userId).Select(uo => uo.Id).ToListAsync();
            return perOrgIds.ContainsAll(userOrgIds);
        }

        /// <summary>
        /// 用户(userId)是否具有权限(perName)
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="perName">权限名</param>
        /// <returns></returns>
        public async Task<bool> HasPermission(string userId, string perName)
        {
            // 1. 通过用户ID和权限名查询有权组织ID集合
            var perOrgIds = (await RoleOrgPerStore.FindOrgByUserIdPerName(userId, perName)).Select(org => org.Id);
            return perOrgIds.Any();
        }

        /// <summary>
        /// 判断用户(userId)有没权限(perName)操作资源(resourceId)
        /// </summary>
        /// <typeparam name="TResource">资源类型</typeparam>
        /// <param name="userId">用户ID</param>
        /// <param name="perName">权限名称</param>
        /// <param name="resourceId">资源ID(主键)</param>
        /// <returns></returns>
        public async Task<bool> HasPermission<TResource>(string userId, string perName, string resourceId) where TResource : class
        {
            // 1. 通过用户ID和权限名查询有权组织ID集合
            var perOrgIds = (await RoleOrgPerStore.FindOrgByUserIdPerName(userId, perName)).Select(org => org.Id);
            // 2. 查询资源所在组织
            var srcOrgIds = (await OrganizationStore.FindByUserIdSrcId<TResource>(userId, resourceId)).Select(uo => uo.Id).ToList();
            return perOrgIds.ContainsAll(srcOrgIds);
        }

        /// <summary>
        /// 查询有权组织
        /// 根据用户名和权限名查询权限组织
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="perName">权限名</param>
        /// <returns></returns>
        public Task<IEnumerable<Organization>> FindOrgByUserIdPerName(string userId, string perName)
        {
            return  RoleOrgPerStore.FindOrgByUserIdPerName(userId, perName);
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
                perOrgIds.AddRange((await OrganizationStore.FindChildrenFromOrgById(orgId)).Select(org => org.Id));
            }
            perOrgIds.AddRange(rootOrgIds);
            // 4. 判断传入的组织ID列表是有权限组织ID列表的子集
            return perOrgIds.ContainsAll(orgIds);
        }

        /// <summary>
        /// 用户(userId)为角色授权(roleOrgPer)
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="roleOrgPer">角色组织权限关系</param>
        /// <returns></returns>
        public async Task CreateByUserId(string userId, RoleOrgPer roleOrgPer)
        {
            await RoleOrgPerStore.CreateByUserId(userId, roleOrgPer.RoleId, roleOrgPer.OrgId, roleOrgPer.PerId);
        }

        /// <summary>
        /// 用户(userId)更新角色授权(json)
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleOrgPer"></param>
        /// <returns></returns>
        public async Task UpdateByUserId(string userId, RoleOrgPer roleOrgPer)
        {
            await RoleOrgPerStore.UpdateByUserId(userId, roleOrgPer);
        }

        /// <summary>
        /// 用户(userId)通过条件(predicate)查询角色权限
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IQueryable<RoleOrgPer> FindByUserId(string userId, Func<RoleOrgPer, bool> predicate)
        {
            return RoleOrgPerStore.Find(predicate);
        }

        /// <summary>
        /// 重新扩展用户组织权限表
        /// </summary>
        /// <returns></returns>
        public async Task ReExpansion()
        {
            await RoleOrgPerStore.ReExpansion();
        }
    }
}
