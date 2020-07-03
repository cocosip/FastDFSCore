using FastDFSCore.Transport;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using Xunit;

namespace FastDFSCore.Tests.Transport
{
    public class DefaultConnectionBuilderTest
    {

        [Fact]
        public void CreateConnection_Test()
        {
            var mockConnection = new Mock<IConnection>();
            mockConnection.Setup(x => x.Id)
                .Returns("123456");

            var connectionAddress = new ConnectionAddress("192.168.0.6", 22122);
            var mockScopeServiceProvider = new Mock<IServiceProvider>();
            mockScopeServiceProvider.Setup(x => x.GetService(typeof(ConnectionAddress)))
                .Returns(connectionAddress);
            mockScopeServiceProvider.Setup(x => x.GetService(typeof(IConnection)))
                .Returns(mockConnection.Object);

            var mockServiceScope = new Mock<IServiceScope>();
            mockServiceScope.Setup(x => x.ServiceProvider)
                .Returns(mockScopeServiceProvider.Object);

            var mockScopeFactory = new Mock<IServiceScopeFactory>();
            mockScopeFactory.Setup(x => x.CreateScope())
                .Returns(mockServiceScope.Object);

            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(mockScopeFactory.Object);


            IConnectionBuilder connectionBuilder = new DefaultConnectionBuilder(mockServiceProvider.Object);
            var connection = connectionBuilder.CreateConnection(connectionAddress);

            Assert.Equal("123456", connection.Id);

            mockScopeServiceProvider.Verify(x => x.GetService(typeof(IConnection)), Times.Once);
            mockScopeServiceProvider.Verify(x => x.GetService(typeof(ConnectionAddress)), Times.Once);

        }

    }
}
