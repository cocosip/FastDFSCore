//using FastDFSCore.Scheduling;
//using FastDFSCore.Utility;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Concurrent;
//using System.Threading;
//using System.Threading.Tasks;

//namespace FastDFSCore.Transport
//{
//    /// <summary>连接池
//    /// </summary>
//    public class Pool
//    {
//        /// <summary>连接池名称
//        /// </summary>
//        public string Name { get; }
//        private bool _isRunning = false;
//        private int _currentConnectionCount;

//        private readonly PoolOption _poolOption;

//        private readonly AutoResetEvent _autoResetEvent;

//        private readonly ILogger _logger;
//        private readonly IScheduleService _scheduleService;
//        private readonly IConnectionBuilder _connectionBuilder;
//        private readonly ConcurrentStack<IConnection> _connections = new ConcurrentStack<IConnection>();

//        /// <summary>Ctor
//        /// </summary>
//        public Pool(ILogger<Pool> logger, IScheduleService scheduleService, IConnectionBuilder connectionBuilder, PoolOption poolOption)
//        {
//            _logger = logger;
//            _scheduleService = scheduleService;
//            _connectionBuilder = connectionBuilder;


//            _autoResetEvent = new AutoResetEvent(false);
//            _currentConnectionCount = 0;

//            _poolOption = poolOption;

//            Name = Guid.NewGuid().ToString();
//        }

//        /// <summary>获取一个连接
//        /// </summary>
//        public async Task<IConnection> GetConnection()
//        {
//            //如果连接为空,则创建新的
//            if (_connections.IsEmpty)
//            {
//                _logger.LogDebug("Current connections is empty! CurrentConnectionCount:{0},MaxConnection:{1}", _currentConnectionCount, _poolOption.MaxConnection);
//                //取不到连接,判断是否还可以创建新的连接,有可能这些连接正在被占用
//                if (_currentConnectionCount < _poolOption.MaxConnection)
//                {
//                    //还可以创建新的连接
//                    return CreateNewConnection();
//                }
//                //无法创建新的连接,只能等待
//                _autoResetEvent.WaitOne(5000);
//            }
//            //获取连接
//            if (!_connections.TryPop(out IConnection connection))
//            {
//                throw new Exception($"无法获取新连接,当前Pool '{_poolOption.EndPoint.ToStringAddress()}'.");
//            }

//            //判断连接是否过期
//            if (IsConnectionExpired(connection))
//            {
//                await RemoveConnection(connection);
//                return await GetConnection();
//            }
//            return connection;
//        }
//        /// <summary>创建新的连接
//        /// </summary>
//        private IConnection CreateNewConnection()
//        {
//            var connectionAddress = new ConnectionAddress()
//            {
//                //ServerEndPoint = _poolOption.EndPoint
//            };

//            var connection = _connectionBuilder.CreateConnection(connectionAddress);
//            Interlocked.Increment(ref _currentConnectionCount);
//            return connection;
//        }

//        private async Task RemoveConnection(IConnection connection)
//        {
//            //关闭连接内的数据
//            await connection.ShutdownAsync();
//            //更新当前连接数
//            if (_currentConnectionCount > 0)
//            {
//                Interlocked.Decrement(ref _currentConnectionCount);
//            }
//        }

//        /// <summary>连接关闭,将连接放会堆栈
//        /// </summary>
//        public void ConnectionClose(IConnection connection)
//        {
//            if (connection != null)
//            {
//                _connections.Push(connection);
//            }
//            _autoResetEvent.Set();
//            _logger.LogDebug("Release a connection,connection name:{0}.", connection.Name);
//        }

//        /// <summary>判断连接是否已经过期
//        /// </summary>
//        private bool IsConnectionExpired(IConnection connection)
//        {
//            return (connection.LastUseTime != default) && ((DateTime.Now - connection.LastUseTime).TotalSeconds > _poolOption.ConnectionLifeTime);
//        }

//        /// <summary>搜索超时的连接,将会断开
//        /// </summary>
//        private void StartScanTimeoutConnectionTask()
//        {
//            _scheduleService.StartTask($"{_poolOption.EndPoint.ToStringAddress()}.{GetType().Name}.ScanTimeoutConnection", ScanTimeoutConnection, _poolOption.ScanTimeoutConnectionInterval * 1000, 1000);
//        }

//        /// <summary>停止搜索超时的连接
//        /// </summary>
//        private void StopScanTimeoutConnectionTask()
//        {
//            _scheduleService.StopTask($"{_poolOption.EndPoint.ToStringAddress()}.{GetType().Name}.ScanTimeoutConnection");
//        }

//        private void ScanTimeoutConnection()
//        {
//            foreach (var connection in _connections)
//            {
//                if (IsConnectionExpired(connection))
//                {
//                    //释放连接
//                    connection.DisposeAsync().Wait();
//                }
//            }
//        }

//        /// <summary>开始运行
//        /// </summary>
//        public void Start()
//        {

//            if (_isRunning)
//            {
//                return;
//            }
//            _isRunning = true;
//            StartScanTimeoutConnectionTask();
//        }

//        /// <summary>关闭连接池
//        /// </summary>
//        public void Shutdown()
//        {
//            if (_isRunning)
//            {
//                StopScanTimeoutConnectionTask();
//                _isRunning = false;
//            }
//            foreach (var connection in _connections)
//            {
//                connection.DisposeAsync().Wait();
//            }
//        }


//    }
//}
