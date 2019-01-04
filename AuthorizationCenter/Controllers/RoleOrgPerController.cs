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

namespace AuthorizationCenter.Controllers
{
    /// <summary>
    /// 角色组织权限控制器
    /// 授权控制器
    /// </summary>
    public class RoleOrgPerController : Controller
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// 角色组织权限关联
        /// </summary>
        public IRoleOrgPerManager<RoleOrgPerJson> RoleOrgPerManager { get; set; }

        /// <summary>
        /// 日志器
        /// </summary>
        public ILogger Logger = LoggerManager.GetLogger(nameof(RoleOrgPerController));

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="context"></param>
        public RoleOrgPerController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 跳转到列表
        /// </summary>
        /// <returns></returns>
        // GET: RoleOrgPer
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.RoleOrgPers.Include(r => r.Org).Include(r => r.Per).Include(r => r.Role);
            return View(await applicationDbContext.ToListAsync());
        }

        /// <summary>
        /// [MVC] 跳转到详情界面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: RoleOrgPer/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roleOrgPer = await _context.RoleOrgPers
                .Include(r => r.Org)
                .Include(r => r.Per)
                .Include(r => r.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (roleOrgPer == null)
            {
                return NotFound();
            }

            return View(roleOrgPer);
        }

        /// <summary>
        /// [MVC] 跳转到新增界面
        /// </summary>
        /// <returns></returns>
        // GET: RoleOrgPer/Create
        public IActionResult Create()
        {
            // 将Entity的数据封装到 SelectList中，制定要生成下拉框选项的value和text属性
            ViewData["OrgId"] = new SelectList(_context.Organizations, nameof(Organization.Id), nameof(Organization.Name));
            ViewData["PerId"] = new SelectList(_context.Permissions, nameof(Permission.Id), nameof(Permission.Name));
            ViewData["RoleId"] = new SelectList(_context.Roles, nameof(Role.Id), nameof(Role.Name));
            return View();
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
        public async Task<IActionResult> Create([Bind("Id,RoleId,OrgId,PerId")] RoleOrgPer roleOrgPer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(roleOrgPer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrgId"] = new SelectList(_context.Organizations, nameof(Organization.Id), nameof(Organization.Name), roleOrgPer.OrgId);
            ViewData["PerId"] = new SelectList(_context.Permissions, nameof(Permission.Id), nameof(Permission.Name), roleOrgPer.PerId);
            ViewData["RoleId"] = new SelectList(_context.Roles, nameof(Role.Id), nameof(Role.Name), roleOrgPer.RoleId);
            return View(roleOrgPer);
        }

        /// <summary>
        /// [MVC] 跳转到编辑界面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: RoleOrgPer/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roleOrgPer = await _context.RoleOrgPers.FindAsync(id);
            if (roleOrgPer == null)
            {
                return NotFound();
            }
            ViewData["OrgId"] = new SelectList(_context.Organizations, nameof(Organization.Id), nameof(Organization.Name), roleOrgPer.OrgId);
            ViewData["PerId"] = new SelectList(_context.Permissions, nameof(Permission.Id), nameof(Permission.Name), roleOrgPer.PerId);
            ViewData["RoleId"] = new SelectList(_context.Roles, nameof(Role.Id), nameof(Role.Name), roleOrgPer.RoleId);
            return View(roleOrgPer);
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
        public async Task<IActionResult> Edit(string id, [Bind("Id,RoleId,OrgId,PerId")] RoleOrgPer roleOrgPer)
        {
            // 0. 参数检查
            if (id != roleOrgPer.Id)
            {
                return NotFound();
            }
            
            

            if (ModelState.IsValid)
            {
                // 1. 权限验证 具有授权权限

                //if (await RoleOrgPerManager.HasPermission(SignUser.Id, "", "Authorization"))
                //{
                //    ModelState.AddModelError("All", "没有权限");
                //    Logger.Warn($"[{nameof(Edit)}] 没有权限 进行角色权限编辑");
                //    return View("Edit", id);
                //}

                try
                {
                    _context.Update(roleOrgPer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoleOrgPerExists(roleOrgPer.Id))
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
            ViewData["OrgId"] = new SelectList(_context.Organizations, nameof(Organization.Id), nameof(Organization.Name), roleOrgPer.OrgId);
            ViewData["PerId"] = new SelectList(_context.Permissions, nameof(Permission.Id), nameof(Permission.Name), roleOrgPer.PerId);
            ViewData["RoleId"] = new SelectList(_context.Roles, nameof(Role.Id), nameof(Role.Name), roleOrgPer.RoleId);
            return View(roleOrgPer);
        }

        /// <summary>
        /// [MVC] 删除 角色组织权限关联
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: RoleOrgPer/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roleOrgPer = await _context.RoleOrgPers
                .Include(r => r.Org)
                .Include(r => r.Per)
                .Include(r => r.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (roleOrgPer == null)
            {
                return NotFound();
            }

            return View(roleOrgPer);
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
            var roleOrgPer = await _context.RoleOrgPers.FindAsync(id);
            _context.RoleOrgPers.Remove(roleOrgPer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoleOrgPerExists(string id)
        {
            return _context.RoleOrgPers.Any(e => e.Id == id);
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
