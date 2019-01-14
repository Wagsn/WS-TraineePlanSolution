using AuthorizationCenter.Dto.Jsons;
using AuthorizationCenter.Managers;
using AuthorizationCenter.Entitys;
using AuthorizationCenter.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using WS.Log;
using AuthorizationCenter.Define;
using Microsoft.AspNetCore.Http;

namespace AuthorizationCenter.Filters
{
    /// <summary>
    /// 权限检查过滤器（异步权限检查）
    /// </summary>
    public class CheckPermission : IAsyncActionFilter
    {
        ///// <summary>
        ///// 用户管理
        ///// </summary>
        //public IUserManager<UserBaseJson> UserManager { get; set; }
        
        /// <summary>
        /// 角色组织权限管理
        /// </summary>
        public IRoleOrgPerManager RoleOrgPerManager { get; set; }

        ///// <summary>
        ///// 角色管理
        ///// </summary>
        //public IRoleManager<RoleJson> RoleManager { get; set; }

        ///// <summary>
        ///// 组织管理
        ///// </summary>
        //public IOrganizationManager<OrganizationJson> OrganizationManager { get; set; }

        /// <summary>
        /// 日志记录器
        /// </summary>
        public ILogger Logger = LoggerManager.GetLogger<CheckPermission>();

        /// <summary>
        /// 权限
        /// </summary>
        public string Permission { get; set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="permission">权限</param>
        public CheckPermission (string permission = null)
        {
            Permission = permission;
            Console.WriteLine($"[{nameof(CheckPermission)}] 权限："+permission);
        }

        /// <summary>
        /// 异步权限检查
        /// </summary>
        /// <param name="context">行为执行上下文</param>
        /// <param name="next">下一个行为执行</param>
        /// <returns></returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //// 1. 检查用户是否登陆
            //UserJson user = GetSignUser(context.HttpContext);
            //if (user == null)
            //{
            //    context.Result = new ContentResult()
            //    {
            //        Content = "当前用户无效",
            //        StatusCode = 403,
            //    };
            //    return;
            //}

            //// 2. 检查权限 如何获取操作组织ID -公司项目通过User查询到OrgId，但是对于多组织体系怎么处理(在方法中手动调用?）
            //if(!await RoleOrgPerManager.HasPermission(user.Id, "orgId", Permission))
            //{
            //    context.Result = new ContentResult()
            //    {
            //        Content = "权限不足",
            //        StatusCode = 403,
            //    };
            //    return;
            //}
            await next();
        }

        /// <summary>
        /// 获取登陆用户简要信息 -每次都是新建一个UserBaseJson对象
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// 登陆用户
        /// </summary>
        private UserJson GetSignUser(HttpContext context)
        {
            if (context.Session.GetString(Constants.USERID) == null)
            {
                return null;
            }
            return new UserJson
            {
                Id = context.Session.GetString(Constants.USERID),
                SignName = context.Session.GetString(Constants.SIGNNAME),
                PassWord = context.Session.GetString(Constants.PASSWORD)
            };
        }
    }
}
