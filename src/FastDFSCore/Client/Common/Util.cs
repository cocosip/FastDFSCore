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

            //StorageId
            var idBuffer = new byte[Consts.FDFS_STORAGE_ID_MAX_SIZE];
            Array.Copy(buffer, 0, idBuffer, 0, idBuffer.Length);
            storageInfo.StorageId = ByteToString(encoding, idBuffer).TrimEnd('\0');

            //ipaddress
            var ipAddressBuffer = new byte[Consts.IP_ADDRESS_SIZE];
            Array.Copy(buffer, 0, ipAddressBuffer, Consts.FDFS_STORAGE_ID_MAX_SIZE, ipAddressBuffer.Length);
            storageInfo.IPAddress = ByteToString(encoding, ipAddressBuffer).TrimEnd('\0');

            //domain
            var domainBuffer = new byte[Consts.FDFS_DOMAIN_NAME_MAX_SIZE];
            Array.Copy(buffer, 0, domainBuffer, Consts.FDFS_STORAGE_ID_MAX_SIZE + Consts.IP_ADDRESS_SIZE, domainBuffer.Length);
            storageInfo.DomainName = ByteToString(encoding, domainBuffer).TrimEnd('\0');

            //version
            var versionBuffer = new byte[Consts.FDFS_VERSION_SIZE];
            Array.Copy(buffer, 0, versionBuffer, Consts.FDFS_STORAGE_ID_MAX_SIZE + Consts.IP_ADDRESS_SIZE + Consts.FDFS_DOMAIN_NAME_MAX_SIZE, versionBuffer.Length);
            storageInfo.DomainName = ByteToString(encoding, versionBuffer).TrimEnd('\0');

            //跳过的字节
            var skipCount = Consts.FDFS_STORAGE_ID_MAX_SIZE + Consts.IP_ADDRESS_SIZE + Consts.FDFS_DOMAIN_NAME_MAX_SIZE + Consts.FDFS_VERSION_SIZE;
            //join time
            storageInfo.JoinTime = new DateTime(BufferToLong(buffer, skipCount));
            //up time
            storageInfo.UpTime = new DateTime(BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE));
            //总容量
            storageInfo.TotalMb = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 2);
            //空余容量
            storageInfo.FreeMb = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 3);
            //上传优先级
            storageInfo.UploadPriority = (int)BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 4);
            //端口号
            storageInfo.StoragePort = (int)BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 5);
            //Http端口号
            storageInfo.StorageHttpPort = (int)BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 6);
            //存储路径数量
            storageInfo.StorePathCount = (int)BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 7);
            //子目录数量
            storageInfo.SubdirCount = (int)BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 8);
            //当前写入的路径
            storageInfo.CurrentWritePath = (int)BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 9);
            //AllocCount
            storageInfo.AllocCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 10);
            //CurrentCount
            storageInfo.CurrentCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 11);
            //MaxCount
            storageInfo.MaxCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 12);
            //上传总数
            storageInfo.TotalUploadCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 13);
            //成功上传数量
            storageInfo.SuccessUploadCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 14);
            //Append总数
            storageInfo.TotalAppendCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 15);
            //成功Append数量
            storageInfo.SuccessAppendCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 16);
            //Modify数量
            storageInfo.TotalModifyCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 17);
            //成功Modify数量
            storageInfo.SuccessModifyCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 18);
            //Truncate总数
            storageInfo.TotalTruncateCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 19);
            //成功Truncate数量
            storageInfo.SuccessTruncateCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 20);
            //设置Meta总数
            storageInfo.TotalSetMetaCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 21);
            //成功设置Meta数量
            storageInfo.SuccessSetMetaCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 22);
            //删除总数
            storageInfo.TotalDeleteCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 23);
            //成功删除数量
            storageInfo.SuccessDeleteCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 24);
            //下载总数
            storageInfo.TotalDownloadCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 25);
            //成功下载数量
            storageInfo.SuccessDownloadCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 26);
            //获取Meta总数
            storageInfo.TotalGetMetaCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 27);
            //成功获取Meta数量
            storageInfo.SuccessGetMetaCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 28);
            //最后源更新时间
            storageInfo.LastSourceUpdate = new DateTime(BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 29));
            //最后同步更新时间
            storageInfo.LastSyncUpdate = new DateTime(BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 30));
            //最后完成同步的时间戳
            storageInfo.LastSyncedTimestamp = new DateTime(BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 31));
            //创建链接总数
            storageInfo.TotalCreateLinkCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 32);
            //成功创建链接数
            storageInfo.SuccessCreateLinkCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 33);
            //删除链接总数
            storageInfo.TotalDeleteLinkCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 34);
            //成功删除链接数
            storageInfo.SuccessDeleteLinkCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 35);
            //上传总字节数
            storageInfo.TotalUploadBytes = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 36);
            //成功上传总字节数
            storageInfo.SuccessUploadBytes = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 37);
            //Append总字节数
            storageInfo.TotalAppendBytes = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 38);
            //成功Append总字节数
            storageInfo.SuccessAppendBytes = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 39);
            //Modify总字节数
            storageInfo.TotalModifyBytes = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 40);
            //成功Modify总字节数
            storageInfo.SuccessModifyBytes = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 41);
            //下载总字节数
            storageInfo.TotalDownloadBytes = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 42);
            //成功下载字节数
            storageInfo.SuccessDownloadBytes = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 43);
            //从其他服务器同步到本地的总字节数
            storageInfo.TotalSyncInBytes = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 44);
            //成功从其他服务器同步到本地的总字节数
            storageInfo.SuccessSyncInBytes = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 45);
            //同步到其他服务器的总字节数
            storageInfo.TotalSyncOutBytes = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 46);
            //成功同步到其他服务器的总字节数
            storageInfo.SuccessSyncOutBytes = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 47);
            //打开文件总数
            storageInfo.TotalFileOpenCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 48);
            //成功打开文件的数量
            storageInfo.SuccessFileOpenCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 49);
            //读取文件总数
            storageInfo.TotalFileReadCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 50);
            //成功读取文件的数量
            storageInfo.SuccessFileReadCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 51);
            //写入文件总数
            storageInfo.TotalFileWriteCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 52);
            //成功写入文件的数量
            storageInfo.SuccessFileWriteCount = BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 53);
            //最后心跳时间
            storageInfo.LastHeartbeatTime = new DateTime(BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 54));



            return storageInfo;
        }

    }
}
