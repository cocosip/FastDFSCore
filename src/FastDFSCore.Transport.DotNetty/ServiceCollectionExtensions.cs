using FastDFSCore.Transport;
using FastDFSCore.Transport.DotNetty;
using Microsoft.Extensions.DependencyInjection;

namespace FastDFSCore
{
    /// <summary>依赖注入扩展
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>添加FastDFS DotNetty传输
        /// </summary>
        public static IServiceCollection AddFastDFSDotNettyTransport(this IServiceCollection services)
        {
            services
                .AddTransient<IConnection, DotNettyConnection>()
                //.AddTransient<FDFSDecoder>()
                //.AddTransient<FDFSReadHandler>()
                //.AddTransient<FDFSDecoder>()

                ;
            return services;
        }
    }
}
