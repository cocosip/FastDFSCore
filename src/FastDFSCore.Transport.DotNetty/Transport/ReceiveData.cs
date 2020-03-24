using DotNetty.Buffers;
using FastDFSCore.Codecs.Messages;
using System;

namespace FastDFSCore.Transport
{
    /// <summary>接收数据
    /// </summary>
    public class ReceiveData
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


        /// <summary>读取ChunkBody数据
        /// </summary>
        public void ReadChunkBody(IByteBuffer buffer, int chunkSize)
        {
            Body = new byte[chunkSize];
            buffer.ReadBytes(Body, 0, chunkSize);
            IsChunkWriting = true;
        }

        /// <summary>读取Header数据
        /// </summary>
        public void ReadHeader(IByteBuffer buffer)
        {
            Header = new FDFSHeader(buffer.ReadLong(), buffer.ReadByte(), buffer.ReadByte());
            IsChunkWriting = false;
        }

        /// <summary>从IByteBuffer中加载当前的ConnectionReceiveItem信息
        /// </summary>
        public void ReadFromBuffer(IByteBuffer buffer)
        {
            Header = new FDFSHeader(buffer.ReadLong(), buffer.ReadByte(), buffer.ReadByte());
            if (Header.Status != 0)
            {
                throw new Exception($"返回Status不正确:{Header.Status}");
            }

            Body = new byte[Header.Length];
            buffer.ReadBytes(Body, 0, (int)Header.Length);
        }

    }
}
