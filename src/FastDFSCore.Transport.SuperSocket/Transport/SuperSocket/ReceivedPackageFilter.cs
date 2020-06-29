using FastDFSCore.Utility;
using SuperSocket.ProtoBase;
using System;
using System.Buffers;

namespace FastDFSCore.Transport.SuperSocket
{
    public class ReceivedPackageFilter : PipelineFilterBase<ReceivedPackage>
    {
        readonly int lengthFieldEndOffset = Consts.FDFS_PROTO_PKG_LEN_SIZE + 2;
        private readonly Func<TransportContext> _getContextAction;

        private bool _foundHeader;
        private long _length = 0;
        private byte _command = 0;
        private byte _status = 0;
        private long _readPosition = 0;

        public ReceivedPackageFilter(Func<TransportContext> getContextAction)
        {
            _getContextAction = getContextAction;
        }


        public override ReceivedPackage Filter(ref SequenceReader<byte> reader)
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
                    if (reader.Length < lengthFieldEndOffset)
                    {
                        return null;
                    }
                    //整个数据包的长度,应该是包长度+2+body

                    var frameLengthBuffer = reader.Sequence.Slice(0, Consts.FDFS_PROTO_PKG_LEN_SIZE).ToArray();

                    var frameLength = ByteUtil.BufferToLong(frameLengthBuffer) + lengthFieldEndOffset;

                    if (reader.Length < frameLength)
                    {
                        //读取全部的数据
                        var readLength = reader.Length;

                        //长度
                        reader.TryReadBigEndian(out long _length);
                        //命令
                        reader.TryRead(out byte _command);
                        //状态
                        reader.TryRead(out byte _status);

                        package.Length = _length;
                        package.Command = _command;
                        package.Status = _status;

                        var bodyLength = readLength - lengthFieldEndOffset;
                        package.Body = reader.Sequence.Slice(bodyLength).ToArray();
                        reader.Advance(bodyLength);

                        //设置读取的进度
                        _readPosition += bodyLength;

                        _foundHeader = true;
                    }
                    else
                    {
                        //全部读完,则直接完成
                        // extract frame
                        var readerLength = reader.Length;
                        //IByteBuffer frame = ExtractFrame(context, input, readerIndex, frameLengthInt);
                        //input.SetReaderIndex(readerIndex + frameLengthInt);

                        //长度
                        reader.TryReadBigEndian(out long _length);
                        //命令
                        reader.TryRead(out byte _command);
                        //状态
                        reader.TryRead(out byte _status);

                        package.Length = _length;
                        package.Command = _command;
                        package.Status = _status;


                        package.Body = reader.Sequence.Slice(0, frameLength).ToArray();
                        reader.Advance(package.Body.Length);

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
                    var chunkSize = Math.Min(unreadLength, reader.Length);

                    package.Body = reader.Sequence.Slice(0, chunkSize).ToArray();
                    reader.Advance(package.Body.Length);

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
                if (reader.Length < lengthFieldEndOffset)
                {
                    return null;
                }
                //整个数据包的长度,应该是包长度+2+body

                var frameLengthBuffer = reader.Sequence.Slice(0, Consts.FDFS_PROTO_PKG_LEN_SIZE).ToArray();
                var frameLength = ByteUtil.BufferToLong(frameLengthBuffer) + lengthFieldEndOffset;

                if (reader.Length < frameLength)
                {
                    return null;
                }


                //长度
                reader.TryReadBigEndian(out long _length);
                //命令
                reader.TryRead(out byte _command);
                //状态
                reader.TryRead(out byte _status);

                package.Length = _length;
                package.Command = _command;
                package.Status = _status;

                package.Body = reader.Sequence.Slice(package.Length).ToArray();
                reader.Advance(package.Body.Length);

                //重置
                Reset();

                package.IsComplete = true;
            }

            return package;
        }

        public override void Reset()
        {
            _foundHeader = false;
            _length = 0;
            _command = 0;
            _status = 0;
            _readPosition = 0;

        }
    }
}
