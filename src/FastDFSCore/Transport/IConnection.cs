using FastDFSCore.Protocols;
using System;
using System.Threading.Tasks;

namespace FastDFSCore.Transport
{
    /// <summary>传输连接
    /// </summary>
    public interface IConnection
    {
        /// <summary>该连接的唯一名称
        /// </summary>
        string Name { get; }

        /// <summary>最后使用时间
        /// </summary>
        DateTime LastUseTime { get; }

        /// <summary>连接关闭的时候的操作
        /// </summary>
        Action<IConnection> OnConnectionClose { get; set; }

        /// <summary>打开连接
        /// </summary>
        Task OpenAsync();

        /// <summary>关闭连接
        /// </summary>
        Task CloseAsync();

        /// <summary>关闭连接,连接被释放
        /// </summary>
        Task ShutdownAsync();

        /// <summary>释放连接
        /// </summary>
        Task DisposeAsync();

        /// <summary>发送数据
        /// </summary>
        Task<FDFSResponse> SendRequestAsync<T>(FDFSRequest<T> request) where T : FDFSResponse, new();
    }
}
