using FastDFSCore.Utility;
using System;
using System.Collections.Generic;

namespace FastDFSCore.Protocols
{
    /// <summary>获取文件MetaData返回
    /// </summary>
    public class GetMetaDataResp : FastDFSResp
    {
        /// <summary>MetaData集合
        /// </summary>
        public IDictionary<string, string> MetaData { get; set; }

        /// <summary>Ctor
        /// </summary>
        public GetMetaDataResp()
        {

        }

        /// <summary>LoadContent
        /// </summary>
        public override void LoadContent(FastDFSOption option, byte[] data)
        {
            MetaData = new Dictionary<string, string>();
            int startIndex = 0;

            int itemSeparaterIndex;
            do
            {
                int keyValueSeparaterIndex = Array.IndexOf<byte>(data, Consts.METADATA_KEY_VALUE_SEPARATOR, startIndex);
                if (keyValueSeparaterIndex < 0)
                    throw new ArgumentException("invalid metadata buffer format");

                string key = ByteUtil.ByteToString(data, startIndex, keyValueSeparaterIndex - startIndex, option.Charset);
                startIndex = keyValueSeparaterIndex + 1;

                itemSeparaterIndex = Array.IndexOf<byte>(data, Consts.METADATA_PAIR_SEPARATER, startIndex);
                string value;
                if (itemSeparaterIndex < 0)
                    value = ByteUtil.ByteToString(data, startIndex, (data.Length - 1) - startIndex, option.Charset);
                else
                    value = ByteUtil.ByteToString(data, startIndex, itemSeparaterIndex - startIndex, option.Charset);
                startIndex = itemSeparaterIndex + 1;

                MetaData.Add(key, value);
            } while (itemSeparaterIndex >= 0);
        }
    }
}
