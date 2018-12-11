using AuthorizationCenter.Models;
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
    public abstract class NameStoreBase<TEntity> : INameStore<TEntity> where TEntity : class
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
        /// Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public abstract IQueryable<TEntity> ById(string id);

        /// <summary>
        /// Name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public abstract IQueryable<TEntity> ByName(string name);

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
        /// 条件删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public abstract Task<IQueryable<TEntity>> DeleteIfId(string id);

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
        /// <param name="query"></param>
        /// <returns></returns>
        public IQueryable<TEntity> List(Func<IQueryable<TEntity>, IQueryable<TEntity>> query)
        {
            return query.Invoke(Context.Set<TEntity>());
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<TEntity> Update(TEntity user)
        {
            var entity = Context.Update(user).Entity;

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Logger.Error("更新实体失败：\r\n" + e);
                throw e;
            }
            return entity;
        }
    }
}
