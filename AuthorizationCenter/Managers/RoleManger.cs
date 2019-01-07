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
        /// 类型映射
        /// </summary>
        IMapper Mapper { get; set; }

        /// <summary>
        /// 日志器
        /// </summary>
        readonly ILogger Logger = LoggerManager.GetLogger(nameof(RoleManger));

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="store"></param>
        /// <param name="mapper"></param>
        /// <param name="userRoleStore"></param>
        /// <param name="roleOrgStore"></param>
        public RoleManger(IRoleStore store, IMapper mapper, IUserRoleStore userRoleStore, IRoleOrgStore roleOrgStore)
        {
            Store = store;
            Mapper = mapper;
            UserRoleStore = userRoleStore;
            RoleOrgStore = roleOrgStore;
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
        /// 新增角色 -通过组织用户ID（UID->OID->RID）
        /// 将角色添加到用户所在组织上
        /// </summary>
        /// <param name="json">新增角色</param>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public async Task CreateByOrgUserId(RoleJson json, string userId)
        {
            // 1. 创建角色
            string roleId = Guid.NewGuid().ToString();
            json.Id = roleId;
            await Store.Create(Mapper.Map<Role>(json));
            // 2. 通过用户ID查询组织ID
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
        /// 查询通过用户ID
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<RoleJson>> FindByUserId(string userId)
        {
            return await Store.FindByUserId(userId).Select(r => Mapper.Map<RoleJson>(r)).ToListAsync();
        }

        /// <summary>
        /// 查询通过用户ID(UID->OID->RID)
        /// 查询用户ID所在组织的所有角色（包含子组织）
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public async Task<IEnumerable<RoleJson>> FindByOrgUserId(string userId)
        {
            //// 1. 查询组织
            //var orgId = (await UserOrgStore.Find(uo => uo.UserId = userId).SingleAsync()).OrgId;

            //// 2. 查询角色
            //var roleIds = await RoleOrgStore.Find(ro => ro.OrgId == orgId).Select(ro => ro.RoleId).ToListAsync();
            //var roles = await Store.Find(role => roleIds.Contains(role.Id)).Select(role => Mapper.Map<RoleJson>(role)).ToListAsync();

            // 判断角色查询权限 -找出可以查询的组织森林（角色组织权限），查询其所有角色

            // 1. 查询用户具有角色查询权限的组织森林，并扩展成组织列表
            // 2. 查询这些所有组织所包含的角色

            var roles = await (from role in Store.Context.Roles
                         where (from ro in Store.Context.RoleOrgs
                                where (from uo in Store.Context.UserOrgs
                                       where uo.UserId == userId
                                       select uo.OrgId).Contains(ro.OrgId)  // 1. 查询组织
                                select ro.RoleId).Contains(role.Id)  
                         select role).Select(role => Mapper.Map<RoleJson>(role)).ToListAsync();  // 2. 查询角色
            return roles;
        }

        //public Task<IEnumerable<RoleJson>> Fin

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
