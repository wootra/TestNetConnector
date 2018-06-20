using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RtwEnums.Network;
namespace NetworkModules
{
    public class UdpServer:UdpServerBase
    {
        public UdpServer(int id = -1, SendType sendType = SendType.Normal)
            : base(sendType)
        {
            this.id = id;
        }
        public UdpServer(int id)
            : base(SendType.Normal)
        {
            this.id = id;
        }
        public UdpServer(SendType sendType)
            : base(sendType)
        {
            this.id = -1;
        }

        public int id = -1;
        public event ConnectionEventHandler ClientConnected;
        public event ConnectionEventHandler ClientDisconnected;
        public event NetworkErrorEventHandler ReadTimeout;
        public event NetworkErrorEventHandler NetError;
        public event TransferEventHandler OnReceived;
        public event NetworkErrorEventHandler OnReadFail;


        protected override void BeginAClient()
        {
            if (ClientConnected != null) ClientConnected(id, new ConnectionEventArgs(ConnType.Connected, "", id));
            
            base.BeginAClient();
        }

        protected override void FinishAClient()
        {
            if (ClientDisconnected != null) ClientDisconnected(id, new ConnectionEventArgs(ConnType.Disconnected, "", id)); 
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
        public override void OnConnectionFailed()
        {
            if (NetError != null) NetError(id, new NetworkErrorEventArgs(id, NetworkErrorEventArgs.NetErrorMsg.CONNECTION_FAIL));
            base.OnConnectionFailed();
        }
        protected override void OnReadFails()
        {
            if (OnReadFail != null) OnReadFail(id, new NetworkErrorEventArgs(id, NetworkErrorEventArgs.NetErrorMsg.FAIL_WHEN_RECV_MSG, "ReadFail"));
            base.OnReadFails();
        }
    }
}
