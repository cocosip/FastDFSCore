using FastDFSCore.Utility;
using SuperSocket.ProtoBase;
using System;
using System.Buffers;

namespace FastDFSCore.Transport.SuperSocket
{
    public class ReceivedPackageFilter : PipelineFilterBase<ReceivedPackage>
    {
        readonly int headerLength = Consts.FDFS_PROTO_PKG_LEN_SIZE + 2;
        private readonly Func<TransportContext> _getCtx;

        private bool _foundHeader;
        private long _length = 0;
        private byte _command = 0;
        private byte _status = 0;
        private long _readPosition = 0;

        public ReceivedPackageFilter(Func<TransportContext> getContext)
        {
            _getCtx = getContext;

        }


        public override ReceivedPackage Filter(ref SequenceReader<byte> reader)
        {
            var transportContext = _getCtx();

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
                    if (reader.Length < headerLength)
                    {
                        return null;
                    }

                    //长度
                    var packLength = ByteUtil.BufferToLong(reader.Sequence.Slice(0, Consts.FDFS_PROTO_PKG_LEN_SIZE).ToArray());

                    var frameLength = packLength + headerLength;

                    if (reader.Length < frameLength)
                    {
                        //缓存中没有把整个文件数据缓存下来,就需要读一部分写一步
                        var readLength = reader.Length;

                        //长度
                        reader.TryReadBigEndian(out _length);

                        //命令                                                                                     
                        reader.TryRead(out _command);
                        //状态
                        reader.TryRead(out _status);

                        package.Length = _length;
                        package.Command = _command;
                        package.Status = _status;

                        var bodyLength = readLength - headerLength;
                        package.Body = reader.Sequence.Slice(headerLength, bodyLength).ToArray();
                        reader.Advance(bodyLength);

                        //设置读取的进度
                        _readPosition += bodyLength;

                        _foundHeader = true;

                    }
                    else
                    {
                        //全部读完,则直接完成

                        //长度
                        reader.TryReadBigEndian(out _length);
                        //命令
                        reader.TryRead(out _command);
                        //状态
                        reader.TryRead(out _status);

                        package.Length = _length;
                        package.Command = _command;
                        package.Status = _status;

                        package.Body = reader.Sequence.Slice(headerLength, packLength).ToArray();
                        reader.Advance(package.Body.Length);

                        _foundHeader = true;

                        //重置
                        ReceiveReset();

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
                        ReceiveReset();

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
                //如果是整包读取,但是还读取不到头,等待下次接收
                if (reader.Length < headerLength)
                {
                    return null;
                }
                //整个数据包的长度,应该是包长度+2+body

                //包的数据长度
                var packLength = ByteUtil.BufferToLong(reader.Sequence.Slice(0, Consts.FDFS_PROTO_PKG_LEN_SIZE).ToArray());

                var frameLength = packLength + headerLength;

                if (reader.Length < frameLength)
                {
                    return null;
                }

                //长度
                reader.TryReadBigEndian(out _length);
                //命令
                reader.TryRead(out _command);
                //状态
                reader.TryRead(out _status);

                package.Length = _length;
                package.Command = _command;
                package.Status = _status;

                package.Body = reader.Sequence.Slice(headerLength, packLength).ToArray();
                reader.Advance(package.Body.Length);

                //重置
                ReceiveReset();

                package.IsComplete = true;
            }

            return package;
        }


        private void ReceiveReset()
        {
            base.Reset();
            _foundHeader = false;
            _length = 0;
            _command = 0;
            _status = 0;
            _readPosition = 0;
        }

    }
}
