using System.Net;

namespace FastDFSCore.Utility
{
    /// <summary>EndPoint扩展
    /// </summary>
    public static class EndPointExtensions
    {
        /// <summary>Parse IPEndPoint to string address. exp: 127.0.0.1:10001
        /// </summary>
        public static string ToStringAddress(this IPEndPoint iPEndPoint)
        {
            return $"{ iPEndPoint.Address.MapToIPv4()}:{iPEndPoint.Port}";
        }

        /// <summary>Parse endPoint to string address. exp: 127.0.0.1:10001
        /// </summary>
        public static string ToStringAddress(this EndPoint endPoint)
        {
            return ((IPEndPoint)endPoint).ToStringAddress();
        }
    }
}
