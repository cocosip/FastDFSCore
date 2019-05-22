using DotNetty.Buffers;
using DotNetty.Common.Internal.Logging;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Streams;
using DotNetty.Handlers.Tls;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using DotNetty.Transport.Libuv;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace FastDFSCore.Client
{
    public class Connection
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger _logger;
        private readonly FDFSOption _option;
        private readonly string _name;
        private readonly Action<Connection> _closeAction;

        private readonly ConnectionSetting _setting;
        private IEventLoopGroup _group;
        private IChannel _channel;

        private bool _isRuning = false;
        private bool _isUsing = false;
        private DateTime _creationTime;
        private DateTime _lastUseTime;

        public string Name { get { return _name; } }
        public bool IsUsing { get { return _isUsing; } }
        public DateTime CreationTime { get { return _creationTime; } }
        public DateTime LastUseTime { get { return _lastUseTime; } }

        private ConnectionContext _connectionContext;
        private TaskCompletionSource<FDFSResponse> _taskCompletionSource = null;
        private FileStream _fs = null;
        public Connection(IServiceProvider provider, FDFSOption option, ConnectionSetting setting, Action<Connection> closeAction)
        {
            _provider = provider;
            _logger = InternalLoggerFactory.DefaultFactory.CreateLogger(option.LoggerName);
            _option = option;
            _setting = setting;
            _closeAction = closeAction;

            _name = "Client_" + Guid.NewGuid().ToString();

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
            if (_setting.UseLibuv)
            {
                _group = new EventLoopGroup();
            }
            else
            {
                _group = new MultithreadEventLoopGroup();
            }

            try
            {
                var bootstrap = new Bootstrap();
                bootstrap.Group(_group);
                if (_setting.UseLibuv)
                {
                    bootstrap.Channel<TcpChannel>();
                }
                else
                {
                    bootstrap.Channel<TcpSocketChannel>();
                }

                bootstrap
                    .Option(ChannelOption.TcpNodelay, _setting.TcpNodelay)
                    .Option(ChannelOption.WriteBufferHighWaterMark, _setting.WriteBufferHighWaterMark)
                    .Option(ChannelOption.WriteBufferLowWaterMark, _setting.WriteBufferHighWaterMark)
                    .Option(ChannelOption.SoRcvbuf, _setting.SoRcvbuf)
                    .Option(ChannelOption.SoSndbuf, _setting.SoSndbuf)
                    .Option(ChannelOption.SoReuseaddr, _setting.SoReuseaddr)
                    .Option(ChannelOption.AutoRead, _setting.AutoRead)
                    .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                    {
                        IChannelPipeline pipeline = channel.Pipeline;

                        if (_setting.IsSsl && _setting.TlsCertificate != null)
                        {
                            var targetHost = _setting.TlsCertificate.GetNameInfo(X509NameType.DnsName, false);

                            pipeline.AddLast("tls", new TlsHandler(stream => new SslStream(stream, true, (sender, certificate, chain, errors) => true), new ClientTlsSettings(targetHost)));
                        }
                        pipeline.AddLast(new LoggingHandler(_option.LoggerName));
                        pipeline.AddLast("fdfs-write", _provider.CreateInstance<FDFSWriteHandler>());

                        Func<ConnectionContext> getContextAction = GetContext;
                        pipeline.AddLast("fdfs-decoder", _provider.CreateInstance<FDFSDecoder>(getContextAction));

                        Action setResultAction = SetResponse;

                        pipeline.AddLast("fdfs-read", _provider.CreateInstance<FDFSReadHandler>(setResultAction));

                    }));

                _channel = _setting.LocalEndPoint == null ? await bootstrap.ConnectAsync(_setting.ServerEndPoint) : await bootstrap.ConnectAsync(_setting.ServerEndPoint, _setting.LocalEndPoint);

                _logger.LogInformation($"Client Run! name:{Name},serverEndPoint:{_channel.RemoteAddress.ToStringAddress()},localAddress:{_channel.LocalAddress.ToStringAddress()}");
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
        public Task<T> SendRequestAsync<T>(FDFSRequest<T> request) where T : FDFSResponse, new()
        {
            _taskCompletionSource = new TaskCompletionSource<FDFSResponse>();
            _connectionContext = new ConnectionContext()
            {
                Response = new T(),
                IsFileUpload = request.IsFileUpload,
                IsFileDownload = request.IsFileDownload,
                FileDownloadPath = request.FileDownloadPath
            };

            var bodyBuffer = request.EncodeBody(_option);
            var headerBuffer = request.Header.ToBytes();
            //文件上传
            if (request.IsFileUpload)
            {

                //var bodyBuffer = request.EncodeBody(_option);
                var newBuffer = new byte[headerBuffer.Length + bodyBuffer.Length];
                Array.Copy(headerBuffer, 0, newBuffer, 0, headerBuffer.Length);
                Array.Copy(bodyBuffer, 0, newBuffer, headerBuffer.Length, bodyBuffer.Length);
                var newStream = new ChunkedStream(new MemoryStream(bodyBuffer));
                var stream = new ChunkedStream(request.Stream);
                _channel.WriteAsync(newStream);
                _channel.WriteAndFlushAsync(stream);
            }
            else
            {
                var buffer = Unpooled.Buffer();
                buffer.WriteBytes(headerBuffer);
                buffer.WriteBytes(request.EncodeBody(_option));
                _channel.WriteAndFlushAsync(buffer).Wait();
            }

            //_fs初始化
            if (_connectionContext.IsFileDownload && _connectionContext.IsDownloadToPath)
            {
                _fs = new FileStream(_connectionContext.FileDownloadPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            }

            return _taskCompletionSource.Task as Task<T>;
        }


        public void Open()
        {
            if (!_isRuning)
            {
                RunAsync().Wait();
            }
            _lastUseTime = DateTime.Now;
        }

        public void Close()
        {
            _isUsing = false;
            _lastUseTime = DateTime.Now;
            _closeAction(this);
        }

        //Buffer数组返回
        private void SetResponse()
        {
            try
            {
                if (_connectionContext.IsFileDownload)
                {
                    //还未下载,正在读取头
                    if (!_connectionContext.IsChunkDownload)
                    {
                        _connectionContext.IsChunkDownload = true;
                    }
                    else
                    {
                        //写入流
                        _fs.Write(_connectionContext.Body, 0, _connectionContext.Body.Length);
                        if (_connectionContext.IsComplete)
                        {
                            var response = _connectionContext.Response;
                            response.SetHeader(_connectionContext.Header);
                            _taskCompletionSource.SetResult(response);
                            //完成
                            SendReceiveComplete();
                        }
                    }
                }
                else
                {
                    var response = _connectionContext.Response;
                    response.SetHeader(_connectionContext.Header);
                    response.LoadContent(_option, _connectionContext.Body);
                    _taskCompletionSource.SetResult(response);
                    //完成
                    SendReceiveComplete();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("接收返回信息出错! {0}", ex);
            }
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
            _taskCompletionSource = null;
            if (_fs != null)
            {
                _fs.Dispose();
            }
        }

    }
}
