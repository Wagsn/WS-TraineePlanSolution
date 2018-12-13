using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationCenter.Dto.Jsons;

namespace AuthorizationCenter.Models
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
        public DbSet<UserBase> UserBases { get; set; }

        /// <summary>
        /// 角色数据集
        /// </summary>
        public DbSet<Role> Roles { get; set; }

        /// <summary>
        /// 权限数据集
        /// </summary>
        public DbSet<Permission> Permissions { get; set; }

        /// <summary>
        /// 组织数据集
        /// </summary>
        public DbSet<Organization> Organizations { get; set; }

        #endregion

        /// <summary>
        /// 在模型创建时
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            #region << 模型映射 >>

            builder.Entity<UserBase>(b =>
            {
                b.ToTable("ws_userbase");
                b.HasIndex(p => p.SignName).IsUnique();
            });

            builder.Entity<Role>(b =>
            {
                b.ToTable("ws_role");  //.HasIndex(i=>i.Name).IsUnique();
                b.Property(p => p.Name);
                //b.Property(p => new { p.Name, p.Id });
            });

            builder.Entity<Permission>(b =>
            {
                b.ToTable("ws_permission");
            });

            builder.Entity<Organization>(b =>
            {
                b.ToTable("ws_organization");
            });

            #endregion
        }

        /// <summary>
        /// 数据库配置
        /// </summary>
        /// <param name="builder">数据库上下文选项创建器</param>
        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);
            // Pomelo.EntityFrameworkCore.MySql 
            // TODO: 采用配置文件的方式
            builder.UseMySql("server=192.168.100.132;database=ws_internship;user=admin;password=123456;");
        }
    }
}
