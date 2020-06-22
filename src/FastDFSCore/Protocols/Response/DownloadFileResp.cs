﻿namespace FastDFSCore.Protocols
{
    /// <summary>下载文件返回值
    /// </summary>
    public class DownloadFileResp : FastDFSResp
    {
        /// <summary>文件二进制
        /// </summary>
        public byte[] ContentBytes { get; set; }

        /// <summary>Ctor
        /// </summary>
        public DownloadFileResp()
        {

        }

        /// <summary>LoadContent
        /// </summary>
        public override void LoadContent(FDFSOption option, byte[] data)
        {
            ContentBytes = data;
        }
    }
}
