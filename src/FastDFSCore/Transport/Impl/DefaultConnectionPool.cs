using FastDFSCore.Scheduling;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace FastDFSCore.Transport
{
    public class DefaultConnectionPool : IConnectionPool
    {
        public string Name { get; }
        public ConnectionAddress ConnectionAddress { get { return _option.ConnectionAddress; } }

        private readonly ILogger _logger;
        private readonly IScheduleService _scheduleService;
        private readonly ConnectionPoolOption _option;
        private readonly IConnectionBuilder _connectionFactory;


        private bool _isRunning = false;
        private int _connectionCount;
        private readonly int _maxConnectionCount;
        private readonly SemaphoreSlim _semaphoreSlim;
        private readonly ConcurrentStack<IConnection> _connectionStack;
        private readonly ConcurrentDictionary<string, IConnection> _connectionDict;

        public DefaultConnectionPool(ILogger<DefaultConnectionPool> logger, IScheduleService scheduleService, ConnectionPoolOption option, IConnectionBuilder connectionFactory)
        {
            _logger = logger;
            _scheduleService = scheduleService;
            _option = option;
            _connectionFactory = connectionFactory;


            Name = Guid.NewGuid().ToString();
            _connectionCount = 0;
            _maxConnectionCount = _option.MaxConnection;
            _semaphoreSlim = new SemaphoreSlim(_option.ConnectionConcurrentThread);
            _connectionStack = new ConcurrentStack<IConnection>();
            _connectionDict = new ConcurrentDictionary<string, IConnection>();
        }

        /// <summary>Get a connection from stack
        /// </summary>
        public IConnection GetConnection()
        {
            if (!_connectionStack.TryPop(out IConnection connection))
            {
                if (_connectionCount < _maxConnectionCount)
                {
                    connection = _connectionFactory.CreateConnection(_option.ConnectionAddress);
                    //BindEvent
                    connection.OnConnectionClose += ConnectionCloseHandler;
                    Interlocked.Increment(ref _connectionCount);

                    if (!_connectionDict.TryAdd(connection.Id, connection))
                    {
                        _logger.LogWarning("Fail add connection to dict! ConnectionAddress:{0}", ConnectionAddress);
                    }

                    return connection;
                }

                _logger.LogDebug("The connection stack is empty and created full, wait for get a return connection.Connection Pool Name '{0}',Server:'{1}'.", Name, _option.ConnectionAddress);
                _semaphoreSlim.Wait(5000);
                return GetConnection();
            }
            return connection;
        }


        /// <summary>Return the connection
        /// </summary>
        public void Return(IConnection connection)
        {
            if (connection != null)
            {
                _connectionStack.Push(connection);
            }
            _semaphoreSlim.Release();
            _logger.LogDebug("Return a connnection,{0}", connection.ConnectionAddress);
        }


        public void Start()
        {
            if (_isRunning)
            {
                _logger.LogDebug("Connection pool '{0}' is already in running!", ConnectionAddress);
                return;
            }
            StartScanTimeoutConnectionTask();
            _isRunning = true;
        }


        public void Shutdown()
        {
            if (!_isRunning)
            {
                _logger.LogDebug("Connection pool '{0}' is already in closed!", ConnectionAddress);
                return;
            }

            StopScanTimeoutConnectionTask();

            //Dispose connecton
            DisposeAllConnections();
            _isRunning = false;
        }


        /// <summary>搜索超时的连接,将会断开
        /// </summary>
        private void StartScanTimeoutConnectionTask()
        {
            _scheduleService.StartTask($"{_option.ConnectionAddress}.{GetType().Name}.ScanTimeoutConnection", ScanTimeoutConnection, _option.ScanTimeoutConnectionInterval * 1000, 1000);
        }

        /// <summary>停止搜索超时的连接
        /// </summary>
        private void StopScanTimeoutConnectionTask()
        {
            _scheduleService.StopTask($"{_option.ConnectionAddress}.{GetType().Name}.ScanTimeoutConnection");
        }

        private void ScanTimeoutConnection()
        {
            foreach (var connection in _connectionStack)
            {
                if (connection.IsExpired())
                {
                    connection.ShutdownAsync().Wait();
                }
            }
        }

        private void DisposeAllConnections()
        {
            foreach (var connection in _connectionDict.Values)
            {
                connection.ShutdownAsync().Wait();
            }
        }


        private void ConnectionCloseHandler(object sender, ConnectionCloseEventArgs e)
        {
            var connection = (IConnection)sender;
            Return(connection);

        }


    }
}
