using DotNetty.Buffers;
using DotNetty.Common.Utilities;
using DotNetty.Handlers.Streams;
using DotNetty.Transport.Channels;
using System.IO;
using System.Threading.Tasks;

namespace FastDFSCore.Client
{
    public class FDFSWriteHandler : ChunkedWriteHandler<IByteBuffer>
    {
        public override Task WriteAsync(IChannelHandlerContext context, object message)
        {
            if (message is ChunkedStream)
            {
                return base.WriteAsync(context, message);
            }
            else
            {
                //release = false;
                return context.WriteAndFlushAsync(message);
            }

            //bool release = true;
            //try
            //{
            //    if (message is ChunkedStream)
            //    {
            //        return base.WriteAsync(context, message);
            //    }
            //    else
            //    {
            //        release = false;
            //        return context.WriteAsync(message);
            //    }
            //}
            //finally
            //{
            //    if (release && !(message is ChunkedStream))
            //    {
            //        ReferenceCountUtil.Release(message);
            //    }
            //}
        }

    }
}
