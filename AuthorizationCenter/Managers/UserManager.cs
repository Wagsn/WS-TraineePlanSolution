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
    public class UserManager : IUserManager<UserBaseJson>
    {
        /// <summary>
        /// 用户存储
        /// </summary>
        public IUserBaseStore Store { get; set; }

        /// <summary>
        /// 类型映射
        /// </summary>
        public IMapper Mapper { get; set; }

        /// <summary>
        /// 日志记录器
        /// </summary>
        public ILogger Logger = LoggerManager.GetLogger(nameof(UserManager));

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="store"></param>
        /// <param name="mapper"></param>
        public UserManager(IUserBaseStore store, IMapper mapper)
        {
            Store = store;
            Mapper = mapper;
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="response"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task Create([Required] ResponseMessage<UserBaseJson> response, [Required] ModelRequest<UserBaseJson> request)
        {
            // 构建模型
            var ub = Mapper.Map<User>(request.Data);
            ub.Id = Guid.NewGuid().ToString();

            // 存储
            try
            {
                var dbub = await Store.Create(ub);
                response.Extension = Mapper.Map<UserBaseJson>(dbub);
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
        public async Task<UserBaseJson> Create(UserBaseJson json)
        {
            var ub = Mapper.Map<User>(json);
            ub.Id = Guid.NewGuid().ToString();
            // 存储
            try
            {
                var dbub = await Store.Create(ub);
                return Mapper.Map<UserBaseJson>(dbub);
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="response"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task Delete([Required] ResponseMessage<UserBaseJson> response, [Required] ModelRequest<UserBaseJson> request)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 批量 查询
        /// </summary>
        /// <param name="response"></param>
        /// <param name="request"></param>
        public async Task List([Required] PagingResponseMessage<UserBaseJson> response, [Required] ModelRequest<UserBaseJson> request)
        {
            response.Extension = await Store.Find().Select(ub => Mapper.Map<UserBaseJson>(ub)).ToListAsync();
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="response"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task Update([Required] ResponseMessage<UserBaseJson> response, [Required] ModelRequest<UserBaseJson> request)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 通过ID查询
        /// </summary>
        /// <param name="response"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task ById([Required] ResponseMessage<UserBaseJson> response, [Required] ModelRequest<UserBaseJson> request)
        {
            response.Extension = await Store.Find(ub => ub.Id == request.Data.Id).Select(ub => Mapper.Map<UserBaseJson>(ub)).FirstOrDefaultAsync();
        }

        /// <summary>
        /// 找到所有
        /// </summary>
        /// <returns></returns>
        public IQueryable<UserBaseJson> Find()
        {
            return Store.Find().Select(user => Mapper.Map<UserBaseJson>(user));
        }
        
        /// <summary>
        /// 条件查询 -异步
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IQueryable<UserBaseJson> Find(Func<UserBaseJson, bool> predicate)
        {
            return Store.Find(ub => predicate(Mapper.Map<UserBaseJson>(ub))).Select(ub => Mapper.Map<UserBaseJson>(ub));
        }

        /// <summary>
        /// 通过Id查询 -异步
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IQueryable<UserBaseJson> FindById(string id)
        {
            return Store.FindById(id).Select(ub => Mapper.Map<UserBaseJson>(ub));
        }

        /// <summary>
        /// 通过Name查询 -异步
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IQueryable<UserBaseJson> FindByName(string name)
        {
            return Store.FindByName(name).Select(ub => Mapper.Map<UserBaseJson>(ub));
        }

        /// <summary>
        /// 更新 -异步
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public  Task<UserBaseJson> Update(UserBaseJson json)
        {
            return Store.Update(Mapper.Map<User>(json), ub => Mapper.Map<UserBaseJson>(ub));
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
        public Task<bool> Exist(Func<UserBaseJson, bool> predicate)
        {
            return Store.Exist(ub=>predicate(Mapper.Map<UserBaseJson>(ub)));
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
        public async Task Delete(UserBaseJson json)
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
    }
}
