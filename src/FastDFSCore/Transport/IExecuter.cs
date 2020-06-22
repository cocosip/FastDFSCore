using FastDFSCore.Protocols;
using System.Net;
using System.Threading.Tasks;

namespace FastDFSCore.Transport
{
    /// <summary>请求执行器
    /// </summary>
    public interface IExecuter
    {
        /// <summary>请求执行器
        /// </summary>
        /// <typeparam name="T">请求的类型<see cref="FastDFSCore.Protocols.FDFSRequest"/></typeparam>
        /// <param name="request">请求</param>
        /// <param name="endPoint">返回</param>
        /// <returns></returns>
        Task<T> Execute<T>(FDFSRequest<T> request, IPEndPoint endPoint = null) where T : FDFSResponse, new();
    }
}
