using AuthorizationCenter.Define;
using AuthorizationCenter.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter
{
    /// <summary>
    /// 数据库初始化器
    /// </summary>
    public class DbIntializer
    {
        /// <summary>
        /// 数据库初始化
        /// </summary>
        /// <param name="context"></param>
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Users.Any())
            {
                return;
            }
            if (context.Organizations.Any())
            {
                return;
            }
            if (context.Roles.Any())
            {
                return;
            }
            if (context.Permissions.Any())
            {
                return;
            }
            if (context.UserRoles.Any())
            {
                return;
            }
            if (context.RoleOrgPers.Any())
            {
                return;
            }
            // 初始用户
            string rootUserId = Guid.NewGuid().ToString();
            context.Add(new User
            {
                Id = rootUserId,
                SignName = "Wagsn",
                PassWord = "123456"
            });

            // 初始角色
            string rootRoleId = Guid.NewGuid().ToString();
            context.Add(new Role
            {
                Id = rootRoleId,
                Name = "RoleRoot",
                Decription = "系统最高权限者"
            });

            // 初始组织
            string rootOrgId = Guid.NewGuid().ToString();
            context.Add(new Organization
            {
                Id = rootOrgId,
                Name = "OrgRoot",
                Description = "根组织",
                ParentId = null
            });
            context.Add(new Organization
            {
                Id = Guid.NewGuid().ToString(),
                ParentId = rootOrgId,
                Name = "重庆新空间",
                Description = "致力于商业地产服务"
            });

            // 初始权限
            string rootPerId = Guid.NewGuid().ToString();
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
                    Id = Guid.NewGuid().ToString(),
                    Name = Constants.USER_MANAGE,
                    Description = "用户管理",
                    ParentId = rootPerId
                },
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

            // 权限授予
            context.Add(new RoleOrgPer
            {
                RoleId = rootRoleId,
                OrgId = rootOrgId,
                PerId = rootPerId
            });
            
            try
            {
                context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine("数据库初始化时失败：\r\n" + e);
            }
        }
    }
}
