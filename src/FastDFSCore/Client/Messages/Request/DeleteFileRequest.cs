using System;

namespace FastDFSCore.Client
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
    ///         
    /// </summary>
    public class DeleteFileRequest : FDFSRequest<DeleteFileResponse>
    {
        public string GroupName { get; set; }

        public string FileId { get; set; }

        public DeleteFileRequest()
        {

        }

        public DeleteFileRequest(string groupName, string fileId)
        {
            GroupName = groupName;
            FileId = fileId;
        }


        /// 1,string groupName
        /// 2,string fileName
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

            Header = new FDFSHeader(length, Consts.STORAGE_PROTO_CMD_DELETE_FILE, 0);

            return bodyBuffer;
        }
    }
}
