using AuthorizationCenter.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Stores
{
    /// <summary>
    /// 存储接口 -这里放的全是接口方法
    /// </summary>
    /// <typeparam name="TEntity">数据库实体</typeparam>
    public interface IStore<TEntity> where TEntity : class
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        ApplicationDbContext Context { get; set; }

        /// <summary>
        /// 批量查询
        /// </summary>
        /// <returns></returns>
        IQueryable<TEntity> Find();

        /// <summary>
        /// 条件查询 -条件表达式
        /// </summary>
        /// <param name="predicate">条件表达式</param>
        /// <returns></returns>
        IQueryable<TEntity> Find(Func<TEntity, bool> predicate);

        /// <summary>
        /// 条件查询 -通过字段匹配查询
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IQueryable<TEntity> Find<TProperty>(Func<TEntity, TProperty> predicate);

        /// <summary>
        /// 新建 创建实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task<TEntity> Create(TEntity entity);

        /// <summary>
        /// 更新 更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task<TEntity> Update(TEntity entity);

        /// <summary>
        /// 更新实体 -条件表达式 -动作表达式 -返回处理后的集合
        /// </summary>
        /// <param name="predicate">条件表达式</param>
        /// <param name="action">动作表达式</param>
        /// <returns></returns>
        Task<IQueryable<TEntity>> Update(Func<TEntity, bool> predicate, Action<TEntity> action);

        /// <summary>
        /// 存在 -异步查询 -条件表达式（Any的参数类型）
        /// 如果数据库没有数据会返回
        /// </summary>
        /// <param name="predicate">条件表达式</param>
        /// <returns></returns>
        Task<bool> Exist(Func<TEntity, bool> predicate);

        /// <summary>
        /// 存在 -条件表达式 -集合所有元素满足条件表达式
        /// </summary>
        /// <param name="predicate">条件表达式</param>
        /// <returns></returns>
        Task<bool> ExistAll(Func<TEntity, bool> predicate);

        /// <summary>
        /// 删除 -条件表达式 -异步
        /// </summary>
        /// <param name="predicate">条件表达式</param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> Delete(Func<TEntity, bool> predicate);

        /// <summary>
        /// 删除 -异步
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task<TEntity> Delete(TEntity entity);
    }
}
