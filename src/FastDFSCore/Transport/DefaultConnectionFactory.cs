using Microsoft.Extensions.DependencyInjection;
using System;

namespace FastDFSCore.Transport
{
    /// <summary>创建连接
    /// </summary>
    public class DefaultConnectionFactory : IConnectionFactory
    {
        private readonly IServiceProvider _provider;

        /// <summary>Ctor
        /// </summary>
        public DefaultConnectionFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        /// <summary>创建连接
        /// </summary>
        public IConnection CreateConnection(ConnectionAddress connectionAddress, Action<IConnection> closeAction)
        {
            using (var scope = _provider.CreateScope())
            {
                var injectConnectionAddress = scope.ServiceProvider.GetService<ConnectionAddress>();

                injectConnectionAddress.ServerEndPoint = connectionAddress.ServerEndPoint;
                injectConnectionAddress.LocalEndPoint = connectionAddress.LocalEndPoint;

                //连接
                var connection = scope.ServiceProvider.GetService<IConnection>();
                //设置关闭时的操作
                connection.OnConnectionClose = closeAction;


                return connection;
            }
        }
    }
}
