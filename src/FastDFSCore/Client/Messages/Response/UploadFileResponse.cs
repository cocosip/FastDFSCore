using System;
using System.Collections.Generic;
using System.Text;

namespace FastDFSCore.Client
{
    /// <summary>上传文件返回
    /// </summary>
    public class UploadFileResponse : FDFSResponse
    {
        public string GroupName { get; set; }

        public string FileId { get; set; }

        public UploadFileResponse()
        {

        }
        public UploadFileResponse(string groupName, string fileId)
        {
            GroupName = groupName;
            FileId = fileId;
        }
    }
}
