using System.IO;
using System.Threading.Tasks;

namespace FastDFSCore.Transport
{
    public class FileWriter : IFileWriter
    {
        private readonly FileStream _fileStream;

        public FileWriter(string filePath)
        {
            _fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
        }

        public Task WriteAsync(byte[] buffer)
        {
            return _fileStream.WriteAsync(buffer, 0, buffer.Length);
        }

        public void Wirte(byte[] buffer)
        {
            _fileStream.Write(buffer, 0, buffer.Length);
        }

        public void Dispose()
        {
            _fileStream?.Flush();
            _fileStream?.Close();
            _fileStream?.Dispose();
        }



    }
}
