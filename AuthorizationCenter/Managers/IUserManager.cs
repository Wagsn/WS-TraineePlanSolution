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
    /// 用户管理
    /// </summary>
    /// <typeparam name="IStore">存储</typeparam>
    /// <typeparam name="TJson">Dto数据隔离映射模型</typeparam>
    public interface IUserManager<IStore, TJson> where IStore : IUserBaseStore<UserBase> where TJson: UserBaseJson
    {
        ///// <summary>
        ///// 查询 或运算 满足条件的都查询
        ///// </summary>
        ///// <param name="response">响应</param>
        ///// <param name="request">请求</param>
        //Task GetOr([Required]ResponseMessage<TJson> response, [Required]ModelRequest<TJson> request);

        ///// <summary>
        ///// 查询 与运算 全部条件满足的查询
        ///// </summary>
        ///// <param name="response">响应</param>
        ///// <param name="request">请求</param>
        //Task GetAnd([Required]ResponseMessage<TJson> response, [Required]ModelRequest<TJson> request);

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
