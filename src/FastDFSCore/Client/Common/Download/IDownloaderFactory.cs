namespace FastDFSCore.Client
{
    /// <summary>下载器工厂
    /// </summary>
    public interface IDownloaderFactory
    {
        /// <summary>创建文件下载器
        /// </summary>
        /// <param name="savePath">保存路径</param>
        /// <returns></returns>
        FileDownloader CreateFileDownloader(string savePath);

        /// <summary>从DI中创建文件下载器
        /// </summary>
        T CreateDownloader<T>(params object[] args) where T : IDownloader;
    }
}
