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
using IOHandling;
using NetworkPacket;

namespace NetworkModules4
{
    public class UdpClientBase:INetConnector
    {
        public event ConnectionEventHandler E_Connection;
        public event NetworkErrorEventHandler E_NetError;
        public event TransferEventHandler E_OnCanReceive;
        public event TransferEventHandler E_OnReceivedInQueue;
        
        #region properties
        //private StreamHandler _sh;
        //private Thread _thread;
        //private Boolean _isEndConnection;
        private InterfaceFunctions _functions;
        public delegate void CallBackFunc();
        private CallBackFunc _callBackFunc;
        private byte[] _tempRecvBuffer;
        //private int _headerSize = 32;
        protected BufferQueue _queue;
        Dictionary<int, Thread> _clientThreads = new Dictionary<int, Thread>();//모든 연결 thread를 관리하기 위한 thread묶음..
        Dictionary<int, UdpClient> _clients = new Dictionary<int, UdpClient>(); //각 thread에 종속된 소켓모음.. 관리되지 않는 소켓이 없게 하기 위함..
        
        int _id = -1;
        int _threadId = 0;

        #endregion
        public enum SendType { Normal = 0, Multicast = 1, Broadcast = 2 };
        SendType _sendType = SendType.Normal;
        public UdpClientBase(int id=-1, SendType type=SendType.Normal):base()
        {
            _tempRecvBuffer = new byte[1024000];//1MByte
            _id = id;
            _sendType = type;
            //_isEndConnection = true;
            _callBackFunc = null;
            _queue = new BufferQueue();
            _functions = new InterfaceFunctions(this, _queue);
        }

        public UdpClientBase(Byte[] tempRecvBuffer, SendType type = SendType.Normal, int id=-1)
            : base()
        {
            _tempRecvBuffer = tempRecvBuffer;
            _id = id;
            _sendType = type;
            //_isEndConnection = true;
            _callBackFunc = null;
            _queue = new BufferQueue();
            _functions = new InterfaceFunctions(this, _queue);
        }
        /*
        public void setHeaderSize(int byteSize)
        {
            _headerSize = byteSize;
        }
         */
        public new int Available
        {
            get
            {
                if (_recvPos == RecvPositions.AfterQueueEntered)
                //if (_recvThread != null && _recvThread.ThreadState == ThreadState.Running)
                {
                    return _queue.Size;
                }
                else
                {
                    return _totalSize;// return _server.Available;
                }
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
        public virtual void ConnectionEvent(ConnType connectionState) {
            if(E_Connection!=null) E_Connection(this, new ConnectionEventArgs(connectionState,"",_id));
        }

        #endregion
        bool _isConnected = false;
        public bool isConnected()
        {
            
            return _isConnected;
        }
        /*
        public StreamHandler getStreamHandler()
        {
            return _sh;
        }
        */

        public string getRemoteIp()
        {
            return _serverIp;
        }
        /*
        public void setEchoRun()
        {
            _callBackFunc = Echo;
        }
        */

        void RemoveThread(int id)
        {
            if (_clientThreads.ContainsKey(id))
            {
                try
                {
                    _clients[id].Client.Blocking = false;
                    _clients[id].Client.Disconnect(false);
                    _clients[id].Client.Close(1000);
                }
                catch { }
                _clients.Remove(id);
                Thread t = _clientThreads[id];
                _clientThreads.Remove(id);

                try
                {
                    _clientThreads[id].Abort();
                    _clientThreads[id].Join(1000);
                }
                catch (Exception e)
                {
                    Win32APIs.SendMsgData("UdpClientBase.RemoveThread:" + e.Message, "LogWindow");

                }

            }
        }


        public Byte[] TempRecvBuffer
        {
            get { return _tempRecvBuffer; }
        }

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

            //_isEndConnection = false;
            _localEP = new IPEndPoint(NetFunctions.getMyIP4Address(), IPEndPoint.MinPort + 1); //this is blank IPEndPoint yet.
            //_localEP = new IPEndPoint(IPAddress.None, port); //this is blank IPEndPoint yet.

            if (server != null && server.Length > 7) _remoteEP = new IPEndPoint(NetFunctions.getIP4Address(_serverIp), _serverPort);
            else _remoteEP = new IPEndPoint(NetFunctions.getMyIP4Address(), _serverPort);

            
            //            setServer(server, port, null, readTimeout, writeTimeout);
        }

        public void Connect(bool runReceiveThreadWhenConnected){
            
            releaseThreads();


            _queue.Clear();
            
            //if((_thread!=null && _thread.ThreadState == ThreadState.Running) || this.isConnected()) Disconnect();
            int id = _threadId;
            UdpClient conn = new UdpClient();
            _clients.Add(id, conn);
            _queue.Clear();//이전 것은 의미가 없으므로 다시 큐를 비워줌..
           
            if (_sendType == SendType.Normal)
            {
                //_uSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                conn.Connect(_serverIp, _serverPort);
                
                //_uSocket.Bind(_localEP);
            }
            else if (_sendType == SendType.Multicast)
            {
                conn.Connect((IPEndPoint)_remoteEP);
                conn.JoinMulticastGroup(NetFunctions.getIP4Address(_serverIp), NetFunctions.getMyIP4Address());

                //_uSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                //_uSocket.Bind(_localEP);
                //_uSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(IPAddress.Parse(server)));

            }
            else //broadcast
            {
                conn.Connect((IPEndPoint)_remoteEP);
                conn.Client.Bind(_localEP);
                conn.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.Broadcast, true);
            }
            _isConnected = true;
            if (runReceiveThreadWhenConnected) runRecvThread();
        }

        public void releaseThreads()
        {
            _threadId++;
            Dictionary<int, Thread> threads = new Dictionary<int, Thread>(_clientThreads);//삭제중에 바뀌는 것을 방지위해..
            foreach (int id in threads.Keys)
            {
                try
                {
                    RemoveThread(id);
                }
                catch { }
            }

            //releaseRecvThread();
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
            Thread thread;
            if (_clientThreads.ContainsKey(_threadId) == false || _clientThreads[_threadId] == null)
            {
                thread = new Thread(new ThreadStart(this.recvLoop));
                _clientThreads[_threadId] = thread;
            }
            else
            {
                thread = _clientThreads[_threadId];
            }
    
            thread.Start();
        }
        public void releaseResources()
        {

            _queue.Clear();

        }
        
        /// <summary>
        /// 모든 실행중인 thread와 연결시도를 끝낸다.
        /// </summary>
        public void Close()
        {

            //_connectionTimer.Stop();
            //_isDisposing = true;

            //releaseThreads();
            Disconnect();

        }

        public void Disconnect(Func<int> funcRunningBeforeCloseSocket = null)
        {
            if (funcRunningBeforeCloseSocket != null) funcRunningBeforeCloseSocket.Invoke();
            releaseThreads();
            releaseResources();
            _isConnected = false;
            ConnectionEvent(ConnType.Disconnected);
            //_connectionState = ConnType.Disconnected;
        }

        /*
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
         */

        enum RecvPositions { AfterRecved, AfterQueueEntered };
        RecvPositions _recvPos = RecvPositions.AfterRecved;
        int _totalSize = 0;
       // int loopZero = 3;
        private void recvLoop()
        {
            int id = _threadId;
            UdpClient conn = _clients[id];

            Console.WriteLine("server ready...");


            Console.WriteLine("accept client!");
            //_sh.setStream(_client.GetStream(),_client.Client);
            BeginAClient();
            int errCount = 0;
            while (id == _threadId)
            {
                funcRunningInServerLoopForClient();
                if (_callBackFunc != null) _callBackFunc();

                
                int size = 0;
                try
                {
                    size = conn.Client.Receive(_tempRecvBuffer, SocketFlags.None);
                    _totalSize = size;
                    _recvPos = RecvPositions.AfterRecved;
                }
                catch {
                    break;
                }

                if (size <= 0)
                {
                    errCount++;
                    if (errCount > 3)
                    {
                        if (conn.Client == null || conn.Client.Blocking == false || conn.Client.Connected == false)
                        {
                            //루프에서 나간다.
                            break;
                        }
                        else
                        {
                            try
                            {
                                conn.Client.Close();
                             
                            }
                            catch { }
                            conn.Client = null;
                            break;
                        }
                    }
                }
                else
                {
                    errCount = 0;
                }
                TransferEventArgs args = new TransferEventArgs(_id, TransferEventArgs.TransferMode.Receive, size, size,"");
                if(E_OnCanReceive!=null) E_OnCanReceive(this, args);

                if (args.IsHandled == false && E_OnReceivedInQueue != null)
                {
                    Byte[] buff = new byte[size];
                    Buffer.BlockCopy(_tempRecvBuffer, 0, buff, 0, size);
                    _queue.enqueueFrom(buff);
                    _recvPos = RecvPositions.AfterQueueEntered;
                    args = new TransferEventArgs(_id, TransferEventArgs.TransferMode.Receive, size, _queue.Size, "");
                
                    if (E_OnReceivedInQueue != null) E_OnReceivedInQueue(this, args);
                }
                /*
                if (available > 0)
                {
                    //buff = new Byte[available];
                    
                }
                else
                {
                    //buff = new byte[_headerSize]; //available이 0일때는 다음에 도착할 header를 기다린다.
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
                */
            }

            FinishAClient();
            ConnectionEvent(ConnType.Disconnected);
            //close();
            //OnConnectionFailed("Connection closed..");
        }

        public int readFromNet(Byte[] buff, int offset, int size)
        {
            UdpClient conn = _clients[_threadId];
            return conn.Client.ReceiveFrom(buff, offset, size, SocketFlags.None, ref _remoteEP);
        }

        public int write(Byte[] buff, int offset, int size)
        {
            UdpClient conn = _clients[_threadId];
            return conn.Client.SendTo(buff, offset, size, SocketFlags.None, _remoteEP);
        }


        public int write(Byte[] buff, int size)
        {
            return write(buff, 0, size);
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
