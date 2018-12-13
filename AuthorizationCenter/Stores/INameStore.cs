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
    public interface IStore<TEntity> where TEntity : class
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
        /// <param name="predicate"></param>
        /// <returns></returns>
        IQueryable<TEntity> List(Func<TEntity, bool> predicate);

        /// <summary>
        /// 新建 创建用户
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<TEntity> Create(TEntity entity);

        /// <summary>
        /// 更新 更新用户
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<TEntity> Update(TEntity entity);
        
        /// <summary>
        /// 条件查询 -匹配函数
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IQueryable<TEntity> Find(Func<TEntity, bool> predicate);

        /// <summary>
        /// 存在 -通过TProperty存在的字段名称在UserBase表中查询
        /// </summary>
        /// <returns></returns>
        Task<bool> Exist<TProperty>(Func<TEntity, TProperty> func);

        /// <summary>
        /// 存在 -异步查询 -Lambda表达式（Any的参数类型）
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<bool> Exist(Func<TEntity, bool> predicate);
    }
}
