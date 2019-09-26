using System;
using System.Net;

namespace FastDFSCore.Client
{
    /// <summary>连接池工厂
    /// </summary>
    public class ConnectionPoolFactory : IConnectionPoolFactory
    {
        private readonly IServiceProvider _provider;

        /// <summary>Ctor
        /// </summary>
        public ConnectionPoolFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        /// <summary>创建连接
        /// </summary>
        public Connection CreateConnection(ConnectionAddress connectionAddress, Action<Connection> closeAction)
        {
            return _provider.CreateInstance<Connection>(connectionAddress, closeAction);
        }

        /// <summary>创建连接池
        /// </summary>
        public Pool CreatePool(IPEndPoint endPoint, int maxConnection, int connectionLifeTime, int scanTimeoutConnectionInterval)
        {
            return _provider.CreateInstance<Pool>(endPoint, maxConnection, connectionLifeTime, scanTimeoutConnectionInterval);
        }

    }
}
