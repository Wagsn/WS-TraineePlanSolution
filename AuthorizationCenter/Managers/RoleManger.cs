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
        IRoleStore Store { get; set; }

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
        readonly ILogger Logger = LoggerManager.GetLogger(nameof(RoleManger));

        public RoleManger(IRoleStore store, IPermissionStore permissionStore, IOrganizationStore organizationStore, IUserRoleStore userRoleStore, IUserOrgStore userOrgStore, IRoleOrgStore roleOrgStore, IRoleOrgPerStore roleOrgPerStore, IMapper mapper)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
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
            return Mapper.Map<RoleJson>(await Store.Create(Mapper.Map<Role>(json)));
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
            await Store.Create(Mapper.Map<Role>(json));
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
            await Store.Create(Mapper.Map<Role>(json));
            // 2. 通过用户ID查询组织ID -用户不能有多个组织
            var orgId = (from uo in Store.Context.UserOrgs
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
            return Store.Delete(Mapper.Map<Role>(json));
        }

        /// <summary>
        /// 条件删除
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public Task DeleteById(string roleId)
        {
            return Store.Delete(role => role.Id == roleId);
        }

        /// <summary>
        /// 存在
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Task<bool> Exist(Func<RoleJson, bool> predicate)
        {
            return Store.Exist(role=> predicate(Mapper.Map<RoleJson>(role)));
        }

        /// <summary>
        /// 存在
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public Task<bool> ExistById(string roleId)
        {
            return Store.Exist(role => role.Id == roleId);
        }

        /// <summary>
        /// 存在通过名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Task<bool> ExistByName(string name)
        {
            return Store.Exist(role => role.Name == name);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IQueryable<RoleJson> Find(Func<RoleJson, bool> predicate)
        {
            return  Store.Find(role => predicate(Mapper.Map<RoleJson>(role))).Select(role=>Mapper.Map<RoleJson>(role));
        }

        /// <summary>
        /// 通过ID查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<RoleJson> FindById(string id)
        {
            return Store.Find(role => role.Id == id).Select(role => Mapper.Map<RoleJson>(role)).SingleOrDefaultAsync();
        }

        /// <summary>
        /// 通过用户ID查询绑定的角色
        /// (UID-[UR]->RID)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<RoleJson>> FindByUserId(string userId)
        {
            return await Store.FindByUserId(userId).Select(r => Mapper.Map<RoleJson>(r)).ToListAsync();
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
            // 1.1 查询用户权限的组织森林根组织ID 
            var rootOrgIds = (await RoleOrgPerStore.FindOrgByUserIdPerName(userId, Constants.ROLE_MANAGE)).Select(org => org.Id);
            // 1.2 扩展成组织列表
            var orgList = new List<Organization>();
            foreach (var orgId in rootOrgIds)
            {
                orgList.AddRange(OrganizationManager.TreeToList(OrganizationStore.FindTreeById(orgId)));
            }
            var orgIds = orgList.Select(org => org.Id).ToList();
            // 2. 查询这些所有组织所包含的角色
            var roles = await (from role in Store.Find()
                               where (from ro in RoleOrgStore.Find()
                                      where orgIds.Contains(ro.OrgId)
                                      select ro.RoleId).Contains(role.Id)
                               select role).Select(role => Mapper.Map<RoleJson>(role)).ToListAsync();
            return roles;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Task<RoleJson> FindByName(string name)
        {
            return Store.Find(role => role.Name == name).Select(role => Mapper.Map<RoleJson>(role)).SingleOrDefaultAsync();
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public async Task<RoleJson> Update(RoleJson json)
        {
            return Mapper.Map<RoleJson>(await Store.Update(Mapper.Map<Role>(json)));
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        public IQueryable<RoleJson> Find()
        {
            return Store.Find(role=>true).Select(role=> Mapper.Map<RoleJson>(role));
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
