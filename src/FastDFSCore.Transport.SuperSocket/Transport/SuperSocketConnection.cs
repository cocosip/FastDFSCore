using FastDFSCore.Protocols;
using FastDFSCore.Transport.SuperSocket;
using FastDFSCore.Utility;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SuperSocket.Client;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FastDFSCore.Transport
{
    public class SuperSocketConnection : BaseConnection
    {
        private IEasyClient<ReceivedPackage> _client = null;
        private TaskCompletionSource<FastDFSResp> _taskCompletionSource = null;
        private TransportContext _context = null;
        private CancellationTokenSource _cancellationTokenSource = null;

        public SuperSocketConnection(ILogger<BaseConnection> logger, IServiceProvider serviceProvider, IOptions<FastDFSOption> option, ConnectionAddress connectionAddress) : base(logger, serviceProvider, option, connectionAddress)
        {

        }



        public override async Task ConnectAsync()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            if (_client == null)
            {
                _client = new EasyClient<ReceivedPackage>(new ReceivedPackageFilter(GetContext));
            }

            var connectSuccess = await _client.ConnectAsync(new IPEndPoint(IPAddress.Parse(ConnectionAddress.IPAddress), ConnectionAddress.Port));
            if (connectSuccess)
            {
                DoReceive();
            }
        }

        public override async Task CloseAsync()
        {
            await _client.CloseAsync();
            _cancellationTokenSource.Cancel();
        }

        private TransportContext GetContext()
        {
            return _context;
        }


        public override Task<FastDFSResp> SendRequestAsync<T>(FastDFSReq<T> request)
        {
            _taskCompletionSource = new TaskCompletionSource<FastDFSResp>();
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
                _client.SendAsync(newBuffer);

                using (request.InputStream)
                {
                    //设置缓冲区大小
                    byte[] buffers = new byte[1024 * 1024];
                    //读取一次
                    int r = request.InputStream.Read(buffers, 0, buffers.Length);
                    //判断本次是否读取到了数据
                    while (r > 0)
                    {
                        _client.SendAsync(buffers).AsTask().Wait();
                        r = request.InputStream.Read(buffers, 0, buffers.Length);
                    }
                }

            }
            else
            {
                _client.SendAsync(newBuffer).AsTask().Wait();
            }
            return _taskCompletionSource.Task;
        }

        private void DoReceive()
        {
            Task.Run(async () =>
            {
                while (!_cancellationTokenSource.IsCancellationRequested && IsRunning)
                {
                    var package = await _client.ReceiveAsync();

                    try
                    {
                        //返回为Strem,需要逐步进行解析
                        var response = _context.Response;
                        response.Header = new FastDFSHeader(package.Length, package.Command, package.Status);

                        if (!_context.IsOutputStream)
                        {
                            response.LoadContent(Option, package.Body);
                        }

                        _taskCompletionSource.SetResult(response);

                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("接收返回信息出错! {0}", ex);
                        throw;
                    }

                    if (package == null) // connection dropped
                        break;
                }

            }, _cancellationTokenSource.Token);

        }

    }
}
