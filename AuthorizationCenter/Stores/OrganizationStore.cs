
using AuthorizationCenter.Entitys;
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
            return Find(org => org.Id == id);
        }

        /// <summary>
        /// 通过组织ID找到所有子组织（包括间接子组织children.children）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IQueryable<Organization> FindChildrenById(string id)
        {
            
            return null;
        }

        /// <summary>
        /// 递归查询
        /// </summary>
        /// <param name="organization"></param>
        /// <returns></returns>
        public List<Organization> RecursionOrganizations(Organization organization)
        {
            List<Organization> result = new List<Organization>();
            // 根据 orgid查询其所有直接子组织
            var orgs = Find(o => organization.Id == o.ParentId).ToList();
            // 遍历其直接子组织
            foreach(var org in orgs)
            {
                result.AddRange(RecursionOrganizations(org));
            }
            return result;
        }

        /// <summary>
        /// 递归查询
        /// </summary>
        /// <param name="result"></param>
        /// <param name="organization"></param>
        /// <returns></returns>
        public void RecursionOrganizations(List<Organization> result, Organization organization)
        {
            // 根据 orgid查询其所有直接子组织
            var orgs = Find(o => organization.Id == o.ParentId).ToList();
            // 遍历其直接子组织
            foreach (var org in orgs)
            {
                result.AddRange(RecursionOrganizations(org));
            }
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
    }
}
