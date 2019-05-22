using DotNetty.Transport.Channels;
using System;

namespace FastDFSCore.Client
{
    public class FDFSReadHandler : SimpleChannelInboundHandler<ConnectionContext>
    {

        private Action _setResultAction;
        public FDFSReadHandler(Action setResultAction)
        {
            _setResultAction = setResultAction;
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, ConnectionContext msg)
        {
            _setResultAction();
        }
    }
}
