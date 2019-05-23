using DotNetty.Buffers;

namespace FastDFSCore.Client
{
    /// <summary>接收数据
    /// </summary>
    public class ConnectionReceiveItem
    {

        /// <summary>是否进入写Chunk了
        /// </summary>
        public bool IsChunkWriting { get; set; }

        /// <summary>头部
        /// </summary>
        public FDFSHeader Header { get; set; }

        /// <summary>返回的数据
        /// </summary>
        public byte[] Body { get; set; }



        public void ReadChunkBody(IByteBuffer buffer, int chunkSize)
        {
            Body = new byte[chunkSize];
            buffer.ReadBytes(Body, 0, chunkSize);
            IsChunkWriting = true;
        }


        public void ReadHeader(IByteBuffer buffer)
        {
            Header = new FDFSHeader(buffer.ReadLong(), buffer.ReadByte(), buffer.ReadByte());
            IsChunkWriting = false;
        }


        public void ReadFromBuffer(IByteBuffer buffer)
        {
            Header = new FDFSHeader(buffer.ReadLong(), buffer.ReadByte(), buffer.ReadByte());
            Body = new byte[Header.Length];
            buffer.ReadBytes(Body, 0, (int)Header.Length);
        }

    }
}
