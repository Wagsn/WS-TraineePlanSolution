using AuthorizationCenter.Dto.Jsons;
using AuthorizationCenter.Dto.Requests;
using AuthorizationCenter.Managers;
using AuthorizationCenter.Models;
using AuthorizationCenter.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WS.Core.Dto;
using WS.Log;
using WS.Text;

namespace AuthorizationCenter.Controllers
{
    public class SignController : Controller
    {
        /// <summary>
        /// 日志器
        /// </summary>
        public ILogger Logger = LoggerManager.GetLogger("SignController");

        /// <summary>
        /// 用户管理
        /// </summary>
        public IUserManager<IUserBaseStore, UserBaseJson> UserManager { get; set; }

        ///// <summary>
        ///// 
        ///// </summary>
        //public SignController() { }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ViewResult Index()
        {
            return View();
        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="userManager"></param>
        public SignController([Required]IUserManager<IUserBaseStore, UserBaseJson> userManager)
        {
            UserManager = userManager;
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IActionResult> SignUp([FromForm]ModelRequest<UserBaseJson> request)
        {
            // 响应体构建
            ResponseMessage<UserBaseJson> response = new ResponseMessage<UserBaseJson>();

            Console.WriteLine(JsonUtil.ToJson(request));

            try
            {
                // 用户创建
                await UserManager.Create(response, request);
            }
            catch (Exception e)
            {
                Logger.Error("用户创建失败" + e);
            }


            return RedirectToRoute(new { controller = "UserBaseJsons", action = "Index" });
        }

        /// <summary>
        /// 签入
        /// </summary>
        /// <returns></returns>
        //[Produces("application/json")]
        public async Task<IActionResult> SignIn([FromForm]UserBaseJson user)
        {
            var username = Request.Form["SignName"].FirstOrDefault() ?? "username";
            var password = Request.Form["PassWord"].FirstOrDefault() ?? "password";
            Console.WriteLine(username + ", " + password);
            Console.WriteLine(JsonUtil.ToJson(user));

            await UserManager.Store.ByName(username).FirstOrDefaultAsync();

            // 登陆成功
            //SessionExtensions 

            Request.HttpContext.Session.SetString("username", username);
            Request.HttpContext.Session.SetString("password", password);


            return RedirectToRoute(new { controller = "UserBaseJsons", action = "Index" });
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
