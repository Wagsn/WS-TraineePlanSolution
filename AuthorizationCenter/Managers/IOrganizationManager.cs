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
    /// 组织管理
    /// </summary>
    /// <typeparam name="TJson">Dto数据分离，映射模型</typeparam>
    public interface IOrganizationManager<TJson> where TJson : OrganizationJson
    {
        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        IQueryable<TJson> Find();

        /// <summary>
        /// 查询通过ID
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        IQueryable<TJson> FindById(string orgId);

        /// <summary>
        /// 通过用户ID和组织ID查询
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="orgId">组织ID</param>
        /// <returns></returns>
        Task<IEnumerable<TJson>> FindByUserIdOrgId(string userId, string orgId);

        /// <summary>
        /// 通过组织ID查询所有子节点，返回组织树
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        Organization FindTreeById(string orgId);

        /// <summary>
        /// 查询通过用户ID -先找角色-再找组织
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        Task<IEnumerable<TJson>> FindPerOrgsByUserId(string userId);

        /// <summary>
        /// 查询用户(userId)所在组织
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        Task<IEnumerable<Organization>> FindFromUserOrgByUserId(string userId);

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
        /// 存在
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<bool> Exist(Func<TJson, bool> predicate);

        /// <summary>
        /// 删除通过ID
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        Task DeleteById(string orgId);
    }
}
