using FastDFSCore.Transport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace FastDFSCore
{
    /// <summary>ServiceProvider扩展
    /// </summary>
    public static class ServiceProviderExtensions
    {
        /// <summary>配置FastDFSCore
        /// </summary>
        public static IServiceProvider ConfigureFastDFSCore(this IServiceProvider provider, Action<FastDFSOption> configure = null)
        {

            var option = provider.GetService<IOptions<FastDFSOption>>().Value;
            configure?.Invoke(option);

            var connectionManager = provider.GetService<IConnectionManager>();
            connectionManager.Initialize();

            return provider;
        }

        /// <summary>根据类型使用IServiceProvider创建对象
        /// </summary>
        public static T CreateInstance<T>(this IServiceProvider provider, params object[] args)
        {
            return (T)ActivatorUtilities.CreateInstance(provider, typeof(T), args);
        }
    }
}
