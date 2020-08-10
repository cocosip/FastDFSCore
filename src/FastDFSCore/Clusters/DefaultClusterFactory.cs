using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace FastDFSCore
{
    public class DefaultClusterFactory : IClusterFactory
    {

        private readonly ConcurrentDictionary<string, ICluster> _clusterDict;

        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly FastDFSOption _option;
        private readonly IClusterSelector _clusterSelector;

        public DefaultClusterFactory(ILogger<DefaultClusterFactory> logger, IServiceProvider serviceProvider, IOptions<FastDFSOption> options, IClusterSelector clusterSelector)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _option = options.Value;
            _clusterSelector = clusterSelector;
            _clusterDict = new ConcurrentDictionary<string, ICluster>();
        }

        /// <summary>Get cluster by name
        /// </summary>
        public ICluster Get(string name)
        {
            //获取默认?
            if (string.IsNullOrWhiteSpace(name))
            {
                if (_option.ClusterConfigurations.Count == 1)
                {
                    name = _option.ClusterConfigurations.FirstOrDefault().Name;
                }
            }

            if (_clusterDict.TryGetValue(name, out ICluster cluster))
            {
                return cluster;
            }

            var configuration = _clusterSelector.Get(name);
            cluster = CreateCluster(configuration);

            if (!_clusterDict.TryAdd(name, cluster))
            {
                _logger.LogWarning("Could not add cluster by name '{0}'.", name);
            }
            return cluster;
        }


        protected ICluster CreateCluster(ClusterConfiguration configuration)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var injectConfiguration = scope.ServiceProvider.GetService<ClusterConfiguration>();

                injectConfiguration.Name = configuration.Name;
                injectConfiguration.Trackers = configuration.Trackers;
                injectConfiguration.ConnectionTimeout = configuration.ConnectionTimeout;
                injectConfiguration.Charset = configuration.Charset;
                injectConfiguration.ConnectionLifeTime = configuration.ConnectionLifeTime;
                injectConfiguration.ConnectionConcurrentThread = configuration.ConnectionConcurrentThread;
                injectConfiguration.ScanTimeoutConnectionInterval = configuration.ScanTimeoutConnectionInterval;
                injectConfiguration.TrackerMaxConnection = configuration.TrackerMaxConnection;
                injectConfiguration.StorageMaxConnection = configuration.StorageMaxConnection;

                return scope.ServiceProvider.GetService<ICluster>();

            }
        }

    }
}
