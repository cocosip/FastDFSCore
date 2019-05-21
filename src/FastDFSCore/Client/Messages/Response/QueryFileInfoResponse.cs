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
    }
}
