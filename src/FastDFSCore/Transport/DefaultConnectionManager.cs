using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace FastDFSCore.Transport
{
    public class ConnectionManager : IConnectionManager
    {
        private readonly ConcurrentDictionary<ConnectionAddress, IConnectionPool> _trackerConnectionPools;
        private readonly ConcurrentDictionary<ConnectionAddress, IConnectionPool> _storageConnectionPools;
        private readonly List<ConnectionAddress> _trackerConnectionAddresses;

        private bool _isInitialized = false;
        private readonly object _trackerSyncObject = new object();
        private readonly object _storageSyncObject = new object();

        private readonly ILogger _logger;
        private readonly IConnectionPoolBuilder _connectionPoolFactory;
        private readonly FastDFSOption _option;

        public ConnectionManager(ILogger<ConnectionManager> logger, IConnectionPoolBuilder connectionPoolFactory, IOptions<FastDFSOption> option)
        {
            _logger = logger;
            _connectionPoolFactory = connectionPoolFactory;
            _option = option.Value;

            _trackerConnectionAddresses = new List<ConnectionAddress>();
            _trackerConnectionPools = new ConcurrentDictionary<ConnectionAddress, IConnectionPool>();
            _storageConnectionPools = new ConcurrentDictionary<ConnectionAddress, IConnectionPool>();
        }

        /// <summary>Get tracker connection
        /// </summary>
        public IConnection GetTrackerConnection()
        {
            var rd = new Random();
            var index = rd.Next(_trackerConnectionAddresses.Count);
            var connectionAddress = _trackerConnectionAddresses[index];

            if (_trackerConnectionPools.TryGetValue(connectionAddress, out IConnectionPool connectionPool))
            {
                lock (_trackerSyncObject)
                {
                    if (!_trackerConnectionPools.TryGetValue(connectionAddress, out connectionPool))
                    {

                        var connectionPoolOption = new ConnectionPoolOption()
                        {
                            ConnectionAddress = connectionAddress,
                            ConnectionLifeTime = _option.ConnectionLifeTime,
                            ConnectionConcurrentThread = _option.ConnectionConcurrentThread,
                            MaxConnection = _option.TrackerMaxConnection,
                            ScanTimeoutConnectionInterval = _option.ScanTimeoutConnectionInterval
                        };

                        connectionPool = _connectionPoolFactory.CreateConnectionPool(connectionPoolOption);
                        if (!_trackerConnectionPools.TryAdd(connectionAddress, connectionPool))
                        {
                            _logger.LogWarning("Fail to add connection pool to tracker connection pools! ConnectionAddress:{0}", connectionAddress);
                        }
                    }
                }
            }

            return connectionPool.Get();
        }

        /// <summary>获取Storage连接
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
                            ConnectionLifeTime = _option.ConnectionLifeTime,
                            ConnectionConcurrentThread = _option.ConnectionConcurrentThread,
                            MaxConnection = _option.StorageMaxConnection,
                            ScanTimeoutConnectionInterval = _option.ScanTimeoutConnectionInterval
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
            return connectionPool.Get();
        }

        public void Initialize()
        {
            if (_isInitialized)
            {
                _logger.LogDebug("ConnectionManager is already initialized!");
                return;
            }

            foreach (var tracker in _option.Trackers)
            {
                var connectionAddress = new ConnectionAddress(tracker.IPAddress, tracker.Port);
                _trackerConnectionAddresses.Add(connectionAddress);
            }

            _logger.LogDebug("'ConnectionManager' initialize for '{0}' ConnectionAddress,[{1}]", _trackerConnectionAddresses.Count, string.Join(",", _trackerConnectionAddresses));

            _isInitialized = true;
        }

        /// <summary>关闭
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
