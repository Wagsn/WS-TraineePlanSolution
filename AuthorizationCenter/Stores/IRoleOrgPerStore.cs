using AuthorizationCenter.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Stores
{
    /// <summary>
    /// 角色组织权限关联存储接口
    /// </summary>
    public interface IRoleOrgPerStore: IStore<RoleOrgPer>
    {
        /// <summary>
        /// 查询用户有权限的所有组织
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="perName">权限名称</param>
        /// <returns></returns>
        Task<List<Organization>> FindOrgByUserIdPerName(string userId, string perName);
    }
}
