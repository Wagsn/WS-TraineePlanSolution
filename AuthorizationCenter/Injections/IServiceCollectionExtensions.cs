using AuthorizationCenter.Dto.Jsons;
using AuthorizationCenter.Managers;
using AuthorizationCenter.Entitys;
using AuthorizationCenter.Stores;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 服务扩展，关键在于 this 参数，必须保证静态类和静态方法
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// 添加用户定义的依赖注入
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static UserDefinedBuilder AddUserDefined(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            // 依赖注入

            services.AddTransient<ApplicationDbContext>();
            services.AddScoped<ITransaction, Transaction<ApplicationDbContext>>();

            #region << Store >>

            services.AddScoped<IUserStore, UserStore>();
            services.AddScoped<IUserRoleStore, UserRoleStore>();
            services.AddScoped<IRoleStore, RoleStore>();
            services.AddScoped<IPermissionStore, PermissionStore>();
            services.AddScoped<IOrganizationStore, OrganizationStore>();
            services.AddScoped<IRoleOrgPerStore, RoleOrgPerStore>();
            services.AddScoped<IUserOrgStore, UserOrgStore>();
            services.AddScoped<IRoleOrgStore, RoleOrgStore>();
            services.AddScoped<IUserPermissionExpansionStore, UserPermissionExpansionStore>();

            #endregion

            #region << Manager >>

            services.AddScoped<IUserManager<UserJson>, UserManager>();
            services.AddScoped<IRoleManager<RoleJson>, RoleManger>();
            services.AddScoped<IUserRoleManager, UserRoleManager>();
            services.AddScoped<IPermissionManager<PermissionJson>, PermissionManager>();
            services.AddScoped<IOrganizationManager, OrganizationManager>();
            services.AddScoped<IRoleOrgPerManager, RoleOrgPerManager>();

            #endregion

            return new UserDefinedBuilder(services);
        }
    }
}
