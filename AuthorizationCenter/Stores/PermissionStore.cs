using AuthorizationCenter.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WS.Log;

namespace AuthorizationCenter.Stores
{
    /// <summary>
    /// 权限存储
    /// </summary>
    public class PermissionStore : StoreBase<Permission>, INameStore<Permission>, IPermissionStore
    {
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="context"></param>
        public PermissionStore(ApplicationDbContext context):base(context){}

        /// <summary>
        /// 通过ID删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<IEnumerable<Permission>> DeleteById(string id)
        {
            return Delete(perm => perm.Id == id);
        }

        /// <summary>
        /// 通过名称删除
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Task<IEnumerable<Permission>> DeleteByName(string name)
        {
            return Delete(per => per.Name == name);
        }

        /// <summary>
        /// 通过ID查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IQueryable<Permission> FindById(string id)
        {
            return Find(per => per.Id == id);
        }

        /// <summary>
        /// 通过名称查询
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IQueryable<Permission> FindByName(string name)
        {
            return Find(per => per.Name == name);
        }
    }
}
