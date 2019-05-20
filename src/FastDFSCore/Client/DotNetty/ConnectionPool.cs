using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace FastDFSCore.Client
{
    /// <summary>连接池
    /// </summary>
    public class ConnectionPool
    {
        /// <summary>IP地址和端口号
        /// </summary>
        public IPEndPoint EndPoint { get; set; }

        /// <summary>最大连接数
        /// </summary>
        public int MaxConnection { get; set; }

    }
}
