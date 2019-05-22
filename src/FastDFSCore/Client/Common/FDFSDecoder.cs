using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;

namespace FastDFSCore.Client
{
    public class FDFSDecoder : MessageToMessageDecoder<IByteBuffer>
    {
        private Func<ConnectionContext> _getConnectionContext;

        public FDFSDecoder(Func<ConnectionContext> getConnectionContext)
        {
            _getConnectionContext = getConnectionContext;
        }


        protected override void Decode(IChannelHandlerContext context, IByteBuffer message, List<object> output)
        {
            object decoded = this.Decode(context, message);
            if (decoded != null)
            {
                output.Add(decoded);
            }
        }

        /// <summary>读取数据
        /// </summary>
        protected virtual object Decode(IChannelHandlerContext context, IByteBuffer input)
        {
            var connectionContext = _getConnectionContext();
            if (connectionContext.IsFileDownload && connectionContext.IsDownloadToPath)
            {
                if (connectionContext.IsChunkDownload)
                {
                    //读取Chunk下载的数据
                    connectionContext.ReadChunkBody(input);
                }
                else
                {
                    //流传输,头部也还未读
                    if (input.ReadableBytes < ConnectionContext.HeaderLength)
                    {
                        return null;
                    }
                    //设置读取位置
                    input.MarkReaderIndex();
                    connectionContext.ReadHeader(input);
                }
            }
            else
            {
                //非流中读取
                if (input.ReadableBytes < ConnectionContext.HeaderLength)
                {
                    return null;
                }
                //设置读取位置
                input.MarkReaderIndex();
                //读取的body长度
                var length = input.ReadLong();
                var totalLength = length + 2;

                //如果当前读取数据不足一个包长
                if (input.ReadableBytes < totalLength)
                {
                    input.ResetReaderIndex();
                    return null;
                }
                connectionContext.ReadHeader(input);
                connectionContext.ReadBody(input);
            }
            return connectionContext;
        }

    }
}
