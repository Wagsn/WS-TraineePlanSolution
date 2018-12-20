using AuthorizationCenter.Dto.Jsons;
using AuthorizationCenter.Dto.Requests;
using AuthorizationCenter.Entitys;
using AuthorizationCenter.Stores;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WS.Core.Dto;

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
        /// 类型映射
        /// </summary>
        IMapper Mapper { get; set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="store"></param>
        /// <param name="mapper"></param>
        /// <param name="userRoleStore"></param>
        public RoleManger(IRoleStore store, IMapper mapper, IUserRoleStore userRoleStore)
        {
            Store = store;
            Mapper = mapper;
            UserRoleStore = userRoleStore;
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
        /// <param name="id"></param>
        /// <returns></returns>
        public Task DeleteById(string id)
        {
            return Store.Delete(role => role.Id == id);
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
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<bool> ExistById(string id)
        {
            return Store.Exist(role => role.Id == id);
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
        public IQueryable<RoleJson> FindByUserId(string userId)
        {
            return Store.FindByUserId(userId).Select(r => Mapper.Map<RoleJson>(r));

            //var query = from r in Store.Find()
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
        public Task<RoleJson> Update(RoleJson json)
        {
            return Store.Update(Mapper.Map<Role>(json), role => Mapper.Map<RoleJson>(role));
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
