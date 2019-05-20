using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace FastDFSCore.Client
{
    public class ClientSetting
    {
        /// <summary>Enable ssl
        /// </summary>
        public bool IsSsl { get; set; }

        /// <summary>X509 cert
        /// </summary>
        public X509Certificate2 TlsCertificate { get; set; }

        /// <summary>Enable libuv
        /// </summary>
        public bool UseLibuv { get; set; }

        /// <summary>Round robin request expired interval (ms)
        /// </summary>
        public int ScanTimeoutRequestInterval { get; set; } = 1000;

        /// <summary>Wait the handler execute time (ms)
        /// </summary>
        public int WaitHandlerExecuteMilliSeconds { get; set; } = 3000;

        /// <summary>Quiet after connection channel close (ms)
        /// </summary>
        public int QuietPeriodMilliSeconds { get; set; } = 100;

        /// <summary>Timeout when close the channel (s)
        /// </summary>
        public int CloseTimeoutSeconds { get; set; } = 1;

        /// <summary>Write buffer high water 2M
        /// </summary>
        public int WriteBufferHighWaterMark { get; set; } = 1024 * 1024 * 2;

        /// <summary>Write buffer low water 1M
        /// </summary>
        public int WriteBufferLowWaterMark { get; set; } = 1024 * 1024;

        /// <summary>Receive
        /// </summary>
        public int SoRcvbuf { get; set; } = 1024 * 1024;

        /// <summary>Send
        /// </summary>
        public int SoSndbuf { get; set; } = 1024 * 1024;

        /// <summary>Whether write and flush now
        /// </summary>
        public bool TcpNodelay { get; set; } = true;

        /// <summary>Reuse ip address
        /// </summary>
        public bool SoReuseaddr { get; set; } = false;

        /// <summary>AutoRead
        /// </summary>
        public bool AutoRead { get; set; } = true;

        /// <summary>Server ip address and port
        /// </summary>
        public IPEndPoint ServerEndPoint { get; set; }

        /// <summary>Local bind ip address and port
        /// </summary>
        public IPEndPoint LocalEndPoint { get; set; }
    }
}
