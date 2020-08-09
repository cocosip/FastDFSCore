using FastDFSCore.Utility;
using System;

namespace FastDFSCore.Protocols
{
    /// <summary>查询文件信息返回
    /// </summary>
    public class QueryFileInfoResp : FastDFSResp
    {
        /// <summary>文件大小
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>文件Crc32信息
        /// </summary>
        public long Crc32 { get; set; }

        /// <summary>Ctor
        /// </summary>
        public QueryFileInfoResp()
        {

        }

        /// <summary>LoadContent
        /// </summary>
        public override void LoadContent(ClusterConfiguration configuration, byte[] data)
        {

            byte[] fileSizeBuffer = new byte[Consts.FDFS_PROTO_PKG_LEN_SIZE];
            byte[] createTimeBuffer = new byte[Consts.FDFS_PROTO_PKG_LEN_SIZE];
            byte[] crcBuffer = new byte[Consts.FDFS_PROTO_PKG_LEN_SIZE];

            Array.Copy(data, 0, fileSizeBuffer, 0, fileSizeBuffer.Length);
            Array.Copy(data, Consts.FDFS_PROTO_PKG_LEN_SIZE, createTimeBuffer, 0, createTimeBuffer.Length);
            Array.Copy(data, Consts.FDFS_PROTO_PKG_LEN_SIZE + Consts.FDFS_PROTO_PKG_LEN_SIZE, crcBuffer, 0, crcBuffer.Length);

            FileSize = ByteUtil.BufferToLong(data, 0);
            CreateTime = new DateTime(1970, 1, 1).AddSeconds(ByteUtil.BufferToLong(data, Consts.FDFS_PROTO_PKG_LEN_SIZE));

            Crc32 = ByteUtil.BufferToLong(data, Consts.FDFS_PROTO_PKG_LEN_SIZE + Consts.FDFS_PROTO_PKG_LEN_SIZE);
        }
    }
}
