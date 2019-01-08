using AuthorizationCenter.Entitys;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WS.Log;

namespace AuthorizationCenter.Stores
{
    /// <summary>
    /// 角色组织权限关联存储实现
    /// </summary>
    public class RoleOrgPerStore: StoreBase<RoleOrgPer>, IRoleOrgPerStore
    {
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="context"></param>
        public RoleOrgPerStore(ApplicationDbContext context)
        {
            Context = context;
            Logger = LoggerManager.GetLogger(nameof(RoleOrgPerStore));
        }

        /// <summary>
        /// 查询用户拥有某项权限的所有组织
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="perName">权限名称</param>
        /// <returns></returns>
        public async Task<List<Organization>> FindOrgByUserIdPerName(string userId, string perName)
        {
            // 如果用户拥有的权限是在该操作权限之上 ROOT > USER_MANAGE > USER_QUERY
            // 可操作权限组织列表获取，通过用户ID和权限名称获取组织列表(U.ID-[UR]->R.ID, P.N-[P]->P.ID-[P]->P.ID)-[ROP]->O.ID-[O]->O.ID
            // 1. 查询该操作的所有权限（包含父级权限）
            // 1.1 查询角色Id列表
            var roleIds = from ur in Context.UserRoles
                          where ur.UserId == userId
                          select ur.RoleId;
            // 1.2 通过权限名称查询权限ID列表
            var perId = (from per in Context.Permissions
                         where per.Name == perName
                         select per.Id).Single();
            // 1.2.2 扩展权限列表
            var perIds = new List<string>();
            perIds.Add(perId);
            perIds.AddRange((await FindParentById(perId)).Select(per => per.Id));
            // 2. 该用户包含该权限列表的任意一项（即该用户拥有该操作的权限）
            var orgs = await (from org in Context.Organizations
                              where (from rop in Context.RoleOrgPers
                                     where perIds.Contains(rop.PerId) && roleIds.Contains(rop.RoleId)
                                     select rop.OrgId).Contains(org.Id)
                              select org).ToListAsync();
            return orgs;
        }

        /// <summary>
        /// 查询所有上级权限（包含自身）
        /// </summary>
        /// <param name="perId">权限ID</param>
        /// <returns></returns>
        public async Task<List<Permission>> FindParentById(string perId)
        {
            var perList = new List<Permission>();
            if (perId == null)
            {
                return perList;
            }
            var per = await (from p in Context.Permissions
                             where p.Id == perId
                             select p).Include(p => p.Parent).SingleAsync();
            perList.Add(per);
            perList.AddRange(await FindParentById(per.ParentId));
            return perList;
        }
    }
}
