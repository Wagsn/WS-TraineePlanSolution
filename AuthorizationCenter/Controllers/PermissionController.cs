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

        /// <summary>
        /// 控制器
        /// </summary>
        /// <param name="context"></param>
        /// <param name="permissionManager"></param>
        public PermissionController(ApplicationDbContext context, IPermissionManager<PermissionJson> permissionManager)
        {
            PermissionManager = permissionManager;
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        // GET: Permission
        public async Task<IActionResult> Index()
        {
            return View(await PermissionManager.Find().ToListAsync());
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

            var per = await PermissionManager.FindById(id).FirstOrDefaultAsync();
            if (per == null)
            {
                return NotFound();
            }

            return View(per);
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <returns></returns>
        // GET: Permission/Create
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        // POST: Permission/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] PermissionJson permission)
        {
            if (ModelState.IsValid)
            {
                await PermissionManager.Create(permission);
                return RedirectToAction(nameof(Index));
            }
            return View(permission);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: Permission/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var per = await PermissionManager.FindById(id).FirstOrDefaultAsync();
            if (per == null)
            {
                return NotFound();
            }
            return View(per);
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
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Description")] PermissionJson permission)
        {
            if (id != permission.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await PermissionManager.Update(permission);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await PermissionManager.Exist(per => per.Id==permission.Id))
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
            return View(permission);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: Permission/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var permission = await PermissionManager.FindById(id).SingleOrDefaultAsync();
            if (permission == null)
            {
                return NotFound();
            }

            return View(permission);
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
            await PermissionManager.DeleteById(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
