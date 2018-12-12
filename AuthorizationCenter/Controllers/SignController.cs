using AuthorizationCenter.Define;
using AuthorizationCenter.Dto.Jsons;
using AuthorizationCenter.Dto.Requests;
using AuthorizationCenter.Managers;
using AuthorizationCenter.Models;
using AuthorizationCenter.Stores;
using AuthorizationCenter.ViewModels.Sign;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
    /// <summary>
    /// 
    /// </summary>
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

        /// <summary>
        /// Session
        /// </summary>
        public ISession Session => HttpContext.Session;
        
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="userManager"></param>
        public SignController(IUserManager<IUserBaseStore, UserBaseJson> userManager)
        {
            UserManager = userManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ViewResult Index(string returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl??"/User/Index";
            return View();
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
        /// <param name="request">请求</param>
        /// <param name="returnUrl">返回URL（跳转）</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SignIn([Required]SignInViewModel request, string returnUrl = null)
        {
            //var username = Request.Form["SignName"].FirstOrDefault() ?? "username";
            //var password = Request.Form["PassWord"].FirstOrDefault() ?? "password";
            Console.WriteLine("request: "+JsonUtil.ToJson(request));
            Console.WriteLine("returnUrl: " + returnUrl);

            ViewData["returnUrl"] = returnUrl;

            // 参数检查
            if (string.IsNullOrWhiteSpace(request.SignName) || string.IsNullOrWhiteSpace(request.PassWord))
            {
                ModelState.AddModelError("All", "用户名或密码不能为空");
                return View(request);
            }
            var user = new UserBaseJson { SignName = request.SignName, PassWord = request.PassWord };

            // 登陆成功
            if (await UserManager.Check(user))
            {
                Session.SetString(Constants.Str.SignName, request.SignName);
                Session.SetString(Constants.Str.PassWord, request.PassWord);
                Console.WriteLine("SignInController->SignIn->ViewBag.SignUser: " + JsonUtil.ToJson(ViewBag.SignUser));
                Logger.Trace("登陆成功");

                // 重定向问题

                //var services = HttpContext.RequestServices;
                //var executor = services.GetRequiredService<ViewResultExecutor>();
                //var viewEngine = services.GetRequiredService<IRazorViewEngine>();
                //var view = viewEngine.GetView(null, "~/Pages/IeAlert.cshtml", true)?.View;
                //if (view != null)
                //{
                //    using (view as IDisposable)
                //    {
                //        await executor.ExecuteAsync(ControllerContext, view, ViewData, TempData, "text/html; charset=utf-8", 200);
                //    }
                //    return new EmptyResult();
                //}
                if (returnUrl != null)
                {
                    return RedirectToLocal(returnUrl);
                    //return RedirectUrlToAction(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            // 登陆失败
            else
            {
                Logger.Trace("登陆失败");
                ModelState.AddModelError("All", "用户名或密码错误");
                return View(nameof(Index), request);
                //return RedirectToLocal(Url.Action("Index", "Sign", new { ReturnUrl = returnUrl }));
            }
            //return View("Index", new { request, returnUrl });
            //return RedirectToRoute(new { controller = "UserBaseJsons", action = "Index"});
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

        private IActionResult RedirectToLocal(string returnUrl)
        {
            Console.WriteLine("登陆跳转URL："+returnUrl);
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        /// <summary>
        /// 将ActionURL解析出ActionName与ControllerName
        /// 再重定向到Action并传递参数
        /// </summary>
        /// <param name="actionURL"></param>
        /// <param name="routeValues"></param>
        /// <returns></returns>
        private IActionResult RedirectUrlToAction(string actionURL, object routeValues)
        {
            Console.WriteLine("RedirectUrlToAction--跳转URL：" + actionURL);
            if (Url.IsLocalUrl(actionURL))
            {
                int lastIndex = actionURL.LastIndexOf('/');
                var ActionName = actionURL.Substring(lastIndex, actionURL.Length-lastIndex);
                Console.WriteLine("ActionName: " + ActionName);
                int secondIndex = actionURL.Substring(0, lastIndex).LastIndexOf('/');
                var ControllerName = actionURL.Substring(secondIndex, lastIndex-secondIndex);
                Console.WriteLine("ControllerName: " + ControllerName);
                return RedirectToAction(ActionName, ControllerName, routeValues);
            }
            else
            {
                // 不是本地地址的话就去主页
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}
