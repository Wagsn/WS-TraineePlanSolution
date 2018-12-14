using AuthorizationCenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Stores
{
    /// <summary>
    /// 权限存储
    /// </summary>
    public class PermissionStore : StoreBase<Permission>, IPermissionStore
    {
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public override IQueryable<Permission> ById(string id)
        //{
        //    throw new NotImplementedException();
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="name"></param>
        ///// <returns></returns>
        //public override IQueryable<Permission> ByName(string name)
        //{
        //    throw new NotImplementedException();
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public override Task<IQueryable<Permission>> DeleteById(string id)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
