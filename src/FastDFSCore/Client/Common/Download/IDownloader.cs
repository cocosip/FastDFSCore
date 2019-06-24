namespace FastDFSCore.Client
{
    public interface IDownloader
    {
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

        void Release();
    }
}
