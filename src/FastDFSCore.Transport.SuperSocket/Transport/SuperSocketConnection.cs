using FastDFSCore.Protocols;
using FastDFSCore.Transport.SuperSocket;
using FastDFSCore.Utility;
using Microsoft.Extensions.Logging;
using SuperSocket.Channel;
using SuperSocket.Client;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FastDFSCore.Transport
{
    public class SuperSocketConnection : BaseConnection
    {
        private IEasyClient<ReceivedPackage> _client = null;
        private TaskCompletionSource<FastDFSResp> _tcs = null;
        private TransportContext _context = null;
        private CancellationTokenSource _cts = null;

        private FileStream _fileStream = null;
        private bool _hasWriteFile = false;
        private FastDFSResp _resp = null;

        public override event EventHandler<DisconnectEventArgs> OnDisconnect;
        public SuperSocketConnection(ILogger<BaseConnection> logger, IServiceProvider serviceProvider, ClusterConfiguration configuration, ConnectionAddress connectionAddress) : base(logger, serviceProvider, configuration, connectionAddress)
        {

        }

        public override async ValueTask ConnectAsync()
        {
            _cts = new CancellationTokenSource();
            if (_client == null)
            {
                Func<TransportContext> getCtx = () => _context;
                _client = new EasyClient<ReceivedPackage>(ServiceProvider.CreateInstance<ReceivedPackageFilter>(getCtx), new ChannelOptions()
                {
                    MaxPackageLength = 1024 * 1024 * 1024
                });
            }

            _client.Closed += async (o, e) =>
            {
                await DisconnectAsync();
            };


            if (await _client.ConnectAsync(new IPEndPoint(IPAddress.Parse(ConnectionAddress.IPAddress), ConnectionAddress.Port)))
            {
                IsConnected = true;
                DoReceive();
            }
            else
            {
                await DisconnectAsync();
            }
        }

        public override async ValueTask DisconnectAsync()
        {
            try
            {
                _cts.Cancel();
                await _client.CloseAsync();
                OnDisconnect?.Invoke(this, new DisconnectEventArgs()
                {
                    Id = Id,
                    ConnectionAddress = ConnectionAddress
                });
            }
            finally
            {
                IsConnected = false;
            }
        }

        public override async ValueTask<FastDFSResp> SendRequestAsync<T>(FastDFSReq<T> request)
        {
            _tcs = new TaskCompletionSource<FastDFSResp>();

            _resp = new T();

            //上下文,当前的信息
            _context = BuildContext<T>(request);

            var bodyBuffer = request.EncodeBody(Configuration);
            if (request.Header.Length == 0)
            {
                request.Header.Length = request.InputStream != null ? request.InputStream.Length + bodyBuffer.Length : bodyBuffer.Length;
            }

            var headerBuffer = request.Header.ToBytes();
            var newBuffer = ByteUtil.Combine(headerBuffer, bodyBuffer);

            //流文件发送
            if (request.InputStream != null)
            {
                await _client.SendAsync(newBuffer);

                using (request.InputStream)
                {
                    var chunkSize = 8192;

                    while (true)
                    {
                        var availableBytes = request.InputStream.Length - request.InputStream.Position;
                        if (availableBytes <= 0)
                            break;

                        int readChunkSize = (int)Math.Min(chunkSize, availableBytes);

                        var buffers = new byte[readChunkSize];
                        _ = await request.InputStream.ReadAsync(buffers, 0, buffers.Length);

                        await _client.SendAsync(buffers);
                    }
                }
            }
            else
            {
                await _client.SendAsync(newBuffer);
            }

            return await _tcs.Task;
        }

        private async void DoReceive()
        {
            while (!_cts.IsCancellationRequested && IsConnected)
            {
                var package = await _client.ReceiveAsync();

                if (package == null)
                    break;

                if (package.IsOutputStream)
                {
                    if (!_hasWriteFile)
                    {
                        _fileStream = new FileStream(package.OutputFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                        _hasWriteFile = true;
                    }

                    //写入文件
                    await _fileStream.WriteAsync(package.Body, 0, package.Body.Length);
                    //刷新到磁盘
                    if (!package.IsComplete)
                        continue;

                    await _fileStream.FlushAsync();
                }

                //返回为Strem,需要逐步进行解析

                _resp.Header = new FastDFSHeader(package.Length, package.Command, package.Status);

                if (!_context.IsOutputStream)
                {
                    _resp.LoadContent(Configuration, package.Body);
                }
                Reset();
                _tcs.SetResult(_resp);
            }
        }

        private void Reset()
        {
            _hasWriteFile = false;
            _fileStream?.Close();
            _fileStream?.Dispose();
            _fileStream = null;
            _context = null;
        }


    }
}
