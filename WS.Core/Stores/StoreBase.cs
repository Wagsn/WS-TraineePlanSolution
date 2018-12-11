using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using WS.Core.Models;
using WS.Text;

namespace WS.Core.Stores
{
    /// <summary>
    /// Store基类，CRUD实现的地方，软删除在这里写，实现了一些通用方法，和工具方法
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TModel"></typeparam>
    public abstract class StoreBase<TContext, TModel> : IStore<TModel> where TContext : DbContext where TModel : TraceUpdateBase // IdentityDbContext
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        public virtual TContext Context { get; }

        /// <summary>
        /// 存储基类构造器
        /// </summary>
        /// <param name="context"></param>
        public StoreBase(TContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// 创建模型
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<TModel> CreateAsync(TModel model, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckNull(model);

            // 添加时间
            model._CreateTime = DateTime.Now;
            model._IsDeleted = false;

            model = Context.Add(model).Entity;

            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch(DbUpdateConcurrencyException e)
            {
                throw new Exception("WS------ StoreBase中保存改变时: \r\n", e);
            }
            
            return model;

        }

        /// <summary>
        /// 批量创建
        /// </summary>
        /// <param name="models"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<List<TModel>> CreateListAsync(List<TModel> models, CancellationToken cancellationToken)
        {
            CheckNull(models);

            // 添加时间
            var currTime = DateTime.Now;
            models.ForEach(model =>
            {
                model._CreateTime = currTime;
                model._IsDeleted = false;
            });

            Context.AddRange(models);

            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return models;
        }

        /// <summary>
        /// 删除模型
        /// </summary>
        /// <param name="model">模型实体</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(TModel model, CancellationToken cancellationToken)
        {
            CheckNull(model);

            // 软删除
            model._DeleteTime = DateTime.Now;
            model._IsDeleted = true;
            Context.Update(model);

            // 硬删除
            //Context.Remove(model);

            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        /// <summary>
        /// 条件删除
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task DeleteIfAsync(Func<IQueryable<TModel>, IQueryable<TModel>> query, CancellationToken cancellationToken)
        {
            CheckNull(query);

            // 软删除
            var delList = await ListAsync(query, cancellationToken);
            var currTime = DateTime.Now;
            delList.ForEach(a=> 
            {
                a._DeleteTime = currTime;
                a._IsDeleted = true;
            });
            Context.UpdateRange(delList);

            // 硬删除
            //Context.RemoveRange(models);

            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="models"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task DeleteListAsync(List<TModel> models, CancellationToken cancellationToken)
        {
            CheckNull(models);

            // 软删除
            var currTime = DateTime.Now;
            models.ForEach(model =>
            {
                model._DeleteTime = currTime;
                model._IsDeleted = true;
            });
            Context.UpdateRange(models);  // 未确认模型是否存在于数据库

            // 硬删除
            //Context.RemoveRange(models);

            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        /// <summary>
        /// 查询模型，通过Equals方法判断相似度，Model需要重写此方法，不过效率不高，推荐ReadAsync(query)方法
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<TModel> ReadAsync(TModel model, CancellationToken cancellationToken)
        {
            CheckNull(model);

            return (await ListAsync(a =>a.Where(b=>b.Equals(model)), cancellationToken)).SingleOrDefault();
        }

        /// <summary>
        /// 批量查询，涉及到DbContext，所以没有实现
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public abstract Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<TModel>, IQueryable<TResult>> query, CancellationToken cancellationToken);

        /// <summary>
        /// 条件查询
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<TResult> ReadAsync<TResult>(Func<IQueryable<TModel>, IQueryable<TResult>> query, CancellationToken cancellationToken)
        {
            CheckNull(query);
            
            // List是否返回为空？
            return (await ListAsync(query, cancellationToken)).SingleOrDefault();
        }

        /// <summary>
        /// 更新模型
        /// </summary>
        /// <param name="model2"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task UpdateAsync(TModel model2, CancellationToken cancellationToken)
        {
            CheckNull(model2);
            Console.WriteLine("WS------- StoreBase UpdateAsync 传入的Model：\r\n" + JsonUtil.ToJson(model2));
            var model = await ReadAsync(a => a.Where(b => model2.Equals(b)), cancellationToken);
            Console.WriteLine("WS------- StoreBase UpdateAsync 从数据库中获取的Model：\r\n"+JsonUtil.ToJson(model));
            model._Update(model2);
            
            // 更新时间
            model._UpdateTime = DateTime.Now;

            Context.Attach(model);
            Context.Update(model);

            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }

        /// <summary>
        /// 条件更新，按照条件更新数据
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task UpdateIfAsync(Func<IQueryable<TModel>, IQueryable<TModel>> query, CancellationToken cancellationToken)
        {
            CheckNull(query);

            // 要求是已经改好的Models，如果做不到，这个方法将被废弃
            var models = await ListAsync(query, cancellationToken);

            // 更新时间
            var currTime = DateTime.Now;
            models.ForEach(model =>
            {
                model._UpdateTime = currTime;
            });

            Context.AttachRange(models);
            Context.UpdateRange(models);

            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        /// <summary>
        /// 更新多个模型
        /// </summary>
        /// <param name="models"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task UpdateList(List<TModel> models, CancellationToken cancellationToken)
        {
            CheckNull(models);

            // 更新时间
            var currTime = DateTime.Now;
            models.ForEach(model => 
            {
                model._UpdateTime = currTime;
            });

            Context.AttachRange(models);
            Context.UpdateRange(models);

            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        /// <summary>
        /// 空检查，暂时不知道参数注解（特性）怎么用
        /// </summary>
        /// <typeparam name="TArgument"></typeparam>
        /// <param name="arg"></param>
        public static  void CheckNull<TArgument>(TArgument arg)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(nameof(arg));
            }
        }
    }
}
