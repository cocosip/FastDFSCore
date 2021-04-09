using System.Collections.Generic;

namespace FastDFSCore
{
    /// <summary>Each cluster configuration
    /// </summary>
    public class ClusterConfiguration
    {
        /// <summary>
        /// 集群名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Tracker地址
        /// </summary>
        public List<Tracker> Trackers { get; set; }

        /// <summary>
        /// 连接超时时间,5s
        /// </summary>
        public int ConnectionTimeout { get; set; } = 5;

        /// <summary>
        /// 编码
        /// </summary>
        public string Charset { get; set; } = "utf-8";

        /// <summary>
        /// 是否生成Token
        /// </summary>
        public bool AntiStealToken { get; set; } = true;

        /// <summary>
        /// Token生成的密钥
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// 连接的有效时间,3600s
        /// </summary>
        public int ConnectionLifeTime { get; set; } = 600;

        /// <summary>
        /// 并发线程数
        /// </summary>
        public int ConnectionConcurrentThread { get; set; } = 3;

        /// <summary>
        /// 查询超时的连接的时间间隔
        /// </summary>
        public int ScanTimeoutConnectionInterval { get; set; } = 10;

        /// <summary>
        /// Tracker最大连接数
        /// </summary>
        public int TrackerMaxConnection { get; set; } = 10;

        /// <summary>
        /// Storage最大连接数
        /// </summary>
        public int StorageMaxConnection { get; set; } = 50;
    }
}
