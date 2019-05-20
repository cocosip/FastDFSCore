using System.Collections.Generic;
using System.Net;
using System.Text;

namespace FastDFSCore.Client
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
        public Encoding Charset { get; set; } = Encoding.UTF8;

        /// <summary>连接的有效时间,3600s
        /// </summary>
        public int ConnectionLifeTime { get; set; } = 3600;

        /// <summary>Tracker最大连接数
        /// </summary>
        public int TrackerMaxConnection { get; set; } = 10;

        /// <summary>Storage最大连接数
        /// </summary>
        public int StorageMaxConnection { get; set; } = 50;

        public string LoggerName { get; set; } = "FDFSLogger";
    }
}
