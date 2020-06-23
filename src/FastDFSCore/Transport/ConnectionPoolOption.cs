namespace FastDFSCore.Transport
{
    /// <summary>连接池配置信息
    /// </summary>
    public class ConnectionPoolOption
    {
        /// <summary>连接地址
        /// </summary>
        public ConnectionAddress ConnectionAddress { get; set; }

        /// <summary>最大连接数
        /// </summary>
        public int MaxConnection { get; set; }

        /// <summary>连接的生命周期(以S为单位)
        /// </summary>
        public int ConnectionLifeTime { get; set; }

        /// <summary>并发线程数
        /// </summary>
        public int ConnectionConcurrentThread { get; set; }

        /// <summary>查询超时的连接的时间间隔
        /// </summary>
        public int ScanTimeoutConnectionInterval { get; set; }
    }
}
