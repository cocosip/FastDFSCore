namespace FastDFSCore.Transport
{
    public interface IConnectionBuilder
    {
        IConnection CreateConnection(ConnectionAddress connectionAddress);
    }
}
