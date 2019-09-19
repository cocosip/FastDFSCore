using DotNetty.Common.Internal.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace FastDFSCore.Client
{
    /// <summary>连接管理器
    /// </summary>
    public class ConnectionManager : IConnectionManager
    {
        private readonly ConcurrentDictionary<IPEndPoint, Pool> _trackerPools = new ConcurrentDictionary<IPEndPoint, Pool>();
        private readonly ConcurrentDictionary<IPEndPoint, Pool> _storagePools = new ConcurrentDictionary<IPEndPoint, Pool>();

        private bool _isRunning = false;
        private readonly ILogger _logger;
        private readonly object SyncObject = new object();
        private readonly IConnectionPoolFactory _connectionPoolFactory;
        private List<IPEndPoint> _trackerEndPoints = new List<IPEndPoint>();
        private readonly FDFSOption _option;

        /// <summary>Ctor
        /// </summary>
        /// <param name="connectionPoolFactory">连接池工厂</param>
        /// <param name="option">FDFSOption</param>
        public ConnectionManager(IConnectionPoolFactory connectionPoolFactory, FDFSOption option)
        {
            _logger = InternalLoggerFactory.DefaultFactory.CreateLogger(option.LoggerName);
            _connectionPoolFactory = connectionPoolFactory;
            _option = option;

        }

        /// <summary>获取Tracker的连接
        /// </summary>
        public async Task<Connection> GetTrackerConnection()
        {
            var rd = new Random();
            var index = rd.Next(_trackerPools.Count);
            var trackerPool = _trackerPools[_trackerEndPoints[index]];
            return await trackerPool.GetConnection();
        }

        /// <summary>获取Storage连接
        /// </summary>
        public async Task<Connection> GetStorageConnection(IPEndPoint endPoint)
        {
            Pool storagePool;
            lock (SyncObject)
            {
                if (!_storagePools.TryGetValue(endPoint, out storagePool))
                {
                    storagePool = _connectionPoolFactory.CreatePool(endPoint, _option.StorageMaxConnection, _option.ConnectionLifeTime, _option.ScanTimeoutConnectionInterval);
                    storagePool.Start();
                    _storagePools.TryAdd(endPoint, storagePool);
                    _logger.LogDebug("_storagePools 中不存在连接池:{0}", endPoint.ToStringAddress());
                }
            }
            return await storagePool.GetConnection();
        }

        /// <summary>运行
        /// </summary>
        public void Start()
        {
            if (_isRunning)
            {
                return;
            }
            _trackerEndPoints = _option.Trackers;
            foreach (var trackerEndPoint in _option.Trackers)
            {
                var pool = _connectionPoolFactory.CreatePool(trackerEndPoint, _option.TrackerMaxConnection, _option.ConnectionLifeTime, _option.ScanTimeoutConnectionInterval);
                pool.Start();
                _trackerPools.TryAdd(trackerEndPoint, pool);
            }
            _isRunning = true;
        }

        /// <summary>关闭
        /// </summary>
        public void Shutdown()
        {
            if (_isRunning)
            {
                foreach (var item in _trackerPools)
                {
                    item.Value.Shutdown();
                }
                foreach (var item in _storagePools)
                {
                    item.Value.Shutdown();
                }
            }
            _isRunning = false;
        }
    }
}
