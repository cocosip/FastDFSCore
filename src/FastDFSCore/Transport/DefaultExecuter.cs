using FastDFSCore.Protocols;
using FastDFSCore.Utility;
using System;
using System.Net;
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
        /// <param name="endPoint">返回</param>
        /// <returns></returns>
        public async Task<T> Execute<T>(FastDFSReq<T> request, IPEndPoint endPoint = null) where T : FastDFSResp, new()
        {
            var connection = endPoint == null ? await _connectionManager.GetTrackerConnection() : await _connectionManager.GetStorageConnection(endPoint);
            if (connection == null)
            {
                throw new NullReferenceException($"Can't find connection,ipaddr:[{endPoint.ToStringAddress()}] ");
            }
            await connection.OpenAsync();
            var response = await connection.SendRequestAsync<T>(request);
            await connection.CloseAsync();
            return response as T;
        }
    }
}
