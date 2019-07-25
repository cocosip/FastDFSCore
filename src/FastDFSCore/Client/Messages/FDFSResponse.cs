namespace FastDFSCore.Client
{
    /// <summary>FastDFSCore 通讯返回
    /// </summary>
    public abstract class FDFSResponse
    {
        /// <summary>头部
        /// </summary>
        public FDFSHeader Header { get; set; }

        /// <summary>设置头部信息
        /// </summary>
        public void SetHeader(long length, byte command, byte status)
        {
            Header = new FDFSHeader(length, command, status);
        }

        /// <summary>设置头部信息
        /// </summary>
        public void SetHeader(FDFSHeader header)
        {
            Header = new FDFSHeader(header.Length, header.Command, header.Status);
        }

        /// <summary>LoadContent
        /// </summary>
        public virtual void LoadContent(FDFSOption option, byte[] data)
        {

        }


    }
}
