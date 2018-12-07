using AuthorizationCenter.Dto.Jsons;
using AuthorizationCenter.Dto.Requests;
using AuthorizationCenter.Managers;
using AuthorizationCenter.Models;
using AuthorizationCenter.Stores;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WS.Core.Dto;

namespace AuthorizationCenter.Controllers
{
    /// <summary>
    /// 用户控制
    /// </summary>
    public class UserBaseController : Controller 
    {
        /// <summary>
        /// 用户管理
        /// </summary>
        public IUserManager<IUserBaseStore<UserBase>, UserBaseJson> UserManager { get; set; }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<ResponseMessage<UserBaseJson>> SignUp(ModelRequest<UserBaseJson> request)
        {
            return null;
        }
        
        /// <summary>
        /// 签入
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<ResponseMessage<UserBaseJson>> SignIn(ModelRequest<UserBaseJson> request)
        {
            return null;
        }

        /// <summary>
        /// 签出
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<ResponseMessage<UserBaseJson>> SignOut(ModelRequest<UserBaseJson> request)
        {
            return null;
        }
    }
}
