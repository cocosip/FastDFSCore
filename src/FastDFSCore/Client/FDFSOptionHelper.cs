using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace FastDFSCore.Client
{
    /// <summary>FastDFSCore配置信息帮助类
    /// </summary>
    public static class FDFSOptionHelper
    {
        /// <summary>从文件中读取配置信息
        /// </summary>
        public static FDFSOption GetFDFSOption(string file)
        {
            if (!File.Exists(file))
            {
                throw new ArgumentException("配置文件不存在");
            }
            var option = new FDFSOption();
            var doc = new XmlDocument();
            XmlReaderSettings settings = new XmlReaderSettings
            {
                IgnoreComments = true //忽略文档里面的注释
            };
            XmlReader reader = XmlReader.Create(file, settings);
            doc.Load(reader);
            XmlNode root = doc.SelectSingleNode("FDFSClient");

            var allTrackers = root.SelectSingleNode("Trackers");
            //全部的Tracker节点
            var trackerNodes = allTrackers.SelectNodes("Tracker");
            foreach (XmlNode trackerNode in trackerNodes)
            {
                var ipAddress = IPAddress.Parse(trackerNode.SelectSingleNode("IPAddress").InnerText);
                var port = int.Parse(trackerNode.SelectSingleNode("Port").InnerText);
                option.Trackers.Add(new IPEndPoint(ipAddress, port));
            }
            //charset
            option.Charset = Encoding.GetEncoding(root.SelectSingleNode("Charset").InnerText);
            //ConnectionTimeout
            option.ConnectionTimeout = int.Parse(root.SelectSingleNode("ConnectionTimeout").InnerText);
            //ConnectionLifeTime
            option.ConnectionLifeTime = int.Parse(root.SelectSingleNode("ConnectionLifeTime").InnerText);
            //ScanTimeoutConnectionInterval
            option.ScanTimeoutConnectionInterval= int.Parse(root.SelectSingleNode("ScanTimeoutConnectionInterval").InnerText);

            //TrackerMaxConnection
            option.TrackerMaxConnection = int.Parse(root.SelectSingleNode("TrackerMaxConnection").InnerText);
            //StorageMaxConnection
            option.StorageMaxConnection = int.Parse(root.SelectSingleNode("StorageMaxConnection").InnerText);
            //LoggerName
            option.LoggerName = root.SelectSingleNode("LoggerName").InnerText;

            //tcp相关配置
            option.TcpSetting = new TcpSetting();
            //Tcp节点
            var tcpNode = root.SelectSingleNode("TcpSetting");
            //退出后指定时间内关闭(ms)
            option.TcpSetting.QuietPeriodMilliSeconds = int.Parse(tcpNode.SelectSingleNode("QuietPeriodMilliSeconds").InnerText);
            //关闭超时时间(s)
            option.TcpSetting.CloseTimeoutSeconds = int.Parse(tcpNode.SelectSingleNode("CloseTimeoutSeconds").InnerText);
            //高水位
            option.TcpSetting.WriteBufferHighWaterMark = int.Parse(tcpNode.SelectSingleNode("WriteBufferHighWaterMark").InnerText);
            //低水位
            option.TcpSetting.WriteBufferLowWaterMark = int.Parse(tcpNode.SelectSingleNode("WriteBufferLowWaterMark").InnerText);
            //接收socket缓存大小
            option.TcpSetting.SoRcvbuf = int.Parse(tcpNode.SelectSingleNode("SoRcvbuf").InnerText);
            //发送socket缓存大小
            option.TcpSetting.SoSndbuf = int.Parse(tcpNode.SelectSingleNode("SoSndbuf").InnerText);
            //Tcp无延迟发送
            option.TcpSetting.TcpNodelay = bool.Parse(tcpNode.SelectSingleNode("TcpNodelay").InnerText);
            //重用端口号
            option.TcpSetting.SoReuseaddr = bool.Parse(tcpNode.SelectSingleNode("SoReuseaddr").InnerText);

            //关闭读取流
            reader.Close();
            return option;
        }


        /// <summary>将配置文件转换成xml字符串
        /// </summary>
        public static string ToXml(FDFSOption option)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<FDFSClient>");
                sb.AppendLine(ParseNote("Trackers信息,可以配置多台"));
                sb.AppendLine("<Trackers>");
                foreach (var tracker in option.Trackers)
                {
                    sb.AppendLine("<Tracker>");
                    sb.Append("<IPAddress>");
                    sb.Append(tracker.Address.ToIPv4Address());
                    sb.AppendLine("</IPAddress>");

                    sb.Append("<Port>");
                    sb.Append(tracker.Port);
                    sb.AppendLine("</Port>");

                    sb.AppendLine("</Tracker>");
                }
                sb.AppendLine("</Trackers>");
                sb.AppendLine(ParseNote("编码"));
                sb.Append("<Charset>");
                sb.Append(option.Charset.BodyName);
                sb.AppendLine("</Charset>");

                sb.AppendLine(ParseNote("连接超时时间(s)"));
                sb.Append("<ConnectionTimeout>");
                sb.Append(option.ConnectionTimeout);
                sb.AppendLine("</ConnectionTimeout>");

                sb.AppendLine(ParseNote("连接的有效时间(s)"));
                sb.Append("<ConnectionLifeTime>");
                sb.Append(option.ConnectionLifeTime);
                sb.AppendLine("</ConnectionLifeTime>");

                sb.AppendLine(ParseNote("查询超时的连接的时间间隔(s)"));
                sb.Append("<ScanTimeoutConnectionInterval>");
                sb.Append(option.ScanTimeoutConnectionInterval);
                sb.AppendLine("</ScanTimeoutConnectionInterval>");

                sb.AppendLine(ParseNote("最大Tracker连接数"));
                sb.Append("<TrackerMaxConnection>");
                sb.Append(option.TrackerMaxConnection);
                sb.AppendLine("</TrackerMaxConnection>");

                sb.AppendLine(ParseNote("最大Storage连接数"));
                sb.Append("<StorageMaxConnection>");
                sb.Append(option.StorageMaxConnection);
                sb.AppendLine("</StorageMaxConnection>");

                sb.Append("<LoggerName>");
                sb.Append(option.LoggerName);
                sb.AppendLine("</LoggerName>");

                //Tcp参数设置
                sb.AppendLine("<TcpSetting>");

                sb.AppendLine(ParseNote("退出后在指定时间内关闭(ms)"));
                sb.Append("<QuietPeriodMilliSeconds>");
                sb.Append(option.TcpSetting.QuietPeriodMilliSeconds);
                sb.AppendLine("</QuietPeriodMilliSeconds>");

                sb.AppendLine(ParseNote("超时关闭时间(s)"));
                sb.Append("<CloseTimeoutSeconds>");
                sb.Append(option.TcpSetting.CloseTimeoutSeconds);
                sb.AppendLine("</CloseTimeoutSeconds>");

                sb.AppendLine(ParseNote("高水位"));
                sb.Append("<WriteBufferHighWaterMark>");
                sb.Append(option.TcpSetting.WriteBufferHighWaterMark);
                sb.AppendLine("</WriteBufferHighWaterMark>");

                sb.AppendLine(ParseNote("低水位"));
                sb.Append("<WriteBufferLowWaterMark>");
                sb.Append(option.TcpSetting.WriteBufferLowWaterMark);
                sb.AppendLine("</WriteBufferLowWaterMark>");

                sb.AppendLine(ParseNote("接收的Socket缓存大小"));
                sb.Append("<SoRcvbuf>");
                sb.Append(option.TcpSetting.SoRcvbuf);
                sb.AppendLine("</SoRcvbuf>");

                sb.AppendLine(ParseNote("发送的Socket缓存大小"));
                sb.Append("<SoSndbuf>");
                sb.Append(option.TcpSetting.SoSndbuf);
                sb.AppendLine("</SoSndbuf>");

                sb.AppendLine(ParseNote("Tcp发送无延迟"));
                sb.Append("<TcpNodelay>");
                sb.Append(option.TcpSetting.TcpNodelay);
                sb.AppendLine("</TcpNodelay>");

                sb.AppendLine(ParseNote("是否重用端口"));
                sb.Append("<SoReuseaddr>");
                sb.Append(option.TcpSetting.SoReuseaddr);
                sb.AppendLine("</SoReuseaddr>");

                sb.AppendLine("</TcpSetting>");

                sb.AppendLine("</FDFSClient>");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"生成Xml配置文件出错!{ex.Message}", ex);
            }
        }


        private static string ParseNote(string note)
        {
            return $"<![CDATA[{note}]]>";
        }



    }
}
