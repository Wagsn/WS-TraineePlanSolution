using Microsoft.AspNetCore.Mvc;

namespace AuthorizationCenter.Controllers
{
    /// <summary>
    /// 用于检查服务连通性
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class CheckController: Controller // ControllerBase
    {
        /// <summary>
        /// 检查
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<string> Get()
        {
            return "The website is working.";
        }

        /// <summary>
        /// 检查
        /// </summary>
        /// <returns></returns>
        [Filters.NoSign]
        public ViewResult Check()
        {
            return View();
        }
    }
}
