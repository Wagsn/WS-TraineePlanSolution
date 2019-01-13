using AuthorizationCenter.Entitys;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WS.Log;
using WS.Text;

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
            //if (Context.Set<TEntity>().Contains(entity))
            //{
            //    throw new Exception("实体已经存在不可以重复添加");
            //}
            try
            {
                Logger.Trace($"[{nameof(Create)}] 新建实体:\r\n{JsonUtil.ToJson(entity)}");
                using(var trans = Context.Database.BeginTransaction())
                {
                    try
                    {
                        Context.Set<TEntity>().Add(entity);
                        //var res =await Context.AddAsync(entity);
                        await Context.SaveChangesAsync();
                        trans.Commit();
                    }
                    catch(Exception e)
                    {
                        //trans.Rollback();
                        Logger.Error($"[{nameof(Create)}] 事务提交失败:\r\n" + e);
                    }
                }
            }
            catch(Exception e)
            {
                Logger.Error($"[{nameof(Create)}] 新建实体:\r\n{JsonUtil.ToJson(entity)}\r\n失败：\r\n"+e);
                throw e;
            }
            return entity;
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
        /// 更新实体 -条件表达式 -动作表达式 -返回处理后的集合
        /// </summary>
        /// <param name="predicate">条件表达式</param>
        /// <param name="action">动作表达式</param>
        /// <returns></returns>
        public async Task<IQueryable<TEntity>> Update(Func<TEntity, bool> predicate, Action<TEntity> action)
        {
            var entitys = Find(predicate);
            await entitys.ForEachAsync(entity => action(entity));
            Context.UpdateRange(entitys);

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Update)}] 条件更新失败: \r\n" + e);
                throw e;
            }
            return entitys;
        }

        /// <summary>
        /// 条件查找 -条件表达式
        /// </summary>
        /// <param name="predicate">条件表达式</param>
        /// <returns></returns>
        public IQueryable<TEntity> Find(Func<TEntity, bool> predicate)
        {
            return Context.Set<TEntity>().Where(entity => predicate(entity));
        }

        /// <summary>
        /// 通过字段匹配查询
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IQueryable<TEntity> Find<TProperty>(Func<TEntity, TProperty> predicate)
        {
            return Context.Set<TEntity>().Where(entity => Compare(entity, predicate(entity)));
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
            return Context.Set<TEntity>().AsNoTracking().AnyAsync(entity => predicate(entity));
        }

        /// <summary>
        /// 存在 -条件表达式 -集合所有元素满足条件表达式
        /// </summary>
        /// <param name="predicate">条件表达式</param>
        /// <returns></returns>
        public Task<bool> ExistAll(Func<TEntity, bool> predicate)
        {
            return Context.Set<TEntity>().AllAsync(entity => predicate(entity));
        }

        /// <summary>
        /// 删除 -条件表达式 -异步
        /// </summary>
        /// <param name="predicate">条件表达式</param>
        /// <returns></returns>
        public async Task<IQueryable<TEntity>> Delete(Func<TEntity, bool> predicate)
        {
            var entitys =Find().Where(entity => predicate(entity));
            Context.RemoveRange(entitys);

            try
            {
                await Context.SaveChangesAsync();
            }
            catch(Exception e)
            {
                Logger.Error($"[{nameof(Delete)}] 条件删除失败: \r\n" + e);
                throw e;
            }
            return entitys;
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
