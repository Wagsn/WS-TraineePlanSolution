using AuthorizationCenter.Entitys;
using AuthorizationCenter.Stores;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WS.Log;

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
        protected IUserRoleStore UserRoleStore { get; set; }

        IRoleOrgPerStore RoleOrgPerStore { get; set; }

        IUserPermissionExpansionStore UserPermissionExpansionStore { get; set; }

        readonly ILogger Logger = LoggerManager.GetLogger<UserRoleManager>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userRoleStore"></param>
        /// <param name="roleOrgPerStore"></param>
        /// <param name="userPermissionExpansionStore"></param>
        public UserRoleManager(IUserRoleStore userRoleStore, IRoleOrgPerStore roleOrgPerStore, IUserPermissionExpansionStore userPermissionExpansionStore)
        {
            UserRoleStore = userRoleStore ?? throw new ArgumentNullException(nameof(userRoleStore));
            RoleOrgPerStore = roleOrgPerStore ?? throw new ArgumentNullException(nameof(roleOrgPerStore));
            UserPermissionExpansionStore = userPermissionExpansionStore ?? throw new ArgumentNullException(nameof(userPermissionExpansionStore));
        }

        /// <summary>
        /// 查询所有
        /// </summary>
        /// <returns></returns>
        public IQueryable<UserRole> Find()
        {
            return UserRoleStore.Find().Include(ur => ur.User).Include(ur => ur.Role);
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
            return UserRoleStore.Create(userRole);
        }

        /// <summary>
        /// 创建用户角色关联
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="userRole">用户角色</param>
        /// <returns></returns>
        public async Task Create(string userId, UserRole userRole)
        {
            await UserRoleStore.CreateByUserId(userId, userRole.UserId, userRole.RoleId);
        }

        /// <summary>
        /// 更新用户角色
        /// </summary>
        /// <param name="userRole"></param>
        /// <returns></returns>
        public Task<UserRole> Update(UserRole userRole)
        {
            return UserRoleStore.Update(userRole);
        }

        /// <summary>
        /// 存在
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Task<bool> Exist(Func<UserRole, bool> predicate)
        {
            return UserRoleStore.Find().AnyAsync(ur => predicate(ur));
        }

        /// <summary>
        /// 通过ID删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<IEnumerable<UserRole>> DeleteById(string id)
        {
            return UserRoleStore.Delete(ur => ur.Id == id);
        }

        /// <summary>
        /// 通过ID删除
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="urId">用户角色ID</param>
        /// <returns></returns>
        public async Task DeleteById(string userId, string urId)
        {
            // 删除用户角色关联
            await UserRoleStore.DeleteByUserId(userId, urId);
        }
    }
}
