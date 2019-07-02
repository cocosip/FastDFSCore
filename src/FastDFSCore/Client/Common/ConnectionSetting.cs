using System.Net;

namespace FastDFSCore.Client
{
    public class ConnectionSetting
    {
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

        /// <summary>关闭连接的有效时间(如果在此时间内,没有任何消息发送,就会关闭)
        /// </summary>
        public int ConnectionLifeTime { get; set; } = 3600;

        /// <summary>Write buffer high water 16M
        /// </summary>
        public int WriteBufferHighWaterMark { get; set; } = 1024 * 1024 * 16;

        /// <summary>Write buffer low water 8M
        /// </summary>
        public int WriteBufferLowWaterMark { get; set; } = 1024 * 1024 * 8;

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
