using FastDFSCore.Scheduling;
using FastDFSCore.Transport;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace FastDFSCore.Tests.Transport
{
    public class DefaultConnectionPoolTest
    {
        private readonly Mock<ILogger<DefaultConnectionPool>> _mockLogger;
        public DefaultConnectionPoolTest()
        {
            _mockLogger = new Mock<ILogger<DefaultConnectionPool>>();
        }


        [Fact]
        public void GetConnection_Test()
        {
            var connectionPoolOption = new ConnectionPoolOption()
            {
                ConnectionAddress = new ConnectionAddress("192.168.0.6", 22122),
                ConnectionConcurrentThread = 1,
                ConnectionLifeTime = 3600,
                MaxConnection = 100,
                ScanTimeoutConnectionInterval = 500
            };

            var mockScheduleService = new Mock<IScheduleService>();
            var mockConnectionBuilder = new Mock<IConnectionBuilder>();
            var mockConnection = new Mock<IConnection>();
            mockConnection.Setup(x => x.Id)
                .Returns("1234567890");
            mockConnection.Setup(x => x.ConnectionAddress)
                .Returns(connectionPoolOption.ConnectionAddress);

            mockConnectionBuilder.Setup(x => x.CreateConnection(connectionPoolOption.ConnectionAddress))
                .Returns(mockConnection.Object);


            IConnectionPool connectionPool = new DefaultConnectionPool(_mockLogger.Object, mockScheduleService.Object, connectionPoolOption, mockConnectionBuilder.Object);

            var connection = connectionPool.GetConnection();

            Assert.NotEmpty(connectionPool.Name);
            Assert.Equal("192.168.0.6", connection.ConnectionAddress.IPAddress);
            Assert.Equal(22122, connection.ConnectionAddress.Port);
            Assert.Equal("1234567890", connection.Id);


            mockConnectionBuilder.Verify(x => x.CreateConnection(connectionPoolOption.ConnectionAddress), Times.Once);

        }

        [Fact]
        public void GetConnection_Return_Test()
        {
            var connectionPoolOption = new ConnectionPoolOption()
            {
                ConnectionAddress = new ConnectionAddress("192.168.0.6", 22122),
                ConnectionConcurrentThread = 1,
                ConnectionLifeTime = 3600,
                MaxConnection = 1,
                ScanTimeoutConnectionInterval = 500
            };

            var mockScheduleService = new Mock<IScheduleService>();
            var mockConnectionBuilder = new Mock<IConnectionBuilder>();
            var mockConnection = new Mock<IConnection>();
            mockConnection.Setup(x => x.Id)
                .Returns("1234567890");
            mockConnection.Setup(x => x.ConnectionAddress)
                .Returns(connectionPoolOption.ConnectionAddress);

            mockConnectionBuilder.Setup(x => x.CreateConnection(connectionPoolOption.ConnectionAddress))
                .Returns(mockConnection.Object);


            IConnectionPool connectionPool = new DefaultConnectionPool(_mockLogger.Object, mockScheduleService.Object, connectionPoolOption, mockConnectionBuilder.Object);

            var connection = connectionPool.GetConnection();

            Assert.NotEmpty(connectionPool.Name);
            Assert.Equal("192.168.0.6", connection.ConnectionAddress.IPAddress);
            Assert.Equal(22122, connection.ConnectionAddress.Port);
            Assert.Equal("1234567890", connection.Id);
            mockConnectionBuilder.Verify(x => x.CreateConnection(connectionPoolOption.ConnectionAddress), Times.Once);

            Task.Run(() =>
            {
                Task.Delay(200);
                connectionPool.Return(connection);
            });
            var connection2 = connectionPool.GetConnection();

            mockConnectionBuilder.Verify(x => x.CreateConnection(connectionPoolOption.ConnectionAddress), Times.Once);

            Assert.Equal(connection.ConnectionAddress, connection2.ConnectionAddress);
            Assert.Equal(connection.Id, connection2.Id);


        }
    }
}
