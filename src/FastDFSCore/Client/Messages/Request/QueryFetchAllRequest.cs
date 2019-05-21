using System.Collections.Generic;

namespace FastDFSCore.Client
{
    /// <summary>
    /// 查询该文件全部的Storage
    /// 
    /// Reqeust 
    ///     Cmd: TRACKER_PROTO_CMD_SERVICE_QUERY_FETCH_ALL 105
    ///     Body: 
    ///     @ FDFS_GROUP_NAME_MAX_LEN bytes: group name
    ///     @ filename bytes: filename
    /// Response
    ///     Cmd: TRACKER_PROTO_CMD_RESP
    ///     Status: 0 right other wrong
    ///     Body: 
    ///     @ FDFS_GROUP_NAME_MAX_LEN bytes: group name
    ///     @ IP_ADDRESS_SIZE - 1 bytes:  storage server ip address (multi)
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: storage server port (multi)
    /// </summary>
    public class QueryFetchAllRequest : FDFSRequest<QueryFetchAllResponse>
    {
        public string GroupName { get; set; }

        public string FileId { get; set; }

        public QueryFetchAllRequest()
        {

        }

        public QueryFetchAllRequest(string groupName, string fileId)
        {
            GroupName = groupName;
            FileId = fileId;
        }

        public override byte[] EncodeBody(FDFSOption option)
        {
            byte[] groupNameBuffer = Util.CreateGroupNameBuffer(option.Charset, GroupName);
            var fileIdBuffer = Util.StringToByte(option.Charset, FileId);
            var length = Consts.FDFS_GROUP_NAME_MAX_LEN + fileIdBuffer.Length;
            List<byte> bodyBuffer = new List<byte>();
            bodyBuffer.AddRange(groupNameBuffer);
            bodyBuffer.AddRange(fileIdBuffer);

            Header = new FDFSHeader(length, Consts.TRACKER_PROTO_CMD_SERVICE_QUERY_FETCH_ALL, 0);

            return bodyBuffer.ToArray();
        }
    }
}
