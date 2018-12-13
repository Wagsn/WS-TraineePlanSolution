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
        /// <param name="predicate"></param>
        /// <returns></returns>
        IQueryable<TEntity> List(Func<TEntity, bool> predicate);

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
        Task<IQueryable<TEntity>> DeleteById(string id);
        
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
