using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 用户定义创建
    /// </summary>
    public class UserDefinedBuilder
    {
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="services"></param>
        public UserDefinedBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        /// <summary>
        /// 服务
        /// </summary>
        IServiceCollection Services { get; }
    }
}
