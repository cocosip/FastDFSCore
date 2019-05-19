using System;
using System.Collections.Generic;
using System.Text;

namespace FastDFSCore.Client
{
    public static class Util
    {
        public static byte[] GetHeaderBytes(long length, byte command, byte status)
        {
            //每个数据的头部都是 8字节长度+2字节状态
            byte[] result = new byte[Consts.FDFS_PROTO_PKG_LEN_SIZE + 2];
            Array.Copy(BitConverter.GetBytes(length), 0, result, 0, Consts.FDFS_PROTO_PKG_LEN_SIZE);
            result[8] = command;
            result[9] = status;
            return result;
        }
    }
}
