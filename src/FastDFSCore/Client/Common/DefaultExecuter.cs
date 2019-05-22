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
            connection.Open();

            var response = await connection.SendRequestAsync<T>(request);

            connection.Close();
            return response;
        }
    }
}
