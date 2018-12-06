using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Controllers
{
    /// <summary>
    /// 用于检查服务连通性
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CheckController
    {
        [HttpGet]
        public ActionResult<string> Get()
        {
            return "The website is working.";
        }
    }
}
