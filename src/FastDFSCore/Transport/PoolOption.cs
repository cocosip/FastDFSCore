using System.Net;

namespace FastDFSCore.Transport
{
    /// <summary>创建连接池的配置信息
    /// </summary>
    public class PoolOption
    {
        /// <summary>连接地址
        /// </summary>
        public IPEndPoint EndPoint { get; set; }

        /// <summary>最大连接数
        /// </summary>
        public int MaxConnection { get; set; }

        /// <summary>连接的生命周期
        /// </summary>
        public int ConnectionLifeTime { get; set; }

        /// <summary>扫描连接过期的时间间隔
        /// </summary>
        public int ScanTimeoutConnectionInterval { get; set; }

    }
}
