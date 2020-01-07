using System.Net;

namespace FastDFSCore.Client
{
    /// <summary>EndPoint扩展
    /// </summary>
    public static class EndPointExtensions
    {
        /// <summary>Parse IPAddress to ipv4 string, exp: 127.0.0.1
        /// </summary>
        public static string ToIPv4Address(this IPAddress iPAddress)
        {
            return iPAddress.MapToIPv4().ToString();
        }

        /// <summary>Parse IPEndPoint to string address. exp: 127.0.0.1:10001
        /// </summary>
        public static string ToStringAddress(this IPEndPoint iPEndPoint)
        {
            return string.Format("{0}:{1}", iPEndPoint.Address.MapToIPv4().ToString(), iPEndPoint.Port);
        }

        /// <summary>Parse endPoint to string address. exp: 127.0.0.1:10001
        /// </summary>
        public static string ToStringAddress(this EndPoint endPoint)
        {
            return ((IPEndPoint)endPoint).ToStringAddress();
        }


    }
}
