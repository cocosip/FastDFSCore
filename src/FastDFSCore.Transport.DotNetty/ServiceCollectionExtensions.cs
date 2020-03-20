using FastDFSCore.Transport;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastDFSCore
{
    /// <summary>依赖注入扩展
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>添加FastDFS DotNetty传输
        /// </summary>
        public static IServiceCollection AddDotNettyTransport(this IServiceCollection services)
        {
            services.AddTransient<IConnection, DotNettyConnection>();

            return services;
        }
    }
}
