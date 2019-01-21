using AuthorizationCenter.Define;
using AuthorizationCenter.Dto.Jsons;
using AuthorizationCenter.Entitys;
using AuthorizationCenter.Stores;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WS.Log;

namespace AuthorizationCenter.Managers
{
    /// <summary>
    /// 角色管理
    /// </summary>
    public class RoleManger : IRoleManager<RoleJson>
    {
        /// <summary>
        /// 角色管理
        /// </summary>
        IRoleStore RoleStore { get; set; }

        /// <summary>
        /// 权限存储
        /// </summary>
        IPermissionStore PermissionStore { get; set; }

        /// <summary>
        /// 组织存储
        /// </summary>
        IOrganizationStore OrganizationStore { get; set; }

        /// <summary>
        /// 用户角色关联存储
        /// </summary>
        IUserRoleStore UserRoleStore { get; set; }

        /// <summary>
        /// 用户组织存储
        /// </summary>
        IUserOrgStore UserOrgStore { get; set; }

        /// <summary>
        /// 角色组织关联存储
        /// </summary>
        IRoleOrgStore RoleOrgStore { get; set; }

        /// <summary>
        /// 角色组织权限存储
        /// </summary>
        IRoleOrgPerStore RoleOrgPerStore { get; set; }

        /// <summary>
        /// 类型映射
        /// </summary>
        IMapper Mapper { get; set; }

        /// <summary>
        /// 日志器
        /// </summary>
        readonly ILogger Logger = LoggerManager.GetLogger<RoleManger>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="store"></param>
        /// <param name="permissionStore"></param>
        /// <param name="organizationStore"></param>
        /// <param name="userRoleStore"></param>
        /// <param name="userOrgStore"></param>
        /// <param name="roleOrgStore"></param>
        /// <param name="roleOrgPerStore"></param>
        /// <param name="mapper"></param>
        public RoleManger(IRoleStore store, IPermissionStore permissionStore, IOrganizationStore organizationStore, IUserRoleStore userRoleStore, IUserOrgStore userOrgStore, IRoleOrgStore roleOrgStore, IRoleOrgPerStore roleOrgPerStore, IMapper mapper)
        {
            RoleStore = store ?? throw new ArgumentNullException(nameof(store));
            PermissionStore = permissionStore ?? throw new ArgumentNullException(nameof(permissionStore));
            OrganizationStore = organizationStore ?? throw new ArgumentNullException(nameof(organizationStore));
            UserRoleStore = userRoleStore ?? throw new ArgumentNullException(nameof(userRoleStore));
            UserOrgStore = userOrgStore ?? throw new ArgumentNullException(nameof(userOrgStore));
            RoleOrgStore = roleOrgStore ?? throw new ArgumentNullException(nameof(roleOrgStore));
            RoleOrgPerStore = roleOrgPerStore ?? throw new ArgumentNullException(nameof(roleOrgPerStore));
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public async Task<RoleJson> Create(RoleJson json)
        {
            return Mapper.Map<RoleJson>(await RoleStore.Create(Mapper.Map<Role>(json)));
        }
        
        /// <summary>
        /// 为某个组织创建某个角色
        /// </summary>
        /// <param name="json"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public async Task CreateByOrgId(RoleJson json, string orgId)
        {
            // 1. 创建角色
            string roleId = Guid.NewGuid().ToString();
            json.Id = roleId;
            await RoleStore.Create(Mapper.Map<Role>(json));
            // 2. 创建角色组织关联
            await RoleOrgStore.Create(new RoleOrg
            {
                Id = Guid.NewGuid().ToString(),
                RoleId = roleId,
                OrgId = orgId
            });
        }

        /// <summary>
        /// 新增角色 -通过组织用户ID（UID-[UO]->OID|RID-->RO）
        /// 将角色添加到用户所在组织上
        /// </summary>
        /// <param name="json">新增角色</param>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public async Task CreateForOrgByUserId(RoleJson json, string userId)
        {
            // 1. 创建角色
            string roleId = Guid.NewGuid().ToString();
            json.Id = roleId;
            await RoleStore.Create(Mapper.Map<Role>(json));
            // 2. 通过用户ID查询组织ID -用户不能有多个组织
            var orgId = (from uo in RoleStore.Context.UserOrgs
                        where uo.UserId == userId
                        select uo.OrgId).Single();
            // TODO：角色名称不能重复
            // 3. 创建角色组织关联
            await RoleOrgStore.Create(new RoleOrg
            {
                Id = Guid.NewGuid().ToString(),
                RoleId = roleId,
                OrgId = orgId
            });
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public Task Delete(RoleJson json)
        {
            return RoleStore.Delete(Mapper.Map<Role>(json));
        }

        /// <summary>
        /// 删除角色(id)
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <returns></returns>
        public Task DeleteById(string roleId)
        {
            return RoleStore.Delete(role => role.Id == roleId);
        }

        /// <summary>
        /// 用户(userId)删除角色(id)
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="id">被删除角色ID</param>
        /// <returns></returns>
        public async Task DeleteByUserId(string userId, string id)
        {
            using(var trans = await RoleStore.Context.Database.BeginTransactionAsync())
            {
                try
                {
                    // 查询所有用户角色关联
                    var userRoles = await UserRoleStore.Find(ur => ur.RoleId == id).AsNoTracking().ToListAsync();
                    // 删除所有用户角色关联
                    foreach (var ur in userRoles)
                    {
                        await UserRoleStore.DeleteByUserId(userId, ur.Id);
                    }
                    // 删除所有用户组织权限关联
                    var roleOrgPers = await RoleOrgPerStore.Delete(rop => rop.RoleId == id);

                    // 删除所有角色组织关联
                    await RoleOrgStore.Delete(ro => ro.RoleId == id);
                    // 删除角色本身
                    await RoleStore.DeleteById(id);
                    trans.Commit();
                }
                catch(Exception e)
                {
                    Logger.Error($"[{nameof(DeleteByUserId)}] 用户({userId})删除角色({id})失败");
                    trans.Rollback();
                    throw new Exception("角色删除失败", e);
                }
            }
            
        }

        /// <summary>
        /// 存在
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Task<bool> Exist(Func<RoleJson, bool> predicate)
        {
            return RoleStore.Exist(role=> predicate(Mapper.Map<RoleJson>(role)));
        }

        /// <summary>
        /// 存在
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public Task<bool> ExistById(string roleId)
        {
            return RoleStore.Exist(role => role.Id == roleId);
        }

        /// <summary>
        /// 存在通过名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Task<bool> ExistByName(string name)
        {
            return RoleStore.Exist(role => role.Name == name);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IQueryable<RoleJson> Find(Func<RoleJson, bool> predicate)
        {
            return  RoleStore.Find(role => predicate(Mapper.Map<RoleJson>(role))).Select(role=>Mapper.Map<RoleJson>(role));
        }

        /// <summary>
        /// 通过ID查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<RoleJson> FindById(string id)
        {
            return RoleStore.Find(role => role.Id == id).Select(role => Mapper.Map<RoleJson>(role)).SingleOrDefaultAsync();
        }

        /// <summary>
        /// 通过用户ID查询绑定的角色
        /// (UID-[UR]->RID)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<RoleJson>> FindByUserId(string userId)
        {
            return await RoleStore.FindByUserId(userId).Select(r => Mapper.Map<RoleJson>(r)).ToListAsync();
        }

        /// <summary>
        /// 查询用户ID所在组织的所有角色（包含子组织的角色）
        /// (((UID-[UR]->RID)|PID)-[ROP]->OID-[RO]->RID)
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public async Task<IEnumerable<RoleJson>> FindRoleOfOrgByUserId(string userId)
        {
            // 1. 查询用户具有角色查询权限的组织森林，并扩展成组织列表
            // 1.1 查询用户权限的组织ID集合
            var orgIds = (await RoleOrgPerStore.FindOrgByUserIdPerName(userId, Constants.ROLE_QUERY)).Select(org => org.Id).ToList();
            // 2. 查询这些所有组织所包含的角色
            // 2.1 查询角色ID集合
            var roleIds = await (from ro in RoleOrgStore.Find()
                                 where orgIds.Contains(ro.OrgId)
                                 select ro.RoleId).ToListAsync();
            // 2.2 查询角色
            var roles = await (from role in RoleStore.Context.Roles
                               where roleIds.Contains(role.Id)
                               select role).AsNoTracking().Select(role => Mapper.Map<RoleJson>(role)).ToListAsync();
            return roles;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Task<RoleJson> FindByName(string name)
        {
            return RoleStore.Find(role => role.Name == name).Select(role => Mapper.Map<RoleJson>(role)).SingleOrDefaultAsync();
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public async Task<RoleJson> Update(RoleJson json)
        {
            return Mapper.Map<RoleJson>(await RoleStore.Update(Mapper.Map<Role>(json)));
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        public IQueryable<RoleJson> Find()
        {
            return RoleStore.Find(role=>true).Select(role=> Mapper.Map<RoleJson>(role));
        }

        ///// <summary>
        ///// 更新
        ///// </summary>
        ///// <param name="prevate"></param>
        ///// <param name="action"></param>
        ///// <returns></returns>
        //public Task<IQueryable<RoleJson>> Update(Func<RoleJson, bool> prevate, Action<RoleJson> action)
        //{
        //    return Store.Update(role=>prevate(Mapper.Map<RoleJson>(role)), role=>actio))
        //    throw new NotImplementedException();
        //}
    }
}
