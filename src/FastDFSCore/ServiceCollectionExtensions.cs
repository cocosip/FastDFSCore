using FastDFSCore.Scheduling;
using FastDFSCore.Transport;
using FastDFSCore.Transport.Download;
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
                .AddSingleton<IFastDFSCoreHost, FastDFSCoreHost>()
                .AddSingleton<IScheduleService, ScheduleService>()
                .AddSingleton<IConnectionManager, ConnectionManager>()
                .AddSingleton<IConnectionPoolFactory, DefaultConnectionPoolFactory>()
                .AddSingleton<IDownloaderFactory, DefaultDownloaderFactory>()
                .AddTransient<IExecuter, DefaultExecuter>()
                .AddTransient<IFastDFSClient, FastDFSClient>()
                .AddSingleton<IConnectionFactory, DefaultConnectionFactory>()
                .AddScoped<PoolOption>()
                .AddScoped<ConnectionAddress>()
                .AddTransient<Pool>();
            return services;
        }
    }
}
