﻿using System;

namespace FastDFSCore.Codecs.Messages
{
    /// <summary>查询一个Group返回
    /// </summary>
    public class ListOneGroupResponse : FDFSResponse
    {
        /// <summary>Group信息
        /// </summary>
        public GroupInfo GroupInfo { get; set; }

        /// <summary>LoadContent
        /// </summary>
        public override void LoadContent(FDFSOption option, byte[] data)
        {
            if (data.Length != Consts.FDFS_GROUP_INFO_SIZE)
            {
                throw new ArgumentException($"返回数据长度:{data.Length},不是有效的GroupInfo数据长度.");
            }
            GroupInfo = EndecodeUtil.DecodeGroupInfo(data, option.Charset);
        }
    }
}
