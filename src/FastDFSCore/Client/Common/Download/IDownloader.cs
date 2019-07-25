namespace FastDFSCore.Client
{
    /// <summary>下载器
    /// </summary>
    public interface IDownloader
    {
        /// <summary>文件保存路径
        /// </summary>
        string SavePath { get; }

        /// <summary>运行
        /// </summary>
        void Run();

        /// <summary>写入数据
        /// </summary>
        void WriteBuffers(byte[] buffers);

        /// <summary>写入完成
        /// </summary>
        void WriteComplete();

        /// <summary>释放
        /// </summary>
        void Release();
    }
}
