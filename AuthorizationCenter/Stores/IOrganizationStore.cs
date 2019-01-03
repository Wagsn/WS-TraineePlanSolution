using AuthorizationCenter.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Stores
{
    /// <summary>
    /// 组织存储
    /// </summary>
    public interface IOrganizationStore : INameStore<Organization>
    {

        /// <summary>
        /// 通过组织ID找到所有子组织（包括间接子组织，但是不包括自身）
        /// </summary>
        /// <param name="id">组织ID</param>
        /// <returns></returns>
        Task<List<Organization>> FindChildrenById(string id);

        /// <summary>
        /// 查询所有父组织
        /// </summary>
        /// <param name="id">组织ID</param>
        /// <returns></returns>
        Task<List<Organization>> FindParentById(string id);
    }
}
