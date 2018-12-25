using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationCenter.Define;
using AuthorizationCenter.Entitys;
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
            // 读取配置文件
            var configuration = ConfigManager.GetConfig(args);

            // 配置文件设置端口号，检查是否符合端口规范，默认端口 5000
            string port = configuration["Port"] ?? "5000";
            var host = ConfigManager.HostInit(args, port);

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
    }

    /// <summary>
    /// 配置文件初始化
    /// </summary>
    public class ConfigManager
    {
        /// <summary>
        /// 获取配置文件
        /// </summary>
        /// <returns></returns>
        public static IConfigurationRoot GetConfig(string[] args)
        {
            // 检查配置文件??
            return new ConfigurationBuilder()
                .AddJsonFile(Constants.CONFIGPATH)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static IWebHost HostInit(string[] args, string port)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls($"http://*:{port}")
                .Build();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {

        }
    }

    ///// <summary>
    ///// 
    ///// </summary>
    //public class HostInitConfig
    //{
    //    /// <summary>
    //    /// 端口
    //    /// </summary>
    //    public string Port { get; set; }
    //}
}
