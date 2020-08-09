using FastDFSCore.Protocols;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace FastDFSCore.Transport
{
    /// <summary>基连接
    /// </summary>
    public abstract class BaseConnection : IConnection
    {
        #region Events

        public virtual event EventHandler<ConnectionCloseEventArgs> OnConnectionClose;
        public abstract event EventHandler<DisconnectEventArgs> OnDisconnect;

        #endregion


        protected ILogger Logger { get; }

        protected ClusterConfiguration Configuration { get; }

        protected IServiceProvider ServiceProvider { get; }

        public string Id { get; }

        public ConnectionAddress ConnectionAddress { get; }

        public bool IsUsing { get; private set; } = false;

        public bool IsConnected { get; protected set; }

        public DateTime CreationTime { get; private set; }

        public DateTime LastUseTime { get; private set; }

        public BaseConnection(ILogger<BaseConnection> logger, IServiceProvider serviceProvider, ClusterConfiguration configuration, ConnectionAddress connectionAddress)
        {
            CreationTime = DateTime.Now;
            LastUseTime = DateTime.Now;
            IsUsing = false;
            IsConnected = false;


            Logger = logger;
            Configuration = configuration;
            ServiceProvider = serviceProvider;

            ConnectionAddress = connectionAddress;

            Id = Guid.NewGuid().ToString();
        }

        public virtual bool IsExpired()
        {
            return (DateTime.Now - LastUseTime).TotalSeconds > Configuration.ConnectionLifeTime;
        }

        public virtual async ValueTask OpenAsync()
        {
            if (!IsConnected)
            {
                await ConnectAsync();
            }
            IsUsing = true;
            LastUseTime = DateTime.Now;
        }

        public virtual ValueTask CloseAsync()
        {
            IsUsing = false;
            LastUseTime = DateTime.Now;

            OnConnectionClose?.Invoke(this, new ConnectionCloseEventArgs()
            {
                Id = Id,
                ConnectionAddress = ConnectionAddress
            });
            return new ValueTask();
        }

        protected virtual TransportContext BuildContext<T>(FastDFSReq<T> request) where T : FastDFSResp, new()
        {
            var context = new TransportContext()
            {
                ReqType = request.GetType(),
                RespType = typeof(T),
                IsInputStream = request.InputStream != null,
                IsOutputStream = request.IsOutputStream,
                OutputFilePath = request.OutputFilePath
            };
            return context;
        }

        /// <summary>发送数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public abstract ValueTask<FastDFSResp> SendRequestAsync<T>(FastDFSReq<T> request) where T : FastDFSResp, new();

        public abstract ValueTask ConnectAsync();

        public abstract ValueTask DisconnectAsync();

    }
}
