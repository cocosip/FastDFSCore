using FastDFSCore.Transport;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FastDFSCore
{
    /// <summary>ServiceCollection扩展
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>添加FastDFSCore
        /// </summary>
        public static IServiceCollection AddFastDFSCore(this IServiceCollection services, Action<FastDFSOption> configure = null)
        {
            if (configure == null)
            {
                configure = o => { };
            }
            services
                .Configure<FastDFSOption>(configure)
                .AddSingleton<IConnectionManager, DefaultConnectionManager>()
                .AddSingleton<IConnectionPoolBuilder, DefaultConnectionPoolBuilder>()
                .AddSingleton<IConnectionBuilder, DefaultConnectionBuilder>()
                .AddSingleton<IConnectionBuilder, DefaultConnectionBuilder>()
                .AddSingleton<IFastDFSClient, FastDFSClient>()
                .AddTransient<IClusterSelector, DefaultClusterSelector>()
                .AddTransient<IExecuter, DefaultExecuter>()
                .AddScoped<IConnectionPool, DefaultConnectionPool>()
                .AddScoped<ConnectionPoolOption>()
                .AddScoped<ConnectionAddress>()
                ;
            return services;
        }
    }
}
