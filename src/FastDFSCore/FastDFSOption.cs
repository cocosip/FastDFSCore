﻿using System;
using System.Collections.Generic;

namespace FastDFSCore
{
    /// <summary>配置信息
    /// </summary>
    public class FastDFSOption
    {
        /// <summary>Tracker地址
        /// </summary>
        public List<Tracker> Trackers { get; set; }

        /// <summary>连接超时时间,5s
        /// </summary>
        public int ConnectionTimeout { get; set; } = 5;

        /// <summary>编码
        /// </summary>
        public string Charset { get; set; } = "utf-8";

        /// <summary>连接的有效时间,3600s
        /// </summary>
        public int ConnectionLifeTime { get; set; } = 600;

        /// <summary>并发线程数
        /// </summary>
        public int ConnectionConcurrentThread { get; set; } = 3;

        /// <summary>查询超时的连接的时间间隔
        /// </summary>
        public int ScanTimeoutConnectionInterval { get; set; } = 10;

        /// <summary>Tracker最大连接数
        /// </summary>
        public int TrackerMaxConnection { get; set; } = 10;

        /// <summary>Storage最大连接数
        /// </summary>
        public int StorageMaxConnection { get; set; } = 50;

        public FastDFSOption()
        {
            Trackers = new List<Tracker>();
        }

    }


    /// <summary>Tracker信息
    /// </summary>
    public class Tracker : IEquatable<Tracker>
    {
        /// <summary>IP地址
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>端口号
        /// </summary>
        public int Port { get; set; }

        /// <summary>Ctor
        /// </summary>
        public Tracker()
        {

        }

        /// <summary>Ctor
        /// </summary>
        public Tracker(string iPAddress, int port)
        {
            IPAddress = iPAddress;
            Port = port;
        }

        /// <summary>Equals
        /// </summary>
        public bool Equals(Tracker other)
        {
            return IPAddress == other.IPAddress && Port == other.Port;
        }
    }
}
