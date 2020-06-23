//using FastDFSCore.Transport.Download;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using System.IO;

//namespace FastDFSCore.Sample
//{
//    public class CustomDownloader : BaseDownloader
//    {
//        private string _path;
//        private FileStream _fs;
//        public CustomDownloader(ILogger<CustomDownloader> logger, IOptions<FDFSOption> option, string path) : base(logger, option)
//        {
//            _path = path;
//        }

//        public override void BeginWrite()
//        {
//            _fs = new FileStream(_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
//        }

//        public override void WriteToFile(byte[] buffers)
//        {
//            _fs.Write(buffers, 0, buffers.Length);
//        }

//        /// <summary>释放
//        /// </summary>
//        public override void Release()
//        {
//            base.Release();
//            if (_fs != null)
//            {
//                _fs.Flush();
//                _fs.Close();
//                _fs.Dispose();
//            }
//        }

//    }
//}
