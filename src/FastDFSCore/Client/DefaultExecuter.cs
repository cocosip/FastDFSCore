using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FastDFSCore.Client
{
    public class DefaultExecuter : IExecuter
    {


        public async Task<T> Execute<T>(FDFSRequest<T> request, IPEndPoint iPEndPoint = null) where T : FDFSResponse
        {
            return await Task.FromResult(default(T));
        }
    }
}
