using System;
using System.Collections.Generic;

namespace FastDFSCore.Client
{
    public class ListStorageResponse : FDFSResponse
    {
        public List<StorageInfo> StorageInfos { get; set; } = new List<StorageInfo>();

        public override void LoadContent(FDFSOption option, byte[] data)
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
                var storageInfo = Util.LoadStorageInfo(option.Charset, buffer);
                StorageInfos.Add(storageInfo);
            }
        }

    }
}
