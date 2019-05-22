using DotNetty.Buffers;
using System;

namespace FastDFSCore.Client
{
    public class ConnectionContext
    {
        public const int HeaderLength = Consts.FDFS_PROTO_PKG_LEN_SIZE + 2;

        public bool IsComplete { get { return Header.Length == Position; } }

        /// <summary>返回信息
        /// </summary>
        public FDFSResponse Response { get; set; }

        /// <summary>是否是上传文件
        /// </summary>
        public bool IsFileUpload { get; set; } = false;

        /// <summary>是否下载文件
        /// </summary>
        public bool IsFileDownload { get; set; } = false;

        /// <summary>是否下载到路径
        /// </summary>
        public bool IsDownloadToPath { get; set; } = false;

        /// <summary>下载文件地址
        /// </summary>
        public string FileDownloadPath { get; set; }

        /// <summary>是否正在进行Chunk下载
        /// </summary>
        public bool IsChunkDownload { get; set; }

        /// <summary>数据已经读取的位置
        /// </summary>
        public long Position { get; set; }

        /// <summary>头部
        /// </summary>
        public FDFSHeader Header { get; set; }

        /// <summary>Body数据
        /// </summary>
        public byte[] Body { get; set; }

        public void ClearBody()
        {
            Body = null;
        }

        public void ReadHeader(IByteBuffer byteBuffer)
        {
            var length = byteBuffer.ReadLong();
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
