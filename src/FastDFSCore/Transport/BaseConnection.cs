using FastDFSCore.Codecs.Messages;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FastDFSCore.Transport
{
    /// <summary>基连接
    /// </summary>
    public abstract class BaseConnection : IConnection
    {

        private int _reConnectAttempt = 0;
        private bool _isUsing = false;
        private readonly DateTime _creationTime;
        private DateTime _lastUseTime;

        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);

        /// <summary>注入
        /// </summary>
        protected IServiceProvider Provider { get; }

        /// <summary>日志
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>配置信息
        /// </summary>
        protected FDFSOption Option { get; }

        /// <summary>连接选项
        /// </summary>
        protected ConnectionAddress ConnectionAddress { get; }

        /// <summary>连接名,生成的唯一值
        /// </summary>
        public string Name { get; }

        /// <summary>是否正在被使用
        /// </summary>
        public bool IsUsing { get { return _isUsing; } }

        /// <summary>是否正在运行
        /// </summary>
        public bool IsRunning { get; protected set; }

        /// <summary>创建时间
        /// </summary>
        public DateTime CreationTime { get { return _creationTime; } }

        /// <summary>最后使用时间
        /// </summary>
        public DateTime LastUseTime { get { return _lastUseTime; } }

        /// <summary>连接关闭时的操作
        /// </summary>
        public Action<IConnection> OnConnectionClose { get; set; }


        /// <summary>Ctor
        /// </summary>
        public BaseConnection(IServiceProvider provider, ILogger<BaseConnection> logger, FDFSOption option, ConnectionAddress connectionAddress)
        {
            _creationTime = DateTime.Now;
            _lastUseTime = DateTime.Now;
            _isUsing = false;
            IsRunning = false;

            Provider = provider;
            Logger = logger;
            Option = option;


            ConnectionAddress = connectionAddress;

            Name = Guid.NewGuid().ToString();
        }

        /// <summary>运行
        /// </summary>
        public abstract Task RunAsync();

        /// <summary>结束运行
        /// </summary>
        public abstract Task ShutdownAsync();

        /// <summary>释放连接
        /// </summary>
        public abstract Task DisposeAsync();

        /// <summary>打开连接
        /// </summary>
        public Task OpenAsync()
        {
            if (!IsRunning)
            {
                AsyncHelper.RunSync(() =>
                {
                    return RunAsync();
                });
            }
            _isUsing = true;
            _lastUseTime = DateTime.Now;
            return Task.FromResult(0);
        }

        /// <summary>关闭连接
        /// </summary>
        public Task CloseAsync()
        {
            _isUsing = false;
            _lastUseTime = DateTime.Now;
            OnConnectionClose?.Invoke(this);
            return Task.FromResult(0);
        }

        /// <summary>发送数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public abstract Task<FDFSResponse> SendRequestAsync<T>(FDFSRequest<T> request) where T : FDFSResponse, new();



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
            if (!Option.TcpSetting.EnableReConnect || Option.TcpSetting.ReConnectMaxCount < _reConnectAttempt)
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
                if (_reConnectAttempt < Option.TcpSetting.ReConnectMaxCount && !reConnectSuccess)
                {
                    Thread.Sleep(Option.TcpSetting.ReConnectIntervalMilliSeconds);
                    await DoReConnectIfNeed();
                }
            }
        }

    }
}
