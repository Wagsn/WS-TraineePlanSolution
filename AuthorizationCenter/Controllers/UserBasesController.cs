using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AuthorizationCenter.Entitys;

namespace AuthorizationCenter.Controllers
{
    /// <summary>
    /// 控制器
    /// </summary>
    //[Route("[controller]")]
    //[ApiController]
    public class UserBasesController : Controller
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        public ApplicationDbContext _context { get; set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="context"></param>
        public UserBasesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        // GET: UserBases
        //[HttpGet("index", Name ="Index")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.UserBases.ToListAsync());
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[HttpGet("Details", Name = "Details")]
        // GET: UserBases/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userBase = await _context.UserBases.FirstOrDefaultAsync(m => m.Id == id);
            if (userBase == null)
            {
                return NotFound();
            }

            return View(userBase);
        }

        /// <summary>
        /// 新建
        /// </summary>
        /// <returns></returns>
        // GET: UserBases/Create
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="userBase"></param>
        /// <returns></returns>
        // POST: UserBases/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SignName,PassWord")] UserBase userBase)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userBase);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userBase);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: UserBases/Edit/5
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
            return View(userBase);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userBase"></param>
        /// <returns></returns>
        // POST: UserBases/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,SignName,PassWord")] UserBase userBase)
        {
            if (id != userBase.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userBase);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserBaseExists(userBase.Id))
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
            return View(userBase);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: UserBases/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userBase = await _context.UserBases
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userBase == null)
            {
                return NotFound();
            }

            return View(userBase);
        }

        /// <summary>
        /// 删除确认
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // POST: UserBases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var userBase = await _context.UserBases.FindAsync(id);
            _context.UserBases.Remove(userBase);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserBaseExists(string id)
        {
            return _context.UserBases.Any(e => e.Id == id);
        }
    }
}
