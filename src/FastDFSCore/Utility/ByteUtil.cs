using System;
using System.Linq;
using System.Text;

namespace FastDFSCore.Utility
{
    /// <summary>二进制数据相关操作
    /// </summary>
    public static class ByteUtil
    {
        /// <summary>字符串转二进制
        /// </summary>
        /// <param name="source">字符串</param>
        /// <param name="charset">编码</param>
        /// <returns></returns>
        public static byte[] StringToByte(string source, string charset = "utf-8")
        {
            return Encoding.GetEncoding(charset).GetBytes(source);
        }

        /// <summary>二进制转String
        /// </summary>
        /// <param name="input">二进制数组</param>
        /// <param name="charset">编码</param>
        /// <returns></returns>
        public static string ByteToString(byte[] input, string charset = "utf-8")
        {
            char[] chars = Encoding.GetEncoding(charset).GetChars(input);
            string result = new string(chars, 0, chars.Length);
            return result;
        }


        /// <summary>二进制转字符串
        /// </summary>
        /// <param name="input">二进制</param>
        /// <param name="startIndex">开始位置</param>
        /// <param name="count">转换的数量</param>
        /// <param name="charset">编码</param>
        /// <returns></returns>
        public static string ByteToString(byte[] input, int startIndex, int count, string charset = "utf-8")
        {
            char[] chars = Encoding.GetEncoding(charset).GetChars(input, startIndex, count);
            string result = new string(chars, 0, chars.Length);
            return result;
        }

        /// <summary>二进制转String
        /// </summary>
        public static string ByteToString(Span<byte> span, string charset = "utf-8")
        {
            return ByteToString(span.ToArray(), charset);
        }

        /// <summary>二进制转字符串
        /// </summary>
        /// <param name="encoding">编码</param>
        /// <param name="input">二进制</param>
        /// <param name="startIndex">开始位置</param>
        /// <param name="count">转换的数量</param>
        /// <returns></returns>
        public static string ByteToString(Encoding encoding, byte[] input, int startIndex, int count)
        {
            char[] chars = encoding.GetChars(input, startIndex, count);
            string result = new string(chars, 0, chars.Length);
            return result;
        }

        /// <summary>long类型转二进制
        /// </summary>
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

        /// <summary>二进制转换为long
        /// </summary>
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

        /// <summary>合并byte数组
        /// </summary>
        public static byte[] Combine(params byte[][] arrays)
        {
            var destination = new byte[arrays.Sum(x => x.Length)];
            var offset = 0;
            foreach (var data in arrays)
            {
                Buffer.BlockCopy(data, 0, destination, offset, data.Length);
                offset += data.Length;
            }
            return destination;
        }
    }
}
