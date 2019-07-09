using FastDFSCore.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Xunit;

namespace FastDFSCore.Tests.FastDFSCore
{
    public class FDFSOptionTranslatorTest
    {
        static IServiceProvider _provider;
        static FDFSOptionTranslatorTest()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddFastDFSCore("FDFSOption.xml");

            _provider = services.BuildServiceProvider();
            _provider.ConfigureFastDFSCore();
        }

        [Fact]
        public void TranslateToOption_Test()
        {
            var option = FDFSOptionTranslatorTest._provider.GetService<FDFSOption>();

            Assert.Equal(option.Charset, Encoding.UTF8);

            Assert.Equal(5, option.ConnectionTimeout);
            Assert.Equal(3200, option.ConnectionLifeTime);
            Assert.Equal(5, option.TrackerMaxConnection);
            Assert.Equal(20, option.StorageMaxConnection);
            Assert.Equal("Name1", option.LoggerName);

            Assert.Equal(30, option.TcpSetting.QuietPeriodMilliSeconds);
            Assert.Equal(1, option.TcpSetting.CloseTimeoutSeconds);
            Assert.Equal(16777216, option.TcpSetting.WriteBufferHighWaterMark);
            Assert.Equal(8388608, option.TcpSetting.WriteBufferLowWaterMark);
            Assert.Equal(1048576, option.TcpSetting.SoRcvbuf);
            Assert.Equal(1048576, option.TcpSetting.SoSndbuf);
            Assert.True(option.TcpSetting.TcpNodelay);
            Assert.False(option.TcpSetting.SoReuseaddr);


            var tracker = option.Trackers.FirstOrDefault();

            Assert.Equal("127.1.1.1", tracker.ToIPv4Address());
            Assert.Equal(22122, tracker.Port);

        }

        [Fact]
        public void TranslateToXml_Test()
        {
            var option = new FDFSOption()
            {
                Charset = Encoding.UTF8,
                ConnectionLifeTime = 200,
                ConnectionTimeout = 300,
                StorageMaxConnection = 2,
                TrackerMaxConnection = 1,
                LoggerName = "L1",
                Trackers = new List<IPEndPoint>()
                {
                    new IPEndPoint(IPAddress.Parse("127.0.0.1"),1333)
                },
                TcpSetting = new TcpSetting()
            };
            var xml = FDFSOptionHelper.ToXml(option);
            File.WriteAllText("test.xml", xml);

            var readOption = FDFSOptionHelper.GetFDFSOption("test.xml");

            Assert.Equal(option.Charset, readOption.Charset);

        }


    }
}
