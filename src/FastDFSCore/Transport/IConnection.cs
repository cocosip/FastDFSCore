using FastDFSCore.Protocols;
using System;
using System.Threading.Tasks;

namespace FastDFSCore.Transport
{
    public interface IConnection
    {
        #region Events

        event EventHandler<ConnectionCloseEventArgs> OnConnectionClose;

        #endregion

        string Id { get; }

        ConnectionAddress ConnectionAddress { get; }

        DateTime LastUseTime { get; }

        bool IsExpired();

        void Open();

        void Close();

        Task<FastDFSResp> SendRequestAsync<T>(FastDFSReq<T> request) where T : FastDFSResp, new();

        Task ConnectAsync();

        Task CloseAsync();
    }
}
