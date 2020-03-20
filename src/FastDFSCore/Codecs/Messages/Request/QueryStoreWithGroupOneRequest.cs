﻿namespace FastDFSCore.Codecs.Messages
{
    /// <summary>
    /// 查询可存储的Storage
    /// 
    /// Reqeust 
    ///     Cmd: TRACKER_PROTO_CMD_SERVICE_QUERY_STORE_WITH_GROUP_ONE 104
    ///     Body: 
    ///     @ FDFS_GROUP_NAME_MAX_LEN bytes: group name
    /// Response
    ///     Cmd: TRACKER_PROTO_CMD_RESP
    ///     Status: 0 right other wrong
    ///     Body: 
    ///     @ FDFS_GROUP_NAME_MAX_LEN bytes: group name
    ///     @ IP_ADDRESS_SIZE - 1 bytes: storage server ip address
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: storage server port
    ///     @ 1 byte: store path index on the storage server
    /// </summary>
    public class QueryStoreWithGroupOneRequest : FDFSRequest<QueryStoreWithGroupResponse>
    {
        /// <summary>组名
        /// </summary>
        public string GropName { get; set; }

        /// <summary>Ctor
        /// </summary>
        public QueryStoreWithGroupOneRequest()
        {
            Header = new FDFSHeader(Consts.TRACKER_PROTO_CMD_SERVICE_QUERY_STORE_WITH_GROUP_ONE);
        }

        /// <summary>Ctor
        /// </summary>
        /// <param name="groupName">组名</param>
        public QueryStoreWithGroupOneRequest(string groupName) : this()
        {
            GropName = groupName;
        }

        /// <summary>EncodeBody
        /// </summary>
        public override byte[] EncodeBody(FDFSOption option)
        {
            //消息体长度为group name的最大长度,16

           var bodyBuffer = EndecodeUtil.EncodeGroupName(GropName, option.Charset);
            //Header = new FDFSHeader(Consts.FDFS_GROUP_NAME_MAX_LEN, Consts.TRACKER_PROTO_CMD_SERVICE_QUERY_STORE_WITH_GROUP_ONE, 0);
            return bodyBuffer;
        }
    }
}
