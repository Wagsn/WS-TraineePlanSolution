using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AuthorizationCenter.Entitys;
using AuthorizationCenter.Managers;
using AuthorizationCenter.Dto.Jsons;
using WS.Log;
using AuthorizationCenter.Define;
using Microsoft.AspNetCore.Http;
using WS.Text;

namespace AuthorizationCenter.Controllers
{
    /// <summary>
    /// 用户角色绑定控制
    /// </summary>
    public class UserRolesController : Controller
    {
        /// <summary>
        /// 用户角色管理
        /// </summary>
        public IUserRoleManager UserRoleManager { get; set; }

        /// <summary>
        /// 用户管理
        /// </summary>
        public IUserManager<UserJson> UserManager { get; set; }

        /// <summary>
        /// 角色管理
        /// </summary>
        public IRoleManager<RoleJson> RoleManager { get; set; }

        /// <summary>
        /// 角色组织权限管理
        /// </summary>
        public IRoleOrgPerManager RoleOrgPerManager { get; set; }

        /// <summary>
        /// 日志记录器
        /// </summary>
        readonly ILogger Logger = LoggerManager.GetLogger<UserRolesController>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userRoleManager"></param>
        /// <param name="userManager"></param>
        /// <param name="roleManager"></param>
        /// <param name="roleOrgPerManager"></param>
        public UserRolesController(IUserRoleManager userRoleManager, IUserManager<UserJson> userManager, IRoleManager<RoleJson> roleManager, IRoleOrgPerManager roleOrgPerManager)
        {
            UserRoleManager = userRoleManager ?? throw new ArgumentNullException(nameof(userRoleManager));
            UserManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            RoleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            RoleOrgPerManager = roleOrgPerManager ?? throw new ArgumentNullException(nameof(roleOrgPerManager));
        }

        /// <summary>
        /// [MVC] 角色绑定
        /// </summary>
        /// <returns></returns>
        // GET: UserRoles
        public async Task<IActionResult> Index(int pageIndex = 0, int pageSize = 10, string errMsg = null)
        {
            try
            {
                // 1. 权限验证
                if (!await RoleOrgPerManager.HasPermission(SignUser.Id, Constants.USERROLE_MANAGE))
                {
                    return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
                }
                // 2. 业务处理
                ViewData["ErrMsg"] = errMsg;
                return View(await UserRoleManager.Find().ToListAsync());
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Index)}] 服务器错误:\r\n{e}");
                return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
            }
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: UserRoles/Details/5
        public async Task<IActionResult> Details(string id)
        {
            
            // 0. 参数检查
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                // 1. 权限验证
                if (!await RoleOrgPerManager.HasPermission(SignUser.Id, Constants.USERROLE_MANAGE))
                {
                    return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
                }
                // 2. 业务处理
                var userRole = await UserRoleManager.FindById(id).FirstOrDefaultAsync();
                if (userRole == null)
                {
                    return NotFound();
                }
                return View(userRole);
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Index)}] 服务器错误:\r\n{e}");
                return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
            }
        }

        /// <summary>
        /// 新增 用户角色关联
        /// </summary>
        /// <returns></returns>
        // GET: UserRoles/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                // 1. 权限验证
                if (!await RoleOrgPerManager.HasPermission(SignUser.Id, Constants.USERROLE_MANAGE))
                {
                    return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
                }
                // 2. 业务处理 查询 所有的用户角色
                ViewData["RoleId"] = new SelectList(RoleManager.Find(), nameof(Role.Id), nameof(Role.Name));
                ViewData["UserId"] = new SelectList(UserManager.Find(), nameof(Entitys.User.Id), nameof(Entitys.User.SignName));
                return View();
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Index)}] 服务器错误:\r\n{e}");
                return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
            }
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="userRole"></param>
        /// <returns></returns>
        // POST: UserRoles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(/*[Bind("Id,UserId,RoleId")]*/ UserRole userRole)
        {
            try
            {
                Logger.Trace($"[] 用户添加用户角色: 用户角色;\r\n{JsonUtil.ToJson(userRole)}");
                // 1. 权限验证
                if (!await RoleOrgPerManager.HasPermission(SignUser.Id, Constants.USERROLE_MANAGE))
                {
                    return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
                }
                await UserRoleManager.Create(SignUser.Id, userRole);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Index)}] 服务器错误:\r\n{e}");
                return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
            }
            //ViewData["Roles"] = _context.Roles;
            //ViewData["Users"] = _context.UserBases;

            // 生成选择框数据
            //ViewData["RoleId"] = new SelectList(RoleManager.Find(), nameof(Role.Id), nameof(Role.Name), userRole.RoleId);
            //ViewData["UserId"] = new SelectList(UserManager.Find(), nameof(Entitys.User.Id), nameof(Entitys.User.SignName), userRole.UserId);
            //return View(userRole);
        }

        /// <summary>
        /// 批量为用户添加角色
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IActionResult> MultCreate(string userId)
        {
            try
            {
                // 1. 权限验证
                if (!await RoleOrgPerManager.HasPermission(SignUser.Id, Constants.USERROLE_MANAGE))
                {
                    return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
                }
                // 2. 业务处理
                ViewData["Roles"] = await RoleManager.FindRoleOfOrgByUserId(SignUser.Id);
                var user = await UserManager.FindById(userId).AsNoTracking().SingleOrDefaultAsync();
                if (user == null)
                {
                    return NotFound();
                }
                return View(user);
            }
            catch(Exception e)
            {
                Logger.Error($"[{nameof(MultCreate)}] 服务器错误:\r\n{e}");
                return RedirectToAction(nameof(Index), new { errMsg = e.Message });
            }
        }

        /// <summary>
        /// 用户批量绑定角色
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MultCreate(UserJson user, IEnumerable<string> roleIds)
        {
            if (user == null || roleIds == null)
            {
                return NotFound();
            }
            try
            {
                Logger.Trace($"[{nameof(MultCreate)}] 用户添加用户角色: 用户:\r\n{JsonUtil.ToJson(user)}, 角色ID;\r\n{JsonUtil.ToJson(roleIds)}");
                // 1. 权限验证
                if (!await RoleOrgPerManager.HasPermission(SignUser.Id, Constants.USERROLE_MANAGE))
                {
                    return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
                }
                //await UserRoleManager.Create(SignUser.Id, userRole);
                foreach(var rId in roleIds)
                {
                    await UserRoleManager.Create(SignUser.Id, new UserRole
                    {
                        UserId = user.Id,
                        RoleId = rId
                    });
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Index)}] 服务器错误:\r\n{e}");
                return RedirectToAction(nameof(Index), new { errMsg = e.Message });
            }
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: UserRoles/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                // 1. 权限验证
                if (!await RoleOrgPerManager.HasPermission(SignUser.Id, Constants.USERROLE_MANAGE))
                {
                    return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
                }
                var userRole = await UserRoleManager.FindById(id).SingleOrDefaultAsync();
                if (userRole == null)
                {
                    return NotFound();
                }
                ViewData["RoleId"] = new SelectList(RoleManager.Find(), nameof(Role.Id), nameof(Role.Name), userRole.RoleId);
                ViewData["UserId"] = new SelectList(UserManager.Find(), nameof(Entitys.User.Id), nameof(Entitys.User.SignName), userRole.UserId);
                return View(userRole);
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Index)}] 服务器错误:\r\n{e}");
                return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
            }
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userRole"></param>
        /// <returns></returns>
        // POST: UserRoles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, /*[Bind("Id,UserId,RoleId")]*/ UserRole userRole)
        {
            // 0. 参数检查
            if (id != userRole.Id)
            {
                return NotFound();
            }
            try
            {
                // 1. 权限验证
                if (!await RoleOrgPerManager.HasPermission(SignUser.Id, Constants.USERROLE_MANAGE))
                {
                    return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
                }
                // 2. 业务处理
                // 重复判断 是否存在UserId和RoleId的关系
                if (await UserRoleManager.Exist(ur => ur.RoleId == userRole.RoleId && ur.UserId == userRole.UserId))
                {
                    ModelState.AddModelError("All", "角色已经被绑定在该用户上");
                    ViewData["RoleId"] = new SelectList(RoleManager.Find(), nameof(Role.Id), nameof(Role.Name), userRole.RoleId);
                    ViewData["UserId"] = new SelectList(UserManager.Find(), nameof(Entitys.User.Id), nameof(Entitys.User.SignName), userRole.UserId);
                    return View(userRole);
                }
                else
                {
                    await UserRoleManager.Update(userRole);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Index)}] 服务器错误:\r\n{e}");
                return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">UserRole.Id</param>
        /// <returns></returns>
        // GET: UserRoles/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                // 1. 权限验证
                if (!await RoleOrgPerManager.HasPermission(SignUser.Id, Constants.USERROLE_MANAGE))
                {
                    return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
                }
                var urs = await UserRoleManager.FindById(id).SingleOrDefaultAsync();
                if (urs == null)
                {
                    return NotFound();
                }

                return View(urs);
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Index)}] 服务器错误:\r\n{e}");
                return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
            }
        }

        /// <summary>
        /// 删除确认
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // POST: UserRoles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                // 1. 权限验证
                if (!await RoleOrgPerManager.HasPermission(SignUser.Id, Constants.USERROLE_MANAGE))
                {
                    return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
                }
                // await UserRoleManager.DeleteById(id);
                await UserRoleManager.DeleteById(SignUser.Id, id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Index)}] 服务器错误:\r\n{e}");
                return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
            }
        }

        /// <summary>
        /// 获取登陆用户简要信息 -每次都是新建一个UserBaseJson对象
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// 登陆用户
        /// </summary>
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
