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
        public enum NetErrorMsg { CONNECTION_FAIL, DISCONNECTED, TOO_MUCH_TRY_WHEN_READ, TOO_MUCH_TRY_WHEN_WRITE, FAIL_WHEN_CLOSE_SOCKET, UNKNOWN_ERROR
                                    ,FAIL_WHEN_SEND_MSG, FAIL_WHEN_RECV_MSG, READ_TIMEOUT, WRITE_TIMEOUT, RECV_THREAD_FINISHED};
        public NetErrorMsg error { get; set; }
        public int id { get; set; }
        public String errorStr;
        
        /* errorMsg  는 NetErrorMsg 의 멤버이다. */
        public NetworkErrorEventArgs(int id, NetErrorMsg errorMsg, String errorStr="")
            : base()
        {
            this.id = id;
            error = errorMsg;
            this.errorStr = errorStr;
        }
    }

}
