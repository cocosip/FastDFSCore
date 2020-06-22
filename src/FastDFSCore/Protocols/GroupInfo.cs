namespace FastDFSCore.Protocols
{
    /// <summary>Group的信息
    /// </summary>
    public class GroupInfo
    {
        /// <summary>组名
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>总容量
        /// </summary>
        public long TotalMB { get; set; }

        /// <summary>剩余容量
        /// </summary>
        public long FreeMB { get; set; }

        /// <summary>Trunk空闲容量
        /// </summary>
        public long TrunkFreeMb { get; set; }

        /// <summary>Storage服务的数量
        /// </summary>
        public int ServerCount { get; set; }

        /// <summary>Storage端口号
        /// </summary>
        public int StoragePort { get; set; }

        /// <summary>StorageServer Http端口号
        /// </summary>
        public int StorageHttpPort { get; set; }

        /// <summary>活动的Server数量
        /// </summary>
        public int ActiveCount { get; set; }

        /// <summary>当前写入的Server的Index
        /// </summary>
        public int CurrentWriteServerIndex { get; set; }

        /// <summary>存储路径数量
        /// </summary>
        public int StorePathCount { get; set; }

        /// <summary>子目录数量
        /// </summary>
        public int SubdirCount { get; set; }

        /// <summary>当前Trunk的FileId
        /// </summary>
        public long CurrentTrunkFileId { get; set; }

        /// <summary>Ctor
        /// </summary>
        public GroupInfo()
        {

        }

    }
}
