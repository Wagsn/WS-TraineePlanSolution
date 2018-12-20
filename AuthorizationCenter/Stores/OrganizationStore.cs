
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
