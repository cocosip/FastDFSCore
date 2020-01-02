using Microsoft.Extensions.DependencyInjection;
using System;

namespace FastDFSCore.Client
{
    /// <summary>ServiceCollection扩展
    /// </summary>
    public static class ServiceCollectionExtensions
    {

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
                .AddSingleton<IConnectionPoolFactory, ConnectionPoolFactory>()
                .AddSingleton<IDownloaderFactory, DefaultDownloaderFactory>()
                .AddTransient<IExecuter, DefaultExecuter>()
                .AddTransient<IFDFSClient, FDFSClient>();
            return services;
        }
    }
}
