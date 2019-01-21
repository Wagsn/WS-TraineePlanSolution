using AuthorizationCenter.Entitys;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WS.Log;

namespace AuthorizationCenter.Stores
{
    /// <summary>
    /// 用户角色存储
    /// </summary>
    public class UserRoleStore : StoreBase<UserRole>, IUserRoleStore
    {
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="context"></param>
        public UserRoleStore(ApplicationDbContext context): base(context){ }

        /// <summary>
        /// 创建用户角色关联
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="userRole">用户角色</param>
        /// <returns></returns>
        public async Task Create(string userId, UserRole userRole)
        {
            try
            {
                Context.Add(userRole);
                await Context.SaveChangesAsync();
            }
            catch(Exception e)
            {
                Console.WriteLine($"保存失败：\r\n{e}");
            }
        }

        /// <summary>
        /// 创建用户角色关系
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="uId"></param>
        /// <param name="rId"></param>
        /// <returns></returns>
        public async Task CreateByUserId(string userId, string uId, string rId)
        {
            // 0. 参数检查
            if(await Exist(ur => ur.UserId == uId && ur.RoleId == rId))
            {
                Logger.Warn($"[{nameof(CreateByUserId)}] 用户({userId})添加用户({uId})角色({rId})关联重复，该关联已经被添加。");
                return;
            }
            using (var trans = await Context.Database.BeginTransactionAsync())
            {
                try
                {
                    // 1. 生成生成用户组织权限
                    // 1.1 找到所有角色组织权限
                    var roleOrgPers = await Context.Set<RoleOrgPer>().Where(rop => rop.RoleId == rId).AsNoTracking().ToListAsync();
                    // 1.2 生成用户组织权限数据
                    var genUserOrgPers = new List<UserPermissionExpansion>();
                    foreach (var rop in roleOrgPers)
                    {
                        genUserOrgPers.Add(new UserPermissionExpansion
                        {
                            Id = Guid.NewGuid().ToString(),
                            UserId = uId,
                            OrganizationId = rop.OrgId,
                            PermissionId = rop.PerId
                        });
                    }
                    // 2. 查询用户组织权限
                    var oldUserOrgPers = await (from uop in Context.Set<UserPermissionExpansion>()
                                                where uop.UserId == uId
                                                select uop).AsNoTracking().ToListAsync();
                    // 3. 获取需要添加的用户组织权限
                    var newUserOrgPers = new List<UserPermissionExpansion>();
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
                            newUserOrgPers.Add(newUop);
                        }
                    }
                    // 4. 添加用户组织权限
                    Context.AddRange(newUserOrgPers);
                    // 5. 添加用户角色关联
                    Context.Add(new UserRole
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = uId,
                        RoleId = rId
                    });
                    await Context.SaveChangesAsync();
                    trans.Commit();
                }
                catch (Exception e)
                {
                    trans.Rollback();
                    Logger.Error($"用户({userId})添加用户({uId})角色{rId}关联失败:\r\n{e}");
                }
            }
        }
        
        ///// <summary>
        ///// 创建用户角色关系
        ///// </summary>
        ///// <param name="userId">用户ID</param>
        ///// <param name="uId"></param>
        ///// <param name="rId"></param>
        ///// <returns></returns>
        //public async Task CreateByUserId(string userId, string uId, string rId)
        //{
        //    using (var trans = await Context.Database.BeginTransactionAsync())
        //    {
        //        try
        //        {
        //            // 1. 生成生成用户组织权限
        //            // 1.1 找到所有角色组织权限
        //            var roleOrgPers = await Context.Set<RoleOrgPer>().Where(rop => rop.RoleId == rId).AsNoTracking().ToListAsync();
        //            // 1.2 生成用户组织权限数据
        //            var genUserOrgPers = new List<UserPermissionExpansion>();
        //            foreach (var rop in roleOrgPers)
        //            {
        //                genUserOrgPers.Add(new UserPermissionExpansion
        //                {
        //                    Id = Guid.NewGuid().ToString(),
        //                    UserId = uId,
        //                    OrganizationId = rop.OrgId,
        //                    PermissionId = rop.PerId
        //                });
        //            }
        //            // 2. 查询用户组织权限
        //            var oldUserOrgPers = await (from uop in Context.Set<UserPermissionExpansion>()
        //                                        where uop.UserId == uId
        //                                        select uop).AsNoTracking().ToListAsync();
        //            // 3. 获取需要添加的用户组织权限
        //            var newUserOrgPers = new List<UserPermissionExpansion>();
        //            foreach (var newUop in newUserOrgPers)
        //            {
        //                bool flag = true;
        //                foreach (var oldUop in oldUserOrgPers)
        //                {
        //                    if (oldUop.OrganizationId == newUop.OrganizationId || oldUop.PermissionId == newUop.PermissionId || oldUop.UserId == newUop.UserId)
        //                    {
        //                        flag = false;
        //                    }
        //                }
        //                if (flag)
        //                {
        //                    newUserOrgPers.Add(newUop);
        //                }
        //            }
        //            // 4. 添加用户组织权限
        //            Context.AddRange(newUserOrgPers);
        //            // 5. 添加用户角色关联
        //            Context.Add(new UserRole
        //            {
        //                Id = Guid.NewGuid().ToString(),
        //                UserId = uId,
        //                RoleId = rId
        //            });
        //            await Context.SaveChangesAsync();
        //            trans.Commit();
        //        }
        //        catch (Exception e)
        //        {
        //            trans.Rollback();
        //            Logger.Error($"用户({userId})添加用户({uId})角色{rId}关联失败:\r\n{e}");
        //        }
        //    }
        //}

        /// <summary>
        /// 用户删除用户角色关联
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="urId"></param>
        /// <returns></returns>
        public async Task DeleteByUserId(string userId, string urId)
        {
            var userRole = await Context.Set<UserRole>().Where(ur => ur.Id == urId).AsNoTracking().SingleOrDefaultAsync();
            if (userRole == null)
            {
                Logger.Warn($"[{nameof(DeleteByUserId)}] 用户({userId})删除的用户角色关联({urId})不存在");
                return;
            }
            await DeleteByUserId(userId, userRole.UserId, userRole.RoleId);
        }

        /// <summary>
        /// 用户删除用户角色关联
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="uId"></param>
        /// <param name="rId"></param>
        /// <returns></returns>
        public async Task DeleteByUserId(string userId, string uId, string rId)
        {
            using (var trans = await Context.Database.BeginTransactionAsync())
            {
                try
                {
                    // 1. 生成生成用户组织权限
                    // 1.1 找到所有角色组织权限
                    var roleOrgPers = await Context.Set<RoleOrgPer>().Where(rop => rop.RoleId == rId).AsNoTracking().ToListAsync();
                    // 1.2 生成用户组织权限数据
                    var genUserOrgPers = new List<UserPermissionExpansion>();
                    foreach (var rop in roleOrgPers)
                    {
                        genUserOrgPers.Add(new UserPermissionExpansion
                        {
                            Id = Guid.NewGuid().ToString(),
                            UserId = uId,
                            OrganizationId = rop.OrgId,
                            PermissionId = rop.PerId
                        });
                    }
                    // 2. 查询该用户的由其它角色产生的用户组织权限关联
                    // U==U01&&R!=R01
                    var oldUserRoles = await (from ur in Context.Set<UserRole>()
                                              where ur.UserId == uId && ur.RoleId != rId
                                              select ur).AsNoTracking().ToListAsync();

                    var oldUserOrgPers = new List<UserPermissionExpansion>();
                    foreach(var oldUserRole in oldUserRoles)
                    {
                        oldUserOrgPers.AddRange(await GenUserPermissionExpansion(oldUserRole));
                    }
                    // 3. 获取需要删除的用户组织权限
                    var subUserOrgPers = new List<UserPermissionExpansion>();
                    foreach (var newUop in genUserOrgPers)
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
                    // 5. 删除用户角色关联
                    var userRoles = await (from ur in Context.Set<UserRole>()
                                           where ur.UserId == uId && ur.RoleId == rId
                                           select ur).AsNoTracking().ToListAsync();
                    Context.RemoveRange(userRoles);
                    await Context.SaveChangesAsync();
                    trans.Commit();
                }
                catch (Exception e)
                {
                    trans.Rollback();
                    Logger.Error($"用户({userId})删除用户({uId})角色{rId}关联失败:\r\n{e}");
                    throw new Exception($"用户({userId})删除用户({uId})角色({rId})关联失败", e);
                }
            }
        }

        /// <summary>
        /// 用户条件删除用户角色关联
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task DeleteByUserId(string userId, Func<UserRole, bool> predicate)
        {
            var userRoles = await Find(predicate).AsNoTracking().ToListAsync();
            foreach(var ur in userRoles)
            {
                await DeleteByUserId(userId, ur.Id);
            }
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

        /// <summary>
        /// 根据用户角色(userRole)查询用户组织权限
        /// </summary>
        /// <param name="userRole">用户角色</param>
        /// <returns></returns>
        public async Task<IEnumerable<UserPermissionExpansion>> FindUserPermissionExpansion(UserRole userRole)
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
            var res = from uop in Context.Set<UserPermissionExpansion>()
                      where userOrgPers.Any(a => a.OrganizationId == uop.OrganizationId && a.PermissionId == uop.PermissionId && a.UserId == uop.UserId)
                      select uop;
            return await res.AsNoTracking().ToListAsync();
        }
    }
}
