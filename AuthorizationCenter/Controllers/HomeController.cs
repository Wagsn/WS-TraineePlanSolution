using Microsoft.AspNetCore.Mvc;
using AuthorizationCenter.Entitys;
using System.Diagnostics;
using AuthorizationCenter.ViewModels;

namespace AuthorizationCenter.Controllers
{
    /// <summary>
    /// 主页控制器
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Controller Name
        /// </summary>
        public const string Name = "Home";

        /// <summary>
        /// 主页 -MVC
        /// </summary>
        /// <returns></returns>
        [Filters.NoSign]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 关于 -MVC
        /// </summary>
        /// <returns></returns>
        [Filters.NoSign]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }
        
        /// <summary>
        /// 联系 -MVC
        /// </summary>
        /// <returns></returns>
        [Filters.NoSign]
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        /// <summary>
        /// 隐私
        /// </summary>
        /// <returns></returns>
        [Filters.NoSign]
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// 错误
        /// </summary>
        /// <returns></returns>
        [Filters.NoSign]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
