namespace FastDFSCore.Client
{
    public class ConnectionContext
    {

        /// <summary>头部
        /// </summary>
        public FDFSHeader Header { get; set; }

        /// <summary>返回消息
        /// </summary>
        public FDFSResponse Response { get; set; }

        /// <summary>是否流请求
        /// </summary>
        public bool StreamRequest { get; set; } = false;

        /// <summary>是否流返回
        /// </summary>
        public bool StreamResponse { get; set; } = false;

        /// <summary>下载文件
        /// </summary>
        public string StreamSavePath { get; set; }

        /// <summary>是否正在分块写入
        /// </summary>
        public bool IsChunkWriting { get; set; }

        /// <summary>数据已经读取的位置
        /// </summary>
        public long Position { get; set; }

    }
}
