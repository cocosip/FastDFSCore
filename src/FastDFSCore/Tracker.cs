using System;

namespace FastDFSCore
{
    /// <summary>Tracker信息
    /// </summary>
    public class Tracker : IEquatable<Tracker>
    {
        /// <summary>IP地址
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>端口号
        /// </summary>
        public int Port { get; set; }

        /// <summary>Ctor
        /// </summary>
        public Tracker()
        {

        }

        /// <summary>Ctor
        /// </summary>
        public Tracker(string iPAddress, int port)
        {
            IPAddress = iPAddress;
            Port = port;
        }

        /// <summary>Equals
        /// </summary>
        public bool Equals(Tracker other)
        {
            return IPAddress == other.IPAddress && Port == other.Port;
        }
    }
}
