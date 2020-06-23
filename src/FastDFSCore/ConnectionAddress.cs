using System;

namespace FastDFSCore
{
    /// <summary>连接信息
    /// </summary>
    public class ConnectionAddress : IEquatable<ConnectionAddress>
    {
        /// <summary>服务端地址
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>服务端端口号
        /// </summary>
        public int Port { get; set; }


        /// <summary>Ctor
        /// </summary>
        public ConnectionAddress()
        {

        }

        /// <summary>Ctor
        /// </summary>
        public ConnectionAddress(string iPAddress, int port)
        {
            IPAddress = iPAddress;
            Port = port;
        }

        /// <summary>判断对象是否相等
        /// </summary>
        public bool Equals(ConnectionAddress other)
        {
            return IPAddress == other.IPAddress && Port == other.Port;
        }

        /// <summary>重写相等方法
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }
            return obj is ConnectionAddress address && Equals(address);
        }

        /// <summary>重写获取HashCode方法
        /// </summary>
        public override int GetHashCode()
        {
            return StringComparer.InvariantCulture.GetHashCode(IPAddress) & Port.GetHashCode();
        }

        /// <summary>ToString
        /// </summary>
        public override string ToString()
        {
            return $"{IPAddress}@{ Port}";
        }
    }
}
