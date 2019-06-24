using System.IO;

namespace FastDFSCore.Client
{
    public class FileDownloader : BaseDownloader
    {
       
        private FileStream _fs;
        public FileDownloader(FDFSOption option, string savePath) : base(option)
        {
            SavePath = savePath;
        }

        public override void BeginWrite()
        {
            _fs = new FileStream(SavePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
        }

        public override void WriteToFile(byte[] buffers)
        {
            _fs.Write(buffers, 0, buffers.Length);
        }

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
