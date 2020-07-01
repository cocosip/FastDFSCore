using System;

namespace FastDFSCore.Transport
{
    public class DisconnectEventArgs : EventArgs
    {
        public string Id { get; set; }
        public ConnectionAddress ConnectionAddress { get; set; }
    }
}
