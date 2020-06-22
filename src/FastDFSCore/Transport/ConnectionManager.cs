using FastDFSCore.Utility;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace FastDFSCore.Transport
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
        /// <param name="logger">日志</param>
        /// <param name="connectionPoolFactory">连接池工厂</param>
        /// <param name="option">FDFSOption</param>
        public ConnectionManager(ILogger<ConnectionManager> logger, IConnectionPoolFactory connectionPoolFactory, IOptions<FDFSOption> option)
        {
            _logger = logger;
            _connectionPoolFactory = connectionPoolFactory;
            _option = option.Value;

        }

        /// <summary>获取Tracker的连接
        /// </summary>
        public async Task<IConnection> GetTrackerConnection()
        {
            var rd = new Random();
            var index = rd.Next(_trackerPools.Count);
            var trackerPool = _trackerPools[_trackerEndPoints[index]];
            return await trackerPool.GetConnection();
        }

        /// <summary>获取Storage连接
        /// </summary>
        public async Task<IConnection> GetStorageConnection(IPEndPoint endPoint)
        {
            Pool storagePool;
            lock (SyncObject)
            {
                if (!_storagePools.TryGetValue(endPoint, out storagePool))
                {
                    var poolOption = new PoolOption()
                    {
                        EndPoint = endPoint,
                        MaxConnection = _option.StorageMaxConnection,
                        ConnectionLifeTime = _option.ConnectionLifeTime,
                        ScanTimeoutConnectionInterval = _option.ScanTimeoutConnectionInterval
                    };

                    storagePool = _connectionPoolFactory.CreatePool(poolOption);
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
                var poolOption = new PoolOption()
                {
                    EndPoint = trackerEndPoint,
                    MaxConnection = _option.TrackerMaxConnection,
                    ConnectionLifeTime = _option.ConnectionLifeTime,
                    ScanTimeoutConnectionInterval = _option.ScanTimeoutConnectionInterval
                };

                var pool = _connectionPoolFactory.CreatePool(poolOption);
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
