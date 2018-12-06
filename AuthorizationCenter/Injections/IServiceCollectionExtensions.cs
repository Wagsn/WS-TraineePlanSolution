using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 服务扩展，关键在于 this 参数，必须保证静态类和静态方法
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        public static UserDefinedBuilder AddUserDefined(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            // 依赖注入

            #region << Store >>



            #endregion

            #region << Manager >>



            #endregion

            return new UserDefinedBuilder(services);
        }
    }
}
