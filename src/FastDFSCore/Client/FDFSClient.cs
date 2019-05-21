using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FastDFSCore.Client
{
    public class FDFSClient : IFDFSClient
    {
        private readonly IExecuter _executer;
        public FDFSClient(IExecuter executer)
        {
            _executer = executer;
        }


        /// <summary>
        /// 获取存储节点
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <returns>存储节点实体类</returns>
        public async Task<StorageNode> GetStorageNodeAsync(string groupName)
        {
            var request = new QueryStoreWithGroupOneRequest(groupName);
            var response = await _executer.Execute(request);
            var storageNode = new StorageNode(response.GroupName, response.IPAddress, response.Port, response.StorePathIndex);
            return storageNode;
        }

        /// <summary>
        /// 查询文件存储的节点
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="fileId">文件名</param>
        /// <returns>存储节点实体类</returns>
        public async Task<StorageNode> QueryStorageNodeForFileAsync(string groupName, string fileId)
        {
            var request = new QueryFetchOneRequest(groupName, fileId);
            var response = await _executer.Execute(request);
            var storageNode = new StorageNode(response.GroupName, response.IPAddress, response.Port, 0);
            return storageNode;
        }

        /// <summary>
        /// 查询文件存储的多个节点
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="fileId">文件名</param>
        /// <returns>多个存储节点实体类</returns>
        public async Task<List<StorageNode>> QueryStorageNodesForFileAsync(string groupName, string fileId)
        {
            var request = new QueryFetchAllRequest(groupName, fileId);
            var response = await _executer.Execute(request);
            var storageNodes = response.IPAddresses.Select(x => new StorageNode(response.GroupName, x, response.Port, 0));
            return storageNodes.ToList();
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="contentByte">文件内容</param>
        /// <param name="fileExt">文件扩展名(注意:不包含".")</param>
        /// <returns>文件名</returns>
        public async Task<string> UploadFileAsync(StorageNode storageNode, byte[] contentByte, string fileExt)
        {
            fileExt = Util.ParseExtWithOut(fileExt);
            var request = new UploadFileRequest(storageNode.StorePathIndex, fileExt, contentByte);
            var response = await _executer.Execute(request, storageNode.EndPoint);
            return response.FileId;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="filename">上传文件文件名</param>
        /// <returns></returns>
        public async Task<string> UploadFileAsync(StorageNode storageNode, string filename)
        {
            string extension = Path.GetExtension(filename).Substring(1);
            var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            var request = new UploadFileRequest(storageNode.StorePathIndex, extension, fs);
            var response = await _executer.Execute(request, storageNode.EndPoint);
            return response.FileId;
        }

        /// <summary>
        /// 上传从文件
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="masterFileId">主文件名</param>
        /// <param name="prefixName">从文件后缀</param>
        /// <param name="contentBytes">文件内容</param>
        /// <param name="fileExt">文件扩展名(注意:不包含".")</param>
        /// <returns>文件名</returns>
        public async Task<string> UploadSlaveFileAsync(string groupName, string masterFileId, string prefixName, byte[] contentBytes, string fileExt)
        {
            var queryUpdateRequest = new QueryUpdateRequest(groupName, masterFileId);
            var queryUpdateResponse = await _executer.Execute(queryUpdateRequest);
            var storageNode = new StorageNode(queryUpdateResponse.GroupName, queryUpdateResponse.IPAddress, queryUpdateResponse.Port, 0);

            var request = new UploadSlaveFileRequest(masterFileId, prefixName, fileExt, contentBytes);
            var response = await _executer.Execute(request, storageNode.EndPoint);
            return response.FileId;
        }

        /// <summary>
        /// 上传从文件
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="masterFileId">主文件名</param>
        /// <param name="prefixName">从文件后缀</param>
        /// <param name="filename">本地文件名</param>
        /// <returns>文件名</returns>
        public async Task<string> UploadSlaveFileAsync(string groupName, string masterFileId, string prefixName, string filename)
        {
            var queryUpdateRequest = new QueryUpdateRequest(groupName, masterFileId);
            var queryUpdateResponse = await _executer.Execute(queryUpdateRequest);
            var storageNode = new StorageNode(queryUpdateResponse.GroupName, queryUpdateResponse.IPAddress, queryUpdateResponse.Port, 0);

            string extension = Path.GetExtension(filename).Substring(1);
            var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            var request = new UploadSlaveFileRequest(masterFileId, prefixName, extension, fs);
            var response = await _executer.Execute(request, storageNode.EndPoint);
            return response.FileId;
        }

        /// <summary>
        /// 上传可以Append的文件
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="contentBytes">文件内容</param>
        /// <param name="fileExt">文件扩展名(注意:不包含".")</param>
        /// <returns>文件名</returns>
        public async Task<string> UploadAppenderFileAsync(StorageNode storageNode, byte[] contentBytes, string fileExt)
        {
            var request = new UploadAppendFileRequest(storageNode.StorePathIndex, fileExt, contentBytes);
            var response = await _executer.Execute(request, storageNode.EndPoint);
            return response.FileId;
        }


        /// <summary>
        /// 上传可以Append的文件
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="filename">本地文件名</param>
        /// <returns>文件名</returns>
        public async Task<string> UploadAppenderFileAsync(StorageNode storageNode, string filename)
        {
            string extension = Path.GetExtension(filename).Substring(1);
            var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            var request = new UploadAppendFileRequest(storageNode.StorePathIndex, extension, fs);
            var response = await _executer.Execute(request, storageNode.EndPoint);
            return response.FileId;
        }

        /// <summary>
        /// 附加文件
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="fileId">文件名</param>
        /// <param name="contentBytes">文件内容</param>
        /// <returns>文件名</returns>
        public async Task<string> AppendFileAsync(string groupName, string fileId, byte[] contentBytes)
        {
            var queryUpdateRequest = new QueryUpdateRequest(groupName, fileId);
            var queryUpdateResponse = await _executer.Execute(queryUpdateRequest);
            var storageNode = new StorageNode(queryUpdateResponse.GroupName, queryUpdateResponse.IPAddress, queryUpdateResponse.Port, 0);

            var request = new AppendFileRequest(fileId, contentBytes);
            var response = await _executer.Execute(request, storageNode.EndPoint);
            return response.FileId;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="fileId">文件名</param>
        public async Task<bool> RemoveFileAsync(string groupName, string fileId)
        {
            var queryUpdateRequest = new QueryUpdateRequest(groupName, fileId);
            var queryUpdateResponse = await _executer.Execute(queryUpdateRequest);
            var storageNode = new StorageNode(queryUpdateResponse.GroupName, queryUpdateResponse.IPAddress, queryUpdateResponse.Port, 0);

            var request = new DeleteFileRequest(groupName, fileId);
            var response = await _executer.Execute(request, storageNode.EndPoint);
            return response.Success;
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="fileId">文件名</param>
        /// <returns>文件内容</returns>
        public async Task<byte[]> DownloadFileAsync(StorageNode storageNode, string fileId)
        {
            var request = new DownloadFileRequest(0L, 0L, storageNode.GroupName, fileId);
            var response = await _executer.Execute(request, storageNode.EndPoint);
            return response.ContentBytes;
        }


        /// <summary>
        /// 增量下载文件
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="fileId">文件名</param>
        /// <param name="offset">从文件起始点的偏移量</param>
        /// <param name="length">要读取的字节数</param>
        /// <returns>文件内容</returns>
        public async Task<byte[]> DownloadFileAsync(StorageNode storageNode, string fileId, long offset, long length)
        {
            var request = new DownloadFileRequest(offset, length, storageNode.GroupName, fileId);
            var response = await _executer.Execute(request, storageNode.EndPoint);
            return response.ContentBytes;
        }

        /// <summary>
        /// 获取文件信息
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="fileId">文件名</param>
        /// <returns></returns>
        public async Task<FDFSFileInfo> GetFileInfo(StorageNode storageNode, string fileId)
        {
            var request = new QueryFileInfoRequest(storageNode.GroupName, fileId);
            var response = await _executer.Execute(request, storageNode.EndPoint);
            return new FDFSFileInfo(response.FileSize, response.CreateTime, response.Crc32);
        }


        /// <summary>
        /// 获取文件媒体信息
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="fileId">文件名</param>
        /// <returns></returns>
        public async Task<IDictionary<string, string>> GetMetaData(StorageNode storageNode, string fileId)
        {
            var request = new GetMetaDataRequest(storageNode.GroupName, fileId);
            var response = await _executer.Execute(request, storageNode.EndPoint);
            return response.MetaData;
        }

        /// <summary>
        /// 设置文件媒体信息
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="fileId"></param>
        /// <param name="metaData">MetaData数据</param>
        /// <param name="option"></param>
        /// <returns></returns>
        public async Task SetMetaData(StorageNode storageNode, string fileId, IDictionary<string, string> metaData, MetaDataOption option = MetaDataOption.Overwrite)
        {
            var request = new SetMetaDataRequest(fileId, storageNode.GroupName, metaData, option);
            var response = await _executer.Execute(request);
        }
    }
}
