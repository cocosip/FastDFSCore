using DotNetty.Transport.Channels;
using System;

namespace FastDFSCore.Transport.DotNetty
{
    /// <summary>数据读取Handler
    /// </summary>
    public class FastDFSReadHandler : SimpleChannelInboundHandler<ReceiveData>
    {
        private readonly Action<ReceiveData> _setResponse;

        /// <summary>Ctor
        /// </summary>
        public FastDFSReadHandler(Action<ReceiveData> setResponse)
        {
            _setResponse = setResponse;
        }

        /// <summary>ChannelRead0
        /// </summary>
        protected override void ChannelRead0(IChannelHandlerContext ctx, ReceiveData msg)
        {
            _setResponse(msg);
        }
    }
}
