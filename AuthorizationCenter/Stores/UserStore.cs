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
    /// 用户核心表存储实现
    /// </summary>
    public class UserStore : StoreBase<User>, IUserStore
    {
        /// <summary>
        /// 用户角色存储
        /// </summary>
        IUserRoleStore UserRoleStore { get; set; }

        /// <summary>
        /// 用户组织关联存储
        /// </summary>
        IUserOrgStore UserOrgStore { get; set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userRoleStore"></param>
        /// <param name="userOrgStore"></param>
        public UserStore(ApplicationDbContext context, IUserRoleStore userRoleStore, IUserOrgStore userOrgStore) :base(context)
        {
            UserRoleStore = userRoleStore;
            UserOrgStore = userOrgStore;
        }

        /// <summary>
        /// 用户(userId)在其组织下创建用户(user)
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="user">用户</param>
        /// <returns></returns>
        public async Task<User> CreateForOrgByUserId(string userId, User user)
        {
            using (var trans = await Context.Database.BeginTransactionAsync())
            {
                try
                {
                    var orgId = await Context.Set<UserOrg>().Where(uo => uo.UserId == userId).Select(uo => uo.OrgId).AsNoTracking().SingleAsync();
                    Context.Add(user);
                    Context.Add(new UserOrg
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = user.Id,
                        OrgId = orgId
                    });
                    await Context.SaveChangesAsync();
                    trans.Commit();
                }
                catch (Exception e)
                {
                    Logger.Error($"事务提交失败:\r\n{e}");
                    trans.Rollback();
                    throw;
                }
            }
            return user;
        }

        /// <summary>
        /// 查询 通过ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IQueryable<User> FindById(string id)
        {
            return Find(ub => ub.Id == id);
        }

        /// <summary>
        /// 在用户组织表中查询组织绑定的所有用户
        /// </summary>
        /// <param name="orgId">组织ID</param>
        /// <returns></returns>
        public IQueryable<User> FindByOrgId(string orgId)
        {
            return from user in Context.Set<User>()
                   where (from uo in Context.Set<UserOrg>()
                          where uo.OrgId == orgId
                          select uo.UserId).Contains(user.Id)
                   select user;
        }


        /// <summary>
        /// 查询所有组织下的用户
        /// </summary>
        /// <param name="orgIds">组织ID集合</param>
        /// <returns></returns>
        public IQueryable<User> FindByOrgId(IEnumerable<string> orgIds)
        {
            return from user in Context.Set<User>()
                   where (from uo in Context.Set<UserOrg>()
                          where orgIds.Contains(uo.OrgId)
                          select uo.UserId).Contains(user.Id)
                   select user;
        }

        ///// <summary>
        ///// 查询所有符合条件(predicate)的组织下的用户
        ///// </summary>
        ///// <param name="predicate">条件</param>
        ///// <returns></returns>
        //public IQueryable<User> FindByOrg(Func<Organization, bool> predicate)
        //{
        //    return from user in Context.Users
        //           where (from uo in Context.UserOrgs
        //                  where orgIds.Contains(uo.OrgId)
        //                  select uo.UserId).Contains(user.Id)
        //           select user;
        //}

        /// <summary>
        /// 查询 -通过名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IQueryable<User> FindByName(string name)
        {
            return Find(ub => ub.SignName == name);
        }

        /// <summary>
        /// 删除 -通过用户ID
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        public Task<IEnumerable<User>> DeleteById(string id)
        {
            // 打印日志
            Logger.Trace($"[{nameof(DeleteById)}] 条件删除用户({id})");
            return Delete(ub => ub.Id == id);
        }

        /// <summary>
        /// 用户(userIds)删除用户(uId)
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="uId">被删除用户ID</param>
        /// <returns></returns>
        public async Task DeleteByUserId(string userId, string uId)
        {
            using(var trans = await Context.Database.BeginTransactionAsync())
            {
                try
                {
                    // 1. 删除用户角色关联（附带删除用户用户组织权限关联）
                    var userRoles = await (from ur in Context.Set<UserRole>()
                                           where ur.UserId == uId
                                           select ur).AsNoTracking().ToListAsync();
                    foreach (var userRole in userRoles)
                    {
                        await UserRoleStore.DeleteByUserId(userId, userRole.UserId, userRole.RoleId);
                    }
                    // 2. 删除用户组织关联
                    var userorgs = from uo in Context.Set<UserOrg>()
                                   where uo.UserId == uId
                                   select uo;
                    Context.AttachRange(userorgs);
                    Context.RemoveRange(userorgs);
                    // 2. 删除自身
                    var users = from user in Context.Set<User>()
                                where user.Id == uId
                                select user;
                    Context.AttachRange(users);
                    Context.RemoveRange(users);
                    await Context.SaveChangesAsync();
                    trans.Commit();
                }
                catch (Exception e)
                {
                    Logger.Error($"[{nameof(DeleteByUserId)}] 用户({userId})删除用户({uId})失败:\r\n{e}");
                    trans.Rollback();
                    throw new Exception($"用户({userId})删除用户({uId})失败", e);
                }
            }
        }

        /// <summary>
        /// 删除符合条件的组织的所有用户
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task DeleteByUserIdOrgId(string userId, Func<Organization, bool> predicate)
        {
            var userIds = await (from uo in Context.Set<UserOrg>()
                                where (from o in Context.Set<Organization>()
                                       where predicate(o)
                                       select o.Id).Contains(uo.OrgId)
                                select uo.UserId).AsNoTracking().ToListAsync();
            await DeleteByUserId(userId, userIds);
        }

        /// <summary>
        /// 删除通过用户ID
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="uIds">被删除用户ID</param>
        /// <returns></returns>
        public async Task DeleteByUserId(string userId, IEnumerable<string> uIds)
        {
            foreach(var uId in uIds)
            {
                await DeleteByUserId(userId, uId);
            }
        }

        /// <summary>
        /// 删除 -通过用户名
        /// </summary>
        /// <param name="name">用户名</param>
        /// <returns></returns>
        public Task<IEnumerable<User>> DeleteByName(string name)
        {
            // 打印日志
            Logger.Trace($"[{nameof(DeleteByName)}] 条件删除用户({name})");
            return Delete(ub => ub.SignName == name);
        }
    }
}
