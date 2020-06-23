using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FastDFSCore.Transport.Download
{
    public class DefaultDownloader : IDownloader
    {
        private FileStream _fileStream = null;

        public DownloaderOption Option { get; }


        public DefaultDownloader(DownloaderOption option)
        {
            Option = option;
        }

        public void BeginWrite()
        {
            if (_fileStream == null)
            {
                _fileStream = new FileStream(Option.FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            }
        }

        public void WriteBuffer(byte[] buffer)
        {
            _fileStream.Write(buffer, 0, buffer.Length);
        }

        public void WriteComplete()
        {
            throw new NotImplementedException();
        }

        public void Release()
        {
            throw new NotImplementedException();
        }
    }
}
