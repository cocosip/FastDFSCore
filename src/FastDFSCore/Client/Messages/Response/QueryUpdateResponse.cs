using System;

namespace FastDFSCore.Client
{
    public class QueryUpdateResponse : FDFSResponse
    {

        public string GroupName { get; set; }
        public string IPAddress { get; set; }
        public int Port { get; set; }

        public QueryUpdateResponse()
        {

        }

        public override void LoadContent(FDFSOption option, byte[] data)
        {
            byte[] groupNameBuffer = new byte[Consts.FDFS_GROUP_NAME_MAX_LEN];
            Array.Copy(data, groupNameBuffer, Consts.FDFS_GROUP_NAME_MAX_LEN);
            GroupName = Util.ByteToString(option.Charset, groupNameBuffer).TrimEnd('\0');

            byte[] ipAddressBuffer = new byte[Consts.IP_ADDRESS_SIZE - 1];
            Array.Copy(data, Consts.FDFS_GROUP_NAME_MAX_LEN, ipAddressBuffer, 0, Consts.IP_ADDRESS_SIZE - 1);
            IPAddress = new string(option.Charset.GetChars(ipAddressBuffer)).TrimEnd('\0');

            byte[] portBuffer = new byte[Consts.FDFS_PROTO_PKG_LEN_SIZE];
            Array.Copy(data, Consts.FDFS_GROUP_NAME_MAX_LEN + Consts.IP_ADDRESS_SIZE - 1,
                portBuffer, 0, Consts.FDFS_PROTO_PKG_LEN_SIZE);
            Port = (int)Util.BufferToLong(portBuffer, 0);
        }


    }
}
