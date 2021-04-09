using FastDFSCore.Protocols;
using FastDFSCore.Transport;
using FastDFSCore.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FastDFSCore
{
    /// <summary>FastDFS客户端
    /// </summary>
    public class FastDFSClient : IFastDFSClient
    {
        private readonly IClusterFactory _clusterFactory;
        private readonly IExecuter _executer;

        /// <summary>Ctor
        /// </summary>
        public FastDFSClient(IExecuter executer, IClusterFactory clusterFactory)
        {
            _executer = executer;
            _clusterFactory = clusterFactory;
        }


        /// <summary>
        /// 获取存储节点
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="clusterName">集群名称</param>
        /// <returns>存储节点实体类</returns>
        public async ValueTask<StorageNode> GetStorageNodeAsync(
            string groupName, 
            string clusterName = "")
        {
            var request = new QueryStoreWithGroupOne(groupName);
            var response = await _executer.Execute(request, clusterName);
            var storageNode = new StorageNode(response.GroupName, response.IPAddress, response.Port, response.StorePathIndex);
            return storageNode;
        }

        /// <summary>查询某个组的组信息
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="clusterName">集群名称</param>
        /// <returns></returns>
        public async ValueTask<GroupInfo> ListOneGroupInfoAsync(
            string groupName, 
            string clusterName = "")
        {
            var request = new ListOneGroup(groupName);
            var response = await _executer.Execute(request, clusterName);
            return response.GroupInfo;
        }

        /// <summary>
        /// 查询全部的组
        /// </summary>
        /// <param name="clusterName">集群名称</param>
        /// <returns></returns>
        public async ValueTask<List<GroupInfo>> ListAllGroupInfosAsync(string clusterName = "")
        {
            var request = new ListAllGroup();
            var response = await _executer.Execute(request, clusterName);
            return response.GroupInfos;
        }

        /// <summary>
        /// 按组名查询全部的Storage信息
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="clusterName">集群名称</param>
        /// <returns></returns>
        public async ValueTask<List<StorageInfo>> ListStorageInfosAsync(
            string groupName, 
            string clusterName = "")
        {
            var request = new ListStorage(groupName);
            var response = await _executer.Execute(request, clusterName);
            return response.StorageInfos;
        }


        /// <summary>
        /// 查询文件存储的节点
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="fileId">文件名</param>
        /// <param name="clusterName">集群名称</param>
        /// <returns>存储节点实体类</returns>
        public async ValueTask<StorageNode> QueryStorageNodeForFileAsync(
            string groupName, 
            string fileId, 
            string clusterName = "")
        {
            var request = new QueryFetchOne(groupName, fileId);
            var response = await _executer.Execute(request, clusterName);
            var storageNode = new StorageNode(response.GroupName, response.IPAddress, response.Port, 0);
            return storageNode;
        }

        /// <summary>
        /// 查询文件存储的多个节点
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="fileId">文件名</param>
        /// <param name="clusterName">集群名称</param>
        /// <returns>多个存储节点实体类</returns>
        public async ValueTask<List<StorageNode>> QueryStorageNodesForFileAsync(
            string groupName, 
            string fileId, 
            string clusterName = "")
        {
            var request = new QueryFetchAll(groupName, fileId);
            var response = await _executer.Execute(request, clusterName);
            var storageNodes = response.IPAddresses.Select(x => new StorageNode(response.GroupName, x, response.Port, 0));
            return storageNodes.ToList();
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="content">文件内容</param>
        /// <param name="fileExt">文件扩展名(注意:不包含".")</param>
        /// <param name="clusterName">集群名称</param>
        /// <returns>文件名</returns>
        public async ValueTask<string> UploadFileAsync(
            StorageNode storageNode,
            byte[] content, 
            string fileExt, 
            string clusterName = "")
        {
            fileExt = fileExt.TrimStart('.');
            var request = new UploadFile(storageNode.StorePathIndex, fileExt, content);
            var response = await _executer.Execute(request, clusterName, storageNode.ConnectionAddress);
            return response.FileId;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="stream">文件流</param>
        /// <param name="fileExt">文件扩展名(注意:不包含".")</param>
        /// <param name="clusterName">集群名称</param>
        /// <returns>文件名</returns>
        public async ValueTask<string> UploadFileAsync(
            StorageNode storageNode, 
            Stream stream,
            string fileExt,
            string clusterName = "")
        {
            fileExt = fileExt.TrimStart('.');
            var request = new UploadFile(storageNode.StorePathIndex, fileExt, stream);
            var response = await _executer.Execute(request, clusterName, storageNode.ConnectionAddress);
            return response.FileId;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="filename">上传文件文件名</param>
        /// <param name="clusterName">集群名称</param>
        /// <returns></returns>
        public async ValueTask<string> UploadFileAsync(
            StorageNode storageNode,
            string filename, 
            string clusterName = "")
        {
            string fileExt = Path.GetExtension(filename).TrimStart('.');
            var fs = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            var request = new UploadFile(storageNode.StorePathIndex, fileExt, fs);
            var response = await _executer.Execute(request, clusterName, storageNode.ConnectionAddress);
            return response.FileId;
        }

        /// <summary>
        /// 上传从文件
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="masterFileId">主文件名</param>
        /// <param name="prefixName">从文件后缀</param>
        /// <param name="content">文件内容</param>
        /// <param name="fileExt">文件扩展名(注意:不包含".")</param>
        /// <param name="clusterName">集群名称</param>
        /// <returns>文件名</returns>
        public async ValueTask<string> UploadSlaveFileAsync(
            string groupName, 
            string masterFileId,
            string prefixName, 
            byte[] content, 
            string fileExt, 
            string clusterName = "")
        {
            fileExt = fileExt.TrimStart('.');
            var queryUpdateRequest = new QueryUpdate(groupName, masterFileId);
            var queryUpdateResponse = await _executer.Execute(queryUpdateRequest, clusterName);
            var storageNode = new StorageNode(queryUpdateResponse.GroupName, queryUpdateResponse.IPAddress, queryUpdateResponse.Port, 0);

            var request = new UploadSlaveFile(masterFileId, prefixName, fileExt, content);
            var response = await _executer.Execute(request, clusterName, storageNode.ConnectionAddress);
            return response.FileId;
        }

        /// <summary>
        /// 上传从文件
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="masterFileId">主文件名</param>
        /// <param name="prefixName">从文件后缀</param>
        /// <param name="filename">本地文件名</param>
        /// <param name="clusterName">集群名称</param>
        /// <returns>文件名</returns>
        public async ValueTask<string> UploadSlaveFileAsync(
            string groupName, 
            string masterFileId,
            string prefixName, 
            string filename, 
            string clusterName = "")
        {
            var queryUpdateRequest = new QueryUpdate(groupName, masterFileId);
            var queryUpdateResponse = await _executer.Execute(queryUpdateRequest, clusterName);
            var storageNode = new StorageNode(queryUpdateResponse.GroupName, queryUpdateResponse.IPAddress, queryUpdateResponse.Port, 0);

            string extension = Path.GetExtension(filename).Substring(1);
            var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            var request = new UploadSlaveFile(masterFileId, prefixName, extension, fs);
            var response = await _executer.Execute(request, clusterName, storageNode.ConnectionAddress);
            return response.FileId;
        }

        /// <summary>
        /// 上传可以Append的文件
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="content">文件内容</param>
        /// <param name="fileExt">文件扩展名(注意:不包含".")</param>
        /// <param name="clusterName">集群名称</param>
        /// <returns>文件名</returns>
        public async ValueTask<string> UploadAppenderFileAsync(
            StorageNode storageNode, 
            byte[] content,
            string fileExt, 
            string clusterName = "")
        {
            fileExt = fileExt.TrimStart('.');
            var request = new UploadAppendFile(storageNode.StorePathIndex, fileExt, content);
            var response = await _executer.Execute(request, clusterName, storageNode.ConnectionAddress);
            return response.FileId;
        }


        /// <summary>
        /// 上传可以Append的文件
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="filename">本地文件名</param>
        /// <param name="clusterName">集群名称</param>
        /// <returns>文件名</returns>
        public async ValueTask<string> UploadAppenderFileAsync(
            StorageNode storageNode, 
            string filename,
            string clusterName = "")
        {
            string extension = Path.GetExtension(filename).TrimStart('.');
            var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            var request = new UploadAppendFile(storageNode.StorePathIndex, extension, fs);
            var response = await _executer.Execute(request, clusterName, storageNode.ConnectionAddress);
            return response.FileId;
        }

        /// <summary>
        /// 附加文件
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="fileId">文件名</param>
        /// <param name="content">文件内容</param>
        /// <param name="clusterName">集群名称</param>
        /// <returns>文件名</returns>
        public async ValueTask<string> AppendFileAsync(
            string groupName, 
            string fileId, 
            byte[] content,
            string clusterName = "")
        {
            var queryUpdateRequest = new QueryUpdate(groupName, fileId);
            var queryUpdateResponse = await _executer.Execute(queryUpdateRequest, clusterName);
            var storageNode = new StorageNode(queryUpdateResponse.GroupName, queryUpdateResponse.IPAddress, queryUpdateResponse.Port, 0);

            var request = new AppendFile(fileId, content);
            var response = await _executer.Execute(request, clusterName, storageNode.ConnectionAddress);
            return response.FileId;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="fileId">文件名</param>
        /// <param name="clusterName">集群名称</param>
        /// <returns>是否删除成功</returns>

        public async ValueTask<bool> RemoveFileAsync(
            string groupName, 
            string fileId, 
            string clusterName = "")
        {
            var queryUpdateRequest = new QueryUpdate(groupName, fileId);
            var queryUpdateResponse = await _executer.Execute(queryUpdateRequest, clusterName);
            var storageNode = new StorageNode(queryUpdateResponse.GroupName, queryUpdateResponse.IPAddress, queryUpdateResponse.Port, 0);

            var request = new DeleteFile(groupName, fileId);
            var response = await _executer.Execute(request, clusterName, storageNode.ConnectionAddress);
            return response.Success;
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="fileId">文件名</param>
        /// <param name="clusterName">集群名称</param>
        /// <returns>文件内容</returns>
        public async ValueTask<byte[]> DownloadFileAsync(
            StorageNode storageNode, 
            string fileId, 
            string clusterName = "")
        {
            var request = new DownloadFile(0L, 0L, storageNode.GroupName, fileId);
            var response = await _executer.Execute(request, clusterName, storageNode.ConnectionAddress);
            return response.Content;
        }


        /// <summary>
        /// 增量下载文件
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="fileId">文件名</param>
        /// <param name="offset">从文件起始点的偏移量</param>
        /// <param name="length">要读取的字节数</param>
        /// <param name="clusterName">集群名称</param>
        /// <returns>文件内容</returns>
        public async ValueTask<byte[]> DownloadFileAsync(
            StorageNode storageNode, 
            string fileId, 
            long offset,
            long length, 
            string clusterName = "")
        {
            var request = new DownloadFile(offset, length, storageNode.GroupName, fileId);
            var response = await _executer.Execute(request, clusterName, storageNode.ConnectionAddress);
            return response.Content;
        }

        /// <summary>
        /// 下载文件到指定的地点
        /// </summary>
        /// <param name="storageNode"></param>
        /// <param name="fileId"></param>
        /// <param name="filePath">文件保存路径</param>
        /// <param name="clusterName">集群名称</param>
        /// <returns></returns>
        public async ValueTask<string> DownloadFileEx(
            StorageNode storageNode,
            string fileId,
            string filePath,
            string clusterName = "")
        {
            var request = new DownloadStreamFile(storageNode.GroupName, fileId)
            {
                OutputFilePath = filePath
            };
            _ = await _executer.Execute(request, clusterName, storageNode.ConnectionAddress);
            return filePath;
        }


        /// <summary>
        /// 获取文件信息
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="fileId">文件名</param>
        /// <param name="clusterName">集群名称</param>
        /// <returns></returns>
        public async ValueTask<FastDFSFileInfo> GetFileInfo(
            StorageNode storageNode, 
            string fileId,
            string clusterName = "")
        {
            var request = new QueryFileInfo(storageNode.GroupName, fileId);
            var response = await _executer.Execute(request, clusterName, storageNode.ConnectionAddress);
            return new FastDFSFileInfo(response.FileSize, response.CreateTime, response.Crc32);
        }


        /// <summary>
        /// 获取文件媒体信息
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="fileId">文件名</param>
        /// <param name="clusterName">集群名称</param>
        /// <returns></returns>
        public async ValueTask<IDictionary<string, string>> GetMetaData(
            StorageNode storageNode, 
            string fileId,
            string clusterName = "")
        {
            var request = new GetMetaData(storageNode.GroupName, fileId);
            var response = await _executer.Execute(request, clusterName, storageNode.ConnectionAddress);
            return response.MetaData;
        }

        /// <summary>
        /// 设置文件媒体信息
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="fileId"></param>
        /// <param name="metaData">MetaData数据</param>
        /// <param name="option"></param>
        /// <param name="clusterName">集群名称</param>
        /// <returns></returns>
        public async ValueTask SetMetaData(
            StorageNode storageNode, 
            string fileId, 
            IDictionary<string, string> metaData,
            MetaDataOption option = MetaDataOption.Overwrite,
            string clusterName = "")
        {
            var request = new SetMetaData(fileId, storageNode.GroupName, metaData, option);
            _ = await _executer.Execute(request, clusterName);
        }

        /// <summary>
        /// 生成文件访问Token
        /// </summary>
        /// <param name="fileId">文件名</param>
        /// <param name="timeStamp">时间戳</param>
        /// <param name="clusterName">集群名称</param>
        /// <returns></returns>
        public string GetToken(
            string fileId,
            int? timeStamp = null,
            string clusterName = "")
        {
            var cluster = _clusterFactory.Get(clusterName);
            var configuration = cluster.GetConfiguration();

            if (timeStamp.HasValue)
            {
                timeStamp = DateTimeUtil.ToInt32(DateTime.Now);
            }

            return Util.GetToken(fileId, timeStamp.Value, configuration.SecretKey, configuration.Charset);
        }

        /// <summary>
        /// 生成文件访问Token
        /// </summary>
        /// <param name="fileId">文件名</param>
        /// <param name="dateTime">时间</param>
        /// <param name="clusterName">集群名称</param>
        /// <returns></returns>
        public string GetToken(
            string fileId,
            DateTime? dateTime = null,
            string clusterName = "")
        {
            var timeStamp = dateTime.HasValue ? DateTimeUtil.ToInt32(dateTime.Value) : DateTimeUtil.ToInt32(DateTime.Now);
            return GetToken(fileId, timeStamp, clusterName);
        }
    }
}
