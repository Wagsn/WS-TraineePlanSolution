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
    public class UserBaseStore : StoreBase<UserBase>, IUserBaseStore
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
        public IQueryable<UserBase> FindById(string id)
        {
            return Find(ub => ub.Id == id);
        }

        /// <summary>
        /// 查询 -ID -映射表达式
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="id"></param>
        /// <param name="map">映射表达式</param>
        /// <returns></returns>
        public IQueryable<TProperty> FindById<TProperty>(string id, Func<UserBase, TProperty> map)
        {
            return Find(ub => ub.Id == id, ub => map(ub));
        }

        /// <summary>
        /// 查询 -通过名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IQueryable<UserBase> FindByName(string name)
        {
            return Find(ub => ub.SignName == name);
        }

        /// <summary>
        /// 查询 -通过名称 -映射表达式
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="map">映射表达式</param>
        /// <returns></returns>
        public IQueryable<TProperty> FindByName<TProperty>(string name, Func<UserBase, TProperty> map)
        {
            return Find(ub => ub.SignName == name, ub => map(ub));
        }

        /// <summary>
        /// 删除 用户核心信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<IQueryable<UserBase>> DeleteById(string id)
        {
            // 打印日志
            Logger.Trace($"[{nameof(DeleteById)}] 条件删除用户({id})");
            return Delete(ub => ub.Id == id);
        }
    }
}
