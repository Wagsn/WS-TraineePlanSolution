using AuthorizationCenter.Define;
using AuthorizationCenter.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WS.Log;

namespace AuthorizationCenter
{
    /// <summary>
    /// 数据库初始化器
    /// </summary>
    public class DbIntializer
    {
        /// <summary>
        /// 日志器
        /// </summary>
        public static ILogger Logger = LoggerManager.GetLogger(nameof(DbIntializer));

        /// <summary>
        /// 数据库初始化
        /// </summary>
        /// <param name="context"></param>
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();
            // 数据是否存在
            if (context.Users.Any()|| context.Organizations.Any()|| context.Roles.Any()|| context.Permissions.Any()|| context.UserRoles.Any()|| context.RoleOrgPers.Any())
            {
                return;
            }

            #region << 数据导入，TODO：数据转移到数据文件 >>

            // 初始用户
            string rootUserId = Guid.NewGuid().ToString();
            string xkjUserRootId = Guid.NewGuid().ToString();
            context.AddRange(new List<User>
            {
                new User
                {
                    Id = rootUserId,
                    SignName = "Wagsn",
                    PassWord = "123456"
                },
                new User
                {
                    Id = Guid.NewGuid().ToString(),
                    SignName = "wangsen",
                    PassWord = "123456"
                },
                new User
                {
                    Id = xkjUserRootId,
                    SignName = "xkjadmin",
                    PassWord = "123456"
                }
            });

            // 初始角色
            string roleRootId = Guid.NewGuid().ToString();
            string xkjRoleRootId = Guid.NewGuid().ToString();
            context.AddRange(new List<Role>
            {
                new Role
                {
                    Id = roleRootId,
                    Name = "SysRoleRoot",
                    Decription = "系统最高权限者"
                },
                new Role
                {
                    Id = xkjRoleRootId,
                    Name = "XKJRoot",
                    Decription = "新空间最高权限者"
                },
                new Role
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "UserManager",
                    Decription = "用户管理员"
                },
                new Role
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "RoleManager",
                    Decription = "角色管理员"
                },
                new Role
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "RoleBindManager",
                    Decription = "角色绑定管理员"
                },
                new Role
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "OrgManager",
                    Decription = "角色管理员"
                },
                new Role
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "AuthManager",
                    Decription = "授权管理员"
                }
            });

            // 初始组织
            string orgRootId = Guid.NewGuid().ToString();
            string xkjOrgRootId = Guid.NewGuid().ToString(); 
            context.AddRange(new List<Organization>
            {
                new Organization
                {
                    Id = orgRootId,
                    Name = "OrgRoot",
                    Description = "根组织",
                    ParentId = null
                },
                new Organization
                {
                    Id = Guid.NewGuid().ToString(),
                    ParentId =orgRootId,
                    Name = "新耀行",
                    Description = "房产中介"
                },
                new Organization
                {
                    Id = xkjOrgRootId,
                    ParentId = orgRootId,
                    Name = "新空间（重庆）科技有限公司",
                    Description = "致力于商业地产服务"
                },
                new Organization
                {
                    Id = Guid.NewGuid().ToString(),
                    ParentId = xkjOrgRootId,
                    Name = "新空间昆明分公司",
                    Description = "新空间昆明分公司"
                }
            });

            // 初始权限
            string perRootId = Guid.NewGuid().ToString();
            string roleManageId = Guid.NewGuid().ToString();
            string roleSaveId = Guid.NewGuid().ToString();
            string roleCreateId = Guid.NewGuid().ToString();
            string userManageId = Guid.NewGuid().ToString();
            string userSaveId = Guid.NewGuid().ToString();
            string orgManageId = Guid.NewGuid().ToString();
            string orgSaveId = Guid.NewGuid().ToString();
            string perManageId = Guid.NewGuid().ToString();
            context.AddRange(new List<Permission>
            {
                new Permission
                {
                    Id = perRootId,
                    Name = Constants.ROOT,
                    Description = "最高权限",
                    ParentId = null
                },
                new Permission
                {
                    Id = userManageId,
                    Name = Constants.USER_MANAGE,
                    Description = "用户管理",
                    ParentId = perRootId
                },
                new Permission
                {
                    Id = userSaveId,
                    Name = Constants.USER_SAVE,
                    Description = "用户保存",
                    ParentId = userManageId
                },
                new Permission
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = Constants.USER_QUERY,
                    Description = "用户查询",
                    ParentId = userManageId
                },
                new Permission
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "USER_DETAILS",
                    Description = "用户详情", // 用户的详细信息（不包括，用户ID，用户名等基础信息）
                    ParentId = userManageId
                },
                new Permission
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = Constants.USER_UPDATE,
                    Description = "用户更新",
                    ParentId = userSaveId
                },
                new Permission
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = Constants.USER_CREATE,
                    Description = "用户创建",
                    ParentId = userSaveId
                },
                new Permission
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = Constants.USER_DELETE,
                    Description = "用户删除",
                    ParentId = userManageId
                }, // Manage == Delete|Update|Create|Query > Delete > Save == Update|Create > Update > Create > Query
                new Permission
                {
                    Id = roleManageId,
                    Name = Constants.ROLE_MANAGE,
                    Description = "角色管理",
                    ParentId = perRootId
                },
                new Permission
                {
                    Id = roleSaveId,
                    Name = "ROLE_SAVE",
                    Description = "角色保存",
                    ParentId = roleManageId
                },
                new Permission
                {
                    Id = roleCreateId,
                    Name = Constants.ROLE_CREATE,
                    Description = "角色添加",
                    ParentId = roleSaveId
                },
                new Permission
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = Constants.ROLE_CREATE_VIEW,
                    Description = "角色角色添加界面",
                    ParentId = roleCreateId
                },
                new Permission
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = Constants.ROLE_QUERY,
                    Description = "角色查询",
                    ParentId = roleManageId
                },
                new Permission
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = Constants.USERROLE_MANAGE,
                    Description = "角色绑定",
                    ParentId = perRootId
                },
                new Permission
                {
                    Id = orgManageId,  // 功能模块
                    Name = Constants.ORG_MANAGE,
                    Description = "组织管理",
                    ParentId = perRootId
                },
                new Permission
                {
                    Id = orgSaveId,
                    Name = Constants.ORG_SAVE,
                    Description = "组织保存",
                    ParentId = orgManageId
                },
                new Permission
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = Constants.ORG_CREATE,
                    Description ="组织创建",
                    ParentId = orgSaveId
                },
                new Permission
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = Constants.ORG_UPDATE,
                    Description ="组织更新",
                    ParentId = orgSaveId
                },
                new Permission
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = Constants.ORG_QUERY,
                    Description = "组织查询",
                    ParentId = orgManageId
                },
                new Permission
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = Constants.AUTH_MANAGE,
                    Description = "授权管理",
                    ParentId = perRootId
                },
                new Permission
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = Constants.PER_QUERY,
                    Description = "权限查询",
                    ParentId = perManageId
                },
                new Permission
                {
                    Id = perManageId,
                    Name = Constants.PER_MANAGE,
                    Description = "权限管理",
                    ParentId = perRootId
                }
            });

            // 角色绑定
            context.AddRange(new List<UserRole>
            {
                new UserRole
                {
                    Id = Guid.NewGuid().ToString(),
                    RoleId = roleRootId,
                    UserId = rootUserId
                },
                new UserRole
                {
                    Id = Guid.NewGuid().ToString(),
                    RoleId = xkjRoleRootId,
                    UserId = xkjUserRootId
                }
            });

            // 用户组织 一对一关系（暂时）
            context.AddRange(new List<UserOrg>
            {
                new UserOrg
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = rootUserId,
                    OrgId = orgRootId
                },
                new UserOrg
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = xkjUserRootId,
                    OrgId = xkjOrgRootId
                }
            });

            // 角色组织
            context.AddRange(new List<RoleOrg>
            {
                new RoleOrg
                {
                    Id = Guid.NewGuid().ToString(),
                    RoleId = roleRootId,
                    OrgId = orgRootId
                },
                new RoleOrg
                {
                    Id = Guid.NewGuid().ToString(),
                    RoleId = xkjRoleRootId,
                    OrgId = xkjOrgRootId
                }
            });

            // 权限授予 
            // XKJAdmin只有角色管理权限管理组织管理角色绑定授权管理以及权限项查询权限，不包含权限项增删改
            context.AddRange(new List<RoleOrgPer>
            {
                new RoleOrgPer
                {
                    RoleId = roleRootId, // 角色
                    OrgId = orgRootId,  // 数据范围
                    PerId = perRootId  // 权限范围
                },
                new RoleOrgPer
                {
                    RoleId = xkjRoleRootId,
                    OrgId = xkjOrgRootId,
                    PerId = userManageId
                },
                new RoleOrgPer
                {
                    RoleId = xkjRoleRootId,
                    OrgId = xkjOrgRootId,
                    PerId = orgManageId
                },
                new RoleOrgPer
                {
                    RoleId = xkjRoleRootId,
                    OrgId = xkjOrgRootId,
                    PerId = roleManageId
                }
            });

            #endregion

            try
            {
                context.SaveChanges();
            }
            catch (Exception e)
            {
                Logger.Error("数据库初始化时失败：\r\n" + e);
            }
        }
    }
}
