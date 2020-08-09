using FastDFSCore.Utility;
using System.Buffers;

namespace FastDFSCore.Protocols
{
    /// <summary>
    /// 查询一个Group
    /// 
    /// Reqeust 
    ///     Cmd: TRACKER_PROTO_CMD_SERVER_LIST_ONE_GROUP 90
    ///     Body:
    ///     @ FDFS_GROUP_NAME_MAX_LEN bytes: the group name to query
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
    public class ListOneGroup : FastDFSReq<ListOneGroupResp>
    {
        /// <summary>组名
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>Ctor
        /// </summary>
        public ListOneGroup()
        {
            Header = new FastDFSHeader(Consts.TRACKER_PROTO_CMD_SERVER_LIST_ONE_GROUP);
        }

        /// <summary>Ctor
        /// </summary>
        /// <param name="groupName">组名</param>
        public ListOneGroup(string groupName) : this()
        {
            GroupName = groupName;
        }

        /// <summary>EncodeBody
        /// </summary>
        public override byte[] EncodeBody(ClusterConfiguration configuration)
        {
            var bodyBuffer = EndecodeUtil.EncodeGroupName(GroupName, configuration.Charset);
            //Header = new FDFSHeader(Consts.FDFS_GROUP_NAME_MAX_LEN, Consts.TRACKER_PROTO_CMD_SERVER_LIST_ONE_GROUP, 0);
            return bodyBuffer;
        }

    }
}
