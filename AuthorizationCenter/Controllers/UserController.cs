using AuthorizationCenter.Define;
using AuthorizationCenter.Dto.Jsons;
using AuthorizationCenter.Entitys;
using AuthorizationCenter.Managers;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WS.Log;
using WS.Text;

namespace AuthorizationCenter.Controllers
{
    /// <summary>
    /// 用户控制
    /// </summary>
    public class UserController : Controller
    {
        /// <summary>
        /// 用户管理
        /// </summary>
        IUserManager<UserJson> UserManager { get; set; }

        /// <summary>
        /// 角色管理
        /// </summary>
        IRoleManager<RoleJson> RoleManager { get; set; }

        /// <summary>
        /// 组织管理
        /// </summary>
        IOrganizationManager OrganizationManager { get; set; }

        /// <summary>
        /// 用户角色关联管理
        /// </summary>
        IUserRoleManager UserRoleManager { get; set; }

        /// <summary>
        /// 角色组织权限管理
        /// </summary>
        IRoleOrgPerManager RoleOrgPerManager { get; set; }
        
        /// <summary>
        /// 类型映射
        /// </summary>
        IMapper Mapper { get; set; }

        /// <summary>
        /// 日志器
        /// </summary>
        readonly ILogger Logger = LoggerManager.GetLogger<UserController>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="roleManager"></param>
        /// <param name="organizationManager"></param>
        /// <param name="userRoleManager"></param>
        /// <param name="roleOrgPerManager"></param>
        /// <param name="mapper"></param>
        public UserController(IUserManager<UserJson> userManager, IRoleManager<RoleJson> roleManager, IOrganizationManager organizationManager, IUserRoleManager userRoleManager, IRoleOrgPerManager roleOrgPerManager, IMapper mapper)
        {
            UserManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            RoleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            OrganizationManager = organizationManager ?? throw new ArgumentNullException(nameof(organizationManager));
            UserRoleManager = userRoleManager ?? throw new ArgumentNullException(nameof(userRoleManager));
            RoleOrgPerManager = roleOrgPerManager ?? throw new ArgumentNullException(nameof(roleOrgPerManager));
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        /// <summary>
        /// 列表 -跳转到列表界面
        /// </summary>
        /// <returns></returns>
        // GET: UserBaseJsons
        public async Task<IActionResult> Index(string orgId, int pageIndex =0, int pageSize =10)
        {
            Logger.Trace($"[{nameof(Index)}] 用户[{SignUser.SignName}]({SignUser.Id})查询组织({orgId??"可见"})下的用户列表, 请求参数: pageIndex: {pageIndex}, pageSize: {pageSize}");
            ViewData[Constants.SIGNUSER] = SignUser;
            try
            {
                if (orgId == null)
                {
                    // 1. 权限验证 -该用户是否存在该权限
                    if (!await RoleOrgPerManager.HasPermission(SignUser.Id, Constants.USER_QUERY))
                    {
                        Logger.Warn($"[{nameof(Index)}] 用户[{SignUser.SignName}]({SignUser.Id})没有权限({Constants.USER_QUERY})");
                        ModelState.AddModelError("All", "没有权限");
                        return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
                    }
                    // 2. 业务处理
                    var users = await UserManager.FindByUserId(SignUser.Id);
                    // 分页查询用户列表 
                    var pageBody = users.Page(pageIndex, pageSize);
                    var data = pageBody.Data;
                    Logger.Trace($"[{nameof(Index)}] 响应数据:\r\n{JsonUtil.ToJson(data)}");
                    ViewData["PageBody"] = pageBody;
                    return View(data);
                }
                else
                {
                    // 1. 权限验证 -该用户在组织(orgId)下是否存在该权限
                    if (!await RoleOrgPerManager.HasPermission(SignUser.Id, Constants.USER_QUERY, orgId))
                    {
                        Logger.Warn($"[{nameof(Index)}] 用户[{SignUser.SignName}]({SignUser.Id})在组织({orgId})下没有权限({Constants.USER_QUERY})");
                        ModelState.AddModelError("All", "没有权限");
                        return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
                    }
                    // 2. 业务处理
                    var users = await UserManager.FindByUserIdOrgId(SignUser.Id, orgId);
                    // 分页查询用户列表 
                    var pageBody = users.Page(pageIndex, pageSize);
                    var data = pageBody.Data;
                    Logger.Trace($"[{nameof(Index)}] 响应数据:\r\n{JsonUtil.ToJson(data)}");
                    ViewData["PageBody"] = pageBody;
                    return View(data);
                }
            }
            catch(Exception e)
            {
                Logger.Error($"[{nameof(Index)}] 服务器错误:\r\n{e.ToString()}");
                return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
            }
        }

        /// <summary>
        /// 详情 -跳转到详情界面
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        // GET: UserBaseJsons/Details/5
        public async Task<IActionResult> Details(string id)
        {
            Logger.Trace($"[{nameof(Details)}] 查看用户详情 用户ID: {id}");
            // 0. 检查参数
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                // 1. 权限检查
                if (!(await RoleOrgPerManager.HasPermissionForUser(SignUser.Id, Constants.USER_QUERY, id)))
                {
                    Logger.Warn($"用户[{SignUser.SignName}]({SignUser.Id})没有权限({Constants.USER_QUERY})操作用户({id})");
                    ModelState.AddModelError("All", "没有权限");
                    return RedirectToAction(nameof(Index));
                }
                // 2. 业务处理
                var user = await UserManager.FindById(id).SingleOrDefaultAsync();
                Logger.Trace($"[{nameof(Details)}] 响应数据:\r\n{JsonUtil.ToJson(user)}");
                if (user == null)
                {
                    return NotFound();
                }
                else
                {
                    // 再查询用户绑定的角色列表
                    ViewData[Constants.ROLES] = await RoleManager.FindByUserId(id);
                    ViewData[Constants.USERROLES] = await UserRoleManager.FindByUserId(id).ToListAsync();
                    ViewData["Organization"] = (await OrganizationManager.FindFromUOByUserId(SignUser.Id)).SingleOrDefault();
                    return View(user);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Details)}] 服务器错误:\r\n{e}");
                return RedirectToAction(nameof(Index));
            }
            
        }

        /// <summary>
        /// [MVC] 跳转到用户新建界面
        /// 在组织(orgId)下创建用户
        /// </summary>
        /// <param name="orgId">组织ID</param>
        /// <returns></returns>
        // GET: UserBaseJsons/Create
        public async Task<IActionResult> Create(string orgId)
        {
            Logger.Trace($"[{nameof(Create)}] 用户[{SignUser.SignName}]({SignUser.Id})跳转到用户新建界面, 请求参数:{nameof(orgId)}({orgId})");
            try
            {
                // 1. 权限验证
                if (orgId == null)
                {
                    if (!await RoleOrgPerManager.HasPermissionInSelfOrg(SignUser.Id, Constants.USER_CREATE))
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
                else
                {
                    if (!await RoleOrgPerManager.HasPermission(SignUser.Id, Constants.USER_CREATE, orgId))
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
                // 查询有权限添加用户的组织
                var organizations = await RoleOrgPerManager.FindOrgByUserIdPerName(SignUser.Id, Constants.USER_CREATE);
                ViewData["OrgId"] = new SelectList(organizations, nameof(Organization.Id), nameof(Organization.Name), orgId);
                return View();
            }
            catch(Exception e)
            {
                Logger.Error($"[{nameof(Details)}] 用户[{SignUser.SignName}]({SignUser.Id})跳转界面(在组织({orgId})下创建用户)失败, 服务器错误:\r\n{e}");
                ViewData["ErrMsg"] = e.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// MVC 创建 -在数据库中添加数据
        /// </summary>
        /// <param name="orgId">组织ID</param>
        /// <param name="userJson">被创建用户</param>
        /// <returns></returns>
        // POST: UserBaseJsons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string orgId, UserJson userJson)
        {
            Logger.Trace($"[{nameof(Create)}] 用户[{SignUser.SignName}]({SignUser.Id})在组织({orgId??"本组织"})新增用户:\r\n{JsonUtil.ToJson(userJson)}");
            // 0. 检查参数
            if (string.IsNullOrWhiteSpace(userJson.SignName) || string.IsNullOrWhiteSpace(userJson.PassWord))
            {
                ModelState.AddModelError("All", "用户名或密码不能为空");
                // 查询有权限添加用户的组织 -TODO: 测试这个东西有没问题
                return RedirectToAction(nameof(Create), new { orgId });
            }
            try
            {
                // 1. 权限检查 -这里创建用户是在自己公司创建 -指定公司创建需要UserOrg表
                if (!(await RoleOrgPerManager.HasPermissionForUser(SignUser.Id, Constants.USER_CREATE, SignUser.Id)))
                {
                    Logger.Warn($"[{nameof(Create)}] 用户[{SignUser.SignName}]({SignUser.Id})没有权限({Constants.USER_CREATE})操作用户({SignUser.Id})");
                    ModelState.AddModelError("All", "没有权限");
                    return RedirectToAction(nameof(Index));
                }
                if (await UserManager.ExistByName(userJson.SignName))
                {
                    ModelState.AddModelError("All", "创建的用户已经存在");
                    return View(userJson);
                }
                // 2. 处理业务
                var user = await UserManager.CreateToOrgByUserId(SignUser.Id, userJson, orgId);
                if (user != null)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("All", "新增失败");
                    return View(userJson);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Details)}] 服务器错误:\r\n{e}");
                ModelState.AddModelError("All", e.Message);
                return View(userJson);
            }
        }

        /// <summary>
        /// MVC 编辑 -跳转到编辑界面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: UserBaseJsons/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            Logger.Trace($"[{nameof(Edit)}] 用户[{SignUser.SignName}]({SignUser.Id})编辑用户({id}) 跳转到编辑界面");
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                // 1. 权限验证
                if(!await RoleOrgPerManager.HasPermissionInSelfOrg(SignUser.Id, Constants.USER_UPDATE))
                {
                    Logger.Warn($"[{nameof(Edit)}] 没有权限");
                    ModelState.AddModelError("All", "没有权限");
                    return RedirectToAction(nameof(Index));
                }
                // 2. 业务处理
                var user = await UserManager.FindById(id).SingleOrDefaultAsync();
                Logger.Trace($"[{nameof(Edit)}] 响应数据:\r\n{JsonUtil.ToJson(user)}");
                if (user == null)
                {
                    Logger.Trace($"[{nameof(Edit)}] 用户[{SignUser.SignName}]({SignUser.Id})进入编辑界面编辑的用户({id})不存在");
                    return NotFound();
                }
                return View(user);
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Edit)}] 服务器错误:\r\n{e}");
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// [MVC] 编辑用户
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <param name="user">用户</param>
        /// <returns></returns>
        // POST: UserBaseJsons/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,SignName,PassWord")] UserJson user)
        {
            Logger.Trace($"[{nameof(Edit)}] 用户[{SignUser.SignName}]({SignUser.Id})编辑用户({id}) 请求参数:\r\n"+JsonUtil.ToJson(user));
            // 0. 参数检查
            if (id != user.Id)
            {
                return NotFound();
            }
            try
            {
                // 1. 权限验证
                if (!await RoleOrgPerManager.HasPermissionInSelfOrg(SignUser.Id, Constants.USER_UPDATE))
                {
                    Logger.Warn($"[{nameof(Edit)}] 没有权限");
                    ModelState.AddModelError("All", "没有权限");
                    return RedirectToAction(nameof(Index));
                }
                // 2. 业务处理
                await UserManager.Update(user);
                // 编辑成功 -跳转到用户列表
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(Edit)}] 用户信息更新失败: " + e);
                if (!await UserManager.ExistById(user.Id))
                {
                    return NotFound();
                }
                ModelState.AddModelError("All", "用户信息更新失败");
                return View(user);
            }
        }

        /// <summary>
        /// MVC 删除 -跳转到删除界面 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        // GET: UserBaseJsons/Delete/5
        public async Task<IActionResult> Delete(string id, string errMsg)
        {
            Logger.Trace($"[{nameof(Delete)}] 删除用户({id})失败{errMsg}");
            // 0. 检查参数
            if (id == null)
            {
                return NotFound();
            }
            ViewData["ErrMsg"] = errMsg;
            try
            {
                // 1. 业务处理
                var userJson = await UserManager.FindById(id).SingleOrDefaultAsync();
                Logger.Trace($"[{nameof(Delete)}] 响应数据:\r\n{JsonUtil.ToJson(userJson)}");
                if (userJson == null)
                {
                    Logger.Trace($"[{nameof(Delete)}] 删除失败 用户未找到->用户ID: " + id);
                    return NotFound();
                }
                return View(userJson);
            }
            catch(Exception e)
            {
                Logger.Error($"[{nameof(Delete)}] 服务器错误:\r\n{e}");
                ModelState.AddModelError("All", e.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// MVC 删除确认 -从数据库删除 -跳转到列表界面 
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        // POST: UserBaseJsons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            Logger.Trace($"[{nameof(DeleteConfirmed)}] 用户[{SignUser.SignName}]()删除出用户({id})");
            try
            {
                // 1. 权限验证
                if(!await RoleOrgPerManager.HasPermission<User>(SignUser.Id, Constants.USER_DELETE, id))
                {
                    Logger.Warn($"[{nameof(Edit)}] 用户[{SignUser.SignName}]({SignUser.Id})没有权限({Constants.USER_DELETE})");
                    ViewData["ErrMsg"] = "没有权限";
                    return RedirectToAction(nameof(Index));
                }
                // 2. 业务处理
                await UserManager.DeleteByUserId(SignUser.Id, id);
                // 删除成功 -跳转到用户列表
                return RedirectToAction(nameof(Index));
            }
            catch(Exception e)
            {
                Logger.Error($"[{nameof(DeleteConfirmed)}] 用户删除失败：\r\n" + e);
                // 跳转回删除界面 -需要在界面说明发生了错误 --不清楚routeValues式怎么匹配的需要了解一下
                return RedirectToAction(nameof(Delete), new { id, errMsg = e.Message});
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
