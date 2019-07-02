using DotNetty.Buffers;
using DotNetty.Common.Internal.Logging;
using DotNetty.Common.Utilities;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Streams;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FastDFSCore.Client
{
    public class Connection
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger _logger;
        private readonly FDFSOption _option;
        private readonly Action<Connection> _closeAction;

        private readonly ConnectionSetting _setting;
        private IEventLoopGroup _group;
        private IChannel _channel;

        private bool _isRuning = false;
        private bool _isUsing = false;
        private DateTime _creationTime;
        private DateTime _lastUseTime;

        public bool IsUsing { get { return _isUsing; } }
        public DateTime CreationTime { get { return _creationTime; } }
        public DateTime LastUseTime { get { return _lastUseTime; } }

        private ConnectionContext _connectionContext;
        private TaskCompletionSource<FDFSResponse> _taskCompletionSource = null;
        private IDownloader _downloader = null;
        public Connection(IServiceProvider provider, FDFSOption option, ConnectionSetting setting, Action<Connection> closeAction)
        {
            _provider = provider;
            _logger = InternalLoggerFactory.DefaultFactory.CreateLogger(option.LoggerName);
            _option = option;
            _setting = setting;
            _closeAction = closeAction;

            _creationTime = DateTime.Now;
            _lastUseTime = DateTime.Now;
            _isRuning = false;
        }

        /// <summary>运行
        /// </summary>
        public async Task RunAsync()
        {
            if (_channel != null && _channel.Registered)
            {
                _logger.LogInformation($"Client is running! Don't run again! ChannelId:{_channel.Id.AsLongText()}");
                return;
            }

            try
            {
                _group = new MultithreadEventLoopGroup();
                var bootstrap = new Bootstrap();
                bootstrap
                    .Group(_group)
                    .Channel<TcpSocketChannel>()
                    .Option(ChannelOption.TcpNodelay, _setting.TcpNodelay)
                    .Option(ChannelOption.WriteBufferHighWaterMark, _setting.WriteBufferHighWaterMark)
                    .Option(ChannelOption.WriteBufferLowWaterMark, _setting.WriteBufferLowWaterMark)
                    .Option(ChannelOption.SoRcvbuf, _setting.SoRcvbuf)
                    .Option(ChannelOption.SoSndbuf, _setting.SoSndbuf)
                    .Option(ChannelOption.SoReuseaddr, _setting.SoReuseaddr)
                    .Option(ChannelOption.AutoRead, _setting.AutoRead)
                    .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                    {
                        IChannelPipeline pipeline = channel.Pipeline;
                        pipeline.AddLast(new LoggingHandler(_option.LoggerName));
                        pipeline.AddLast("fdfs-write", new ChunkedWriteHandler<IByteBuffer>());
                        pipeline.AddLast("fdfs-decoder", new FDFSDecoder(GetContext));
                        pipeline.AddLast("fdfs-read", new FDFSReadHandler(SetResponse));

                    }));

                _channel = _setting.LocalEndPoint == null ? await bootstrap.ConnectAsync(_setting.ServerEndPoint) : await bootstrap.ConnectAsync(_setting.ServerEndPoint, _setting.LocalEndPoint);

                _isRuning = true;

                _logger.LogInformation($"Client Run! serverEndPoint:{_channel.RemoteAddress.ToStringAddress()},localAddress:{_channel.LocalAddress.ToStringAddress()}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                await _group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(_setting.QuietPeriodMilliSeconds), TimeSpan.FromSeconds(_setting.CloseTimeoutSeconds));
            }

        }

        /// <summary>关闭运行
        /// </summary>
        public async Task ShutdownAsync()
        {
            try
            {
                await _channel.CloseAsync();
            }
            finally
            {
                await _group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(_setting.QuietPeriodMilliSeconds), TimeSpan.FromSeconds(_setting.CloseTimeoutSeconds));
            }
        }

        /// <summary>发送信息
        /// </summary>
        public Task<FDFSResponse> SendRequestAsync<T>(FDFSRequest<T> request) where T : FDFSResponse, new()
        {
            _taskCompletionSource = new TaskCompletionSource<FDFSResponse>();
            //上下文,当前的信息
            _connectionContext = CreateContext<T>(request);
            //初始化保存流
            if (_connectionContext.StreamResponse && request.Downloader != null)
            {
                _downloader = request.Downloader;
                _downloader?.Run();
            }

            var bodyBuffer = request.EncodeBody(_option);
            var headerBuffer = request.Header.ToBytes();
            List<byte> newBuffer = new List<byte>();
            newBuffer.AddRange(headerBuffer);
            newBuffer.AddRange(bodyBuffer);
            //流文件发送
            if (request.StreamRequest)
            {
                _channel.WriteAsync(Unpooled.WrappedBuffer(newBuffer.ToArray()));
                var stream = new FixChunkedStream(request.RequestStream, 1024 * 32);
                _channel.WriteAndFlushAsync(stream).Wait();
            }
            else
            {
                _channel.WriteAndFlushAsync(Unpooled.WrappedBuffer(newBuffer.ToArray())).Wait();
            }
            return _taskCompletionSource.Task;
        }

        public async Task OpenAsync()
        {
            if (!_isRuning)
            {
                await RunAsync();
            }
            _isUsing = true;
            _lastUseTime = DateTime.Now;
        }

        public async Task CloseAsync()
        {
            _isUsing = false;
            _lastUseTime = DateTime.Now;
            _closeAction(this);
            await Task.FromResult(0);
        }


        #region Private method

        /// <summary>设置返回值
        /// </summary>
        private void SetResponse(ConnectionReceiveItem receiveItem)
        {
            try
            {
                //返回为Strem,需要逐步进行解析
                if (_connectionContext.StreamResponse)
                {
                    if (receiveItem.IsChunkWriting)
                    {
                        //写入流

                        //_fs.Write(receiveItem.Body, 0, receiveItem.Body.Length);
                        //写入Body
                        _downloader.WriteBuffers(receiveItem.Body);
                        _connectionContext.WritePosition += receiveItem.Body.Length;
                        if (_connectionContext.IsWriteCompleted)
                        {
                            var response = _connectionContext.Response;
                            response.SetHeader(_connectionContext.Header);
                            _taskCompletionSource.SetResult(response);
                            //完成
                            SendReceiveComplete();
                        }
                    }
                    else
                    {
                        //文件流读取,刚读取头部
                        _downloader?.Run();
                    }
                }
                else
                {
                    var response = _connectionContext.Response;
                    response.SetHeader(receiveItem.Header);
                    response.LoadContent(_option, receiveItem.Body);
                    _taskCompletionSource.SetResult(response);
                    //完成
                    SendReceiveComplete();
                }

                //释放
                ReferenceCountUtil.SafeRelease(receiveItem);

            }
            catch (Exception ex)
            {
                _logger.LogError("接收返回信息出错! {0}", ex);
                throw;
            }
        }
        private ConnectionContext CreateContext<T>(FDFSRequest<T> request) where T : FDFSResponse, new()
        {
            var context = new ConnectionContext()
            {
                Response = new T(),
                StreamRequest = request.StreamRequest,
                StreamResponse = request.StreamResponse,
                IsChunkWriting = false,
                ReadPosition = 0,
                WritePosition = 0
            };
            return context;
        }
        private ConnectionContext GetContext()
        {
            return _connectionContext;
        }

        /// <summary>一次的发送与接收完成
        /// </summary>
        private void SendReceiveComplete()
        {
            _connectionContext = null;
            _downloader?.WriteComplete();
        }


        #endregion

        public async Task DisposeAsync()
        {
            await ShutdownAsync().ConfigureAwait(false);
            _connectionContext = null;
            _downloader?.Release();
        }

    }
}
