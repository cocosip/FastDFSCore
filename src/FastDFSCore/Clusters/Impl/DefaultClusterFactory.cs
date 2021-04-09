using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace FastDFSCore
{
    public class DefaultClusterFactory : IClusterFactory
    {

        private readonly ConcurrentDictionary<string, ICluster> _clusterDict;

        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly FastDFSOptions _options;
        private readonly IClusterSelector _clusterSelector;

        public DefaultClusterFactory(
            ILogger<DefaultClusterFactory> logger,
            IServiceProvider serviceProvider,
            IOptions<FastDFSOptions> options,
            IClusterSelector clusterSelector)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _options = options.Value;
            _clusterSelector = clusterSelector;
            _clusterDict = new ConcurrentDictionary<string, ICluster>();
        }

        /// <summary>Get cluster by name
        /// </summary>
        public ICluster Get(string name)
        {
            //ensure there are only one configuration in option
            if (string.IsNullOrWhiteSpace(name))
            {
                if (_options.ClusterConfigurations.Count == 1)
                {
                    name = _options.ClusterConfigurations.FirstOrDefault().Name;
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

        /// <summary>
        /// Get all clusters
        /// </summary>
        /// <returns></returns>
        public List<ICluster> GetClusters()
        {
            return _clusterDict.Values.ToList();
        }

        /// <summary>
        /// Release all clusters and cluster connection pool
        /// </summary>
        public void Release()
        {
            foreach (var kv in _clusterDict)
            {
                kv.Value.Release();
            }
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

                injectConfiguration.AntiStealToken = configuration.AntiStealToken;
                injectConfiguration.SecretKey = configuration.SecretKey;

                injectConfiguration.TrackerMaxConnection = configuration.TrackerMaxConnection;
                injectConfiguration.StorageMaxConnection = configuration.StorageMaxConnection;

                return scope.ServiceProvider.GetService<ICluster>();

            }
        }

    }
}
