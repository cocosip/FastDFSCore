using Microsoft.Extensions.DependencyInjection;
using System;

namespace FastDFSCore.Transport
{
    public class DefaultConnectionPoolBuilder : IConnectionPoolBuilder
    {
        private readonly IServiceProvider _serviceProvider;

        public DefaultConnectionPoolBuilder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IConnectionPool CreateConnectionPool(ConnectionPoolOption option)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var injectOption = scope.ServiceProvider.GetService<ConnectionPoolOption>();
                injectOption.ConnectionAddress = option.ConnectionAddress;
                injectOption.MaxConnection = option.MaxConnection;
                injectOption.ConnectionConcurrentThread = option.ConnectionConcurrentThread;
                injectOption.ConnectionLifeTime = option.ConnectionLifeTime;
                injectOption.ScanTimeoutConnectionInterval = option.ScanTimeoutConnectionInterval;

                return scope.ServiceProvider.GetService<IConnectionPool>();
            }
        }

    }
}
