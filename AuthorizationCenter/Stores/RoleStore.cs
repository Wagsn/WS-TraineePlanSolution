using AuthorizationCenter.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WS.Log;

namespace AuthorizationCenter.Stores
{
    /// <summary>
    /// 角色存储
    /// </summary>
    public class RoleStore : StoreBase<Role>, IRoleStore
    {
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="context"></param>
        public RoleStore(ApplicationDbContext context)
        {
            Context = context;
            Logger = LoggerManager.GetLogger(GetType().Name);
        }

        /// <summary>
        /// 通过ID删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<IQueryable<Role>> DeleteById(string id)
        {
            return Delete(role => role.Id == id);
        }

        /// <summary>
        /// 用户(userId)删除角色(id)
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="id">被删除角色ID</param>
        /// <returns></returns>
        public async Task DeleteByUserId(string userId, string id)
        {
            using(var trans = await Context.Database.BeginTransactionAsync())
            {
                try
                {
                    // 1. 删除关联实体
                    // 1.1 删除角色组织实体
                    var roleOrgs = from ro in Context.Set<RoleOrg>()
                                   where ro.RoleId == id
                                   select ro;
                    Context.AttachRange(roleOrgs);
                    Context.RemoveRange(roleOrgs);
                    // 1.2 删除角色权限实体
                    var roleOrgPers = from rop in Context.Set<RoleOrgPer>()
                                      where rop.RoleId == id
                                      select rop;
                    Context.AttachRange(roleOrgPers);
                    Context.RemoveRange(roleOrgPers);
                    // 1.3 删除用户角色实体
                    var userRoles = from ur in Context.Set<UserRole>()
                                    where ur.RoleId == id
                                    select ur;
                    Context.AttachRange(userRoles);
                    Context.RemoveRange(userRoles);
                    // 2. 删除角色实体
                    var roles = from role in Context.Set<Role>()
                                where role.Id == id
                                select role;
                    Context.AttachRange(roles);
                    Context.RemoveRange(roles);
                    await Context.SaveChangesAsync();
                    trans.Commit();
                }
                catch(Exception e)
                {
                    Logger.Error($"[{nameof(DeleteByUserId)}] 删除角色失败:\r\n{e}");
                    trans.Rollback();
                    throw new Exception("删除角色失败", e);  // 因为删除失败，避免上层代码继续执行，将异常抛出
                }
            }

        }

        /// <summary>
        /// 通过名称删除
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Task<IQueryable<Role>> DeleteByName(string name)
        {
            return Delete(role => role.Name == name);
        }

        /// <summary>
        /// 通过ID查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IQueryable<Role> FindById(string id)
        {
            return Find(role => role.Id == id);
        }

        /// <summary>
        /// 通过名称查询
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IQueryable<Role> FindByName(string name)
        {
            return Find(role => role.Name == name);
        }

        /// <summary>
        /// 通过用户ID查询角色
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IQueryable<Role> FindByUserId(string userId)
        {
            return from r in Context.Roles
                   where (from ur in Context.UserRoles
                          where ur.UserId == userId
                          select ur.RoleId).Contains(r.Id)
                   select r;


            //var query = from r in Find()
            //            where (from ur in UserRoleStore.Find()
            //                   where ur.UserId == id
            //                   select ur.RoleId).Contains(r.Id)
            //            select Mapper.Map<RoleJson>(r);
            //return query;

            // 这里是两条语句，分别SQL之后再在程序中执行关联
            //return UserRoleStore.Find(it => it.UserId == id).Join(Store.Context.Roles, a => a.RoleId, b => b.Id, (a, b) => b).Select(r => Mapper.Map<RoleJson>(r));
            ;
            //var roleids = UserRoleStore.Context.UserRoles.Where(ur => ur.UserId == id).Select(ur => ur.RoleId);
            //return Store.Find(r => roleids.Contains(r.Id)).Select(r => Mapper.Map<RoleJson>(r));
        }
        /// <summary>
        /// 查询通过组织ID
        /// </summary>
        /// <param name="orgId">组织ID</param>
        /// <returns></returns>
        public IEnumerable<Role> FindByOrgId(string orgId)
        {
            return from r in Context.Roles
                   where (from ro in Context.RoleOrgs
                          where ro.OrgId == orgId
                          select ro.RoleId).Contains(r.Id)
                   select r;
        }
    }
}
