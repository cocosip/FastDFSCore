using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace FastDFSCore.Client
{
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
            //TrackerMaxConnection
            option.TrackerMaxConnection = int.Parse(root.SelectSingleNode("TrackerMaxConnection").InnerText);
            //StorageMaxConnection
            option.StorageMaxConnection = int.Parse(root.SelectSingleNode("StorageMaxConnection").InnerText);
            //LoggerName
            option.LoggerName = root.SelectSingleNode("LoggerName").InnerText;
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
                sb.AppendLine("<Trackers>");
                foreach (var tracker in option.Trackers)
                {
                    sb.AppendLine("<Tracker>");
                    sb.Append("<IPAddress>");
                    sb.Append(tracker.Address.ToIPv4Address());
                    sb.AppendLine("</IPAddress>");

                    sb.Append("<Port>");
                    sb.Append(tracker.Port.ToString());
                    sb.AppendLine("</Port>");

                    sb.AppendLine("</Tracker>");
                }
                sb.AppendLine("</Trackers>");

                sb.Append("<Charset>");
                sb.Append(option.Charset.BodyName);
                sb.AppendLine("</Charset>");

                sb.Append("<ConnectionTimeout>");
                sb.Append(option.ConnectionTimeout.ToString());
                sb.AppendLine("</ConnectionTimeout>");

                sb.Append("<ConnectionLifeTime>");
                sb.Append(option.ConnectionLifeTime.ToString());
                sb.AppendLine("</ConnectionLifeTime>");

                sb.Append("<TrackerMaxConnection>");
                sb.Append(option.TrackerMaxConnection.ToString());
                sb.AppendLine("</TrackerMaxConnection>");

                sb.Append("<StorageMaxConnection>");
                sb.Append(option.StorageMaxConnection.ToString());
                sb.AppendLine("</StorageMaxConnection>");

                sb.Append("<LoggerName>");
                sb.Append(option.LoggerName);
                sb.AppendLine("</LoggerName>");

                sb.AppendLine("</FDFSClient>");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"生成Xml配置文件出错!{ex.Message}", ex);
            }
        }




    }
}
