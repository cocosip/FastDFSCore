using System;
using System.Collections.Generic;

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

        }

        /// <summary>Ctor
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="fileId">文件FileId</param>
        public DeleteFileRequest(string groupName, string fileId)
        {
            GroupName = groupName;
            FileId = fileId;
        }


        /// <summary>EncodeBody
        /// </summary>
        public override byte[] EncodeBody(FDFSOption option)
        {

            byte[] groupNameBuffer = Util.CreateGroupNameBuffer(option.Charset, GroupName);
            if (groupNameBuffer.Length > Consts.FDFS_GROUP_NAME_MAX_LEN)
            {
                throw new ArgumentException("GroupName is too long.");
            }
            byte[] fileIdBuffer = Util.StringToByte(option.Charset, FileId);

            var length = Consts.FDFS_GROUP_NAME_MAX_LEN + fileIdBuffer.Length;
            List<byte> bodyBuffer = new List<byte>();
            bodyBuffer.AddRange(groupNameBuffer);
            bodyBuffer.AddRange(fileIdBuffer);

            Header = new FDFSHeader(length, Consts.STORAGE_PROTO_CMD_DELETE_FILE, 0);

            return bodyBuffer.ToArray();
        }
    }
}
