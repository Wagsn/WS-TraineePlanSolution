using AuthorizationCenter.Define;
using AuthorizationCenter.Dto.Jsons;
using AuthorizationCenter.Dto.Requests;
using AuthorizationCenter.Managers;
using AuthorizationCenter.Models;
using AuthorizationCenter.Stores;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WS.Core.Dto;
using WS.Log;
using WS.Text;

namespace AuthorizationCenter.Controllers
{
    /// <summary>
    /// 用户控制
    /// </summary>
    public class UserController : Controller
    {
        /// <summary>
        /// 上下文
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// 用户管理
        /// </summary>
        public IUserManager<IUserBaseStore, UserBaseJson> UserManager { get; set; }
        
        /// <summary>
        /// 类型映射
        /// </summary>
        public IMapper Mapper { get; set; }

        /// <summary>
        /// 日志器
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="context">数据库上下文</param>
        /// <param name="userManager">用户管理</param>
        /// <param name="mapper">类型映射</param>
        public UserController(ApplicationDbContext context, IUserManager<IUserBaseStore, UserBaseJson> userManager, IMapper mapper)
        {
            _context = context;
            UserManager = userManager;
            Mapper = mapper;
            Logger = LoggerManager.GetLogger(GetType().Name);
        }

        /// <summary>
        /// 列表 -跳转到列表界面
        /// </summary>
        /// <returns></returns>
        // GET: UserBaseJsons
        public async Task<IActionResult> Index()
        {
            PagingResponseMessage<UserBaseJson> response = new PagingResponseMessage<UserBaseJson>();

            await UserManager.List(response, new ModelRequest<UserBaseJson>());

            ViewData[Constants.Str.SIGNUSER] = GetSignUser();

            Console.WriteLine("ViewData[\"SignUser\"]: " + JsonUtil.ToJson(ViewData[Constants.Str.SIGNUSER]));

            return View(response);  //Mapper.Map<UserBaseJson>(await _context.UserBases.ToListAsync())
        }

        /// <summary>
        /// 详情 -跳转到详情界面
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        // GET: UserBaseJsons/Details/5
        public async Task<IActionResult> Details(string id)
        {
            // 检查参数
            if (id == null)
            {
                return NotFound();
            }
            // 请求响应构造
            ResponseMessage<UserBaseJson> response = new ResponseMessage<UserBaseJson>();
            ModelRequest<UserBaseJson> request = new ModelRequest<UserBaseJson>
            {
                User = GetSignUser(),
                Data = new UserBaseJson { Id = id }
            };
            // 业务处理
            await UserManager.ById(response, request);
            // MVC 响应构造
            if (response.Code == ResponseDefine.SuccessCode)
            {
                return View(response.Extension);
            }
            else if(response.Code == ResponseDefine.NotFound)
            {
                return NotFound();
            }
            // 返回到主列表
            return View(nameof(Index));
        }

        /// <summary>
        /// MVC 创建 -跳转到新建界面
        /// </summary>
        /// <returns></returns>
        // GET: UserBaseJsons/Create
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// MVC 创建 -在数据库中添加数据
        /// </summary>
        /// <param name="userBaseJson">被创建用户</param>
        /// <returns></returns>
        // POST: UserBaseJsons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SignName,PassWord")] UserBaseJson userBaseJson)
        {
            if (ModelState.IsValid)
            {
                Logger.Trace("User Create Request: "+JsonUtil.ToJson(userBaseJson));
                // 检查参数
                if (string.IsNullOrWhiteSpace(userBaseJson.SignName)|| string.IsNullOrWhiteSpace(userBaseJson.PassWord))
                {
                    ModelState.AddModelError("All", "用户名或密码不能为空");
                    return View();
                }
                // 检查权限 - CheckPermissionFilter
                
                // 检查有效 被创建用户是否存在
                if (UserManager.IsExistForName(userBaseJson))
                {
                    ModelState.AddModelError("All", "创建的用户已经存在");
                    return View();
                }

                // 处理业务
                ResponseMessage<UserBaseJson> response = new ResponseMessage<UserBaseJson>();
                try
                {
                    await UserManager.Create(response, new ModelRequest<UserBaseJson>
                    {
                        Data = userBaseJson,
                        User = GetSignUser()
                    });
                    if (response.Code == ResponseDefine.SuccessCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("All", response.Message);
                        return View(userBaseJson);
                    }
                }
                catch(Exception e)
                {
                    ModelState.AddModelError("All", e.Message);
                    return View(userBaseJson);
                }
            }
            return View(userBaseJson);
        }

        /// <summary>
        /// MVC 编辑 -跳转到编辑界面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: UserBaseJsons/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userBase = await _context.UserBases.FindAsync(id);
            if (userBase == null)
            {
                return NotFound();
            }
            return View(Mapper.Map<UserBaseJson>(userBase));
        }

        /// <summary>
        /// MVC 编辑
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userBaseJson"></param>
        /// <returns></returns>
        // POST: UserBaseJsons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,SignName,PassWord")] UserBaseJson userBaseJson)
        {
            if (id != userBaseJson.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(Mapper.Map<UserBase>(userBaseJson));
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserBaseExists(userBaseJson.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(userBaseJson);
        }

        /// <summary>
        /// MVC 删除 -跳转到删除界面 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: UserBaseJsons/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            // 检查参数
            if (id == null)
            {
                return NotFound();
            }
            // 检查有效

            var userBase = await _context.UserBases.FirstOrDefaultAsync(m => m.Id == id);
            if (userBase == null)
            {
                return NotFound();
            }

            return View(Mapper.Map<UserBaseJson>(userBase));
        }

        /// <summary>
        /// MVC 删除确认 -从数据库删除 -跳转到列表界面 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // POST: UserBaseJsons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var userBaseJson = await _context.UserBases.FindAsync(id);
            _context.UserBases.Remove(userBaseJson);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// 用户存在 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool UserBaseExists(string id)
        {
            return _context.UserBases.Any(e => e.Id == id);
        }


        /// <summary>
        /// 获取登陆用户简要信息
        /// </summary>
        /// <returns></returns>
        private UserBaseJson GetSignUser()
        {
            return new UserBaseJson
            {
                // Id
                Id = HttpContext.Session.GetString(Constants.Str.USERID),
                SignName = HttpContext.Session.GetString(Constants.Str.SIGNNAME),
                PassWord = HttpContext.Session.GetString(Constants.Str.PASSWORD)
            };
        }

    }
}
