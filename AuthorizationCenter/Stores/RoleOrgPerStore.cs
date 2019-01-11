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
        /// 组织存储 -NOTE：小心循环调用
        /// </summary>
        public IOrganizationStore OrganizationStore { get; set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="context"></param>
        public RoleOrgPerStore(ApplicationDbContext context, IOrganizationStore organizationStore)
        {
            Context = context;
            OrganizationStore = organizationStore;
            Logger = LoggerManager.GetLogger<RoleOrgPerStore>();
        }

        /// <summary>
        /// 查询用户拥有某项权限（用户可能拥有其父级权限）的所有组织
        /// 如果用户拥有的权限是在该操作权限之上 ROOT > USER_MANAGE > USER_QUERY
        /// 有权组织列表获取，通过用户ID和权限名称获取组织列表(U.ID-[UR]->R.ID, P.N-[P]->P.ID-[P]->P.ID)-[ROP]->O.ID-[O]->O.ID
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="perName">权限名称</param>
        /// <returns></returns>
        public async Task<IEnumerable<Organization>> FindOrgByUserIdPerName(string userId, string perName)
        {
            // 1. 查询该操作的所有权限（包含父级权限）
            // 1.1 通过权限名称查询权限ID
            var perId = (from per in Context.Permissions
                         where per.Name == perName
                         select per.Id).Single();
            // 1.2 查询权限ID的所有父级权限构成权限ID集合（包含自身）
            var perIds = (await FindParentById(perId)).Select(per => per.Id);
            // 2. 查询用户包含的角色Id列表
            var roleIds = from ur in Context.UserRoles
                          where ur.UserId == userId
                          select ur.RoleId;
            // 3. 通过权限ID集合和角色ID集合查询有权根组织ID集合
            var orgs = await (from org in Context.Organizations
                              where (from rop in Context.RoleOrgPers
                                     where perIds.Contains(rop.PerId) && roleIds.Contains(rop.RoleId) // 通过权限和角色查询组织
                                     select rop.OrgId).Contains(org.Id)
                              select org).ToListAsync();
            // 4. 扩展成组织列表
            var orgList = new List<Organization>();
            orgList.AddRange(orgs);
            foreach (var org in orgs)
            {
                orgList.AddRange(await OrganizationStore.FindChildren(org));
            }
            return orgList.Distinct();
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
