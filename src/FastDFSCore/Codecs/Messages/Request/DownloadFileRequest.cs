using FastDFSCore.Utility;
using System.Collections.Generic;

namespace FastDFSCore.Codecs.Messages
{
    /// <summary>
    /// 下载文件
    /// 
    /// Reqeust 
    ///     Cmd: STORAGE_PROTO_CMD_DOWNLOAD_FILE 14
    ///     Body:
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: file offset
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: download file bytes      
    ///     @ FDFS_GROUP_NAME_MAX_LEN bytes: group name
    ///     @ filename bytes: filename
    /// Response
    ///     Cmd: STORAGE_PROTO_CMD_RESP
    ///     Status: 0 right other wrong
    ///     Body: 
    ///     @ file content
    /// </summary>
    public class DownloadFileRequest : FDFSRequest<DownloadFileResponse>
    {
        /// <summary>偏移量
        /// </summary>
        public long Offset { get; set; }

        /// <summary>下载的大小
        /// </summary>
        public long ByteSize { get; set; }

        /// <summary>组名
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>文件FileId
        /// </summary>
        public string FileId { get; set; }

        /// <summary>Ctor
        /// </summary>
        public DownloadFileRequest()
        {
            Header = new FDFSHeader(Consts.STORAGE_PROTO_CMD_DOWNLOAD_FILE);
        }

        /// <summary>Ctor
        /// </summary>
        /// <param name="offset">偏移量</param>
        /// <param name="byteSize">下载的大小</param>
        /// <param name="groupName">组名</param>
        /// <param name="fileId">文件FileId</param>
        public DownloadFileRequest(long offset, long byteSize, string groupName, string fileId) : this()
        {
            Offset = offset;
            ByteSize = byteSize;
            GroupName = groupName;
            FileId = fileId;
        }

        /// <summary>EncodeBody
        /// </summary>
        public override byte[] EncodeBody(FDFSOption option)
        {
            var groupNameBuffer = EndecodeUtil.EncodeGroupName(GroupName, option.Charset);
            //文件偏移量
            var offsetBuffer = EndecodeUtil.EncodeLong(Offset);
            //下载文件的大小,全部下载用0
            var byteSizeBuffer = EndecodeUtil.EncodeLong(ByteSize);
            //文件FileId数组
            var fileIdBuffer = EndecodeUtil.EncodeString(FileId, option.Charset);

            //long length = Consts.FDFS_PROTO_PKG_LEN_SIZE + Consts.FDFS_PROTO_PKG_LEN_SIZE + Consts.FDFS_GROUP_NAME_MAX_LEN + FileId.Length;

            return ByteUtil.Combine(offsetBuffer, byteSizeBuffer, groupNameBuffer, fileIdBuffer);
        }



    }
}
