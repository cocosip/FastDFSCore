using System;

namespace FastDFSCore.Protocols
{
    /// <summary>文件信息
    /// </summary>
    public class FastDFSFileInfo
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
        public FastDFSFileInfo()
        {

        }

        /// <summary>Ctor
        /// </summary>
        /// <param name="fileSize">文件大小</param>
        /// <param name="createTime">创建时间</param>
        /// <param name="crc32">文件Crc32信息</param>
        public FastDFSFileInfo(long fileSize, DateTime createTime, long crc32)
        {
            FileSize = fileSize;
            CreateTime = createTime;
            Crc32 = crc32;
        }
    }
}
