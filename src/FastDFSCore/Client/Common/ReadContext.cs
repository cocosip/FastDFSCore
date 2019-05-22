namespace FastDFSCore.Client
{
    public class ReadContext
    {

        public bool IsComplete { get { return Length == Position; } }

        /// <summary>是否使用流接收
        /// </summary>
        public bool StreamReceive { get; set; }

        /// <summary>是否是流读取中的Chunk块
        /// </summary>
        public bool IsStreamChunk { get; set; }

        /// <summary>Body读取的位置
        /// </summary>
        public long Position { get; set; }

        /// <summary>Body长度
        /// </summary>
        public long Length { get; set; }

        /// <summary>Command命令
        /// </summary>
        public byte Command { get; set; }

        /// <summary>状态
        /// </summary>
        public byte Status { get; set; }

        /// <summary>Body数据
        /// </summary>
        public byte[] Body { get; set; }

        /// <summary>增加读取位置
        /// </summary>
        public void AttachPosition(int length)
        {
            Position = Position + length;
        }
    }
}
