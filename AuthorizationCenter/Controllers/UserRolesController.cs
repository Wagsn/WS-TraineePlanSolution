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
    /// 用户角色绑定控制
    /// </summary>
    public class UserRolesController : Controller
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// 用户角色管理
        /// </summary>
        public IUserRoleManager UserRoleManager { get; set; }

        /// <summary>
        /// 用户管理
        /// </summary>
        public IUserManager<UserBaseJson> UserManager { get; set; }

        /// <summary>
        /// 角色管理
        /// </summary>
        public IRoleManager<RoleJson> RoleManager { get; set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userRoleManager"></param>
        /// <param name="userManager"></param>
        /// <param name="roleManager"></param>
        public UserRolesController(ApplicationDbContext context, IUserRoleManager userRoleManager, IUserManager<UserBaseJson> userManager, IRoleManager<RoleJson> roleManager)
        {
            _context = context;
            UserRoleManager = userRoleManager;
            UserManager = userManager;
            RoleManager = roleManager;
        }
        
        /// <summary>
        /// 主页
        /// </summary>
        /// <returns></returns>
        // GET: UserRoles
        public async Task<IActionResult> Index()
        {
            return View(await UserRoleManager.Find().ToListAsync());
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: UserRoles/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var userRole = await UserRoleManager.FindById(id).FirstOrDefaultAsync();
            if (userRole == null)
            {
                return NotFound();
            }

            return View(userRole);
        }

        /// <summary>
        /// 新增 用户角色关联
        /// </summary>
        /// <returns></returns>
        // GET: UserRoles/Create
        public IActionResult Create()
        {
            // 查询 所有的用户角色
            ViewData["RoleId"] = new SelectList(RoleManager.Find(), "Id", "Name");
            ViewData["UserId"] = new SelectList(UserManager.Find(), "Id", "SignName");
            return View();
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
        public async Task<IActionResult> Create([Bind("Id,UserId,RoleId")] UserRole userRole)
        {
            if (ModelState.IsValid)
            {
                await UserRoleManager.Create(userRole);
                return RedirectToAction(nameof(Index));
            }
            //ViewData["Roles"] = _context.Roles;
            //ViewData["Users"] = _context.UserBases;

            // 这个东东很奇特啊，显示的是Name字段，得到的是？
            ViewData["RoleId"] = new SelectList(RoleManager.Find(), "Id", "Name", userRole.RoleId);
            ViewData["UserId"] = new SelectList(UserManager.Find(), "Id", "SignName", userRole.UserId);
            return View(userRole);
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
            
            var userRole = await UserRoleManager.FindById(id).SingleOrDefaultAsync();
            if (userRole == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(RoleManager.Find(), "Id", "Name", userRole.RoleId);
            ViewData["UserId"] = new SelectList(UserManager.Find(), "Id", "SignName", userRole.UserId);
            return View(userRole);
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
        public async Task<IActionResult> Edit(string id, [Bind("Id,UserId,RoleId")] UserRole userRole)
        {
            if (id != userRole.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 重复判断 是否存在UserId和RoleId的关系
                    if(await UserRoleManager.Exist(ur=> ur.RoleId==userRole.RoleId && ur.UserId == userRole.UserId))
                    {
                        ModelState.AddModelError("All", "角色已经被绑定在该用户上");
                        ViewData["RoleId"] = new SelectList(RoleManager.Find(), "Id", "Name", userRole.RoleId);
                        ViewData["UserId"] = new SelectList(UserManager.Find(), "Id", "SignName", userRole.UserId);
                        return View(userRole);
                    }
                    else
                    {
                        await UserRoleManager.Update(userRole);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await UserRoleManager.Exist(ur => ur.Id == userRole.Id))
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
            ViewData["RoleId"] = new SelectList(RoleManager.Find(), "Id", "Name", userRole.RoleId);
            ViewData["UserId"] = new SelectList(UserManager.Find(), "Id", "SignName", userRole.UserId);
            return View(userRole);
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
            var urs = await UserRoleManager.FindById(id).ToListAsync();
            if (urs == null)
            {
                return NotFound();
            }

            return View(urs);
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
            await UserRoleManager.DeleteById(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
