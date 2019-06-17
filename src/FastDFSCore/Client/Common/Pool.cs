using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FastDFSCore.Client
{
    public class Pool
    {
        private int _connectionLifeTime;
        private int _currentConnectionCount;
        public IPEndPoint EndPoint { get; }
        public int MaxConnection { get; }
        private ConcurrentStack<Connection> _connections = new ConcurrentStack<Connection>();

        private readonly IConnectionPoolFactory _connectionPoolFactory;

        public Pool(IConnectionPoolFactory connectionPoolFactory, IPEndPoint endPoint, int maxConnection, int connectionLifeTime)
        {
            _connectionPoolFactory = connectionPoolFactory;
            EndPoint = endPoint;
            MaxConnection = maxConnection;
            _currentConnectionCount = 0;
            _connectionLifeTime = connectionLifeTime;
        }

        /// <summary>获取一个连接
        /// </summary>
        public async Task<Connection> GetConnection()
        {
            Connection connection;
            if (!_connections.TryPop(out connection))
            {
                //取不到连接,判断是否还可以创建新的连接,有可能这些连接正在被占用
                if (_currentConnectionCount < MaxConnection)
                {
                    //还可以创建新的连接
                    connection = CreateNewConnection();
                    //新建的放入堆栈中
                    _connections.Push(connection);
                }
            }
            //无连接可用了
            if (connection == null)
            {
                throw new ArgumentOutOfRangeException($"无可用的连接,连接地址:{EndPoint.Address}:{EndPoint.Port}");
            }
            //判断连接是否过期
            if (IsConnectionExpired(connection.LastUseTime))
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
            var setting = new ConnectionSetting()
            {
                ServerEndPoint = EndPoint
            };

            var connection = _connectionPoolFactory.CreateConnection(setting, ConnectionClose);
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


        private void ConnectionClose(Connection connection)
        {
            _connections.Push(connection);
        }

        /// <summary>判断连接是否已经过期
        /// </summary>
        private bool IsConnectionExpired(DateTime lastUseTime)
        {
            return (lastUseTime != default(DateTime)) && ((DateTime.Now - lastUseTime).TotalSeconds > _connectionLifeTime);
        }


        public void Shutdown()
        {
            foreach (var connection in _connections)
            {
                connection.DisposeAsync().Wait();
            }
        }


    }
}
