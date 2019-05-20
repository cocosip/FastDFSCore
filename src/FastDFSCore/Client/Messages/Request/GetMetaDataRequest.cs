using System;

namespace FastDFSCore.Client
{
    /// <summary>
    /// 获取文件Meta
    /// 
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
    public class GetMetaDataRequest : FDFSRequest<GetMetaDataResponse>
    {
        public string GroupName { get; set; }

        /// <summary>文件Id
        /// </summary>
        public string FileId { get; set; }


        public GetMetaDataRequest()
        {

        }

        public GetMetaDataRequest(string groupName, string fileId)
        {
            GroupName = groupName;
            FileId = fileId;
        }

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

            Header = new FDFSHeader(length, Consts.STORAGE_PROTO_CMD_GET_METADATA, 0);
            return bodyBuffer;
        }
    }
}
