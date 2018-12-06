using AuthorizationCenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Stores
{
    /// <summary>
    /// 用户存储
    /// </summary>
    public interface IUserBaseStore<TModel> where TModel : UserBase
    {
        /// <summary>
        /// 查询 根据用户ID查询用户
        /// </summary>
        /// <param name="signuser"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        IQueryable<TModel> ById(TModel signuser, string id);

        /// <summary>
        /// 查询 通过用户名查询用户
        /// </summary>
        /// <param name="signuser"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        IQueryable<TModel> ByName(TModel signuser, string name);

        /// <summary>
        /// 新建 创建用户
        /// </summary>
        /// <param name="signuser"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        TModel Create(TModel signuser, TModel user);

        /// <summary>
        /// 更新 更新用户
        /// </summary>
        /// <param name="signuser"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        TModel Update(TModel signuser, TModel user);

        /// <summary>
        /// 删除 删除用户
        /// </summary>
        /// <param name="signuser"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        TModel Delete(TModel signuser, string userid);
    }
}
