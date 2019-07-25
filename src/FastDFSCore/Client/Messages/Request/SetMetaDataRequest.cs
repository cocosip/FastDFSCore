using System;
using System.Collections.Generic;

namespace FastDFSCore.Client
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
    public class SetMetaDataRequest : FDFSRequest<SetMetaDataResonse>
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

        /// <summary>MetaDataOption选项,<see cref="FastDFSCore.Client.MetaDataOption"/>
        /// </summary>
        public MetaDataOption Option { get; set; }

        /// <summary>Ctor
        /// </summary>
        public SetMetaDataRequest()
        {

        }

        /// <summary>Ctor
        /// </summary>
        /// <param name="fileId">文件FileId</param>
        /// <param name="groupName">组名</param>
        /// <param name="metaData">MetaData</param>
        /// <param name="option">MetaDataOption</param>
        public SetMetaDataRequest(string fileId, string groupName, IDictionary<string, string> metaData, MetaDataOption option)
        {
            FileId = fileId;
            GroupName = groupName;
            MetaData = metaData;
            Option = option;
        }

        /// <summary>EncodeBody
        /// </summary>
        public override byte[] EncodeBody(FDFSOption option)
        {

            string optionString = (Option == MetaDataOption.Overwrite) ? "O" : "M";

            byte[] optionBuffer = option.Charset.GetBytes(optionString);


            byte[] groupNameBuffer = new byte[Consts.FDFS_GROUP_NAME_MAX_LEN];
            var groupNameBytes = option.Charset.GetBytes(GroupName);
            Array.Copy(groupNameBytes, 0, groupNameBuffer, 0, groupNameBytes.Length);

            byte[] fileIdBuffer = option.Charset.GetBytes(FileId);
            byte[] metaDataBuffer = CreateMetaDataBuffer(option, MetaData);

            byte[] fileIdLengthBuffer = Util.LongToBuffer(fileIdBuffer.Length);
            byte[] metaDataSizeBuffer = Util.LongToBuffer(metaDataBuffer.Length);

            int length = Consts.FDFS_PROTO_PKG_LEN_SIZE +  // filename length
                         Consts.FDFS_PROTO_PKG_LEN_SIZE +  // metadata size
                         1 +  // operation flag
                         Consts.FDFS_GROUP_NAME_MAX_LEN +  // group name
                         fileIdBuffer.Length +  // file name
                         metaDataBuffer.Length;            // metadata 

            List<byte> bodyBuffer = new List<byte>();
            bodyBuffer.AddRange(fileIdLengthBuffer);
            bodyBuffer.AddRange(metaDataSizeBuffer);
            bodyBuffer.AddRange(optionBuffer);
            bodyBuffer.AddRange(groupNameBuffer);
            bodyBuffer.AddRange(fileIdBuffer);
            bodyBuffer.AddRange(metaDataBuffer);

            Header = new FDFSHeader(length, Consts.STORAGE_PROTO_CMD_SET_METADATA, 0);

            return bodyBuffer.ToArray();
        }

        private byte[] CreateMetaDataBuffer(FDFSOption option, IDictionary<string, string> metaData)
        {
            List<byte> metaDataBuffer = new List<byte>();
            foreach (KeyValuePair<string, string> p in metaData)
            {
                // insert a separater if this is not the first meta data item.
                if (metaDataBuffer.Count != 0)
                {
                    metaDataBuffer.Add(Consts.METADATA_PAIR_SEPARATER);
                }

                metaDataBuffer.AddRange(Util.StringToByte(option.Charset, p.Key));
                metaDataBuffer.Add(Consts.METADATA_KEY_VALUE_SEPARATOR);
                metaDataBuffer.AddRange(Util.StringToByte(option.Charset, p.Value));
            }
            return metaDataBuffer.ToArray();
        }
    }
}
