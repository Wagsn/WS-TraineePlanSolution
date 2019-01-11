using AuthorizationCenter.Dto.Jsons;
using AuthorizationCenter.Dto.Requests;
using AuthorizationCenter.Entitys;
using AuthorizationCenter.Stores;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WS.Core.Dto;

namespace AuthorizationCenter.Managers
{
    /// <summary>
    /// 用户管理
    /// </summary>
    public interface IUserManager<TJson> where TJson: UserJson
    {
        /// <summary>
        /// 通过ID查询
        /// </summary>
        /// <param name="response"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        Task ById([Required]ResponseMessage<TJson> response, [Required]ModelRequest<TJson> request);

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        Task<TJson> Create(TJson json);

        /// <summary>
        /// 用户在自己的组织下创建用户
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="json">用户</param>
        /// <returns></returns>
        Task<TJson> CreateToOrgByUserId(string userId, TJson json);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        Task<TJson> Update(TJson json);

        /// <summary>
        /// 查询所有 用户
        /// </summary>
        /// <returns></returns>
        IQueryable<TJson> Find();

        /// <summary>
        /// 条件查询 -异步查询
        /// </summary>
        /// <param name="func">表达式</param>
        /// <returns></returns>
        IQueryable<TJson> Find(Func<TJson, bool> func);

        /// <summary>
        /// 通过ID查询 -异步查询 -只取第一个 -没有返回空
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IQueryable<TJson> FindById(string id);

        /// <summary>
        /// 通过用户ID查询有权查看的用户列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        Task<IEnumerable<TJson>> FindByUserId(string userId);
        
        /// <summary>
        /// 通过Name查询 -异步查询 -只取第一个 -没有返回空
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IQueryable<TJson> FindByName(string name);

        /// <summary>
        /// 通过ID判断存在 -异步
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        Task<bool> ExistById(string id);

        /// <summary>
        /// 存在 -Lambda表达式
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        Task<bool> Exist(Func<TJson, bool> func);

        /// <summary>
        /// 存在Name -异步
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<bool> ExistByName(string name);

        /// <summary>
        /// 删除 -异步
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        Task Delete(TJson json);

        /// <summary>
        /// 通过ID删除 -异步
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        Task DeleteById(string id);

        /// <summary>
        /// 删除通过用户ID
        /// </summary>
        /// <param name="userId">登陆用户ID</param>
        /// <param name="id">删除用户ID</param>
        /// <returns></returns>
        Task DeleteByUserId(string userId, string id);
    }
}
