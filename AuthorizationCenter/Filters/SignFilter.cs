using AuthorizationCenter.Define;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using WS.Log;

namespace AuthorizationCenter.Filters
{
    /// <summary>
    /// 登陆过滤器
    /// </summary>
    public class SignFilter : ActionFilterAttribute
    {
        /// <summary>
        /// 日志器
        /// </summary>
        public ILogger Logger = LoggerManager.GetLogger<SignFilter>();

        /// <summary>
        /// 当动作执行中 
        /// </summary>
        /// <param name="context">动作执行上下文</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // 判断是否需要检查权限
            var noNeedCheck = false;
            if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                noNeedCheck = controllerActionDescriptor.MethodInfo.GetCustomAttributes(inherit: true)
                  .Any(a => a.GetType().Equals(typeof(NoSignAttribute)));
            }
            if (noNeedCheck) return;

            // 检查登陆
            // 获取登陆信息
            var userid = context.HttpContext.Session.GetString(Constants.USERID);
            var signname = context.HttpContext.Session.GetString(Constants.SIGNNAME);
            var password = context.HttpContext.Session.GetString(Constants.PASSWORD);

            // 检查登陆信息
            if (userid == null && signname == null)
            {
                Logger.Trace($"[{nameof(OnActionExecuting)}] 用户未登陆");
                context.Result = new RedirectResult("/Sign/Index");
            }
            base.OnActionExecuting(context);
        }
    }
    /// <summary>
    /// 不需要权限登陆的地方加个特性
    /// </summary>
    public class NoSignAttribute : ActionFilterAttribute { }
}
