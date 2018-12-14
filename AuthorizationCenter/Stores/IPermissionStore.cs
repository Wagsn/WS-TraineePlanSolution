using AuthorizationCenter.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Stores
{
    /// <summary>
    /// 权限存储
    /// </summary>
    public interface IPermissionStore : IStore<Permission> { }
}
