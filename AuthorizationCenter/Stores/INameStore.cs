using AuthorizationCenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Stores
{
    /// <summary>
    /// 有Id和Name属性的单实体泛型接口
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface INameStore<TEntity> where TEntity : class
    {
        /// <summary>
        /// 数据库上下文 转移
        /// </summary>
        ApplicationDbContext Context { get; set; }

        /// <summary>
        /// 批量查询
        /// </summary>
        /// <returns></returns>
        IQueryable<TEntity> List();

        /// <summary>
        /// 条件查询
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        IQueryable<TEntity> List(Func<IQueryable<TEntity>, IQueryable<TEntity>> query);

        /// <summary>
        /// 查询 根据用户ID查询用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IQueryable<TEntity> ById(string id);

        /// <summary>
        /// 查询 通过用户名查询用户
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IQueryable<TEntity> ByName(string name);

        /// <summary>
        /// 新建 创建用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<TEntity> Create(TEntity user);

        /// <summary>
        /// 更新 更新用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<TEntity> Update(TEntity user);

        /// <summary>
        /// 删除 删除用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IQueryable<TEntity>> DeleteIfId(string id);
    }
}
