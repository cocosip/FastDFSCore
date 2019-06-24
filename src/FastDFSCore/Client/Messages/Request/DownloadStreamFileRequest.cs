using System.Collections.Generic;

namespace FastDFSCore.Client
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
    public class DownloadStreamFileRequest : FDFSRequest<DownloadStreamFileResponse>
    {
        public long Offset { get; set; }

        public long ByteSize { get; set; }

        public string GroupName { get; set; }

        public string FileId { get; set; }

        public override bool StreamResponse => true;
        public DownloadStreamFileRequest()
        {

        }

        public DownloadStreamFileRequest(string groupName, string fileId)
        {
            Offset = 0;
            ByteSize = 0;
            GroupName = groupName;
            FileId = fileId;
        }

        public override byte[] EncodeBody(FDFSOption option)
        {
            byte[] groupNameBuffer = Util.CreateGroupNameBuffer(option.Charset, GroupName);
            //文件偏移量
            byte[] offsetBuffer = Util.LongToBuffer(Offset);
            //下载文件的大小,全部下载用0
            byte[] byteSizeBuffer = Util.LongToBuffer(ByteSize);
            //文件FileId数组
            byte[] fileIdBuffer = Util.StringToByte(option.Charset, FileId);

            long length = Consts.FDFS_PROTO_PKG_LEN_SIZE + Consts.FDFS_PROTO_PKG_LEN_SIZE + Consts.FDFS_GROUP_NAME_MAX_LEN + FileId.Length;

            List<byte> bodyBuffer = new List<byte>();
            bodyBuffer.AddRange(offsetBuffer);
            bodyBuffer.AddRange(byteSizeBuffer);
            bodyBuffer.AddRange(groupNameBuffer);
            bodyBuffer.AddRange(fileIdBuffer);

            Header = new FDFSHeader(length, Consts.STORAGE_PROTO_CMD_DOWNLOAD_FILE, 0);
            return bodyBuffer.ToArray();
        }

    }
}
