using FastDFSCore.Protocols;
using System;
using System.Threading.Tasks;

namespace FastDFSCore.Transport
{
    public interface IConnection
    {
        #region Events

        event EventHandler<ConnectionCloseEventArgs> OnConnectionClose;

        event EventHandler<DisconnectEventArgs> OnDisconnect;

        #endregion

        string Id { get; }

        ConnectionAddress ConnectionAddress { get; }

        bool IsUsing { get; }
        
        DateTime LastUseTime { get; }

        bool IsExpired();

        ValueTask OpenAsync();

        ValueTask CloseAsync();

        ValueTask<FastDFSResp> SendRequestAsync<T>(FastDFSReq<T> request) where T : FastDFSResp, new();

        ValueTask ConnectAsync();

        ValueTask DisconnectAsync();
    }
}
