using FastDFSCore.Utility;

namespace FastDFSCore.Protocols
{
    /// <summary>
    /// 删除文件
    /// 
    /// Reqeust 
    ///     Cmd: STORAGE_PROTO_CMD_DELETE_FILE 12
    ///     Body:
    ///     @ FDFS_GROUP_NAME_MAX_LEN bytes: group name
    ///     @ filename bytes: filename
    /// Response
    ///     Cmd: STORAGE_PROTO_CMD_RESP
    ///     Status: 0 right other wrong
    ///     Body:       
    /// </summary>
    public class DeleteFileRequest : FDFSRequest<DeleteFileResponse>
    {
        /// <summary>组名
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>文件FileId
        /// </summary>
        public string FileId { get; set; }

        /// <summary>Ctor
        /// </summary>
        public DeleteFileRequest()
        {
            Header = new FDFSHeader(Consts.STORAGE_PROTO_CMD_DELETE_FILE);
        }

        /// <summary>Ctor
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="fileId">文件FileId</param>
        public DeleteFileRequest(string groupName, string fileId) : this()
        {
            GroupName = groupName;
            FileId = fileId;
        }


        /// <summary>EncodeBody
        /// </summary>
        public override byte[] EncodeBody(FDFSOption option)
        {
            var groupNameBuffer = EndecodeUtil.EncodeGroupName(GroupName, option.Charset);
            var fileIdBuffer = EndecodeUtil.EncodeString(FileId, option.Charset);

            //var length = Consts.FDFS_GROUP_NAME_MAX_LEN + fileIdBuffer.Length;
            return ByteUtil.Combine(groupNameBuffer, fileIdBuffer);

        }
    }
}
