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
    /// 角色存储
    /// </summary>
    public class RoleStore : StoreBase<Role>, IRoleStore
    {

        /// <summary>
        /// 用户角色存储
        /// </summary>
        IUserRoleStore UserRoleStore { get; set; }

        /// <summary>
        /// 角色组织权限存储
        /// </summary>
        IRoleOrgPerStore RoleOrgPerStore { get; set; }


        IRoleOrgStore RoleOrgStore { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userRoleStore"></param>
        /// <param name="roleOrgPerStore"></param>
        /// <param name="roleOrgStore"></param>
        public RoleStore(ApplicationDbContext context, IUserRoleStore userRoleStore, IRoleOrgPerStore roleOrgPerStore, IRoleOrgStore roleOrgStore):base(context)
        {
            UserRoleStore = userRoleStore ?? throw new ArgumentNullException(nameof(userRoleStore));
            RoleOrgPerStore = roleOrgPerStore ?? throw new ArgumentNullException(nameof(roleOrgPerStore));
            RoleOrgStore = roleOrgStore ?? throw new ArgumentNullException(nameof(roleOrgStore));
        }


        /// <summary>
        /// 通过ID删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<IEnumerable<Role>> DeleteById(string id)
        {
            return Delete(role => role.Id == id);
        }

        /// <summary>
        /// 用户(userId)删除角色(id)
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="rId">被删除角色ID</param>
        /// <returns></returns>
        public async Task DeleteByUserId(string userId, string rId)
        {
            using(var trans = await Context.Database.BeginTransactionAsync())
            {
                try
                {
                    // 删除所有用户角色关联
                    await UserRoleStore.DeleteByUserId(userId, ur => ur.RoleId == rId);
                    // 删除所有角色组织权限关联
                    await RoleOrgPerStore.Delete(rop => rop.RoleId == rId);
                    // 删除所有角色组织关联
                    await RoleOrgStore.Delete(ro => ro.RoleId == rId);
                    // 删除角色本身
                    await DeleteById(rId);
                    trans.Commit();
                }
                catch(Exception e)
                {
                    Logger.Error($"[{nameof(DeleteByUserId)}] 用户({userId})删除角色({rId})失败:\r\n{e}");
                    trans.Rollback();
                    throw new Exception($"用户({userId})删除角色({rId})失败", e);  // 因为删除失败，避免上层代码继续执行，将异常抛出
                }
            }
        }

        /// <summary>
        /// 用户(userId)条件(predicate)删除角色
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task DeleteByUserId(string userId, Func<Role, bool> predicate)
        {
            var roles = await Find(predicate).AsNoTracking().ToListAsync();
            foreach(var role in roles)
            {
                await DeleteByUserId(userId, role.Id);
            }
        }

        /// <summary>
        /// 用户(userId)条件(predicate)删除角色
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task DeleteByUserIdOrgId(string userId, Func<Organization, bool> predicate)
        {
            var roleIds = await (from ro in Context.Set<RoleOrg>()
                                 where (from o in Context.Set<Organization>()
                                        where predicate(o)
                                        select o.Id).Contains(ro.OrgId)
                                 select ro.RoleId).AsNoTracking().ToListAsync();
            //await DeleteByUserId(userId, role => roleIds.Contains(role.Id));
            foreach(var roleId in roleIds)
            {
                await DeleteByUserId(userId, roleId);
            }
        }

        /// <summary>
        /// 通过名称删除
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Task<IEnumerable<Role>> DeleteByName(string name)
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
        public IQueryable<Role> FindByOrgId(string orgId)
        {
            return from r in Context.Set<Role>()
                   where (from ro in Context.Set<RoleOrg>()
                          where ro.OrgId == orgId
                          select ro.RoleId).Contains(r.Id)
                   select r;
        }
    }
}
