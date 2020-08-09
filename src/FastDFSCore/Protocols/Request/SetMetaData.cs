using FastDFSCore.Utility;
using System.Buffers;
using System.Collections.Generic;

namespace FastDFSCore.Protocols
{
    /// <summary>
    /// set metat data from storage server
    /// 
    /// Reqeust 
    ///     Cmd: STORAGE_PROTO_CMD_SET_METADATA 13
    ///     Body:
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: filename length 
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: meta data size 
    ///     @ 1 bytes: operation flag,  
    ///         'O' for overwrite all old metadata 
    ///         'M' for merge, insert when the meta item not exist, otherwise update it
    ///     @ FDFS_GROUP_NAME_MAX_LEN bytes: group name 
    ///     @ filename bytes: filename
    ///     @ meta data bytes: each meta data seperated by \x01,
    ///         name and value seperated by \x02
    /// Response
    ///     Cmd: STORAGE_PROTO_CMD_RESP
    ///     Status: 0 right other wrong
    ///     Body: 
    ///         
    /// </summary>
    public class SetMetaData : FastDFSReq<SetMetaDataResp>
    {
        /// <summary>文件FileId
        /// </summary>
        public string FileId { get; set; }

        /// <summary>组名
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>MetaData字典
        /// </summary>
        public IDictionary<string, string> MetaData { get; set; }

        /// <summary>MetaDataOption选项,<see cref="FastDFSCore.MetaDataOption"/>
        /// </summary>
        public MetaDataOption Option { get; set; }

        /// <summary>Ctor
        /// </summary>
        public SetMetaData()
        {
            Header = new FastDFSHeader(Consts.STORAGE_PROTO_CMD_SET_METADATA);
        }

        /// <summary>Ctor
        /// </summary>
        /// <param name="fileId">文件FileId</param>
        /// <param name="groupName">组名</param>
        /// <param name="metaData">MetaData</param>
        /// <param name="option">MetaDataOption</param>
        public SetMetaData(string fileId, string groupName, IDictionary<string, string> metaData, MetaDataOption option) : this()
        {
            FileId = fileId;
            GroupName = groupName;
            MetaData = metaData;
            Option = option;
        }

        /// <summary>EncodeBody
        /// </summary>
        public override byte[] EncodeBody(ClusterConfiguration configuration)
        {
            string optionString = (Option == MetaDataOption.Overwrite) ? "O" : "M";
            var optionBuffer = EndecodeUtil.EncodeString(optionString, configuration.Charset);

            var groupNameBuffer = EndecodeUtil.EncodeGroupName(GroupName, configuration.Charset);


            var fileIdBuffer = ByteUtil.StringToByte(FileId, configuration.Charset);
            var metaDataBuffer = CreateMetaDataBuffer(configuration, MetaData);

            var fileIdLengthBuffer = ByteUtil.LongToBuffer(fileIdBuffer.Length);
            var metaDataSizeBuffer = ByteUtil.LongToBuffer(metaDataBuffer.Length);

            //int length = Consts.FDFS_PROTO_PKG_LEN_SIZE +  // filename length
            //             Consts.FDFS_PROTO_PKG_LEN_SIZE +  // metadata size
            //             1 +  // operation flag
            //             Consts.FDFS_GROUP_NAME_MAX_LEN +  // group name
            //             fileIdBuffer.Length +  // file name
            //             metaDataBuffer.Length;            // metadata 


            return ByteUtil.Combine(fileIdLengthBuffer, metaDataSizeBuffer, optionBuffer, groupNameBuffer, fileIdBuffer, metaDataBuffer);
        }

        private byte[] CreateMetaDataBuffer(ClusterConfiguration configuration, IDictionary<string, string> metaData)
        {
            List<byte> metaDataBuffer = new List<byte>();
            foreach (KeyValuePair<string, string> p in metaData)
            {
                // insert a separater if this is not the first meta data item.
                if (metaDataBuffer.Count != 0)
                {
                    metaDataBuffer.Add(Consts.METADATA_PAIR_SEPARATER);
                }

                metaDataBuffer.AddRange(EndecodeUtil.EncodeString(p.Key, configuration.Charset));
                metaDataBuffer.Add(Consts.METADATA_KEY_VALUE_SEPARATOR);
                metaDataBuffer.AddRange(EndecodeUtil.EncodeString(p.Value, configuration.Charset));
            }
            return metaDataBuffer.ToArray();
        }
    }
}
