using System.Collections.Generic;
using System.Threading.Tasks;

namespace FastDFSCore.Sample
{
    public interface ISampleAppService
    {
        string GetToken(string fileId);

        /// <summary>查询Storage信息
        /// </summary>
        Task ListStorageInfosAsync(string groupName);

        Task<List<string>> BatchUploadAsync(string groupName, string directoryPath);

        Task<List<string>> BatchDownloadAsync(string groupName, List<string> fileIds, string saveDirectory);
    }
}
