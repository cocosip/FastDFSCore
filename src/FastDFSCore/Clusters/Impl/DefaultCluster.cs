using FastDFSCore.Transport;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;

namespace FastDFSCore
{
    public class DefaultCluster : ICluster
    {
        private readonly ConcurrentDictionary<ConnectionAddress, IConnectionPool> _trackerConnectionPools;
        private readonly ConcurrentDictionary<ConnectionAddress, IConnectionPool> _storageConnectionPools;

        private readonly object _trackerSyncObject = new();
        private readonly object _storageSyncObject = new();

        private readonly ILogger _logger;
        private readonly IConnectionPoolBuilder _connectionPoolFactory;
        private readonly ClusterConfiguration _configuration;

        /// <summary>
        /// Cluster name
        /// </summary>
        public string Name { get { return _configuration?.Name; } }

        public DefaultCluster(
            ILogger<DefaultCluster> logger, 
            IConnectionPoolBuilder connectionPoolFactory,
            ClusterConfiguration configuration)
        {
            _logger = logger;
            _connectionPoolFactory = connectionPoolFactory;
            _configuration = configuration;

            _trackerConnectionPools = new ConcurrentDictionary<ConnectionAddress, IConnectionPool>();
            _storageConnectionPools = new ConcurrentDictionary<ConnectionAddress, IConnectionPool>();
        }

        /// <summary>
        /// 获取集群的配置信息
        /// </summary>
        /// <returns></returns>
        public ClusterConfiguration GetConfiguration()
        {
            return _configuration;
        }

        /// <summary>
        /// Get tracker connection
        /// </summary>
        public IConnection GetTrackerConnection()
        {
            var rd = new Random();
            var index = rd.Next(_configuration.Trackers.Count);
            var tracker = _configuration.Trackers[index];
            var connectionAddress = new ConnectionAddress(tracker.IPAddress, tracker.Port);

            if (!_trackerConnectionPools.TryGetValue(connectionAddress, out IConnectionPool connectionPool))
            {
                lock (_trackerSyncObject)
                {
                    if (!_trackerConnectionPools.TryGetValue(connectionAddress, out connectionPool))
                    {

                        var connectionPoolOption = new ConnectionPoolOption()
                        {
                            ConnectionAddress = connectionAddress,
                            ConnectionLifeTime = _configuration.ConnectionLifeTime,
                            ConnectionConcurrentThread = _configuration.ConnectionConcurrentThread,
                            MaxConnection = _configuration.TrackerMaxConnection,
                            ScanTimeoutConnectionInterval = _configuration.ScanTimeoutConnectionInterval
                        };

                        connectionPool = _connectionPoolFactory.CreateConnectionPool(connectionPoolOption);
                        if (!_trackerConnectionPools.TryAdd(connectionAddress, connectionPool))
                        {
                            _logger.LogWarning("Fail to add connection pool to tracker connection pools! ConnectionAddress:{0}", connectionAddress);
                        }
                        else
                        {
                            connectionPool.Start();
                        }
                    }
                }
            }

            return connectionPool.GetConnection();
        }

        /// <summary>
        /// Get storage connection
        /// </summary>
        public IConnection GetStorageConnection(ConnectionAddress connectionAddress)
        {
            if (!_storageConnectionPools.TryGetValue(connectionAddress, out IConnectionPool connectionPool))
            {
                lock (_storageSyncObject)
                {
                    if (!_storageConnectionPools.TryGetValue(connectionAddress, out connectionPool))
                    {
                        var connectionPoolOption = new ConnectionPoolOption()
                        {
                            ConnectionAddress = connectionAddress,
                            ConnectionLifeTime = _configuration.ConnectionLifeTime,
                            ConnectionConcurrentThread = _configuration.ConnectionConcurrentThread,
                            MaxConnection = _configuration.StorageMaxConnection,
                            ScanTimeoutConnectionInterval = _configuration.ScanTimeoutConnectionInterval
                        };

                        connectionPool = _connectionPoolFactory.CreateConnectionPool(connectionPoolOption);
                        if (!_storageConnectionPools.TryAdd(connectionAddress, connectionPool))
                        {
                            _logger.LogWarning("Fail to add connection pool to storage connection pools! ConnectionAddress:{0}", connectionAddress);
                        }
                    }
                }
            }

            if (connectionPool == null)
            {
                throw new ArgumentException($"Can't find any connection pools for {connectionAddress}");
            }
            return connectionPool.GetConnection();
        }

        /// <summary>
        /// Release
        /// </summary>
        public void Release()
        {
            foreach (var item in _trackerConnectionPools)
            {
                item.Value.Shutdown();
            }

            foreach (var item in _storageConnectionPools)
            {
                item.Value.Shutdown();
            }

        }

    }
}
