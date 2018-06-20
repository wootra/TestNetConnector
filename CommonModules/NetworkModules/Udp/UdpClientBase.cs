using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using DataHandling;

namespace NetworkModules
{
    public class UdpClientBase:IClientBase,INetConnector
    {
        #region properties
        //private StreamHandler _sh;
        private Thread _thread;
        protected bool _isRecvEventEnabled = true;
        private Boolean _isEndConnection;

        public delegate void CallBackFunc();
        private CallBackFunc _callBackFunc;


        protected BufferQueue _queue;
        private Boolean _isRecvQueueUsing = false;
        private FileStream _fileForSave = null;
        #endregion
        public enum SendType { Normal = 0, Multicast = 1, Broadcast = 2 };
        SendType _sendType = SendType.Normal;
        public UdpClientBase(SendType type=SendType.Normal)
        {
            _sendType = type;
            _isEndConnection = true;
            _callBackFunc = null;
            useReceiveQueue(null);
        }

        public void useReceiveQueue(FileStream file=null)
        {
            _fileForSave = file;
            _queue = new BufferQueue();
            _isRecvQueueUsing = true;
        }

        public int Available{
            get{
                if (_queue != null) return _queue.Size;
                else return _uSocket.Available;
            }
        }
        protected virtual void OnRead(int size, int totalSize) {  }
        
        public void stopUsingReceiveQueue()
        {
            _queue.Clear();
            _isRecvQueueUsing = false;
        }
        public Boolean IsReceiveQueueUsing() { return _isRecvQueueUsing; }



        public Boolean isEndConnection()
        {
            return _isEndConnection;
        }

        public bool isConnected()
        {
            return true;
            //if (_uSocket!=null && (_queue.Count > 0)) return true;
            //else return (_uSocket == null) ? false : _uSocket.Connected;
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

        public string getSourceIp()
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
        UdpClient _client;
        Socket _uSocket;
        String _serverIp;
        int _serverPort;
        public void setServer(string server, int port, Boolean isUsingReceiveQueue=true, FileStream saveFile=null, int readTimeout = 0, int writeTimeout = 0)
        {
            //_localEP = new IPEndPoint(IPAddress.Any, port); //IPAddress.Any 는 서버전용.
            _readTimeout = readTimeout;
            _writeTimeout = writeTimeout;
            _serverIp = server;
            _serverPort = port;

            _isEndConnection = false;
            _localEP = new IPEndPoint(NetFunctions.getMyIP4Address(), IPEndPoint.MinPort + 1); //this is blank IPEndPoint yet.
            //_localEP = new IPEndPoint(IPAddress.None, port); //this is blank IPEndPoint yet.

            if (server != null && server.Length > 7) _remoteEP = new IPEndPoint(NetFunctions.getIP4Address(_serverIp), _serverPort);
            else _remoteEP = new IPEndPoint(NetFunctions.getMyIP4Address(), _serverPort);
            if (_uSocket != null)
            {
                try
                {
                    _uSocket.Close();
                }
                catch { }
            }

            if (_sendType == SendType.Normal)
            {
                //_uSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                _client = new UdpClient(server, port);
                _client.Connect((IPEndPoint)_remoteEP);
                
                _uSocket = _client.Client;
                
                //_uSocket.Bind(_localEP);
            }
            else if (_sendType == SendType.Multicast)
            {
                _client = new UdpClient(server, port);
                _uSocket = _client.Client;
                _client.Connect((IPEndPoint)_remoteEP);
                _client.JoinMulticastGroup(NetFunctions.getIP4Address(server), NetFunctions.getMyIP4Address());
                
                //_uSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                //_uSocket.Bind(_localEP);
                //_uSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(IPAddress.Parse(server)));

            }
            else //broadcast
            {
                _uSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                _uSocket.Bind(_localEP);
                _uSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.Broadcast, true);
            }



            useReceiveQueue(saveFile); //udp는 무조건 queue를 쓴다.

            runRecvThread();
//            setServer(server, port, null, readTimeout, writeTimeout);
        }

        public void setServerInfo(string server, int port, Boolean isUsingReceiveQueue = true, FileStream saveFile = null, int readTimeout = 0, int writeTimeout = 0)
        {
            //_localEP = new IPEndPoint(IPAddress.Any, port); //IPAddress.Any 는 서버전용.
            _isEndConnection = false;
            _localEP = new IPEndPoint(NetFunctions.getMyIP4Address(), IPEndPoint.MinPort + 1); //this is blank IPEndPoint yet.
            _readTimeout = readTimeout;
            _writeTimeout = writeTimeout;
            _serverIp = server;
            _serverPort = port;
            //_localEP = new IPEndPoint(IPAddress.None, port); //this is blank IPEndPoint yet.

            if (server != null && server.Length > 7) _remoteEP = new IPEndPoint(NetFunctions.getIP4Address(_serverIp), _serverPort);
            else _remoteEP = new IPEndPoint(NetFunctions.getMyIP4Address(), _serverPort);
            if (_uSocket != null)
            {
                try
                {
                    _uSocket.Close();
                }
                catch { }
            }

           

            useReceiveQueue(saveFile); //udp는 무조건 queue를 쓴다.

            
            //            setServer(server, port, null, readTimeout, writeTimeout);
        }
        public void ConnectToServer(bool startRecvWhenConnected)
        {

            if((_thread!=null && _thread.ThreadState == ThreadState.Running) || this.isConnected()) close();

            if (_sendType == SendType.Normal)
            {
                //_uSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                _client = new UdpClient();//_serverIp, _serverPort);
                _client.Connect((IPEndPoint)_remoteEP);

                _uSocket = _client.Client;

                //_uSocket.Bind(_localEP);
            }
            else if (_sendType == SendType.Multicast)
            {
                _client = new UdpClient();//server, port);
                _client.Connect((IPEndPoint)_remoteEP);
                _uSocket = _client.Client;
                _client.JoinMulticastGroup(NetFunctions.getIP4Address(_serverIp), NetFunctions.getMyIP4Address());

                //_uSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                //_uSocket.Bind(_localEP);
                //_uSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(IPAddress.Parse(server)));

            }
            else //broadcast
            {
                _client = new UdpClient();
                _client.Connect((IPEndPoint)_remoteEP);
                _uSocket = _client.Client;// new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                _uSocket.Bind(_localEP);
                _uSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.Broadcast, true);
            }

            if (startRecvWhenConnected) runRecvThread();
        }

        enum TimeoutKind { Read = 0, Write };
        void onTick(object sender)
        {
            TimeoutKind k = (TimeoutKind)sender;
            switch (k)
            {
                case TimeoutKind.Read:
                    OnReadTimeout();
                    break;
                case TimeoutKind.Write:
                    OnWriteTimeout();
                    break;
            }
        }

        protected virtual void OnWriteTimeout() { }
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
                _thread = new Thread(new ThreadStart(this.serverLoop));
                _thread.Start();
            }
            else
            {
                _thread.Start();
            }
        }

        public void close()
        {
            _queue.Clear();
            if (_isEndConnection == true) return;
            endThisClient();
            if (_thread!=null && _thread.IsAlive)
            {
                _isEndConnection = true;
                _thread.Join(100);
                if (_thread != null && _thread.ThreadState == ThreadState.Running)
                {
                    Thread tt = _thread;
                    _thread = null;
                    FinishAClient();
                    tt.Abort();
                }
                try
                {
                    _client.Close();
                    
                    //_uSocket.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
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
        protected virtual void OnReadFails(String errMsg="") { }
        
        public void readMore()
        {
            int size = 0;


            try
            {
                size = _uSocket.ReceiveFrom(_tempBuff, ref _remoteEP);
                if (size > 0)
                {
                    _queue.enqueueFrom(_tempBuff, 0, size);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("socket exception" + e.ToString());
                return;

            }


        }
        Byte[] _tempBuff = new Byte[8192];
        private void serverLoop(){
            try
            {
                BeginAClient();

                while (!_isEndConnection)
                {
                    //_sh.setStream(_client.GetStream(),_client.Client);
                    
                    int blankCount = 200;

                    int size = _uSocket.ReceiveFrom(_tempBuff, ref _remoteEP);

                    funcRunningInServerLoopForClient();
                    if (_callBackFunc != null) _callBackFunc();


                    if (size > 0)
                    {
                        _queue.enqueueFrom(_tempBuff, 0, size);
                        OnRead(size, _queue.Size);
                        blankCount = 200;
                    }
                    else
                    {
                        blankCount--;
                    }



                }
                FinishAClient();
                Console.WriteLine("exit from while in Server...");
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
        public void SuspendRecvEvent()
        {
            _isRecvEventEnabled = false;
        }
        public void ResumeRecvEvent()
        {
            _isRecvEventEnabled = true;
        }

        public int read(Array buff, int size = -1)
        {
            return read(buff, 0, size);
        }
        public virtual void OnReadTimeout(){}
        public virtual void OnConnectionFailed(String errMsg="") { }
        Byte[] tempBuff = new Byte[8192];
        public int read(Array buff, int offset, int size)
        {
            int realSize = (_queue.Size > size) ? size : _queue.Size;
            _queue.dequeueTo(buff, offset, realSize);
            return realSize;
        }
        public void setReceiveTimeout(int timeout){ _readTimeout = timeout;}
        public void setSendTimeout(int timeout){_writeTimeout = timeout;}


        public int write(Byte[] buff, int size = -1)
        {
            return write(buff, 0, size);
        }
        public int write(Byte[] buff, int offset, int size)
        {
            if (size < 0) size = buff.Length;
            
            return _uSocket.SendTo(buff, offset, size, SocketFlags.None, _remoteEP);
        }
 
    }
}
