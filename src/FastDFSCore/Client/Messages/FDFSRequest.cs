namespace FastDFSCore.Client
{
    public abstract class FDFSRequest<T> where T : FDFSResponse
    {
        /// <summary>头部
        /// </summary>
        public FDFSHeader Header { get; set; }

        /// <summary>是否Stream传输
        /// </summary>
        public virtual bool StreamTransfer { get; set; } = false;

        /// <summary>对当前body进行编码
        /// </summary>
        public abstract byte[] EncodeBody(FDFSOption option);

        /// <summary>设置头部
        /// </summary>
        public abstract void SetHeader(long length, byte command, byte status);

    }
}
