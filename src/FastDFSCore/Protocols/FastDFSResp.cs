namespace FastDFSCore.Protocols
{
    /// <summary>FastDFSCore 通讯返回
    /// </summary>
    public abstract class FastDFSResp
    {
        public FastDFSHeader Header { get; set; }

        /// <summary>设置头部信息
        /// </summary>
        public void SetHeader(long length, byte command, byte status)
        {
            Header = new FastDFSHeader(length, command, status);
        }

        /// <summary>设置头部信息
        /// </summary>
        public void SetHeader(FastDFSHeader header)
        {
            Header = new FastDFSHeader(header.Length, header.Command, header.Status);
        }

        /// <summary>LoadContent
        /// </summary>
        public virtual void LoadContent(ClusterConfiguration configuration, byte[] data)
        {

        }


    }
}
