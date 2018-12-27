using AuthorizationCenter.Define;
using AuthorizationCenter.Dto.Jsons;
using AuthorizationCenter.Dto.Requests;
using AuthorizationCenter.Managers;
using AuthorizationCenter.Entitys;
using AuthorizationCenter.Stores;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WS.Core.Dto;
using WS.Log;
using WS.Text;
using System.ComponentModel.DataAnnotations;

namespace AuthorizationCenter.Controllers
{
    /// <summary>
    /// 用户控制
    /// </summary>
    public class UserController : Controller
    {
        /// <summary>
        /// 用户管理
        /// </summary>
        public IUserManager<UserBaseJson> UserManager { get; set; }

        /// <summary>
        /// 角色管理
        /// </summary>
        public IRoleManager<RoleJson> RoleManager { get; set; }

        /// <summary>
        /// 用户角色关联管理
        /// </summary>
        public IUserRoleManager UserRoleManager { get; set; }
        
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
        /// <param name="userManager"></param>
        /// <param name="roleManager"></param>
        /// <param name="userRoleManager"></param>
        /// <param name="mapper"></param>
        public UserController(IUserManager<UserBaseJson> userManager, IRoleManager<RoleJson> roleManager, IUserRoleManager userRoleManager, IMapper mapper)
        {
            UserManager = userManager;
            RoleManager = roleManager;
            UserRoleManager = userRoleManager;
            Mapper = mapper;
            Logger = LoggerManager.GetLogger(GetType().Name);
        }

        /// <summary>
        /// 列表 -跳转到列表界面
        /// </summary>
        /// <returns></returns>
        // GET: UserBaseJsons
        public async Task<IActionResult> Index(int pageIndex =0, int pageSize =10)
        {
            ViewData[Constants.SIGNUSER] = SignUser;

            // 分页查询用户列表
            return View(await Functions.Page(UserManager.Find(), pageIndex, pageSize).ToListAsync());
        }

        /// <summary>
        /// 详情 -跳转到详情界面
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        // GET: UserBaseJsons/Details/5
        public async Task<IActionResult> Details(string id)
        {
            Logger.Trace($"[{nameof(Details)}] 查看用户详情->用户ID: {id}");
            // 检查参数
            if (id == null)
            {
                return NotFound();
            }
            // 请求响应构造
            ResponseMessage<UserBaseJson> response = new ResponseMessage<UserBaseJson>();
            ModelRequest<UserBaseJson> request = new ModelRequest<UserBaseJson>
            {
                User = SignUser,
                Data = new UserBaseJson { Id = id }
            };
            // 业务处理
            try
            {
                var user = await UserManager.FindById(id).SingleOrDefaultAsync();
                if (user == null)
                {
                    return NotFound();
                }
                else
                {
                    // 查询成功
                    // 再查询用户绑定的角色列表
                    ViewData[Constants.ROLES] = await RoleManager.FindByUserId(id).ToListAsync();
                    ViewData[Constants.USERROLES] = await UserRoleManager.FindByUserId(id).ToListAsync();
                    return View(user);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return View(nameof(Index));
            }
            
        }

        /// <summary>
        /// MVC 创建 -跳转到新建界面
        /// </summary>
        /// <returns></returns>
        // GET: UserBaseJsons/Create
        public IActionResult Create()
        {
            Logger.Trace($"[{nameof(Create)}] 跳转到用户新建界面");
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
                Logger.Trace($"[{nameof(Create)}]User Create Request: "+JsonUtil.ToJson(userBaseJson));
                // 检查参数
                if (string.IsNullOrWhiteSpace(userBaseJson.SignName)|| string.IsNullOrWhiteSpace(userBaseJson.PassWord))
                {
                    ModelState.AddModelError("All", "用户名或密码不能为空");
                    return View();
                }
                // 检查权限 - CheckPermissionFilter
                
                // 检查有效 被创建用户是否存在
                if (await UserManager.ExistByName(userBaseJson.SignName))
                {
                    ModelState.AddModelError("All", "创建的用户已经存在");
                    return View();
                }

                // 处理业务
                try
                {
                    // return UserManager.Create(this, userBaseJson); // (Controller, Request)=>IActionResult;
                    var user =await UserManager.Create(userBaseJson);
                    if (user != null)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("All", "新增失败");
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
            Logger.Trace($"[{nameof(Edit)}]进入编辑界面-编辑用户({id})");
            if (id == null)
            {
                return NotFound();
            }

            var userBase = await UserManager.FindById(id).SingleOrDefaultAsync();
            if (userBase == null)
            {
                Logger.Trace($"[{nameof(Edit)}]进入编辑界面-用户不存在({id})");
                return NotFound();
            }
            return View(Mapper.Map<UserBaseJson>(userBase));
        }

        /// <summary>
        /// MVC 编辑 -修改数据库记录
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <param name="userBaseJson">用户</param>
        /// <returns></returns>
        // POST: UserBaseJsons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,SignName,PassWord")] UserBaseJson userBaseJson)
        {
            Logger.Trace($"[{nameof(Edit)}] 编辑用户({id}) Request: \r\n"+JsonUtil.ToJson(userBaseJson));
            if (id != userBaseJson.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await UserManager.Update(userBaseJson);
                }
                catch (Exception e)
                {
#warning 存在异常风险 --先不管了
                    if (! await UserManager.ExistById(userBaseJson.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        Logger.Error($"[{nameof(Edit)}] 用户信息更新失败: " + e);
                    }
                    ModelState.AddModelError("All", "用户信息更新失败");
                    // 业务处理失败 -返回编辑界面
                    return View(userBaseJson);
                }
                // 编辑成功 -跳转到用户列表
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("All", "模型验证失败");
            // 模型验证失败 -返回编辑界面
            return View(userBaseJson);
        }

        /// <summary>
        /// MVC 删除 -跳转到删除界面 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        // GET: UserBaseJsons/Delete/5
        public async Task<IActionResult> Delete(string id, string errMsg = null)
        {
            Logger.Trace($"[{nameof(Delete)}] 删除用户({id})");
            // 检查参数
            if (id == null)
            {
                return NotFound();
            }
            // 业务处理
            var userJson = await UserManager.FindById(id).SingleOrDefaultAsync();
            if (userJson == null)
            {
                Logger.Trace($"[{nameof(Delete)}] 删除失败 用户未找到->用户ID: "+id);
                return NotFound();
            }
            return View(userJson);
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
            Logger.Trace($"[{nameof(DeleteConfirmed)}] 删除确认->用户ID: " + id);
            try
            {
                // 业务处理
                await UserManager.DeleteById(id);
                // 删除成功 -跳转到用户列表
                return RedirectToAction(nameof(Index));
            }
            catch(Exception e)
            {
                Logger.Error($"[{nameof(DeleteConfirmed)}] 用户删除失败：\r\n" + e);
                // 跳转回删除界面 -需要在界面说明发生了错误 --不清楚routeValues式怎么匹配的需要了解一下
                return RedirectToAction(nameof(Delete), new { id, errMsg = e.Message});
            }
        }

        /// <summary>
        /// 获取登陆用户简要信息 -每次都是新建一个UserBaseJson对象
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// 登陆用户
        /// </summary>
        private UserBaseJson SignUser
        {
            get
            {
                if (HttpContext.Session.GetString(Constants.USERID) == null)
                {
                    return null;
                }
                return new UserBaseJson
                {
                    Id = HttpContext.Session.GetString(Constants.USERID),
                    SignName = HttpContext.Session.GetString(Constants.SIGNNAME),
                    PassWord = HttpContext.Session.GetString(Constants.PASSWORD)
                };
            }
            set
            {
                if (value == null)
                {
                    HttpContext.Session.Remove(Constants.USERID);
                    HttpContext.Session.Remove(Constants.SIGNNAME);
                    HttpContext.Session.Remove(Constants.PASSWORD);
                }
                else
                {
                    HttpContext.Session.SetString(Constants.USERID, value.Id);
                    HttpContext.Session.SetString(Constants.SIGNNAME, value.SignName);
                    HttpContext.Session.SetString(Constants.PASSWORD, value.PassWord);
                }
            }
        }
    }
}
