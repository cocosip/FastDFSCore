using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastDFSCore.Client
{
    public class FDFSReadHandler : ByteToMessageDecoder
    {
        
        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            object decoded = this.Decode(context, input);
            if (decoded != null)
            {
                output.Add(decoded);
            }
        }

        protected virtual object Decode(IChannelHandlerContext context, IByteBuffer input)
        {

            return null;
        }
    }
}
