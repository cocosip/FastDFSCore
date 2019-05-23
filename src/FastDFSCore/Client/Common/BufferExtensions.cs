using DotNetty.Buffers;

namespace FastDFSCore.Client
{
    public static class BufferExtensions
    {
        public static long ReadULong(this IByteBuffer byteBuffer)
        {
            var data = new byte[8];
            byteBuffer.ReadBytes(data, 0, data.Length);
            return Util.BufferToLong(data);
        }
    }
}
