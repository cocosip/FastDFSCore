using FastDFSCore.Protocols;
using FastDFSCore.Transport;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FastDFSCore.Tests
{
    public class FastDFSClientTest
    {

        [Fact]
        public async Task GetStorageNodeAsync_Test()
        {
            var mockExecuter = new Mock<IExecuter>();
            mockExecuter.Setup(x => x.Execute(It.IsAny<FastDFSReq<QueryStoreWithGroupResp>>(), It.IsAny<string>(), It.IsAny<ConnectionAddress>())).ReturnsAsync(new QueryStoreWithGroupResp()
            {
                GroupName = "group2",
                IPAddress = "192.168.0.2",
                Port = 23000,
                StorePathIndex = 0
            });
            IFastDFSClient client = new FastDFSClient(mockExecuter.Object);
            var node = await client.GetStorageNodeAsync("group1");
            Assert.Equal("group2", node.GroupName);
            Assert.Equal(new ConnectionAddress("192.168.0.2", 23000), node.ConnectionAddress);

            mockExecuter.Verify(x => x.Execute(It.IsAny<FastDFSReq<QueryStoreWithGroupResp>>(), It.IsAny<string>(), It.IsAny<ConnectionAddress>()), Times.Once);

        }

        [Fact]
        public async Task ListOneGroupInfoAsync_Test()
        {
            var mockExecuter = new Mock<IExecuter>();
            mockExecuter.Setup(x => x.Execute(It.IsAny<FastDFSReq<ListOneGroupResp>>(), It.IsAny<string>(), It.IsAny<ConnectionAddress>())).ReturnsAsync(new ListOneGroupResp()
            {
                GroupInfo = new GroupInfo()
                {
                    GroupName = "group2"
                }
            });

            IFastDFSClient client = new FastDFSClient(mockExecuter.Object);
            var groupInfo = await client.ListOneGroupInfoAsync("group1");
            Assert.Equal("group2", groupInfo.GroupName);

            mockExecuter.Verify(x => x.Execute(It.IsAny<FastDFSReq<ListOneGroupResp>>(), It.IsAny<string>(), It.IsAny<ConnectionAddress>()), Times.Once);
        }

        [Fact]
        public async Task ListAllGroupInfosAsync_Test()
        {
            var mockExecuter = new Mock<IExecuter>();
            mockExecuter.Setup(x => x.Execute(It.IsAny<FastDFSReq<ListAllGroupResp>>(), It.IsAny<string>(), It.IsAny<ConnectionAddress>())).ReturnsAsync(new ListAllGroupResp()
            {
                GroupInfos = new List<GroupInfo>()
                {
                    new GroupInfo()
                    {
                        GroupName="group1"
                    }
                }
            });

            IFastDFSClient client = new FastDFSClient(mockExecuter.Object);
            var groupInfos = await client.ListAllGroupInfosAsync();
            Assert.Single(groupInfos);

            mockExecuter.Verify(x => x.Execute(It.IsAny<FastDFSReq<ListAllGroupResp>>(), It.IsAny<string>(), It.IsAny<ConnectionAddress>()), Times.Once);
        }



    }
}
