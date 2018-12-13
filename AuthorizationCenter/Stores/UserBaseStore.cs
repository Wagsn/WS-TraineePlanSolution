using AuthorizationCenter.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
        public override async Task<IQueryable<UserBase>> DeleteIfId(string id)
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
        /// 存在 与运算 (null 忽略)
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool IsExistAnd(UserBase user)
        {
            Console.WriteLine("IsExistAnd user: "+JsonUtil.ToJson(user));
            var query = from ub in Context.UserBases
                        where (string.IsNullOrWhiteSpace(user.Id) ? true : ub.Id == user.Id)
                        && (string.IsNullOrWhiteSpace(user.PassWord) ? true : ub.PassWord == user.PassWord)
                        && (string.IsNullOrWhiteSpace(user.SignName) ? true : ub.SignName == user.SignName)
                        select ub;
            Console.WriteLine("IsExistAnd->User Count: "+ query.Count());
            if (query.Count() == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// TODO 
        /// </summary>
        /// <typeparam name="TNoNamee"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public bool IsExistAnd<TNoNamee>(Func<UserBase, TNoNamee> func)
        {
            // 遍历 TType 字段

            return false;
        }
    }
}
