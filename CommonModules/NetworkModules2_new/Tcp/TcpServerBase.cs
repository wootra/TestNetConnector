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
    public class TcpServerBase:INetConnector, IDisposable
    {
        public event ConnectionEventHandler E_Connection;
        public event NetworkErrorEventHandler E_NetError;
        public event TransferEventHandler E_OnReceived;

        #region properties
        public enum SendType { Normal = 0, Multicast = 1, Broadcast = 2 };
        private TcpListener _listener;
        private TcpClient _client;
        //private StreamHandler _sh;
        private Thread _thread;
        private Boolean _isEndServer;
        private Boolean _isEndConnection;
        private InterfaceFunctions _functions;

        public delegate void CallBackFunc();
        private CallBackFunc _callBackFunc;

        private Timer _readTimer;
        private Timer _writeTimer;

        protected BufferQueue _queue;
        SendType _sendType;
        int _id = -1;
        #endregion

        public TcpServerBase(int id, SendType type = SendType.Normal)
        {
            _id = id;
            _listener = null;
            _sendType = type;
            //_sh = null;
            _client = null;
            _isEndServer = true;
            _isEndConnection = true;
            _callBackFunc = null;
            _queue = new BufferQueue();
            _functions = new InterfaceFunctions(this, _queue);
        }

        public InterfaceFunctions Interface { get { return _functions; } }

        public int Available{
            get{
                return (_client!=null)? _client.Available : 0;
            }
        }

        public int QueueAvailable
        {
            get
            {
                return _queue.Size;
            }
        }

        public bool isConnected()
        {
            return (_client == null || _client.Client == null) ? false : _client.Connected;
        }
        
        public TcpClient getClient()
        {
            return _client;
        }

        protected void ConnectionEvent(ConnType conn)
        {
            if (E_Connection != null) E_Connection(this, new ConnectionEventArgs(conn));
        }

        public string getDestIp()
        {
            try
            {
                IPEndPoint ep = (IPEndPoint)(getClient().Client.RemoteEndPoint);
                //String ip = _client.Client.AddressFamily.ToString();
                
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
        public void ConnectToServer(string server, int port,int readTimeout = 0, int writeTimeout = 0)
        {
            ServerReady(server, port, readTimeout, writeTimeout);
        }
        public void ServerReady(string server, int port, int readTimeout = 0, int writeTimeout = 0)
        {
            setServerInfo(server, port, readTimeout, writeTimeout);
            ReadyForClient();

//            setServer(server, port, null, readTimeout, writeTimeout);
        }

        /// <summary>
        /// 서버의 정보를 적는다.
        /// 바로 받는 루틴을 시작하지는 않는다.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="port"></param>
        /// <param name="isUsingReceiveQueue"></param>
        /// <param name="saveFile"></param>
        /// <param name="readTimeout"></param>
        /// <param name="writeTimeout"></param>
        public void setServerInfo(string server, int port, int readTimeout = 0, int writeTimeout = 0)
        {

            if (_listener != null) return;

            _isEndServer = false;

            IPEndPoint ipe;
            if (server != null && server.Length > 7) ipe = new IPEndPoint(NetFunctions.getIP4Address(server), port);
            else ipe = new IPEndPoint(NetFunctions.getMyIP4Address(), port);
            _listener = new TcpListener(ipe);
            _listener.Start();

            _readTimeout = readTimeout;
            _writeTimeout = writeTimeout;
            

            //            setServer(server, port, null, readTimeout, writeTimeout);
        }

        public void startReceiving()
        {
            ReadyForClient();
        }

        enum TimeoutKind { Read = 0, Write };
        void onTimeoutTick(object sender)
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
                
                if (_thread.ThreadState == ThreadState.Aborted || _thread.ThreadState == ThreadState.AbortRequested)
                {
                    _thread = new Thread(new ThreadStart(this.serverLoop));
                }
                if(_thread.ThreadState!= ThreadState.Running) _thread.Start();
            }
        }

        public void Disconnect(Func<int> runBeforeCloseSocket=null)
        {
            if (_isEndServer == true) return;
            endThisClient();
            _isEndServer = true;

            if (_client.Connected)
            {
                if (runBeforeCloseSocket != null) runBeforeCloseSocket.Invoke();
                _client.Client.Blocking = false;
                _client.Client.ReceiveTimeout = 10;
                _client.Client.SendTimeout = 10;
            }
            try
            {
                if (_listener != null)
                {
                    _listener.Stop();
                    _listener = null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            if (_thread != null && _thread.IsAlive)
            {
                
                
                FinishAClient();
                Thread tt = _thread;
                _thread = null;
                tt.Abort();
            }
            _thread = null;
        }

        public void endThisClient()
        {
            //if(_sh!=null) _sh.closeStream();
            _isEndConnection = true;
            //_isRecvQueueUsing = false;
            if (_client != null && _client.Client!=null)
            {
                try
                {
                    _client.Client.Close();
                }
                catch { }
                //FinishAClient();
                
            }
            _client = null;

        }


        private int _headerSize = 32; //headerSize는 데이터가 도착한 이후 available이 0일 때 기본으로 받을크기를 정하는데 사용된다.
        public void setHeaderSize(int byteSize)
        {
            _headerSize = byteSize;
        }

        Byte[] _tempBuff = new Byte[8192];
        int _totalSize = 0;
        Socket _runningSocket = null;
        private void serverLoop(){
 
                while (!_isEndServer)
                {
                    Console.WriteLine("server ready...");
                    try
                    {
                        _client = _listener.AcceptTcpClient();
                    }
                    catch
                    {
                        this.Disconnect();
                        OnConnectionFailed();
                        break;
                    }
                    recvLoop();
                }
                Console.WriteLine("exit from while in Server...");
                _listener = null;
           

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

                int available = _client.Client.Available;
                Byte[] buff;
                int size;
                if (available > 0)
                {
                    buff = new Byte[available];
                    size = _client.Client.Receive(buff, 0, available, SocketFlags.None);
                }
                else
                {
                    buff = new byte[_headerSize]; //available이 0일때는 다음에 도착할 header를 기다린다.
                    size = _client.Client.Receive(buff, 0, _headerSize, SocketFlags.None);
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

        public virtual void OnReadTimeout(){}
        public virtual void OnConnectionFailed() { }
        Byte[] tempBuff = new Byte[8192];

        public void setReceiveTimeout(int timeout){ _readTimeout = timeout;}
        public void setSendTimeout(int timeout){_writeTimeout = timeout;}


        /*
        public void Echo() //for test
        {
            string msg = string.Empty;
            try
            {

                
                if (String.IsNullOrEmpty(msg = _sh.getReader().ReadLine()) == false)
                {
                    Console.WriteLine("server::" + msg);
                    _sh.getWriter().WriteLine("server Sended:" + msg);
                    _sh.getWriter().Flush();
                }
                else
                {
                    endServerFromLoop();
                    Console.WriteLine("server end!");
                }
          
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
         */

        public void Dispose()
        {
            _isEndServer = true;
            _isEndConnection = true;
            _client.Close();
            
            _listener = null;
            _client = null;
        }


        public void ReadyForClient()
        {
            _queue.Clear();
            runRecvThread();
        }

        public void Connect(bool runReceiveThreadWhenConnected)
        {
            ReadyForClient();
        }

        public int readFromNet(byte[] buff, int offset, int size)
        {
            return _client.Client.Receive(buff, offset, size, SocketFlags.None);
        }

        public int write(byte[] buff, int offset, int size)
        {
            return _client.Client.Send(buff, offset, size, SocketFlags.None);
        }
    }
}
