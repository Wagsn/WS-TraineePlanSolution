
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
    /// 组织存储
    /// </summary>
    public class OrganizationStore : StoreBase<Organization>, IOrganizationStore
    {
        /// <summary>
        /// 组织存储
        /// </summary>
        /// <param name="context"></param>
        public OrganizationStore(ApplicationDbContext context)
        {
            Context = context;
            Logger = LoggerManager.GetLogger(GetType().Name);
        }

        /// <summary>
        /// 删除通过ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<IQueryable<Organization>> DeleteById(string id)
        {
            return Delete(org => org.Id == id);
        }

        /// <summary>
        /// 删除通过名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Task<IQueryable<Organization>> DeleteByName(string name)
        {
            return Delete(org => org.Name == name);
        }

        /// <summary>
        /// 查询通过ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IQueryable<Organization> FindById(string id)
        {
            return Find(org => org.Id == id).Include(org => org.Children);
        }

        /// <summary>
        /// 查询通过用户ID在UserOrg表中
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public IQueryable<Organization> FindByUserId(string userId)
        {
            return from org in Context.Organizations
                   where (from uo in Context.UserOrgs
                          where true
                          select uo.OrgId).Contains(org.Id)
                   select org;
        }

        /// <summary>
        /// 通过组织ID找到所有子组织（包含自身）
        /// </summary>
        /// <param name="orgId">组织ID</param>
        /// <returns></returns>
        public async Task<List<Organization>> FindChildrenById(string orgId)
        {
            List<Organization> result = new List<Organization>();
            //if(orgId == null)
            //{
            //    return result;
            //}
            var org = await Find(o => o.Id == orgId).SingleAsync();
            result.Add(org);
            // 根据 orgid查询其所有直接子组织
            var orgs = await Find(o => orgId == o.ParentId).ToListAsync();
            // 遍历其直接子组织
            foreach (var o in orgs)
            {
                result.AddRange(await FindChildrenById(o.Id));
            }
            return result;
        }

        /// <summary>
        /// 通过组织找到所有子组织（不包含自身）
        /// </summary>
        /// <param name="organization">组织</param>
        /// <returns></returns>
        public async Task<List<Organization>> FindChildren(Organization organization)
        {
            List<Organization> result = new List<Organization>();
            //if(org == null)
            //{
            //    return result;
            //}
            // 根据 orgid查询其所有直接子组织
            var orgs = await Find(org => organization.Id == org.ParentId).ToListAsync();
            result.AddRange(orgs);
            // 遍历其直接子组织
            foreach (var org in orgs)
            {
                result.AddRange(await FindChildren(org));
            }
            return result;
        }

        /// <summary>
        /// 递归查询所有节点，构成一棵树返回
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public Organization FindTreeById(string orgId)
        {
            var org = (from o in Find()
                        where o.Id == orgId
                        select o).Include(o => o.Children).SingleOrDefault();
            if(org == null)
            {
                return null;
            }
            for (int i = 0; i < org.Children.Count; i++)
            {
                org.Children[i] = FindTreeById(org.Children[i].Id);
            }
            return org;
        }

        /// <summary>
        /// 递归查询子组织集合
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public async Task<List<Organization>> RecursionChildren(string orgId)
        {
            List<Organization> result = new List<Organization>();
            // 根据 orgid查询其所有直接子组织
            var orgs = await Find(o => orgId == o.ParentId).ToListAsync();
            // 将直接子组织加入结果集
            result.AddRange(orgs);
            // 遍历其直接子组织
            foreach(var org in orgs)
            {
                result.AddRange(await RecursionChildren(org.Id));
            }
            return result;
        }

        /// <summary>
        /// 查询通过名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IQueryable<Organization> FindByName(string name)
        {
            return Find(org => org.Id == name);
        }

        /// <summary>
        /// 查询所有父组织
        /// </summary>
        /// <param name="id">组织ID</param>
        /// <returns></returns>
        public async Task<List<Organization>> FindParentById(string id)
        {
            List<Organization> result = new List<Organization>();
            var org = Context.Set<Organization>().Where(o => o.Id == id).Single();
            while (org.ParentId != null)
            {
                var temp = await Context.Set<Organization>().Where(o => o.Id == org.ParentId).SingleOrDefaultAsync();
                result.Add(temp);
                org = temp;
            }
            return result;
        }
    }
}
