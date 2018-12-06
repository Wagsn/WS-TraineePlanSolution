using AuthorizationCenter.Dto.Request;
using AuthorizationCenter.Models;
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
        /// 注册
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<ResponseMessage<UserBase>> SignUp(ModelRequest<UserBase> request)
        {
            return null;
        }
        
        /// <summary>
        /// 签入
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<ResponseMessage<UserBase>> SignIn(ModelRequest<UserBase> request)
        {
            return null;
        }

        /// <summary>
        /// 签出
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<ResponseMessage<UserBase>> SignOut(ModelRequest<UserBase> request)
        {
            return null;
        }
    }
}
