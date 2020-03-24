﻿using Microsoft.Extensions.Logging;
using System.IO;

namespace FastDFSCore.Transport.Download
{
    /// <summary>文件下载器
    /// </summary>
    public class FileDownloader : BaseDownloader
    {

        private FileStream _fs;

        /// <summary>Ctor
        /// </summary>
        public FileDownloader(ILogger<BaseDownloader> logger, FDFSOption option, string savePath) : base(logger, option)
        {
            Path = savePath;
        }

        /// <summary>开始写入文件
        /// </summary>
        public override void BeginWrite()
        {
            _fs = new FileStream(Path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
        }

        /// <summary>将二进制写入文件
        /// </summary>
        public override void WriteToFile(byte[] buffers)
        {
            _fs.Write(buffers, 0, buffers.Length);
        }

        /// <summary>释放文件
        /// </summary>
        public override void Release()
        {
            base.Release();
            if (_fs != null)
            {
                //释放之前先刷盘
                _fs.Flush();
                _fs.Close();
                _fs.Dispose();
            }
        }
    }
}
