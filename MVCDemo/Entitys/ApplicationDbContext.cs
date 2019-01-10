using Microsoft.EntityFrameworkCore;

namespace MVCDemo.Entitys
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        ///// <summary>
        ///// 待办项
        ///// </summary>
        //public DbSet<TodoItem> TodoItems { get; set; }

        /// <summary>
        /// 学生
        /// </summary>
        public DbSet<Student> Students { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<TodoItem>(b =>
            //{
            //    b.ToTable("ws_todo_todoitem");
            //});
            modelBuilder.Entity<Student>(b =>
            {
                b.ToTable("student");
            });
        }
    }
}
