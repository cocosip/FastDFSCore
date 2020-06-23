using Microsoft.Extensions.DependencyInjection;
using System;

namespace FastDFSCore.Transport
{
    public class DefaultConnectionBuilder : IConnectionBuilder
    {
        private readonly IServiceProvider _serviceProvider;

        public DefaultConnectionBuilder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IConnection CreateConnection(ConnectionAddress connectionAddress)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var injectConnectionAddress = scope.ServiceProvider.GetService<ConnectionAddress>();

                injectConnectionAddress.IPAddress = connectionAddress.IPAddress;
                injectConnectionAddress.Port = connectionAddress.Port;

                //连接
                var connection = scope.ServiceProvider.GetService<IConnection>();

                return connection;
            }
        }
    }
}
