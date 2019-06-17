using System;
using System.Net;

namespace FastDFSCore.Client
{
    public class ConnectionPoolFactory : IConnectionPoolFactory
    {
        private readonly IServiceProvider _provider;
        public ConnectionPoolFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        /// <summary>创建连接
        /// </summary>
        public Connection CreateConnection(ConnectionSetting setting, Action<Connection> closeAction)
        {
            return _provider.CreateInstance<Connection>(setting, closeAction);
        }

        /// <summary>创建连接池
        /// </summary>
        public Pool CreatePool(IPEndPoint endPoint, int maxConnection, int connectionLifeTime)
        {
            return _provider.CreateInstance<Pool>(endPoint, maxConnection, connectionLifeTime);
        }

    }
}
