using FastDFSCore.Utility;
using System.Buffers;

namespace FastDFSCore.Protocols
{
    /// <summary>
    /// 获取文件Meta
    /// Reqeust 
    ///     Cmd: STORAGE_PROTO_CMD_GET_METADATA 15
    ///     Body:   
    ///     @ FDFS_GROUP_NAME_MAX_LEN bytes: group name
    ///     @ filename bytes: filename
    /// Response
    ///     Cmd: STORAGE_PROTO_CMD_RESP
    ///     Status: 0 right other wrong
    ///     Body: 
    ///     @ meta data buff, each meta data seperated by \x01, name and value seperated by \x02
    /// </summary>
    public class GetMetaData : FastDFSReq<GetMetaDataResp>
    {
        /// <summary>组名
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>文件Id
        /// </summary>
        public string FileId { get; set; }

        /// <summary>Ctor
        /// </summary>
        public GetMetaData()
        {
            Header = new FastDFSHeader(Consts.STORAGE_PROTO_CMD_GET_METADATA);
        }

        /// <summary>Ctor
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="fileId">文件FileId</param>
        public GetMetaData(string groupName, string fileId)
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
