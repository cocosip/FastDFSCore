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

        public static string ByteToString(Encoding encoding, byte[] input)
        {
            char[] chars = encoding.GetChars(input);
            string result = new string(chars, 0, chars.Length);
            return result;
        }

        public static string ByteToString(Encoding encoding, byte[] input, int startIndex, int count)
        {
            char[] chars = encoding.GetChars(input, startIndex, count);
            string result = new string(chars, 0, chars.Length);
            return result;
        }


        public static byte[] LongToBuffer(long l)
        {
            byte[] buffer = new byte[8];
            buffer[0] = (byte)((l >> 56) & 0xFF);
            buffer[1] = (byte)((l >> 48) & 0xFF);
            buffer[2] = (byte)((l >> 40) & 0xFF);
            buffer[3] = (byte)((l >> 32) & 0xFF);
            buffer[4] = (byte)((l >> 24) & 0xFF);
            buffer[5] = (byte)((l >> 16) & 0xFF);
            buffer[6] = (byte)((l >> 8) & 0xFF);
            buffer[7] = (byte)(l & 0xFF);
            return buffer;
        }

        public static long BufferToLong(byte[] buffer, int offset = 0)
        {
#pragma warning disable CS0675 
            return (((long)(buffer[offset] >= 0 ? buffer[offset] : 256 + buffer[offset])) << 56) |
                  (((long)(buffer[offset + 1] >= 0 ? buffer[offset + 1] : 256 + buffer[offset + 1])) << 48) |
                  (((long)(buffer[offset + 2] >= 0 ? buffer[offset + 2] : 256 + buffer[offset + 2])) << 40) |
                  (((long)(buffer[offset + 3] >= 0 ? buffer[offset + 3] : 256 + buffer[offset + 3])) << 32) |
                  (((long)(buffer[offset + 4] >= 0 ? buffer[offset + 4] : 256 + buffer[offset + 4])) << 24) |
                  (((long)(buffer[offset + 5] >= 0 ? buffer[offset + 5] : 256 + buffer[offset + 5])) << 16) |
                  (((long)(buffer[offset + 6] >= 0 ? buffer[offset + 6] : 256 + buffer[offset + 6])) << 8) |
                  ((buffer[offset + 7] >= 0 ? buffer[offset + 7] : 256 + buffer[offset + 7]));
#pragma warning restore CS0675 

        }

    }
}
