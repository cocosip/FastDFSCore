using System;
using System.Collections.Generic;
using System.Text;

namespace FastDFSCore.Client
{
    public class DeleteFileResponse : FDFSResponse
    {
        public bool Success { get; set; }

        public DeleteFileResponse()
        {

        }
        public DeleteFileResponse(bool success)
        {
            Success = success;
        }
    }
}
