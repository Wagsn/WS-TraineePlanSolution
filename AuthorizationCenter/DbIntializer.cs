using AuthorizationCenter.Define;
using AuthorizationCenter.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public static ILogger Logger = LoggerManager.GetLogger<DbIntializer>();

        /// <summary>
        /// 数据库初始化
        /// </summary>
        /// <param name="context"></param>
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();
            // 数据是否存在
            if (context.Users.Any() || context.Organizations.Any() || context.Roles.Any() || context.Permissions.Any() || context.UserRoles.Any() || context.RoleOrgPers.Any())
            {
                return;
            }

            #region << 数据导入，TODO：数据转移到数据文件 >>

            using (var trans = context.Database.BeginTransaction())
            {
                try
                {
                    // 初始用户
                    string root_UserId = Guid.NewGuid().ToString();
                    string xkj_UserId = Guid.NewGuid().ToString();
                    context.AddRange(new List<User>
                    {
                        new User
                        {
                            Id = root_UserId,
                            SignName = "Wagsn",
                            PassWord = "123456"
                        },
                        new User
                        {
                            Id = xkj_UserId,
                            SignName = "xkjadmin",
                            PassWord = "123456"
                        }
                    });

                    // 初始角色
                    string root_RoleId = Guid.NewGuid().ToString();
                    string xkj_RoleId = Guid.NewGuid().ToString();
                    context.AddRange(new List<Role>
                    {
                        new Role
                        {
                            Id = root_RoleId,
                            Name = "SysRoleRoot",
                            Decription = "系统最高权限者"
                        },
                        new Role
                        {
                            Id = xkj_RoleId,
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
                    string root_OrgId = Guid.NewGuid().ToString();
                    string xyh_OrgId = Guid.NewGuid().ToString();
                    string xkj_OrgId = Guid.NewGuid().ToString();
                    string xkj_km_OrgId = Guid.NewGuid().ToString();
                    context.AddRange(new List<Organization>
                    {
                        new Organization
                        {
                            Id = root_OrgId,
                            Name = "SysOrgRoot",
                            Description = "系统默认根组织",
                            ParentId = null
                        },
                        new Organization
                        {
                            Id = xyh_OrgId,
                            ParentId =root_OrgId,
                            Name = "新耀行",
                            Description = "房产中介"
                        },
                        new Organization
                        {
                            Id = xkj_OrgId,
                            ParentId = root_OrgId,
                            Name = "新空间（重庆）科技有限公司",
                            Description = "致力于商业地产服务"
                        },
                        new Organization
                        {
                            Id = xkj_km_OrgId,
                            ParentId = xkj_OrgId,
                            Name = "新空间昆明分公司",
                            Description = "新空间昆明分公司"
                        }
                    });

                    // 组织扩展
                    context.AddRange(new List<OrganizationRelation>
                    {
                        new OrganizationRelation
                        {
                            Id = Guid.NewGuid().ToString(),
                            ParentId = root_OrgId,
                            SonId = xkj_OrgId,
                            IsDirect = true
                        },
                        new OrganizationRelation
                        {
                            Id = Guid.NewGuid().ToString(),
                            ParentId = root_OrgId,
                            SonId = xyh_OrgId,
                            IsDirect = true
                        },
                        new OrganizationRelation
                        {
                            Id = Guid.NewGuid().ToString(),
                            ParentId = root_OrgId,
                            SonId = xkj_km_OrgId,
                            IsDirect = false
                        },
                        new OrganizationRelation
                        {
                            Id = Guid.NewGuid().ToString(),
                            ParentId = xkj_OrgId,
                            SonId = xkj_km_OrgId,
                            IsDirect =true
                        }
                    });

                    // 初始权限
                    string root_PerId = Guid.NewGuid().ToString();
                    string role_manage_PerId = Guid.NewGuid().ToString();
                    string role_save_PerId = Guid.NewGuid().ToString();
                    string role_create_PerId = Guid.NewGuid().ToString();
                    string user_manage_PerId = Guid.NewGuid().ToString();
                    string user_save_PerId = Guid.NewGuid().ToString();
                    string org_manage_PerId = Guid.NewGuid().ToString();
                    string org_save_PerId = Guid.NewGuid().ToString();
                    string per_manage_PerId = Guid.NewGuid().ToString();
                    context.AddRange(new List<Permission>
                    {
                        new Permission
                        {
                            Id = root_PerId,
                            Name = Constants.ROOT,
                            Description = "最高权限",
                            ParentId = null
                        },
                        new Permission
                        {
                            Id = user_manage_PerId,
                            Name = Constants.USER_MANAGE,
                            Description = "用户管理",
                            ParentId = root_PerId
                        },
                        new Permission
                        {
                            Id = user_save_PerId,
                            Name = Constants.USER_SAVE,
                            Description = "用户保存",
                            ParentId = user_manage_PerId
                        },
                        new Permission
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = Constants.USER_QUERY,
                            Description = "用户查询",
                            ParentId = user_manage_PerId
                        },
                        new Permission
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = Constants.USER_DETAILS,
                            Description = "用户详情", // 用户的详细信息（不包括，用户ID，用户名等基础信息）
                            ParentId = user_manage_PerId
                        },
                        new Permission
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = Constants.USER_UPDATE,
                            Description = "用户更新",
                            ParentId = user_save_PerId
                        },
                        new Permission
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = Constants.USER_CREATE,
                            Description = "用户创建",
                            ParentId = user_save_PerId
                        },
                        new Permission
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = Constants.USER_DELETE,
                            Description = "用户删除",
                            ParentId = user_manage_PerId
                        }, // Manage == Delete|Update|Create|Query > Delete > Save == Update|Create > Update > Create > Query
                        new Permission
                        {
                            Id = role_manage_PerId,
                            Name = Constants.ROLE_MANAGE,
                            Description = "角色管理",
                            ParentId = root_PerId
                        },
                        new Permission
                        {
                            Id = role_save_PerId,
                            Name = Constants.ROLE_SAVE,
                            Description = "角色保存",
                            ParentId = role_manage_PerId
                        },
                        new Permission
                        {
                            Id = role_create_PerId,
                            Name = Constants.ROLE_CREATE,
                            Description = "角色添加",
                            ParentId = role_save_PerId
                        },
                        new Permission
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = Constants.ROLE_CREATE_VIEW,
                            Description = "角色角色添加界面",
                            ParentId = role_create_PerId
                        },
                        new Permission
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = Constants.ROLE_QUERY,
                            Description = "角色查询",
                            ParentId = role_manage_PerId
                        },
                        new Permission
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = Constants.USERROLE_MANAGE,
                            Description = "角色绑定",
                            ParentId = root_PerId
                        },
                        new Permission
                        {
                            Id = org_manage_PerId,  // 功能模块
                            Name = Constants.ORG_MANAGE,
                            Description = "组织管理",
                            ParentId = root_PerId
                        },
                        new Permission
                        {
                            Id = org_save_PerId,
                            Name = Constants.ORG_SAVE,
                            Description = "组织保存",
                            ParentId = org_manage_PerId
                        },
                        new Permission
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = Constants.ORG_CREATE,
                            Description ="组织创建",
                            ParentId = org_save_PerId
                        },
                        new Permission
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = Constants.ORG_UPDATE,
                            Description ="组织更新",
                            ParentId = org_save_PerId
                        },
                        new Permission
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = Constants.ORG_QUERY,
                            Description = "组织查询",
                            ParentId = org_manage_PerId
                        },
                        new Permission
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = Constants.ORG_DELETE,
                            Description = "组织删除",
                            ParentId = org_manage_PerId
                        },
                        new Permission
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = Constants.AUTH_MANAGE,
                            Description = "授权管理",
                            ParentId = root_PerId
                        },
                        new Permission
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = Constants.PER_QUERY,
                            Description = "权限查询",
                            ParentId = per_manage_PerId
                        },
                        new Permission
                        {
                            Id = per_manage_PerId,
                            Name = Constants.PER_MANAGE,
                            Description = "权限管理",
                            ParentId = root_PerId
                        }
                    });

                    // 角色绑定
                    context.AddRange(new List<UserRole>
                    {
                        new UserRole
                        {
                            Id = Guid.NewGuid().ToString(),
                            RoleId = root_RoleId,
                            UserId = root_UserId
                        },
                        new UserRole
                        {
                            Id = Guid.NewGuid().ToString(),
                            RoleId = xkj_RoleId,
                            UserId = xkj_UserId
                        }
                    });

                    // 用户组织 一对一关系（暂时）
                    context.AddRange(new List<UserOrg>
                    {
                        new UserOrg
                        {
                            Id = Guid.NewGuid().ToString(),
                            UserId = root_UserId,
                            OrgId = root_OrgId
                        },
                        new UserOrg
                        {
                            Id = Guid.NewGuid().ToString(),
                            UserId = xkj_UserId,
                            OrgId = xkj_OrgId
                        }
                    });

                            // 角色组织
                    context.AddRange(new List<RoleOrg>
                    {
                        new RoleOrg
                        {
                            Id = Guid.NewGuid().ToString(),
                            RoleId = root_RoleId,
                            OrgId = root_OrgId
                        },
                        new RoleOrg
                        {
                            Id = Guid.NewGuid().ToString(),
                            RoleId = xkj_RoleId,
                            OrgId = xkj_OrgId
                        }
                    });

                    // 权限授予 
                    // XKJAdmin只有角色管理权限管理组织管理角色绑定授权管理以及权限项查询权限，不包含权限项增删改
                    context.AddRange(new List<RoleOrgPer>
                    {
                        new RoleOrgPer
                        {
                            RoleId = root_RoleId, // 角色
                            OrgId = root_OrgId,  // 数据范围
                            PerId = root_PerId  // 权限范围
                        },
                        new RoleOrgPer
                        {
                            RoleId = xkj_RoleId,
                            OrgId = xkj_OrgId,
                            PerId = user_manage_PerId
                        },
                        new RoleOrgPer
                        {
                            RoleId = xkj_RoleId,
                            OrgId = xkj_OrgId,
                            PerId = org_manage_PerId
                        },
                        new RoleOrgPer
                        {
                            RoleId = xkj_RoleId,
                            OrgId = xkj_OrgId,
                            PerId = role_manage_PerId
                        }
                    });

                    // 权限扩展
                    context.AddRange(new List<UserPermissionExpansion>
                    {
                        new UserPermissionExpansion
                        {
                            Id = Guid.NewGuid().ToString(),
                            UserId = root_UserId,
                            OrganizationId = root_OrgId,
                            PermissionId = root_PerId
                        },
                        new UserPermissionExpansion
                        {
                            Id = Guid.NewGuid().ToString(),
                            UserId = xkj_UserId,
                            OrganizationId = xkj_OrgId,
                            PermissionId = user_manage_PerId
                        },
                        new UserPermissionExpansion
                        {
                            Id = Guid.NewGuid().ToString(),
                            UserId = xkj_UserId,
                            OrganizationId = xkj_OrgId,
                            PermissionId = org_manage_PerId
                        },
                        new UserPermissionExpansion
                        {
                            Id = Guid.NewGuid().ToString(),
                            UserId = xkj_UserId,
                            OrganizationId = xkj_OrgId,
                            PermissionId = role_manage_PerId
                        }
                    });
                    context.SaveChanges();
                    trans.Commit();
                }
                catch (Exception e)
                {
                    Logger.Error("数据库初始化时失败：\r\n" + e);
                    trans.Rollback();
                }
            }
            #endregion
        }
    }
}
