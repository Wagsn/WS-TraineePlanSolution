using AuthorizationCenter.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Managers
{
    /// <summary>
    /// 管理接口 TODO
    /// </summary>
    public interface IManager<TStore, TEntity, TJson> where TStore : IStore<TEntity> where TEntity : class
    {
        // CRUD

    }
}
