using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationCenter.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AuthorizationCenter
{
    /// <summary>
    /// 程序
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 入口
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    DbIntializer.Initialize(context);

                }
                catch (Exception e)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(e, "An error occurred while seeding the database.");
                }
            }

            host.Run();
        }

        /// <summary>
        /// 站点生成器
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IWebHost BuildWebHost(string[] args) =>
               WebHost.CreateDefaultBuilder(args)
                   .UseStartup<Startup>()
                   .Build();

    }
}
