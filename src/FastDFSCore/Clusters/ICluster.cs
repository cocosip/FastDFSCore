using FastDFSCore.Transport;

namespace FastDFSCore
{
    public interface ICluster
    {
        /// <summary>Get tracker connection
        /// </summary>
        IConnection GetTrackerConnection();

        /// <summary>获取Storage连接
        /// </summary>
        IConnection GetStorageConnection(ConnectionAddress connectionAddress);

        /// <summary>关闭
        /// </summary>
        void Release();

    }
}
