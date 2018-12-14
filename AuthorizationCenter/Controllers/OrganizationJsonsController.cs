﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AuthorizationCenter.Dto.Jsons;
using AuthorizationCenter.Entitys;
using AutoMapper;

namespace AuthorizationCenter.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("[controller]")]
    public class OrganizationJsonsController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        public ApplicationDbContext _context { get; set; }

        /// <summary>
        /// 类型映射
        /// </summary>
        public IMapper Mapper { get; set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="context"></param>
        /// <param name="mapper"></param>
        public OrganizationJsonsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            Mapper = mapper;
        }

        /// <summary>
        /// 列表 MVC
        /// </summary>
        /// <returns></returns>
        [HttpGet("index", Name = "OrganizationIndex")]
        // GET: OrganizationJsons
        public async Task<IActionResult> Index()
        {
            return View( Mapper.Map<List<OrganizationJson>>(await _context.Organizations.ToListAsync()));
        }

        /// <summary>
        /// 详情 MVC
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: OrganizationJsons/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound(); 
            }

            var organizationJson = await _context.Organizations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (organizationJson == null)
            {
                return NotFound();
            }

            return View(organizationJson);
        }

        /// <summary>
        /// 创建 MVC
        /// </summary>
        /// <returns></returns>
        // GET: OrganizationJsons/Create
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// 创建 MVC
        /// </summary>
        /// <param name="organizationJson"></param>
        /// <returns></returns>
        // POST: OrganizationJsons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost(Name = "OrganizationCreate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ParentId,Name,Description")] OrganizationJson organizationJson)
        {
            if (ModelState.IsValid)
            {
                _context.Add(organizationJson);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(organizationJson);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: OrganizationJsons/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var organizationJson = await _context.Organizations.FindAsync(id);
            if (organizationJson == null)
            {
                return NotFound();
            }
            return View(organizationJson);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id"></param>
        /// <param name="organizationJson"></param>
        /// <returns></returns>
        // POST: OrganizationJsons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,ParentId,Name,Description")] OrganizationJson organizationJson)
        {
            if (id != organizationJson.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(organizationJson);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrganizationJsonExists(organizationJson.Id))
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
            return View(organizationJson);
        }

        /// <summary>
        /// 删除 MVC
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: OrganizationJsons/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var organizationJson = await _context.Organizations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (organizationJson == null)
            {
                return NotFound();
            }

            return View(organizationJson);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // POST: OrganizationJsons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var organizationJson = await _context.Organizations.FindAsync(id);
            _context.Organizations.Remove(organizationJson);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrganizationJsonExists(string id)
        {
            return _context.Organizations.Any(e => e.Id == id);
        }
    }
}
