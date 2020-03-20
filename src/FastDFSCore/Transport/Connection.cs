//using DotNetty.Buffers;
//using DotNetty.Common.Utilities;
//using DotNetty.Handlers.Logging;
//using DotNetty.Handlers.Streams;
//using DotNetty.Transport.Bootstrapping;
//using DotNetty.Transport.Channels;
//using DotNetty.Transport.Channels.Sockets;
//using FastDFSCore.Codecs.Messages;
//using FastDFSCore.Extensions;
//using FastDFSCore.Transport.Download;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Threading;
//using System.Threading.Tasks;

//namespace FastDFSCore.Transport
//{
//    /// <summary>客户端连接
//    /// </summary>
//    public class Connection
//    {
//        private readonly IServiceProvider _provider;
//        private readonly ILogger _logger;
//        private readonly FDFSOption _option;
//        private readonly ConnectionAddress _connectionAddress;
//        private readonly Action<Connection> _closeAction;

//        private IEventLoopGroup _group;
//        private IChannel _channel;
//        private Bootstrap _bootStrap;

//        private int _reConnectAttempt = 0;
//        private bool _isRuning = false;
//        private bool _isUsing = false;
//        private readonly DateTime _creationTime;
//        private DateTime _lastUseTime;

//        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);

//        /// <summary>是否正在使用
//        /// </summary>
//        public bool IsUsing { get { return _isUsing; } }

//        /// <summary>创建时间
//        /// </summary>
//        public DateTime CreationTime { get { return _creationTime; } }

//        /// <summary>最后使用时间
//        /// </summary>
//        public DateTime LastUseTime { get { return _lastUseTime; } }

//        /// <summary>名称
//        /// </summary>
//        public string Name { get; private set; }

//        private ConnectionContext _connectionContext;
//        private TaskCompletionSource<FDFSResponse> _taskCompletionSource = null;
//        private IDownloader _downloader = null;

//        /// <summary>Ctor
//        /// </summary>
//        /// <param name="provider">provider</param>
//        /// <param name="logger">logger</param>
//        /// <param name="option">FDFSOption</param>
//        /// <param name="connectionAddress"></param>
//        /// <param name="closeAction"></param>
//        public Connection(IServiceProvider provider, ILogger<Connection> logger, FDFSOption option, ConnectionAddress connectionAddress, Action<Connection> closeAction)
//        {
//            _provider = provider;
//            _logger = logger;
//            _option = option;
//            _connectionAddress = connectionAddress;
//            _closeAction = closeAction;

//            _creationTime = DateTime.Now;
//            _lastUseTime = DateTime.Now;
//            _isRuning = false;
//            Name = Guid.NewGuid().ToString();
//        }

//        /// <summary>运行
//        /// </summary>
//        public async Task RunAsync()
//        {
//            if (_channel != null && _channel.Registered)
//            {
//                _logger.LogInformation($"Client is running! Don't run again! ChannelId:{_channel.Id.AsLongText()}");
//                return;
//            }
//            var tcpSetting = _option.TcpSetting;
//            try
//            {

//                _group = new MultithreadEventLoopGroup();
//                _bootStrap = new Bootstrap();
//                _bootStrap
//                    .Group(_group)
//                    .Channel<TcpSocketChannel>()
//                    .Option(ChannelOption.TcpNodelay, tcpSetting.TcpNodelay)
//                    .Option(ChannelOption.WriteBufferHighWaterMark, tcpSetting.WriteBufferHighWaterMark)
//                    .Option(ChannelOption.WriteBufferLowWaterMark, tcpSetting.WriteBufferLowWaterMark)
//                    .Option(ChannelOption.SoRcvbuf, tcpSetting.SoRcvbuf)
//                    .Option(ChannelOption.SoSndbuf, tcpSetting.SoSndbuf)
//                    .Option(ChannelOption.SoReuseaddr, tcpSetting.SoReuseaddr)
//                    .Option(ChannelOption.AutoRead, true)
//                    .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
//                    {
//                        IChannelPipeline pipeline = channel.Pipeline;
//                        pipeline.AddLast(new LoggingHandler(_option.LoggerName));
//                        pipeline.AddLast("fdfs-write", new ChunkedWriteHandler<IByteBuffer>());
//                        pipeline.AddLast("fdfs-decoder", new FDFSDecoder(GetContext));
//                        pipeline.AddLast("fdfs-read", new FDFSReadHandler(SetResponse));

//                        //重连
//                        if (_option.TcpSetting.EnableReConnect)
//                        {
//                            //Reconnect to server
//                            pipeline.AddLast("reconnect", _provider.CreateInstance<ReConnectHandler>(_option, new Func<Task>(DoReConnectIfNeed)));
//                        }

//                    }));

//                await DoConnect();

//                _isRuning = true;
//                _logger.LogInformation($"Client Run! serverEndPoint:{_channel.RemoteAddress.ToStringAddress()},localAddress:{_channel.LocalAddress.ToStringAddress()}");
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex.Message);
//                await _group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(tcpSetting.QuietPeriodMilliSeconds), TimeSpan.FromSeconds(tcpSetting.CloseTimeoutSeconds));
//            }

//        }

//        /// <summary>关闭运行
//        /// </summary>
//        public async Task ShutdownAsync()
//        {
//            try
//            {
//                await _channel.CloseAsync();
//                _isRuning = false;
//            }
//            finally
//            {
//                await _group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(_option.TcpSetting.QuietPeriodMilliSeconds), TimeSpan.FromSeconds(_option.TcpSetting.CloseTimeoutSeconds));
//            }
//        }

//        /// <summary>发送信息
//        /// </summary>
//        public Task<FDFSResponse> SendRequestAsync<T>(FDFSRequest<T> request) where T : FDFSResponse, new()
//        {
//            _taskCompletionSource = new TaskCompletionSource<FDFSResponse>();
//            //上下文,当前的信息
//            _connectionContext = CreateContext<T>(request);
//            //初始化保存流
//            if (_connectionContext.StreamResponse && request.Downloader != null)
//            {
//                _downloader = request.Downloader;
//                _downloader?.Run();
//            }

//            var bodyBuffer = request.EncodeBody(_option);
//            if (request.Header.Length == 0)
//            {
//                request.Header.Length = request.StreamRequest ? request.RequestStream.Length + bodyBuffer.Length : bodyBuffer.Length;
//            }

//            var headerBuffer = request.Header.ToBytes();
//            List<byte> newBuffer = new List<byte>();
//            newBuffer.AddRange(headerBuffer);
//            newBuffer.AddRange(bodyBuffer);

//            //流文件发送
//            if (request.StreamRequest)
//            {
//                _channel.WriteAsync(Unpooled.WrappedBuffer(newBuffer.ToArray()));
//                var stream = new FixChunkedStream(request.RequestStream);
//                _channel.WriteAndFlushAsync(stream);
//            }
//            else
//            {
//                _channel.WriteAndFlushAsync(Unpooled.WrappedBuffer(newBuffer.ToArray()));
//            }
//            return _taskCompletionSource.Task;
//        }

//        /// <summary>打开连接
//        /// </summary>
//        public async Task OpenAsync()
//        {
//            if (!_isRuning)
//            {
//                await RunAsync();
//            }
//            _isUsing = true;
//            _lastUseTime = DateTime.Now;
//        }

//        /// <summary>关闭连接
//        /// </summary>
//        public async Task CloseAsync()
//        {
//            _isUsing = false;
//            _lastUseTime = DateTime.Now;
//            _closeAction(this);
//            await Task.FromResult(0);
//        }


//        #region Private method

//        /// <summary>连接到服务器端
//        /// </summary>
//        private async Task DoConnect()
//        {
//            _channel = _connectionAddress.LocalEndPoint == null ? await _bootStrap.ConnectAsync(_connectionAddress.ServerEndPoint) : await _bootStrap.ConnectAsync(_connectionAddress.ServerEndPoint, _connectionAddress.LocalEndPoint);

//            _logger.LogInformation($"Client DoConnect! name:{Name},serverEndPoint:{_channel.RemoteAddress.ToStringAddress()},localAddress:{_channel.LocalAddress.ToStringAddress()}");
//        }

//        /// <summary>重连机制
//        /// </summary>
//        private async Task DoReConnectIfNeed()
//        {
//            if (!_option.TcpSetting.EnableReConnect || _option.TcpSetting.ReConnectMaxCount < _reConnectAttempt)
//            {
//                return;
//            }
//            if (_channel != null && !_channel.Active)
//            {
//                await _semaphoreSlim.WaitAsync();
//                bool reConnectSuccess = false;
//                try
//                {
//                    _logger.LogInformation($"Try to reconnect server!");
//                    await DoConnect();
//                    Interlocked.Exchange(ref _reConnectAttempt, 0);
//                    reConnectSuccess = true;
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError("ReConnect fail!{0}", ex.Message);
//                }
//                finally
//                {
//                    Interlocked.Increment(ref _reConnectAttempt);
//                    _semaphoreSlim.Release();
//                }
//                //Try again!
//                if (_reConnectAttempt < _option.TcpSetting.ReConnectMaxCount && !reConnectSuccess)
//                {
//                    Thread.Sleep(_option.TcpSetting.ReConnectIntervalMilliSeconds);
//                    await DoReConnectIfNeed();
//                }
//            }
//        }

//        /// <summary>设置返回值
//        /// </summary>
//        private void SetResponse(ConnectionReceiveItem receiveItem)
//        {
//            try
//            {
//                //返回为Strem,需要逐步进行解析
//                if (_connectionContext.StreamResponse)
//                {
//                    if (receiveItem.IsChunkWriting)
//                    {
//                        //写入流

//                        //_fs.Write(receiveItem.Body, 0, receiveItem.Body.Length);
//                        //写入Body
//                        _downloader.WriteBuffers(receiveItem.Body);
//                        _connectionContext.WritePosition += receiveItem.Body.Length;
//                        if (_connectionContext.IsWriteCompleted)
//                        {
//                            var response = _connectionContext.Response;
//                            response.SetHeader(_connectionContext.Header);
//                            _taskCompletionSource.SetResult(response);
//                            //完成
//                            SendReceiveComplete();
//                        }
//                    }
//                    else
//                    {
//                        //文件流读取,刚读取头部
//                        _downloader?.Run();
//                    }
//                }
//                else
//                {
//                    var response = _connectionContext.Response;
//                    response.SetHeader(receiveItem.Header);
//                    response.LoadContent(_option, receiveItem.Body);
//                    _taskCompletionSource.SetResult(response);
//                    //完成
//                    SendReceiveComplete();
//                }

//                //释放
//                ReferenceCountUtil.SafeRelease(receiveItem);

//            }
//            catch (Exception ex)
//            {
//                _logger.LogError("接收返回信息出错! {0}", ex);
//                throw;
//            }
//        }
//        private ConnectionContext CreateContext<T>(FDFSRequest<T> request) where T : FDFSResponse, new()
//        {
//            var context = new ConnectionContext()
//            {
//                Response = new T(),
//                StreamRequest = request.StreamRequest,
//                StreamResponse = request.StreamResponse,
//                IsChunkWriting = false,
//                ReadPosition = 0,
//                WritePosition = 0
//            };
//            return context;
//        }
//        private ConnectionContext GetContext()
//        {
//            return _connectionContext;
//        }

//        /// <summary>一次的发送与接收完成
//        /// </summary>
//        private void SendReceiveComplete()
//        {
//            _connectionContext = null;
//            _downloader?.WriteComplete();
//        }


//        #endregion

//        /// <summary>释放连接
//        /// </summary>
//        public async Task DisposeAsync()
//        {
//            await ShutdownAsync();
//            _connectionContext = null;
//            _downloader?.Release();
//        }

//    }
//}
