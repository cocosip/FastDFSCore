using FastDFSCore.Client;
using System.IO;

namespace FastDFSCore.Sample
{
    public class CustomDownloader : BaseDownloader
    {
        private string _path;
        private FileStream _fs;
        public CustomDownloader(FDFSOption option, string path) : base(option)
        {
            _path = path;
        }

        public override void BeginWrite()
        {
            _fs = new FileStream(_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
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
                _fs.Flush();
                _fs.Close();
                _fs.Dispose();
            }
        }

    }
}
