using DotNetty.Transport.Channels;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FastDFSCore.Transport.DotNetty
{
    /// <summary>数据读取Handler
    /// </summary>
    public class FastDFSHandler : SimpleChannelInboundHandler<ReceivedPackage>
    {
        private bool _hasWriteFile = false;
        private FileStream _fileStream = null;
        private readonly Action<ReceivedPackage> _handleReceivedPack;
        private readonly Func<Exception, Task> _handleExceptionCaught;


        /// <summary>Ctor
        /// </summary>
        public FastDFSHandler(Action<ReceivedPackage> handleReceivedPack, Func<Exception, Task> handleExceptionCaught)
        {
            _handleReceivedPack = handleReceivedPack;
            _handleExceptionCaught = handleExceptionCaught;
        }

        /// <summary>ChannelRead0
        /// </summary>
        protected override void ChannelRead0(IChannelHandlerContext ctx, ReceivedPackage msg)
        {
            if (msg.IsOutputStream)
            {
                if (!_hasWriteFile)
                {
                    _fileStream = new FileStream(msg.OutputFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                }

                //写入文件
                _fileStream.Write(msg.Body, 0, msg.Body.Length);
                _hasWriteFile = true;
                //刷新到磁盘
                if (msg.IsComplete)
                {
                    _fileStream.Flush();
                    _handleReceivedPack(msg);

                    Reset();
                }
            }
            else
            {
                _handleReceivedPack(msg);
            }
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            _handleExceptionCaught?.Invoke(exception);
        }

        private void Reset()
        {

            _fileStream?.Close();
            _fileStream?.Dispose();
            _hasWriteFile = false;
        }




    }
}
