using System;
using System.Net;

namespace FastDFSCore.Client
{
    /// <summary>连接池工厂
    /// </summary>
    public interface IConnectionPoolFactory
    {
        /// <summary>创建连接
        /// </summary>
        Connection CreateConnection(ConnectionAddress connectionAddress, Action<Connection> closeAction);

        /// <summary>创建连接池
        /// </summary>
        Pool CreatePool(IPEndPoint endPoint, int maxConnection, int connectionLifeTime);
    }
}
