using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RtwEnums.Network;

namespace NetworkModules
{
    public class TcpClientRTW:TcpClientBase
    {
        public TcpClientRTW(int id = -1,SendType type=SendType.Normal)
            : base(id,type)
        {
        }
        public TcpClientRTW(int id)
            : base(id, SendType.Normal)
        {
        }
        public TcpClientRTW(SendType type)
            : base(-1, type)
        {
        }
        
        public event ConnectionEventHandler RecvLoopStart;
        public event ConnectionEventHandler RecvLoopEnd;
        public event ConnectionEventHandler ConnectionEventOccured;
        public event NetworkErrorEventHandler ReadTimeout;
        public event NetworkErrorEventHandler WriteTimeout;
        public event NetworkErrorEventHandler NetError;
        public event TransferEventHandler OnReceived;
        public event NetworkErrorEventHandler OnReadFail;

        protected override void ConnectionEvent(ConnType conn)
        {
            if (ConnectionEventOccured != null) ConnectionEventOccured(_id, new ConnectionEventArgs(conn,"",_id));
        }
        protected override void NetworkError(NetworkErrorEventArgs e)
        {
            if (NetError != null) NetError(_id, e);
            base.NetworkError(e);
        }
        protected override void BeginAClient()
        {
            if (RecvLoopStart != null) RecvLoopStart(_id, new ConnectionEventArgs(ConnType.Connected, "", _id));
            
            base.BeginAClient();
        }

        protected override void FinishAClient()
        {
            if (RecvLoopEnd != null) RecvLoopEnd(_id, new ConnectionEventArgs(ConnType.Disconnected, "", _id)); 
            base.FinishAClient();
        }

        public override void OnReadTimeout()
        {
            if (ReadTimeout != null) ReadTimeout(_id, new NetworkErrorEventArgs(_id, NetworkErrorEventArgs.NetErrorMsg.READ_TIMEOUT, "ReadTimeout"));
            base.OnReadTimeout();
        }
        protected override void OnRead(int size, int totalSize)
        {
            if (OnReceived != null && _isRecvEventEnabled) OnReceived(_id, new TransferEventArgs(_id, TransferEventArgs.TransferMode.Receive, size, totalSize, "ReceiveData"));
        }
        /*
        public override void OnConnectionFailed(String errMsg="")
        {
            //if (NetError != null) NetError(_id, new NetworkErrorEventArgs(_id, NetworkErrorEventArgs.NetErrorMsg.CONNECTION_FAIL, errMsg));
            base.OnConnectionFailed(errMsg);
        }
         */
        protected override void OnReadFails(String errMsg="")
        {
            if (OnReadFail != null) OnReadFail(_id, new NetworkErrorEventArgs(_id, NetworkErrorEventArgs.NetErrorMsg.FAIL_WHEN_RECV_MSG, "ReadFail:"+errMsg));
            base.OnReadFails();
        }

    }
}
