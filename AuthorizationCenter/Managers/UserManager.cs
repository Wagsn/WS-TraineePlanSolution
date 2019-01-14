using AuthorizationCenter.Define;
using AuthorizationCenter.Dto.Jsons;
using AuthorizationCenter.Dto.Requests;
using AuthorizationCenter.Entitys;
using AuthorizationCenter.Stores;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WS.Core.Dto;
using WS.Log;

namespace AuthorizationCenter.Managers
{
    /// <summary>
    /// 用户管理实现
    /// </summary>
    public class UserManager : IUserManager<UserJson>
    {
        /// <summary>
        /// 用户存储
        /// </summary>
        public IUserStore Store { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IUserOrgStore UserOrgStore { get; set; }

        /// <summary>
        /// 角色组织权限存储
        /// </summary>
        public IRoleOrgPerStore RoleOrgPerStore { get; set; }

        /// <summary>
        /// 组织存储
        /// </summary>
        public IOrganizationStore OrganizationStore { get; set; }

        /// <summary>
        /// 类型映射
        /// </summary>
        public IMapper Mapper { get; set; }

        /// <summary>
        /// 日志记录器
        /// </summary>
        public ILogger Logger = LoggerManager.GetLogger<UserManager>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="store"></param>
        /// <param name="userOrgStore"></param>
        /// <param name="roleOrgPerStore"></param>
        /// <param name="organizationStore"></param>
        /// <param name="mapper"></param>
        public UserManager(IUserStore store, IUserOrgStore userOrgStore, IRoleOrgPerStore roleOrgPerStore, IOrganizationStore organizationStore, IMapper mapper)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
            UserOrgStore = userOrgStore ?? throw new ArgumentNullException(nameof(userOrgStore));
            RoleOrgPerStore = roleOrgPerStore ?? throw new ArgumentNullException(nameof(roleOrgPerStore));
            OrganizationStore = organizationStore ?? throw new ArgumentNullException(nameof(organizationStore));
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="response"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task Create([Required] ResponseMessage<UserJson> response, [Required] ModelRequest<UserJson> request)
        {
            // 构建模型
            var ub = Mapper.Map<User>(request.Data);
            ub.Id = Guid.NewGuid().ToString();

            // 存储
            try
            {
                var dbub = await Store.Create(ub);
                response.Extension = Mapper.Map<UserJson>(dbub);
            }
            catch (Exception e)
            {
                response.Wrap(ResponseDefine.BadRequset, e.Message);  // 给前端返回简称 Store层中打印全部错误消息日志
                throw e;
            }
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public async Task<UserJson> Create(UserJson json)
        {
            var user = Mapper.Map<User>(json);
            user.Id = Guid.NewGuid().ToString();
            // 存储
            var dbUser = await Store.Create(user);
            return Mapper.Map<UserJson>(dbUser);
        }

        /// <summary>
        /// 用户在自己的组织下创建用户
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="json">用户</param>
        /// <returns></returns>
        public async Task<UserJson> CreateForOrgByUserId(string userId, UserJson json)
        {
            json.Id = Guid.NewGuid().ToString();
            await Store.CreateForOrgByUserId(userId, Mapper.Map<User>(json));
            return json;
        }

        /// <summary>
        /// 用户(userId)添加用户(json)到组织(orgId)下
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="json">用户</param>
        /// <param name="orgId">组织ID</param>
        /// <returns></returns>
        public async Task<UserJson> CreateToOrgByUserId(string userId, UserJson json, string orgId)
        {
            json.Id = Guid.NewGuid().ToString();
            // 1. 创建用户
            await Store.Create(Mapper.Map<User>(json));
            // 2. 创建用户组织关联
            await UserOrgStore.Create(new UserOrg
            {
                Id = Guid.NewGuid().ToString(),
                UserId = json.Id,
                OrgId = orgId
            });
            return json;
        }

        /// <summary>
        /// 批量 查询
        /// </summary>
        /// <param name="response"></param>
        /// <param name="request"></param>
        public async Task List([Required] PagingResponseMessage<UserJson> response, [Required] ModelRequest<UserJson> request)
        {
            response.Extension = await Store.Find().Select(ub => Mapper.Map<UserJson>(ub)).ToListAsync();
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="response"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task Update([Required] ResponseMessage<UserJson> response, [Required] ModelRequest<UserJson> request)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 通过ID查询
        /// </summary>
        /// <param name="response"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task ById([Required] ResponseMessage<UserJson> response, [Required] ModelRequest<UserJson> request)
        {
            response.Extension = await Store.Find(ub => ub.Id == request.Data.Id).Select(ub => Mapper.Map<UserJson>(ub)).FirstOrDefaultAsync();
        }

        /// <summary>
        /// 找到所有
        /// </summary>
        /// <returns></returns>
        public IQueryable<UserJson> Find()
        {
            return Store.Find().Select(user => Mapper.Map<UserJson>(user));
        }
        
        /// <summary>
        /// 条件查询 -异步
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IQueryable<UserJson> Find(Func<UserJson, bool> predicate)
        {
            return Store.Find(ub => predicate(Mapper.Map<UserJson>(ub))).Select(ub => Mapper.Map<UserJson>(ub));
        }

        /// <summary>
        /// 通过Id查询 -异步
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IQueryable<UserJson> FindById(string id)
        {
            return Store.FindById(id).Select(ub => Mapper.Map<UserJson>(ub));
        }

        /// <summary>
        /// 通过用户ID查询有权查看的用户列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public async Task<IEnumerable<UserJson>> FindByUserId(string userId)
        {
            // 1. 查询有权组织
            var perOrgs = await RoleOrgPerStore.FindOrgByUserIdPerName(userId, Constants.USER_QUERY);
            // 2. 查询用户集合
            var result = new List<User>();
            foreach(var org in perOrgs)
            {
                result.AddRange(await Store.FindByOrgId(org.Id).ToListAsync());
            }
            return result.Select(user => Mapper.Map<UserJson>(user));
        }

        /// <summary>
        /// 通过Name查询 -异步
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IQueryable<UserJson> FindByName(string name)
        {
            return Store.FindByName(name).AsNoTracking().Select(ub => Mapper.Map<UserJson>(ub));
        }

        /// <summary>
        /// 更新 -异步
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public async Task<UserJson> Update(UserJson json)
        {
            return Mapper.Map<UserJson>(await Store.Update(Mapper.Map<User>(json)));
        }

        /// <summary>
        /// 存在通过ID -异步
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<bool> ExistById(string id)
        {
            return Store.Exist(ub => ub.Id == id);
        }

        /// <summary>
        /// 存在 -异步
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Task<bool> Exist(Func<UserJson, bool> predicate)
        {
            return Store.Exist(ub=>predicate(Mapper.Map<UserJson>(ub)));
        }

        /// <summary>
        /// 存在Name
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns></returns>
        public Task<bool> ExistByName(string name)
        {
            return Store.Exist(ub => ub.SignName == name);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="json">Dto</param>
        /// <returns></returns>
        public async Task Delete(UserJson json)
        {
            await Store.DeleteById(json.Id);
        }

        /// <summary>
        /// 通过ID删除 -异步
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public async Task DeleteById(string id)
        {
            await Store.DeleteById(id);
        }

        /// <summary>
        /// 用户(userId)删除用户(id)
        /// </summary>
        /// <param name="userId">登陆用户ID</param>
        /// <param name="id">删除用户ID</param>
        /// <returns></returns>
        public async Task DeleteByUserId(string userId, string id)
        {
            // 删除该用户的所有关联
            await Store.DeleteByUserId(userId, id);
        }
    }
}
