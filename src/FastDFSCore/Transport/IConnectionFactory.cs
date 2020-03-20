using System;

namespace FastDFSCore.Transport
{
    /// <summary>连接工厂
    /// </summary>
    public interface IConnectionFactory
    {
        /// <summary>创建连接
        /// </summary>
        IConnection CreateConnection(ConnectionAddress connectionAddress, Action<IConnection> closeAction);
    }
}
