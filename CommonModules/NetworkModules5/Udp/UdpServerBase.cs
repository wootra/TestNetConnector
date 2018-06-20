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
using NetworkPacket;

namespace NetworkModules5
{
    public class UdpServerBase:INetConnector
    {
        public enum SendType { Normal = 0, Multicast = 1, Broadcast = 2 };
        public event ConnectionEventHandler E_Connection;
        public event NetworkErrorEventHandler E_NetError;
        public event TransferEventHandler E_OnCanReceive;
        public event TransferEventHandler E_OnReceivedInQueue;

        #region properties
        //private StreamHandler _sh;
        private Thread _thread;
        private Boolean _isEndConnection;
        private String _ip;
        public delegate void CallBackFunc();
        private CallBackFunc _callBackFunc;
        private InterfaceFunctions _functions;
        //private Byte[] _tempBuff = new Byte[10000000];
        private byte[] _tempRecvBuffer;

        protected BufferQueue _queue = new BufferQueue(100);
        SendType _sendType;
        int _id = -1;
        int _headerSize = 32;
        #endregion

        public UdpServerBase(int id, SendType type=SendType.Normal){
            _tempRecvBuffer = new byte[1024000];//1MByte
           
            _id = id;
            _sendType = type;
            _isEndConnection = true;
            _callBackFunc = null;
            _functions = new InterfaceFunctions(this, _queue);
            
        }
        public UdpServerBase(Byte[] tempRecvBuffer, SendType type = SendType.Normal, int id = -1)
            : base()
        {
            _tempRecvBuffer = tempRecvBuffer;
            _id = id;
            _sendType = type;
            _isEndConnection = true;
            _callBackFunc = null;
            _queue = new BufferQueue();
            _functions = new InterfaceFunctions(this, _queue);
        }

        public void setHeaderSize(int size)
        {
            _headerSize = size;
        }

        public InterfaceFunctions Interface { get { return _functions; } }

        protected virtual void OnRead(int size, int totalSize) {  }

        public void SetRemoteEp(String ip, int port=-1)
        {
            if (port == -1) port = _port;
            _remoteEP = new IPEndPoint(IPAddress.Parse(ip), port); //this is blank IPEndPoint yet.
        }

        public Boolean isEndConnection()
        {
            return _isEndConnection;
        }

        public bool isConnected()
        {
            return (_uSocket == null) ? false : true;// _uSocket.Connected;
        }
        /*
        public StreamHandler getStreamHandler()
        {
            return _sh;
        }
        */

        public Socket getClient()
        {
            return _uSocket;
        }

        public string getRemoteIp()
        {
            try
            {
                IPEndPoint ep = (IPEndPoint)_remoteEP;
                return ep.Address.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine("NetHandler::getClientIp() - " + e.Message);
                return "0.0.0.0";
            }
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
        Socket _uSocket;
        int _port;

        /// <summary>
        /// Port number..
        /// </summary>
        public int RemotePort
        {
            get { return _port; }
        }
        public void setServerInfo(string initIp, int port, int readTimeout = 0, int writeTimeout = 0)
        {

            _ip = initIp;
            _port = port;
            _isEndConnection = false;

            //if (server != null && server.Length > 7) _localEP = new IPEndPoint(NetFunctions.getIP4Address(server), port);
            //else _localEP = new IPEndPoint(NetFunctions.getMyIP4Address(), port);
            _readTimeout = readTimeout;
            _writeTimeout = writeTimeout;

            //            setServer(server, port, null, readTimeout, writeTimeout);
        }
        
        protected void ConnectionEvent(ConnType conn)
        {
            if (E_Connection != null) E_Connection(this, new ConnectionEventArgs(conn));
        }

        public void ReadyForClient()
        {
            _queue.Clear();
            if (_uSocket != null)
            {
                try
                {
                    _uSocket.Close();
                    _queue.Clear();
                    
                }
                catch { }
            }
            if (_thread!=null && _thread.ThreadState == ThreadState.Running)
            {
                Thread tt = _thread;
                _thread = null;
                tt.Abort();
            }

            _localEP = new IPEndPoint(IPAddress.Any, _port); //IPAddress.Any 는 서버전용.

            
            if (_sendType == SendType.Normal)
            {
                _uSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                try
                {
                    _uSocket.Bind(_localEP);
                }
                catch(Exception e)
                {
                    throw e;
                }

            }
            else if (_sendType == SendType.Multicast)
            {
                _uSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                try
                {
                    _uSocket.Bind(_localEP);
                }
                catch (Exception e)
                {
                    throw e;
                }
                _uSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(IPAddress.Parse(_ip)));
                _uSocket.SetSocketOption(SocketOptionLevel.Udp, SocketOptionName.MulticastTimeToLive, 1);
            }
            else //broadcast
            {
                _uSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                try
                {
                    _uSocket.Bind(_localEP);
                }
                catch (Exception e)
                {
                    throw e;
                }
                _uSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.Broadcast, true);
            }
            if(_readTimeout>0) _uSocket.ReceiveTimeout = _readTimeout;
            if(_writeTimeout>0) _uSocket.SendTimeout = _writeTimeout;

            _remoteEP = new IPEndPoint(IPAddress.None, _port); //this is blank IPEndPoint yet.
            
            runRecvThread();
        }
        public void ConnectToServer(string initIp, int port, int readTimeout = 0, int writeTimeout = 0)
        {
            ServerReady(initIp, port, readTimeout, writeTimeout);
        }

        /// <summary>
        /// setServerInfo 와 ReadyForClient를 차례로 수행한다.
        /// </summary>
        /// <param name="initIp"></param>
        /// <param name="port"></param>
        /// <param name="readTimeout"></param>
        /// <param name="writeTimeout"></param>
        public void ServerReady(string initIp, int port, int readTimeout = 0, int writeTimeout = 0)
        {
            setServerInfo(initIp, port, readTimeout, writeTimeout);

            ReadyForClient();
//            setServer(server, port, null, readTimeout, writeTimeout);
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


        /*
        public void setServer(string server, int port, Encoding encoding, int readTimeout = 0, int writeTimeout = 0)
        //if timeout is 0, networkStream will use default timeout.
        {
            if (_listener != null) return;

            IPEndPoint ipe = new IPEndPoint(NetFunctions.getMyIP4Address(), port);
            _listener = new TcpListener(ipe);
            _listener.Start();
            //if (encoding == null) _sh = new StreamHandler();
            //else _sh = new StreamHandler(encoding);
            //_sh.setStreamTimeout(readTimeout, writeTimeout);
        }
        */
        protected virtual void funcRunningInServerLoopForClient(){}

        protected virtual void BeginAClient(){}
        
        protected virtual void FinishAClient(){}
        
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
                _thread.Priority = ThreadPriority.AboveNormal;
                _thread.IsBackground = true;
                _thread.Start();
            }
            else
            {
                if (_thread.ThreadState != ThreadState.Running)
                {
                    try {
                        _thread.Abort(); 
                    }
                    catch { }
                    _thread = new Thread(new ThreadStart(this.recvLoop));
                    _thread.Start();
                }

            }
        }
        public void Close()
        {
            Disconnect();
        }

        public void Disconnect(Func<int> funcRunBeforeCloseSocket = null)
        {
            if (_isEndConnection == true) return;
            endThisClient();
            if (_thread!=null && _thread.IsAlive)
            {
                _isEndConnection = true;
                
                try
                {
                    if (_uSocket.Connected)
                    {
                        if(funcRunBeforeCloseSocket!=null) funcRunBeforeCloseSocket.Invoke();
                    }
                    _uSocket.ReceiveTimeout = 10;
                    _uSocket.SendTimeout = 10;
                    _uSocket.Blocking = false;
                    _uSocket.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                _thread.Join(100);//.Abort();
                if (_thread!=null && _thread.ThreadState == ThreadState.Running)
                {
                    
                    FinishAClient();
                    Thread tt = _thread;
                    _thread = null;
                    tt.Abort();
                }
                
                
               // _thread.Join();
            }
            _thread = null;
        }

        public int readMore()
        {
            int size=0;
            

            try
            {
                int available = _uSocket.Available;
                Byte[] buff;
                if (available > 0)
                {
                    buff = new Byte[available];
                    size = _uSocket.ReceiveFrom(buff, 0, available, SocketFlags.None, ref _remoteEP);
                }
                else
                {
                    buff = _tempRecvBuffer; //available이 0일때는 다음에 도착할 header를 기다린다.
                    _uSocket.Blocking = true;
                    size = _uSocket.ReceiveFrom(buff, ref _remoteEP);
                }
                if (size > 0)
                {
                    if (size == buff.Length) _queue.Enqueue(buff);
                    else _queue.enqueueFrom(buff, 0, size);
                }
                return size;
            }
            catch (Exception e)
            {
                Console.WriteLine("socket exception" + e.ToString());
                return -1;
                
            }

            
        }
        public int Available
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

        public void endThisClient()
        {
            //if(_sh!=null) _sh.closeStream();
            _isEndConnection = true;
           // _isRecvQueueUsing = false;

        }
        protected virtual void OnReadFails() { }
        enum RecvPositions { AfterRecved, AfterQueueEntered };
        RecvPositions _recvPos = RecvPositions.AfterRecved;

        int _totalSize = 0;
       // int loopZero = 3;
        private void recvLoop()
        {
            
            Console.WriteLine("server ready...");


            _isEndConnection = false;
            Console.WriteLine("accept client!");
            //_sh.setStream(_client.GetStream(),_client.Client);
            BeginAClient();
            int errCount = 0;
            while (!_isEndConnection)
            {
                funcRunningInServerLoopForClient();
                if (_callBackFunc != null) _callBackFunc();

                
                int size = 0;
                try
                {
                    size = _uSocket.ReceiveFrom(_tempRecvBuffer, ref _remoteEP);// 0, available, SocketFlags.None); ;
                    _totalSize = size;
                }
                catch
                {
                    break;
                }
                _recvPos = RecvPositions.AfterRecved;
                if (size <= 0)
                {
                    errCount++;
                    if (errCount > 1)
                    {
                        if (_uSocket == null || _uSocket.Blocking == false || _uSocket.Connected == false)
                        {
                            //루프에서 나간다.
                            break;
                        }
                        else
                        {
                            try
                            {
                                _uSocket.Close();
                            }
                            catch { }
                            _uSocket = null;
                            break;

                        }
                    }
                }
                else
                {
                    errCount = 0;
                }
                TransferEventArgs args = new TransferEventArgs(_id, _tempRecvBuffer, TransferEventArgs.TransferMode.Receive, size, _totalSize, "");
                
                if (E_OnCanReceive != null) E_OnCanReceive(this, args);

                if (args.IsHandled == false && E_OnReceivedInQueue != null)
                {
                    Byte[] buff = new byte[size];
                    Buffer.BlockCopy(_tempRecvBuffer, 0, buff, 0, size);
                    _queue.enqueueFrom(buff);
                    _recvPos = RecvPositions.AfterQueueEntered;

                    args = new TransferEventArgs(_id, _tempRecvBuffer, TransferEventArgs.TransferMode.Receive, size, _queue.Size, "");

                    if (E_OnReceivedInQueue != null) E_OnReceivedInQueue(this, args);

                    
                }
                /*
                if (_callBackFunc != null) _callBackFunc();

                int available = _uSocket.Available;
                Byte[] buff;
                int size;
                if (available > 0)
                {
                    buff = new Byte[available];
                    size = _uSocket.ReceiveFrom(buff, 0, available, SocketFlags.None, ref _remoteEP);
                }
                else
                {
                    buff = _tempBuff; //available이 0일때는 다음에 도착할 header를 기다린다.
                    _uSocket.Blocking = true;
                    size = _uSocket.ReceiveFrom(buff, ref _remoteEP);
                    totalCount++;
                }
                if (size > 0)
                {
                    if (size == buff.Length) _queue.Enqueue(buff);
                    else _queue.enqueueFrom(buff, 0, size);
                    totalSize += (ulong)size;
                    if (_queue.Size != (int)totalSize)
                    {
                        Console.Write("total size is not match to queueSize");
                    }
                    if (E_OnReceived != null) E_OnReceived(this, new TransferEventArgs(_id, TransferEventArgs.TransferMode.Receive, size, _queue.Size));

                    loopZero = 3;
                    totalDataCount++;
                    

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

        public void Connect(bool runReceiveThreadWhenConnected)
        {
            ReadyForClient();
        }

        public int readFromNet(byte[] buff, int offset, int size)
        {
            return _uSocket.ReceiveFrom(buff, offset, size, SocketFlags.None, ref _remoteEP);
        }

        public int write(byte[] buff, int offset, int size)
        {
            return _uSocket.SendTo(buff, offset, size, SocketFlags.None, _remoteEP);
        }


        public int write(Byte[] buff, int size)
        {
            return write(buff, 0, size);
        }
    }

    public class AsyncResult : IAsyncResult
    {
        public object AsyncState
        {
            get { return null; }
        }

        public WaitHandle AsyncWaitHandle
        {
            get { return null; }
        }

        public bool CompletedSynchronously
        {
            get { return false; }
        }

        public bool IsCompleted
        {
            get { return true; }
        }
    }
}
