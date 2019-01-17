using AuthorizationCenter.Entitys;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WS.Log;
using WS.Text;

namespace AuthorizationCenter.Stores
{
    /// <summary>
    /// 角色组织权限关联存储实现
    /// </summary>
    public class RoleOrgPerStore: StoreBase<RoleOrgPer>, IRoleOrgPerStore
    {
        /// <summary>
        /// 组织存储 -NOTE：小心循环调用
        /// </summary>
        public IOrganizationStore OrganizationStore { get; set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="context"></param>
        /// <param name="organizationStore"></param>
        public RoleOrgPerStore(ApplicationDbContext context, IOrganizationStore organizationStore):base(context)
        {
            OrganizationStore = organizationStore;
        }

        /// <summary>
        /// 查询用户拥有某项权限（用户可能拥有其父级权限）的所有组织
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="perName">权限名称</param>
        /// <returns></returns>
        public async Task<IEnumerable<Organization>> FindOrgByUserIdPerName(string userId, string perName)
        {
            // return await FindOrgFromURAndROPByUserIdPerName(userId, perName);
            return await FindOrgFromUOPByUserIdPerName(userId, perName);
        }

        /// <summary>
        /// 查询用户拥有某项权限（用户可能拥有其父级权限）的所有组织
        /// 如果用户拥有的权限是在该操作权限之上 ROOT > USER_MANAGE > USER_QUERY
        /// 有权组织列表获取，通过用户ID和权限名称获取组织列表(U.ID-[UR]->R.ID, P.N-[P]->P.ID-[P]->P.ID)-[ROP]->O.ID-[O]->O.ID
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="perName">权限名称</param>
        /// <returns></returns>
        public async Task<IEnumerable<Organization>> FindOrgFromURAndROPByUserIdPerName(string userId, string perName)
        {
            // 1. 查询该操作的所有权限（包含父级权限）
            // 1.1 通过权限名称查询权限ID
            var perId = await (from per in Context.Permissions
                               where per.Name == perName
                               select per.Id).AsNoTracking().SingleAsync();
            // 1.2 查询权限ID的所有父级权限构成权限ID集合（包含自身）
            var perIds = (await FindParentById(perId)).Select(per => per.Id);
            // 2. 查询用户包含的角色Id列表
            var roleIds = await (from ur in Context.UserRoles
                                 where ur.UserId == userId
                                 select ur.RoleId).AsNoTracking().ToListAsync();
            // 3. 通过权限ID集合和角色ID集合查询有权根组织ID集合
            var orgIds = await (from rop in Context.RoleOrgPers
                                where perIds.Contains(rop.PerId)
                                && (roleIds).Contains(rop.RoleId) // 通过权限和角色查询组织
                                select rop.OrgId).AsNoTracking().ToListAsync();
            // 4. 扩展成组织列表
            var orgList = await OrganizationStore.FindChildrenFromRelById(orgIds).AsNoTracking().ToListAsync();
            Logger.Trace($"[{nameof(FindOrgByUserIdPerName)}] 用户({userId})拥有权限({perName})的组织有:\r\n{JsonUtil.ToJson(orgList)}");
            return orgList;
        }

        /// <summary>
        /// 查询用户拥有某项权限（用户可能拥有其父级权限）的所有组织
        /// 如果用户拥有的权限是在该操作权限之上 ROOT > USER_MANAGE > USER_QUERY
        /// 有权组织列表获取，通过用户ID和权限名称获取组织列表(U.ID-[UR]->R.ID, P.N-[P]->P.ID-[P]->P.ID)-[ROP]->O.ID-[O]->O.ID
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="perName">权限名称</param>
        /// <returns></returns>
        public async Task<IEnumerable<Organization>> FindOrgFromUOPByUserIdPerName(string userId, string perName)
        {
            // 1. 查询可以执行该操作的所有权限（包含父级权限）
            // 1.1 通过权限名称查询权限ID
            var perId = await (from per in Context.Permissions
                               where per.Name == perName
                               select per.Id).AsNoTracking().SingleAsync();
            // 1.2 查询权限ID的所有父级权限构成权限ID集合（包含自身）
            var perIds = (await FindParentById(perId)).Select(per => per.Id);
            // 2. 通过权限ID集合和用户ID查询有权根组织ID集合
            var orgIds = await (from uop in Context.Set<UserPermissionExpansion>()
                                where uop.UserId == userId
                                    && (perIds).Contains(uop.PermissionId)  // 如果用户的权限是该权限的父权限，则表示用户用户该权限
                                select uop.OrganizationId).AsNoTracking().ToListAsync();
            // 3. 扩展成组织列表
            var orgList = await OrganizationStore.FindChildrenFromRelById(orgIds).AsNoTracking().ToListAsync();
            Logger.Trace($"[{nameof(FindOrgByUserIdPerName)}] 用户({userId})拥有权限({perName})的组织有:\r\n{JsonUtil.ToJson(orgList)}");
            return orgList;
        }

        /// <summary>
        /// 查询所有上级权限（包含自身）
        /// </summary>
        /// <param name="perId">权限ID</param>
        /// <returns></returns>
        public async Task<List<Permission>> FindParentById(string perId)
        {
            var perList = new List<Permission>();
            if (perId == null)
            {
                return perList;
            }
            var per = await (from p in Context.Set<Permission>()
                             where p.Id == perId
                             select p).AsNoTracking().Include(p => p.Parent).SingleAsync();
            perList.Add(per);
            perList.AddRange(await FindParentById(per.ParentId));
            return perList;
        }

        /// <summary>
        /// 用户(userId)更新角色组织权限(roleOrgPer)
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="roleOrgPer">角色组织权限</param>
        /// <returns></returns>
        public async Task UpdateByUserId(string userId, RoleOrgPer roleOrgPer)
        {
            using (var trans = await Context.Database.BeginTransactionAsync())
            {
                try
                {
                    // 1. 查询旧的角色组织权限
                    var oldRoleOrgPer = await Context.Set<RoleOrgPer>().Where(rop => rop.Id == roleOrgPer.Id).SingleOrDefaultAsync();
                    // 2. 查询并删除旧的用户组织权限
                    var oldUserOrgPers = await (from uop in Context.Set<UserPermissionExpansion>()
                                             where uop.OrganizationId == oldRoleOrgPer.OrgId && uop.PermissionId == oldRoleOrgPer.PerId
                                                && (from ur in Context.Set<UserRole>()
                                                    where ur.RoleId == oldRoleOrgPer.RoleId
                                                    select ur.UserId).Contains(uop.UserId)
                                             select uop).AsNoTracking().ToListAsync();
                    Context.AttachRange(oldUserOrgPers);
                    Context.RemoveRange(oldUserOrgPers);
                    // 3. 生成并添加新的用户组织权限
                    var newUserOrgPers = await GenUserPermissionExpansion(roleOrgPer);
                    Context.AddRange(newUserOrgPers);
                    // 4. 更新角色组织权限
                    Context.Attach(roleOrgPer);
                    Context.Update(roleOrgPer);
                    await Context.SaveChangesAsync();
                    trans.Commit();
                }
                catch(Exception e)
                {
                    Logger.Error($"[{nameof(UpdateByUserId)}] 用户({userId})更新角色组织权限:\r\n{JsonUtil.ToJson(roleOrgPer)}\r\n失败:\r\n{e}");
                    trans.Rollback();
                    throw new Exception("用户()更新角色组织权限()失败", e);
                }
            }
        }

        /// <summary>
        /// 用户(userId)创建角色组织权限(roleOrgPer)
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="roleOrgPer">角色组织权限</param>
        /// <returns></returns>
        public async Task CreateByUserId(string userId, RoleOrgPer roleOrgPer)
        {
            using(var trans = await Context.Database.BeginTransactionAsync())
            {
                try
                {
                    // 1. 添加角色组织权限关联
                    Context.Add(roleOrgPer);
                    // 2. 添加用户组织权限关联
                    var userOrgPers = await GenUserPermissionExpansion(roleOrgPer);
                    Context.AddRange(userOrgPers);
                    await Context.SaveChangesAsync();
                    trans.Commit();
                }
                catch(Exception e)
                {
                    Logger.Error($"[{nameof(CreateByUserId)}] 用户({userId})创建角色组织权限:\r\n{JsonUtil.ToJson(roleOrgPer)}\r\n失败:\r\n{e}");
                    trans.Rollback();
                    throw new Exception($"用户({userId})创建角色组织权限失败", e);
                }
            }
        }

        /// <summary>
        /// 用户(userId)删除角色组织权限(ropId)
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="ropId">角色组织权限ID</param>
        /// <returns></returns>
        public async Task DeleteByUserId(string userId, string ropId)
        {
            using (var trans = await Context.Database.BeginTransactionAsync())
            {
                try
                {
                    // 1. 查询角色组织权限关联
                    var roleOrgPer = await Context.Set<RoleOrgPer>().Where(rop => rop.Id == ropId).AsNoTracking().SingleOrDefaultAsync();
                    if(roleOrgPer == null)
                    {
                        Logger.Warn($"[{nameof(CreateByUserId)}] 用户({userId})删除角色组织权限({ropId})不存在");
                        return;
                    }
                    // 2. 查询并删除用户组织权限关联
                    var userOrgPers = await (from uop in Context.Set<UserPermissionExpansion>()
                                             where uop.OrganizationId == roleOrgPer.OrgId && uop.PermissionId == roleOrgPer.PerId
                                                && (from ur in Context.Set<UserRole>()
                                                    where ur.RoleId == roleOrgPer.RoleId
                                                    select ur.UserId).Contains(uop.UserId)
                                             select uop).AsNoTracking().ToListAsync();
                    Context.AttachRange(userOrgPers);
                    Context.RemoveRange(userOrgPers);
                    // 3. 删除角色组织权限
                    Context.Attach(roleOrgPer);
                    Context.Remove(roleOrgPer);
                    await Context.SaveChangesAsync();
                    trans.Commit();
                }
                catch (Exception e)
                {
                    Logger.Error($"[{nameof(CreateByUserId)}] 用户({userId})删除角色组织权限({ropId})\r\n失败:\r\n{e}");
                    trans.Rollback();
                    throw new Exception($"用户({userId})删除角色组织权限({ropId})失败", e);
                }
            }
        }

        /// <summary>
        /// 用户组织权限扩展
        /// </summary>
        /// <returns></returns>
        public async Task ReExpansion()
        {
            using(var trans =await Context.Database.BeginTransactionAsync())
            {
                try
                {
                    // 1. 删除扩展
                    var oldUserOrgPers = await Context.Set<UserPermissionExpansion>().AsNoTracking().ToListAsync();
                    //Console.WriteLine("老的用户权限扩展: \r\n" + JsonUtil.ToJson(oldUserOrgPers));
                    Context.AttachRange(oldUserOrgPers);
                    Context.RemoveRange(oldUserOrgPers);
                    // 2. 生成扩展
                    var userOrgPers = await GenUserPermissionExpansion();
                    //Console.WriteLine("新的用户权限扩展" + JsonUtil.ToJson(userOrgPers));
                    // 3. 添加扩展
                    Context.AddRange(userOrgPers);
                    await Context.SaveChangesAsync();
                    trans.Commit();
                }
                catch(Exception e)
                {
                    Console.WriteLine("服务器错误:\r\n" + e);
                    trans.Rollback();
                    throw new Exception("用户组织权限扩展失败", e);
                }
            }
        }

        /// <summary>
        /// 生成用户权限扩展表
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<UserPermissionExpansion>> GenUserPermissionExpansion()
        {
            // 1. 找到所有用户角色关系
            var userRoles = await Context.Set<UserRole>().AsNoTracking().ToListAsync();
            // 2. 找到所有角色组织权限关系
            var roleOrgPers = await Context.Set<RoleOrgPer>().AsNoTracking().ToListAsync();
            // 3. 生成用户组织权限数据
            var userOrgPers = new List<UserPermissionExpansion>();
            foreach (var userRole in userRoles)
            {
                foreach (var rop in roleOrgPers)
                {
                    if (userRole.RoleId == rop.RoleId)
                    {
                        userOrgPers.Add(new UserPermissionExpansion
                        {
                            Id = Guid.NewGuid().ToString(),
                            UserId = userRole.UserId,
                            OrganizationId = rop.OrgId,
                            PermissionId = rop.PerId
                        });
                    }
                }
            }
            return userOrgPers;
        }

        /// <summary>
        /// 根据角色组织权限(roleOrgPer)生成用户组织权限
        /// </summary>
        /// <param name="roleOrgPer">角色组织权限</param>
        /// <returns></returns>
        public async Task<IEnumerable<UserPermissionExpansion>> GenUserPermissionExpansion(RoleOrgPer roleOrgPer)
        {
            // 1. 找到所有用户角色关系
            var userRoles = await Context.Set<UserRole>().Where(ur=> ur.RoleId==roleOrgPer.RoleId).AsNoTracking().ToListAsync();
            // 2. 生成用户组织权限数据
            var userOrgPers = new List<UserPermissionExpansion>();
            foreach(var ur in userRoles)
            {
                userOrgPers.Add(new UserPermissionExpansion
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = ur.UserId,
                    OrganizationId = roleOrgPer.OrgId,
                    PermissionId = roleOrgPer.PerId
                });
            }
            return userOrgPers;
        }

        /// <summary>
        /// 根据用户角色(userRole)生成用户组织权限
        /// </summary>
        /// <param name="userRole">用户角色</param>
        /// <returns></returns>
        public async Task<IEnumerable<UserPermissionExpansion>> GenUserPermissionExpansion(UserRole userRole)
        {
            // 1. 找到所有角色组织权限
            var roleOrgPers = await Context.Set<RoleOrgPer>().Where(rop => rop.RoleId == userRole.RoleId).AsNoTracking().ToListAsync();
            // 2. 生成用户组织权限数据
            var userOrgPers = new List<UserPermissionExpansion>();
            foreach (var rop in roleOrgPers)
            {
                userOrgPers.Add(new UserPermissionExpansion
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userRole.UserId,
                    OrganizationId = rop.OrgId,
                    PermissionId = rop.PerId
                });
            }
            return userOrgPers;
        }
    }
}
