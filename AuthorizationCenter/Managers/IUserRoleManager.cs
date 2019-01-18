using AuthorizationCenter.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Managers
{
    /// <summary>
    /// 用户角色管理接口
    /// </summary>
    public interface IUserRoleManager
    {
        /// <summary>
        /// 查询所有
        /// </summary>
        /// <returns></returns>
        IQueryable<UserRole> Find();

        /// <summary>
        /// 通过ID查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IQueryable<UserRole> FindById(string id);

        /// <summary>
        /// 通过用户ID查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IQueryable<UserRole> FindByUserId(string id);

        /// <summary>
        /// 创建用户角色关联
        /// </summary>
        /// <param name="userRole"></param>
        /// <returns></returns>
        Task<UserRole> Create(UserRole userRole);
        
        /// <summary>
        /// 创建用户角色关联
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="userRole">用户角色</param>
        /// <returns></returns>
        Task Create(string userId, UserRole userRole);

        /// <summary>
        /// 更新用户角色
        /// </summary>
        /// <param name="userRole"></param>
        /// <returns></returns>
        Task<UserRole> Update(UserRole userRole);

        /// <summary>
        /// 存在
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<bool> Exist(Func<UserRole, bool> predicate);

        /// <summary>
        /// 通过ID删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IEnumerable<UserRole>> DeleteById(string id);

        /// <summary>
        /// 通过ID删除
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="id">用户角色ID</param>
        /// <returns></returns>
        Task DeleteById(string userId, string id);

    }
}
