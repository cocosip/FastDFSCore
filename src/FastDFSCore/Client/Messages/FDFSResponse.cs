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
    }
}
