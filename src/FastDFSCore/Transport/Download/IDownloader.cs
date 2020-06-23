namespace FastDFSCore.Transport.Download
{
    public interface IDownloader
    {

        DownloaderOption Option { get; }

        void BeginWrite();

        void WriteBuffer(byte[] buffer);

        void WriteComplete();

        void Release();
    }
}
