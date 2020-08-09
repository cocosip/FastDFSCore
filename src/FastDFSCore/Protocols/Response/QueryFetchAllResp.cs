using FastDFSCore.Utility;
using System;
using System.Collections.Generic;

namespace FastDFSCore.Protocols
{
    /// <summary>查询全部的Group
    /// </summary>
    public class QueryFetchAllResp : FastDFSResp
    {
        /// <summary>组名
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>IP地址集合
        /// </summary>
        public List<string> IPAddresses { get; set; }

        /// <summary>端口号
        /// </summary>
        public int Port { get; set; }

        /// <summary>Ctor
        /// </summary>
        public QueryFetchAllResp()
        {

        }

        /// <summary>LoadContent
        /// </summary>
        public override void LoadContent(ClusterConfiguration configuration, byte[] data)
        {

            int bytesRead = 0;

            byte[] groupNameBuffer = new byte[Consts.FDFS_GROUP_NAME_MAX_LEN];
            Array.Copy(data, bytesRead, groupNameBuffer, 0, Consts.FDFS_GROUP_NAME_MAX_LEN);
            GroupName = EndecodeUtil.DecodeString(groupNameBuffer, configuration.Charset);
            //Util.ByteToString(option.Charset, groupNameBuffer).TrimEnd('\0');
            bytesRead += Consts.FDFS_GROUP_NAME_MAX_LEN;

            byte[] ipAddressBuffer = new byte[Consts.IP_ADDRESS_SIZE - 1];
            Array.Copy(data, bytesRead, ipAddressBuffer, 0, Consts.IP_ADDRESS_SIZE - 1);
            IPAddresses.Add(EndecodeUtil.DecodeString(ipAddressBuffer, configuration.Charset));
            //IPAddresses.Add(new string(option.Charset.GetChars(ipAddressBuffer)).TrimEnd('\0'));
            bytesRead += Consts.IP_ADDRESS_SIZE - 1;

            byte[] portBuffer = new byte[Consts.FDFS_PROTO_PKG_LEN_SIZE];
            Array.Copy(data, bytesRead, portBuffer, 0, Consts.FDFS_PROTO_PKG_LEN_SIZE);
            Port = (int)ByteUtil.BufferToLong(portBuffer, 0);
            bytesRead += Consts.FDFS_PROTO_PKG_LEN_SIZE;

            while (data.Length - bytesRead >= Consts.IP_ADDRESS_SIZE - 1)
            {
                ipAddressBuffer = new byte[Consts.IP_ADDRESS_SIZE - 1];
                Array.Copy(data, bytesRead, ipAddressBuffer, 0, Consts.IP_ADDRESS_SIZE - 1);
                IPAddresses.Add(EndecodeUtil.DecodeString(ipAddressBuffer, configuration.Charset));
                // IPAddresses.Add(new string(option.Charset.GetChars(ipAddressBuffer)).TrimEnd('\0'));
                bytesRead += Consts.IP_ADDRESS_SIZE - 1;
            }
        }
    }
}
