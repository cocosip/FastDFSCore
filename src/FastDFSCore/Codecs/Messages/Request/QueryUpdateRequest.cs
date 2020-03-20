﻿using FastDFSCore.Utility;
using System.Collections.Generic;

namespace FastDFSCore.Codecs.Messages
{
    /// <summary>
    ///查询更新文件时的Storage
    ///Reqeust 
    ///    Cmd: TRACKER_PROTO_CMD_SERVICE_QUERY_UPDATE 103
    ///    Body:
    ///    @ FDFS_GROUP_NAME_MAX_LEN bytes:  group name
    ///    @ filename bytes: filename
    ///Response
    ///    Cmd: TRACKER_PROTO_CMD_RESP
    ///    Status: 0 right other wrong
    ///    Body: 
    ///    @ FDFS_GROUP_NAME_MAX_LEN bytes: group name
    ///    @ IP_ADDRESS_SIZE - 1 bytes: storage server ip address
    ///    @ FDFS_PROTO_PKG_LEN_SIZE bytes: storage server port
    /// </summary>
    public class QueryUpdateRequest : FDFSRequest<QueryUpdateResponse>
    {
        /// <summary>组名
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>文件FileId
        /// </summary>
        public string FileId { get; set; }

        /// <summary>Ctor
        /// </summary>
        public QueryUpdateRequest()
        {
            Header = new FDFSHeader(Consts.TRACKER_PROTO_CMD_SERVICE_QUERY_UPDATE);
        }

        /// <summary>Ctor
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="fileId">文件FileId</param>
        public QueryUpdateRequest(string groupName, string fileId) : this()
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
