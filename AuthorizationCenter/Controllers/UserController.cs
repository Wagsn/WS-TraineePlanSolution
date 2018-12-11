using AuthorizationCenter.Dto.Jsons;
using AuthorizationCenter.Managers;
using AuthorizationCenter.Models;
using AuthorizationCenter.Stores;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WS.Core.Dto;

namespace AuthorizationCenter.Controllers
{
    /// <summary>
    /// 用户控制
    /// </summary>
    public class UserController : Controller
    {
        /// <summary>
        /// 上下文
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// 用户管理
        /// </summary>
        public IUserManager<IUserBaseStore, UserBaseJson> UserManager { get; set; }
        
        /// <summary>
        /// 类型映射
        /// </summary>
        public IMapper Mapper { get; set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userManager"></param>
        /// <param name="mapper"></param>
        public UserController(ApplicationDbContext context, IUserManager<IUserBaseStore, UserBaseJson> userManager, IMapper mapper)
        {
            _context = context;
            UserManager = userManager;
            Mapper = mapper;
        }


        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        // GET: UserBaseJsons
        public async Task<IActionResult> Index()
        {
            PagingResponseMessage<UserBaseJson> response = new PagingResponseMessage<UserBaseJson>();

            await UserManager.List(response, new Dto.Requests.ModelRequest<UserBaseJson>());



            return View(response);  //Mapper.Map<UserBaseJson>(await _context.UserBases.ToListAsync())
        }

        // GET: UserBaseJsons/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userBaseJson = Mapper.Map<UserBaseJson> (await _context.UserBases.FirstOrDefaultAsync(m => m.Id == id));
            if (userBaseJson == null)
            {
                return NotFound();
            }

            return View(userBaseJson);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        // GET: UserBaseJsons/Create
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userBaseJson"></param>
        /// <returns></returns>
        // POST: UserBaseJsons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SignName,PassWord")] UserBaseJson userBaseJson)
        {
            if (ModelState.IsValid)
            {
                if(string.IsNullOrWhiteSpace(userBaseJson.SignName)|| string.IsNullOrWhiteSpace(userBaseJson.PassWord))
                {
                    ModelState.AddModelError("All", "用户名或密码不能为空");
                    return RedirectToAction(nameof(Index));
                }
                // 验证是否存在
                //var dbUserBase = UserManager.
                // 添加到数据库
                var userbase = Mapper.Map<UserBase>(userBaseJson);
                userbase.Id = Guid.NewGuid().ToString();
                _context.Add(userbase);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userBaseJson);
        }

        // GET: UserBaseJsons/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userBaseJson = await _context.UserBaseJson.FindAsync(id);
            if (userBaseJson == null)
            {
                return NotFound();
            }
            return View(userBaseJson);
        }

        // POST: UserBaseJsons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,SignName,PassWord")] UserBaseJson userBaseJson)
        {
            if (id != userBaseJson.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userBaseJson);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserBaseJsonExists(userBaseJson.Id))
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
            return View(userBaseJson);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: UserBaseJsons/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userBaseJson = await _context.UserBaseJson
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userBaseJson == null)
            {
                return NotFound();
            }

            return View(userBaseJson);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // POST: UserBaseJsons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var userBaseJson = await _context.UserBaseJson.FindAsync(id);
            _context.UserBaseJson.Remove(userBaseJson);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserBaseJsonExists(string id)
        {
            return _context.UserBaseJson.Any(e => e.Id == id);
        }
    }
}
