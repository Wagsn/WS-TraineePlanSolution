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
    /// 权限控制
    /// </summary>
    public class PermissionController : Controller
    {
        /// <summary>
        /// 权限管理
        /// </summary>
        protected IPermissionManager<PermissionJson> PermissionManager { get; set; }

        IRoleOrgPerManager RoleOrgPerManager { get; set; }

        readonly ILogger Logger = LoggerManager.GetLogger<PermissionController>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="permissionManager"></param>
        /// <param name="roleOrgPerManager"></param>
        public PermissionController(IPermissionManager<PermissionJson> permissionManager, IRoleOrgPerManager roleOrgPerManager)
        {
            PermissionManager = permissionManager ?? throw new ArgumentNullException(nameof(permissionManager));
            RoleOrgPerManager = roleOrgPerManager ?? throw new ArgumentNullException(nameof(roleOrgPerManager));
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        // GET: Permission
        public async Task<IActionResult> Index(int pageIndex = 0, int pageSize = 10)
        {
            var data = await PermissionManager.Find().ToListAsync();
            return View(data);
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: Permission/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                // 1. 权限验证

                // 2. 业务处理
                var permission = await PermissionManager.FindById(id).FirstOrDefaultAsync();
                Logger.Trace($"[{nameof(Index)}] 响应数据:\r\n{JsonUtil.ToJson(permission)}");
                if (permission == null)
                {
                    return NotFound();
                }
                return View(permission);
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Details)}] 服务器错误:\r\n{e}");
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <returns></returns>
        // GET: Permission/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                // 1. 权限验证
                if (!await RoleOrgPerManager.HasPermissionInSelfOrg(SignUser.Id, Constants.PER_MANAGE))
                {
                    ModelState.AddModelError("All", "没有权限");
                    ViewData["AllErr"] = "没有权限";
                    return RedirectToAction(nameof(Index));
                }
                return View();
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Details)}] 服务器错误:\r\n{e}");
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="permission">权限</param>
        /// <returns></returns>
        // POST: Permission/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] PermissionJson permission)
        {
            Logger.Trace($"[{nameof(Create)}] 请求参数:\r\n{JsonUtil.ToJson(permission)}");
            try
            {
                // 1. 权限验证
                if(!await RoleOrgPerManager.HasPermissionInSelfOrg(SignUser.Id, Constants.PER_MANAGE))
                {
                    ModelState.AddModelError("All", "没有权限");
                    return RedirectToAction(nameof(Index));
                }
                // 2. 业务处理
                await PermissionManager.Create(permission);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Details)}] 服务器错误:\r\n{e}");
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: Permission/Edit/5
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
                if (!await RoleOrgPerManager.HasPermissionInSelfOrg(SignUser.Id, Constants.PER_MANAGE))
                {
                    ModelState.AddModelError("All", "没有权限");
                    return RedirectToAction(nameof(Index));
                }
                // 2. 业务处理
                var permission = await PermissionManager.FindById(id).FirstOrDefaultAsync();
                Logger.Trace($"[{nameof(Edit)}] 响应数据:\r\n{JsonUtil.ToJson(permission)}");
                if (permission == null)
                {
                    return NotFound();
                }
                return View(permission);
            }
            catch(Exception e)
            {
                Logger.Error($"[{nameof(Details)}] 服务器错误:\r\n{e}");
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        // POST: Permission/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, /*[Bind("Id,Name,Description")]*/ PermissionJson permission)
        {
            Logger.Trace($"[{nameof(Edit)}] 请求参数:\r\n{JsonUtil.ToJson(permission)}");
            if (permission == null || permission.Id==null || permission.ParentId==null)
            {
                ModelState.AddModelError("All", "参数不能为空");
                return View();
            }
            if (id != permission.Id)
            {
                return NotFound();
            }

            try
            {
                // 1. 权限验证
                if (!await RoleOrgPerManager.HasPermissionInSelfOrg(SignUser.Id, Constants.PER_MANAGE))
                {
                    ModelState.AddModelError("All", "没有权限");
                    return RedirectToAction(nameof(Index));
                }
                // 2. 业务处理
                if (!await PermissionManager.Exist(per => per.Id == permission.Id))
                {
                    return NotFound();
                }
                await PermissionManager.Update(permission);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Edit)}] 服务器错误:\r\n{e}");
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: Permission/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            Logger.Trace($"[{nameof(Delete)}] 请求参数: 权限ID({id})");
            // 0. 参数检查
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                // 1. 权限验证
                if (!await RoleOrgPerManager.HasPermissionInSelfOrg(SignUser.Id, Constants.PER_MANAGE))
                {
                    ModelState.AddModelError("All", "没有权限");
                    return RedirectToAction(nameof(Index));
                }
                // 2. 业务处理
                var permission = await PermissionManager.FindById(id).SingleOrDefaultAsync();
                Logger.Trace($"[{nameof(Index)}] 响应数据:\r\n{JsonUtil.ToJson(permission)}");
                if (permission == null)
                {
                    return NotFound();
                }
                return View(permission);
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Edit)}] 服务器错误:\r\n{e}");
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// 删除确认
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // POST: Permission/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            // 0. 参数检查
            if(id == null)
            {
                return NotFound();
            }
            try
            {
                // 1. 权限验证
                if (!await RoleOrgPerManager.HasPermissionInSelfOrg(SignUser.Id, Constants.PER_MANAGE))
                {
                    ModelState.AddModelError("All", "没有权限");
                    return RedirectToAction(nameof(Index));
                }
                // 2. 业务处理
                await PermissionManager.DeleteById(id);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception e)
            {
                Logger.Error($"[{nameof(Edit)}] 服务器错误:\r\n{e}");
                return RedirectToAction(nameof(Index));
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
