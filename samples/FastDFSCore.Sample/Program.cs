using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace FastDFSCore.Sample
{
    class Program
    {
        static IServiceProvider _provider;
        static IFastDFSClient _fdfsClinet;

        static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            services.AddLogging(l =>
            {
                l.AddConsole(c =>
                {
                    c.LogToStandardErrorThreshold = LogLevel.Trace;
                });
            });
            services
                .AddFastDFSCore(c =>
                {
                    c.Trackers = new List<Tracker>()
                    {
                        new Tracker("192.168.0.6",22122)
                    };
                })
                .AddFastDFSDotNettyTransport();


            _provider = services.BuildServiceProvider();
            _provider.ConfigureFastDFSCore();

            _fdfsClinet = _provider.GetService<IFastDFSClient>();

            RunAsync();
            Console.ReadLine();
        }

        public static async void RunAsync()
        {

            await BatchUploadTest();
            await BatchDownloadTest();

            //获取GroupInfo
            // await GroupInfoAsync();
            //获取StorageInfo
            // await StorageInfoAsync();
            //await BatchCustomDownloadTest();
        }


        /// <summary>查询组信息
        /// </summary>
        public static async Task GroupInfoAsync()
        {
            var groupInfo = await _fdfsClinet.ListOneGroupInfoAsync("group1");
            Console.WriteLine("获取单个GroupInfo:{0}", groupInfo);

            var groupInfos = await _fdfsClinet.ListAllGroupInfosAsync();
            foreach (var g in groupInfos)
            {
                Console.WriteLine("查询全部GroupInfo:{0}", g);
            }

        }

        /// <summary>查询Storage信息
        /// </summary>
        public static async Task StorageInfoAsync()
        {
            var storageInfos = await _fdfsClinet.ListStorageInfosAsync("group1");
            foreach (var s in storageInfos)
            {
                Console.WriteLine("查询全部StorageInfo:{0}", s);
            }
        }



        static List<string> UploadFileIds = new List<string>();

        /// <summary>批量上传测试
        /// </summary>
        public static async Task BatchUploadTest()
        {

            Console.WriteLine("-------------批量上传测试---------");
            Stopwatch watch = new Stopwatch();
            var storageNode = await _fdfsClinet.GetStorageNodeAsync("group1");
            var dir =
            new DirectoryInfo(@"D:\DicomTests");
            //new DirectoryInfo(@"D:\Pictures");
            //new DirectoryInfo(@"D:\DicomTest\BigTest");
            //G:\安装文件\SystemISO
            var fileInfos = dir.GetFiles();
            long totalSize = 0;
            watch.Start();
            foreach (var fileInfo in fileInfos)
            {
                //var fileId = await _fdfsClinet.UploadFileAsync(storageNode, File.ReadAllBytes(fileInfo.FullName),"dcm");
                var fileId = await _fdfsClinet.UploadFileAsync(storageNode, fileInfo.FullName);
                Console.WriteLine("FileId:{0}", fileId);
                UploadFileIds.Add(fileId);
                totalSize += fileInfo.Length;
            }
            watch.Stop();
            Console.WriteLine("共上传:{0}个文件,总共:{1}Mb,花费:{2},速度:{3} Mb/s", fileInfos.Length, (totalSize / (1024.00 * 1024.00)).ToString("F2"), watch.Elapsed, ((totalSize / (watch.Elapsed.TotalSeconds * 1024.0 * 1024.0))).ToString("F2"));

        }

        /// <summary>批量下载测试
        /// </summary>
        public static async Task BatchDownloadTest()
        {
            Console.WriteLine("-------------批量下载测试---------");
            Stopwatch watch = new Stopwatch();
            var storageNode = await _fdfsClinet.GetStorageNodeAsync("group1");
            var saveDir = @"G:\DownloadTest";
            if (!Directory.Exists(saveDir))
            {
                Directory.CreateDirectory(saveDir);
            }
            watch.Start();
            foreach (var fileId in UploadFileIds)
            {
                var ext = GetPathExtension(fileId);
                var savePath = Path.Combine(saveDir, $"{Guid.NewGuid().ToString()}{ext}");
                await _fdfsClinet.DownloadFileEx(storageNode, fileId, savePath);
                Console.WriteLine("下载文件,FileId:{0},保存路径:{1}", fileId, savePath);
            }
            watch.Stop();
            //获取下载的文件总大小
            var dir = new DirectoryInfo(saveDir);
            var fileInfos = dir.GetFiles();
            long totalSize = 0;
            foreach (var fileInfo in fileInfos)
            {
                totalSize += fileInfo.Length;
            }
            Console.WriteLine("共下载:{0}个文件,总共:{1}Mb,花费:{2},速度:{3} Mb/s", fileInfos.Length, (totalSize / (1024.00 * 1024.00)), watch.Elapsed, (totalSize / (watch.Elapsed.TotalSeconds * 1024.0 * 1024.0)).ToString("F2"));

        }

        /// <summary>批量自定义下载
        /// </summary>
        public static async Task BatchCustomDownloadTest()
        {
            Console.WriteLine("-------------批量下载测试---------");
            Stopwatch watch = new Stopwatch();
            var storageNode = await _fdfsClinet.GetStorageNodeAsync("group1");
            var saveDir = @"G:\DownloadTest";
            if (!Directory.Exists(saveDir))
            {
                Directory.CreateDirectory(saveDir);
            }
            watch.Start();
            foreach (var fileId in UploadFileIds)
            {
                var ext = GetPathExtension(fileId);
                var savePath = Path.Combine(saveDir, $"{Guid.NewGuid().ToString()}{ext}");
                //await _fdfsClinet.DownloadFileEx(storageNode, fileId, _downloaderFactory.CreateDownloader<CustomDownloader>(savePath));
                Console.WriteLine("下载文件,FileId:{0},保存路径:{1}", fileId, savePath);
            }
            watch.Stop();
            //获取下载的文件总大小
            var dir = new DirectoryInfo(saveDir);
            var fileInfos = dir.GetFiles();
            long totalSize = 0;
            foreach (var fileInfo in fileInfos)
            {
                totalSize += fileInfo.Length;
            }
            Console.WriteLine("共下载:{0}个文件,总共:{1}Mb,花费:{2},速度:{3} Mb/s", fileInfos.Length, (totalSize / (1024.00 * 1024.00)), watch.Elapsed, (totalSize / (watch.Elapsed.TotalSeconds * 1024.0 * 1024.0)).ToString("F2"));

        }

        /// <summary>上传单个文件
        /// </summary>
        public static async Task UploadFileSample()
        {
            try
            {
                var storageNode = await _fdfsClinet.GetStorageNodeAsync("group1");
                var filename = @"F:\img\1.jpg";
                //-------------批量上传测试---------
                //FileId:M00/02/BB/wKgABl0AkCeAbAdaAG04UC5fN08906.exe
                //FileId:M00/02/BB/wKgABl0AkfAAAAABAH8oAOpc-Jk809.iso
                //@"D:\Pictures\1.jpg";

                var fileId = await _fdfsClinet.UploadFileAsync(storageNode, filename);
                //var fileId = await _fdfsClinet.UploadFileAsync(storageNode, File.ReadAllBytes(filename), "jpg");
                Console.WriteLine("上传文件位置:{0},路径:{1}", filename, fileId);
            }
            catch (Exception ex)
            {
                var a = ex.Message;
            }
        }

        /// <summary>下载单个文件
        /// </summary>
        public static async Task DownloadToPath()
        {
            Console.WriteLine("测试下载文件");
            var storageNode = await _fdfsClinet.GetStorageNodeAsync("group1");
            var downloadFileId = "M00/03/C5/wKgABl0LMOGAW3fEAAIMphIw1AQ895.dcm";
            var result = await _fdfsClinet.DownloadFileEx(storageNode, downloadFileId, @"D:\2.dcm");
            Console.WriteLine("下载文件名:{0},大小:{1}", downloadFileId, result.Length);

        }


        /// <summary>获取某个路径中文件的扩展名
        /// </summary>
        public static string GetPathExtension(string path)
        {
            if (path != "" && path.IndexOf('.') >= 0)
            {
                return path.Substring(path.LastIndexOf('.'));
            }
            return "";
        }

    }
}
