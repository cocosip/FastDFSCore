using FastDFSCore.Protocols;
using System;
using System.Threading.Tasks;

namespace FastDFSCore.Transport
{
    public interface IConnection
    {
        string Name { get; }

        ConnectionAddress ConnectionAddress { get; }

        DateTime LastUseTime { get; }

        bool IsExpired();

        Action<IConnection> OnConnectionClose { get; set; }

        Task OpenAsync();

        Task CloseAsync();

        Task ShutdownAsync();

        Task DisposeAsync();

        Task<FastDFSResp> SendRequestAsync<T>(FastDFSReq<T> request) where T : FastDFSResp, new();
    }
}
