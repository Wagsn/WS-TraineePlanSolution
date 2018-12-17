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
    /// 角色控制器
    /// </summary>
    public class RolesController : Controller
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// 角色管理
        /// </summary>
        public IRoleManager<RoleJson> RoleManager { get; set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="context"></param>
        public RolesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 角色列表
        /// </summary>
        /// <returns></returns>
        // GET: Roles
        public async Task<IActionResult> Index()
        {
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
        /// <param name="reqbody"></param>
        /// <returns></returns>
        // POST: Roles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Decription")] RoleJson reqbody)
        {
            if (ModelState.IsValid)
            {
                await RoleManager.Create(reqbody);
                return RedirectToAction(nameof(Index));
            }
            return View(reqbody);
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
    }
}
