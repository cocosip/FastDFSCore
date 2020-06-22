using DotNetty.Buffers;
using DotNetty.Common.Utilities;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Streams;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using FastDFSCore.Protocols;
using FastDFSCore.Transport.DotNetty;
using FastDFSCore.Transport.Download;
using FastDFSCore.Utility;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
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

        private TransportContext _transportContext;
        private TaskCompletionSource<FastDFSResp> _taskCompletionSource = null;
        private IDownloader _downloader = null;

        /// <summary>Ctor
        /// </summary>
        public DotNettyConnection(IFastDFSCoreHost host, ILogger<BaseConnection> logger, IOptions<FDFSOption> option, ConnectionAddress connectionAddress) : base(host, logger, option, connectionAddress)
        {

        }

        /// <summary>运行
        /// </summary>
        public override async Task RunAsync()
        {
            if (_channel != null && _channel.Registered)
            {
                Logger.LogInformation($"Client is running! Don't run again! ChannelId:{_channel.Id.AsLongText()}");
                return;
            }
            var tcpSetting = Option.TcpSetting;
            try
            {

                _group = new MultithreadEventLoopGroup();
                _bootStrap = new Bootstrap();
                _bootStrap
                    .Group(_group)
                    .Channel<TcpSocketChannel>()
                    .Option(ChannelOption.TcpNodelay, tcpSetting.TcpNodelay)
                    .Option(ChannelOption.WriteBufferHighWaterMark, 16777216)
                    .Option(ChannelOption.WriteBufferLowWaterMark, 8388608)
                    .Option(ChannelOption.SoReuseaddr, tcpSetting.SoReuseaddr)
                    .Option(ChannelOption.AutoRead, true)
                    .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                    {
                        IChannelPipeline pipeline = channel.Pipeline;
                        pipeline.AddLast(new LoggingHandler(typeof(DotNettyConnection)));
                        pipeline.AddLast("fdfs-write", new ChunkedWriteHandler<IByteBuffer>());
                        pipeline.AddLast("fdfs-decoder", Host.ServiceProvider.CreateInstance<FDFSDecoder>(new Func<TransportContext>(GetContext)));
                        pipeline.AddLast("fdfs-read", Host.ServiceProvider.CreateInstance<FDFSReadHandler>(new Action<ReceiveData>(SetResponse)));

                        //重连
                        if (Option.TcpSetting.EnableReConnect)
                        {
                            //Reconnect to server
                            pipeline.AddLast("reconnect", Host.ServiceProvider.CreateInstance<ReConnectHandler>(Option, new Func<Task>(DoReConnectIfNeed)));
                        }

                    }));

                await DoConnect();

                IsRunning = true;
                Logger.LogInformation($"Client Run! serverEndPoint:{_channel.RemoteAddress.ToStringAddress()},localAddress:{_channel.LocalAddress.ToStringAddress()}");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                await _group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
            }
        }


        /// <summary>关闭连接
        /// </summary>
        public override async Task ShutdownAsync()
        {
            try
            {
                await _channel.CloseAsync();
                IsRunning = false;
            }
            finally
            {
                await _group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
            }
        }

        /// <summary>释放连接
        /// </summary>
        public override async Task DisposeAsync()
        {
            await ShutdownAsync();
            _transportContext = null;
            _downloader?.Release();
        }

        /// <summary>发送数据
        /// </summary>
        public override Task<FastDFSResp> SendRequestAsync<T>(FastDFSReq<T> request)
        {
            _taskCompletionSource = new TaskCompletionSource<FastDFSResp>();
            //上下文,当前的信息
            _transportContext = CreateContext<T>(request);
            //初始化保存流
            if (_transportContext.StreamResponse && request.Downloader != null)
            {
                _downloader = request.Downloader;
                _downloader?.Run();
            }

            var bodyBuffer = request.EncodeBody(Option);
            if (request.Header.Length == 0)
            {
                request.Header.Length = request.InputStream != null ? request.InputStream.Length + bodyBuffer.Length : bodyBuffer.Length;
            }

            var headerBuffer = request.Header.ToBytes();
            List<byte> newBuffer = new List<byte>();
            newBuffer.AddRange(headerBuffer);
            newBuffer.AddRange(bodyBuffer);

            //流文件发送
            if (request.InputStream != null)
            {
                _channel.WriteAsync(Unpooled.WrappedBuffer(newBuffer.ToArray()));
                var stream = new FixChunkedStream(request.InputStream);
                _channel.WriteAndFlushAsync(stream);
            }
            else
            {
                _channel.WriteAndFlushAsync(Unpooled.WrappedBuffer(newBuffer.ToArray()));
            }
            return _taskCompletionSource.Task;
        }

        /// <summary>是否可用,可发送
        /// </summary>
        protected override bool IsAvailable()
        {
            return _channel != null && !_channel.Active;
        }

        /// <summary>连接操作
        /// </summary>
        protected override async Task DoConnect()
        {
            _channel = ConnectionAddress.LocalEndPoint == null ? await _bootStrap.ConnectAsync(ConnectionAddress.ServerEndPoint) : await _bootStrap.ConnectAsync(ConnectionAddress.ServerEndPoint, ConnectionAddress.LocalEndPoint);

            Logger.LogInformation($"Client DoConnect! name:{Name},serverEndPoint:{_channel.RemoteAddress.ToStringAddress()},localAddress:{_channel.LocalAddress.ToStringAddress()}");
        }


        private TransportContext GetContext()
        {
            return _transportContext;
        }


        /// <summary>一次的发送与接收完成
        /// </summary>
        private void SendReceiveComplete()
        {
            _transportContext = null;
            _downloader?.WriteComplete();
        }


        /// <summary>设置返回值
        /// </summary>
        private void SetResponse(ReceiveData receiveData)
        {
            try
            {
                //返回为Strem,需要逐步进行解析
                if (_transportContext.StreamResponse)
                {
                    if (receiveData.IsChunkWriting)
                    {
                        //写入流

                        //_fs.Write(receiveItem.Body, 0, receiveItem.Body.Length);
                        //写入Body
                        _downloader.WriteBuffers(receiveData.Body);
                        _transportContext.WritePosition += receiveData.Body.Length;
                        if (_transportContext.IsWriteCompleted)
                        {
                            var response = _transportContext.Response;
                            response.SetHeader(_transportContext.Header);
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
                    var response = _transportContext.Response;
                    response.SetHeader(receiveData.Header);
                    response.LoadContent(Option, receiveData.Body);
                    _taskCompletionSource.SetResult(response);
                    //完成
                    SendReceiveComplete();
                }

                //释放
                ReferenceCountUtil.SafeRelease(receiveData);

            }
            catch (Exception ex)
            {
                Logger.LogError("接收返回信息出错! {0}", ex);
                throw;
            }
        }

        private TransportContext CreateContext<T>(FastDFSReq<T> request) where T : FastDFSResp, new()
        {
            var context = new TransportContext()
            {
                Response = new T(),
                StreamRequest = request.InputStream != null,
                StreamResponse = request.StreamResponse,
                IsChunkWriting = false,
                ReadPosition = 0,
                WritePosition = 0
            };
            return context;
        }

    }
}
