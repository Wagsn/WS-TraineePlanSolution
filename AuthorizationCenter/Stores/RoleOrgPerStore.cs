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
    public class RoleOrgPerStore : StoreBase<RoleOrgPer>, IRoleOrgPerStore
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
        public RoleOrgPerStore(ApplicationDbContext context, IOrganizationStore organizationStore) : base(context)
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
            var orgList = await OrganizationStore.FindChildrenFromOrgRelById(orgIds).AsNoTracking().ToListAsync();
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
            var orgList = await OrganizationStore.FindChildrenFromOrgRelById(orgIds).Include(org => org.Parent).AsNoTracking().ToListAsync();
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
        /// 用户(userId)更新角色组织权限(roleOrgPer) -不可用
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
                    var newUserOrgPers = await GenUserPermissionExpansion(roleOrgPer.RoleId, roleOrgPer.OrgId, roleOrgPer.PerId);
                    Context.AddRange(newUserOrgPers);
                    // 4. 更新角色组织权限
                    Context.Attach(roleOrgPer);
                    Context.Update(roleOrgPer);
                    await Context.SaveChangesAsync();
                    trans.Commit();
                }
                catch (Exception e)
                {
                    Logger.Error($"[{nameof(UpdateByUserId)}] 用户({userId})更新角色组织权限:\r\n{JsonUtil.ToJson(roleOrgPer)}\r\n失败:\r\n{e}");
                    trans.Rollback();
                    throw new Exception("用户()更新角色组织权限()失败", e);
                }
            }
        }

        ///// <summary>
        ///// 用户(userId)创建角色组织权限(roleOrgPer)
        ///// </summary>
        ///// <param name="userId">用户ID</param>
        ///// <param name="roleOrgPer">角色组织权限</param>
        ///// <returns></returns>
        //public async Task CreateByUserId(string userId, RoleOrgPer roleOrgPer)
        //{
        //    using (var trans = await Context.Database.BeginTransactionAsync())
        //    {
        //        try
        //        {
        //            // 1. 查询旧的用户组织权限关联
                    
        //            var oldUserOrgPers = await (from uop in Context.Set<UserPermissionExpansion>()
        //                                        where uop.OrganizationId==roleOrgPer.OrgId&&uop.PermissionId==roleOrgPer.PerId
        //                                        select uop).AsNoTracking().ToListAsync();
        //            // 2. 生成的用户组织权限关联
        //            var genUserOrgPers = await GenUserPermissionExpansion(roleOrgPer.RoleId, roleOrgPer.OrgId, roleOrgPer.PerId);
        //            // 3. 得到需要添加的用户组织权限关联
        //            var newUops = new List<UserPermissionExpansion>();
        //            foreach(var newUop in newUops)
        //            {
        //                bool flag = true;
        //                foreach(var oldUop in oldUserOrgPers)
        //                {
        //                    if(oldUop.OrganizationId == newUop.OrganizationId && oldUop.PermissionId == newUop.PermissionId && oldUop.UserId == newUop.UserId)
        //                    {
        //                        flag = false;
        //                    }
        //                }
        //                if (flag)
        //                {
        //                    newUops.Add(newUop);
        //                }
        //            }
        //            // 4. 添加需要的用户组织权限关联
        //            Context.AddRange(newUops);
        //            // 5. 添加角色组织权限
        //            if (roleOrgPer.Id == null)
        //            {
        //                roleOrgPer.Id = Guid.NewGuid().ToString();
        //            }
        //            Context.Add(roleOrgPer);

        //            await Context.SaveChangesAsync();
        //            trans.Commit();
        //        }
        //        catch (Exception e)
        //        {
        //            Logger.Error($"[{nameof(CreateByUserId)}] 用户({userId})创建角色组织权限:\r\n{JsonUtil.ToJson(roleOrgPer)}\r\n失败:\r\n{e}");
        //            trans.Rollback();
        //            throw new Exception($"用户({userId})创建角色组织权限失败", e);
        //        }
        //    }
        //}

        /// <summary>
        /// 用户(userId)创建角色组织权限(roleOrgPer)
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="rId"></param>
        /// <param name="oId"></param>
        /// <param name="pId"></param>
        /// <returns></returns>
        public async Task CreateByUserId(string userId, string rId, string oId, string pId)
        {
            if(await Exist(rop=> rop.RoleId==rId && rop.OrgId == oId && rop.PerId == pId))
            {
                Logger.Warn($"[{nameof(CreateByUserId)}] 用户({userId})添加角色({rId})组织({oId})权限({pId})关联重复，该关联已经存在");
                return;
            }
            using (var trans = await Context.Database.BeginTransactionAsync())
            {
                try
                {
                    // 1. 查询旧的用户组织权限关联

                    var oldUserOrgPers = await (from uop in Context.Set<UserPermissionExpansion>()
                                                where uop.OrganizationId == oId && uop.PermissionId == pId
                                                select uop).AsNoTracking().ToListAsync();
                    // 2. 生成的用户组织权限关联
                    var genUserOrgPers = await GenUserPermissionExpansion(rId, oId, pId);
                    // 3. 得到需要添加的用户组织权限关联
                    var newUops = new List<UserPermissionExpansion>();
                    foreach (var newUop in genUserOrgPers)
                    {
                        bool flag = true;
                        foreach (var oldUop in oldUserOrgPers)
                        {
                            if (oldUop.OrganizationId == newUop.OrganizationId && oldUop.PermissionId == newUop.PermissionId && oldUop.UserId == newUop.UserId)
                            {
                                flag = false;
                            }
                        }
                        if (flag)
                        {
                            newUops.Add(newUop);
                        }
                    }
                    // 4. 添加需要的用户组织权限关联
                    Context.AddRange(newUops);
                    // 5. 添加角色组织权限
                    Context.Add(new RoleOrgPer
                    {
                        Id = Guid.NewGuid().ToString(),
                        RoleId =rId,
                        OrgId =oId,
                        PerId =pId
                    });

                    await Context.SaveChangesAsync();
                    trans.Commit();
                }
                catch (Exception e)
                {
                    Logger.Error($"[{nameof(CreateByUserId)}] 用户({userId})创建角色({rId})组织({oId})权限({pId})关联失败:\r\n{e}");
                    trans.Rollback();
                    throw new Exception($"用户({userId})创建角色组织权限失败", e);
                }
            }
        }
        
        /// <summary>
        /// 用户(userId)删除角色组织权限(ropId)
        /// 当删除角色时要删除用户角色关联和角色组织关联和角色权限关联（当用户角色关联删除完后角色生成的用户权限也被删除完了）
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
                    if (roleOrgPer == null)
                    {
                        Logger.Warn($"[{nameof(CreateByUserId)}] 用户({userId})删除角色组织权限({ropId})不存在");
                        return;
                    }
                    // 1. 查询旧的关联表 
                    var oldUserRoles = await (from ur in Context.Set<UserRole>()
                                              where ur.RoleId != roleOrgPer.RoleId
                                              select ur).AsNoTracking().ToListAsync();
                    var oldUserOrgPers = new List<UserPermissionExpansion>();
                    foreach (var oldUserRole in oldUserRoles)
                    {
                        oldUserOrgPers.AddRange(await GenUserPermissionExpansion(oldUserRole.UserId, oldUserRole.RoleId));
                    }
                    // 2. 生成新的关联表
                    var genUserOrgPers =await GenUserPermissionExpansion(roleOrgPer.RoleId, roleOrgPer.OrgId, roleOrgPer.PerId);
                    // 3. 筛选删除的用户组织权限关联
                    var subUserOrgPers = new List<UserPermissionExpansion>();
                    foreach(var newUop in genUserOrgPers)
                    {
                        bool flag = true;
                        foreach (var oldUop in oldUserOrgPers)
                        {
                            if ((oldUop.OrganizationId == newUop.OrganizationId && oldUop.PermissionId == newUop.PermissionId && oldUop.UserId == newUop.UserId))
                            {
                                flag = false;
                            }
                        }
                        if (flag)
                        {
                            subUserOrgPers.Add(newUop);
                        }
                    }
                    // 4. 删除用户组织权限关联
                    var delUops = new List<UserPermissionExpansion>();
                    foreach (var newUop in subUserOrgPers)
                    {
                        delUops.AddRange(await (from uop in Context.Set<UserPermissionExpansion>()
                                                where uop.OrganizationId == newUop.OrganizationId && uop.PermissionId == newUop.PermissionId && uop.UserId == newUop.UserId
                                                select uop).AsNoTracking().ToListAsync());
                    }
                    Context.RemoveRange(delUops);
                    // 3. 删除角色组织权限
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
        /// 用户(userId)删除角色组织权限(ropId)
        /// 当删除角色时要删除用户角色关联和角色组织关联和角色权限关联（当用户角色关联删除完后角色生成的用户权限也被删除完了）
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task DeleteByUserId(string userId, Func<RoleOrgPer, bool> predicate)
        {
            var ropIds = await Find(predicate).Select(rop => rop.Id).AsNoTracking().ToListAsync();
            foreach(var ropId in ropIds)
            {
                await DeleteByUserId(userId, ropId);
            }
        }

        /// <summary>
        /// 用户组织权限扩展
        /// </summary>
        /// <returns></returns>
        public async Task ReExpansion()
        {
            using (var trans = await Context.Database.BeginTransactionAsync())
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
                catch (Exception e)
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
                    if (userRole.RoleId == rop.RoleId && !userOrgPers.Any(uop => uop.UserId==userRole.UserId&&uop.OrganizationId==rop.OrgId&&uop.PermissionId==rop.PerId))
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
            // 去重
            var result = new List<UserPermissionExpansion>();
            foreach(var userOrgPer in userOrgPers)
            {
                if(!result.Any(uop => uop.UserId == userOrgPer.UserId && uop.OrganizationId == userOrgPer.OrganizationId && uop.PermissionId == userOrgPer.PermissionId))
                {
                    result.Add(userOrgPer);
                }
            }
            return result;
        }

        /// <summary>
        /// 根据角色组织权限生成用户组织权限
        /// </summary>
        /// <param name="rId"></param>
        /// <param name="oId"></param>
        /// <param name="pId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<UserPermissionExpansion>> GenUserPermissionExpansion(string rId, string oId, string pId)
        {
            // 1. 找到所有用户角色关系
            var userRoles = await Context.Set<UserRole>().Where(ur => ur.RoleId == rId).AsNoTracking().ToListAsync();
            // 2. 生成用户组织权限数据
            var userOrgPers = new List<UserPermissionExpansion>();
            foreach (var ur in userRoles)
            {
                userOrgPers.Add(new UserPermissionExpansion
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = ur.UserId,
                    OrganizationId = oId,
                    PermissionId = pId
                });
            }
            return userOrgPers;
        }

        /// <summary>
        /// 根据用户角色(userRole)生成用户组织权限
        /// </summary>
        /// <param name="uId"></param>
        /// <param name="rId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<UserPermissionExpansion>> GenUserPermissionExpansion(string uId, string rId)
        {
            // 1. 找到所有角色组织权限
            var roleOrgPers = await Context.Set<RoleOrgPer>().Where(rop => rop.RoleId == rId).AsNoTracking().ToListAsync();
            // 2. 生成用户组织权限数据
            var userOrgPers = new List<UserPermissionExpansion>();
            foreach (var rop in roleOrgPers)
            {
                userOrgPers.Add(new UserPermissionExpansion
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = uId,
                    OrganizationId = rop.OrgId,
                    PermissionId = rop.PerId
                });
            }
            return userOrgPers;
        }

        ///// <summary>
        ///// 根据用户角色(userRole)查询用户组织权限
        ///// </summary>
        ///// <param name="userRole">用户角色</param>
        ///// <returns></returns>
        //public async Task<IEnumerable<UserPermissionExpansion>> FindUserPermissionExpansion(UserRole userRole)
        //{
        //    // 1. 找到所有角色组织权限
        //    var roleOrgPers = await Context.Set<RoleOrgPer>().Where(rop => rop.RoleId == userRole.RoleId).AsNoTracking().ToListAsync();
        //    // 2. 生成用户组织权限数据
        //    var userOrgPers = new List<UserPermissionExpansion>();
        //    foreach (var rop in roleOrgPers)
        //    {
        //        userOrgPers.Add(new UserPermissionExpansion
        //        {
        //            Id = Guid.NewGuid().ToString(),
        //            UserId = userRole.UserId,
        //            OrganizationId = rop.OrgId,
        //            PermissionId = rop.PerId
        //        });
        //    }
        //    var res = from uop in Context.Set<UserPermissionExpansion>()
        //              where userOrgPers.Any(a => a.OrganizationId==uop.OrganizationId && a.PermissionId ==uop.PermissionId&&a.UserId==uop.UserId)
        //              select uop;
        //    return await res.AsNoTracking().ToListAsync();
        //}

        class Comparer : IEqualityComparer<UserPermissionExpansion>
        {
            public bool Equals(UserPermissionExpansion x, UserPermissionExpansion y)
            {
                throw new NotImplementedException();
            }

            public int GetHashCode(UserPermissionExpansion obj)
            {
                throw new NotImplementedException();
            }
        }
    }
}
