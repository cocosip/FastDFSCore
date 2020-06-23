using FastDFSCore.Scheduling;
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
                .AddSingleton<IScheduleService, ScheduleService>()
                .AddSingleton<IConnectionPoolBuilder, DefaultConnectionPoolBuilder>()
                .AddSingleton<IConnectionBuilder, DefaultConnectionBuilder>()
                //.AddSingleton<IConnectionManager, ConnectionManager>()
                .AddTransient<IExecuter, DefaultExecuter>()
                .AddTransient<IFastDFSClient, FastDFSClient>()
                .AddSingleton<IConnectionBuilder, DefaultConnectionBuilder>()
                .AddScoped<ConnectionPoolOption>()
                .AddScoped<ConnectionAddress>()
                ;
            return services;
        }
    }
}
