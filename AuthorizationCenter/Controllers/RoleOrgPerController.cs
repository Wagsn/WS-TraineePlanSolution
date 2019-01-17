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
using Microsoft.AspNetCore.Http;
using AuthorizationCenter.Define;
using WS.Log;
using WS.Text;

namespace AuthorizationCenter.Controllers
{
    /// <summary>
    /// 角色组织权限控制器
    /// 授权控制器
    /// </summary>
    public class RoleOrgPerController : Controller
    {
        /// <summary>
        /// 角色组织权限关联
        /// </summary>
        public IRoleOrgPerManager RoleOrgPerManager { get; set; }

        /// <summary>
        /// 权限项管理
        /// </summary>
        public IPermissionManager<PermissionJson> PermissionManager { get; set; }

        /// <summary>
        /// 角色管理
        /// </summary>
        public IRoleManager<RoleJson> RoleManager { get; set; }

        /// <summary>
        /// 日志器
        /// </summary>
        public ILogger Logger = LoggerManager.GetLogger<RoleOrgPerController>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleOrgPerManager"></param>
        /// <param name="permissionManager"></param>
        /// <param name="roleManager"></param>
        public RoleOrgPerController(IRoleOrgPerManager roleOrgPerManager, IPermissionManager<PermissionJson> permissionManager, IRoleManager<RoleJson> roleManager)
        {
            RoleOrgPerManager = roleOrgPerManager ?? throw new ArgumentNullException(nameof(roleOrgPerManager));
            PermissionManager = permissionManager ?? throw new ArgumentNullException(nameof(permissionManager));
            RoleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        }


        /// <summary>
        /// 跳转到授权列表
        /// 查看自己能看到的授权（获取有授权权限的组织，根据组织找关联）
        /// </summary>
        /// <returns></returns>
        // GET: RoleOrgPer
        public async Task<IActionResult> Index(int pageIndex = 0, int pageSize = 10)
        {
            try
            {
                // 1. 权限验证
                if (!await RoleOrgPerManager.HasPermission(SignUser.Id, Constants.AUTH_MANAGE))
                {
                    return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
                }
                // 2. 业务处理 -查询用户(userId)的角色组织权限列表
                var roleOrgPers = await RoleOrgPerManager.FindFromOrgByUserId(SignUser.Id);
                // 分页（TODO）
                return View(roleOrgPers);
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Index)}] 服务器错误:\r\n{e}");
                return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
            }
        }

        /// <summary>
        /// [MVC] 跳转到详情界面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: RoleOrgPer/Details/5
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
                if (!await RoleOrgPerManager.HasPermission(SignUser.Id, Constants.AUTH_MANAGE))
                {
                    return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
                }
                // 2. 业务处理 -查询用户可以管理的
                var roleOrgPer = (await RoleOrgPerManager.FindFromOrgByUserId(SignUser.Id)).FirstOrDefault(rop => rop.Id==id);
                if (roleOrgPer == null)
                {
                    return NotFound();
                }

                return View(roleOrgPer);
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Index)}] 服务器错误:\r\n{e}");
                return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
            }
        }

        /// <summary>
        /// [MVC] 跳转到新增界面
        /// </summary>
        /// <returns></returns>
        // GET: RoleOrgPer/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                // 1. 权限验证
                if (!await RoleOrgPerManager.HasPermission(SignUser.Id, Constants.AUTH_MANAGE))
                {
                    return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
                }
                // 2. 业务处理
                // 将Entity的数据封装到 SelectList中，制定要生成下拉框选项的value和text属性
                // 查询到可以查询到的组织
                ViewData["OrgId"] = new SelectList(await RoleOrgPerManager.FindOrgByUserIdPerName(SignUser.Id, Constants.ORG_QUERY), nameof(Organization.Id), nameof(Organization.Name));
                // 查询到可以查询到权限项 
                ViewData["PerId"] = new SelectList(await PermissionManager.FindPerByUserId(SignUser.Id), nameof(Permission.Id), nameof(Permission.Name));
                // 查询用户所在组织的角色
                ViewData["RoleId"] = new SelectList(await RoleManager.FindRoleOfOrgByUserId(SignUser.Id), nameof(Role.Id), nameof(Role.Name));
                return View();
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Index)}] 服务器错误:\r\n{e}");
                return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
            }
        }

        /// <summary>
        /// [MVC] 新增
        /// </summary>
        /// <param name="roleOrgPer"></param>
        /// <returns></returns>
        // POST: RoleOrgPer/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(/*[Bind("Id,RoleId,OrgId,PerId")]*/ RoleOrgPer roleOrgPer)
        {
            try
            {
                // 1. 权限处理
                if(!await RoleOrgPerManager.HasPermission(SignUser.Id, Constants.AUTH_MANAGE))
                {
                    Logger.Warn($"[] 用户[{SignUser.SignName}]({SignUser.Id})没有权限({Constants.AUTH_MANAGE})");
                    return RedirectToAction(nameof(Index));
                }
                // 2. 业务处理
                await RoleOrgPerManager.CreateByUserId(SignUser.Id,  roleOrgPer);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception e)
            {
                Logger.Error($"[{nameof(Create)}] 用户[{SignUser.SignName}]({SignUser.Id})删除角色权限:\r\n{JsonUtil.ToJson(roleOrgPer)}\r\n失败:\r\n{e}");
                ViewData["OrgId"] = new SelectList(await RoleOrgPerManager.FindOrgByUserIdPerName(SignUser.Id, Constants.ORG_QUERY), nameof(Organization.Id), nameof(Organization.Name), roleOrgPer.OrgId);
                ViewData["PerId"] = new SelectList(await PermissionManager.FindPerByUserId(SignUser.Id), nameof(Permission.Id), nameof(Permission.Name), roleOrgPer.PerId);
                ViewData["RoleId"] = new SelectList(await RoleManager.FindRoleOfOrgByUserId(SignUser.Id), nameof(Role.Id), nameof(Role.Name), roleOrgPer.RoleId);
                return View(roleOrgPer);
            }
        }

        /// <summary>
        /// [MVC] 跳转到编辑界面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: RoleOrgPer/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            // 0. 参数检查
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                // 1. 权限验证
                if (!await RoleOrgPerManager.HasPermission(SignUser.Id, Constants.AUTH_MANAGE))
                {
                    Logger.Warn($"[] 用户[{SignUser.SignName}]({SignUser.Id})没有权限({Constants.AUTH_MANAGE})");
                    return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
                }
                // 2. 业务处理
                var roleOrgPer = (await RoleOrgPerManager.FindFromOrgByUserId(SignUser.Id)).FirstOrDefault(rop => rop.Id == id);
                if (roleOrgPer == null)
                {
                    return NotFound();
                }
                ViewData["OrgId"] = new SelectList(await RoleOrgPerManager.FindOrgByUserIdPerName(SignUser.Id, Constants.ORG_QUERY), nameof(Organization.Id), nameof(Organization.Name), roleOrgPer.OrgId);
                ViewData["PerId"] = new SelectList(await PermissionManager.FindPerByUserId(SignUser.Id), nameof(Permission.Id), nameof(Permission.Name), roleOrgPer.PerId);
                ViewData["RoleId"] = new SelectList(await RoleManager.FindRoleOfOrgByUserId(SignUser.Id), nameof(Role.Id), nameof(Role.Name), roleOrgPer.RoleId);
                return View(roleOrgPer);
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Index)}] 服务器错误:\r\n{e}");
                return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
            }
        }

        /// <summary>
        /// [MVC] 编辑角色组织权限关联
        /// </summary>
        /// <param name="id"></param>
        /// <param name="roleOrgPer"></param>
        /// <returns></returns>
        // POST: RoleOrgPer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, /*[Bind("Id,RoleId,OrgId,PerId")]*/ RoleOrgPer roleOrgPer)
        {
            // 0. 参数检查
            if (id != roleOrgPer.Id)
            {
                return NotFound();
            }

            try
            {
                // 1. 权限验证 
                if (!await RoleOrgPerManager.HasPermission(SignUser.Id, Constants.AUTH_MANAGE))
                {
                    Logger.Warn($"[] 用户[{SignUser.SignName}]({SignUser.Id})没有权限({Constants.AUTH_MANAGE})");
                    return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
                }
                // 2. 业务处理
                await RoleOrgPerManager.UpdateByUserId(SignUser.Id, roleOrgPer);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Index)}] 服务器错误:\r\n{e}");
                return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
            }
        }

        /// <summary>
        /// [MVC] 删除 角色组织权限关联
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: RoleOrgPer/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            // 0.参数检查
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                // 1. 权限验证
                if (!await RoleOrgPerManager.HasPermission(SignUser.Id, Constants.AUTH_MANAGE))
                {
                    Logger.Warn($"[] 用户[{SignUser.SignName}]({SignUser.Id})没有权限({Constants.AUTH_MANAGE})");
                    return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
                }
                // 2. 业务处理
                var roleOrgPer = await RoleOrgPerManager.FindByUserId(SignUser.Id).Where(rop => rop.Id == id).SingleOrDefaultAsync();
                if (roleOrgPer == null)
                {
                    return NotFound();
                }
                return View(roleOrgPer);
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Index)}] 服务器错误:\r\n{e}");
                return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
            }
        }

        /// <summary>
        /// [MVC] 添加 角色组织权限关联
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // POST: RoleOrgPer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                // 1. 权限验证
                if (!await RoleOrgPerManager.HasPermission(SignUser.Id, Constants.AUTH_MANAGE))
                {
                    Logger.Warn($"[] 用户[{SignUser.SignName}]({SignUser.Id})没有权限({Constants.AUTH_MANAGE})");
                    return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
                }
                // 2. 业务处理
                await RoleOrgPerManager.DeleteByUserId(SignUser.Id, rop => rop.Id == id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Index)}] 服务器错误:\r\n{e}");
                return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
            }
        }

        /// <summary>
        /// 重新扩展用户组织权限表
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> ReExpansion()
        {
            try
            {
                await RoleOrgPerManager.ReExpansion();
                return RedirectToAction(nameof(Index));
            }
            catch(Exception e)
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
