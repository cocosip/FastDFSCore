using FastDFSCore.Protocols;
using System.Threading.Tasks;

namespace FastDFSCore.Transport
{
    /// <summary>请求执行器
    /// </summary>
    public interface IExecuter
    {
        /// <summary>请求执行器
        /// </summary>
        /// <typeparam name="T">请求的类型<see cref="FastDFSCore.Protocols.FastDFSReq"/></typeparam>
        /// <param name="request">请求</param>
        /// <param name="clusterName">集群名</param>
        /// <param name="connectionAddress">返回</param>
        /// <returns></returns>
        ValueTask<T> Execute<T>(FastDFSReq<T> request, string clusterName, ConnectionAddress connectionAddress = null) where T : FastDFSResp, new();
    }
}
