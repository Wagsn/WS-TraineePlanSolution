﻿using AuthorizationCenter.Dto.Jsons;
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
    public interface IUserManager<TJson> where TJson: UserBaseJson
    {
        ///// <summary>
        ///// 查询 或运算 满足条件的都查询（null忽略）
        ///// </summary>
        ///// <param name="response">响应</param>
        ///// <param name="request">请求</param>
        //Task GetOr([Required]ResponseMessage<TJson> response, [Required]ModelRequest<TJson> request);

        ///// <summary>
        ///// 查询 与运算 全部条件满足的查询（null忽略）
        ///// </summary>
        ///// <param name="response">响应</param>
        ///// <param name="request">请求</param>
        //Task GetAnd([Required]ResponseMessage<TJson> response, [Required]ModelRequest<TJson> request);

        /// <summary>
        /// 通过ID查询
        /// </summary>
        /// <param name="response"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        Task ById([Required]ResponseMessage<TJson> response, [Required]ModelRequest<TJson> request);

        ///// <summary>
        ///// 存储
        ///// </summary>
        //IStore Store { get; set; }

        /// <summary>
        /// 批量查询
        /// </summary>
        /// <param name="response"></param>
        /// <param name="request"></param>
        Task List([Required]PagingResponseMessage<TJson> response, [Required]ModelRequest<TJson> request);

        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="response">响应</param>
        /// <param name="request">请求</param>
        Task Create([Required]ResponseMessage<TJson>  response, [Required]ModelRequest<TJson> request);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="response">响应</param>
        /// <param name="request">请求</param>
        /// <returns></returns>
        Task Update([Required]ResponseMessage<TJson> response, [Required]ModelRequest<TJson> request);

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        Task<TJson> Create(TJson json);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        Task<TJson> Update(TJson json);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="response">响应</param>
        /// <param name="request">请求</param>
        /// <returns></returns>
        Task Delete([Required]ResponseMessage<TJson> response, [Required]ModelRequest<TJson> request);

        ///// <summary>
        ///// 通过主键查询
        ///// </summary>
        ///// <param name="keys"></param>
        ///// <returns></returns>
        //Task FindByKeys(params string[] keys);

        ///// <summary>
        ///// 查询 -通过字段名组查询
        ///// </summary>
        ///// <typeparam name="TProperty"></typeparam>
        ///// <param name="output"></param>
        ///// <param name="lambda"></param>
        ///// <returns></returns>
        //Task Find<TProperty>(List<TJson> output, Expression<Func<UserBaseJson, TProperty>> lambda);
        //Task<List<TJson>> Find<TProperty>(Expression<Func<UserBaseJson, TProperty>> lambda);

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
    }
}
