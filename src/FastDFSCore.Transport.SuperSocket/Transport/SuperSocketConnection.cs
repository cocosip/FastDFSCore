using FastDFSCore.Protocols;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace FastDFSCore.Transport
{
    public class SuperSocketConnection : BaseConnection
    {

        public SuperSocketConnection(ILogger<BaseConnection> logger, IServiceProvider serviceProvider, IOptions<FastDFSOption> option, ConnectionAddress connectionAddress) : base(logger, serviceProvider, option, connectionAddress)
        {

        }

        public override Task RunAsync()
        {
            throw new NotImplementedException();
        }

        public override Task<FastDFSResp> SendRequestAsync<T>(FastDFSReq<T> request)
        {
            throw new NotImplementedException();
        }

        public override Task ShutdownAsync()
        {
            throw new NotImplementedException();
        }

        protected override Task DoConnect()
        {
            throw new NotImplementedException();
        }

        protected override bool IsAvailable()
        {
            throw new NotImplementedException();
        }
    }
}
