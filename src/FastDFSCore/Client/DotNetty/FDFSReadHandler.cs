using DotNetty.Transport.Channels;
using System;

namespace FastDFSCore.Client
{
    public class FDFSReadHandler : SimpleChannelInboundHandler<ConnectionReceiveItem>
    {

        private Action<ConnectionReceiveItem> _setResponseAction;
        public FDFSReadHandler(Action<ConnectionReceiveItem> setResponseAction)
        {
            _setResponseAction = setResponseAction;
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, ConnectionReceiveItem msg)
        {
            _setResponseAction(msg);
        }
    }
}
