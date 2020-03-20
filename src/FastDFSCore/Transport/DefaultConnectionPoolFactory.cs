using Microsoft.Extensions.DependencyInjection;
using System;

namespace FastDFSCore.Transport
{
    /// <summary>连接池工厂
    /// </summary>
    public class DefaultConnectionPoolFactory : IConnectionPoolFactory
    {
        private readonly IServiceProvider _provider;

        /// <summary>Ctor
        /// </summary>
        public DefaultConnectionPoolFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        /// <summary>创建连接池
        /// </summary>
        public Pool CreatePool(PoolOption option)
        {
            //return _provider.CreateInstance<Pool>(option);

            using (var scope = _provider.CreateScope())
            {
                var injectOption = scope.ServiceProvider.GetService<PoolOption>();
                injectOption.EndPoint = option.EndPoint;
                injectOption.MaxConnection = option.MaxConnection;
                injectOption.ConnectionLifeTime = option.ConnectionLifeTime;
                injectOption.ScanTimeoutConnectionInterval = option.ScanTimeoutConnectionInterval;
                return scope.ServiceProvider.GetService<Pool>();
            }
        }

    }
}
