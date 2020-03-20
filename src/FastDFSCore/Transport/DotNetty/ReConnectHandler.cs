using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace FastDFSCore.Transport.DotNetty
{
    /// <summary>Client reconnect when connection shutdown
    /// </summary>
    public class ReConnectHandler : ChannelHandlerAdapter
    {
        private readonly ILogger _logger;
        private readonly FDFSOption _option;
        private readonly Func<Task> _reConnectAction;

        /// <summary>Ctor
        /// </summary>
        public ReConnectHandler(ILogger<ReConnectHandler> logger, FDFSOption option, Func<Task> reConnectAction)
        {
            _logger = logger;
            _option = option;
            _reConnectAction = reConnectAction;
        }

        /// <summary>ChannelInactive
        /// </summary>
        public override void ChannelInactive(IChannelHandlerContext context)
        {
            _logger.LogInformation("Channel:{0} is inactive!", context.Channel.Id.AsLongText());
            context.Channel.EventLoop.Schedule(() => _reConnectAction(), TimeSpan.FromSeconds(_option.TcpSetting.ReConnectDelaySeconds));
        }
    }
}
