﻿using AuthorizationCenter.Dto.Jsons;
using AuthorizationCenter.Dto.Requests;
using AuthorizationCenter.Models;
using AuthorizationCenter.Stores;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using WS.Core.Dto;

namespace AuthorizationCenter.Managers
{
    /// <summary>
    /// 用户管理
    /// </summary>
    /// <typeparam name="IStore">存储</typeparam>
    /// <typeparam name="TJson">Dto数据隔离映射模型</typeparam>
    public interface IUserManager<IStore, TJson> where IStore : IUserBaseStore where TJson: UserBaseJson
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

        /// <summary>
        /// 存储
        /// </summary>
        IStore Store { get; set; }

        /// <summary>
        /// 检查用户密码是否正确
        /// </summary>
        /// <param name="user"></param>
        Task<bool> Check(TJson user);

        /// <summary>
        /// 是否存在 每个属性都要匹配（null忽略）
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        bool IsExistForName(TJson user);

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
        /// 删除
        /// </summary>
        /// <param name="response">响应</param>
        /// <param name="request">请求</param>
        /// <returns></returns>
        Task Delete([Required]ResponseMessage<TJson> response, [Required]ModelRequest<TJson> request);
    }
}
