using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkModules
{
    public delegate void NetworkErrorEventHandler(object sender, NetworkErrorEventArgs e);
/*    
    public static class NetErrorMsg
    {
        public static int CONNECTION_FAIL = 0;
        public static int DISCONNECTED = 1;
    }
    */
    public class NetworkErrorEventArgs : EventArgs
    {
        public enum NetErrorMsg { CONNECTION_FAIL, DISCONNECTED };
        private NetErrorMsg error { get; set; }
        public int id { get; set; }
        
        /* errorMsg  는 NetErrorMsg 의 멤버이다. */
        public NetworkErrorEventArgs(int id, NetErrorMsg errorMsg)
            : base()
        {
            this.id = id;
            error = errorMsg;
        }
    }

}
