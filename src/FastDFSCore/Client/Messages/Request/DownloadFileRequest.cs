using System;
using System.Collections.Generic;
using System.Text;

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
    public class DownloadFileRequest : FDFSRequest<DownloadFileResponse>
    {
        public long Offset { get; set; }

        public long ByteSize { get; set; }

        public string GroupName { get; set; }

        public string FileId { get; set; }


        public DownloadFileRequest()
        {

        }

        public DownloadFileRequest(long offset, long byteSize, string groupName, string fileId)
        {
            Offset = offset;
            ByteSize = byteSize;
            GroupName = groupName;
            FileId = fileId;
        }

        public override bool ResponseStream => true;

        public override byte[] EncodeBody(FDFSOption option)
        {
            var groupNameBuffer = option.Charset.GetBytes(GroupName);
            if (groupNameBuffer.Length > Consts.FDFS_GROUP_NAME_MAX_LEN)
            {
                throw new ArgumentException("GroupName is too long.");
            }

            long length = Consts.FDFS_PROTO_PKG_LEN_SIZE + Consts.FDFS_PROTO_PKG_LEN_SIZE + Consts.FDFS_GROUP_NAME_MAX_LEN + FileId.Length;

            //文件偏移量
            byte[] offsetBuffer = BitConverter.GetBytes(Offset);
            //下载文件的大小,全部下载用0
            byte[] byteSizeBuffer = BitConverter.GetBytes(ByteSize);
            //文件FileId数组
            byte[] fileIdBuffer = option.Charset.GetBytes(FileId);

            var bodyBuffer = new byte[length];
            Array.Copy(offsetBuffer, 0, bodyBuffer, 0, offsetBuffer.Length);
            Array.Copy(byteSizeBuffer, 0, bodyBuffer, Consts.FDFS_PROTO_PKG_LEN_SIZE, byteSizeBuffer.Length);
            Array.Copy(groupNameBuffer, 0, bodyBuffer, Consts.FDFS_PROTO_PKG_LEN_SIZE +
             Consts.FDFS_PROTO_PKG_LEN_SIZE, groupNameBuffer.Length);
            Array.Copy(fileIdBuffer, 0, bodyBuffer, Consts.FDFS_PROTO_PKG_LEN_SIZE +
              Consts.FDFS_PROTO_PKG_LEN_SIZE + Consts.FDFS_GROUP_NAME_MAX_LEN, fileIdBuffer.Length);

            Header = new FDFSHeader(length, Consts.STORAGE_PROTO_CMD_DOWNLOAD_FILE, 0);
            return bodyBuffer;
        }



    }
}
