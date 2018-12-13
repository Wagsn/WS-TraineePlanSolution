using AuthorizationCenter.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WS.Log;

namespace AuthorizationCenter.Stores
{
    /// <summary>
    /// 包含Id和Name属性的抽象类
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
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
        /// 创建
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<TEntity> Create(TEntity user)
        {
            if (Context.Set<TEntity>().Contains(user))
            {
                throw new Exception("用户已经存在不可以重复添加");
            }
            var entity =Context.Add(user).Entity;

            try
            {
                await Context.SaveChangesAsync();
            }
            catch(Exception e)
            {
                Logger.Error("新建实体失败：\r\n"+e);
                throw e;
            }
            return entity;
        }

        /// <summary>
        /// 批量查询
        /// </summary>
        /// <returns></returns>
        public IQueryable<TEntity> List()
        {
            return Context.Set<TEntity>();
        }

        /// <summary>
        /// 条件查询
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IQueryable<TEntity> List(Func<TEntity, bool> predicate)
        {
            return Context.Set<TEntity>().Where(e => predicate(e));
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity"></param>
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
                Logger.Error("更新实体失败：\r\n" + e);
                throw e;
            }
            return result;
        }

        /// <summary>
        /// 查找
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IQueryable<TEntity> Find(Func<TEntity, bool> predicate)
        {
            return Context.Set<TEntity>().Where(ub => predicate(ub));
        }

        /// <summary>
        /// 存在
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="func"></param>
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
        /// 存在
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Task<bool> Exist(Func<TEntity, bool> predicate)
        {
            return Context.Set<TEntity>().AnyAsync(ub => predicate(ub));
        }
    }
}
