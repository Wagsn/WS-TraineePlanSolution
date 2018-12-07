using AuthorizationCenter.Dto.Jsons;
using AuthorizationCenter.Dto.Requests;
using AuthorizationCenter.Models;
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
    /// <typeparam name="IStore">存储</typeparam>
    /// <typeparam name="TJson">Dto数据分离，映射模型</typeparam>
    public interface IRoleManager<IStore, TJson> where IStore : IRoleStore<Role> where TJson : RoleJson
    {
        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="response">响应</param>
        /// <param name="request">请求</param>
        Task Create([Required]ResponseMessage<TJson> response, [Required]ModelRequest<TJson> request);

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
