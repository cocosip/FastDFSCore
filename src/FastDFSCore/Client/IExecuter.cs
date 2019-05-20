using System.Net;
using System.Threading.Tasks;

namespace FastDFSCore.Client
{
    public interface IExecuter
    {
        Task<T> Execute<T>(FDFSRequest<T> request, IPEndPoint iPEndPoint = null) where T : FDFSResponse;
    }
}
