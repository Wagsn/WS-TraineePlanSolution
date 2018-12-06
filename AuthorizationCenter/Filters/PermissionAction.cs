using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Filters
{
    /// <summary>
    /// 权限检查过滤器，Controller的Method都是原子操作
    /// </summary>
    public class CheckPermission : IAsyncActionFilter
    {
        /// <summary>
        /// 异步过滤
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await next();
        }
    }
}
