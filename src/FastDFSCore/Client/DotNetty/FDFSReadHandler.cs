using DotNetty.Transport.Channels;
using System;

namespace FastDFSCore.Client
{
    /// <summary>数据读取Handler
    /// </summary>
    public class FDFSReadHandler : SimpleChannelInboundHandler<ConnectionReceiveItem>
    {

        private readonly Action<ConnectionReceiveItem> _setResponseAction;

        /// <summary>Ctor
        /// </summary>
        /// <param name="setResponseAction">设置结果Action</param>
        public FDFSReadHandler(Action<ConnectionReceiveItem> setResponseAction)
        {
            _setResponseAction = setResponseAction;
        }

        /// <summary>ChannelRead0
        /// </summary>
        protected override void ChannelRead0(IChannelHandlerContext ctx, ConnectionReceiveItem msg)
        {
            _setResponseAction(msg);
        }
    }
}
