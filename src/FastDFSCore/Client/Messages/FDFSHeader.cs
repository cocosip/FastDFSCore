namespace FastDFSCore.Client
{
    public class FDFSHeader
    {
        /// <summary>消息体长度(除头部数据外)
        /// </summary>
        public long Length { get; set; }

        /// <summary>命令
        /// </summary>
        public byte Command { get; }

        /// <summary>状态
        /// </summary>
        public byte Status { get; }

        public FDFSHeader()
        {

        }

        public FDFSHeader(long length, byte command, byte status)
        {
            Length = length;
            Command = command;
            Status = status;
        }
    }
}
