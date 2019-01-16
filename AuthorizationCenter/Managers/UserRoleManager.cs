using AuthorizationCenter.Entitys;
using AuthorizationCenter.Stores;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Managers
{
    /// <summary>
    /// 用户角色管理实现
    /// </summary>
    public class UserRoleManager : IUserRoleManager
    {
        /// <summary>
        /// 存储
        /// </summary>
        protected IUserRoleStore Store { get; set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="store"></param>
        public UserRoleManager(IUserRoleStore store)
        {
            Store = store;
        }

        /// <summary>
        /// 查询所有
        /// </summary>
        /// <returns></returns>
        public IQueryable<UserRole> Find()
        {
            return Store.Find().Include(ur => ur.User).Include(ur => ur.Role);
        }

        /// <summary>
        /// 通过ID查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IQueryable<UserRole> FindById(string id)
        {
            return Find().Where(ur => ur.Id == id);
        }

        /// <summary>
        /// 通过用户ID查询
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        public IQueryable<UserRole> FindByUserId(string id)
        {
            return Find().Where(ur => ur.UserId == id);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="userRole"></param>
        /// <returns></returns>
        public Task<UserRole> Create(UserRole userRole)
        {
            // 前端没有传Id上来
            userRole.Id = Guid.NewGuid().ToString();
            return Store.Create(userRole);
        }

        /// <summary>
        /// 更新用户角色
        /// </summary>
        /// <param name="userRole"></param>
        /// <returns></returns>
        public Task<UserRole> Update(UserRole userRole)
        {
            return Store.Update(userRole);
        }

        /// <summary>
        /// 存在
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Task<bool> Exist(Func<UserRole, bool> predicate)
        {
            return Store.Find().AnyAsync(ur => predicate(ur));
        }

        /// <summary>
        /// 通过ID删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<IEnumerable<UserRole>> DeleteById(string id)
        {
            return Store.Delete(ur => ur.Id == id);
        }
    }
}
