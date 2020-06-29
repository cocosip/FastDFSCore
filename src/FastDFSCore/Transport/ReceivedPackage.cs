namespace FastDFSCore.Transport
{
    public class ReceivedPackage
    {
        /// <summary>消息体长度(除头部数据外)
        /// </summary>
        public long Length { get; set; }

        /// <summary>命令
        /// </summary>
        public byte Command { get; set; }

        /// <summary>状态
        /// </summary>
        public byte Status { get; set; }

        /// <summary>返回是否为流
        /// </summary>
        public bool IsOutputStream { get; set; }

        public string OutputFilePath { get; set; }

        public byte[] Body { get; set; }

        public bool IsComplete { get; set; }

    }
}
