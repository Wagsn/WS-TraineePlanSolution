using AuthorizationCenter.Dto.Jsons;
using AuthorizationCenter.Dto.Requests;
using AuthorizationCenter.Entitys;
using AuthorizationCenter.Stores;
using System;
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
        /// <param name="id"></param>
        /// <returns></returns>
        IQueryable<TJson> FindById(string id);

        /// <summary>
        /// 通过组织ID查询所有子节点，返回组织树
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Organization FindChildrenById(string id);

        /// <summary>
        /// 查询通过用户ID -先找角色-再找组织
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IQueryable<TJson> FindByUserId(string id);

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
        Task<bool> Exist(Func<OrganizationJson, bool> predicate);

        /// <summary>
        /// 删除通过ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteById(string id);
    }
}
