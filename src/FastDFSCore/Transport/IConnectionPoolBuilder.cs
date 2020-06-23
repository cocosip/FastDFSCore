namespace FastDFSCore.Transport
{
    public interface IConnectionPoolBuilder
    {
        IConnectionPool CreateConnectionPool(ConnectionPoolOption option);
    }
}
