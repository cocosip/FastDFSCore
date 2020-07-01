using FastDFSCore.Transport;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FastDFSCore
{
    /// <summary>依赖注入扩展
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>添加FastDFS DotNetty传输
        /// </summary>
        public static IServiceCollection AddFastDFSDotNetty(this IServiceCollection services, Action<DotNettyOption> configure = null)
        {
            if (configure == null)
            {
                configure = c => { };
            }

            services
                .Configure<DotNettyOption>(configure)
                .AddScoped<IConnection, DotNettyConnection>();
            return services;
        }

    }
}
