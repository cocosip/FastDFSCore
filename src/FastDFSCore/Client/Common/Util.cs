using System;
using System.Text;

namespace FastDFSCore.Client
{
    public static class Util
    {
        /// <summary>去除扩展名开始的.
        /// </summary>
        public static string ParseExtWithOut(string fileExt)
        {
            return fileExt.TrimStart('.');
        }

        /// <summary>生成GroupName数组
        /// </summary>
        public static byte[] CreateGroupNameBuffer(Encoding encoding, string groupName)
        {
            byte[] groupBytes = new byte[Consts.FDFS_GROUP_NAME_MAX_LEN];
            byte[] bytes = encoding.GetBytes(groupName);
            Array.Copy(bytes, groupBytes, Math.Min(groupBytes.Length, bytes.Length));
            return groupBytes;
        }

        public static byte[] CreateFileExtBuffer(Encoding encoding, string fileExt)
        {
            byte[] extBuffer = new byte[Consts.FDFS_FILE_EXT_NAME_MAX_LEN];
            byte[] bse = encoding.GetBytes(fileExt);
            int ext_name_len = bse.Length;
            if (ext_name_len > Consts.FDFS_FILE_EXT_NAME_MAX_LEN)
            {
                ext_name_len = Consts.FDFS_FILE_EXT_NAME_MAX_LEN;
            }
            Array.Copy(bse, 0, extBuffer, 0, ext_name_len);
            return extBuffer;
        }

        public static byte[] CreatePrefixBuffer(Encoding encoding, string prefix)
        {
            byte[] prefixBuffer = new byte[Consts.FDFS_FILE_PREFIX_MAX_LEN];
            byte[] prefixData = encoding.GetBytes(prefix);
            Array.Copy(prefixData, 0, prefixBuffer, 0, prefixData.Length);
            return prefixBuffer;
        }


        public static byte[] StringToByte(Encoding encoding, string input)
        {
            return encoding.GetBytes(input);
        }

        public static string ByteToString(Encoding encoding, byte[] input)
        {
            char[] chars = encoding.GetChars(input);
            string result = new string(chars, 0, chars.Length);
            return result;
        }

        public static string ByteToString(Encoding encoding, byte[] input, int startIndex, int count)
        {
            char[] chars = encoding.GetChars(input, startIndex, count);
            string result = new string(chars, 0, chars.Length);
            return result;
        }


        public static byte[] LongToBuffer(long l)
        {
            byte[] buffer = new byte[8];
            buffer[0] = (byte)((l >> 56) & 0xFF);
            buffer[1] = (byte)((l >> 48) & 0xFF);
            buffer[2] = (byte)((l >> 40) & 0xFF);
            buffer[3] = (byte)((l >> 32) & 0xFF);
            buffer[4] = (byte)((l >> 24) & 0xFF);
            buffer[5] = (byte)((l >> 16) & 0xFF);
            buffer[6] = (byte)((l >> 8) & 0xFF);
            buffer[7] = (byte)(l & 0xFF);
            return buffer;
        }

        public static long BufferToLong(byte[] buffer, int offset = 0)
        {
#pragma warning disable CS0675 
            return (((long)(buffer[offset] >= 0 ? buffer[offset] : 256 + buffer[offset])) << 56) |
                  (((long)(buffer[offset + 1] >= 0 ? buffer[offset + 1] : 256 + buffer[offset + 1])) << 48) |
                  (((long)(buffer[offset + 2] >= 0 ? buffer[offset + 2] : 256 + buffer[offset + 2])) << 40) |
                  (((long)(buffer[offset + 3] >= 0 ? buffer[offset + 3] : 256 + buffer[offset + 3])) << 32) |
                  (((long)(buffer[offset + 4] >= 0 ? buffer[offset + 4] : 256 + buffer[offset + 4])) << 24) |
                  (((long)(buffer[offset + 5] >= 0 ? buffer[offset + 5] : 256 + buffer[offset + 5])) << 16) |
                  (((long)(buffer[offset + 6] >= 0 ? buffer[offset + 6] : 256 + buffer[offset + 6])) << 8) |
                  ((buffer[offset + 7] >= 0 ? buffer[offset + 7] : 256 + buffer[offset + 7]));
#pragma warning restore CS0675 

        }


        public static GroupInfo LoadGroupInfo(Encoding encoding, byte[] buffer)
        {
            var groupInfo = new GroupInfo();
            //GroupName
            var groupNameBuffer = new byte[Consts.FDFS_GROUP_NAME_MAX_LEN + 1];
            Array.Copy(buffer, 0, groupNameBuffer, 0, groupNameBuffer.Length);
            groupInfo.GroupName = ByteToString(encoding, groupNameBuffer).TrimEnd('\0');

            //total disk
            groupInfo.TotalMb = BufferToLong(buffer, Consts.FDFS_GROUP_NAME_MAX_LEN + 1);
            //free disk storage in MB
            groupInfo.FreeMb = BufferToLong(buffer, Consts.FDFS_GROUP_NAME_MAX_LEN + 1 + Consts.FDFS_PROTO_PKG_LEN_SIZE * 1);
            //TrunkFreeMb
            groupInfo.TrunkFreeMb = BufferToLong(buffer, Consts.FDFS_GROUP_NAME_MAX_LEN + 1 + Consts.FDFS_PROTO_PKG_LEN_SIZE * 2);

            //storage server count
            groupInfo.ServerCount = (int)BufferToLong(buffer, Consts.FDFS_GROUP_NAME_MAX_LEN + 1 + Consts.FDFS_PROTO_PKG_LEN_SIZE * 3);
            //storage server port
            groupInfo.StoragePort = (int)BufferToLong(buffer, Consts.FDFS_GROUP_NAME_MAX_LEN + 1 + Consts.FDFS_PROTO_PKG_LEN_SIZE * 4);
            //storage server http port
            groupInfo.StorageHttpPort = (int)BufferToLong(buffer, Consts.FDFS_GROUP_NAME_MAX_LEN + 1 + Consts.FDFS_PROTO_PKG_LEN_SIZE * 5);
            //active server count
            groupInfo.ActiveCount = (int)BufferToLong(buffer, Consts.FDFS_GROUP_NAME_MAX_LEN + 1 + Consts.FDFS_PROTO_PKG_LEN_SIZE * 6);
            //current write server index
            groupInfo.CurrentWriteServerIndex = (int)BufferToLong(buffer, Consts.FDFS_GROUP_NAME_MAX_LEN + 1 + Consts.FDFS_PROTO_PKG_LEN_SIZE * 7);
            //store path count on storage server
            groupInfo.StorePathCount = (int)BufferToLong(buffer, Consts.FDFS_GROUP_NAME_MAX_LEN + 1 + Consts.FDFS_PROTO_PKG_LEN_SIZE * 8);
            // subdir count per path on storage server
            groupInfo.SubdirCount = (int)BufferToLong(buffer, Consts.FDFS_GROUP_NAME_MAX_LEN + 1 + Consts.FDFS_PROTO_PKG_LEN_SIZE * 9);
            //
            groupInfo.CurrentTrunkFileId = (int)BufferToLong(buffer, Consts.FDFS_GROUP_NAME_MAX_LEN + 1 + Consts.FDFS_PROTO_PKG_LEN_SIZE * 10);

            return groupInfo;
        }

        public static StorageInfo LoadStorageInfo(Encoding encoding, byte[] buffer)
        {
            var storageInfo = new StorageInfo();
            //状态
            storageInfo.Status = buffer[0];

            //Id
            var idBuffer = new byte[Consts.FDFS_STORAGE_ID_MAX_SIZE];
            Array.Copy(buffer, 1, idBuffer, 0, idBuffer.Length);
            storageInfo.StorageId = ByteToString(encoding, idBuffer).TrimEnd('\0').TrimStart('\a');

            //ipaddress
            var ipAddressBuffer = new byte[Consts.IP_ADDRESS_SIZE];
            Array.Copy(buffer, 1 + Consts.FDFS_STORAGE_ID_MAX_SIZE, ipAddressBuffer, 0, ipAddressBuffer.Length);
            storageInfo.IPAddress = ByteToString(encoding, ipAddressBuffer).TrimEnd('\0');

            //src ipaddress
            var srcIPAddressBuffer = new byte[Consts.IP_ADDRESS_SIZE];
            Array.Copy(buffer, 1 + Consts.FDFS_STORAGE_ID_MAX_SIZE + Consts.IP_ADDRESS_SIZE, srcIPAddressBuffer, 0, srcIPAddressBuffer.Length);
            storageInfo.SrcIPAddress = ByteToString(encoding, srcIPAddressBuffer).TrimEnd('\0');

            //domain
            var domainBuffer = new byte[Consts.FDFS_DOMAIN_NAME_MAX_SIZE];
            Array.Copy(buffer, 1 + Consts.FDFS_STORAGE_ID_MAX_SIZE + Consts.IP_ADDRESS_SIZE * 2, domainBuffer, 0, domainBuffer.Length);
            storageInfo.DomainName = ByteToString(encoding, domainBuffer).TrimEnd('\0');

            //version
            var versionBuffer = new byte[Consts.FDFS_VERSION_SIZE];
            Array.Copy(buffer, 1 + Consts.FDFS_STORAGE_ID_MAX_SIZE + Consts.IP_ADDRESS_SIZE * 2 + Consts.FDFS_DOMAIN_NAME_MAX_SIZE, versionBuffer, 0, versionBuffer.Length);
            storageInfo.Version = ByteToString(encoding, versionBuffer).TrimEnd('\0');

            //跳过的字节
            var skipCount = 1 + Consts.FDFS_STORAGE_ID_MAX_SIZE + Consts.IP_ADDRESS_SIZE * 2 + Consts.FDFS_DOMAIN_NAME_MAX_SIZE + Consts.FDFS_VERSION_SIZE;
            //182

            //join time
            storageInfo.JoinTime = DateTimeUtil.ToDateTime((int)BufferToLong(buffer, skipCount));
            //up time
            storageInfo.UpTime = DateTimeUtil.ToDateTime((int)BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE));
            //总容量
            storageInfo.TotalMb = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 2);
            //空余容量
            storageInfo.FreeMb = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 3);
            //上传优先级
            storageInfo.UploadPriority = (int)BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 4);
            //存储路径数量
            storageInfo.StorePathCount = (int)BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 5);
            //子目录数
            storageInfo.SubdirCount = (int)BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 6);
            //当前写入路径数量
            storageInfo.CurrentWritePath = (int)BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 7);
            //端口号
            storageInfo.StoragePort = (int)BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 8);
            //Http端口号
            storageInfo.StorageHttpPort = (int)BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 9);
            //AllocCount
            storageInfo.AllocCount = BitConverter.ToInt32(buffer, Consts.FDFS_PROTO_PKG_LEN_SIZE * 10);

            //BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 10);
            //CurrentCount
            storageInfo.CurrentCount = BitConverter.ToInt32(buffer, Consts.FDFS_PROTO_PKG_LEN_SIZE * 10 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 11);
            //MaxCount
            storageInfo.MaxCount = BitConverter.ToInt32(buffer, Consts.FDFS_PROTO_PKG_LEN_SIZE * 10 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE * 2);
            //BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 12);
            //上传总数
            storageInfo.TotalUploadCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 11 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //成功上传数量
            storageInfo.SuccessUploadCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 12 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //Append总数
            storageInfo.TotalAppendCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 13 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //成功Append数量
            storageInfo.SuccessAppendCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 14 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //Modify数量
            storageInfo.TotalModifyCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 15 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //成功Modify数量
            storageInfo.SuccessModifyCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 16 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //Truncate总数
            storageInfo.TotalTruncateCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 17 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //成功Truncate数量
            storageInfo.SuccessTruncateCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 18 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //设置Meta总数
            storageInfo.TotalSetMetaCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 19 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //成功设置Meta数量
            storageInfo.SuccessSetMetaCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 20 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //删除总数
            storageInfo.TotalDeleteCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 21 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //成功删除数量
            storageInfo.SuccessDeleteCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 22 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //下载总数
            storageInfo.TotalDownloadCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 23 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //成功下载数量
            storageInfo.SuccessDownloadCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 24 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //获取Meta总数
            storageInfo.TotalGetMetaCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 25 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //成功获取Meta数量
            storageInfo.SuccessGetMetaCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 26 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //创建链接总数
            storageInfo.TotalCreateLinkCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 27 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //成功创建链接数
            storageInfo.SuccessCreateLinkCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 28 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //删除链接总数
            storageInfo.TotalDeleteLinkCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 29 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //成功删除链接数
            storageInfo.SuccessDeleteLinkCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 30 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //上传总字节数
            storageInfo.TotalUploadBytes = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 31 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //成功上传总字节数
            storageInfo.SuccessUploadBytes = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 32 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //Append总字节数
            storageInfo.TotalAppendBytes = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 33 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //成功Append总字节数
            storageInfo.SuccessAppendBytes = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 34 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //Modify总字节数
            storageInfo.TotalModifyBytes = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 35 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //成功Modify总字节数
            storageInfo.SuccessModifyBytes = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 36 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //下载总字节数
            storageInfo.TotalDownloadBytes = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 37 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //成功下载字节数
            storageInfo.SuccessDownloadBytes = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 38 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //从其他服务器同步到本地的总字节数
            storageInfo.TotalSyncInBytes = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 39 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //成功从其他服务器同步到本地的总字节数
            storageInfo.SuccessSyncInBytes = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 40 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //同步到其他服务器的总字节数
            storageInfo.TotalSyncOutBytes = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 41 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //成功同步到其他服务器的总字节数
            storageInfo.SuccessSyncOutBytes = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 42 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //打开文件总数
            storageInfo.TotalFileOpenCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 43 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //成功打开文件的数量
            storageInfo.SuccessFileOpenCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 44 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //读取文件总数
            storageInfo.TotalFileReadCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 45 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //成功读取文件的数量
            storageInfo.SuccessFileReadCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 46 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //写入文件总数
            storageInfo.TotalFileWriteCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 47 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //成功写入文件的数量
            storageInfo.SuccessFileWriteCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 48 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);

            //最后源更新时间
            storageInfo.LastSourceUpdate = DateTimeUtil.ToDateTime((int)BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 49 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE));
            //最后同步更新时间
            storageInfo.LastSyncUpdate = DateTimeUtil.ToDateTime((int)BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 50 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE));
            //最后完成同步的时间戳
            storageInfo.LastSyncedTimestamp = DateTimeUtil.ToDateTime((int)BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 51 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE));
            //最后心跳时间
            storageInfo.LastHeartbeatTime = DateTimeUtil.ToDateTime((int)BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 52 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE));
            //是否TrunkServer
            storageInfo.IsTrunkServer = BitConverter.ToBoolean(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 53 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);

            return storageInfo;
        }

    }
}
