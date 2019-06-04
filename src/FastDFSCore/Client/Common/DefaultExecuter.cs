using System.Net;
using System.Threading.Tasks;

namespace FastDFSCore.Client
{
    public class DefaultExecuter : IExecuter
    {
        private IConnectionManager _connectionManager;
        public DefaultExecuter(IConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        public async Task<T> Execute<T>(FDFSRequest<T> request, IPEndPoint endPoint = null) where T : FDFSResponse, new()
        {
            var connection = endPoint == null ? await _connectionManager.GetTrackerConnection() : await _connectionManager.GetStorageConnection(endPoint);
            await connection.OpenAsync();
            var response = await connection.SendRequestAsync<T>(request);
            await connection.CloseAsync();
            return response as T;
        }
    }
}
