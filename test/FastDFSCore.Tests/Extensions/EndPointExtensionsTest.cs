using System.Net;
using Xunit;
using FastDFSCore.Extensions;

namespace FastDFSCore.Tests.Extensions
{
    public class EndPointExtensionsTest
    {
        [Fact]
        public void ToIPv4Address_Test()
        {
            var ipAddress1 = IPAddress.Parse("127.0.0.1");
            var ipAddressString1 = ipAddress1.ToIPv4Address();
            Assert.Equal("127.0.0.1", ipAddressString1);
        }

        [Fact]
        public void ToStringAddress_Test()
        {
            var ipEndPoint1 = new IPEndPoint(IPAddress.Parse("192.168.0.100"), 9555);
            var ipEndPointString1 = ipEndPoint1.ToStringAddress();
            Assert.Equal("192.168.0.100:9555", ipEndPointString1);

            var ipEndPoint2 = new IPEndPoint(IPAddress.Parse("192.133.55.10"), 3321);
            var ipEndPointString2 = ipEndPoint2.ToStringAddress();
            Assert.Equal("192.133.55.10:3321", ipEndPointString2);
        }
    }
}
