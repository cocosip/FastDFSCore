using System;

namespace FastDFSCore.Client
{
    /// <summary>根据Group名称查询Store
    /// </summary>
    public class QueryStoreWithGroupRequest : FDFSRequest<QueryStoreWithGroupResponse>
    {
        /// <summary>组名称
        /// </summary>
        public string GropName { get; set; }

        public QueryStoreWithGroupRequest()
        {

        }

        public QueryStoreWithGroupRequest(string groupName)
        {
            GropName = groupName;
        }


        public override void SetHeader(long length, byte command, byte status)
        {
            Header = new FDFSHeader(length, Consts.TRACKER_PROTO_CMD_SERVICE_QUERY_STORE_WITH_GROUP_ONE, 0);
        }


        public override byte[] EncodeBody(FDFSOption option)
        {
            //消息体长度为group name的最大长度,16
            var body = new byte[Consts.FDFS_GROUP_NAME_MAX_LEN];
            var groupNameBuffer = option.Charset.GetBytes(GropName);
            Array.Copy(groupNameBuffer, 0, body, 0, groupNameBuffer.Length);
            return body;
        }
    }
}
