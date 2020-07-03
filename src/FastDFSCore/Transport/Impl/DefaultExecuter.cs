using FastDFSCore.Protocols;
using System;
using System.Threading.Tasks;

namespace FastDFSCore.Transport
{
    /// <summary>请求执行器
    /// </summary>
    public class DefaultExecuter : IExecuter
    {
        private readonly IConnectionManager _connectionManager;

        /// <summary>Ctor
        /// </summary>
        /// <param name="connectionManager">连接管理器</param>
        public DefaultExecuter(IConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        /// <summary>请求执行器
        /// </summary>
        /// <typeparam name="T">请求的类型<see cref="FastDFSCore.Protocols.FastDFSReq"/></typeparam>
        /// <param name="request">请求</param>
        /// <param name="connectionAddress">返回</param>
        /// <returns></returns>
        public async ValueTask<T> Execute<T>(FastDFSReq<T> request, ConnectionAddress connectionAddress = null) where T : FastDFSResp, new()
        {
            var connection = connectionAddress == null ? _connectionManager.GetTrackerConnection() : _connectionManager.GetStorageConnection(connectionAddress);
            if (connection == null)
            {
                throw new NullReferenceException($"Can't find connection,ipaddr:{connectionAddress} ");
            }
            await connection.OpenAsync();
            var response = await connection.SendRequestAsync<T>(request);
            await connection.CloseAsync();
            return response as T;
        }
    }
}
