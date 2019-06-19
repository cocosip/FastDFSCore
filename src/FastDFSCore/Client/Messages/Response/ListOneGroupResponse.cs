using System;

namespace FastDFSCore.Client
{
    public class ListOneGroupResponse : FDFSResponse
    {
        public GroupInfo GroupInfo { get; set; }


        public override void LoadContent(FDFSOption option, byte[] data)
        {
            if (data.Length != Consts.FDFS_GROUP_INFO_SIZE)
            {
                throw new ArgumentException($"返回数据长度:{data.Length},不是有效的GroupInfo数据长度.");
            }
            GroupInfo = Util.LoadGroupInfo(option.Charset, data);
        }
    }
}
