using System;

namespace FastDFSCore.Client
{
    public class DefaultDownloaderFactory : IDownloaderFactory
    {
        private readonly IServiceProvider _provider;
        public DefaultDownloaderFactory(IServiceProvider provider)
        {
            _provider = provider;
        }


        public FileDownloader CreateFileDownloader(string savePath)
        {
            return _provider.CreateInstance<FileDownloader>(savePath);
        }

        public T CreateDownloader<T>(params object[] args) where T : IDownloader
        {
            return _provider.CreateInstance<T>(args);
        }
    }
}
