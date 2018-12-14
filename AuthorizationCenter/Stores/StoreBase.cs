using AuthorizationCenter.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WS.Log;

namespace AuthorizationCenter.Stores
{
    /// <summary>
    /// 存储抽象类 
    /// </summary>
    /// <typeparam name="TEntity">数据库实体</typeparam>
    public abstract class StoreBase<TEntity> : IStore<TEntity> where TEntity : class
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        public ApplicationDbContext Context { get ; set; }

        /// <summary>
        /// 日志工具
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// 新建实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task<TEntity> Create(TEntity entity)
        {
            if (Context.Set<TEntity>().Contains(entity))
            {
                throw new Exception("实体已经存在不可以重复添加");
            }
            var result =Context.Add(entity).Entity;

            try
            {
                await Context.SaveChangesAsync();
            }
            catch(Exception e)
            {
                Logger.Error($"[{nameof(Create)}] 新建实体失败：\r\n"+e);
                throw e;
            }
            return result;
        }

        /// <summary>
        /// 批量查询
        /// </summary>
        /// <returns></returns>
        public IQueryable<TEntity> Find()
        {
            return Context.Set<TEntity>();
        }

        /// <summary>
        /// 更新实体 -异步
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task<TEntity> Update(TEntity entity)
        {
            var result = Context.Update(entity).Entity;
            try
            {
                await Context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Update)}] 更新实体失败：\r\n" + e);
                throw e;
            }
            return result;
        }

        /// <summary>
        /// 更新 更新用户
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="map">条件表达式</param>
        /// <returns></returns>
        public async Task<TProperty> Update<TProperty>(TEntity entity, Func<TEntity, TProperty> map)
        {
            return map(await Update(entity));
        }

        /// <summary>
        /// 更新实体 -条件表达式 -动作表达式 -返回处理后的集合
        /// </summary>
        /// <param name="predicate">条件表达式</param>
        /// <param name="action">动作表达式</param>
        /// <returns></returns>
        public async Task<IQueryable<TEntity>> Update(Func<TEntity, bool> predicate, Action<TEntity> action)
        {
            var ubs = Find(predicate);
            await ubs.ForEachAsync(ub => action(ub));
            Context.UpdateRange(ubs);

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Update)}] 条件更新失败: \r\n" + e);
                throw e;
            }
            return ubs;
        }

        /// <summary>
        /// 查找 -条件表达式
        /// </summary>
        /// <param name="predicate">条件表达式</param>
        /// <returns></returns>
        public IQueryable<TEntity> Find(Func<TEntity, bool> predicate)
        {
            return Context.Set<TEntity>().Where(ub => predicate(ub));
        }

        /// <summary>
        /// 条件查询 -条件表达式 -映射表达式
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="predicate">条件表达式</param>
        /// <param name="map">映射表达式</param>
        /// <returns></returns>
        public IQueryable<TProperty> Find<TProperty>(Func<TEntity, bool> predicate, Func<TEntity, TProperty> map)
        {
            return Context.Set<TEntity>().Where(e => predicate(e)).Select(e => map(e));
        }

        /// <summary>
        /// 存在 -异步 -实体映射表达式 -比较映射实体存在的字段 -性能损失
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="func">实体映射表达式</param>
        /// <returns></returns>
        public Task<bool> Exist<TProperty>(Func<TEntity, TProperty> func)
        {
            return Context.Set<TEntity>().AnyAsync(ub => Compare(ub, func(ub)));
        }

        /// <summary>
        /// 比较 -TProperty存在的字段与TSource中的同名字段进行比较 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="src"></param>
        /// <param name="prop"></param>
        /// <returns></returns>
        private bool Compare<TSource, TProperty>(TSource src, TProperty prop)
        {
            foreach (System.Reflection.PropertyInfo p in prop.GetType().GetProperties())
            {
                if (src.GetType().GetProperty(p.Name).GetValue(src).Equals(p.GetValue(prop)))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 存在 -条件表达式 -Any
        /// </summary>
        /// <param name="predicate">条件表达式</param>
        /// <returns></returns>
        public Task<bool> Exist(Func<TEntity, bool> predicate)
        {
            return Context.Set<TEntity>().AnyAsync(ub => predicate(ub));
        }

        /// <summary>
        /// 存在 -条件表达式 -集合所有元素满足条件表达式
        /// </summary>
        /// <param name="predicate">条件表达式</param>
        /// <returns></returns>
        public Task<bool> ExistAll(Func<TEntity, bool> predicate)
        {
            return Context.Set<TEntity>().AllAsync(ub => predicate(ub));
        }

        /// <summary>
        /// 删除 -条件表达式 -异步
        /// </summary>
        /// <param name="predicate">条件表达式</param>
        /// <returns></returns>
        public async Task<IQueryable<TEntity>> Delete(Func<TEntity, bool> predicate)
        {
            var ubs =Find().Where(ub => predicate(ub));
            Context.RemoveRange(ubs);

            try
            {
                await Context.SaveChangesAsync();
            }
            catch(Exception e)
            {
                Logger.Error($"[{nameof(Delete)}] 条件删除失败: \r\n" + e);
                throw e;
            }
            return ubs;
        }
        
        /// <summary>
        /// 删除 -异步
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task<TEntity> Delete(TEntity entity)
        {
            var result =Context.Remove(entity).Entity;

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Delete)}] 实体删除失败: \r\n" + e);
                throw e;
            }
            return result;
        }
    }
}
