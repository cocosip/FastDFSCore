using DotNetty.Buffers;

namespace FastDFSCore.Client
{
    public static class ByteBufferExtensions
    {
        public static long ReadULong(this IByteBuffer byteBuffer)
        {
            var dataBuffer = new byte[8];
            byteBuffer.ReadBytes(dataBuffer, 0, dataBuffer.Length);
            return Util.BufferToLong(dataBuffer);
        }
    }
}
