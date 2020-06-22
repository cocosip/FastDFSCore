using FastDFSCore.Protocols;

namespace FastDFSCore.Transport
{
    /// <summary>连接上下文
    /// </summary>
    public class TransportContext
    {
        /// <summary>头部
        /// </summary>
        public FastDFSHeader Header { get; set; }

        /// <summary>返回消息
        /// </summary>
        public FastDFSResp Response { get; set; }

        /// <summary>是否流请求
        /// </summary>
        public bool StreamRequest { get; set; } = false;

        /// <summary>是否流返回
        /// </summary>
        public bool StreamResponse { get; set; } = false;

        /// <summary>是否正在分块写入
        /// </summary>
        public bool IsChunkWriting { get; set; }

        /// <summary>数据已经读取的位置
        /// </summary>
        public long ReadPosition { get; set; }

        /// <summary>数据已写的位置
        /// </summary>
        public long WritePosition { get; set; }

        /// <summary>是否读取完成
        /// </summary>
        public bool IsReadCompleted
        {
            get
            {
                if (Header != null)
                {
                    return Header.Length == ReadPosition && ReadPosition > 0;
                }
                return false;
            }
        }

        /// <summary>是否写入完成
        /// </summary>
        public bool IsWriteCompleted
        {
            get { return Header.Length == WritePosition; }
        }

        /// <summary>获取未读的长度
        /// </summary>
        public long GetUnreadLength()
        {
            return Header.Length - ReadPosition;
        }



    }
}
