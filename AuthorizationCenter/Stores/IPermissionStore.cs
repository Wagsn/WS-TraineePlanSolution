using AuthorizationCenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Stores
{
    /// <summary>
    /// 权限存储
    /// </summary>
    public interface IPermissionStore<TModel> where TModel : Permission
    {
        /// <summary>
        /// 查询 根据ID
        /// </summary>
        /// <param name="user"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        IQueryable<TModel> ById(UserBase user, string id);

        /// <summary>
        /// 查询 通过名称
        /// </summary>
        /// <param name="user"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        IQueryable<TModel> ByName(UserBase user, string name);

        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="user"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        TModel Create(UserBase user, TModel model);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="user"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        TModel Update(UserBase user, TModel model);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="user"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        TModel Delete(UserBase user, string id);
    }
}
