using System;

namespace FastDFSCore.Client
{
    /// <summary>通讯的头部
    /// </summary>
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

        /// <summary>Ctor
        /// </summary>
        public FDFSHeader()
        {

        }

        /// <summary>Ctor
        /// </summary>
        public FDFSHeader(long length, byte command, byte status)
        {
            Length = length;
            Command = command;
            Status = status;
        }

        /// <summary>头部转换成二进制数据
        /// </summary>
        public byte[] ToBytes()
        {
            byte[] result = new byte[Consts.FDFS_PROTO_PKG_LEN_SIZE + 2];
            byte[] pkglen = Util.LongToBuffer(Length);
            Array.Copy(pkglen, 0, result, 0, pkglen.Length);
            result[Consts.FDFS_PROTO_PKG_LEN_SIZE] = Command;
            result[Consts.FDFS_PROTO_PKG_LEN_SIZE + 1] = Status;
            return result;
        }
    }
}
