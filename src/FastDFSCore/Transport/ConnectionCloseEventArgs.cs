using System;

namespace FastDFSCore.Transport
{
    public class ConnectionCloseEventArgs : EventArgs
    {
        public string Id { get; set; }

        public ConnectionAddress ConnectionAddress { get; set; }

    }
}
