using System;

namespace FastDFSCore.Client
{
    /// <summary>文件信息
    /// </summary>
    public class FDFSFileInfo
    {
        public long FileSize { get; set; }

        /// <summary>创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>文件Crc32信息
        /// </summary>
        public long Crc32 { get; set; }

        public FDFSFileInfo()
        {

        }

        public FDFSFileInfo(long fileSize, DateTime createTime, long crc32)
        {
            FileSize = fileSize;
            CreateTime = createTime;
            Crc32 = crc32;
        }
    }
}
