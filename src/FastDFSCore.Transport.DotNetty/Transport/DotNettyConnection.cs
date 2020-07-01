using DotNetty.Buffers;
using DotNetty.Common.Utilities;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Streams;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using FastDFSCore.Protocols;
using FastDFSCore.Transport.DotNetty;
using FastDFSCore.Utility;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Threading.Tasks;

namespace FastDFSCore.Transport
{
    /// <summary>DotNetty连接
    /// </summary>
    public class DotNettyConnection : BaseConnection
    {
        private IEventLoopGroup _group;
        private IChannel _channel;
        private Bootstrap _bootStrap;

        private readonly DotNettyOption _dotNettyOption;
        public override event EventHandler<DisconnectEventArgs> OnDisconnect;

        private TransportContext _context = null;
        private TaskCompletionSource<FastDFSResp> _tcs = null;
        private FastDFSResp _resp = null;

        public DotNettyConnection(ILogger<BaseConnection> logger, IServiceProvider serviceProvider, IOptions<FastDFSOption> option, IOptions<DotNettyOption> dotNettyOption, ConnectionAddress connectionAddress) : base(logger, serviceProvider, option, connectionAddress)
        {
            _dotNettyOption = dotNettyOption.Value;
        }


        /// <summary>发送数据
        /// </summary>
        public override Task<FastDFSResp> SendRequestAsync<T>(FastDFSReq<T> request)
        {
            _tcs = new TaskCompletionSource<FastDFSResp>();

            _resp = new T();

            //上下文,当前的信息
            _context = BuildContext<T>(request);

            var bodyBuffer = request.EncodeBody(Option);
            if (request.Header.Length == 0)
            {
                request.Header.Length = request.InputStream != null ? request.InputStream.Length + bodyBuffer.Length : bodyBuffer.Length;
            }

            var headerBuffer = request.Header.ToBytes();
            var newBuffer = ByteUtil.Combine(headerBuffer, bodyBuffer);

            //流文件发送
            if (request.InputStream != null)
            {
                _channel.WriteAsync(Unpooled.WrappedBuffer(newBuffer));
                var stream = new FixChunkedStream(request.InputStream);
                _channel.WriteAndFlushAsync(stream);
            }
            else
            {
                _channel.WriteAndFlushAsync(Unpooled.WrappedBuffer(newBuffer));
            }
            return _tcs.Task;
        }


        /// <summary>运行
        /// </summary>
        public override async Task ConnectAsync()
        {
            if (_channel != null && _channel.Registered)
            {
                Logger.LogInformation($"Client is running! Don't run again! ChannelId:{_channel.Id.AsLongText()}");
                return;
            }

            try
            {
                _group = new MultithreadEventLoopGroup();
                _bootStrap = new Bootstrap();
                _bootStrap
                    .Group(_group)
                    .Channel<TcpSocketChannel>()
                    .Option(ChannelOption.TcpNodelay, _dotNettyOption.TcpNodelay)
                    .Option(ChannelOption.WriteBufferHighWaterMark, _dotNettyOption.WriteBufferHighWaterMark)
                    .Option(ChannelOption.WriteBufferLowWaterMark, _dotNettyOption.WriteBufferLowWaterMark)
                    .Option(ChannelOption.SoReuseaddr, _dotNettyOption.SoReuseaddr)
                    .Option(ChannelOption.AutoRead, true)
                    .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                    {
                        IChannelPipeline pipeline = channel.Pipeline;

                        Func<TransportContext> getCtx = () => _context;

                        pipeline.AddLast(new LoggingHandler(typeof(DotNettyConnection)));
                        pipeline.AddLast("fdfs-write", new ChunkedWriteHandler<IByteBuffer>());
                        pipeline.AddLast("fdfs-decoder", ServiceProvider.CreateInstance<FastDFSDecoder>(getCtx));
                        pipeline.AddLast("fdfs-handler", ServiceProvider.CreateInstance<FastDFSHandler>(new Action<ReceivedPackage>(HandleReceivedPack)));

                    }));

                _channel = await _bootStrap.ConnectAsync(new IPEndPoint(IPAddress.Parse(ConnectionAddress.IPAddress), ConnectionAddress.Port));

                IsConnected = true;
                Logger.LogInformation($"Client connect! serverEndPoint:{_channel.RemoteAddress.ToStringAddress()},localAddress:{_channel.LocalAddress.ToStringAddress()}");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                IsConnected = false;
                await _group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
            }
        }


        /// <summary>关闭连接
        /// </summary>
        public override async Task DisconnectAsync()
        {
            try
            {
                await _channel.CloseAsync();
                IsConnected = false;

                OnDisconnect?.Invoke(this, new DisconnectEventArgs()
                {
                    Id = Id,
                    ConnectionAddress = ConnectionAddress
                });

            }
            finally
            {
                await _group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
            }
        }


        /// <summary>设置返回值
        /// </summary>
        private void HandleReceivedPack(ReceivedPackage package)
        {
            try
            {
                //返回为Strem,需要逐步进行解析

                _resp.Header = new FastDFSHeader(package.Length, package.Command, package.Status);

                if (!_context.IsOutputStream)
                {
                    _resp.LoadContent(Option, package.Body);
                }

                _context = null;
                _tcs.SetResult(_resp);

                //释放
                ReferenceCountUtil.SafeRelease(package);

            }
            catch (Exception ex)
            {
                Logger.LogError("接收返回信息出错! {0}", ex);
                throw;
            }
        }

    }
}
