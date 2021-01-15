using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using Xunit;

namespace FastDFSCore.Tests
{
    public class DefaultClusterSelectorTest
    {
        [Fact]
        public void ClusterConfigurations_Empty_Test()
        {
            var optionsValue = new FastDFSOptions();

            var options = Options.Create<FastDFSOptions>((FastDFSOptions)optionsValue);
            IClusterSelector clusterSelector = new DefaultClusterSelector(options);

            Assert.Throws<ArgumentNullException>(() =>
            {
                clusterSelector.Get("cluster1");
            });
        }

        [Fact]
        public void ClusterConfigurations_Get_Default_Test()
        {
            var optionValue = new FastDFSOptions()
            {
                ClusterConfigurations = new List<ClusterConfiguration>()
                {
                    new ClusterConfiguration()
                    {
                        Name="cluster2",
                        Charset="utf-8"
                    }
                }
            };

            var options = Options.Create<FastDFSOptions>((FastDFSOptions)optionValue);
            IClusterSelector clusterSelector = new DefaultClusterSelector(options);

            var configuration = clusterSelector.Get("cluster1");
            Assert.Equal("cluster2", configuration.Name);

        }


        [Fact]
        public void ClusterConfigurations_Get_Test()
        {
            var optionsValue = new FastDFSOptions()
            {
                ClusterConfigurations = new List<ClusterConfiguration>()
                {
                    new ClusterConfiguration()
                    {
                        Name="cluster1",
                        Charset="utf-8",
                        TrackerMaxConnection=1,
                        StorageMaxConnection=2,
                        Trackers=new List<Tracker>()
                        {
                            new Tracker("127.0.0.1",22122)
                        }
                    }
                }
            };

            var options = Options.Create<FastDFSOptions>((FastDFSOptions)optionsValue);
            IClusterSelector clusterSelector = new DefaultClusterSelector(options);

            var configuration = clusterSelector.Get("cluster1");
            Assert.Equal("cluster1", configuration.Name);
            Assert.Equal(1, configuration.TrackerMaxConnection);
            Assert.Equal(2, configuration.StorageMaxConnection);
        }

    }
}
