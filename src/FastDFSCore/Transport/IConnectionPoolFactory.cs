using System;
using System.Net;

namespace FastDFSCore.Transport
{
    /// <summary>连接池工厂
    /// </summary>
    public interface IConnectionPoolFactory
    {
        /// <summary>创建连接池
        /// </summary>
        Pool CreatePool(PoolOption option);
    }
}
