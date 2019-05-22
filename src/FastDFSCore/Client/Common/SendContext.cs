namespace FastDFSCore.Client
{
    /// <summary>消息发送上下文
    /// </summary>
    public class SendContext
    {
        public FDFSRequest Request { get; set; }

        /// <summary>响应消息
        /// </summary>
        public FDFSResponse Response { get; set; }

        public SendContext()
        {

        }

        public SendContext(FDFSRequest request, FDFSResponse response)
        {
            Request = request;
            Response = response;
        }
    }
}
