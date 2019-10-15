using DotNetty.Common.Internal.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace FastDFSCore.Client
{
    /// <summary>下载文件基类
    /// </summary>
    public abstract class BaseDownloader : IDownloader
    {
        private bool _isRunning = false;
        private int _isComplete = 0;
        private ILogger Logger { get; }

        /// <summary>FDFSOption <see cref="FastDFSCore.Client.FDFSOption"/>
        /// </summary>
        protected FDFSOption Option { get; }

        private readonly ConcurrentQueue<byte[]> _pendingWriteQueue = new ConcurrentQueue<byte[]>();

        /// <summary>Ctor
        /// </summary>
        public BaseDownloader(ILoggerFactory loggerFactory, FDFSOption option)
        {
            Option = option;
            Logger = loggerFactory.CreateLogger(option.LoggerName);
        }


        /// <summary>文件保存的路径
        /// </summary>
        public string SavePath { get; protected set; } = "";

        /// <summary>Run
        /// </summary>
        public void Run()
        {
            if (_isRunning)
            {
                return;
            }

            _isRunning = true;

            //开始运行,初始化文件流
            BeginWrite();

            Task.Run(() =>
            {
                while (_isComplete == 0)
                {
                    try
                    {
                        if (_pendingWriteQueue.TryDequeue(out byte[] buffer))
                        {
                            //写入文件
                            WriteToFile(buffer);
                        }
                        else
                        {
                            Thread.Sleep(3);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("下载出现错误,{0}", ex.Message);
                        Interlocked.Exchange(ref _isComplete, 1);
                        throw;
                    }
                }
                //释放
                Release();
            });
        }


        /// <summary>写入数据
        /// </summary>
        public void WriteBuffers(byte[] buffer)
        {
            _pendingWriteQueue.Enqueue(buffer);
        }

        /// <summary>写入完成
        /// </summary>
        public void WriteComplete()
        {
            while (!_pendingWriteQueue.IsEmpty)
            {
                Thread.Sleep(5);
            }
            Interlocked.Exchange(ref _isComplete, 1);
        }


        /// <summary>开始运行
        /// </summary>
        public abstract void BeginWrite();

        /// <summary>写入到文件的操作
        /// </summary>
        public abstract void WriteToFile(byte[] buffers);


        /// <summary>释放
        /// </summary>
        public virtual void Release()
        {
            Interlocked.Exchange(ref _isComplete, 1);
        }
    }
}
