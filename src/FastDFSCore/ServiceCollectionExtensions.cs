using FastDFSCore.Scheduling;
using FastDFSCore.Transport;
using FastDFSCore.Transport.Download;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace FastDFSCore
{
    /// <summary>ServiceCollection扩展
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>获取注册的Singleton对象的实例
        /// </summary>
        public static T GetSingletonInstanceOrNull<T>(this IServiceCollection services)
        {
            return (T)services
                .FirstOrDefault(d => d.ServiceType == typeof(T))?
                .ImplementationInstance;
        }

        /// <summary>获取注册的Singleton对象的实例
        /// </summary>
        public static T GetSingletonInstance<T>(this IServiceCollection services)
        {
            var service = services.GetSingletonInstanceOrNull<T>();
            if (service == null)
            {
                throw new InvalidOperationException("Could not find singleton service: " + typeof(T).AssemblyQualifiedName);
            }
            return service;
        }


        /// <summary>添加FastDFS
        /// </summary>
        public static IServiceCollection AddFastDFSCore(this IServiceCollection services, Action<FDFSOption> configure = null)
        {
            var option = new FDFSOption();
            configure?.Invoke(option);
            return services.AddFastDFSCoreInternal(option);
        }

        /// <summary>添加FastDFS
        /// </summary>
        public static IServiceCollection AddFastDFSCore(this IServiceCollection services, string file)
        {
            var option = FDFSOptionHelper.GetFDFSOption(file);
            return services.AddFastDFSCoreInternal(option);
        }

        /// <summary>添加FastDFS的具体实现
        /// </summary>
        internal static IServiceCollection AddFastDFSCoreInternal(this IServiceCollection services, FDFSOption option)
        {
            services
                .AddSingleton<FDFSOption>(option)
                .AddSingleton<IScheduleService, ScheduleService>()
                .AddSingleton<IConnectionManager, ConnectionManager>()
                .AddSingleton<IConnectionPoolFactory, DefaultConnectionPoolFactory>()
                .AddSingleton<IDownloaderFactory, DefaultDownloaderFactory>()
                .AddTransient<IExecuter, DefaultExecuter>()
                .AddTransient<IFDFSClient, FDFSClient>()
                .AddSingleton<IConnectionFactory, DefaultConnectionFactory>()
                .AddScoped<PoolOption>()
                .AddScoped<ConnectionAddress>()
                .AddTransient<Pool>()
                ;
            return services;
        }
    }
}
