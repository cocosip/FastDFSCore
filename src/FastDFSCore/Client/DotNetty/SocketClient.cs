using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Tls;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using DotNetty.Transport.Libuv;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace FastDFSCore.Client
{
    public class SocketClient
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger _logger;
        private readonly IScheduleService _scheduleService;
        private readonly FDFSOption _option;
        private readonly ClientSetting _setting;
        public string Name { get; }

        /// <summary>服务器端EndPoint
        /// </summary>
        public IPEndPoint ServerEndPoint { get; }
        public string LocalIPAddress
        {
            get
            {
                return _clientChannel?.LocalAddress.ToStringAddress() ?? "";
            }
        }
        private IEventLoopGroup _group;
        private IChannel _clientChannel;
        //private readonly ConcurrentDictionary<string, ResponseFuture> _responseFutureDict;

        public SocketClient(IServiceProvider provider, ILoggerFactory loggerFactory, FDFSOption option, IScheduleService scheduleService, ClientSetting setting)
        {
            _provider = provider;
            _option = option;
            _logger = loggerFactory.CreateLogger(option.LoggerName);
            _scheduleService = scheduleService;
            _setting = setting;

            Name = "SocketClient-" + Guid.NewGuid().ToString();
            ServerEndPoint = _setting.ServerEndPoint;
            //_pushMessageHandlerDict = new Dictionary<int, IPushMessageHandler>();
            //_responseFutureDict = new ConcurrentDictionary<string, ResponseFuture>();
        }

        /// <summary>Run
        /// </summary>
        public async Task RunAsync()
        {
            if (_clientChannel != null && _clientChannel.Registered)
            {
                _logger.LogInformation($"Client is running! Don't run again! ChannelId:{_clientChannel.Id.AsLongText()}");
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

                        //_setting.PipelineConfigure?.Invoke(pipeline);

                    }));

                _clientChannel = _setting.LocalEndPoint == null ? await bootstrap.ConnectAsync(_setting.ServerEndPoint) : await bootstrap.ConnectAsync(_setting.ServerEndPoint, _setting.LocalEndPoint);

                _logger.LogInformation($"Client Run! name:{Name},serverEndPoint:{_clientChannel.RemoteAddress.ToStringAddress()},localAddress:{_clientChannel.LocalAddress.ToStringAddress()}");
                //Scan timeout request
                StartScanTimeoutRequestTask();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                StopScanTimeoutRequestTask();
                await _group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(_setting.QuietPeriodMilliSeconds), TimeSpan.FromSeconds(_setting.CloseTimeoutSeconds));
            }

        }

        /// <summary>Close
        /// </summary>
        public async Task CloseAsync()
        {
            if (_clientChannel == null)
            {
                return;
            }
            try
            {
                await _clientChannel.CloseAsync();
                StopScanTimeoutRequestTask();
            }
            finally
            {
                await _group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(_setting.QuietPeriodMilliSeconds), TimeSpan.FromSeconds(_setting.CloseTimeoutSeconds));
            }
        }


        ///// <summary>Send message, return ResponseMessage
        ///// </summary>
        //public Task<ResponseMessage> SendAsync(RequestMessage request, int timeoutMillis)
        //{
        //    CheckChannel(_clientChannel);
        //    if (_clientChannel.IsWritable)
        //    {
        //        var taskCompletionSource = new TaskCompletionSource<ResponseMessage>();
        //        var responseFuture = new ResponseFuture(request, timeoutMillis, taskCompletionSource);

        //        if (!_responseFutureDict.TryAdd(request.Id, responseFuture))
        //        {
        //            throw new Exception($"Add remoting request response future failed. request id:{request.Id}");
        //        }
        //        _clientChannel.WriteAndFlushAsync(request).Wait();
        //        return taskCompletionSource.Task;
        //    }
        //    else
        //    {
        //        _logger.LogInformation("Channel is not writable!");
        //        Thread.Sleep(1000);
        //        return Task.FromResult(ResponseMessage.BuildExceptionResponse(request, "Channel is not writable"));
        //    }
        //}




        private void CheckChannel(IChannel channel)
        {
            if (channel == null)
            {
                throw new ArgumentException("Channel is null.");
            }
            if (!channel.Open || !channel.Active)
            {
                throw new Exception($"Current channel is not useable,channelId:{channel.Id.AsShortText()}");
            }
        }


        private void StartScanTimeoutRequestTask()
        {
            _scheduleService.StartTask($"{Name}.{GetType().Name}.ScanTimeoutRequest", ScanTimeoutRequest, 1000, _setting.ScanTimeoutRequestInterval);
        }

        private void StopScanTimeoutRequestTask()
        {
            _scheduleService.StopTask($"{Name}.{GetType().Name}.ScanTimeoutRequest");
        }

        private void ScanTimeoutRequest()
        {
            //var timeoutKeyList = new List<string>();
            //foreach (var entry in _responseFutureDict)
            //{
            //    if (entry.Value.IsTimeout())
            //    {
            //        timeoutKeyList.Add(entry.Key);
            //    }
            //}
            //foreach (var key in timeoutKeyList)
            //{
            //    if (_responseFutureDict.TryRemove(key, out ResponseFuture responseFuture))
            //    {
            //        var request = responseFuture.Request;
            //        responseFuture.SetResponse(ResponseMessage.BuildExceptionResponse(request, "Remoting request timeout."));
            //        _logger.LogInformation($"Removed timeout request, name: {Name}, requestId: {responseFuture.Request.Id}");
            //    }
            //}
        }

        //private void HandleResponseMessage(ResponseMessage responseMessage)
        //{
        //    if (_responseFutureDict.TryRemove(responseMessage.RequestId, out ResponseFuture responseFuture))
        //    {
        //        if (responseFuture.SetResponse(responseMessage))
        //        {
        //            _logger.LogDebug($"Remoting response back, name: {Name}, request code: {responseFuture.Request.Code}, requect id: {responseFuture.Request.Id}, time spent: {(DateTime.Now - responseFuture.BeginTime).TotalMilliseconds},beginTime:{responseFuture.BeginTime.ToString("yyyy-MM-dd HH:mm:ss fff")}");
        //        }
        //        else
        //        {
        //            _logger.LogDebug($"Set remoting response failed, name: {Name}, responseId: {responseMessage.Id}");
        //        }
        //    }
        //}

    }
}
