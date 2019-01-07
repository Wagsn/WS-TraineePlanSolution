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
        public IRoleManager<RoleJson> RoleManager { get; set; }

        /// <summary>
        /// 日志器
        /// </summary>
        public readonly ILogger Logger = LoggerManager.GetLogger(nameof(RolesController));

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="roleManager"></param>
        public RolesController(IRoleManager<RoleJson> roleManager)
        {
            RoleManager = roleManager;
        }

        /// <summary>
        /// 角色列表
        /// </summary>
        /// <returns></returns>
        // GET: Roles
        public async Task<IActionResult> Index()
        {
            // 通过登陆的用户查询组织，通过组织查询角色 => 通过用户查询角色
            RoleManager.FindByOrgUserId(SignUser.Id);  // List<RoleJson>
            return View(await RoleManager.Find().ToListAsync());
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: Roles/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rolejson = await RoleManager.FindById(id);
            if (rolejson == null)
            {
                return NotFound();
            }
            return View(rolejson);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <returns></returns>
        // GET: Roles/Create
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="userJson"></param>
        /// <returns></returns>
        // POST: Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Decription")] RoleJson userJson)
        {
            Logger.Trace($"[{nameof(Create)}] Request: \r\n{JsonUtil.ToJson(userJson)}");
            try
            {
                // 创建角色 -与组织关联
                await RoleManager.Create(userJson);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception e)
            {
                Logger.Error($"[{nameof(Create)}] 错误："+e);
                return View(userJson);
            }
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: Roles/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rolejson = await RoleManager.FindById(id);
            if (rolejson == null)
            {
                return NotFound();
            }
            return View(rolejson);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id"></param>
        /// <param name="reqbody"></param>
        /// <returns></returns>
        // POST: Roles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Decription")] RoleJson reqbody)
        {
            if (id != reqbody.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await RoleManager.Update(reqbody);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if(!await RoleManager.ExistById(reqbody.Id))
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
            return View(reqbody);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: Roles/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rolejson = await RoleManager.FindById(id);
            if (rolejson == null)
            {
                return NotFound();
            }

            return View(rolejson);
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
            await RoleManager.DeleteById(id);
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
