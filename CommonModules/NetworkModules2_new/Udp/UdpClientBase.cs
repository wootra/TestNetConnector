using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using DataHandling;
using RtwEnums.Network;

namespace NetworkModules
{
    public class UdpClientBase:UdpClient,INetConnector
    {
        public event ConnectionEventHandler E_Connection;
        public event NetworkErrorEventHandler E_NetError;
        public event TransferEventHandler E_OnReceived;

        #region properties
        //private StreamHandler _sh;
        private Thread _thread;
        private Boolean _isEndConnection;
        private InterfaceFunctions _functions;
        public delegate void CallBackFunc();
        private CallBackFunc _callBackFunc;

        private int _headerSize = 32;
        protected BufferQueue _queue;
        int _id = -1;
        #endregion
        public enum SendType { Normal = 0, Multicast = 1, Broadcast = 2 };
        SendType _sendType = SendType.Normal;
        public UdpClientBase(int id=-1, SendType type=SendType.Normal):base()
        {
            _id = id;
            _sendType = type;
            _isEndConnection = true;
            _callBackFunc = null;
            _queue = new BufferQueue();
            _functions = new InterfaceFunctions(this, _queue);
        }

        public void setHeaderSize(int byteSize)
        {
            _headerSize = byteSize;
        }
        public new int Available
        {
            get
            {
                return _queue.Size;
            }
        }

        public InterfaceFunctions Interface
        {
            get { return _functions; }
        }

        #region Virtual functions

        protected virtual void funcRunningInServerLoopForClient() { }

        protected virtual void BeginAClient() { }

        protected virtual void FinishAClient() { }
        public virtual void ConnectionEvent(ConnType connectionState) { }

        #endregion
        
        public bool isConnected()
        {
            
            return true;
        }
        /*
        public StreamHandler getStreamHandler()
        {
            return _sh;
        }
        */

        public string getDestIp()
        {
            return _serverIp;
        }
        /*
        public void setEchoRun()
        {
            _callBackFunc = Echo;
        }
        */

        //vurtual function should override in child class
        //if timeout is 0, networkStream will use default timeout.
        int _readTimeout = 0;
        int _writeTimeout = 0;
        EndPoint _localEP;
        EndPoint _remoteEP;

        String _serverIp;
        int _serverPort;
        public void ConnectToServer(string server, int port, int readTimeout = 0, int writeTimeout = 0)
        {
            setServerInfo(server, port, readTimeout, writeTimeout);
            Connect(true);
            //_localEP = new IPEndPoint(IPAddress.Any, port); //IPAddress.Any 는 서버전용.
//            setServer(server, port, null, readTimeout, writeTimeout);
        }

        public void setServerInfo(string server, int port, int readTimeout = 0, int writeTimeout = 0)
        {
            _readTimeout = readTimeout;
            _writeTimeout = writeTimeout;
            _serverIp = server;
            _serverPort = port;

            _isEndConnection = false;
            _localEP = new IPEndPoint(NetFunctions.getMyIP4Address(), IPEndPoint.MinPort + 1); //this is blank IPEndPoint yet.
            //_localEP = new IPEndPoint(IPAddress.None, port); //this is blank IPEndPoint yet.

            if (server != null && server.Length > 7) _remoteEP = new IPEndPoint(NetFunctions.getIP4Address(_serverIp), _serverPort);
            else _remoteEP = new IPEndPoint(NetFunctions.getMyIP4Address(), _serverPort);

            
            //            setServer(server, port, null, readTimeout, writeTimeout);
        }

        public void Connect(bool runReceiveThreadWhenConnected){
            _queue.Clear();
            if((_thread!=null && _thread.ThreadState == ThreadState.Running) || this.isConnected()) Disconnect();


            if (_sendType == SendType.Normal)
            {
                //_uSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                base.Connect(_serverIp, _serverPort);
                
                //_uSocket.Bind(_localEP);
            }
            else if (_sendType == SendType.Multicast)
            {
                base.Connect((IPEndPoint)_remoteEP);
                base.JoinMulticastGroup(NetFunctions.getIP4Address(_serverIp), NetFunctions.getMyIP4Address());

                //_uSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                //_uSocket.Bind(_localEP);
                //_uSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(IPAddress.Parse(server)));

            }
            else //broadcast
            {
                base.Connect((IPEndPoint)_remoteEP);
                base.Client.Bind(_localEP);
                base.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.Broadcast, true);
            }

            if (runReceiveThreadWhenConnected) runRecvThread();
        }

        enum TimeoutKind { Read = 0, Write };
        void onTick(object sender)
        {
            TimeoutKind k = (TimeoutKind)sender;
            switch (k)
            {
                case TimeoutKind.Read:
                    if (E_NetError != null) E_NetError(this, new NetworkErrorEventArgs(_id, NetworkErrorEventArgs.NetErrorMsg.READ_TIMEOUT));
                    break;
                case TimeoutKind.Write:
                    if (E_NetError != null) E_NetError(this, new NetworkErrorEventArgs(_id, NetworkErrorEventArgs.NetErrorMsg.WRITE_TIMEOUT));
                    break;
            }
        }

        public void setFuncInLoop(CallBackFunc callBackFunc)
        {
            _callBackFunc = callBackFunc;
        }
        public void removeFuncInLoop()
        {
            _callBackFunc = null;
        }

        public void runRecvThread(){
            if (_thread == null || _thread.IsAlive == false)
            {
                _thread = new Thread(new ThreadStart(this.recvLoop));
                _thread.Start();
            }
            else
            {
                _thread.Start();
            }
        }

        public void Disconnect(Func<int> runWhenCloseSocket = null)
        {
            _queue.Clear();
            if (_isEndConnection == true) return;
            endThisClient();
            if (_thread!=null && _thread.IsAlive)
            {
                _isEndConnection = true;
                
                try
                {
                    if(runWhenCloseSocket!=null) runWhenCloseSocket.Invoke();
                    this.Close();

                    //_uSocket.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                this.Client.Blocking = false;
                this.Client.ReceiveTimeout = 10;
                this.Client.SendTimeout = 10;


                _thread.Join(100);
                if (_thread != null && _thread.ThreadState == ThreadState.Running)
                {
                    Thread tt = _thread;
                    _thread = null;
                    FinishAClient();
                    tt.Abort();
                }
                
               // _thread.Join();
            }
            _thread = null;
        }

        public void endThisClient()
        {
            //if(_sh!=null) _sh.closeStream();
            _isEndConnection = true;
           // _isRecvQueueUsing = false;

        }

        int loopZero = 3;
        private void recvLoop()
        {

            Console.WriteLine("server ready...");


            _isEndConnection = false;
            Console.WriteLine("accept client!");
            //_sh.setStream(_client.GetStream(),_client.Client);
            BeginAClient();

            while (!_isEndConnection)
            {
                funcRunningInServerLoopForClient();
                if (_callBackFunc != null) _callBackFunc();

                int available = base.Available;
                Byte[] buff;
                int size;
                if (available > 0)
                {
                    buff = new Byte[available];
                    size = this.Client.Receive(buff, 0, available, SocketFlags.None);
                }
                else
                {
                    buff = new byte[_headerSize]; //available이 0일때는 다음에 도착할 header를 기다린다.
                    size = this.Client.Receive(buff, 0, _headerSize, SocketFlags.None);
                }
                if (size > 0)
                {
                    if (size == buff.Length) _queue.Enqueue(buff);
                    else _queue.enqueueFrom(buff, 0, size);

                    if (E_OnReceived != null) E_OnReceived(this, new TransferEventArgs(_id, TransferEventArgs.TransferMode.Receive, size, _queue.Size));
                    loopZero = 3;
                }
                else loopZero--;

                if (loopZero == 0 || size < 0)
                {
                    if (E_NetError != null) E_NetError(this, new NetworkErrorEventArgs(_id, NetworkErrorEventArgs.NetErrorMsg.READ_TIMEOUT));
                    break;
                }
                if (size == 0) continue;

            }

            FinishAClient();
            ConnectionEvent(ConnType.Disconnected);
            //close();
            //OnConnectionFailed("Connection closed..");
        }

        public int readFromNet(Byte[] buff, int offset, int size)
        {
            return this.Client.ReceiveFrom(buff, offset, size, SocketFlags.None, ref _remoteEP);
        }

        public int write(Byte[] buff, int offset, int size)
        {
            return this.Client.SendTo(buff, offset, size, SocketFlags.None, _remoteEP);
        }





        public void ServerReady(string server, int port, int readTimeout = 0, int writeTimeout = 0)
        {
            ConnectToServer(server, port, readTimeout, writeTimeout);
        }

        public void ReadyForClient()
        {
            Connect(true);
        }


    }
}
