using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WS.Core.Dto;

namespace AuthorizationCenter.Controllers
{
    /// <summary>
    /// 用于检查服务连通性
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CheckController: Controller // ControllerBase
    {
        ///// <summary>
        ///// 检查
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //public ActionResult<string> Get()
        //{
        //    return "The website is working.";
        //}

        /// <summary>
        /// 检查
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ResponseMessage<string> GetResponse()
        {
            ResponseMessage<string> response = new ResponseMessage<string>
            {
                Extension = "666"
            };
            return response;
        }
    }
}
