using AuthorizationCenter.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AuthorizationCenter.Stores
{
    /// <summary>
    /// 用户存储
    /// </summary>
    public interface IUserBaseStore : IStore<UserBase>
    {
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IQueryable<UserBase> FindById(string id);

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="id"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        IQueryable<TProperty> FindById<TProperty>(string id, Func<UserBase, TProperty> map);

        /// <summary>
        /// 查询 通过用户名查询用户
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IQueryable<UserBase> FindByName(string name);

        /// <summary>
        /// 查询 通过用户名查询用户
        /// </summary>
        /// <param name="name"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        IQueryable<TProperty> FindByName<TProperty>(string name, Func<UserBase, TProperty> map);

        /// <summary>
        /// 删除 删除用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IQueryable<UserBase>> DeleteById(string id);
    }
}
