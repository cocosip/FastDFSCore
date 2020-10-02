using System;
using System.Security.Cryptography;
using System.Text;

namespace FastDFSCore.Utility
{
    public static class Util
    {
        /// <summary>
        /// 获取MD5
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        internal static string GetMD5(byte[] data)
        {
            using (var md5 = MD5.Create())
            {
                var buffer = md5.ComputeHash(data);
                return BitConverter.ToString(buffer).Replace("-", "");
            }
        }

        /// <summary>
        /// 生成文件的Token
        /// </summary>
        /// <param name="fileId">文件Id</param>
        /// <param name="timeStamp">当前时间戳</param>
        /// <param name="secretKey">加密密码</param>
        /// <param name="charset">编码</param>
        /// <returns></returns>
        public static string GetToken(string fileId, int timeStamp, string secretKey, string charset = "utf-8")
        {
            var fileIdBuffer = Encoding.GetEncoding(charset).GetBytes(fileId);
            var secretKeyBuffer = Encoding.GetEncoding(charset).GetBytes(secretKey);
            var timestampBuffer = Encoding.GetEncoding(charset).GetBytes(timeStamp.ToString());
            var data = ByteUtil.Combine(fileIdBuffer, secretKeyBuffer, timestampBuffer);
            return GetMD5(data).ToLower();
        }

    }
}
