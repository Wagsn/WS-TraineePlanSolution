using AuthorizationCenter.Dto.Jsons;
using AuthorizationCenter.Dto.Requests;
using AuthorizationCenter.Entitys;
using AuthorizationCenter.Stores;
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
    /// <typeparam name="TJson">Dto数据分离，映射模型</typeparam>
    public interface IRoleManager<TJson> where TJson : RoleJson
    {
        ///// <summary>
        ///// 新建 API
        ///// </summary>
        ///// <param name="response">响应</param>
        ///// <param name="request">请求</param>
        //Task Create([Required]ResponseMessage<TJson> response, [Required]ModelRequest<TJson> request);

        ///// <summary>
        ///// 更新 API
        ///// </summary>
        ///// <param name="response">响应</param>
        ///// <param name="request">请求</param>
        ///// <returns></returns>
        //Task Update([Required]ResponseMessage<TJson> response, [Required]ModelRequest<TJson> request);

        ///// <summary>
        ///// 删除 API
        ///// </summary>
        ///// <param name="response">响应</param>
        ///// <param name="request">请求</param>
        ///// <returns></returns>
        //Task Delete([Required]ResponseMessage<TJson> response, [Required]ModelRequest<TJson> request);

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        Task<TJson> Create(TJson json);

        /// <summary>
        /// 新增通过用户ID
        /// </summary>
        /// <param name="json">新增角色</param>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        Task CreateForOrgByUserId(TJson json, string userId);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        Task<TJson> Update(TJson json);

        ///// <summary>
        ///// 更新
        ///// </summary>
        ///// <param name="prevate"></param>
        ///// <param name="action"></param>
        ///// <returns></returns>
        //Task<IQueryable<TJson>> Update(Func<TJson, bool> prevate, Action<TJson> action);

        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        IQueryable<TJson> Find();
        
        /// <summary>
        /// 条件查询 -异步查询
        /// </summary>
        /// <param name="predicate">表达式</param>
        /// <returns></returns>
        IQueryable<TJson> Find(Func<TJson, bool> predicate);

        /// <summary>
        /// 通过ID查询 -异步查询 -只取第一个 -没有返回空
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<TJson> FindById(string id);

        /// <summary>
        /// 查询通过用户ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IEnumerable<RoleJson>> FindByUserId(string id);

        /// <summary>
        /// 通过用户ID查询组织下的角色
        /// UID-[UO]->OID-[RO]->RID
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IEnumerable<RoleJson>> FindRoleOfOrgByUserId(string userId);

        /// <summary>
        /// 通过Name查询 -异步查询 -只取第一个 -没有返回空
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<TJson> FindByName(string name);

        /// <summary>
        /// 通过ID判断存在 -异步
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        Task<bool> ExistById(string id);

        /// <summary>
        /// 存在 -Lambda表达式
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<bool> Exist(Func<TJson, bool> predicate);

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
