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

        private TransportContext _transportContext;

        private TaskCompletionSource<FastDFSResp> _taskCompletionSource = null;

        private IFileWriter _fileWriter = null;

        public DotNettyConnection(ILogger<BaseConnection> logger, IServiceProvider serviceProvider, IOptions<FastDFSOption> option, ConnectionAddress connectionAddress) : base(logger, serviceProvider, option, connectionAddress)
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

            try
            {

                _group = new MultithreadEventLoopGroup();
                _bootStrap = new Bootstrap();
                _bootStrap
                    .Group(_group)
                    .Channel<TcpSocketChannel>()
                    .Option(ChannelOption.TcpNodelay, true)
                    .Option(ChannelOption.WriteBufferHighWaterMark, 16777216)
                    .Option(ChannelOption.WriteBufferLowWaterMark, 8388608)
                    //.Option(ChannelOption.SoReuseaddr, tcpSetting.SoReuseaddr)
                    .Option(ChannelOption.AutoRead, true)
                    .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                    {
                        IChannelPipeline pipeline = channel.Pipeline;
                        pipeline.AddLast(new LoggingHandler(typeof(DotNettyConnection)));
                        pipeline.AddLast("fdfs-write", new ChunkedWriteHandler<IByteBuffer>());
                        pipeline.AddLast("fdfs-decoder", ServiceProvider.CreateInstance<FastDFSDecoder>(new Func<TransportContext>(GetContext)));
                        pipeline.AddLast("fdfs-read", ServiceProvider.CreateInstance<FastDFSReadHandler>(new Action<ReceiveData>(SetResponse)));

                        //重连
                        if (Option.EnableReConnect)
                        {
                            //Reconnect to server
                            pipeline.AddLast("reconnect", ServiceProvider.CreateInstance<ReConnectHandler>(Option, new Func<Task>(DoReConnectIfNeed)));
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


        /// <summary>发送数据
        /// </summary>
        public override Task<FastDFSResp> SendRequestAsync<T>(FastDFSReq<T> request)
        {
            _taskCompletionSource = new TaskCompletionSource<FastDFSResp>();
            //上下文,当前的信息
            _transportContext = CreateContext<T>(request);
            //初始化保存流
            if (request.IsOutputStream && !string.IsNullOrWhiteSpace(request.OutputFilePath))
            {
                _fileWriter = new FileWriter(request.OutputFilePath);
            }

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
            _channel = await _bootStrap.ConnectAsync(new IPEndPoint(IPAddress.Parse(ConnectionAddress.IPAddress), ConnectionAddress.Port));
            Logger.LogInformation($"Client DoConnect! Id:{Id},serverEndPoint:{_channel.RemoteAddress.ToStringAddress()},localAddress:{_channel.LocalAddress.ToStringAddress()}");

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
            _fileWriter?.Dispose();
            _fileWriter = null;
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
                        _fileWriter.Wirte(receiveData.Body);

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
                        //_downloader?.Run();
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
                StreamResponse = request.IsOutputStream,
                IsChunkWriting = false,
                ReadPosition = 0,
                WritePosition = 0
            };
            return context;
        }

    }
}
