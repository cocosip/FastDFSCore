﻿using FastDFSCore.Protocols;
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

        private bool _isUsing = false;
        private readonly DateTime _creationTime;
        private DateTime _lastUseTime;

        protected ILogger Logger { get; }

        protected FastDFSOption Option { get; }

        protected IServiceProvider ServiceProvider { get; }

        public string Id { get; }

        public ConnectionAddress ConnectionAddress { get; }

        public bool IsUsing { get { return _isUsing; } }

        public bool IsConnected { get; protected set; }

        public DateTime CreationTime { get { return _creationTime; } }

        public DateTime LastUseTime { get { return _lastUseTime; } }

        public BaseConnection(ILogger<BaseConnection> logger, IServiceProvider serviceProvider, IOptions<FastDFSOption> option, ConnectionAddress connectionAddress)
        {
            _creationTime = DateTime.Now;
            _lastUseTime = DateTime.Now;
            _isUsing = false;
            IsConnected = false;


            Logger = logger;
            Option = option.Value;
            ServiceProvider = serviceProvider;

            ConnectionAddress = connectionAddress;

            Id = Guid.NewGuid().ToString();
        }

        public virtual bool IsExpired()
        {
            return (DateTime.Now - _lastUseTime).TotalSeconds > Option.ConnectionLifeTime;
        }

        public virtual async ValueTask OpenAsync()
        {
            if (!IsConnected)
            {
                await ConnectAsync();
            }
            _isUsing = true;
            _lastUseTime = DateTime.Now;
        }

        public virtual ValueTask CloseAsync()
        {
            _isUsing = false;
            _lastUseTime = DateTime.Now;

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
        public abstract Task<FastDFSResp> SendRequestAsync<T>(FastDFSReq<T> request) where T : FastDFSResp, new();

        public abstract Task ConnectAsync();

        public abstract Task DisconnectAsync();

    }
}
