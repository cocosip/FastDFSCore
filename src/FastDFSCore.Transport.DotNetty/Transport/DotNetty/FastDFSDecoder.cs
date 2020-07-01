using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;

namespace FastDFSCore.Transport.DotNetty
{
    /// <summary>FDFS长度解码器
    /// </summary>
    public class FastDFSDecoder : ByteToMessageDecoder
    {

        //长度
        readonly int headerLength = Consts.FDFS_PROTO_PKG_LEN_SIZE + 2;
        private readonly Func<TransportContext> _getContextAction;

        private bool _foundHeader = false;
        private long _length = 0;
        private byte _command = 0;
        private byte _status = 0;
        private long _readPosition = 0;

        /// <summary>Ctor
        /// </summary>
        public FastDFSDecoder(Func<TransportContext> getContextAction)
        {
            _getContextAction = getContextAction;
        }

        /// <summary>Decode
        /// </summary>
        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            object decoded = Decode(context, input);
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

            var transportContext = _getContextAction();

            var package = new ReceivedPackage()
            {
                IsComplete = false,
                IsOutputStream = transportContext.IsOutputStream,
                OutputFilePath = transportContext.OutputFilePath
            };

            if (transportContext.IsOutputStream)
            {
                if (!_foundHeader)
                {
                    //未读取到头部,并且长度不足头部
                    if (input.ReadableBytes < headerLength)
                    {
                        return null;
                    }
                    //整个数据包的长度,应该是包长度+2+body
                    long frameLength = input.GetLong(input.ReaderIndex) + headerLength;

                    int frameLengthInt = (int)frameLength;
                    if (input.ReadableBytes < frameLengthInt)
                    {
                        //读取全部的数据
                        var readLength = input.ReadableBytes;
                        IByteBuffer frame = this.ExtractFrame(context, input, input.ReaderIndex, readLength);
                        input.SetReaderIndex(input.ReaderIndex + readLength);

                        _length = frame.ReadLong();
                        _command = frame.ReadByte();
                        _status = frame.ReadByte();

                        package.Length = _length;
                        package.Command = _command;
                        package.Status = _status;

                        var bodyLength = readLength - headerLength;

                        package.Body = new byte[bodyLength];
                        frame.ReadBytes(package.Body, 0, package.Body.Length);

                        //设置读取的进度
                        _readPosition += bodyLength;

                        _foundHeader = true;
                    }
                    else
                    {
                        //全部读完,则直接完成
                        // extract frame
                        int readerIndex = input.ReaderIndex;
                        IByteBuffer frame = ExtractFrame(context, input, readerIndex, frameLengthInt);
                        input.SetReaderIndex(readerIndex + frameLengthInt);

                        package.Length = frame.ReadLong();
                        package.Command = frame.ReadByte();
                        package.Status = frame.ReadByte();

                        package.Body = new byte[package.Length];
                        frame.ReadBytes(package.Body, 0, (int)package.Length);

                        //重置
                        Reset();

                        package.IsComplete = true;
                    }

                }
                else
                {
                    //不是第一次读

                    package.Length = _length;
                    package.Command = _command;
                    package.Status = _status;

                    //未读的长度
                    var unreadLength = _length - _readPosition;
                    //需要写的长度,剩余长度与当前接收包体两者取小值
                    var chunkSize = Math.Min(unreadLength, input.ReadableBytes);
                    IByteBuffer frame = this.ExtractFrame(context, input, input.ReaderIndex, (int)chunkSize);
                    //设置读的标记,读到头为止
                    input.SetReaderIndex(input.ReaderIndex + (int)chunkSize);

                    package.Body = new byte[chunkSize];
                    frame.ReadBytes(package.Body, 0, (int)chunkSize);

                    //判断是否完成
                    if (_readPosition + chunkSize >= _length)
                    {
                        //完成
                        Reset();

                        package.IsComplete = true;

                    }
                    else
                    {
                        //设置读取进度
                        _readPosition += chunkSize;
                    }
                }
            }
            else
            {
                //读取不到头部
                if (input.ReadableBytes < headerLength)
                {
                    return null;
                }
                //整个数据包的长度,应该是包长度+2+body
                long frameLength = input.GetLong(input.ReaderIndex) + headerLength;
                int frameLengthInt = (int)frameLength;
                if (input.ReadableBytes < frameLengthInt)
                {
                    return null;
                }
                // extract frame
                int readerIndex = input.ReaderIndex;
                IByteBuffer frame = ExtractFrame(context, input, readerIndex, frameLengthInt);
                input.SetReaderIndex(readerIndex + frameLengthInt);

                package.Length = frame.ReadLong();
                package.Command = frame.ReadByte();
                package.Status = frame.ReadByte();

                package.Body = new byte[package.Length];
                frame.ReadBytes(package.Body, 0, (int)package.Length);

                //重置
                Reset();

                package.IsComplete = true;
            }

            return package;
        }

        /// <summary>获取未调整的长度
        /// </summary>
        protected long GetUnadjustedFrameLength(IByteBuffer buffer, int offset)
        {
            return buffer.GetLong(offset);
        }


        /// <summary>提取框架数据
        /// </summary>
        protected virtual IByteBuffer ExtractFrame(IChannelHandlerContext context, IByteBuffer buffer, int index, int length)
        {
            IByteBuffer buff = buffer.Slice(index, length);
            buff.Retain();
            return buff;
        }


        private void Reset()
        {
            _foundHeader = false;
            _length = 0;
            _command = 0;
            _status = 0;
            _readPosition = 0;
        }
    }
}
