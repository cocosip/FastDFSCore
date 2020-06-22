using System;
using System.Collections.Generic;

namespace FastDFSCore.Protocols
{
    /// <summary>获取全部的Group返回信息
    /// </summary>
    public class ListAllGroupResponse : FDFSResponse
    {
        /// <summary>Group信息列表
        /// </summary>
        public List<GroupInfo> GroupInfos { get; set; } = new List<GroupInfo>();

        /// <summary>LoadContent
        /// </summary>
        public override void LoadContent(FDFSOption option, byte[] data)
        {
            if (data.Length % Consts.FDFS_GROUP_INFO_SIZE != 0)
            {
                throw new ArgumentException("返回数据长度不正确,不是'FDFS_GROUP_INFO_SIZE' 的整数倍.");
            }
            var count = data.Length / Consts.FDFS_GROUP_INFO_SIZE;
            for (int i = 0; i < count; i++)
            {
                var buffer = new byte[Consts.FDFS_GROUP_INFO_SIZE];
                Array.Copy(data, i * Consts.FDFS_GROUP_INFO_SIZE, buffer, 0, buffer.Length);
                var groupInfo = EndecodeUtil.DecodeGroupInfo(buffer, option.Charset);
                GroupInfos.Add(groupInfo);
            }
        }
    }
}
