using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationCenter.Dto.Jsons;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthorizationCenter.Entitys
{
    /// <summary>
    /// 数据库上下文
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// 构造器
        /// </summary>
        public ApplicationDbContext() { }

        /// <summary>
        /// 应用数据库上下文
        /// </summary>
        /// <param name="options"></param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){ }

        #region << DbSet 数据集 >>

        /// <summary>
        /// 用户数据集
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// 角色数据集
        /// </summary>
        public DbSet<Role> Roles { get; set; }

        /// <summary>
        /// 用户角色
        /// </summary>
        public DbSet<UserRole> UserRoles { get; set; }

        /// <summary>
        /// 用户角色
        /// </summary>
        public DbSet<UserOrg> UserOrgs{ get; set; }

        /// <summary>
        /// 权限数据集
        /// </summary>
        public DbSet<Permission> Permissions { get; set; }

        /// <summary>
        /// 组织数据集
        /// </summary>
        public DbSet<Organization> Organizations { get; set; }

        /// <summary>
        /// 组织关系表
        /// </summary>
        public DbSet<OrganizationRelation> OrganizationRelations { get; set; }

        /// <summary>
        /// 角色组织权限
        /// </summary>
        public DbSet<RoleOrgPer> RoleOrgPers { get; set; }

        /// <summary>
        /// 角色组织
        /// </summary>
        public DbSet<RoleOrg> RoleOrgs { get; set; }

        /// <summary>
        /// 用户权限扩展表
        /// </summary>
        public DbSet<UserPermissionExpansion> UserPermissionExpansions { get; set; }

        /// <summary>
        /// 待办项
        /// </summary>
        public DbSet<TodoItem> TodoItems { get; set; }

        #endregion

        /// <summary>
        /// 在模型创建时
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            #region << 模型映射 >>

            builder.Entity<User>(b =>
            {
                b.ToTable("user");
                //b.HasIndex(p => p.SignName).IsUnique();
            });

            builder.Entity<Role>(b =>
            {
                b.ToTable("role");  //.HasIndex(i=>i.Name).IsUnique();
                b.Property(p => p.Name);
                //b.Property(p => new { p.Name, p.Id });
            });

            builder.Entity<UserRole>(b =>
            {
                b.ToTable("user_role_relation"); // .HasKey(prop => new { prop.RoleId, prop.UserId });
                // 多对多关联
                b.HasOne(e => e.User).WithMany(u => u.UserRoles);
                b.HasOne(e => e.Role).WithMany(r => r.UserRoles);
            });

            builder.Entity<UserOrg>(b =>
            {
                b.ToTable("user_org_relation");
            });

            builder.Entity<Permission>(b =>
            {
                b.ToTable("permission");
            });

            builder.Entity<Organization>(b =>
            {
                b.ToTable("organization");
                b.HasOne(e => e.Parent).WithMany(org => org.Children);  // 外键关联
            });

            builder.Entity<OrganizationRelation>(b =>
            {
                b.ToTable("org_relation");
            });

            builder.Entity<RoleOrgPer>(b =>
            {
                b.ToTable("role_org_per_relation");
                //b.Property(p => p.Id).ValueGeneratedOnAddOrUpdate();
            });

            builder.Entity<RoleOrg>(b =>
            {
                b.ToTable("role_org_relation");
            });

            builder.Entity<UserPermissionExpansion>(b =>
            {
                b.ToTable("user_org_per_expansion");
            });
            #endregion
        }

        ///// <summary>
        ///// 数据库配置 -迁移到Startup.cs -生成基架时取消注释
        ///// </summary>
        ///// <param name="builder">数据库上下文选项创建器</param>
        //protected override void OnConfiguring(DbContextOptionsBuilder builder)
        //{
        //    base.OnConfiguring(builder);
        //    // Pomelo.EntityFrameworkCore.MySql 
        //    // TODO: 采用配置文件的方式
        //    builder.UseMySql("server=192.168.100.132;database=ws_internship;user=admin;password=123456;");
        //}
    }
}
