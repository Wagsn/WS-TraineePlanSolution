using AuthorizationCenter.Entitys;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WS.Log;

namespace AuthorizationCenter.Stores
{
    /// <summary>
    /// 用户核心表存储实现
    /// </summary>
    public class UserStore : StoreBase<User>, IUserStore
    {

        private readonly ITransaction _transaction;
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="context"></param>
        /// <param name="transaction"></param>
        public UserStore(ApplicationDbContext context, ITransaction transaction)
        {
            Context = context;
            _transaction = transaction;
            Logger = LoggerManager.GetLogger<UserStore>();
        }

        /// <summary>
        /// 用户在其组织下创建用户
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="user">用户</param>
        /// <returns></returns>
        public async Task<User> CreateForOrgByUserId(string userId, User user)
        {
            using (var trans = await Context.Database.BeginTransactionAsync())
            {
                try
                {
                    var orgId = await Context.UserOrgs.Where(uo => uo.UserId == userId).Select(uo => uo.OrgId).AsNoTracking().SingleAsync();
                    Context.Add(user);
                    Context.Add(new UserOrg
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = user.Id,
                        OrgId = orgId
                    });
                    await Context.SaveChangesAsync();
                    trans.Commit();
                }
                catch (Exception e)
                {
                    Logger.Error($"事务提交失败:\r\n{e}");
                    trans.Rollback();
                    throw;
                }
            }
            return user;
        }

        /// <summary>
        /// 查询 通过ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IQueryable<User> FindById(string id)
        {
            return Find(ub => ub.Id == id);
        }

        /// <summary>
        /// 通过组织ID查询
        /// </summary>
        /// <param name="orgId">组织ID</param>
        /// <returns></returns>
        public IQueryable<User> FindByOrgId(string orgId)
        {
            return from user in Context.Users
                   where (from uo in Context.UserOrgs
                          where uo.OrgId == orgId
                          select uo.UserId).Contains(user.Id)
                   select user;
        }

        /// <summary>
        /// 查询 -通过名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IQueryable<User> FindByName(string name)
        {
            return Find(ub => ub.SignName == name);
        }

        /// <summary>
        /// 删除 -通过用户ID
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        public Task<IQueryable<User>> DeleteById(string id)
        {
            // 打印日志
            Logger.Trace($"[{nameof(DeleteById)}] 条件删除用户({id})");
            return Delete(ub => ub.Id == id);
        }

        /// <summary>
        /// 删除通过用户ID
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="id">被删除用户ID</param>
        /// <returns></returns>
        public async Task DeleteByUserId(string userId, string id)
        {
            using(var trans = await Context.Database.BeginTransactionAsync())
            {
                try
                {
                    var userroles = from ur in Context.Set<UserRole>()
                                    where ur.UserId == id
                                    select ur;
                    Context.AttachRange(userroles);
                    Context.RemoveRange(userroles);
                    var userorgs = from uo in Context.Set<UserOrg>()
                                   where uo.UserId == id
                                   select uo;
                    Context.AttachRange(userorgs);
                    Context.RemoveRange(userorgs);
                    var users = from user in Context.Set<User>()
                                where user.Id == id
                                select user;
                    Context.AttachRange(users);
                    Context.RemoveRange(users);
                    await Context.SaveChangesAsync();
                    trans.Commit();
                }
                catch (Exception e)
                {
                    Logger.Error($"[{nameof(DeleteByUserId)}] 删除用户失败:\r\n{e}");
                    trans.Rollback();
                    throw new Exception("删除用户失败", e);
                }
            }
            

        }

        /// <summary>
        /// 删除 -通过用户名
        /// </summary>
        /// <param name="name">用户名</param>
        /// <returns></returns>
        public Task<IQueryable<User>> DeleteByName(string name)
        {
            // 打印日志
            Logger.Trace($"[{nameof(DeleteByName)}] 条件删除用户({name})");
            return Delete(ub => ub.SignName == name);
        }
    }
}
