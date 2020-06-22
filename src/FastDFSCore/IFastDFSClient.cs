using FastDFSCore.Protocols;
using FastDFSCore.Transport.Download;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FastDFSCore
{
    /// <summary>FastDFS客户端
    /// </summary>
    public interface IFastDFSClient
    {

        /// <summary>
        /// 获取存储节点
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <returns>存储节点实体类</returns>
        Task<StorageNode> GetStorageNodeAsync(string groupName);

        /// <summary>查询某个组的组信息
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <returns></returns>
        Task<GroupInfo> ListOneGroupInfoAsync(string groupName);

        /// <summary>查询全部的组
        /// </summary>
        Task<List<GroupInfo>> ListAllGroupInfosAsync();

        /// <summary>按组名查询全部的Storage信息
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <returns></returns>
        Task<List<StorageInfo>> ListStorageInfosAsync(string groupName);

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
        /// <param name="stream">文件流</param>
        /// <param name="fileExt">文件扩展名(注意:不包含".")</param>
        /// <returns>文件名</returns>
        Task<string> UploadFileAsync(StorageNode storageNode, Stream stream, string fileExt);

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

        /// <summary>
        /// 下载文件到指定的地点
        /// </summary>
        /// <param name="storageNode"></param>
        /// <param name="fileId"></param>
        /// <param name="filePath">文件保存路径</param>
        /// <returns></returns>
        Task<string> DownloadFileEx(StorageNode storageNode, string fileId, string filePath);

        /// <summary>
        /// 自定义下载文件
        /// </summary>
        /// <param name="storageNode"></param>
        /// <param name="fileId"></param>
        /// <param name="downloader">文件下载器</param>
        /// <returns></returns>
        Task<string> DownloadFileEx(StorageNode storageNode, string fileId, IDownloader downloader);

        /// <summary>
        /// 获取文件信息
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="fileId">文件名</param>
        /// <returns></returns>
        Task<FastDFSFileInfo> GetFileInfo(StorageNode storageNode, string fileId);

        /// <summary>
        /// 获取文件媒体信息
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="fileId">文件名</param>
        /// <returns></returns>
        Task<IDictionary<string, string>> GetMetaData(StorageNode storageNode, string fileId);

        /// <summary>
        /// 设置文件媒体信息
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="fileId"></param>
        /// <param name="metaData">MetaData数据</param>
        /// <param name="option"></param>
        /// <returns></returns>
        Task SetMetaData(StorageNode storageNode, string fileId, IDictionary<string, string> metaData, MetaDataOption option = MetaDataOption.Overwrite);

    }
}