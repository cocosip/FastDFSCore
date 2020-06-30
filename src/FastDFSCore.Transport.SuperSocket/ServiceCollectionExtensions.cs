using FastDFSCore.Transport;
using Microsoft.Extensions.DependencyInjection;

namespace FastDFSCore
{
    /// <summary>依赖注入扩展
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>添加FastDFS DotNetty传输
        /// </summary>
        public static IServiceCollection AddFastDFSSuperSocket(this IServiceCollection services)
        {
            services
                .AddScoped<IConnection, SuperSocketConnection>();
            return services;
        }

    }
}
