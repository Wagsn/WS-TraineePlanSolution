using System;
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
using AuthorizationCenter.Filters;
using AuthorizationCenter.Dto.Responses;
using WS.Log;

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
        /// 日志记录器
        /// </summary>
        public ILogger Logger = LoggerManager.GetLogger(nameof(OrganizationController));

        /// <summary>
        /// 组织管理
        /// </summary>
        /// <param name="organizationManager"></param>
        public OrganizationController(IOrganizationManager<OrganizationJson> organizationManager)
        {
            OrganizationManager = organizationManager;
        }

        /// <summary>
        /// 列表 -MVC
        /// </summary>
        /// <returns></returns>
        // GET: Organization
        public async Task<IActionResult> Index()
        {
            try
            {
                // TODO 筛选出登陆用户可见的组织 -根据用户找到角色 -根据角色找到组织 -根据父组织找到所有子组织
                var orgnazitions = await OrganizationManager.FindByUserId(SignUser.Id).ToListAsync();

                Console.WriteLine(JsonUtil.ToJson(orgnazitions));

                ViewData["list"] = JsonUtil.ToJson(orgnazitions);

                return View(orgnazitions);
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Index)}] 组织列表获取失败：\r\n" + e);
                return View();
            }
        }

        /// <summary>
        /// 列表 -API -只能看到自己所在的组织（查找方式，通过角色->三者关联->组织）
        /// </summary>
        /// <returns></returns>
        public async Task<PageResponesBody<OrganizationJson>> List()
        {
            try
            {
                var orgnazitions = await OrganizationManager.FindByUserId(SignUser.Id).ToListAsync();

                Console.WriteLine(JsonUtil.ToJson(orgnazitions));

                return new PageResponesBody<OrganizationJson>
                {
                    Data = orgnazitions
                };
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Index)}] 组织列表获取失败：\r\n" + e);
                return new PageResponesBody<OrganizationJson>().ServerError(e);
            }
        }

        /// <summary>
        /// 详情 -MVC
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: Organization/Details/5
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { Constants.ORG_MANAGE })]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try{
                var organization = await OrganizationManager.FindById(id).SingleOrDefaultAsync();
                if (organization == null)
                {
                    return NotFound();
                }

                return View(organization);
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Details)}] 组织详情查看失败" + e);
                return View(nameof(Index));
            }
        }

        /// <summary>
        /// 创建 -MVC -组织列表（用来选择父组织）
        /// </summary>
        /// <returns></returns>
        // GET: Organization/Create
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { Constants.ORG_MANAGE })]
        public async Task<IActionResult> Create(string orgid = null)
        {
            try
            {
                //if()
                var organizations = await OrganizationManager.Find().ToListAsync();
                ViewData["OrgId"] = new SelectList(organizations, nameof(Organization.Id), nameof(Organization.Name));
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Create)}] 组织列表查询失败：\r\n" + e);
            }
            return View();
        }

        /// <summary>
        /// 创建 -MVC
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
        /// 编辑 -MVC
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
        /// 编辑 -MVC
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
                    if (!await OrganizationManager.Exist(org => org.Id == id))
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
        /// 删除 -MVC
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: Organization/Delete/5
        [TypeFilter(typeof(CheckPermission))]
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
        /// 通过ID删除 -API
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[HttpDelete]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[]{Constants.ORG_MANAGE})]
        public async Task<ResponseBody<OrganizationJson>> DeleteById (string id)
        {
            if(id == null)
            {
                return ResponseBody.NotFound<OrganizationJson>("通过ID找不到组织");
            }

            try
            {
                var organization = await OrganizationManager.FindById(id).SingleOrDefaultAsync();
                return ResponseBody.WrapData(organization);
                //if (organization == null)
                //{
                //    return ResponseBody.NotFound<OrganizationJson>("通过ID找不到组织");
                //}
                //return new ResponseBody<OrganizationJson>
                //{
                //    Data = organization
                //};
            }
            catch (Exception e)
            {
                return ResponseBody.ServerError<OrganizationJson>(e);
            }
        }

        /// <summary>
        /// 删除确认 -MVC
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
