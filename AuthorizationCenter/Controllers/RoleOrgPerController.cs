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
    public class RoleOrgPerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoleOrgPerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: RoleOrgPer
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.RoleOrgPers.Include(r => r.Org).Include(r => r.Per).Include(r => r.Role);
            return View(await applicationDbContext.ToListAsync());
        }

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

        // GET: RoleOrgPer/Create
        public IActionResult Create()
        {
            // 将Entity的数据封装到 SelectList中，制定要生成下拉框选项的value和text属性
            ViewData["OrgId"] = new SelectList(_context.Organizations, nameof(Organization.Id), nameof(Organization.Name));
            ViewData["PerId"] = new SelectList(_context.Permissions, nameof(Permission.Id), nameof(Permission.Name));
            ViewData["RoleId"] = new SelectList(_context.Roles, nameof(Role.Id), nameof(Role.Name));
            return View();
        }

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

        // POST: RoleOrgPer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,RoleId,OrgId,PerId")] RoleOrgPer roleOrgPer)
        {
            if (id != roleOrgPer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
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
    }
}
