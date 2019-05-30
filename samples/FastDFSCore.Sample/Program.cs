using Microsoft.Extensions.DependencyInjection;
using System;
using FastDFSCore.Client;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetty.Common.Internal.Logging;
using Microsoft.Extensions.Logging.Console;
using System.IO;
using System.Diagnostics;

namespace FastDFSCore.Sample
{
    class Program
    {
        static IServiceProvider _provider;
        static IFDFSClient _fdfsClinet;
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            IServiceCollection services = new ServiceCollection();
            services.AddFastDFSCore(c =>
            {
                c.TrackerMaxConnection = 10;
                c.StorageMaxConnection = 50;
                c.Trackers = new List<IPEndPoint>()
                {
                    new IPEndPoint(IPAddress.Parse("192.168.0.6"),22122)
                };
            });
#pragma warning disable CS0618 // 类型或成员已过时
            //InternalLoggerFactory.DefaultFactory.AddProvider(new ConsoleLoggerProvider((s, level) => true, false));
#pragma warning restore CS0618 // 类型或成员已过时

            _provider = services.BuildServiceProvider();

            _fdfsClinet = _provider.GetService<IFDFSClient>();

            RunAsync().Wait();

            Console.ReadLine();
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.ExceptionObject.ToString());
        }

        public static async Task RunAsync()
        {
            await BatchUploadTest().ConfigureAwait(false);
            //await BatchUploadTest().ConfigureAwait(false);
            await BatchDownloadTest().ConfigureAwait(false);
        }

        /// <summary>上传单个文件
        /// </summary>
        public static async Task UploadFileSample()
        {
            try
            {
                var storageNode = await _fdfsClinet.GetStorageNodeAsync("group1");
                var filename = @"F:\img\1.jpg";
                //@"D:\Pictures\1.jpg";

                var fileId = await _fdfsClinet.UploadFileAsync(storageNode, filename).ConfigureAwait(false);
                //var fileId = await _fdfsClinet.UploadFileAsync(storageNode, File.ReadAllBytes(filename), "jpg");
                Console.WriteLine("上传文件位置:{0},路径:{1}", filename, fileId);
            }
            catch (Exception ex)
            {
                var a = ex.Message;
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
            var dir = new DirectoryInfo(@"G:\Kayisoft\TMEasy PACS\DICOM 100 Test2");
            //new DirectoryInfo(@"G:\Kayisoft\TMEasy PACS\DICOM 100 Test");
            var fileInfos = dir.GetFiles();
            long totalSize = 0;
            watch.Start();
            foreach (var fileInfo in fileInfos)
            {
                var fileId = await _fdfsClinet.UploadFileAsync(storageNode, fileInfo.FullName).ConfigureAwait(false);
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



        /// <summary>下载单个文件
        /// </summary>
        public static async Task DownloadToPath()
        {
            Console.WriteLine("测试下载文件");
            var storageNode = await _fdfsClinet.GetStorageNodeAsync("group1");
            var downloadFileId = "M00/00/09/wKgBcFzm-XKAGti4AAYn98u2gV8562.jpg";
            var result = await _fdfsClinet.DownloadFileEx(storageNode, downloadFileId, @"D:\2.jpg");
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
