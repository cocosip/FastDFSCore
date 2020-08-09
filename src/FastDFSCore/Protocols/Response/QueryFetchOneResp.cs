using FastDFSCore.Utility;
using System;

namespace FastDFSCore.Protocols
{
    /// <summary>查询一个可用的Storage返回
    /// </summary>
    public class QueryFetchOneResp : FastDFSResp
    {

        /// <summary>组名
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>IP地址
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>端口号
        /// </summary>
        public int Port { get; set; }

        /// <summary>Ctor
        /// </summary>
        public QueryFetchOneResp()
        {

        }

        /// <summary>Ctor
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="iPAddress">IP地址</param>
        /// <param name="port">端口号</param>
        public QueryFetchOneResp(string groupName, string iPAddress, int port)
        {
            GroupName = groupName;
            IPAddress = iPAddress;
            Port = port;
        }

        /// <summary>LoadContent
        /// </summary>
        public override void LoadContent(ClusterConfiguration configuration, byte[] data)
        {
            byte[] groupNameBuffer = new byte[Consts.FDFS_GROUP_NAME_MAX_LEN];
            Array.Copy(data, groupNameBuffer, Consts.FDFS_GROUP_NAME_MAX_LEN);
            GroupName = EndecodeUtil.DecodeString(groupNameBuffer);

            byte[] ipAddressBuffer = new byte[Consts.IP_ADDRESS_SIZE - 1];
            Array.Copy(data, Consts.FDFS_GROUP_NAME_MAX_LEN, ipAddressBuffer, 0, Consts.IP_ADDRESS_SIZE - 1);
            IPAddress = EndecodeUtil.DecodeString(ipAddressBuffer);
            //new string(option.Charset.GetChars(ipAddressBuffer)).TrimEnd('\0');

            byte[] portBuffer = new byte[Consts.FDFS_PROTO_PKG_LEN_SIZE];
            Array.Copy(data, Consts.FDFS_GROUP_NAME_MAX_LEN + Consts.IP_ADDRESS_SIZE - 1,
                portBuffer, 0, Consts.FDFS_PROTO_PKG_LEN_SIZE);
            
            Port = (int)ByteUtil.BufferToLong(portBuffer, 0);
        }


    }
}
