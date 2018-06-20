using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using DataHandling;
using Timers = System.Timers;
using RtwEnums.Network;
namespace NetworkModules
{
    public class TcpClientBase:IClientBase,INetConnector
    {
        public enum SendType { Normal = 0, Multicast = 1, Broadcast = 2 };
        SendType _sendType = SendType.Normal;
        #region properties

        private TcpClient _client;
        //private StreamHandler _sh;
        private Thread _thread;
        protected int _id=-1;
        private Boolean _isEndConnection;
        private Boolean _runRecvThreadWhenConnected = false;
        public delegate void CallBackFunc();
        private CallBackFunc _callBackFunc;
        private CallBackFunc _connected;
        protected bool _isRecvEventEnabled = true;
        private Timer _readTimer;
        private Timer _writeTimer;
        private Timers.Timer _connectionTimer;
        protected BufferQueue _queue;
        private Boolean _isRecvQueueUsing = false;
        private FileStream _fileForSave = null;
        #endregion

        public TcpClientBase(int id=-1, SendType type=SendType.Normal){
            _id = id;
            _sendType = type;
            //_sh = null;
            _client = null;
            _isEndConnection = true;
            _callBackFunc = null;
            _connectionTimer = new System.Timers.Timer();
            _connectionTimer.Interval = 1000;
            _connectionTimer.Elapsed += new Timers.ElapsedEventHandler(_connectionTimer_Elapsed);
            
        }
        protected void setId(int id){ this._id = id;}

        public void useReceiveQueue(FileStream file=null)
        {
            _fileForSave = file;
            _queue = new BufferQueue();
            _isRecvQueueUsing = true;
        }

        public int Available{
            get{
                if (_queue != null) return _queue.Size;
                else
                {
                    if (_client == null) throw new Exception("client socket is not available for int Abailable: TcpClientBase::Available");
                    else return _client.Available + _totalSize;
                }
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
            try
            {
                Boolean isConn = (_client == null) ? false : _client.Client.Connected;
                if (isConn) return true;
                else
                {
                    return false;
                }
            }
            catch {
                return false;
            }
        }
        /*
        public StreamHandler getStreamHandler()
        {
            return _sh;
        }
        */

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
        String _serverIp = "";
        int _serverPort = 0;
        Thread _checkConnectionThread = null;
        public void setServer(string server, int port, Boolean isUsingReceiveQueue=false, FileStream saveFile=null, int readTimeout = 0, int writeTimeout = 0)
        {
            

            _isEndConnection = false;

            if (isUsingReceiveQueue) useReceiveQueue(saveFile); 

            _readTimeout = readTimeout;
            _writeTimeout = writeTimeout;
            _serverIp = server;
            _serverPort = port;
            
            
            StartConnection(2000,true);


//            setServer(server, port, null, readTimeout, writeTimeout);
        }

        public void setServerInfo(string server, int port, Boolean isUsingReceiveQueue = false, FileStream saveFile = null, int readTimeout = 0, int writeTimeout = 0)
        {


            _isEndConnection = false;

            if (isUsingReceiveQueue) useReceiveQueue(saveFile);

            _readTimeout = readTimeout;
            _writeTimeout = writeTimeout;
            _serverIp = server;
            _serverPort = port;
            _runRecvThreadWhenConnected = false;


            //            setServer(server, port, null, readTimeout, writeTimeout);
        }
        int _connectionTimeout = 1000;
        public void StartConnection(int connectionTimeoutWithMs=1000, bool runRecvThreadWhenConnected=false)
        {
            _connectionTimeout = connectionTimeoutWithMs;
            _runRecvThreadWhenConnected = runRecvThreadWhenConnected;

            ConnectionEvent(ConnType.Connecting);

            if (_client != null)
            {
                try
                {
                    _client.Close();
                    _client = null;
                }
                catch (Exception e)
                {
                    Console.WriteLine("이전에 만든 client가 끊어지지 않아서 끊는 도중에 에러가 발생했습니다." + e.Message);
                }
            }
            _connectionTimer.Interval = connectionTimeoutWithMs;
            _connectionTimer.Start();
            if (_checkConnectionThread == null)
            {
                
                _checkConnectionThread = new Thread(new ThreadStart(checkConnected));
                _checkConnectionThread.Start(); //connecting중이라는 이벤트를 생성하기 위하여 thread를 사용한다.
            }
            else
            {
                try
                {
                    _checkConnectionThread.Start(); //connecting중이라는 이벤트를 생성하기 위하여 thread를 사용한다.
                }
                catch (ThreadStateException e)
                {
                    _checkConnectionThread = new Thread(new ThreadStart(checkConnected));
                    _checkConnectionThread.Start(); //connecting중이라는 이벤트를 생성하기 위하여 thread를 사용한다.
                }
                catch (ThreadStartException e)
                {
                    _checkConnectionThread = new Thread(new ThreadStart(checkConnected));
                    _checkConnectionThread.Start(); //connecting중이라는 이벤트를 생성하기 위하여 thread를 사용한다.
                }
            }
        }

        void _connectionTimer_Elapsed(object sender, Timers.ElapsedEventArgs e)
        {
            //_client.Close();
            _connectionTimer.Stop();
        }

        
        protected virtual void ConnectionEvent(ConnType conn){}
        protected virtual void NetworkError(NetworkErrorEventArgs e) { }
        private void checkConnected()
        {
            _client = new TcpClient();
            try
            {
                _client.Connect(_serverIp, _serverPort);
            }
            catch { }

            try
            {

                if (_client == null || _client.Connected == false)
                {
                    ConnectionEvent(ConnType.Disconnected);
                    OnConnectionFailed();
                    NetworkError(new NetworkErrorEventArgs(_id, NetworkErrorEventArgs.NetErrorMsg.CONNECTION_FAIL, "Connection Failed: Timeout"));
                    Console.WriteLine("Failed Connecting To server.. TcpClientBase::connectToServer() - ");
                    try
                    {
                        _client.Close();

                    }
                    catch { }
                    _client = null;
                }
                else
                {
                    ConnectionEvent((ConnType.Connected));
                    if (_runRecvThreadWhenConnected) runRecvThread();
                }
            }
            catch(NullReferenceException) {
                ConnectionEvent(ConnType.Disconnected);
                _client = null;
                NetworkError(new NetworkErrorEventArgs(_id, NetworkErrorEventArgs.NetErrorMsg.CONNECTION_FAIL, "Connection Failed: Timeout"));
            }
            _checkConnectionThread = null;            
            //_checkConnectionThread.Abort();

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
                _isEndConnection = false;
                _thread = new Thread(new ThreadStart(this.recvLoop));
                _thread.Start();
            }
            else
            {
                _thread.Start();
            }
                
        }

        public void close()
        {
            if (_thread == null)
            {
                try
                {
                    if (_client != null && _client.Connected) _client.Close();
                }
                catch { }
                _client = null;
                ConnectionEvent(ConnType.Disconnected);
                return;
            }
            if (_thread!=null && _thread.ThreadState == ThreadState.Running)
            {
                _isEndConnection = true; //thread 종료조건 설정 후
                try
                {
                    if (_client.Connected) _client.Close(); //잡고 있을지 모르는 소켓을 닫아주고
                }
                catch { }
                _client = null;
                
                _thread.Join(100);//기다린다.
                if (_thread!=null && _thread.ThreadState == ThreadState.Running)//기다려도 끝나지 않는다면
                {
                    Thread tt = _thread;
                    _thread = null;
                    tt.Abort();//강제로 끝낸다.
                }
                _thread = null;
            }
        }

        protected virtual void OnReadFails(String errMsg="") { }
        
        public void readMore()
        {
            int unit = 0, size = 0;
            

            try
            {
                if (_client == null || _client.Client == null) return;
                else unit = _client.Client.Available;
            }
            catch
            {
                unit = 1;
            }
            if (unit <= 0) return;
            
            size = _client.Client.Receive(_tempBuff, _totalSize, unit, SocketFlags.None);
            _totalSize += size;

            if (size > 0 && _isRecvQueueUsing)
            {
                _queue.enqueueFrom(_tempBuff, 0, size);
            }


        }
        Byte[] _tempBuff = new Byte[8192];
        int _totalSize = 0;
        int loopZero = 3;
        private void recvLoop()
        {

            Console.WriteLine("server ready...");


            _isEndConnection = false;
            Console.WriteLine("accept client!");
            //_sh.setStream(_client.GetStream(),_client.Client);
            BeginAClient();

            int unit = 0;
            int blankCount = 200;
            while (_client != null && _client.Connected && !_isEndConnection)
            {
                funcRunningInServerLoopForClient();
                if (_callBackFunc != null) _callBackFunc();

                
                try
                {
                    if (_client == null || _client.Client == null) unit = 1;
                    else unit = _client.Client.Available;
                }
                catch
                {
                    unit = 1;
                }
                try
                {
                    int size = _client.Client.Receive(_tempBuff, _totalSize, unit, SocketFlags.None);
                    if (size == 0) loopZero--;
                    else loopZero = 3;
                    if (loopZero == 0 || size<0)
                    {
                        OnReadFails();
                        //OnConnectionFailed();
                        //FinishAClient();
                        break;
                    }
                    
                    if (size == 0) continue;
                    _totalSize += size;
                }
                catch
                {
                    if (_client == null || _client.Client == null || _client.Connected == false) blankCount = 0;
                }
                if ((unit != 0 && _totalSize <= 0) || blankCount == 0)
                {
                    //close();
                    OnReadFails();
                    
                    break;
                }
                if (_totalSize > 0)
                {
                    if (_isRecvQueueUsing)
                    {
                        _queue.enqueueFrom(_tempBuff, 0, _totalSize);
                        OnRead(_totalSize, _queue.Size);
                        _totalSize = 0;
                    }
                    else
                    {
                        OnRead(_totalSize, _client.Client.Available+_totalSize);
                    }
                    blankCount = 200;
                }
                else
                {
                    blankCount--;
                }
                
                
            }
            Console.WriteLine("wait other Client...");
            
            
            FinishAClient();
            ConnectionEvent(ConnType.Disconnected);
            //close();
            //OnConnectionFailed("Connection closed..");
        }
        public virtual void OnReadTimeout(){}
        public virtual void OnConnectionFailed(String errMsg="") { }
        Byte[] tempBuff = new Byte[8192];

        public void setReceiveTimeout(int timeout){ _readTimeout = timeout;}
        public void setSendTimeout(int timeout){_writeTimeout = timeout;}

        public void SuspendRecvEvent()
        {
            _isRecvEventEnabled = false;
        }
        public void ResumeRecvEvent()
        {
            _isRecvEventEnabled = true;
        }

        public int receiveData(Array buff, int offset=-1, int size=-1)
        {
            //이 메서드는 ReceiveQueue를 사용하지 않을 때 유효하다.
            //ReceiveQueue를 사용한다면, 이벤트 OnRead를 사용하여 데이터의 크기가 자신이 원하는 크기만큼 받아졌는지 판단 한 뒤
            //사용하여야 할 것이다.
            if (size < 0) size = _client.Available;

            if (_client != null)
            {
                int dataSize = 0;
                int totalSize = 0;
                try
                {
                    int count = 200;

                    if (_readTimeout > 0)
                    {
                        _readTimer = new System.Threading.Timer(onTick, TimeoutKind.Read, _readTimeout, _readTimeout);
                    }
                    
                    while (totalSize < size && count-- > 0)
                    {
                        dataSize = _client.Client.Receive(tempBuff, 0, size - totalSize, SocketFlags.None);

                        Buffer.BlockCopy(tempBuff, 0, buff, offset + totalSize, dataSize);

                        if (dataSize <= 0)
                        {
                            OnConnectionFailed();
                            //endThisClient();
                            return -1;
                        }
                        else
                        {
                            totalSize += dataSize;
                        }
                    }
                    if(_readTimer!=null) _readTimer.Dispose();
                    //Console.WriteLine("all data received:" + totalSize);
                }
                catch (SocketException )
                {

                    FinishAClient();
                    _client.Close();
                    OnReadFails();
                    return -1;
                }
                catch 
                {
                    throw;
                }
                if (totalSize != size) return -1;
                else return totalSize;
            }
            return -1;
        }

        public int read(Array buff, int size = -1)
        {
            return read(buff, 0, size);
        }
        public int read(Array buff, int offset, int size)
        {
            if (_isRecvQueueUsing)
            {
                int realSize = (_queue.Size > size) ? size : _queue.Size;
                _queue.dequeueTo(buff, offset, realSize);
                return realSize;
            }
            else
            {
                Buffer.BlockCopy(_tempBuff, 0, buff, offset, _totalSize);//먼저 이미 저장된 데이터를 가져온다.
                int rest = (_client.Client.Available>0)? receiveData(buff, offset+_totalSize, size) : 0;//다음에 남은 데이터를 저장한다.
                int all = rest + _totalSize;
                _totalSize = 0;
                return all;
            }
        }

        public int write(Byte[] buff, int size = -1)
        {
            return write(buff, 0, size);
        }
        public int write(Byte[] buff, int offset, int size)
        {
            if (size < 0) size = buff.Length;

            if (_client != null)
            {

                if (_writeTimeout > 0)
                {
                    _writeTimer = new Timer(onTick, TimeoutKind.Write, _writeTimeout, _writeTimeout);
                }

                int dataSize = _client.Client.Send(buff, offset, size, SocketFlags.None);
                if(_writeTimer!=null) _writeTimer.Dispose();
                return dataSize;
            }
            return -1;
        }
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
    }
}
