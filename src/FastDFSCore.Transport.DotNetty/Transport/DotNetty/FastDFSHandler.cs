using DotNetty.Transport.Channels;
using System;
using System.IO;

namespace FastDFSCore.Transport.DotNetty
{
    /// <summary>数据读取Handler
    /// </summary>
    public class FastDFSHandler : SimpleChannelInboundHandler<ReceivedPackage>
    {
        private bool _hasWriteFile = false;
        private FileStream _fileStream = null;
        private readonly Action<ReceivedPackage> _setResponse;

        /// <summary>Ctor
        /// </summary>
        public FastDFSHandler(Action<ReceivedPackage> setResponse)
        {
            _setResponse = setResponse;
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
                    _setResponse(msg);

                    Reset();
                }
            }
            else
            {
                _setResponse(msg);
            }
        }


        private void Reset()
        {

            _fileStream?.Close();
            _fileStream?.Dispose();
            _hasWriteFile = false;
        }




    }
}
