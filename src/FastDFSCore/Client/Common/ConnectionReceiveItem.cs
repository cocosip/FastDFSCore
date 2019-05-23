using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastDFSCore.Client
{
    /// <summary>接收数据
    /// </summary>
    public class ConnectionReceiveItem
    {
        public const int HeaderLength = Consts.FDFS_PROTO_PKG_LEN_SIZE + 2;

        public bool IsCompleted { get; set; }

        /// <summary>是否流返回
        /// </summary>
        public bool StreamResponse { get; set; }

        /// <summary>是否进入写Chunk了
        /// </summary>
        public bool IsChunkWriting { get; set; }

        /// <summary>位置
        /// </summary>
        public long Position { get; set; }

        /// <summary>头部
        /// </summary>
        public FDFSHeader Header { get; set; }

        /// <summary>返回的数据
        /// </summary>
        public byte[] Body { get; set; }

        public void ReadHeader(long length, IByteBuffer byteBuffer)
        {
            var command = byteBuffer.ReadByte();
            var status = byteBuffer.ReadByte();
            Header = new FDFSHeader(length, command, status);
        }

        public void ReadChunkBody(IByteBuffer byteBuffer)
        {
            var chunkSize = Math.Min(Header.Length - Position, byteBuffer.ReadableBytes);
            Body = new byte[chunkSize];
            byteBuffer.ReadBytes(Body, 0, Body.Length);
            Position = Position + chunkSize;
        }

        public void ReadBody(IByteBuffer byteBuffer)
        {
            Body = new byte[Header.Length];
            byteBuffer.ReadBytes(Body, 0, Body.Length);
        }
    }
}
