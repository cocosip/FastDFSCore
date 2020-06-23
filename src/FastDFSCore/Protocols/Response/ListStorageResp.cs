using System;
using System.Collections.Generic;

namespace FastDFSCore.Protocols
{
    /// <summary>列出Storage返回
    /// </summary>
    public class ListStorageResp : FastDFSResp
    {
        /// <summary>Storage信息列表
        /// </summary>
        public List<StorageInfo> StorageInfos { get; set; } = new List<StorageInfo>();

        /// <summary>LoadContent
        /// </summary>
        public override void LoadContent(FastDFSOption option, byte[] data)
        {
            if (data.Length % Consts.FDFS_STORAGE_INFO_SIZE != 0)
            {
                throw new ArgumentException("返回数据长度不正确,不是'FDFS_STORAGE_INFO_SIZE' 的整数倍.");
            }
            var count = data.Length / Consts.FDFS_STORAGE_INFO_SIZE;
            for (int i = 0; i < count; i++)
            {
                var buffer = new byte[Consts.FDFS_STORAGE_INFO_SIZE];
                Array.Copy(data, i * Consts.FDFS_STORAGE_INFO_SIZE, buffer, 0, buffer.Length);
                var storageInfo = EndecodeUtil.DecodeStorageInfo(buffer, option.Charset);
                StorageInfos.Add(storageInfo);
            }
        }

    }
}
