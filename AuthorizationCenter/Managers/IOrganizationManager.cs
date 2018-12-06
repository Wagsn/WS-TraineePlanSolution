using AuthorizationCenter.Dto.Request;
using AuthorizationCenter.Models;
using AuthorizationCenter.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WS.Core.Dto;

namespace AuthorizationCenter.Managers
{
    /// <summary>
    /// 组织管理
    /// </summary>
    /// <typeparam name="IStore"></typeparam>
    /// <typeparam name="TModel"></typeparam>
    public interface IOrganizationManager<IStore, TModel> where IStore : IOrganizationStore<TModel> where TModel : Organization
    {
        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="response"></param>
        /// <param name="request"></param>
        Task Create(ResponseMessage<TModel> response, ModelRequest<TModel> request);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="response"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        Task Update(ResponseMessage<TModel> response, ModelRequest<TModel> request);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="response"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        Task Delete(ResponseMessage<TModel> response, ModelRequest<TModel> request);
    }
}
