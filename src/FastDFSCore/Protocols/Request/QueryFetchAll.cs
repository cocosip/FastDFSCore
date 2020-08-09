using FastDFSCore.Utility;
using System.Buffers;

namespace FastDFSCore.Protocols
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
    public class QueryFetchAll : FastDFSReq<QueryFetchAllResp>
    {
        /// <summary>组名
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>文件FileId
        /// </summary>
        public string FileId { get; set; }

        /// <summary>Ctor
        /// </summary>
        public QueryFetchAll()
        {
            Header = new FastDFSHeader(Consts.TRACKER_PROTO_CMD_SERVICE_QUERY_FETCH_ALL);
        }

        /// <summary>Ctor
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="fileId">文件FileId</param>
        public QueryFetchAll(string groupName, string fileId) : this()
        {
            GroupName = groupName;
            FileId = fileId;
        }

        /// <summary>EncodeBody
        /// </summary>
        public override byte[] EncodeBody(ClusterConfiguration configuration)
        {
            var groupNameBuffer = EndecodeUtil.EncodeGroupName(GroupName, configuration.Charset);
            var fileIdBuffer = EndecodeUtil.EncodeString(FileId, configuration.Charset);
            //var length = Consts.FDFS_GROUP_NAME_MAX_LEN + fileIdBuffer.Length;

            return ByteUtil.Combine(groupNameBuffer, fileIdBuffer);
        }
    }
}
