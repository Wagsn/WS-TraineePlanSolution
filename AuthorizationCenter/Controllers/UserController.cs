using AuthorizationCenter.Define;
using AuthorizationCenter.Dto.Jsons;
using AuthorizationCenter.Dto.Requests;
using AuthorizationCenter.Managers;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
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
        /// 用户管理
        /// </summary>
        IUserManager<UserJson> UserManager { get; set; }

        /// <summary>
        /// 角色管理
        /// </summary>
        IRoleManager<RoleJson> RoleManager { get; set; }

        /// <summary>
        /// 组织管理
        /// </summary>
        IOrganizationManager<OrganizationJson> OrganizationManager { get; set; }

        /// <summary>
        /// 用户角色关联管理
        /// </summary>
        IUserRoleManager UserRoleManager { get; set; }

        /// <summary>
        /// 角色组织权限管理
        /// </summary>
        IRoleOrgPerManager RoleOrgPerManager { get; set; }
        
        /// <summary>
        /// 类型映射
        /// </summary>
        IMapper Mapper { get; set; }

        /// <summary>
        /// 日志器
        /// </summary>
        readonly ILogger Logger = LoggerManager.GetLogger<UserController>();

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="roleManager"></param>
        /// <param name="userRoleManager"></param>
        /// <param name="roleOrgPerManager"></param>
        /// <param name="mapper"></param>
        public UserController(IUserManager<UserJson> userManager, IRoleManager<RoleJson> roleManager, IUserRoleManager userRoleManager, IRoleOrgPerManager roleOrgPerManager, IMapper mapper)
        {
            UserManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            RoleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            UserRoleManager = userRoleManager ?? throw new ArgumentNullException(nameof(userRoleManager));
            RoleOrgPerManager = roleOrgPerManager ?? throw new ArgumentNullException(nameof(roleOrgPerManager));
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }



        /// <summary>
        /// 列表 -跳转到列表界面
        /// </summary>
        /// <returns></returns>
        // GET: UserBaseJsons
        public async Task<IActionResult> Index(int pageIndex =0, int pageSize =10)
        {
            Logger.Trace($"[{nameof(Index)}] 请求参数: pageIndex: {pageIndex}, pageSize: {pageSize}");
            ViewData[Constants.SIGNUSER] = SignUser;
            try
            {
                // 2. 业务处理
                var users = await UserManager.FindByUserId(SignUser.Id);
                // 分页查询用户列表 
                var data = users.AsQueryable().Page(pageIndex, pageSize).ToList();
                Logger.Trace($"[{nameof(Index)}] 响应数据:\r\n{JsonUtil.ToJson(data)}");
                return View(data);
            }
            catch(Exception e)
            {
                Logger.Error($"[{nameof(Index)}] 服务器错误:\r\n{e.ToString()}");
                return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
            }
        }

        /// <summary>
        /// 详情 -跳转到详情界面
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        // GET: UserBaseJsons/Details/5
        public async Task<IActionResult> Details(string id)
        {
            Logger.Trace($"[{nameof(Details)}] 查看用户详情 用户ID: {id}");
            // 0. 检查参数
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                // 1. 权限检查
                if (!(await RoleOrgPerManager.HasPermissionForUser(SignUser.Id, Constants.USER_QUERY, id)))
                {
                    Logger.Warn($"用户[{SignUser.SignName}]({SignUser.Id})没有权限({Constants.USER_QUERY})操作用户({id})");
                    ModelState.AddModelError("All", "没有权限");
                    return RedirectToAction(nameof(Index));
                }
                // 2. 业务处理
                var user = await UserManager.FindById(id).SingleOrDefaultAsync();
                Logger.Trace($"[{nameof(Details)}] 响应数据:\r\n{JsonUtil.ToJson(user)}");
                if (user == null)
                {
                    return NotFound();
                }
                else
                {
                    // 再查询用户绑定的角色列表
                    ViewData[Constants.ROLES] = await RoleManager.FindByUserId(id);
                    ViewData[Constants.USERROLES] = await UserRoleManager.FindByUserId(id).ToListAsync();
                    return View(user);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Details)}] 服务器错误:\r\n{e}");
                return RedirectToAction(nameof(Index));
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
        /// <param name="userJson">被创建用户</param>
        /// <returns></returns>
        // POST: UserBaseJsons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserJson userJson)
        {
            Logger.Trace($"[{nameof(Create)}] 用户[{SignUser.SignName}]({SignUser.Id})新增用户:\r\n{JsonUtil.ToJson(userJson)}");
            // 0. 检查参数
            if (string.IsNullOrWhiteSpace(userJson.SignName) || string.IsNullOrWhiteSpace(userJson.PassWord))
            {
                ModelState.AddModelError("All", "用户名或密码不能为空");
                return View();
            }
            try
            {
                // 1. 权限检查 -这里创建用户是在自己公司创建 -指定公司创建需要UserOrg表
                if (!(await RoleOrgPerManager.HasPermissionForUser(SignUser.Id, Constants.USER_CREATE, SignUser.Id)))
                {
                    Logger.Warn($"用户[{SignUser.SignName}]({SignUser.Id})没有权限({Constants.USER_CREATE})操作用户({SignUser.Id})");
                    ModelState.AddModelError("All", "没有权限");
                    return RedirectToAction(nameof(Index));
                }
                if (await UserManager.ExistByName(userJson.SignName))
                {
                    ModelState.AddModelError("All", "创建的用户已经存在");
                    return View(userJson);
                }
                // 2. 处理业务
                var user = UserManager.CreateToOrgByUserId(SignUser.Id, userJson);
                if (user != null)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("All", "新增失败");
                    return View(userJson);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Details)}] 服务器错误:\r\n{e}");
                ModelState.AddModelError("All", e.Message);
                return View(userJson);
            }
        }

        /// <summary>
        /// MVC 编辑 -跳转到编辑界面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: UserBaseJsons/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            Logger.Trace($"[{nameof(Edit)}] 请求参数: 用户ID({id})");
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                var user = await UserManager.FindById(id).SingleOrDefaultAsync();
                Logger.Trace($"[{nameof(Edit)}] 响应数据:\r\n{JsonUtil.ToJson(user)}");
                if (user == null)
                {
                    Logger.Trace($"[{nameof(Edit)}] 进入编辑界面-用户不存在({id})");
                    return NotFound();
                }
                return View(Mapper.Map<UserJson>(user));
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Edit)}] 服务器错误:\r\n{e}");
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// MVC 编辑 -修改数据库记录
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <param name="user">用户</param>
        /// <returns></returns>
        // POST: UserBaseJsons/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,SignName,PassWord")] UserJson user)
        {
            Logger.Trace($"[{nameof(Edit)}] 编辑用户({id}) Request: \r\n"+JsonUtil.ToJson(user));
            // 0. 参数检查
            if (id != user.Id)
            {
                return NotFound();
            }
            // 1. 权限检查 -编辑用户组织在登陆用户的用户管理权限所在组织之下
            // HasPermissionForUserId(signUserId, perId, userId);  // 判断登陆用户对某用户（可以查询到组织）具有某项权限
            try
            {
                // 2. 业务处理
                await UserManager.Update(user);
                // 编辑成功 -跳转到用户列表
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Edit)}] 用户信息更新失败: " + e);
                if (!await UserManager.ExistById(user.Id))
                {
                    return NotFound();
                }
                ModelState.AddModelError("All", "用户信息更新失败");
                return View(user);
            }
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
            // 0. 检查参数
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                // 1. 业务处理
                var userJson = await UserManager.FindById(id).SingleOrDefaultAsync();
                Logger.Trace($"[{nameof(Delete)}] 响应数据:\r\n{JsonUtil.ToJson(userJson)}");
                if (userJson == null)
                {
                    Logger.Trace($"[{nameof(Delete)}] 删除失败 用户未找到->用户ID: " + id);
                    return NotFound();
                }
                return View(userJson);
            }
            catch(Exception e)
            {
                Logger.Error($"[{nameof(Delete)}] 服务器错误:\r\n{e}");
                ModelState.AddModelError("All", e.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// MVC 删除确认 -从数据库删除 -跳转到列表界面 
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        // POST: UserBaseJsons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            Logger.Trace($"[{nameof(DeleteConfirmed)}] 删除确认->用户ID: " + id);
            try
            {
                // 1. 权限验证
                // 2. 业务处理
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
        private UserJson SignUser
        {
            get
            {
                if (HttpContext.Session.GetString(Constants.USERID) == null)
                {
                    return null;
                }
                return new UserJson
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
