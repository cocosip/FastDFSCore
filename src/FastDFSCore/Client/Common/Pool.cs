using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FastDFSCore.Client
{
    /// <summary>连接池
    /// </summary>
    public class Pool
    {
        private bool _isRunning = false;
        private readonly IPEndPoint _endPoint;
        private readonly int _maxConnection;
        private readonly int _connectionLifeTime;
        private readonly int _scanTimeoutConnectionInterval;
        private int _currentConnectionCount;

        private readonly SemaphoreSlim _semaphoreSlim;
        private readonly IScheduleService _scheduleService;
        private readonly IConnectionPoolFactory _connectionPoolFactory;
        private readonly ConcurrentStack<Connection> _connections = new ConcurrentStack<Connection>();

        /// <summary>Ctor
        /// </summary>
        public Pool(IScheduleService scheduleService, IConnectionPoolFactory connectionPoolFactory, IPEndPoint endPoint, int maxConnection, int connectionLifeTime, int scanTimeoutConnectionInterval)
        {
            _scheduleService = scheduleService;
            _connectionPoolFactory = connectionPoolFactory;
            _endPoint = endPoint;
            _maxConnection = maxConnection;

            _semaphoreSlim = new SemaphoreSlim(1, maxConnection);

            _currentConnectionCount = 0;
            _connectionLifeTime = connectionLifeTime;
            _scanTimeoutConnectionInterval = scanTimeoutConnectionInterval;
        }

        /// <summary>获取一个连接
        /// </summary>
        public async Task<Connection> GetConnection()
        {
            //如果连接为空,则创建新的
            if (_connections.IsEmpty)
            {
                //取不到连接,判断是否还可以创建新的连接,有可能这些连接正在被占用
                if (_currentConnectionCount < _maxConnection)
                {
                    //还可以创建新的连接
                    return CreateNewConnection();
                }
                //无法创建新的连接,只能等待
                await _semaphoreSlim.WaitAsync();
            }
            //获取连接
            if (!_connections.TryPop(out Connection connection))
            {
                throw new Exception($"无法获取新连接,当前Pool:{_endPoint.Address}@{_endPoint.Port}");
            }

            //判断连接是否过期
            if (IsConnectionExpired(connection))
            {
                await RemoveConnection(connection);
                return await GetConnection();
            }
            return connection;
        }
        /// <summary>创建新的连接
        /// </summary>
        private Connection CreateNewConnection()
        {
            var connectionAddress = new ConnectionAddress()
            {
                ServerEndPoint = _endPoint
            };

            var connection = _connectionPoolFactory.CreateConnection(connectionAddress, ConnectionClose);
            Interlocked.Increment(ref _currentConnectionCount);
            return connection;
        }

        private async Task RemoveConnection(Connection connection)
        {
            //关闭连接内的数据
            await connection.ShutdownAsync();
            //更新当前连接数
            Interlocked.Decrement(ref _currentConnectionCount);
        }

        /// <summary>连接关闭,将连接放会堆栈
        /// </summary>
        private void ConnectionClose(Connection connection)
        {
            if (connection != null)
            {
                _connections.Push(connection);
                _semaphoreSlim.Release();
            }
        }

        /// <summary>判断连接是否已经过期
        /// </summary>
        private bool IsConnectionExpired(Connection connection)
        {
            return (connection.LastUseTime != default) && ((DateTime.Now - connection.LastUseTime).TotalSeconds > _connectionLifeTime);
        }

        /// <summary>搜索超时的连接,将会断开
        /// </summary>
        private void StartScanTimeoutConnectionTask()
        {
            _scheduleService.StartTask($"{_endPoint.ToStringAddress()}.{GetType().Name}.ScanTimeoutConnection", ScanTimeoutConnection, _scanTimeoutConnectionInterval * 1000, 1000);
        }

        /// <summary>停止搜索超时的连接
        /// </summary>
        private void StopScanTimeoutConnectionTask()
        {
            _scheduleService.StopTask($"{_endPoint.ToStringAddress()}.{GetType().Name}.ScanTimeoutConnection");
        }

        private void ScanTimeoutConnection()
        {
            foreach (var connection in _connections)
            {
                if (IsConnectionExpired(connection))
                {
                    connection.DisposeAsync().Wait();
                }
            }
        }

        /// <summary>开始运行
        /// </summary>
        public void Start()
        {

            if (_isRunning)
            {
                return;
            }
            _isRunning = true;
            StartScanTimeoutConnectionTask();
        }

        /// <summary>关闭连接池
        /// </summary>
        public void Shutdown()
        {
            if (_isRunning)
            {
                StopScanTimeoutConnectionTask();
                _isRunning = false;
            }
            foreach (var connection in _connections)
            {
                connection.DisposeAsync().Wait();
            }
        }


    }
}
