using System;
using System.Collections.Generic;
using System.Text;

namespace FastDFSCore.Client
{
    /// <summary>
    /// 查询文件信息
    /// 
    /// Reqeust 
    ///     Cmd: STORAGE_PROTO_CMD_QUERY_FILE_INFO 22
    ///     Body:   
    ///     @ FDFS_GROUP_NAME_MAX_LEN bytes: group name
    ///     @ filename bytes: filename
    /// Response
    ///     Cmd: STORAGE_PROTO_CMD_RESP
    ///     Status: 0 right other wrong
    ///     Body: 
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: file size
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: file create timestamp
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: file CRC32 signature
    /// </summary>
    public class QueryFileInfoRequest : FDFSRequest<QueryFileInfoResponse>
    {
        public string GroupName { get; set; }

        public string FileId { get; set; }

        public QueryFileInfoRequest()
        {

        }

        public QueryFileInfoRequest(string groupName, string fileId)
        {
            GroupName = groupName;
            FileId = fileId;
        }

        public override byte[] EncodeBody(FDFSOption option)
        {
            var groupNameBuffer = option.Charset.GetBytes(GroupName);
            if (groupNameBuffer.Length > Consts.FDFS_GROUP_NAME_MAX_LEN)
            {
                throw new ArgumentException("GroupName is too long.");
            }
            var fileIdBuffer = option.Charset.GetBytes(FileId);
            var length = Consts.FDFS_GROUP_NAME_MAX_LEN + fileIdBuffer.Length;
            var bodyBuffer = new byte[length];
            Array.Copy(groupNameBuffer, 0, bodyBuffer, 0, groupNameBuffer.Length);
            Array.Copy(fileIdBuffer, 0, bodyBuffer, Consts.FDFS_GROUP_NAME_MAX_LEN, fileIdBuffer.Length);

            Header = new FDFSHeader(length, Consts.STORAGE_PROTO_CMD_QUERY_FILE_INFO, 0);
            return bodyBuffer;
        }
    }
}
