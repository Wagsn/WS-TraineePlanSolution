using AuthorizationCenter.Dto.Jsons;
using AuthorizationCenter.Dto.Requests;
using AuthorizationCenter.Models;
using AuthorizationCenter.Stores;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
        /// 批量
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
    }
}
