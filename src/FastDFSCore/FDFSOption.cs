using System.Collections.Generic;
using System.Net;

namespace FastDFSCore
{
    /// <summary>FastDFS配置信息
    /// </summary>
    public class FDFSOption
    {
        /// <summary>Tracker的地址
        /// </summary>
        public List<IPEndPoint> Trackers { get; set; } = new List<IPEndPoint>();

        /// <summary>连接超时时间,5s
        /// </summary>
        public int ConnectionTimeout { get; set; } = 5;

        /// <summary>编码
        /// </summary>
        public string Charset { get; set; } = "utf-8";

        /// <summary>连接的有效时间,3600s
        /// </summary>
        public int ConnectionLifeTime { get; set; } = 600;

        /// <summary>查询超时的连接的时间间隔
        /// </summary>
        public int ScanTimeoutConnectionInterval { get; set; } = 10;

        /// <summary>Tracker最大连接数
        /// </summary>
        public int TrackerMaxConnection { get; set; } = 10;

        /// <summary>Storage最大连接数
        /// </summary>
        public int StorageMaxConnection { get; set; } = 50;

        /// <summary>SocketTcp相关参数设置
        /// </summary>
        public TcpSetting TcpSetting { get; set; }

        /// <summary>Ctor
        /// </summary>
        public FDFSOption()
        {
            TcpSetting = new TcpSetting();
        }

    }

    /// <summary>Tcp的相关配置
    /// </summary>
    public class TcpSetting
    {
        /// <summary>Whether write and flush now
        /// </summary>
        public bool TcpNodelay { get; set; } = true;

        /// <summary>Reuse ip address
        /// </summary>
        public bool SoReuseaddr { get; set; } = false;

        /// <summary>Enable client to reConnect the server
        /// </summary>
        public bool EnableReConnect { get; set; } = false;

        /// <summary>ReConnect delay seconds (s)
        /// </summary>
        public int ReConnectDelaySeconds { get; set; } = 2;

        /// <summary>ReConnect interval
        /// </summary>
        public int ReConnectIntervalMilliSeconds { get; set; } = 1000;

        /// <summary>Try reConnect max count
        /// </summary>
        public int ReConnectMaxCount { get; set; } = 10;
    }
}
