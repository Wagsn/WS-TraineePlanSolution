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
    /// 组织架构管理模块
    /// </summary>
    [Route("[controller]/[action]")]
    public class OrganizationController : Controller
    {
        /// <summary>
        /// 管理
        /// </summary>
        IOrganizationManager OrganizationManager { get; set; }

        /// <summary>
        /// 角色组织权限管理
        /// </summary>
        IRoleOrgPerManager RoleOrgPerManager { get; set; }

        /// <summary>
        /// 日志记录器
        /// </summary>
        readonly ILogger Logger = LoggerManager.GetLogger<OrganizationController>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="organizationManager"></param>
        /// <param name="roleOrgPerManager"></param>
        public OrganizationController(IOrganizationManager organizationManager, IRoleOrgPerManager roleOrgPerManager)
        {
            OrganizationManager = organizationManager ?? throw new ArgumentNullException(nameof(organizationManager));
            RoleOrgPerManager = roleOrgPerManager ?? throw new ArgumentNullException(nameof(roleOrgPerManager));
        }

        /// <summary>
        /// [MVC] 组织管理-组织列表
        /// 只能查看ORG_QUERY的组织森林
        /// 如果orgId存在，查看该组织下的组织，不存在则查看该用户可见的组织
        /// </summary>
        /// <returns></returns>
        // GET: Organization
        [HttpGet]
        public async Task<IActionResult> Index(string orgId, int pageIndex = 0, int pageSize = 10)
        {
            try
            {
                // 0. 参数检查
                if (orgId == null)
                {
                    // 1. 权限验证 ROOT_ORG_QUERY U.ID-[UR]->R.ID|P.ID-[ROP]->Exist(O.ID)
                    if (!await RoleOrgPerManager.HasPermission(SignUser.Id, Constants.ORG_QUERY))
                    {
                        Logger.Warn($"[{nameof(Index)}] 用户[{SignUser.SignName}]({SignUser.Id})没有权限查询组织");
                        return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
                    }
                    // 2. 业务处理 -查询有权组织 森林列表
                    var orgnazitions = await OrganizationManager.FindPerOrgsByUserId(SignUser.Id);

                    Logger.Trace($"[{nameof(Index)}] 响应数据:\r\n{JsonUtil.ToJson(orgnazitions)}");

                    ViewData["list"] = JsonUtil.ToJson(orgnazitions);

                    return View(orgnazitions);
                }
                else
                {
                    // 1. 权限验证 ROOT_ORG_QUERY U.ID-[UR]->R.ID|P.ID-[ROP]->Exist(O.ID)
                    if (!await RoleOrgPerManager.HasPermission(SignUser.Id, Constants.ORG_QUERY, orgId))
                    {
                        Logger.Warn($"[{nameof(Index)}] 用户[{SignUser.SignName}]({SignUser.Id})没有权限查询组织({orgId})");
                        return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
                    }
                    // 2. 业务处理
                    var orgnazitions = (await OrganizationManager.FindByUserIdOrgId(SignUser.Id, orgId)).ToList();
                    orgnazitions.ForEach(org => org.Children = null);

                    Logger.Trace($"[{nameof(Index)}] 响应数据:\r\n{JsonUtil.ToJson(orgnazitions)}");

                    ViewData["list"] = JsonUtil.ToJson(orgnazitions);

                    return View(orgnazitions);
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("All", "获取失败");
                Logger.Error($"[{nameof(Index)}] 服务器错误:\r\n" + e);
                return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
            }
        }

        /// <summary>
        /// [API] 列表 -代码编写中 
        /// -只能看到自己所在的组织（查找方式，通过角色->三者关联->组织）
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<PageResponesBody<OrganizationJson>> List()
        {
            try
            {
                var organizations = await OrganizationManager.FindPerOrgsByUserId(SignUser.Id);

                Console.WriteLine(JsonUtil.ToJson(organizations));

                return new PageResponesBody<OrganizationJson>
                {
                    Data = organizations.ToList()
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
        [HttpGet("{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { Constants.ORG_MANAGE })]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try{
                // 1. 权限检查
                if(!await RoleOrgPerManager.HasPermission(SignUser.Id, Constants.ORG_QUERY, id))
                {
                    Logger.Warn($"[{nameof(Details)}] 权限不足 用户[{SignUser.SignName}]({SignUser.Id})没有权限({Constants.ORG_QUERY})操作组织({id})");
                    ModelState.AddModelError("All", "权限不足");
                    return RedirectToAction(nameof(Index));
                }
                // 2. 业务处理
                var organization = await OrganizationManager.FindById(id).SingleOrDefaultAsync();
                Logger.Trace($"[{nameof(Details)}] 响应数据:\r\n{JsonUtil.ToJson(organization)}");
                if (organization == null)
                {
                    return NotFound();
                }
                return View(organization);
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Details)}] 服务器错误:\r\n{e}");
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// 创建 -MVC -组织列表（用来选择父组织）
        /// </summary>
        /// <param name="id">父组织ID</param>
        /// <returns></returns>
        // GET: Organization/Create
        [HttpGet]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { Constants.ORG_MANAGE })]
        public async Task<IActionResult> Create(string id)
        {
            Logger.Trace($"[{nameof(Create)}] 用户[{SignUser.SignName}]({SignUser.Id})在组织({id})下创建组织, 跳转到创建界面");
            try
            {
                // 1. 权限检查 -创建组织的视图权限
                if (!await RoleOrgPerManager.HasPermission(SignUser.Id, Constants.ORG_CREATE))
                {
                    Logger.Warn($"[{nameof(Details)}] 权限不足 用户[{SignUser.SignName}]({SignUser.Id})没有权限({Constants.ORG_CREATE})");
                    ModelState.AddModelError("All", "权限不足");
                    return RedirectToAction(nameof(Index));
                }
                // 2. 业务处理 -查询有权限(ORG_CREATE)的组织
                var org_create_PerOrgs = await RoleOrgPerManager.FindOrgByUserIdPerName(SignUser.Id, Constants.ORG_CREATE);
                Logger.Trace($"[{nameof(Index)}] 响应数据:\r\n{JsonUtil.ToJson(org_create_PerOrgs)}");
                // 如果id不为空，则只显示一个
                if (id != null)
                {
                    // 1. 权限检查 -创建组织的视图权限
                    if (!await RoleOrgPerManager.HasPermission(SignUser.Id, Constants.ORG_CREATE, id))
                    {
                        Logger.Warn($"[{nameof(Details)}] 权限不足 用户[{SignUser.SignName}]({SignUser.Id})没有权限({Constants.ORG_CREATE})操作组织({id})");
                        ModelState.AddModelError("All", "权限不足");
                        return RedirectToAction(nameof(Index));
                    }
                    org_create_PerOrgs = org_create_PerOrgs.Where(org => org.Id == id).ToList();
                }
                ViewData["OrgId"] = new SelectList(org_create_PerOrgs, nameof(Organization.Id), nameof(Organization.Name), id);
                return View();
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Create)}] 用户[{SignUser.SignName}]({SignUser.Id})在组织({id})下创建组织, 失败, 服务器错误:\r\n" + e);
                ModelState.AddModelError("All", "页面跳转失败");
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// 创建 -MVC
        /// </summary>
        /// <param name="organization">组织</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(/*[Bind("Id,ParentId,Name,Description")]*/ OrganizationJson organization)
        {
            Logger.Trace($"[{nameof(Create)}] 用户[{SignUser.SignName}]({SignUser.Id})创建组织:\r\n{JsonUtil.ToJson(organization)}");
            try
            {
                // 0. 参数检查
                if (organization == null || organization.ParentId == null)
                {
                    ModelState.AddModelError("All", "参数异常");
                    return View();
                }
                // 1. 权限检查
                if (!await RoleOrgPerManager.HasPermission(SignUser.Id, Constants.ORG_CREATE, organization.ParentId))
                {
                    Logger.Warn($"[{nameof(Details)}] 权限不足 用户[{SignUser.SignName}]({SignUser.Id})没有权限({Constants.ORG_CREATE})操作组织({organization.ParentId})");
                    ModelState.AddModelError("All", "权限不足");
                    return RedirectToAction(nameof(Index));
                }
                // 2. 业务处理
                // TODO：检查是否存在循环（允许有向无环图的产生，而不仅仅是树形结构）
                await OrganizationManager.CreateByUserId(SignUser.Id, organization);
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Create)}] 服务器发生错误：\r\n" + e);
            }
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// 编辑 -MVC
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: Organization/Edit/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            Logger.Trace($"[{nameof(Edit)}] 请求参数: id: " + id);
            // 0. 参数检查
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                // 1. 权限检查
                if (!await RoleOrgPerManager.HasPermission(SignUser.Id, Constants.ORG_UPDATE, id))
                {
                    Logger.Warn($"[{nameof(Details)}] 权限不足 用户[{SignUser.SignName}]({SignUser.Id})没有权限({Constants.ORG_UPDATE})操作组织({id})");
                    ModelState.AddModelError("All", "权限不足");
                    return RedirectToAction(nameof(Index));
                }
                // 2. 业务处理
                var organization = await OrganizationManager.FindById(id).SingleOrDefaultAsync();
                if (organization == null)
                {
                    return NotFound();
                }
                Logger.Trace($"[{nameof(Edit)}] 响应数据：\r\n" + JsonUtil.ToJson(organization));
                return View(organization);
            }
            catch(Exception e)
            {
                Logger.Error($"[{nameof(Edit)}] 服务器错误：\r\n" + e);
                ModelState.AddModelError("All", "编辑界面跳转失败");
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// 编辑 -MVC
        /// </summary>
        /// <param name="id"></param>
        /// <param name="organization"></param>
        /// <returns></returns>
        // POST: Organization/Edit/5
        [HttpPost("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, /*[Bind("Id,ParentId,Name,Description")]*/ OrganizationJson organization)
        {
            Logger.Trace($"[{nameof(Edit)}] 请求参数: id: {id}\r\n{JsonUtil.ToJson(organization)}");
            // 0. 参数检查
            if (id != organization.Id)
            {
                return NotFound();
            }
            //if (organization.Name == null)
            //{
            //    ModelState.AddModelError("Name", "组织名不能为空");
            //    return View(organization);
            //}
            try
            {
                // 1. 权限检查
                if (!await RoleOrgPerManager.HasPermission(SignUser.Id, Constants.ORG_UPDATE, id))
                {
                    Logger.Warn($"[{nameof(Details)}] 权限不足 用户[{SignUser.SignName}]({SignUser.Id})没有权限({Constants.ORG_UPDATE})操作组织({id})");
                    ModelState.AddModelError("All", "权限不足");
                    return RedirectToAction(nameof(Index));
                }
                // 2. 业务处理
                await OrganizationManager.UpdateByUserId(SignUser.Id, organization);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Edit)}] 服务器错误:\r\n{e}");
                ModelState.AddModelError("All", "保存失败");
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// [MVC] 组织管理-组织删除界面跳转
        /// </summary>
        /// <param name="id">组织ID</param>
        /// <returns></returns>
        // GET: Organization/Delete/5
        [HttpGet("{id}")]
        [TypeFilter(typeof(CheckPermission))]
        public async Task<IActionResult> Delete(string id)
        {
            // 0. 参数检查
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                // 1. 权限检查
                if (!await RoleOrgPerManager.HasPermission(SignUser.Id, Constants.ORG_DELETE, id))
                {
                    Logger.Warn($"[{nameof(Details)}] 权限不足 用户[{SignUser.SignName}]({SignUser.Id})没有权限({Constants.ORG_DELETE})操作组织({id})");
                    ModelState.AddModelError("All", "权限不足");
                    return RedirectToAction(nameof(Index));
                }
                // 2. 业务处理
                var organization = await OrganizationManager.FindById(id).SingleOrDefaultAsync();
                Logger.Trace($"[{nameof(Edit)}] 响应数据：\r\n" + JsonUtil.ToJson(organization));
                if (organization == null)
                {
                    return NotFound();
                }
                return View(organization); // /Orgznization/Create
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Edit)}] 服务器错误:\r\n{e}");
                ModelState.AddModelError("All", "界面跳转失败");
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// [API] 组织管理-通过ID删除 -代码编写中
        /// </summary>
        /// <param name="id">组织ID</param>
        /// <returns></returns>
        //[HttpDelete]
        [HttpDelete("{id}")]
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
        /// [MVC] 组织管理-组织删除确认
        /// </summary>
        /// <param name="id">组织ID</param>
        /// <returns></returns>
        // POST: Organization/Delete/5
        [HttpPost("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {

            // 0. 参数检查
            if(id == null)
            {
                return NotFound();
            }
            try
            {
                // 1. 权限检查
                if (!await RoleOrgPerManager.HasPermission(SignUser.Id, Constants.ORG_DELETE, id))
                {
                    Logger.Warn($"[{nameof(Details)}] 权限不足 用户[{SignUser.SignName}]({SignUser.Id})没有权限({Constants.ORG_DELETE})操作组织({id})");
                    ModelState.AddModelError("All", "权限不足");
                    return RedirectToAction(nameof(Index));
                }
                // 2. 业务处理
                await OrganizationManager.DeleteByUserId(SignUser.Id, id);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception e)
            {
                Logger.Error($"[{nameof(Edit)}] 服务器错误:\r\n{e}");
                ModelState.AddModelError("All", "保存失败");
                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        /// <summary>
        /// 获取登陆用户简要信息 -每次都是新建一个UserBaseJson对象
        /// </summary>
        /// <returns></returns>
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
