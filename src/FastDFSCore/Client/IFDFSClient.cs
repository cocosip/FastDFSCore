using System.Collections.Generic;
using System.Threading.Tasks;

namespace FastDFSCore.Client
{
    public interface IFDFSClient
    {

        /// <summary>
        /// 获取存储节点
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <returns>存储节点实体类</returns>
        Task<StorageNode> GetStorageNodeAsync(string groupName);

        /// <summary>
        /// 查询文件存储的节点
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="fileId">文件名</param>
        /// <returns>存储节点实体类</returns>
        Task<StorageNode> QueryStorageNodeForFileAsync(string groupName, string fileId);

        /// <summary>
        /// 查询文件存储的多个节点
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="fileId">文件名</param>
        /// <returns>多个存储节点实体类</returns>
        Task<List<StorageNode>> QueryStorageNodesForFileAsync(string groupName, string fileId);

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="contentByte">文件内容</param>
        /// <param name="fileExt">文件扩展名(注意:不包含".")</param>
        /// <returns>文件名</returns>
        Task<string> UploadFileAsync(StorageNode storageNode, byte[] contentByte, string fileExt);

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="filename">上传文件文件名</param>
        /// <returns></returns>
        Task<string> UploadFileAsync(StorageNode storageNode, string filename);

        /// <summary>
        /// 上传从文件
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="masterFileId">主文件名</param>
        /// <param name="prefixName">从文件后缀</param>
        /// <param name="contentBytes">文件内容</param>
        /// <param name="fileExt">文件扩展名(注意:不包含".")</param>
        /// <returns>文件名</returns>
        Task<string> UploadSlaveFileAsync(string groupName, string masterFileId, string prefixName, byte[] contentBytes, string fileExt);

        /// <summary>
        /// 上传从文件
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="masterFileId">主文件名</param>
        /// <param name="prefixName">从文件后缀</param>
        /// <param name="filename">本地文件名</param>
        /// <returns>文件名</returns>
        Task<string> UploadSlaveFileAsync(string groupName, string masterFileId, string prefixName, string filename);

        /// <summary>
        /// 上传可以Append的文件
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="contentBytes">文件内容</param>
        /// <param name="fileExt">文件扩展名(注意:不包含".")</param>
        /// <returns>文件名</returns>
        Task<string> UploadAppenderFileAsync(StorageNode storageNode, byte[] contentBytes, string fileExt);

        /// <summary>
        /// 上传可以Append的文件
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="filename">本地文件名</param>
        /// <returns>文件名</returns>
        Task<string> UploadAppenderFileAsync(StorageNode storageNode, string filename);

        /// <summary>
        /// 附加文件
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="fileId">文件名</param>
        /// <param name="contentBytes">文件内容</param>
        /// <returns>文件名</returns>
        Task<string> AppendFileAsync(string groupName, string fileId, byte[] contentBytes);

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="fileId">文件名</param>
        Task<bool> RemoveFileAsync(string groupName, string fileId);

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="fileId">文件名</param>
        /// <returns>文件内容</returns>
        Task<byte[]> DownloadFileAsync(StorageNode storageNode, string fileId);

        /// <summary>
        /// 增量下载文件
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="fileId">文件名</param>
        /// <param name="offset">从文件起始点的偏移量</param>
        /// <param name="length">要读取的字节数</param>
        /// <returns>文件内容</returns>
        Task<byte[]> DownloadFileAsync(StorageNode storageNode, string fileId, long offset, long length);
    }
}
