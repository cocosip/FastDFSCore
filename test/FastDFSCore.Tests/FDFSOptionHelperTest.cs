using System;
using System.IO;
using Xunit;

namespace FastDFSCore.Tests
{
    public class FDFSOptionTransformerTest
    {

        [Fact]
        public void GetFDFSOption_ToXml_Test()
        {
            IFDFSOptionTransformer transformer = new FDFSOptionTransformer();
            var option = transformer.GetOptionFromFile("FDFSOption.xml");
            Assert.NotNull(option);

            Assert.Single(option.Trackers);
            Assert.Equal("utf-8", option.Charset);
            Assert.Equal(5, option.ConnectionTimeout);
            Assert.Equal(3200, option.ConnectionLifeTime);
            Assert.Equal(10, option.ScanTimeoutConnectionInterval);
            Assert.Equal(5, option.TrackerMaxConnection);
            Assert.Equal(20, option.StorageMaxConnection);
            Assert.True(option.TcpSetting.TcpNodelay);
            Assert.False(option.TcpSetting.SoReuseaddr);
            Assert.True(option.TcpSetting.EnableReConnect);
            Assert.Equal(3, option.TcpSetting.ReConnectDelaySeconds);
            Assert.Equal(1001, option.TcpSetting.ReConnectIntervalMilliSeconds);
            Assert.Equal(10, option.TcpSetting.ReConnectMaxCount);


            var xml = transformer.ToXml(option);

            var tempPath = Path.Combine(AppContext.BaseDirectory, "t1.xml");
            File.WriteAllText(tempPath, xml);

            var option2 = transformer.GetOptionFromFile(tempPath);
            Assert.Single(option2.Trackers);
            Assert.Equal("utf-8", option2.Charset);
            Assert.Equal(5, option2.ConnectionTimeout);
            Assert.Equal(3200, option2.ConnectionLifeTime);
            Assert.Equal(10, option2.ScanTimeoutConnectionInterval);
            Assert.Equal(5, option2.TrackerMaxConnection);
            Assert.Equal(20, option2.StorageMaxConnection);
            Assert.True(option2.TcpSetting.TcpNodelay);
            Assert.False(option2.TcpSetting.SoReuseaddr);
            Assert.True(option2.TcpSetting.EnableReConnect);
            Assert.Equal(3, option2.TcpSetting.ReConnectDelaySeconds);
            Assert.Equal(1001, option2.TcpSetting.ReConnectIntervalMilliSeconds);
            Assert.Equal(10, option2.TcpSetting.ReConnectMaxCount);
            File.Delete(tempPath);
        }

    }
}
