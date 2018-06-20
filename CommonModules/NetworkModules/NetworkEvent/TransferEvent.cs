using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkModules
{
    public delegate void TransferEventHandler(object sender, TransferEventArgs e);
    /*    
        public static class NetErrorMsg
        {
            public static int CONNECTION_FAIL = 0;
            public static int DISCONNECTED = 1;
        }
        */
    public class TransferEventArgs : EventArgs
    {
        public enum TransferMode
        {
            Send=0, Receive
        };
        public TransferMode tmode { get; set; }
        public int id { get; set; }
        public String description;
        public int size;
        public int totalSize;

        /* errorMsg  는 NetErrorMsg 의 멤버이다. */
        public TransferEventArgs(int id, TransferMode transferMode, int size, int totalSize, String description = "")
            : base()
        {
            this.id = id;
            tmode = transferMode;
            this.description = description;
            this.size = size;
            this.totalSize = totalSize;
        }
    }
}
