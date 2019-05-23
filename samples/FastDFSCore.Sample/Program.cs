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
            IServiceCollection services = new ServiceCollection();
            services.AddFastDFSCore(c =>
            {
                c.TrackerMaxConnection = 10;
                c.StorageMaxConnection = 20;
                c.Trackers = new List<IPEndPoint>()
                {
                    new IPEndPoint(IPAddress.Parse("192.168.0.129"),22122)
                };
            });
#pragma warning disable CS0618 // 类型或成员已过时
            //InternalLoggerFactory.DefaultFactory.AddProvider(new ConsoleLoggerProvider((s, level) => true, false));
#pragma warning restore CS0618 // 类型或成员已过时

            _provider = services.BuildServiceProvider();

            _fdfsClinet = _provider.GetService<IFDFSClient>();

            BatchUpload().Wait();

            Console.ReadLine();
        }

        /// <summary>上传单个文件
        /// </summary>
        public static async Task UploadFileSample()
        {
            var storageNode = await _fdfsClinet.GetStorageNodeAsync("group1");
            var filename = @"D:\Pictures\1.jpg";

            var fileId = await _fdfsClinet.UploadFileAsync(storageNode, filename).ConfigureAwait(false);
            //var fileId = await _fdfsClinet.UploadFileAsync(storageNode, File.ReadAllBytes(filename), "jpg");
            Console.WriteLine("上传文件位置:{0},路径:{1}", filename, fileId);
        }



        public static async Task BatchUpload()
        {
            List<string> uploadFiles = new List<string>();
            Stopwatch watch = new Stopwatch();
            var storageNode = await _fdfsClinet.GetStorageNodeAsync("group1");
            //var dir = //@"G:\Kayisoft\TMEasy PACS\测试Dicom";
            //@"G:\Kayisoft\TMEasy PACS\DICOM 100 Test2";

            //var files = Directory.GetFiles(dir);
            //watch.Start();
            //foreach (var filePath in files)
            //{
            //    var fileId = await _fdfsClinet.UploadFileAsync(storageNode, filePath).ConfigureAwait(false);
            //    Console.WriteLine("FileId:{0}", fileId);
            //    uploadFiles.Add(fileId);
            //}
            //watch.Stop();

            ////计算文件大小
            //long length = 0;
            //foreach (var filePath in files)
            //{
            //    var file = new FileInfo(filePath);
            //    length += file.Length;
            //}
            //var size = (length / 1024.00) / 1024.00;
            //Console.WriteLine("共:{0}个文件,总共:{1}mb,花费:{2}", files.Length, size, watch.Elapsed);

            //Console.WriteLine("开始下载文件下载文件");
            //watch.Reset();
            //watch.Start();
            //foreach (var uploadFile in uploadFiles)
            //{
            //    var bytes = await _fdfsClinet.DownloadFileAsync(storageNode, uploadFile);
            //    Console.WriteLine("下载文件:{0}", uploadFile);
            //}
            //watch.Stop();
            //Console.WriteLine("共:{0}个文件,总共:{1}mb,花费:{2}", files.Length, size, watch.Elapsed);

            var downloadFileId = "M00/00/0D/wKgAgVzmwcSAGzatAAguSOLvVco450.dcm";

            var result = await _fdfsClinet.DownloadFileAsync(storageNode, downloadFileId);

            Console.WriteLine("下载文件名:{0},大小:{1}", downloadFileId, result.Length);

        }




    }
}
