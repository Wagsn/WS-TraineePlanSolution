using AuthorizationCenter.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Stores
{
    /// <summary>
    /// 角色存储
    /// </summary>
    public class RoleStore : StoreBase<Role>, IRoleStore
    {
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
        /// 通过ID查询
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="id"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        public IQueryable<TProperty> FindById<TProperty>(string id, Func<Role, TProperty> map)
        {
            return Find(role => role.Id == id, role => map(role));
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
        /// 通过名称查询
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="name"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        public IQueryable<TProperty> FindByName<TProperty>(string name, Func<Role, TProperty> map)
        {
            return Find(role => role.Name == name, role => map(role));
        }
    }
}
