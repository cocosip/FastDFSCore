using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;

namespace FastDFSCore.Client
{
    /// <summary>FDFS长度解码器
    /// </summary>
    public class FDFSDecoder : ByteToMessageDecoder
    {

        //长度
        readonly int lengthFieldLength = Consts.FDFS_PROTO_PKG_LEN_SIZE;
        readonly int lengthFieldEndOffset = Consts.FDFS_PROTO_PKG_LEN_SIZE + 2;
        private Func<ConnectionContext> getConnectionContextAction;
        public FDFSDecoder(Func<ConnectionContext> getConnectionContext)
        {
            this.getConnectionContextAction = getConnectionContext;
            this.lengthFieldEndOffset = lengthFieldLength + 2;
        }

        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            object decoded = this.Decode(context, input);
            if (decoded != null)
            {
                output.Add(decoded);
            }
        }

        /// <summary>
        ///     Create a frame out of the <see cref="IByteBuffer" /> and return it.
        /// </summary>
        /// <param name="context">
        ///     The <see cref="IChannelHandlerContext" /> which this <see cref="ByteToMessageDecoder" /> belongs
        ///     to.
        /// </param>
        /// <param name="input">The <see cref="IByteBuffer" /> from which to read data.</param>
        /// <returns>The <see cref="IByteBuffer" /> which represents the frame or <c>null</c> if no frame could be created.</returns>
        protected virtual object Decode(IChannelHandlerContext context, IByteBuffer input)
        {
            var connectionContext = getConnectionContextAction();

            var receiveItem = new ConnectionReceiveItem();

            if (connectionContext.StreamResponse)
            {
                //已经开始Chunk写入,那么直接就读取返回数据,不停的去保存到返回值中去
                if (connectionContext.IsChunkWriting)
                {
                    //读取Chunk下载的数据
                    if (!connectionContext.IsReadCompleted)
                    {
                        //未读的长度
                        var unreadLength = connectionContext.GetUnreadLength();
                        //需要写的长度,剩余长度与当前接收包体两者取小值
                        var chunkSize = Math.Min(unreadLength, input.ReadableBytes);
                        IByteBuffer frame = this.ExtractFrame(context, input, input.ReaderIndex, (int)chunkSize);
                        //设置读的标记,读到头为止
                        input.SetReaderIndex(input.ReaderIndex + (int)chunkSize);
                        receiveItem.ReadChunkBody(frame, (int)chunkSize);
                        connectionContext.ReadPosition += chunkSize;
                    }
                    else
                    {
                        return null;
                    }

                }
                else
                {
                    //流传输,头部也还未读
                    if (input.ReadableBytes < lengthFieldEndOffset)
                    {
                        return null;
                    }
                    IByteBuffer frame = ExtractFrame(context, input, input.ReaderIndex, lengthFieldEndOffset);
                    //设置读的标记,读到头为止
                    input.SetReaderIndex(input.ReaderIndex + lengthFieldEndOffset);
                    receiveItem.ReadHeader(frame);
                    //头部读完
                    connectionContext.IsChunkWriting = true;
                    connectionContext.Header = receiveItem.Header;
                }
            }
            else
            {
                if (input.ReadableBytes < lengthFieldEndOffset)
                {
                    return null;
                }
                //整个数据包的长度,应该是包长度+2+body
                long frameLength = input.GetLong(input.ReaderIndex) + lengthFieldEndOffset;
                int frameLengthInt = (int)frameLength;
                if (input.ReadableBytes < frameLengthInt)
                {
                    return null;
                }
                // extract frame
                int readerIndex = input.ReaderIndex;
                int actualFrameLength = frameLengthInt;
                IByteBuffer frame = ExtractFrame(context, input, readerIndex, actualFrameLength);
                input.SetReaderIndex(readerIndex + actualFrameLength);
                receiveItem.ReadFromBuffer(frame);
            }
            ReferenceCountUtil.Release(input);
            return receiveItem;
        }

        protected long GetUnadjustedFrameLength(IByteBuffer buffer, int offset)
        {
            return buffer.GetLong(offset);
        }

        protected virtual IByteBuffer ExtractFrame(IChannelHandlerContext context, IByteBuffer buffer, int index, int length)
        {
            IByteBuffer buff = buffer.Slice(index, length);
            buff.Retain();
            return buff;
        }

    }
}
