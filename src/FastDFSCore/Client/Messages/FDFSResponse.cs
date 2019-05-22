namespace FastDFSCore.Client
{
    public abstract class FDFSResponse
    {
        /// <summary>头部
        /// </summary>
        public FDFSHeader Header { get; set; }


        public void SetHeader(long length, byte command, byte status)
        {
            Header = new FDFSHeader(length, command, status);
        }

        public void SetHeader(FDFSHeader header)
        {
            Header = new FDFSHeader(header.Length, header.Command, header.Status);
        }

        public virtual void LoadContent(FDFSOption option, byte[] data)
        {

        }


    }
}
