namespace FastDFSCore.Transport
{
    public interface IConnectionPool
    {
        string Name { get; }

        /// <summary>Get a connection from stack
        /// </summary>
        IConnection GetConnection();

        /// <summary>Return the connection
        /// </summary>
        void Return(IConnection connection);

        void Start();

        void Shutdown();
    }
}
