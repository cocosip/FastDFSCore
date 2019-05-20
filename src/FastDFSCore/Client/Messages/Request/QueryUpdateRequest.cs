using System;

namespace FastDFSCore.Client
{
    /// <summary>
    /// 查询更新文件时的Storage
    /// 
    /// Reqeust 
    ///     Cmd: TRACKER_PROTO_CMD_SERVICE_QUERY_UPDATE 103
    ///     Body:
    ///     @ FDFS_GROUP_NAME_MAX_LEN bytes:  group name
    ///     @ filename bytes: filename
    /// Response
    ///     Cmd: TRACKER_PROTO_CMD_RESP
    ///     Status: 0 right other wrong
    ///     Body: 
    ///     @ FDFS_GROUP_NAME_MAX_LEN bytes: group name
    ///     @ IP_ADDRESS_SIZE - 1 bytes: storage server ip address
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: storage server port

    /// </summary>
    public class QueryUpdateRequest : FDFSRequest<QueryUpdateResponse>
    {
        public string GroupName { get; set; }

        public string FileId { get; set; }

        public QueryUpdateRequest()
        {

        }

        public QueryUpdateRequest(string groupName, string fileId)
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

            Header = new FDFSHeader(length, Consts.TRACKER_PROTO_CMD_SERVICE_QUERY_UPDATE, 0);
            return bodyBuffer;
        }
    }
}
