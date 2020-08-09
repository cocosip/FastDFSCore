using System.Buffers;

namespace FastDFSCore.Protocols
{
    /// <summary>
    /// 查询全部Group
    /// 
    /// Reqeust 
    ///     Cmd: TRACKER_PROTO_CMD_SERVER_LIST_ALL_GROUP 91
    ///     Body:
    /// Response
    ///     Cmd: TRACKER_PROTO_CMD_RESP
    ///     Status: 0 right other wrong
    ///     Body: 
    ///     @ FDFS_GROUP_NAME_MAX_LEN+1 bytes: group name
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: total disk storage in MB
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: free disk storage in MB
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: trunk free storage in MB
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: storage server count
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: storage server port
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: storage server http port
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: active server count
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: current write server index
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: store path count on storage server
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: subdir count per path on storage server
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: current_trunk_file_id
    /// </summary>
    public class ListAllGroup : FastDFSReq<ListAllGroupResp>
    {
        /// <summary>Ctor
        /// </summary>
        public ListAllGroup()
        {
            Header = new FastDFSHeader(0, Consts.TRACKER_PROTO_CMD_SERVER_LIST_ALL_GROUPS, 0);
        }

        /// <summary>EncodeBody
        /// </summary>
        public override byte[] EncodeBody(ClusterConfiguration configuration)
        {
            //Header = new FDFSHeader(0, Consts.TRACKER_PROTO_CMD_SERVER_LIST_ALL_GROUPS, 0);
            var bodyBuffer = new byte[0];
            return bodyBuffer;
        }
    }
}
