namespace FastDFSCore.Transport
{
    public interface IConnectionManager
    {
        /// <summary>Get tracker connection
        /// </summary>
        IConnection GetTrackerConnection();

        /// <summary>Get storage connection
        /// </summary>
        IConnection GetStorageConnection(ConnectionAddress connectionAddress);

        void Release();
    }
}
