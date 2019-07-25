using System;

namespace FastDFSCore.Client
{
    /// <summary>下载器工厂
    /// </summary>
    public class DefaultDownloaderFactory : IDownloaderFactory
    {
        private readonly IServiceProvider _provider;

        /// <summary>Ctor
        /// </summary>
        public DefaultDownloaderFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        /// <summary>创建文件下载器
        /// </summary>
        /// <param name="savePath">保存路径</param>
        /// <returns></returns>
        public FileDownloader CreateFileDownloader(string savePath)
        {
            return _provider.CreateInstance<FileDownloader>(savePath);
        }

        /// <summary>从DI中创建文件下载器
        /// </summary>
        public T CreateDownloader<T>(params object[] args) where T : IDownloader
        {
            return _provider.CreateInstance<T>(args);
        }
    }
}
