using FastDFSCore.Transport;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FastDFSCore
{
    /// <summary>ServiceProvider扩展
    /// </summary>
    public static class ServiceProviderExtensions
    {
        /// <summary>创建对象
        /// </summary>
        public static object CreateInstance(this IServiceProvider provider, Type type, params object[] args)
        {
            return ActivatorUtilities.CreateInstance(provider, type, args);
        }

        /// <summary>创建对象
        /// </summary>
        public static T CreateInstance<T>(this IServiceProvider provider, params object[] args)
        {
            return (T)ActivatorUtilities.CreateInstance(provider, typeof(T), args);
        }


        /// <summary>配置FastDFSCore
        /// </summary>
        public static IServiceProvider ConfigureFastDFSCore(this IServiceProvider provider)
        {
            //连接管理器
            var connectionManager = provider.GetService<IConnectionManager>();
            if (connectionManager == null)
            {
                throw new NullReferenceException("IConnectionManager is null!");
            }
            connectionManager.Start();
            return provider;
        }
    }
}
