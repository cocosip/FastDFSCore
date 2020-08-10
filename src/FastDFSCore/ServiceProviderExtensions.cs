using Microsoft.Extensions.DependencyInjection;
using System;

namespace FastDFSCore
{
    /// <summary>ServiceProvider扩展
    /// </summary>
    public static class ServiceProviderExtensions
    {
        /// <summary>根据类型使用IServiceProvider创建对象
        /// </summary>
        public static T CreateInstance<T>(this IServiceProvider provider, params object[] args)
        {
            return (T)ActivatorUtilities.CreateInstance(provider, typeof(T), args);
        }
    }
}
