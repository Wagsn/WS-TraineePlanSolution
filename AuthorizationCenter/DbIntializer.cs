using AuthorizationCenter.Models;
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
            
            if (context.UserBases.Any())
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
        }
    }
}
