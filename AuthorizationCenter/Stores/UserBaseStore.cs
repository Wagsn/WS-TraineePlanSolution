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
    }
}
