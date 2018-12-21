using AuthorizationCenter.Dto.Jsons;
using AuthorizationCenter.Entitys;
using AuthorizationCenter.Stores;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Managers
{
    /// <summary>
    /// 权限管理
    /// </summary>
    public class PermissionManager : IPermissionManager<PermissionJson>
    {
        /// <summary>
        /// 存储
        /// </summary>
        protected IPermissionStore Store { get; set; }

        /// <summary>
        /// 类型映射
        /// </summary>
        protected IMapper Mapper { get; set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="store"></param>
        /// <param name="mapper"></param>
        public PermissionManager(IPermissionStore store, IMapper mapper)
        {
            Store = store;
            Mapper = mapper;
        }

        /// <summary>
        /// 查询所有
        /// </summary>
        /// <returns></returns>
        public IQueryable<PermissionJson> Find()
        {
            return Store.Find().Select(per => Mapper.Map<PermissionJson>(per));
        }

        /// <summary>
        /// 通过ID查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IQueryable<PermissionJson> FindById(string id)
        {
            return Find().Where(ur => ur.Id == id);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public async Task Create(PermissionJson permission)
        {
            // 前端没有传Id上来
            permission.Id = Guid.NewGuid().ToString();
            await Store.Create(Mapper.Map<Permission>(permission));
        }

        /// <summary>
        /// 更新用户角色
        /// </summary>
        /// <param name="userRole"></param>
        /// <returns></returns>
        public async Task Update(PermissionJson userRole)
        {
            await Store.Update(Mapper.Map<Permission>(userRole));
        }

        /// <summary>
        /// 存在
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Task<bool> Exist(Func<PermissionJson, bool> predicate)
        {
            return Store.Find().AnyAsync(ur => predicate(Mapper.Map<PermissionJson>(ur)));
        }

        /// <summary>
        /// 通过ID删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IQueryable<PermissionJson>> DeleteById(string id)
        {
            return (await Store.Delete(ur => ur.Id == id)).Select(per =>Mapper.Map<PermissionJson>(per));
        }
    }
}
