using System;

namespace FastDFSCore.Client
{
    public class QueryFileInfoResponse : FDFSResponse
    {
        public long FileSize { get; set; }

        /// <summary>创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>文件Crc32信息
        /// </summary>
        public long Crc32 { get; set; }


        public QueryFileInfoResponse()
        {

        }

        public override void LoadContent(FDFSOption option, byte[] data)
        {

            byte[] fileSizeBuffer = new byte[Consts.FDFS_PROTO_PKG_LEN_SIZE];
            byte[] createTimeBuffer = new byte[Consts.FDFS_PROTO_PKG_LEN_SIZE];
            byte[] crcBuffer = new byte[Consts.FDFS_PROTO_PKG_LEN_SIZE];

            Array.Copy(data, 0, fileSizeBuffer, 0, fileSizeBuffer.Length);
            Array.Copy(data, Consts.FDFS_PROTO_PKG_LEN_SIZE, createTimeBuffer, 0, createTimeBuffer.Length);
            Array.Copy(data, Consts.FDFS_PROTO_PKG_LEN_SIZE + Consts.FDFS_PROTO_PKG_LEN_SIZE, crcBuffer, 0, crcBuffer.Length);

            FileSize = Util.BufferToLong(data, 0);
            CreateTime = new System.DateTime(1970, 1, 1).AddSeconds(Util.BufferToLong(data, Consts.FDFS_PROTO_PKG_LEN_SIZE));

            Crc32 = Util.BufferToLong(data, Consts.FDFS_PROTO_PKG_LEN_SIZE + Consts.FDFS_PROTO_PKG_LEN_SIZE);
        }
    }
}
