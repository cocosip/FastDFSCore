using System;
using System.Net;

namespace FastDFSCore.Client
{
    public interface IConnectionPoolFactory
    {
        /// <summary>创建连接
        /// </summary>
        Connection CreateConnection(ConnectionSetting setting, Action<Connection> closeAction);

        /// <summary>创建连接池
        /// </summary>
        Pool CreatePool(IPEndPoint endPoint, int maxConnection, int connectionLifeTime);
    }
}
