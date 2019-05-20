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
using WS.Log;

namespace AuthorizationCenter
{
    /// <summary>
    /// 程序
    /// </summary>
    public class Program
    {
        static readonly WS.Log.ILogger Logger = LoggerManager.GetLogger<Program>();
        
        /// <summary>
        /// 入口
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            try
            {
                // 读取配置文件
                var configuration = ConfigManager.GetConfig(args);
                // 获取主机
                var host = ConfigManager.GetHost(args, configuration);
                // 数据库初始化
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
                Logger.Info("host.Run()");
                host.Run();
            }
            catch(Exception e)
            {
                Logger.Error($"[{nameof(Main)}] 应用程序错误:\r\n{e}");
            }
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
        /// <param name="args">参数</param>
        /// <returns></returns>
        public static IConfigurationRoot GetConfig(string[] args)
        {
            // 检查配置文件??
            return new ConfigurationBuilder()
                .AddJsonFile(Constants.CONFIG_PATH)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();
        }

        /// <summary>
        /// 主机初始化
        /// </summary>
        /// <param name="args">参数</param>
        /// <param name="config">配置</param>
        /// <returns></returns>
        public static IWebHost GetHost(string[] args, IConfigurationRoot config)
        {
            // 配置文件设置端口号，检查是否符合端口规范，默认端口 5000
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls($"http://*:{config["Port"]}")
                .Build();
        }

        ///// <summary>
        ///// 初始化
        ///// </summary>
        //public static void Init()
        //{

        //}
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
