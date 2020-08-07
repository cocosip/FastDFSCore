using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace FastDFSCore.Transport
{
    public class DefaultConnectionPool : IConnectionPool
    {
        public string Name { get; }
        public ConnectionAddress ConnectionAddress { get { return _option.ConnectionAddress; } }

        private readonly ILogger _logger;
        private readonly ConnectionPoolOption _option;
        private readonly IConnectionBuilder _connectionFactory;


        private CancellationTokenSource _cts;

        private bool _isRunning = false;
        private int _connectionCount;
        private readonly int _maxConnectionCount;
        private readonly SemaphoreSlim _semaphoreSlim;
        private readonly ConcurrentStack<IConnection> _connectionStack;
        private readonly ConcurrentDictionary<string, IConnection> _connectionDict;

        public DefaultConnectionPool(ILogger<DefaultConnectionPool> logger, ConnectionPoolOption option, IConnectionBuilder connectionFactory)
        {
            _logger = logger;
            _option = option;
            _connectionFactory = connectionFactory;

            Name = Guid.NewGuid().ToString();
            _connectionCount = 0;
            _cts = new CancellationTokenSource();

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
                    connection.OnDisconnect += DisconnectHandler;

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

            if (connection.IsExpired() && !connection.IsUsing)
            {
                Interlocked.Decrement(ref _connectionCount);
                if (!_connectionDict.TryRemove(connection.Id, out IConnection _))
                {
                    _logger.LogWarning("Connection is expired,but remove from dict fail!");
                }
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

            StartScanTimeoutConnection();
            _isRunning = true;
        }

        public void Shutdown()
        {
            if (!_isRunning)
            {
                _logger.LogDebug("Connection pool '{0}' is already in closed!", ConnectionAddress);
                return;
            }

            _cts.Cancel();

            //Dispose connecton
            DisposeAllConnections();
            _isRunning = false;
        }

        private void StartScanTimeoutConnection()
        {
            Task.Factory.StartNew(async () =>
            {
                await Task.Delay(1000);

                while (!_cts.IsCancellationRequested)
                {
                    try
                    {
                        foreach (var connection in _connectionStack)
                        {
                            if (connection.IsExpired() && !connection.IsUsing)
                            {
                                await connection.DisconnectAsync();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "ScanTimeoutConnection caught some exception! {0}", ex.Message);
                    }
                    await Task.Delay(_option.ScanTimeoutConnectionInterval * 1000);
                }

            }, TaskCreationOptions.LongRunning);
      
        }

        private void DisposeAllConnections()
        {
            Task.Run(async () =>
            {
                foreach (var connection in _connectionDict.Values)
                {
                    await connection.DisconnectAsync();
                }

                foreach (var connection in _connectionStack)
                {
                    await connection.DisconnectAsync();
                }
            });
        }


        private void ConnectionCloseHandler(object sender, ConnectionCloseEventArgs e)
        {
            var connection = (IConnection)sender;
            Return(connection);
        }

        private void DisconnectHandler(object sender, DisconnectEventArgs e)
        {
            var connection = (IConnection)sender;
            connection.OnConnectionClose -= ConnectionCloseHandler;
            connection.OnDisconnect -= DisconnectHandler;
            Interlocked.Decrement(ref _connectionCount);
        }


    }
}
