using FastDFSCore.Protocols;
using System;
using System.Text;

namespace FastDFSCore.Utility
{
    /// <summary>编码,解码操作
    /// </summary>
    public static class EndecodeUtil
    {
        /// <summary>按照长度限制编码成数组
        /// </summary>
        public static byte[] EncodeLimit(string source, int length, string charset = "utf-8")
        {
            var buffer = new byte[length];
            var sourceBuffer = Encoding.GetEncoding(charset).GetBytes(source);
            Array.Copy(sourceBuffer, buffer, Math.Min(buffer.Length, sourceBuffer.Length));
            return buffer;
        }


        /// <summary>按照长度限制编码成数组
        /// </summary>
        public static byte[] EncodeString(string source, string charset = "utf-8")
        {
            var buffer = Encoding.GetEncoding(charset).GetBytes(source);
            return buffer;
        }

        /// <summary>编码Long类型
        /// </summary>
        public static byte[] EncodeLong(long l)
        {
            return ByteUtil.LongToBuffer(l);
        }


        /// <summary>解码字符串操作
        /// </summary>
        public static string DecodeString(byte[] buffer, string charset = "utf-8")
        {
            return ByteUtil.ByteToString(buffer, charset).TrimEnd('\0');
        }

        /// <summary>组名转二进制数组
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="charset">字符集</param>
        /// <returns></returns>
        public static byte[] EncodeGroupName(string groupName, string charset = "utf-8")
        {
            return EncodeLimit(groupName, Consts.FDFS_GROUP_NAME_MAX_LEN, charset);
        }

        /// <summary>扩展名转二进制数组
        /// </summary>
        /// <param name="ext">扩展名</param>
        /// <param name="charset">字符集</param>
        /// <returns></returns>
        public static byte[] EncodeFileExt(string ext, string charset = "utf-8")
        {
            return EncodeLimit(ext, Consts.FDFS_FILE_EXT_NAME_MAX_LEN, charset);
        }

        /// <summary>前缀转二进制数组
        /// </summary>
        /// <param name="prefix">前缀</param>
        /// <param name="charset">字符集</param>
        /// <returns></returns>
        public static byte[] EncodePrefix(string prefix, string charset = "utf-8")
        {
            return EncodeLimit(prefix, Consts.FDFS_FILE_PREFIX_MAX_LEN, charset);
        }

        /// <summary>从二进制中解码GroupInfo
        /// </summary>
        public static GroupInfo DecodeGroupInfo(byte[] buffer, string charset = "utf-8")
        {
            var groupInfo = new GroupInfo();

            //var bufferSpan = buffer.AsSpan();

            //GroupName
            var groupNameBuffer = new byte[Consts.FDFS_GROUP_NAME_MAX_LEN + 1];
            Array.Copy(buffer, 0, groupNameBuffer, 0, groupNameBuffer.Length);
            groupInfo.GroupName = ByteUtil.ByteToString(groupNameBuffer, charset).TrimEnd('\0');

            //total disk
            groupInfo.TotalMB = ByteUtil.BufferToLong(buffer, Consts.FDFS_GROUP_NAME_MAX_LEN + 1);
            //free disk storage in MB
            groupInfo.FreeMB = ByteUtil.BufferToLong(buffer, Consts.FDFS_GROUP_NAME_MAX_LEN + 1 + Consts.FDFS_PROTO_PKG_LEN_SIZE * 1);
            //TrunkFreeMb
            groupInfo.TrunkFreeMb = ByteUtil.BufferToLong(buffer, Consts.FDFS_GROUP_NAME_MAX_LEN + 1 + Consts.FDFS_PROTO_PKG_LEN_SIZE * 2);

            //storage server count
            groupInfo.ServerCount = (int)ByteUtil.BufferToLong(buffer, Consts.FDFS_GROUP_NAME_MAX_LEN + 1 + Consts.FDFS_PROTO_PKG_LEN_SIZE * 3);
            //storage server port
            groupInfo.StoragePort = (int)ByteUtil.BufferToLong(buffer, Consts.FDFS_GROUP_NAME_MAX_LEN + 1 + Consts.FDFS_PROTO_PKG_LEN_SIZE * 4);
            //storage server http port
            groupInfo.StorageHttpPort = (int)ByteUtil.BufferToLong(buffer, Consts.FDFS_GROUP_NAME_MAX_LEN + 1 + Consts.FDFS_PROTO_PKG_LEN_SIZE * 5);
            //active server count
            groupInfo.ActiveCount = (int)ByteUtil.BufferToLong(buffer, Consts.FDFS_GROUP_NAME_MAX_LEN + 1 + Consts.FDFS_PROTO_PKG_LEN_SIZE * 6);
            //current write server index
            groupInfo.CurrentWriteServerIndex = (int)ByteUtil.BufferToLong(buffer, Consts.FDFS_GROUP_NAME_MAX_LEN + 1 + Consts.FDFS_PROTO_PKG_LEN_SIZE * 7);
            //store path count on storage server
            groupInfo.StorePathCount = (int)ByteUtil.BufferToLong(buffer, Consts.FDFS_GROUP_NAME_MAX_LEN + 1 + Consts.FDFS_PROTO_PKG_LEN_SIZE * 8);
            // subdir count per path on storage server
            groupInfo.SubdirCount = (int)ByteUtil.BufferToLong(buffer, Consts.FDFS_GROUP_NAME_MAX_LEN + 1 + Consts.FDFS_PROTO_PKG_LEN_SIZE * 9);
            //
            groupInfo.CurrentTrunkFileId = (int)ByteUtil.BufferToLong(buffer, Consts.FDFS_GROUP_NAME_MAX_LEN + 1 + Consts.FDFS_PROTO_PKG_LEN_SIZE * 10);

            return groupInfo;
        }

        /// <summary>从二进制中解码StorageInfo
        /// </summary>
        public static StorageInfo DecodeStorageInfo(byte[] buffer, string charset = "utf-8")
        {
            var storageInfo = new StorageInfo
            {
                //状态
                Status = buffer[0]
            };

            var span = buffer.AsSpan();

            //Id
            var idSpan = span.Slice(1, Consts.FDFS_STORAGE_ID_MAX_SIZE);
            storageInfo.StorageId = ByteUtil.ByteToString(idSpan, charset).TrimEnd('\0').TrimStart('\a');

            //ipaddress
            var ipSpan = span.Slice(1 + Consts.FDFS_STORAGE_ID_MAX_SIZE, Consts.IP_ADDRESS_SIZE);
            storageInfo.IPAddress = ByteUtil.ByteToString(ipSpan, charset).TrimEnd('\0');

            //src ipaddress
            var srcIpSpan = span.Slice(1 + Consts.FDFS_STORAGE_ID_MAX_SIZE + Consts.IP_ADDRESS_SIZE, Consts.IP_ADDRESS_SIZE);
            storageInfo.SrcIPAddress = ByteUtil.ByteToString(srcIpSpan, charset).TrimEnd('\0');

            //domain
            var domainSpan = span.Slice(1 + Consts.FDFS_STORAGE_ID_MAX_SIZE + Consts.IP_ADDRESS_SIZE * 2, Consts.FDFS_DOMAIN_NAME_MAX_SIZE);
            storageInfo.DomainName = ByteUtil.ByteToString(domainSpan, charset).TrimEnd('\0');

            //version
            var versionSpan = span.Slice(1 + Consts.FDFS_STORAGE_ID_MAX_SIZE + Consts.IP_ADDRESS_SIZE * 2 + Consts.FDFS_DOMAIN_NAME_MAX_SIZE, Consts.FDFS_VERSION_SIZE);
            storageInfo.Version = ByteUtil.ByteToString(versionSpan, charset).TrimEnd('\0');

            //跳过的字节
            var skipCount = 1 + Consts.FDFS_STORAGE_ID_MAX_SIZE + Consts.IP_ADDRESS_SIZE * 2 + Consts.FDFS_DOMAIN_NAME_MAX_SIZE + Consts.FDFS_VERSION_SIZE;
            //182

            //join time
            storageInfo.JoinTime = DateTimeUtil.ToDateTime((int)ByteUtil.BufferToLong(buffer, skipCount));
            //up time
            storageInfo.UpTime = DateTimeUtil.ToDateTime((int)ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE));
            //总容量
            storageInfo.TotalMb = ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 2);
            //空余容量
            storageInfo.FreeMb = ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 3);
            //上传优先级
            storageInfo.UploadPriority = (int)ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 4);
            //存储路径数量
            storageInfo.StorePathCount = (int)ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 5);
            //子目录数
            storageInfo.SubdirCount = (int)ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 6);
            //当前写入路径数量
            storageInfo.CurrentWritePath = (int)ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 7);
            //端口号
            storageInfo.StoragePort = (int)ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 8);
            //Http端口号
            storageInfo.StorageHttpPort = (int)ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 9);
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
            storageInfo.TotalUploadCount = ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 11 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //成功上传数量
            storageInfo.SuccessUploadCount = ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 12 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //Append总数
            storageInfo.TotalAppendCount = ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 13 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //成功Append数量
            storageInfo.SuccessAppendCount = ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 14 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //Modify数量
            storageInfo.TotalModifyCount = ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 15 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //成功Modify数量
            storageInfo.SuccessModifyCount = ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 16 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //Truncate总数
            storageInfo.TotalTruncateCount = ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 17 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //成功Truncate数量
            storageInfo.SuccessTruncateCount = ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 18 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //设置Meta总数
            storageInfo.TotalSetMetaCount = ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 19 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //成功设置Meta数量
            storageInfo.SuccessSetMetaCount = ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 20 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //删除总数
            storageInfo.TotalDeleteCount = ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 21 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //成功删除数量
            storageInfo.SuccessDeleteCount = ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 22 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //下载总数
            storageInfo.TotalDownloadCount = ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 23 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //成功下载数量
            storageInfo.SuccessDownloadCount = ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 24 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //获取Meta总数
            storageInfo.TotalGetMetaCount = ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 25 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //成功获取Meta数量
            storageInfo.SuccessGetMetaCount = ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 26 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //创建链接总数
            storageInfo.TotalCreateLinkCount = ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 27 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //成功创建链接数
            storageInfo.SuccessCreateLinkCount = ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 28 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //删除链接总数
            storageInfo.TotalDeleteLinkCount = ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 29 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //成功删除链接数
            storageInfo.SuccessDeleteLinkCount = ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 30 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //上传总字节数
            storageInfo.TotalUploadBytes = ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 31 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //成功上传总字节数
            storageInfo.SuccessUploadBytes = ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 32 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //Append总字节数
            storageInfo.TotalAppendBytes = ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 33 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //成功Append总字节数
            storageInfo.SuccessAppendBytes = ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 34 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //Modify总字节数
            storageInfo.TotalModifyBytes = ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 35 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //成功Modify总字节数
            storageInfo.SuccessModifyBytes = ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 36 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //下载总字节数
            storageInfo.TotalDownloadBytes = ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 37 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //成功下载字节数
            storageInfo.SuccessDownloadBytes = ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 38 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //从其他服务器同步到本地的总字节数
            storageInfo.TotalSyncInBytes = ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 39 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //成功从其他服务器同步到本地的总字节数
            storageInfo.SuccessSyncInBytes = ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 40 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //同步到其他服务器的总字节数
            storageInfo.TotalSyncOutBytes = ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 41 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //成功同步到其他服务器的总字节数
            storageInfo.SuccessSyncOutBytes = ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 42 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //打开文件总数
            storageInfo.TotalFileOpenCount = ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 43 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //成功打开文件的数量
            storageInfo.SuccessFileOpenCount = ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 44 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //读取文件总数
            storageInfo.TotalFileReadCount = ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 45 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //成功读取文件的数量
            storageInfo.SuccessFileReadCount = ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 46 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //写入文件总数
            storageInfo.TotalFileWriteCount = ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 47 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);
            //成功写入文件的数量
            storageInfo.SuccessFileWriteCount = ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 48 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);

            //最后源更新时间
            storageInfo.LastSourceUpdate = DateTimeUtil.ToDateTime((int)ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 49 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE));
            //最后同步更新时间
            storageInfo.LastSyncUpdate = DateTimeUtil.ToDateTime((int)ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 50 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE));
            //最后完成同步的时间戳
            storageInfo.LastSyncedTimestamp = DateTimeUtil.ToDateTime((int)ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 51 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE));
            //最后心跳时间
            storageInfo.LastHeartbeatTime = DateTimeUtil.ToDateTime((int)ByteUtil.BufferToLong(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 52 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE));
            //是否TrunkServer
            storageInfo.IsTrunkServer = BitConverter.ToBoolean(buffer, skipCount + Consts.FDFS_PROTO_PKG_LEN_SIZE * 53 + Consts.FDFS_PROTO_PKG_INT_LEN_SIZE);

            return storageInfo;
        }
    }
}
