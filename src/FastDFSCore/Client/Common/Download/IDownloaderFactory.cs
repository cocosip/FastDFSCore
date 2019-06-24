namespace FastDFSCore.Client
{
    public interface IDownloaderFactory
    {
        FileDownloader CreateFileDownloader(string savePath);

        T CreateDownloader<T>(params object[] args) where T : IDownloader;
    }
}
