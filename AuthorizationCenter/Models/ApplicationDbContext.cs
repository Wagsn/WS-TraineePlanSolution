using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Models
{
    /// <summary>
    /// 书据库上下文
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// 应用数据库上下文
        /// </summary>
        /// <param name="options"></param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){ }

        #region << DbSet 数据集 >>

        /// <summary>
        /// 用户数据集
        /// </summary>
        public DbSet<UserBase> Users { get; set; }

        /// <summary>
        /// 角色数据集
        /// </summary>
        public DbSet<Role> Roles { get; set; }

        /// <summary>
        /// 全信啊数据集
        /// </summary>
        public DbSet<Permission> Permissions { get; set; }

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
            });

            builder.Entity<Role>(b =>
            {
                b.ToTable("ws_role");
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
            // 采用配置文件的方式
            builder.UseMySql("server=localhost;database=ws_internship;user=admin;password=123456;");
        }
    }
}
