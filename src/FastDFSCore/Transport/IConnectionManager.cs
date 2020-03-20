using System.Net;
using System.Threading.Tasks;

namespace FastDFSCore.Transport
{
    /// <summary>连接管理器
    /// </summary>
    public interface IConnectionManager
    {
        /// <summary>获取Tracker的连接
        /// </summary>
        Task<IConnection> GetTrackerConnection();

        /// <summary>获取Storage连接
        /// </summary>
        Task<IConnection> GetStorageConnection(IPEndPoint endPoint);

        /// <summary>运行
        /// </summary>
        void Start();

        /// <summary>关闭
        /// </summary>
        void Shutdown();
    }
}
