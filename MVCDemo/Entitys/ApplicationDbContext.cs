using Microsoft.EntityFrameworkCore;
using MVCDemo.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCDemo.Entitys
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        /// <summary>
        /// 待办项
        /// </summary>
        public DbSet<TodoItem> TodoItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TodoItem>(b =>
                {
                    b.ToTable("ws_todo_todoitem");
                    //b.Property(p => p._IsDeleted);
                });
        }





    }
}
