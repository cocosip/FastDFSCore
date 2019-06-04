using System.Net;
using System.Threading.Tasks;

namespace FastDFSCore.Client
{
    public interface IConnectionManager
    {
        /// <summary>获取Tracker的连接
        /// </summary>
        Task<Connection> GetTrackerConnection();

        /// <summary>获取Storage连接
        /// </summary>
        Task<Connection> GetStorageConnection(IPEndPoint endPoint);

        /// <summary>运行
        /// </summary>
        void Start();

        /// <summary>关闭
        /// </summary>
        void Shutdown();
    }
}
