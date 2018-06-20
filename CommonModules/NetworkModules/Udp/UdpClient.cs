using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RtwEnums.Network;
namespace NetworkModules
{
    public class UdpClientRTW:UdpClientBase
    {

        public UdpClientRTW(int id=-1,SendType type=SendType.Normal)
            : base(type)
        {
            this.id = id;
        }
        public UdpClientRTW(SendType type)
            : base(type)
        {
            this.id = -1;
        }
        public UdpClientRTW(int id)
            : base(SendType.Normal)
        {
            this.id = id;
        }

        public int id = -1;
        public event ConnectionEventHandler ServerConnected;
        public event ConnectionEventHandler ServerDisconnected;
        public event NetworkErrorEventHandler ReadTimeout;
        public event NetworkErrorEventHandler WriteTimeout;
        public event NetworkErrorEventHandler NetError;
        public event TransferEventHandler OnReceived;
        public event NetworkErrorEventHandler OnReadFail;

        protected override void BeginAClient()
        {
            if (ServerConnected != null) ServerConnected(id, new ConnectionEventArgs(ConnType.Connected, "", id));
            
            base.BeginAClient();
        }

        protected override void FinishAClient()
        {
            if (ServerDisconnected != null) ServerDisconnected(id, new ConnectionEventArgs(ConnType.Disconnected, "", id)); 
            base.FinishAClient();
        }

        public override void OnReadTimeout()
        {
            if (ReadTimeout != null) ReadTimeout(id, new NetworkErrorEventArgs(id, NetworkErrorEventArgs.NetErrorMsg.READ_TIMEOUT, "ReadTimeout"));
            base.OnReadTimeout();
        }
        protected override void OnRead(int size, int totalSize)
        {
            if (OnReceived != null && _isRecvEventEnabled) OnReceived(id, new TransferEventArgs(id, TransferEventArgs.TransferMode.Receive, size, totalSize, "ReceiveData"));
        }
        public override void OnConnectionFailed(String errMsg="")
        {
            if (NetError != null) NetError(id, new NetworkErrorEventArgs(id, NetworkErrorEventArgs.NetErrorMsg.CONNECTION_FAIL, errMsg));
            base.OnConnectionFailed();
        }
        protected override void OnReadFails(String errMsg="")
        {
            if (OnReadFail != null) OnReadFail(id, new NetworkErrorEventArgs(id, NetworkErrorEventArgs.NetErrorMsg.FAIL_WHEN_RECV_MSG, "ReadFail:"+errMsg));
            base.OnReadFails();
        }
    }
}
