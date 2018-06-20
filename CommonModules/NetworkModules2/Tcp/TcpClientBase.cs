using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using DataHandling;
using System.IO;
using IOHandling;
using Timers = System.Timers;
using RtwEnums.Network;

namespace NetworkModules
{
    public class TcpClientBase:INetConnector
    {
        #region properties
        public event ConnectionEventHandler E_Connection;
        public event NetworkErrorEventHandler E_NetError;
        public event TransferEventHandler E_OnReceived;

        //private StreamHandler _sh;
        private Thread _recvThread;
        protected int _id=-1;
        TcpClient _server;

        private InterfaceFunctions _functions;
        private Boolean _isEndConnection;
        private Boolean _runRecvThreadWhenConnected = false;

        int _connectionTimeout = 2000;
        private BufferQueue _queue = new BufferQueue();

        private Timer _readTimer;
        private Timer _writeTimer;
        private Timers.Timer _connectionTimer;
        bool _isDisposing = false;
        StreamWriter _errorLogWriter = null;
        bool _isConnecting = false;
        //private Boolean _isRecvQueueUsing = false;
        
        #endregion

        public TcpClientBase(int id, StreamWriter errorLog=null):base(){

            init(id);
            if (errorLog != null) _errorLogWriter = errorLog;
        }

        public TcpClientBase(StreamWriter errorLog=null)
        {
            init(-1);
            if (errorLog != null) _errorLogWriter = errorLog;
        }

        public int Available
        {
            get
            {
                if (_recvThread != null && _recvThread.ThreadState == ThreadState.Running)
                {
                    return _queue.Size;
                }
                else
                {
                    return _server.Available;
                }
                
            }
        }

        public Socket Client { 
            get {
                if (_server != null) return _server.Client;
                else return null;
            } 
        }

        public InterfaceFunctions Interface { get { return _functions; } }

        void init(int id)
        {
            _server = new TcpClient();
            _id = id;
            _functions = new InterfaceFunctions(this, _queue);
            //_sh = null;
            _isEndConnection = true;

            _connectionTimer = new System.Timers.Timer();
            _connectionTimer.Interval = 1000;
            _connectionTimer.Elapsed += new Timers.ElapsedEventHandler(_connectionTimer_Elapsed);
            _connectionTimer.Start();
        }
        protected void Dispose(bool disposing)
        {
            Disconnect();
        }
        protected void setId(int id){ this._id = id;}

        #region Virtual functions
        protected virtual void funcRunningInServerLoopForClient() { }

        protected virtual void BeginAClient() { }

        protected virtual void FinishAClient() { }

        #endregion

        //public Boolean IsReceiveQueueUsing() { return _isRecvQueueUsing; }

        private int _headerSize = 32; //headerSize는 데이터가 도착한 이후 available이 0일 때 기본으로 받을크기를 정하는데 사용된다.
        public void setHeaderSize(int byteSize)
        {
            _headerSize = byteSize;
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
        String _serverIp = "";
        int _serverPort = 0;
        Thread _checkConnectionThread = null;
        bool _isPermarnentConnecting = false;
        /// <summary>
        /// 서버에 바로 연결을 시도한다. timeout시간은 200이다.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="port"></param>
        /// <param name="readTimeout"></param>
        /// <param name="writeTimeout"></param>
        public void ConnectToServer(string server, int port, int readTimeout = 0, int writeTimeout = 0)
        {
            setServerInfo(server, port, readTimeout, writeTimeout);
            StartConnection(0, true);
        }

        public void ConnectToServer(string server, int port, int connectionTimeout=2000, int readTimeout = 0, int writeTimeout = 0)
        {
            
            setServerInfo(server, port, readTimeout, writeTimeout);
            if (_connectionState== ConnType.Disconnected)
            {
                StartConnection(connectionTimeout, true);
            }
        }
        public void Connect(bool runReceiveThreadWhenConnected, int connectionTimeout=1000)
        {
            if (_connectionState == ConnType.Disconnected)
            {
                StartConnection(connectionTimeout, runReceiveThreadWhenConnected);
            }
        }
        public void ConnectForever(bool runReceiveThreadWhenConnected)
        {
            if (_connectionState == ConnType.Disconnected)
            {
                StartConnection(0, runReceiveThreadWhenConnected);
            }
        }
        
        /// <summary>
        /// 연결을 Close할 때까지 계속 연결을 시도한다.
        /// </summary>
        /// <param name="runReceiveThreadWhenConnected"></param>
        public void Connect(bool runReceiveThreadWhenConnected)
        {
            if (_connectionState == ConnType.Disconnected)
            {
                StartConnection(0, runReceiveThreadWhenConnected);
            }
        }

        public void setServerInfo(string server, int port, int readTimeout = 0, int writeTimeout = 0)
        {
            _isEndConnection = false;
            _readTimeout = readTimeout;
            _writeTimeout = writeTimeout;
            _serverIp = server;
            _serverPort = port;
        }


        public void releaseResources(){

            _queue.Clear();
            
        }
        public void releaseThreads()
        {
            
            
            releaseRecvThread();
        }

        public void writeLog(String log)
        {
            if (_errorLogWriter != null) _errorLogWriter.WriteLine(log);
        }

        public void releaseRecvThread()
        {
            if (_recvThread == null)
            {
                return;
            }

            if (_recvThread != null)
            {
                _isEndConnection = true; //thread 종료조건 설정 후
                while (_isRecvThreadRunning)
                {
                    writeLog("waiting recv thread stopping...");
                    if(_server!=null && _server.Client!=null){
                        if(_server.Client.Blocking) _server.Client.Blocking = false;
                        try{
                            _server.Close();
                        }catch{
                            Win32APIs.SendMsgData("TcpClientBase:Thread해제시 소켓클로즈 안됨.", "LogWindow");
            
                        }
                        _server.Client = null;
                        _recvThread.Abort();
                        _recvThread = null; 
                    }
                    Thread.Sleep(100);
                }
            }
        }

        /// <summary>
        /// Thread를 끝내지 않고 connection만 끝낸다.
        /// 이 명령은 ConnectionEvent를 발생시키지 않지만 PermanantConnection모드에서는
        /// 다시 접속을 시도한다.
        /// </summary>
        public void Disconnect(Func<int> funcRunningBeforeCloseSocket=null)
        {
            releaseResources();
            
            if (_server != null && _server.Client != null)
            {
                if (_server.Client.Blocking) _server.Client.Blocking = false;
                Win32APIs.SendMsgData("Blocking 해제...", "LogWindow");
            }

            
            if (_server!=null && _server.Client != null)
            {
                Byte[] buff = new Byte[1];
                SocketError error = SocketError.NoData;
                _server.SendTimeout = 1000;
                if (funcRunningBeforeCloseSocket != null) funcRunningBeforeCloseSocket.Invoke();
                    
                    
                //int recv = _server.Client.Send(buff, 0, 1, SocketFlags.None, out error);
                //Win32APIs.SendMsgData("1byte 보낸 것 성공?"+recv+"\r\n", "LogWindow");
                //if (error != SocketError.Success)
                //{
                    //   Win32APIs.SendMsgData("소켓손상...", "LogWindow");
                    //이미 소켓이 손상된 경우..
                    // _server = null;
                //}
                //else {//끝내기 전에 사용자가 원하는 작업 한 가지를 하고 끝낼 수 있다.
                    
                    try
                    {
                        Win32APIs.SendMsgData("disconnect시도", "LogWindow");
                        try
                        {
                            _server.Client.Disconnect(false);
                            Win32APIs.SendMsgData("disconnect완료", "LogWindow");
                        }
                        catch(Exception ex)
                        {
                            Win32APIs.SendMsgData("disconnect시 에러:"+ex.Message, "LogWindow");
                        }
                        Win32APIs.SendMsgData("close시도", "LogWindow");
                        _server.Client.Close(1000);
                        Win32APIs.SendMsgData("close완료", "LogWindow");
                    }
                    catch(Exception e) {
                        Win32APIs.SendMsgData("close시 에러..\r\n"+e.Message, "LogWindow");
                    }
                        
                //}
            }
             //미리 삭제된 개체였을 가능성..
            _server = null;
            _connectionState = ConnType.Disconnected;
            //ConnectionEvent(ConnType.Disconnected);
        }

        
        /// <summary>
        /// Thread를 끝내지 않고 접속만 다시 닫았다 연다.
        /// </summary>
        public void reconnect()
        {
            if (_connectionState != ConnType.Disconnected) //접속상태거나 접속중이라면
            {
                Disconnect(); //접속을 끊고..
                
                if(_isPermarnentConnecting==false) StartConnection(_connectionTimeout, _runRecvThreadWhenConnected);
            }
            else
            {
                StartConnection(_connectionTimeout, _runRecvThreadWhenConnected);
            }
        }

        /// <summary>
        /// 모든 실행중인 thread와 연결시도를 끝낸다.
        /// </summary>
        public void Close()
        {
            _isDisposing = true;
            _isPermarnentConnecting = false;
            _connectionTimer.Stop();
            _isDisposing = true;
            _isPermarnentConnecting = false;

            try
            {
                _checkConnectionThread.Abort();
                _checkConnectionThread = null;
            }
            catch { }

            Disconnect();
            releaseThreads();
        }

        public void setReceiveTimeout(int timeout) { _readTimeout = timeout; }
        public void setSendTimeout(int timeout) { _writeTimeout = timeout; }

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

        public bool isConnected()
        {
            try
            {
                //int time = 3;
                
                //if(time==0 && _server.Connected==false) 
                  //  _server = new TcpClient(_serverIp, _serverPort);
                /*
                try
                {
                    if (_server.Connected) ConnectionEvent(ConnType.Connected);
                }
                catch { }
                 */
                if (_server != null && _server.Client != null)
                {
                    bool isconnected = _server.Client.Connected;
                    if (isconnected) ConnectionEvent(ConnType.Connected); //만일의 경우 갱신이 늦게 되었을 때를 대비하여
                    
                    return _server.Client.Connected;
                }
                else return false;
            }
            catch {
                //ConnectionEvent(ConnType.Disconnected);
                return false;
                //throw;
            }
        }

        public ConnType ConnectionState{
            get
            { 
                return _connectionState;
            }
        }

        public int readFromNet(Byte[] buff, int offset, int size)
        {
            SocketError error = new SocketError();
            //if (sockError != null) sockError = error;
            return _server.Client.Receive(buff, offset, size, SocketFlags.None, out error);
        }
        
        /// <summary>
        /// 소켓에서 값을 읽어온다. 만일 못읽어들어올 경우, 100번을 시도하고 -1을 리턴한다.
        /// 소켓이 삭제되었으면 -1을 리턴한다.
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="offset"></param>
        /// <param name="byteSize"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public int readFromNet(Byte[] buff, int offset, int byteSize, out SocketError error)
        {
            int total = 0;
            int size = 0;
            error = SocketError.NoData;
            int fail = 0;
            while (total < byteSize)
            {
                size = _server.Client.Receive(buff, offset, byteSize, SocketFlags.None, out error);
                if (size > 0) total += size;
                else if (size < 0){
                    return -1;
                }
                else
                {
                    fail++;
                    if (fail > 100) return -1;
                }
            }
            
            return total;
        }

        public int write(Byte[] buff, int offset, int size)
        {
            return _server.Client.Send(buff, offset, size, SocketFlags.None);
        }

        
        /// <summary>
        /// 연결시도를 시작한다. 무한대기를 위해서는 connectionTimeout을 0으로 하면 된다.
        /// </summary>
        /// <param name="connectionTimeoutWithMs">연결시도 timeout 시간. ms단위. 0으로 하면 무한히 연결시도를한다.</param>
        /// <param name="runRecvThreadWhenConnected">true일 경우 receive thread를 연결되자마자 자동으로 시작함.</param>
        void StartConnection(int connectionTimeoutWithMs=2000, bool runRecvThreadWhenConnected=false)
        {
            if (_isConnecting) return;
            _isConnecting = true;
            
            if (connectionTimeoutWithMs <= 0)
            {
                _isPermarnentConnecting = true;
            }
            else
            {
                _isPermarnentConnecting = false;
            }

            if (_connectionState != ConnType.Disconnected) //현재 연결진행중이면 그냥 나간다.
            {
                Win32APIs.SendMsgData("StartConnection: 이미 연결되었거나 연결중 - 무시", "LogWindow");
                return;
            }
            else
            {
                if (_connectionTimer.Enabled == true)
                {
                    _connectionTimer.Stop(); //timeout을 위하여 
                }
                
                _connectionTimer.Interval = (_isPermarnentConnecting) ? 1000 : connectionTimeoutWithMs; //무한대기는 1000ms마다 체크한다.
                
                

                ConnectionEvent(ConnType.Connecting);
                
                _isDisposing = false;
                _queue.Clear();
                
                _runRecvThreadWhenConnected = runRecvThreadWhenConnected;

                if (_server != null && _server.Client != null)
                {
                    try
                    {
                        Win32APIs.SendMsgData("StartConnection: 기존 연결 Disconnect", "LogWindow");
                        _server.Client.Disconnect(false);
                        Win32APIs.SendMsgData("StartConnection: 이전 연결 Disconnect성공:", "LogWindow");
                    }
                    catch (Exception e)
                    {
                        Win32APIs.SendMsgData("StartConnection: 이전 소켓 disconnect시 실패:" + e.Message, "LogWindow");
                    }
                    try
                    {
                        Win32APIs.SendMsgData("StartConnection: 기존 소켓 Close", "LogWindow");
                        _server.Client.Close(1000);
                        Win32APIs.SendMsgData("StartConnection: 이전 소켓 Close성공:", "LogWindow");
                    }
                    catch (Exception e)
                    {
                        Win32APIs.SendMsgData("StartConnection: 이전 소켓 Close시 실패:" + e.Message, "LogWindow");
                    }

                }
                _server = null;
                if (_checkConnectionThread != null) //기존에 연결시도중인 연결이 있다면 삭제하는 루틴..
                {
                    Win32APIs.SendMsgData("StartConnection: 기존 Thread 삭제 시도", "LogWindow");

                    if (_checkConnectionThread.ThreadState != ThreadState.Aborted && _checkConnectionThread.ThreadState != ThreadState.AbortRequested && _checkConnectionThread.ThreadState != ThreadState.Stopped)
                    {
                        Win32APIs.SendMsgData("StartConnection: 기존 Thread 삭제 중- 현재상태:"+_checkConnectionThread.ThreadState.ToString(), "LogWindow");

                        _checkConnectionThread.Abort();
                        _checkConnectionThread = null;
                        Win32APIs.SendMsgData("StartConnection: 기존 Thread 삭제 완료", "LogWindow");

                    }
                    else
                    {
                        Win32APIs.SendMsgData("StartConnection: 기존 Thread 삭제 필요없음.", "LogWindow");
                    }
                }
                Win32APIs.SendMsgData("StartConnection: 소켓 새로 생성", "LogWindow");
                _server = new TcpClient();
                Win32APIs.SendMsgData("StartConnection: 소켓 새로 생성 완료", "LogWindow");

                _checkConnectionThread = new Thread(new ThreadStart(tryConnection));

                _checkConnectionThread.Start();
                _connectionTimer.Start();
            }
        }

        
        /// <summary>
        /// 타이머가 계속 돌면서 연결중인지 아닌지를 검사함..
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _connectionTimer_Elapsed(object sender, Timers.ElapsedEventArgs e)
        {
            if (_serverIp.Length == 0) return; //초기상태
            try
            {
                if (_server.Client.Connected) //연결되었으면
                {
                    ConnectionEvent(ConnType.Connected); //연결이 되어있는 상태라면 이 명령은 효과없음..
                    
                }
                else //연결이 아직 안되었는데,,
                {
                    Win32APIs.SendMsgData("접속 타이머: 연결 아직 안됨.", "LogWindow");
                    
                        Win32APIs.SendMsgData("접속 타이머: 접속 시도중.", "LogWindow");
                        if (_isPermarnentConnecting) //계속 체크중인 것
                        {
                            ConnectionEvent(ConnType.Connecting);
                            
                        }
                        else //timeout에 걸린 것
                        {
                            _server.Client.Close(1000);
                            ///_server.Close(); //연결 중이라고 해도 끊음.. 
                            ConnectionEvent(ConnType.Disconnected);
                        }
                    
                }
            }
            catch(Exception ex) { //소켓이 손상되었을 때
                _connectionTimer.Stop();
                Win32APIs.SendMsgData("접속 타이머: 소켓 손상의심: "+ex.Message, "LogWindow");
                    if (_isPermarnentConnecting) //계속 체크중인 것
                    {
                        ConnectionEvent(ConnType.Disconnected);
                        Win32APIs.SendMsgData("접속 타이머: 계속 접속모드 - 재접속", "LogWindow");
                        if(_isDisposing==false) StartConnection(0, _runRecvThreadWhenConnected); //소켓을 다시 만들어야한다.
                    }
                    else //다시 만들 필요가 없다. 기다리지 않는 모드이므로..
                    {
                        Win32APIs.SendMsgData("접속 타이머: 단일 접속모드 - 접속끊음.", "LogWindow");
                        ConnectionEvent(ConnType.Disconnected);
                    }
                
            }

            
        }

        ConnType _connectionState = ConnType.Disconnected;
        /// <summary>
        /// 상태가 변할 경우 이벤트를 내려줌..같은 상태를 호출할 경우 아무 일도 하지 않음..
        /// </summary>
        /// <param name="conn"></param>
        public void ConnectionEvent(ConnType conn){
            if (_connectionState != conn) //상태가 달라질때만 호출..
            {
                _connectionState = conn;
                if (E_Connection != null) E_Connection(this, new ConnectionEventArgs(conn));
            }
        }

        protected virtual void NetworkError(NetworkErrorEventArgs e) {
            if (E_NetError != null) E_NetError(this, e);
        }

        
        private void tryConnection()
        {
            if(_server!=null && _server.Client.Connected){
                //_checkConnectionThread.Abort();
                return;
            }else{
                if (_isDisposing == false)
                {
                    try
                    {
                        Win32APIs.SendMsgData("tryConnection: 접속 시도", "LogWindow");
                        _server.Connect(_serverIp, _serverPort);
                        Win32APIs.SendMsgData("tryConnection: 접속 시도 끝", "LogWindow");

                        if (_server != null && _server.Client != null && _server.Connected == true)
                        {
                            Win32APIs.SendMsgData("tryConnection: 연결됨1", "LogWindow");
                            ConnectionEvent(ConnType.Connected);
                            Win32APIs.SendMsgData("tryConnection: 연결됨2", "LogWindow");
                            _isConnecting = false;
                            //_checkConnectionThread.Abort();
                        }
                        else
                        {
                            Win32APIs.SendMsgData("tryConnection:연결된 후 뭔가 잘못되었다.", "LogWindow");
                            if (_isPermarnentConnecting)
                            {
                                Win32APIs.SendMsgData("tryConnection:계속 접속모드: 재접속 호출", "LogWindow");
                                _connectionState = ConnType.Disconnected;
                                _isConnecting = false;
                                _connectionTimer.Start();
                                //if (_isDisposing == false) StartConnection(_connectionTimeout, _runRecvThreadWhenConnected);

                                return;
                            }
                            else
                            {
                                Win32APIs.SendMsgData("tryConnection:연결시도 마침.", "LogWindow");
                                _isConnecting = false;
                                ConnectionEvent(ConnType.Disconnected);
                                //_checkConnectionThread.Abort();
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        Win32APIs.SendMsgData("tryConnection:CONNECTION 실패 1:" + e.Message + "재접속 시도", "LogWindow");
                        Win32APIs.SendMsgData("tryConnection:재접속시도", "LogWindow");

                        if (_isPermarnentConnecting)
                        {
                            Win32APIs.SendMsgData("tryConnection:계속 접속모드: 재접속 호출", "LogWindow");
                            _connectionState = ConnType.Disconnected;
                            _isConnecting = false;
                            _connectionTimer.Start();
                            //if (_isDisposing == false) StartConnection(_connectionTimeout, _runRecvThreadWhenConnected);
                        }
                        else
                        {
                            _isConnecting = false;
                            Win32APIs.SendMsgData("tryConnection:연결시도 마침.", "LogWindow");
                            ConnectionEvent(ConnType.Disconnected);

                        }
                       // _checkConnectionThread.Abort();
                        return;
                    }
                }
                else
                {
                    //_checkConnectionThread.Abort();
                    return;
                }
            }

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

        public void runRecvThread(){
            if (_recvThread == null || _recvThread.IsAlive == false)
            {
                _isEndConnection = false;
                _recvThread = new Thread(new ThreadStart(this.recvLoop));
                _recvThread.Start();
            }
            else
            {
                _recvThread.Start();
            }
                
        }

        public int ReceiveTimeout { get { return _server.ReceiveTimeout; } set { _server.ReceiveTimeout = value; } }
        

        int loopZero = 3;
        /// <summary>
        /// 다른 곳에서 access하지 말 것. 오직 recvLoop()함수의 while문을 안에 있는지 나타내는 변수이다.
        /// </summary>
        bool _isRecvThreadRunning = false;
        private void recvLoop()
        {

            Console.WriteLine("server ready...");


            _isEndConnection = false;
            Console.WriteLine("accept client!");
            //_sh.setStream(_client.GetStream(),_client.Client);
            BeginAClient();
            try
            {
                _isRecvThreadRunning = true;
                while (!_isEndConnection)
                {
                    int available = _server.Available;
                    Byte[] buff;
                    int size;
                    if (available > 0)
                    {
                        buff = new Byte[available];
                        size = _server.Client.Receive(buff, 0, available, SocketFlags.None);
                    }
                    else
                    {
                        buff = new byte[_headerSize]; //available이 0일때는 다음에 도착할 header를 기다린다.
                        size = _server.Client.Receive(buff, 0, _headerSize, SocketFlags.None);
                    }
                    if (_isDisposing)
                    {
                        _isEndConnection = true;
                        return; //thread를 끝낸다. connection이나 neterror 이벤트를 발생시키지 않는다.
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
                
            }
            catch {
            
            }
            _isRecvThreadRunning = false;

            _isEndConnection = false;
            FinishAClient();
            if (E_NetError != null) E_NetError(this, new NetworkErrorEventArgs(_id, NetworkErrorEventArgs.NetErrorMsg.RECV_THREAD_FINISHED));
            //ConnectionEvent(ConnType.Disconnected);
            //close();
            //OnConnectionFailed("Connection closed..");
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
