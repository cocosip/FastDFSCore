using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FastDFSCore.Sample
{
    public class SampleAppService : ISampleAppService
    {
        private readonly ILogger _logger;
        private readonly IFastDFSClient _client;
        public SampleAppService(ILogger<SampleAppService> logger, IFastDFSClient client)
        {
            _logger = logger;
            _client = client;
        }

        /// <summary>查询Storage信息
        /// </summary>
        public async Task ListStorageInfosAsync(string groupName)
        {
            _logger.LogInformation("---List Storage Infos ---");
            var storageInfos = await _client.ListStorageInfosAsync("group1");
            foreach (var storage in storageInfos)
            {
                _logger.LogInformation("Storage:[StorageId:{0},Status:{1},IP:{2},Version:{3},TotalMb:{4},FreeMb:{5}]", storage.StorageId, storage.Status, storage.IPAddress, storage.Version, storage.TotalMb, storage.FreeMb);
            }
            _logger.LogInformation("---End of List Storage Infos ---");
        }


        public async Task<List<string>> BatchUploadAsync(string groupName, string directoryPath)
        {
            _logger.LogInformation("---Batch Upload ---");
            Stopwatch watch = new Stopwatch();
            var fileIds = new List<string>();
            var dir = new DirectoryInfo(directoryPath);
            var fileInfos = dir.GetFiles();
            long totalSize = 0;
            watch.Start();

            var storageNode = await _client.GetStorageNodeAsync(groupName);

            foreach (var fileInfo in fileInfos)
            {
                //var buffer = File.ReadAllBytes(fileInfo.FullName);
                //var fileId = await _client.UploadFileAsync(storageNode, buffer, "dcm");
                var fileId = await _client.UploadFileAsync(storageNode, fileInfo.FullName);
                _logger.LogDebug("Upload File, FileId:{0} , LocalPath:{1}", fileId, fileInfo.FullName);
                fileIds.Add(fileId);
                totalSize += fileInfo.Length;
            }

            watch.Stop();

            var totalMb = totalSize * 1.0 / (1024 * 1024.0);
            var speed = totalMb / watch.Elapsed.TotalSeconds;

            _logger.LogInformation("Batch Upload '{0}'Files, TotalMB:'{1} MB',Cost:'{2} ms',Upload Speed:'{3} MB/s'··· ", fileInfos.Length, totalMb.ToString("F2"), watch.Elapsed.TotalMilliseconds, speed.ToString("F2"));

            _logger.LogInformation("--- End of Batch Upload ---");

            return fileIds;
        }

        public async Task<List<string>> BatchDownloadAsync(string groupName, List<string> fileIds, string saveDirectory)
        {
            _logger.LogInformation("--- Batch Download Test ---");
            Stopwatch watch = new Stopwatch();
            var savePaths = new List<string>();
            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }
            watch.Start();

            var storageNode = await _client.GetStorageNodeAsync(groupName);
            foreach (var fileId in fileIds)
            {
                var ext = GetPathExtension(fileId);
                var savePath = Path.Combine(saveDirectory, $"{Guid.NewGuid()}{ext}");
                await _client.DownloadFileEx(storageNode, fileId, savePath);
                _logger.LogDebug("Download File '{0}' To Path '{1}' ", fileId, savePath);
                savePaths.Add(savePath);
            }

            watch.Stop();
            //获取下载的文件总大小
            var dir = new DirectoryInfo(saveDirectory);
            var fileInfos = dir.GetFiles();
            long totalSize = 0;
            foreach (var fileInfo in fileInfos)
            {
                if (savePaths.Contains(fileInfo.FullName))
                {
                    totalSize += fileInfo.Length;
                }
            }

            var totalMb = totalSize * 1.0 / (1024 * 1024);
            var speed = totalMb / watch.Elapsed.TotalSeconds;

            _logger.LogInformation("Batch Download '{0}' Files, TotalMB:'{1}',Cost:'{2} ms',Download Speed:'{3} MB/s'··· ", fileIds.Count, totalMb.ToString("F2"), watch.Elapsed.TotalMilliseconds, speed.ToString("F2"));

            _logger.LogInformation("--- End of Batch Download Test ---");
            return savePaths;
        }


        /// <summary>获取某个路径中文件的扩展名
        /// </summary>
        public string GetPathExtension(string path)
        {
            if (path != "" && path.IndexOf('.') >= 0)
            {
                return path.Substring(path.LastIndexOf('.'));
            }
            return "";
        }

    }
}
