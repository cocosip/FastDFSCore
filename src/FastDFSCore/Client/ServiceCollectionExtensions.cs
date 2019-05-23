using Microsoft.Extensions.DependencyInjection;
using System;

namespace FastDFSCore.Client
{
    public static class ServiceCollectionExtensions
    {

        /// <summary>添加FastDFS
        /// </summary>
        public static IServiceCollection AddFastDFSCore(this IServiceCollection services, Action<FDFSOption> configure = null)
        {
            var option = new FDFSOption();
            configure?.Invoke(option);

            services
                .AddSingleton<FDFSOption>(option)
                .AddSingleton<IConnectionManager, ConnectionManager>()
                .AddTransient<IExecuter, DefaultExecuter>()
                .AddTransient<IFDFSClient, FDFSClient>();

            return services;
        }
    }
}
