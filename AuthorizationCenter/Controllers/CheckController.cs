namespace AuthorizationCenter.Controllers
{
    /// <summary>
    /// 用于检查服务连通性
    /// </summary>

    [Microsoft.AspNetCore.Mvc.Route("[controller]")]
    [Microsoft.AspNetCore.Mvc.ApiController]
    public class CheckController: Microsoft.AspNetCore.Mvc.Controller // ControllerBase
    {
        /// <summary>
        /// 检查
        /// </summary>
        /// <returns></returns>
        [Filters.NoSign]
        [Microsoft.AspNetCore.Mvc.HttpGet]
        public Microsoft.AspNetCore.Mvc.ActionResult<string> Get()
        {
            return "The website is working.";
        }

        /// <summary>
        /// 检查
        /// </summary>
        /// <returns></returns>
        [Filters.NoSign]
        [Microsoft.AspNetCore.Mvc.HttpGet("check")]
        public Microsoft.AspNetCore.Mvc.ViewResult Check()
        {
            return View();
        }
    }
}
