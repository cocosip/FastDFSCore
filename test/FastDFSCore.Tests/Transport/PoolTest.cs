using FastDFSCore.Scheduling;
using FastDFSCore.Transport;
using FastDFSCore.Transport.DotNetty;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Net;
using Xunit;

namespace FastDFSCore.Tests.Transport
{
    public class PoolTest
    {

        private Mock<IScheduleService> _mockScheduleService;
        private Mock<IConnectionPoolFactory> _mockConnectionPoolFactory;
        private Mock<ILogger<Pool>> _mockLogger;
        public PoolTest()
        {
            _mockScheduleService = new Mock<IScheduleService>();
            _mockConnectionPoolFactory = new Mock<IConnectionPoolFactory>();
            _mockLogger = new Mock<ILogger<Pool>>();
        }


        [Fact]
        public void GetConnection_Test()
        {

            var option = FDFSOptionHelper.GetFDFSOption("FDFSOption.xml");
            var connection = new Connection(default(IServiceProvider), default(ILogger<Connection>), option, new ConnectionAddress(), c => { });

            var ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 22122);
            var pool = new Pool(_mockLogger.Object, _mockScheduleService.Object, _mockConnectionPoolFactory.Object, ipEndPoint, 2, 300, 20); _mockConnectionPoolFactory.Setup(x => x.CreateConnection(It.Is<ConnectionAddress>(v => v.ServerEndPoint == ipEndPoint), It.Is<Action<Connection>>(x => true))).Returns(connection);

            var connection1 = AsyncHelper.RunSync(() =>
            {
                return pool.GetConnection();
            });

            var connection2 = AsyncHelper.RunSync(() =>
            {
                return pool.GetConnection();
            });

            Assert.NotNull(connection1);
            Assert.NotNull(connection2);
            pool.ConnectionClose(connection1);

            var connection3 = AsyncHelper.RunSync(() =>
            {
                return pool.GetConnection();
            });
            Assert.NotNull(connection3);

        }

    }
}
