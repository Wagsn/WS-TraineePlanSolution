using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Stores
{
    /// <summary>
    /// 存储 -有ID有名实体 -基于IStore
    /// </summary>
    public interface INameStore<TEntity> : IStore<TEntity> where TEntity : class
    {
        /// <summary>
        /// 查询 -通过ID查询
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        IQueryable<TEntity> FindById(string id);

        /// <summary>
        /// 查询 -通过名称查询
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns></returns>
        IQueryable<TEntity> FindByName(string name);

        /// <summary>
        /// 删除 -通过ID删除
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> DeleteById(string id);

        /// <summary>
        /// 删除 -通过名称删除
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> DeleteByName(string name);
    }
}
