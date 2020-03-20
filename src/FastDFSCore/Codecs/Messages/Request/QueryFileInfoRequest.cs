using System.Collections.Generic;

namespace FastDFSCore.Codecs.Messages
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
        /// <summary>组名
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>文件FileId
        /// </summary>
        public string FileId { get; set; }

        /// <summary>Ctor
        /// </summary>
        public QueryFileInfoRequest()
        {

        }

        /// <summary>Ctor
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="fileId">文件FileId</param>
        public QueryFileInfoRequest(string groupName, string fileId)
        {
            GroupName = groupName;
            FileId = fileId;
        }

        /// <summary>EncodeBody
        /// </summary>
        public override byte[] EncodeBody(FDFSOption option)
        {
            byte[] groupNameBuffer = EndecodeUtil.EncodeGroupName(GroupName, option.Charset);
            var fileIdBuffer = EndecodeUtil.EncodeString(FileId, option.Charset);
            var length = Consts.FDFS_GROUP_NAME_MAX_LEN + fileIdBuffer.Length;
            List<byte> bodyBuffer = new List<byte>();
            bodyBuffer.AddRange(groupNameBuffer);
            bodyBuffer.AddRange(fileIdBuffer);

            Header = new FDFSHeader(length, Consts.STORAGE_PROTO_CMD_QUERY_FILE_INFO, 0);
            return bodyBuffer.ToArray();
        }
    }
}
