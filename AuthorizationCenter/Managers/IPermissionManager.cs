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
    /// 权限管理
    /// </summary>
    /// <typeparam name="TJson">Dto数据分离，映射模型</typeparam>
    public interface IPermissionManager<TJson> where TJson : PermissionJson
    {
        /// <summary>
        /// 查询所有
        /// </summary>
        /// <returns></returns>
        IQueryable<TJson> Find();

        /// <summary>
        /// 通过ID查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IQueryable<TJson> FindById(string id);

        /// <summary>
        /// 通过ID查询
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IEnumerable<TJson>> FindByUserId(string userId);

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        Task Create(TJson json);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        Task Update(TJson json);

        /// <summary>
        /// 条件存在
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<bool> Exist(Func<TJson, bool> predicate);

        /// <summary>
        /// 通过ID删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IQueryable<TJson>> DeleteById(string id);
    }
}
