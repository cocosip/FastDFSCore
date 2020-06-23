namespace FastDFSCore.Transport.Download
{
    public interface IDownloaderFactory
    {
        IDownloader CreateDownloader(DownloaderOption option);
    }
}
