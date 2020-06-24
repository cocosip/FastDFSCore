using FastDFSCore.Protocols;
using System;
using System.Threading.Tasks;

namespace FastDFSCore.Transport
{
    public interface IConnection
    {
        string Id { get; }

        ConnectionAddress ConnectionAddress { get; }

        event EventHandler<ConnectionCloseEventArgs> OnConnectionClose;

        DateTime LastUseTime { get; }

        bool IsExpired();

        Task OpenAsync();

        Task CloseAsync();

        Task ShutdownAsync();

        Task DisposeAsync();

        Task<FastDFSResp> SendRequestAsync<T>(FastDFSReq<T> request) where T : FastDFSResp, new();
    }
}
