using System.Threading.Tasks;

namespace FastDFSCore.Transport
{
    public interface IFileWriter
    {
        Task WriteAsync(byte[] buffer);

        void Wirte(byte[] buffer);

        void Dispose();
    }
}
