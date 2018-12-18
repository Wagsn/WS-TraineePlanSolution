using AuthorizationCenter.Dto.Jsons;
using AuthorizationCenter.Managers;
using AuthorizationCenter.Entitys;
using AuthorizationCenter.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AuthorizationCenter.Filters
{
    /// <summary>
    /// 权限检查过滤器（异步权限检查）
    /// </summary>
    public class CheckPermission : IAsyncActionFilter
    {
        /// <summary>
        /// 用户管理
        /// </summary>
        public IUserManager<UserBaseJson> UserManager { get; set; }

        /// <summary>
        /// 权限管理
        /// </summary>
        public IPermissionManager<IPermissionStore, PermissionJson> PermissionManager { get; set; }

        /// <summary>
        /// 角色管理
        /// </summary>
        public IRoleManager<RoleJson> RoleManager { get; set; }

        /// <summary>
        /// 组织管理
        /// </summary>
        public IOrganizationManager<IOrganizationStore, OrganizationJson> OrganizationManager { get; set; }

        /// <summary>
        /// 异步过滤
        /// </summary>
        /// <param name="context">行为执行上下文</param>
        /// <param name="next">下一个行为执行</param>
        /// <returns></returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // 检查context中携带的数据，判断权限是否满足
            
            // 1. 检查参数

            //if (context?.HttpContext?.User == null)
            //{
            //    context.Result = new ContentResult()
            //    {
            //        Content = "用户未登录",
            //        StatusCode = 403
            //    };
            //    return;
            //}

            // 2. 检查用户

            //UserBaseJson user = new UserBaseJson();
            //if (user == null)
            //{
            //    context.Result = new ContentResult()
            //    {
            //        Content = "当前用户无效",
            //        StatusCode = 403,
            //    };
            //    return;
            //}

            // 3. 检查权限


            await next();
        }
    }
}
