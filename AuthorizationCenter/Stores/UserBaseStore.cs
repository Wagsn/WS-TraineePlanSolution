using AuthorizationCenter.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WS.Log;
using WS.Text;

namespace AuthorizationCenter.Stores
{
    /// <summary>
    /// 用户核心表存储实现
    /// </summary>
    public class UserBaseStore : NameStoreBase<UserBase>, IUserBaseStore
    {
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="dbContext"></param>
        public UserBaseStore([Required]ApplicationDbContext dbContext)
        {
            Logger = LoggerManager.GetLogger(GetType().Name);
            Context = dbContext;
        }

        /// <summary>
        /// 查询 通过ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override IQueryable<UserBase> ById(string id)
        {
            var query = from ub in Context.UserBases
                        where ub.Id == id
                        select ub;
            return query;
        }

        /// <summary>
        /// 查询 通过名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override IQueryable<UserBase> ByName(string name)
        {
            var query = from ub in Context.UserBases
                        where ub.SignName == name
                        select ub;
            return query;
        }

        /// <summary>
        /// 删除 用户核心信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override async Task<IQueryable<UserBase>> DeleteById(string id)
        {
            var result = from ub in Context.UserBases
                         where ub.Id == id
                         select ub;
            Context.RemoveRange(result);

            // 打印日志
            Logger.Trace($"批量删除用户\r\n{JsonUtil.ToJson(result)}");

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
                throw e;
            }

            return result;
        }

        /// <summary>
        /// 存在 -Lambda表达式
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Task<bool> Exist(Func<UserBase, bool> predicate)
        {
            return Context.Set<UserBase>().AnyAsync(ub => predicate(ub));
        }

        /// <summary>
        /// 存在 -暂时不用
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public Task<bool> Exist<TProperty>(Func<UserBase, TProperty> func)
        {
            return Context.Set<UserBase>().AnyAsync(ub=> Compare(ub, func(ub)));
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
        /// 查询 -异步查询
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IQueryable<UserBase> Find(Func<UserBase, bool> predicate)
        {
            return Context.Set<UserBase>().Where(ub =>predicate(ub));
        }
    }
}
