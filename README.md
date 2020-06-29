# FastDFSCore (c# client of FastDFS)

[![996.icu](https://img.shields.io/badge/link-996.icu-red.svg)](https://996.icu) [![GitHub](https://img.shields.io/github/license/mashape/apistatus.svg)](https://github.com/cocosip/FastDFSCore/blob/master/LICENSE) ![GitHub last commit](https://img.shields.io/github/last-commit/cocosip/FastDFSCore.svg) ![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/cocosip/FastDFSCore.svg)

| Build Server | Platform | Build Status |
| ------------ | -------- | ------------ |
| Azure Pipelines| Windows |[![Build Status](https://dev.azure.com/cocosip/FastDFSCore/_apis/build/status/cocosip.FastDFSCore?branchName=master&jobName=Windows)](https://dev.azure.com/cocosip/FastDFSCore/_build/latest?definitionId=5&branchName=master)|
| Azure Pipelines| Linux |[![Build Status](https://dev.azure.com/cocosip/FastDFSCore/_apis/build/status/cocosip.FastDFSCore?branchName=master&jobName=Linux)](https://dev.azure.com/cocosip/FastDFSCore/_build/latest?definitionId=5&branchName=master)|

| Package  | Version |Preview| Downloads|
| -------- | ------- |------ |-------- |
| `FastDFSCore` | [![NuGet](https://img.shields.io/nuget/v/FastDFSCore.svg)](https://www.nuget.org/packages/FastDFSCore) | [![NuGet](https://img.shields.io/nuget/vpre/FastDFSCore.svg)](https://www.nuget.org/packages/FastDFSCore) |![NuGet](https://img.shields.io/nuget/dt/FastDFSCore.svg)|
| `FastDFSCore.Transport.DotNetty` | [![NuGet](https://img.shields.io/nuget/v/FastDFSCore.Transport.DotNetty.svg)](https://www.nuget.org/packages/FastDFSCore.Transport.DotNetty)|[![NuGet](https://img.shields.io/nuget/vpre/FastDFSCore.Transport.DotNetty.svg)](https://www.nuget.org/packages/FastDFSCore.Transport.DotNetty) |![NuGet](https://img.shields.io/nuget/dt/FastDFSCore.Transport.DotNetty.svg)|
| `FastDFSCore.Transport.SuperSocket` | [![NuGet](https://img.shields.io/nuget/v/FastDFSCore.Transport.SuperSocket.svg)](https://www.nuget.org/packages/FastDFSCore.Transport.SuperSocket)|[![NuGet](https://img.shields.io/nuget/vpre/FastDFSCore.Transport.SuperSocket.svg)](https://www.nuget.org/packages/FastDFSCore.Transport.SuperSocket) |![NuGet](https://img.shields.io/nuget/dt/FastDFSCore.Transport.SuperSocket.svg)|

## Features

- Base on `netstandard2.0`
- Base on `DotNetty` or `SuperSocket` Communication
- Support connection pool
- Support for file streams to upload, network streams download to the local

## FastDFS

- [FastDFS Project](https://github.com/happyfish100/fastdfs)

## Guide

- [Install Guide](/docs/fastdfs安装.md)

## Sample

```c#
var services = new ServiceCollection();
services
    .AddLogging(l =>
    {
        l.AddConsole(c =>
        {
            c.LogToStandardErrorThreshold = LogLevel.Trace;
        });
    })
    .AddFastDFSCore(c=>{
        c.Trackers = new List<Tracker>()
        {
            new Tracker("192.168.0.6",22122)
        };
    })
    .AddFastDFSDotNetty();

var provider = services.BuildServiceProvider();

var client = _provider.GetService<IFastDFSClient>();
var storageNode = await fdfsClient.GetStorageNodeAsync("group1");
var fileId= await fdfsClient.UploadFileAsync(storageNode, @"D:\sample1.txt");

var savePath=Path.Combine(@"D:\sample2.txt");
await client.DownloadFileEx(storageNode, fileId, savePath);

```

> [more sample code](https://github.com/cocosip/FastDFSCore/blob/master/samples/FastDFSCore.Sample/Program.cs)
