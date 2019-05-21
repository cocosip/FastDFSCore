using System;
using System.Text;

namespace FastDFSCore.Client
{
    public static class Util
    {
        /// <summary>去除扩展名开始的.
        /// </summary>
        public static string ParseExtWithOut(string fileExt)
        {
            return fileExt.TrimStart('.');
        }

        /// <summary>生成GroupName数组
        /// </summary>
        public static byte[] CreateGroupNameBuffer(Encoding encoding, string groupName)
        {
            byte[] groupBytes = new byte[Consts.FDFS_GROUP_NAME_MAX_LEN];
            byte[] bytes = encoding.GetBytes(groupName);
            Array.Copy(bytes, groupBytes, Math.Min(groupBytes.Length, bytes.Length));
            return groupBytes;
        }

        public static byte[] CreateFileExtBuffer(Encoding encoding, string fileExt)
        {
            byte[] extBuffer = new byte[Consts.FDFS_FILE_EXT_NAME_MAX_LEN];
            byte[] bse = encoding.GetBytes(fileExt);
            int ext_name_len = bse.Length;
            if (ext_name_len > Consts.FDFS_FILE_EXT_NAME_MAX_LEN)
            {
                ext_name_len = Consts.FDFS_FILE_EXT_NAME_MAX_LEN;
            }
            Array.Copy(bse, 0, extBuffer, 0, ext_name_len);
            return extBuffer;
        }

        public static byte[] CreatePrefixBuffer(Encoding encoding, string prefix)
        {
            byte[] prefixBuffer = new byte[Consts.FDFS_FILE_PREFIX_MAX_LEN];
            byte[] prefixData = encoding.GetBytes(prefix);
            Array.Copy(prefixData, 0, prefixBuffer, 0, prefixData.Length);
            return prefixBuffer;
        }


        public static byte[] StringToByte(Encoding encoding, string input)
        {
            return encoding.GetBytes(input);
        }

        public static byte[] LongToBuffer(long l)
        {
            return BitConverter.GetBytes(l);
        }
    }
}
