﻿using DotNetty.Common.Internal.Logging;
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
        private ConcurrentDictionary<IPEndPoint, Pool> _trackerPools = new ConcurrentDictionary<IPEndPoint, Pool>();
        private ConcurrentDictionary<IPEndPoint, Pool> _storagePools = new ConcurrentDictionary<IPEndPoint, Pool>();

        private readonly ILogger _logger;
        private readonly object syncObject = new object();
        private readonly IConnectionPoolFactory _connectionPoolFactory;
        private readonly List<IPEndPoint> _trackerEndPoints = new List<IPEndPoint>();
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
            _trackerEndPoints = _option.Trackers;
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
            lock (syncObject)
            {
                if (!_storagePools.TryGetValue(endPoint, out storagePool))
                {
                    storagePool = _connectionPoolFactory.CreatePool(endPoint, _option.StorageMaxConnection, _option.ConnectionLifeTime);
                    _storagePools.TryAdd(endPoint, storagePool);
                }
            }
            return await storagePool.GetConnection();
        }

        /// <summary>运行
        /// </summary>
        public void Start()
        {
            foreach (var trackerEndPoint in _option.Trackers)
            {
                var pool = _connectionPoolFactory.CreatePool(trackerEndPoint, _option.TrackerMaxConnection, _option.ConnectionLifeTime);
                _trackerPools.TryAdd(trackerEndPoint, pool);
            }
        }

        /// <summary>关闭
        /// </summary>
        public void Shutdown()
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
    }
}
