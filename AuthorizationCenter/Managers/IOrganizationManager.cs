using AuthorizationCenter.Dto.Jsons;
using AuthorizationCenter.Dto.Requests;
using AuthorizationCenter.Models;
using AuthorizationCenter.Stores;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using WS.Core.Dto;

namespace AuthorizationCenter.Managers
{
    /// <summary>
    /// 组织管理
    /// </summary>
    /// <typeparam name="IStore">存储</typeparam>
    /// <typeparam name="TJson">Dto数据分离，映射模型</typeparam>
    public interface IOrganizationManager<IStore, TJson> where IStore : IOrganizationStore<Organization> where TJson : OrganizationJson
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
