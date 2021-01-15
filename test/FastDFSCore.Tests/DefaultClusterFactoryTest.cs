using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace FastDFSCore.Tests
{
    public class DefaultClusterFactoryTest
    {
        private Mock<ILogger<DefaultClusterFactory>> _mockLogger;

        public DefaultClusterFactoryTest()
        {
            _mockLogger = new Mock<ILogger<DefaultClusterFactory>>();
        }


        [Fact]
        public void Get_Default_CreateNew_Test()
        {
            var optionsValue = new FastDFSOptions()
            {
                ClusterConfigurations = new List<ClusterConfiguration>()
                {
                    new ClusterConfiguration()
                    {
                        Name="cluster1",
                        Trackers=new List<Tracker>()
                        {
                            new Tracker("127.0.0.1",22122)
                        }
                    }
                }
            };

            var options = Options.Create<FastDFSOptions>((FastDFSOptions)optionsValue);

            var mockCluster = new Mock<ICluster>();
            mockCluster.Setup(x => x.Name).Returns("cluster1");

            var mockScopeServiceProvider = new Mock<IServiceProvider>();
            mockScopeServiceProvider.Setup(x => x.GetService(typeof(ICluster)))
                .Returns(mockCluster.Object);

            mockScopeServiceProvider.Setup(x => x.GetService(typeof(ClusterConfiguration)))
             .Returns(optionsValue.ClusterConfigurations.FirstOrDefault());

            var mockServiceScope = new Mock<IServiceScope>();
            mockServiceScope.Setup(x => x.ServiceProvider)
                .Returns(mockScopeServiceProvider.Object);

            var mockScopeFactory = new Mock<IServiceScopeFactory>();
            mockScopeFactory.Setup(x => x.CreateScope())
                .Returns(mockServiceScope.Object);

            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(mockScopeFactory.Object);

            var mockClusterSelector = new Mock<IClusterSelector>();
            mockClusterSelector.Setup(x => x.Get("cluster1")).Returns(optionsValue.ClusterConfigurations.FirstOrDefault());


            IClusterFactory clusterFactory = new DefaultClusterFactory(_mockLogger.Object, mockServiceProvider.Object, options, mockClusterSelector.Object);

            var cluster = clusterFactory.Get("");

            mockClusterSelector.Verify(x => x.Get("cluster1"), Times.Once);
            mockScopeServiceProvider.Verify(x => x.GetService(typeof(ICluster)), Times.Once);
            mockScopeServiceProvider.Verify(x => x.GetService(typeof(ClusterConfiguration)), Times.Once);

            var cluster2 = clusterFactory.Get("");

            Assert.Equal("cluster1", cluster2.Name);
        }

    }
}
