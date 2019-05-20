using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastDFSCore.Client
{
    public class FDFSReadHandler: ChannelHandlerAdapter
    {

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {

            base.ChannelRead(context, message);

        }

    }
}
