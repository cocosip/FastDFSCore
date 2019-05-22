using System;
using System.Collections.Generic;

namespace FastDFSCore.Client
{
    public class GetMetaDataResponse : FDFSResponse
    {
        public IDictionary<string, string> MetaData { get; set; }


        public GetMetaDataResponse()
        {

        }

        public override void LoadContent(FDFSOption option, byte[] data)
        {
            MetaData = new Dictionary<string, string>();
            int itemSeparaterIndex = -1;
            int keyValueSeparaterIndex = -1;
            int startIndex = 0;

            do
            {
                string key = null, value = null;

                keyValueSeparaterIndex = Array.IndexOf<byte>(data, Consts.METADATA_KEY_VALUE_SEPARATOR, startIndex);
                if (keyValueSeparaterIndex < 0)
                    throw new ArgumentException("invalid metadata buffer format");

                key = Util.ByteToString(option.Charset, data, startIndex, keyValueSeparaterIndex - startIndex);
                startIndex = keyValueSeparaterIndex + 1;

                itemSeparaterIndex = Array.IndexOf<byte>(data, Consts.METADATA_PAIR_SEPARATER, startIndex);

                if (itemSeparaterIndex < 0)
                    value = Util.ByteToString(option.Charset, data, startIndex, (data.Length - 1) - startIndex);
                else
                    value = Util.ByteToString(option.Charset, data, startIndex, itemSeparaterIndex - startIndex);
                startIndex = itemSeparaterIndex + 1;

                MetaData.Add(key, value);
            } while (itemSeparaterIndex >= 0);
        }
    }
}
