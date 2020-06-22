using System;

namespace FastDFSCore.Protocols
{
    /// <summary>Storage信息
    /// </summary>
    public class StorageInfo
    {
        /// <summary>状态
        /// </summary>
        public byte Status { get; set; }

        /// <summary>存储Id
        /// </summary>
        public string StorageId { get; set; }

        /// <summary>IP地址
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>源头IP地址
        /// </summary>
        public string SrcIPAddress { get; set; }

        /// <summary>域名
        /// </summary>
        public string DomainName { get; set; }

        /// <summary>版本号
        /// </summary>
        public string Version { get; set; }

        /// <summary>加入时间
        /// </summary>
        public DateTime JoinTime { get; set; }

        /// <summary>运行时间
        /// </summary>
        public DateTime UpTime { get; set; }

        /// <summary>总大小
        /// </summary>
        public long TotalMb { get; set; }

        /// <summary>剩余空间大小
        /// </summary>
        public long FreeMb { get; set; }

        /// <summary>上传的优先级
        /// </summary>
        public int UploadPriority { get; set; }

        /// <summary>存储路径数量
        /// </summary>
        public int StorePathCount { get; set; }

        /// <summary>子目录数量
        /// </summary>
        public int SubdirCount { get; set; }

        /// <summary>当前写入路径
        /// </summary>
        public int CurrentWritePath { get; set; }

        /// <summary>Storage端口号
        /// </summary>
        public int StoragePort { get; set; }

        /// <summary>StorageHttp端口号
        /// </summary>
        public int StorageHttpPort { get; set; }

        /// <summary>alloc_count
        /// </summary>
        public int AllocCount { get; set; }

        /// <summary>当前数量
        /// </summary>
        public int CurrentCount { get; set; }

        /// <summary>最大数量
        /// </summary>
        public int MaxCount { get; set; }

        /// <summary>上传总数
        /// </summary>
        public long TotalUploadCount { get; set; }

        /// <summary>上传成功数量
        /// </summary>
        public long SuccessUploadCount { get; set; }

        /// <summary>Append总数
        /// </summary>
        public long TotalAppendCount { get; set; }

        /// <summary>成功Append数量
        /// </summary>
        public long SuccessAppendCount { get; set; }

        /// <summary>修改总数
        /// </summary>
        public long TotalModifyCount { get; set; }

        /// <summary>成功修改总数
        /// </summary>
        public long SuccessModifyCount { get; set; }

        /// <summary>截断总数
        /// </summary>
        public long TotalTruncateCount { get; set; }

        /// <summary>成功截断的总数
        /// </summary>
        public long SuccessTruncateCount { get; set; }

        /// <summary>设置Meta总数
        /// </summary>
        public long TotalSetMetaCount { get; set; }

        /// <summary>成功设置Meta总数
        /// </summary>
        public long SuccessSetMetaCount { get; set; }

        /// <summary>删除的总数
        /// </summary>
        public long TotalDeleteCount { get; set; }

        /// <summary>成功删除的总数
        /// </summary>
        public long SuccessDeleteCount { get; set; }

        /// <summary>下载总数
        /// </summary>
        public long TotalDownloadCount { get; set; }

        /// <summary>成功下载的总数
        /// </summary>
        public long SuccessDownloadCount { get; set; }

        /// <summary>获取Meta总数
        /// </summary>
        public long TotalGetMetaCount { get; set; }

        /// <summary>成功获取Meta的总数
        /// </summary>
        public long SuccessGetMetaCount { get; set; }

        /// <summary>创建连接的总数
        /// </summary>
        public long TotalCreateLinkCount { get; set; }

        /// <summary>成功创建连接的数量
        /// </summary>
        public long SuccessCreateLinkCount { get; set; }

        /// <summary>删除连接的总数
        /// </summary>
        public long TotalDeleteLinkCount { get; set; }

        /// <summary>成功删除连接的数量
        /// </summary>
        public long SuccessDeleteLinkCount { get; set; }

        /// <summary>上传的总字节数
        /// </summary>
        public long TotalUploadBytes { get; set; }

        /// <summary>成功上传的字节数
        /// </summary>
        public long SuccessUploadBytes { get; set; }

        /// <summary>Append总字节数
        /// </summary>
        public long TotalAppendBytes { get; set; }

        /// <summary>成功Append字节数
        /// </summary>
        public long SuccessAppendBytes { get; set; }

        /// <summary>modify总字节数
        /// </summary>
        public long TotalModifyBytes { get; set; }

        /// <summary>成功modify字节数
        /// </summary>
        public long SuccessModifyBytes { get; set; }

        /// <summary>下载总字节数
        /// </summary>
        public long TotalDownloadBytes { get; set; }

        /// <summary>成功下载字节数
        /// </summary>
        public long SuccessDownloadBytes { get; set; }

        /// <summary>从其他服务器同步到本地的总字节数
        /// </summary>
        public long TotalSyncInBytes { get; set; }

        /// <summary>成功从其他服务器同步到本地的总字节数
        /// </summary>
        public long SuccessSyncInBytes { get; set; }

        /// <summary>同步到其他服务器的总字节数
        /// </summary>
        public long TotalSyncOutBytes { get; set; }

        /// <summary>成功同步到其他服务器的总字节数
        /// </summary>
        public long SuccessSyncOutBytes { get; set; }

        /// <summary>打开文件总数
        /// </summary>
        public long TotalFileOpenCount { get; set; }

        /// <summary>成功打开文件的数量
        /// </summary>
        public long SuccessFileOpenCount { get; set; }

        /// <summary>读取文件总数
        /// </summary>
        public long TotalFileReadCount { get; set; }

        /// <summary>成功读取文件的数量
        /// </summary>
        public long SuccessFileReadCount { get; set; }

        /// <summary>写入文件总数
        /// </summary>
        public long TotalFileWriteCount { get; set; }

        /// <summary>成功写入文件的数量
        /// </summary>
        public long SuccessFileWriteCount { get; set; }

        /// <summary>最后更新时间
        /// </summary>
        public DateTime LastSourceUpdate { get; set; }

        /// <summary>最后开始同步时间
        /// </summary>
        public DateTime LastSyncUpdate { get; set; }

        /// <summary>最后已同步的时间戳
        /// </summary>
        public DateTime LastSyncedTimestamp { get; set; }

        /// <summary>最后心跳时间
        /// </summary>
        public DateTime LastHeartbeatTime { get; set; }

        /// <summary>是否为Trunk服务器
        /// </summary>
        public bool IsTrunkServer { get; set; }

        /// <summary>Ctor
        /// </summary>
        public StorageInfo()
        {

        }
    }
}
