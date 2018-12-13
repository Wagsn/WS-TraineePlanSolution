using AuthorizationCenter.Dto.Jsons;
using AuthorizationCenter.Dto.Requests;
using AuthorizationCenter.Models;
using AuthorizationCenter.Stores;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using WS.Core.Dto;

namespace AuthorizationCenter.Managers
{
    /// <summary>
    /// 用户管理实现
    /// </summary>
    public class UserManager : IUserManager<IUserBaseStore, UserBaseJson>
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
            var ub = Mapper.Map<UserBase>(request.Data);
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
            response.Extension = Mapper.Map<List<UserBaseJson>>(await Store.List().ToListAsync());
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
        /// 检查用户密码否错误
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> Check(UserBaseJson user)
        {
            var userbase = await Store.ByName(user.SignName).FirstOrDefaultAsync();

            if (userbase == null)
            {
                return false;
            }

            if (userbase.PassWord == user.PassWord)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 通过ID查询
        /// </summary>
        /// <param name="response"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task ById([Required] ResponseMessage<UserBaseJson> response, [Required] ModelRequest<UserBaseJson> request)
        {
            response.Extension = Mapper.Map<UserBaseJson>(await Store.ById(request.Data.Id).FirstOrDefaultAsync());
        }
        
        /// <summary>
        /// 条件查询 -异步
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public async Task<List<UserBaseJson>> Find(Func<UserBaseJson, bool> func)
        {
            return Mapper.Map<List<UserBaseJson>>(await Store.Find(ub => func(Mapper.Map<UserBaseJson>(ub))).ToListAsync());
        }

        /// <summary>
        /// 通过Id查询 -异步
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<UserBaseJson> FindById(string id)
        {
            return Mapper.Map<UserBaseJson>(await Store.Find(ub => ub.Id == id).FirstOrDefaultAsync());
        }

        /// <summary>
        /// 更新 -异步
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public async Task<UserBaseJson> Update(UserBaseJson json)
        {
            return Mapper.Map<UserBaseJson>(await Store.Update(Mapper.Map<UserBase>(json)));
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
        /// <param name="func"></param>
        /// <returns></returns>
        public Task<bool> Exist(Func<UserBaseJson, bool> func)
        {
            return Store.Exist(ub=>func(Mapper.Map<UserBaseJson>(ub)));
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
        /// <param name="json"></param>
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
