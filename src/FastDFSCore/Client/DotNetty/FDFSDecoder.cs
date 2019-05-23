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

            var connectionReceiveItem = new ConnectionReceiveItem();

            //if (connectionContext.StreamResponse)
            //{
            //    //已经开始Chunk写入,那么直接就读取返回数据,不停的去保存到返回值中去
            //    if (connectionContext.IsChunkWriting)
            //    {
            //        //读取Chunk下载的数据
            //        connectionReceiveItem.ReadChunkBody(input);
            //    }
            //    else
            //    {
            //        //流传输,头部也还未读
            //        if (input.ReadableBytes < ConnectionReceiveItem.HeaderLength)
            //        {
            //            return null;
            //        }
            //        //设置读取位置
            //        input.MarkReaderIndex();
            //        var length = input.ReadLong();
            //        connectionReceiveItem.ReadHeader(length, input);
            //    }
            //}
            //else
            //{
            //    //非流中读取
            //    if (input.ReadableBytes < ConnectionReceiveItem.HeaderLength)
            //    {
            //        return null;
            //    }

            //    _a = $"长度:{input.GetLong(input.ReaderIndex)},ReaderIndex:{input.ReaderIndex},ReadableBytes:{input.ReadableBytes}";
            //    //读取的body长度

            //    var length = input.ReadLong();
            //    var totalLength = length + 2;
            //    //如果当前读取数据不足一个包长
            //    if (input.ReadableBytes < totalLength)
            //    {
            //        return null;
            //    }
            //    //跳过头部
            //    connectionReceiveItem.ReadHeader(length, input);
            //    connectionReceiveItem.ReadBody(input);
            //}
            return connectionReceiveItem;
        }

    }
}
