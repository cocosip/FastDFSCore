using FastDFSCore.Transport;

namespace FastDFSCore
{
    public interface ICluster
    {
        /// <summary>
        /// Cluster name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Get tracker connection
        /// </summary>
        IConnection GetTrackerConnection();

        /// <summary>
        /// Get storage connection
        /// </summary>
        IConnection GetStorageConnection(ConnectionAddress connectionAddress);

        /// <summary>
        /// Release 
        /// </summary>
        void Release();

    }
}
