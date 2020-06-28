using FastDFSCore.Protocols;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FastDFSCore.Transport
{
    /// <summary>基连接
    /// </summary>
    public abstract class BaseConnection : IConnection
    {
        #region Events

        public event EventHandler<ConnectionCloseEventArgs> OnConnectionClose;

        #endregion



        private int _reConnectAttempt = 0;
        private bool _isUsing = false;
        private readonly DateTime _creationTime;
        private DateTime _lastUseTime;

        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);

        protected ILogger Logger { get; }

        protected FastDFSOption Option { get; }

        protected IServiceProvider ServiceProvider { get; }

        public string Id { get; }

        public ConnectionAddress ConnectionAddress { get; }

        public bool IsUsing { get { return _isUsing; } }

        public bool IsRunning { get; protected set; }

        public DateTime CreationTime { get { return _creationTime; } }

        public DateTime LastUseTime { get { return _lastUseTime; } }

        public BaseConnection(ILogger<BaseConnection> logger, IServiceProvider serviceProvider, IOptions<FastDFSOption> option, ConnectionAddress connectionAddress)
        {
            _creationTime = DateTime.Now;
            _lastUseTime = DateTime.Now;
            _isUsing = false;
            IsRunning = false;


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

        /// <summary>打开连接
        /// </summary>
        public virtual void Open()
        {
            if (!IsRunning)
            {
                RunAsync().Wait();
            }
            _isUsing = true;
            _lastUseTime = DateTime.Now;
        }


        /// <summary>关闭连接
        /// </summary>
        public virtual void Close()
        {
            _isUsing = false;
            _lastUseTime = DateTime.Now;

            OnConnectionClose?.Invoke(this, new ConnectionCloseEventArgs()
            {
                Id = Id,
                ConnectionAddress = ConnectionAddress
            });
        }

        //public abstract Task ConnectAsync();

        //public abstract Task CloseAsync();


        /// <summary>运行
        /// </summary>
        public abstract Task RunAsync();

        /// <summary>结束运行
        /// </summary>
        public abstract Task ShutdownAsync();



        /// <summary>发送数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public abstract Task<FastDFSResp> SendRequestAsync<T>(FastDFSReq<T> request) where T : FastDFSResp, new();

        /// <summary>当前连接是否可用,可以发送数据
        /// </summary>
        protected abstract bool IsAvailable();

        /// <summary>连接到服务器端的操作
        /// </summary>
        protected abstract Task DoConnect();

        /// <summary>重连机制
        /// </summary>
        protected async Task DoReConnectIfNeed()
        {
            if (!Option.EnableReConnect || Option.ReConnectMaxCount < _reConnectAttempt)
            {
                return;
            }
            if (IsAvailable())
            {
                await _semaphoreSlim.WaitAsync();
                bool reConnectSuccess = false;
                try
                {
                    Logger.LogInformation($"Try to reconnect server!");
                    await DoConnect();
                    Interlocked.Exchange(ref _reConnectAttempt, 0);
                    reConnectSuccess = true;
                }
                catch (Exception ex)
                {
                    Logger.LogError("ReConnect fail!{0}", ex.Message);
                }
                finally
                {
                    Interlocked.Increment(ref _reConnectAttempt);
                    _semaphoreSlim.Release();
                }
                //Try again!
                if (_reConnectAttempt < Option.ReConnectMaxCount && !reConnectSuccess)
                {
                    Thread.Sleep(Option.ReConnectIntervalMilliSeconds);
                    await DoReConnectIfNeed();
                }
            }
        }


    }
}
