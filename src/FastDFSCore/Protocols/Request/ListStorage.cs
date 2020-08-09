using FastDFSCore.Utility;
using System.Buffers;

namespace FastDFSCore.Protocols
{
    /// <summary>列出Storage
    /// </summary>
    public class ListStorage : FastDFSReq<ListStorageResp>
    {
        /// <summary>组名称
        ///Reqeust 
        ///    Cmd: TRACKER_PROTO_CMD_SERVER_LIST_STORAGE 92
        ///    Body:
        ///    @ FDFS_GROUP_NAME_MAX_LEN bytes: the group name to query
        ///Response
        ///    Cmd: TRACKER_PROTO_CMD_RESP
        ///    Status: 0 right other wrong
        ///    Body:(n)
        ///    @ 1 byte: status
        ///    @ FDFS_STORAGE_ID_MAX_SIZE bytes:StorageId 
        ///    @ FDFS_IPADDR_SIZE bytes: ip address
        ///    @ FDFS_IPADDR_SIZE bytes: ip address 
        ///    @ FDFS_DOMAIN_NAME_MAX_SIZE  bytes : domain name of the web server
        ///    @ IP_ADDRESS_SIZE bytes: source storage server ip address
        ///    @ FDFS_VERSION_SIZE bytes: storage server version
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: join time (join in timestamp)
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: up time (start timestamp)
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: total space in MB
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: free space in MB
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: upload priority
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: storage server port
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: storage http port
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: store path count
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: subdir count per path
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: current write path
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: alloc_count
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: current_count
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: max_count
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: total upload count
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: success upload count
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: total append count
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: success append count
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: total modify count
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: success modify count
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: total truncate count
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: success truncate count
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: total set metadata count
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: success set metadata count  
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: total delete count
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: success delete count
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: total download count
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: success download count
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: total get metadata count
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: success get metadata count
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: last_source_update
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: last_sync_update
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: last_synced_timestamp
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: total create link count
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: success create link count
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: total delete link count
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: success delete link count
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: total_upload_bytes
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: success_upload_bytes
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: total_append_bytes
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: success_append_bytes
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: total_modify_bytes
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: success_modify_bytes
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: total_download_bytes
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: success_download_bytes
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: total_sync_in_bytes
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: success_sync_in_bytes
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: total_sync_out_bytes
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: success_sync_out_bytes
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: total_file_open_count
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: success_file_open_count
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: total_file_read_count
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: success_file_read_count
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: total_file_write_count
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: success_file_write_count
        ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes:  last heart beat timestamp
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>Ctor
        /// </summary>
        public ListStorage()
        {
            Header = new FastDFSHeader(Consts.TRACKER_PROTO_CMD_SERVER_LIST_STORAGE);
        }

        /// <summary>Ctor
        /// </summary>
        /// <param name="groupName">组名称</param>
        public ListStorage(string groupName) : this()
        {
            GroupName = groupName;
        }

        /// <summary>EncodeBody
        /// </summary>
        public override byte[] EncodeBody(ClusterConfiguration configuration)
        {
            var bodyBuffer = EndecodeUtil.EncodeGroupName(GroupName, configuration.Charset);
            //Header = new FDFSHeader(Consts.FDFS_GROUP_NAME_MAX_LEN, Consts.TRACKER_PROTO_CMD_SERVER_LIST_STORAGE, 0);
            return bodyBuffer;
        }
    }
}
