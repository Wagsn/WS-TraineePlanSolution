using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationCenter.Define;
using AuthorizationCenter.Entitys;
using AuthorizationCenter.Filters;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AuthorizationCenter
{
    /// <summary>
    /// 启动
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// 配置
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// 服务
        /// </summary>
        /// <param name="services"></param>
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(Constants.CONFIG_PATH)
                .AddEnvironmentVariables()
                .Build();

            services.AddMvc(config => config.Filters.Add(typeof(SignFilter))).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSession();

            // 这里应该是依赖注入 对象映射
            services.AddAutoMapper();

            // 自己扩展得的一个方法，用于依赖注入
            services.AddUserDefined();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v1",
                    Title = "授权中心接口文档",
                    //TermsOfService = new Uri("http://localhost:5000/api/swagger")
                });
                c.IgnoreObsoleteActions();
                //Set the comments path for the swagger json and ui.
                //var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(Constants.DOCDIR, "api.xml");
                c.IncludeXmlComments(xmlPath);
                //c.OperationFilter<HttpHeaderOperation>(); // 添加httpHeader参数
            });

            services.AddDbContext<ApplicationDbContext>(it =>
            {
                it.UseMySql(configuration["Data:DefaultConnection:ConnectionString"] ?? "server=localhost;database=ws_traineeplan;user=admin;password=123456;");
                //it.UseMySql("server=192.168.100.132;database=ws_internship;user=admin;password=123456;");
            });
        }

        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //else
            //{
            //    app.UseExceptionHandler("/Shared/Error");
            //}

            
            // 启用Swagger中间件以生成Swagger作为JSON数据（必须在app.UseMvc();之前）
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthorizationCenter API V1");
                c.ShowExtensions();
                //c.ShowRequestHeaders();
            });

            // app.UseMvc之前
            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            

            // 默认访问index.html
            app.UseDefaultFiles();
            // 允许访问wwwroot
            app.UseStaticFiles();
        }
    }
}
