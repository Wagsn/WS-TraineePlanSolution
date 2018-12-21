using AuthorizationCenter.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter
{
    /// <summary>
    /// 
    /// </summary>
    public class DbIntializer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            //if (context.UserBases.Any())
            //{
            //    return;
            //}
            //if (context.Organizations.Any())
            //{
            //    return;
            //}
            //if (context.Roles.Any())
            //{
            //    return;
            //}
            //if (context.Permissions.Any())
            //{
            //    return;
            //}

            if (!context.UserBases.Any(u => u.SignName =="Wagsn"))
            {
                context.Add(new User
                {
                    Id = Guid.NewGuid().ToString(),
                    SignName = "Wagsn",
                    PassWord = "123456"
                });
                try
                {
                    context.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine("数据库初始化时插入核心用户失败：\r\n" + e);
                }
            }
            if (!context.Roles.Any(r => r.Name=="SuperRoot"))
            {
                context.Add(new Role
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "SuperRoot",
                    Decription = "系统最高用户"
                });
                context.SaveChanges();
                
            }
            if(!context.UserRoles.Any(ur => ur.RoleId == context.Roles.Where(r => r.Name == "SuperRoot").Single().Id&&ur.UserId== context.UserBases.Where(u => u.SignName == "Wagsn").Single().Id))
            {
                context.Add(new UserRole
                {
                    Id = Guid.NewGuid().ToString(),
                    RoleId = context.Roles.Where(r => r.Name == "SuperRoot").Single().Id,
                    UserId = context.UserBases.Where(u => u.SignName == "Wagsn").Single().Id
                });
                context.SaveChanges();
            }
            

            

        }
    }
}
