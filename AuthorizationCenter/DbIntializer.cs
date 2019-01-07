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
                }
            });

            // 初始角色
            string rootRoleId = Guid.NewGuid().ToString();
            context.AddRange(new List<Role>
            {
                new Role
                {
                    Id = rootRoleId,
                    Name = "RoleRoot",
                    Decription = "系统最高权限者"
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
            string rootOrgId = Guid.NewGuid().ToString();
            string xkjCqId = Guid.NewGuid().ToString(); 
            context.AddRange(new List<Organization>
            {
                new Organization
                {
                    Id = rootOrgId,
                    Name = "OrgRoot",
                    Description = "根组织",
                    ParentId = null
                },
                new Organization
                {
                    Id = xkjCqId,
                    ParentId = rootOrgId,
                    Name = "新空间（重庆）科技有限公司",
                    Description = "致力于商业地产服务"
                },
                new Organization
                {
                    Id = Guid.NewGuid().ToString(),
                    ParentId = xkjCqId,
                    Name = "新空间昆明分公司",
                    Description = "新空间昆明分公司"
                }
            });

            // 初始权限
            string rootPerId = Guid.NewGuid().ToString();
            string userManageId = Guid.NewGuid().ToString();
            context.AddRange(new List<Permission>
            {
                new Permission
                {
                    Id = rootPerId,
                    Name = Constants.ROOT,
                    Description = "最高权限",
                    ParentId = null
                },
                new Permission
                {
                    Id = userManageId,
                    Name = Constants.USER_MANAGE,
                    Description = "用户管理",
                    ParentId = rootPerId
                },
                new Permission
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "USER_QUERY",
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
                    Name = "USER_UPDATE",
                    Description = "用户更新",
                    ParentId = userManageId
                },
                new Permission
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "USER_CREATE",
                    Description = "用户创建",
                    ParentId = userManageId
                },
                new Permission
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "USER_DELETE",
                    Description = "用户删除",
                    ParentId = userManageId
                }, // Manage == Delete|Update|Create|Query > Delete > Save == Update|Create > Update > Create > Query
                new Permission
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = Constants.ROLE_MANAGE,
                    Description = "角色管理",
                    ParentId = rootPerId
                },
                new Permission
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = Constants.ROLE_BIND_MANAGE,
                    Description = "角色绑定",
                    ParentId = rootPerId
                },
                new Permission
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = Constants.ORG_MANAGE,
                    Description = "组织管理",
                    ParentId = rootPerId
                },
                new Permission
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = Constants.AUTH_MANAGE,
                    Description = "授权管理",
                    ParentId = rootPerId
                }
            });

            // 角色绑定
            context.Add(new UserRole
            {
                Id = Guid.NewGuid().ToString(),
                RoleId = rootRoleId,
                UserId = rootUserId
            });

            // 用户组织 一对一关系（暂时）
            context.AddRange(new List<UserOrg>
            {
                new UserOrg
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = rootUserId,
                    OrgId = rootOrgId
                }
            });

            // 角色组织
            context.AddRange(new List<RoleOrg>
            {
                new RoleOrg
                {
                    Id = Guid.NewGuid().ToString(),
                    RoleId = rootRoleId,
                    OrgId = rootOrgId
                }
            });

            // 权限授予
            context.Add(new RoleOrgPer
            {
                RoleId = rootRoleId,
                OrgId = rootOrgId,
                PerId = rootPerId
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
