using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthorizationCenter.Managers;
using AuthorizationCenter.Dto.Jsons;
using WS.Log;
using WS.Text;
using Microsoft.AspNetCore.Http;
using AuthorizationCenter.Define;
using AuthorizationCenter.Entitys;

namespace AuthorizationCenter.Controllers
{
    /// <summary>
    /// 角色控制器
    /// </summary>
    public class RolesController : Controller
    {

        /// <summary>
        /// 角色管理
        /// </summary>
        IRoleManager<RoleJson> RoleManager { get; set; }

        /// <summary>
        /// 角色组织权限管理
        /// </summary>
        IRoleOrgPerManager RoleOrgPerManager { get; set; }

        /// <summary>
        /// 日志器
        /// </summary>
        readonly ILogger Logger = LoggerManager.GetLogger<RolesController>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleManager"></param>
        /// <param name="roleOrgPerManager"></param>
        public RolesController(IRoleManager<RoleJson> roleManager, IRoleOrgPerManager roleOrgPerManager)
        {
            RoleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            RoleOrgPerManager = roleOrgPerManager ?? throw new ArgumentNullException(nameof(roleOrgPerManager));
        }



        /// <summary>
        /// [MVC] 角色管理-角色列表
        /// 管理属于自己组织的角色（包含子组织的角色）
        /// TODO：管理自己有权限管理角色的组织的角色
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index(int pageIndex = 0, int pageSize = 10)
        {
            try
            {
                // 1. 权限检查
                // 2. 业务处理
                Logger.Trace($"[{nameof(Index)}] 角色管理主页");
                // 通过登陆的用户查询组织，通过组织查询角色
                var roles = await RoleManager.FindRoleOfOrgByUserId(SignUser.Id);
                Logger.Trace($"[{nameof(Index)}] 响应数据: \r\n{JsonUtil.ToJson(roles)}");
                return View(roles);
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Index)}] 服务器错误:\r\n{e.ToString()}");
                ModelState.AddModelError("All", e.Message);
                return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
            }
        }

        /// <summary>
        /// [MVC] 角色管理-角色详情界面
        /// </summary>
        /// <param name="id">角色ID，对应前端route</param>
        /// <returns></returns>
        // GET: Roles/Details/5
        public async Task<IActionResult> Details(string id)
        {
            Logger.Trace($"[{nameof(Details)}] 请求参数: 角色({id})");
            // 0. 参数检查
            if (id == null)
            {
                return NotFound();
            }
            // 1. 权限检查

            // 2. 业务处理
            try
            {
                var role = await RoleManager.FindById(id);
                Logger.Trace($"[{nameof(Details)}] 响应数据:\r\n{JsonUtil.ToJson(role)}");
                if (role == null)
                {
                    return NotFound();
                }
                return View(role);
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Index)}] 服务器错误:\r\n{e.ToString()}");
                ModelState.AddModelError("All", e.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// [MVC] 角色管理-新增角色界面
        /// </summary>
        /// <returns></returns>
        // GET: Roles/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                // 1. 权限检查
                if (!await RoleOrgPerManager.HasPermissionInSelfOrg(SignUser.Id, Constants.ROLE_CREATE_VIEW))
                {
                    Logger.Warn($"[{nameof(Create)}] 用户[{SignUser.SignName}]({SignUser.Id})没有权限({Constants.ROLE_CREATE_VIEW})");
                    ModelState.AddModelError("All", "没有权限");
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Create)}] 服务器错误: \r\n{e.ToString()}");
                ModelState.AddModelError("All", e.Message);
                return RedirectToAction(nameof(Index));
            }
            // 1. 权限检查
            return View();
        }

        /// <summary>
        /// [MVC] 角色管理-新增角色
        /// 将角色添加到登陆用户的组织上
        /// TODO：将角色添加到有权限添加的组织
        /// </summary>
        /// <param name="role">用户</param>
        /// <returns></returns>
        // POST: Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(/*[Bind("Id,Name,Decription")]*/ RoleJson role)
        {
            Logger.Trace($"[{nameof(Create)}] 请求参数; \r\n{JsonUtil.ToJson(role)}");
            // 0. 参数检查
            if (role == null)
            {
                return View(nameof(Create));
            }
            try
            {
                // 1. 权限检查
                if(!await RoleOrgPerManager.HasPermissionInSelfOrg(SignUser.Id, Constants.ROLE_CREATE))
                {
                    Logger.Warn($"[{nameof(Create)}] 用户[{SignUser.SignName}]({SignUser.Id})没有权限({Constants.ROLE_CREATE})");
                    ModelState.AddModelError("All", "没有权限");
                    return View(role);
                }
                // 2. 业务处理
                // 创建角色 -与组织关联
                // TODO: 新增一个接口：在指定组织下创建角色（RoleManager.CreateByOrgIdUserId(role, orgId, SignUser.Id)）
                await RoleManager.CreateForOrgByUserId(role, SignUser.Id);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception e)
            {
                Logger.Error($"[{nameof(Create)}] 服务器错误: \r\n{e.ToString()}");
                ModelState.AddModelError("All", e.Message);
                return View(role);
            }
        }

        /// <summary>
        /// [MVC] 角色管理-角色编辑界面
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <returns></returns>
        // GET: Roles/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            Logger.Trace($"[{nameof(Edit)}] 请求参数; 角色({id})");
            // 0. 参数检查
            if (id == null)
            {
                return NotFound();
            }
            // 2. 业务处理
            try
            {
                // 1. 权限检查 ROOT_ROLE_SAVE_UPDATE_VIEW
                if (!await RoleOrgPerManager.HasPermission<Role>(SignUser.Id, Constants.ROLE_MANAGE, id))
                {
                    Logger.Warn($"[{nameof(Edit)}] 用户[{SignUser.SignName}]({SignUser.Id})没有权限({Constants.ROLE_MANAGE})");
                    ModelState.AddModelError("All", "没有权限");
                    return RedirectToAction(nameof(Index));
                }
                var role = await RoleManager.FindById(id);
                Logger.Trace($"[{nameof(Details)}] 响应数据:\r\n{JsonUtil.ToJson(role)}");
                if (role == null)
                {
                    return NotFound();
                }
                return View(role);
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Edit)}] 服务器错误: \r\n{e.ToString()}");
                ModelState.AddModelError("All", e.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// [MVC] 角色管理-角色编辑
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <param name="role">角色</param>
        /// <returns></returns>
        // POST: Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Decription")] RoleJson role)
        {
            Logger.Trace($"[{nameof(Create)}] 请求参数; 角色({id}) \r\n{JsonUtil.ToJson(role)}");
            // 0. 参数检查
            if (id != role.Id)
            {
                return NotFound();
            }
            try
            {
                // 1. 权限检查ROOT_ROLE_UPDATE (TODO: ROLE > ROLE_SAVE > ROLE_SAVE_UPDATE)
                if (!await RoleOrgPerManager.HasPermission<Role>(SignUser.Id, Constants.ROLE_MANAGE, id))
                {
                    Logger.Warn($"[{nameof(Edit)}] 用户[{SignUser.SignName}]({SignUser.Id})没有权限({Constants.ROLE_MANAGE})");
                    ModelState.AddModelError("All", "没有权限");
                    return RedirectToAction(nameof(Index));
                }
                // 2. 业务处理
                await RoleManager.Update(role);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                if (!await RoleManager.ExistById(role.Id))
                {
                    return NotFound();
                }
                else
                {
                    Logger.Error($"[{nameof(Edit)}] 服务器错误: \r\n{e.ToString()}");
                    ModelState.AddModelError("All", e.Message);
                    return View(role);
                }
            }
        }

        /// <summary>
        /// [MVC] 角色管理-角色删除界面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: Roles/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            Logger.Trace($"[{nameof(Edit)}] 请求参数; 角色({id})");
            // 0. 参数检查
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                // 1. 权限检查 ROOT_ROLE_DELETE > ROOT_ROLE_DELETE_VIEW
                if(!await RoleOrgPerManager.HasPermission<Role>(SignUser.Id, Constants.ROLE_MANAGE, id))
                {
                    Logger.Warn($"[{nameof(Delete)}] 用户[{SignUser.SignName}]({SignUser.Id})没有权限({Constants.ROLE_MANAGE})");
                    ModelState.AddModelError("All", "没有权限");
                    return RedirectToAction(nameof(Index));
                }
                // 2. 业务处理
                var role = await RoleManager.FindById(id);
                Logger.Trace($"[{nameof(Delete)}] 响应数据:\r\n{JsonUtil.ToJson(role)}");
                if (role == null)
                {
                    return NotFound();
                }
                return View(role);
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Edit)}] 服务器错误: \r\n{e.ToString()}");
                ModelState.AddModelError("All", e.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// 删除确认
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // POST: Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            Logger.Trace($"[{nameof(DeleteConfirmed)}] 请求参数; 角色({id})");
            // 0. 参数检查
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                // 1. 权限验证
                if (!await RoleOrgPerManager.HasPermission<Role>(SignUser.Id, Constants.ROLE_MANAGE, id))
                {
                    Logger.Warn($"[{nameof(DeleteConfirmed)}] 没有权限");
                    ModelState.AddModelError("All", "没有权限");
                    return RedirectToAction(nameof(Index));
                }
                // 2. 业务处理 -级联删除
                await RoleManager.DeleteByUserId(SignUser.Id, id);
            }
            catch(Exception e)
            {
                Logger.Error($"[{nameof(Edit)}] 服务器错误: \r\n{e.ToString()}");
                ModelState.AddModelError("All", e.Message);
                return View(nameof(Delete), id);
            }
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// 登陆用户信息
        /// </summary>
        private UserJson SignUser
        {
            get
            {
                // 判断是否存在登陆信息
                if (HttpContext.Session.GetString(Constants.USERID) == null)
                {
                    return null;
                }
                // 返回登陆信息
                return new UserJson
                {
                    Id = HttpContext.Session.GetString(Constants.USERID),
                    SignName = HttpContext.Session.GetString(Constants.SIGNNAME),
                    PassWord = HttpContext.Session.GetString(Constants.PASSWORD)
                };
            }
            set
            {
                // 清除登陆信息
                if (value == null)
                {
                    HttpContext.Session.Remove(Constants.USERID);
                    HttpContext.Session.Remove(Constants.SIGNNAME);
                    HttpContext.Session.Remove(Constants.PASSWORD);
                }
                // 添加登陆信息
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
