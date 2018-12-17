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

            if (!context.UserBases.Any())
            {
                context.Add(new UserBase
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

            

        }
    }
}
