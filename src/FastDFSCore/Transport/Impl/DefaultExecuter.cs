using FastDFSCore.Protocols;
using System;
using System.Threading.Tasks;

namespace FastDFSCore.Transport
{
    /// <summary>请求执行器
    /// </summary>
    public class DefaultExecuter : IExecuter
    {
        private readonly IClusterFactory _clusterFactory;

        /// <summary>Ctor
        /// </summary>
        public DefaultExecuter(IClusterFactory clusterFactory)
        {
            _clusterFactory = clusterFactory;
        }

        /// <summary>请求执行器
        /// </summary>
        /// <typeparam name="T">请求的类型<see cref="FastDFSReq"/></typeparam>
        /// <param name="request">请求</param>
        /// <param name="clusterName">集群名</param>
        /// <param name="connectionAddress">返回</param>
        /// <returns></returns>
        public async ValueTask<T> Execute<T>(FastDFSReq<T> request, string clusterName, ConnectionAddress connectionAddress = null) where T : FastDFSResp, new()
        {
            var cluster = _clusterFactory.Get(clusterName);

            var connection = connectionAddress == null ? cluster.GetTrackerConnection() : cluster.GetStorageConnection(connectionAddress);
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
