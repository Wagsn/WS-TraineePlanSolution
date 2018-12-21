﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AuthorizationCenter.Entitys;
using WS.Text;
using AuthorizationCenter.Dto.Jsons;
using AuthorizationCenter.Managers;
using AuthorizationCenter.Define;
using Microsoft.AspNetCore.Http;

namespace AuthorizationCenter.Controllers
{
    /// <summary>
    /// 组织控制
    /// </summary>
    public class OrganizationController : Controller
    {
        /// <summary>
        /// 管理
        /// </summary>
        public IOrganizationManager<OrganizationJson> OrganizationManager { get; set; }
        
        /// <summary>
        /// 组织管理
        /// </summary>
        /// <param name="organizationManager"></param>
        public OrganizationController(IOrganizationManager<OrganizationJson> organizationManager)
        {
            OrganizationManager = organizationManager;
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        // GET: Organization
        public async Task<IActionResult> Index()
        {
            // TODO 筛选出登陆用户可见的组织 -根据用户找到角色 -根据角色找到组织 -根据父组织找到所有子组织
            var orgs = await OrganizationManager.FindByUserId(SignUser.Id).ToListAsync();
            var organizations = await OrganizationManager.Find().ToListAsync();

            Console.WriteLine(JsonUtil.ToJson(orgs));
            
            return View(orgs);
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: Organization/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var organization = await OrganizationManager.FindById(id).SingleOrDefaultAsync();
            if (organization == null)
            {
                return NotFound();
            }

            return View(organization);
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <returns></returns>
        // GET: Organization/Create
        public async Task<IActionResult> Create()
        {
            ViewData["OrgId"] = new SelectList(OrganizationManager.Find(), nameof(Organization.Id), nameof(Organization.Name), await OrganizationManager.Find().FirstOrDefaultAsync());
            return View();
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="organization"></param>
        /// <returns></returns>
        // POST: Organization/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ParentId,Name,Description")] OrganizationJson organization)
        {
            if (ModelState.IsValid)
            {
                // 检查是否存在循环（允许有向无环图的产生，而不仅仅是树形结构）
                await OrganizationManager.Create(organization);
                return RedirectToAction(nameof(Index));
            }
            return View(organization);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: Organization/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var organization = await OrganizationManager.FindById(id).SingleOrDefaultAsync();
            if (organization == null)
            {
                return NotFound();
            }
            return View(organization);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id"></param>
        /// <param name="organization"></param>
        /// <returns></returns>
        // POST: Organization/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,ParentId,Name,Description")] OrganizationJson organization)
        {
            if (id != organization.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await OrganizationManager.Update(organization);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await OrganizationManager.Exist(org => org.Id ==id))
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
            return View(organization);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: Organization/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var organization = await OrganizationManager.FindById(id).SingleOrDefaultAsync();
            if (organization == null)
            {
                return NotFound();
            }

            return View(organization);
        }

        /// <summary>
        /// 删除确认
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // POST: Organization/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await OrganizationManager.DeleteById(id);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// 获取登陆用户简要信息 -每次都是新建一个UserBaseJson对象
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// 登陆用户
        /// </summary>
        private UserBaseJson SignUser
        {
            get
            {
                if (HttpContext.Session.GetString(Constants.USERID) == null)
                {
                    return null;
                }
                return new UserBaseJson
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
