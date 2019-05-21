using DotNetty.Buffers;
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

        private TaskCompletionSource<FDFSResponse> _taskCompletionSource;
        private RequestInfo _requestInfo;
        public Connection(IServiceProvider provider, ILoggerFactory loggerFactory, FDFSOption option, ConnectionSetting setting, Action<Connection> closeAction)
        {
            _provider = provider;
            _logger = loggerFactory.CreateLogger(option.LoggerName);
            _option = option;
            _setting = setting;
            _closeAction = closeAction;

            _name = "Client_" + Guid.NewGuid().ToString();

            _creationTime = DateTime.Now;
            _isRuning = false;
        }

        /// <summary>Run
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
                        pipeline.AddLast("fdfs-read", _provider.CreateInstance<FDFSReadHandler>());

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
            if (_channel == null)
            {
                return;
            }
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
            var headerBuffer = request.Header.ToBytes();
            //请求信息
            _requestInfo = new RequestInfo(request.StreamTransfer, request.ResponseStream);

            //使用Stream传输
            if (request.StreamTransfer)
            {
                var bodyBuffer = request.EncodeBody(_option);
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
                //while (!_channel.IsWritable)
                //{
                //    Thread.Sleep(20);
                //}
                _channel.WriteAndFlushAsync(buffer).Wait();
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
        private void SetBufferResponse(IByteBuffer input)
        {
            try
            {


                if (_taskCompletionSource != null)
                {
                    _taskCompletionSource.SetResult(default(FDFSResponse));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("接收返回信息出错! {0}", ex);
            }
        }
    }


    class RequestInfo
    {

        /// <summary>是否Stream传输
        /// </summary>
        public bool StreamTransfer { get; set; }

        /// <summary>返回值是否Stream读取
        /// </summary>
        public bool ResponseStream { get; set; }

        public RequestInfo(bool streamTransfer, bool responseStream)
        {
            StreamTransfer = streamTransfer;
            ResponseStream = responseStream;
        }
    }
}
