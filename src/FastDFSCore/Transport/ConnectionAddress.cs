using System.Net;

namespace FastDFSCore.Transport
{
    /// <summary>连接信息
    /// </summary>
    public class ConnectionAddress
    {
        /// <summary>服务器端IP地址
        /// </summary>
        public IPEndPoint ServerEndPoint { get; set; }

        /// <summary>客户端IP地址
        /// </summary>
        public IPEndPoint LocalEndPoint { get; set; }

        /// <summary>Ctor
        /// </summary>
        public ConnectionAddress()
        {

        }
    }
}
