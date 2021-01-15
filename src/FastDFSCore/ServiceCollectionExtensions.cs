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
        public static IServiceCollection AddFastDFSCore(this IServiceCollection services, Action<FastDFSOptions> configure = null)
        {
            if (configure == null)
            {
                configure = o => { };
            }
            services
                .Configure<FastDFSOptions>(configure)
                .AddSingleton<IConnectionPoolBuilder, DefaultConnectionPoolBuilder>()
                .AddSingleton<IConnectionBuilder, DefaultConnectionBuilder>()
                .AddSingleton<IConnectionBuilder, DefaultConnectionBuilder>()
                .AddSingleton<IFastDFSClient, FastDFSClient>()
                .AddSingleton<IClusterFactory, DefaultClusterFactory>()
                .AddTransient<IClusterSelector, DefaultClusterSelector>()
                .AddScoped<ICluster, DefaultCluster>()
                .AddScoped<ClusterConfiguration>()
                .AddTransient<IExecuter, DefaultExecuter>()
                .AddScoped<IConnectionPool, DefaultConnectionPool>()
                .AddScoped<ConnectionPoolOption>()
                .AddScoped<ConnectionAddress>()
                ;
            return services;
        }
    }
}
