﻿using System;
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

namespace NetworkModules3
{
    public class TcpServerBase:INetConnector, IDisposable
    {
        public event ConnectionEventHandler E_Connection;
        public event NetworkErrorEventHandler E_NetError;
        public event TransferEventHandler E_OnCanReceive;
        public event TransferEventHandler E_OnReceivedInQueue;

        #region properties
        public enum SendType { Normal = 0, SendAllClients };
        private TcpListener _listener;
        private TcpClient[] _client;
        //private StreamHandler _sh;
        private Thread _listenerThread;
        private Thread[] _recvThreads;
        private Boolean _isEndServer;
        private Boolean[] _isEndConnection;
        private InterfaceFunctions[] _functions;
        int[] _totalSize;
        public delegate void CallBackFunc();
        private CallBackFunc _callBackFunc;
        private bool _isRunRecvThreadWhenConnected = true;
        private Timer _readTimer;
        private Timer _writeTimer;

        protected BufferQueue[] _queue;
        private List<byte[]> _tempRecvBuffer;
        int _maxClientNum = 1;
        SendType _sendType;
        
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public TcpServerBase(int maxClientNum=1, SendType type = SendType.Normal, bool runRecvThreadWhenConnected = true, int bufferSize=1024000)
        {
            _maxClientNum = maxClientNum;
            _isRunRecvThreadWhenConnected = runRecvThreadWhenConnected;
            init(type, maxClientNum);
            _tempRecvBuffer = new List<Byte[]>();
            for (int i = 0; i < maxClientNum; i++)
            {
                _tempRecvBuffer.Add(new byte[bufferSize]);
            }
        }
        void init(SendType type, int clients)
        {
            _listener = null;
            _sendType = type;
            _totalSize = new int[clients];
            //_sh = null;
            _client = new TcpClient[clients];
            _recvThreads = new Thread[clients];
            _isEndServer = true;
            _isEndConnection = new bool[clients];
            _callBackFunc = null;
            _queue = new BufferQueue[clients];
            _functions = new InterfaceFunctions[clients];
            
            for(int i=0; i<clients; i++){
                _client[i] = null;
                _queue[i] = new BufferQueue();
                _functions[i] = new InterfaceFunctions(this, _queue[i]);
                _totalSize[i] = 0;
                _isEndConnection[i] = false;
            }

        }

        public InterfaceFunctions[] Interfaces { get { return _functions; } }
        
        public InterfaceFunctions Interface { get { return _functions[getFirstClient()]; } }



        public int Available{
            get{
                if (_recvPos == RecvPositions.AfterQueueEntered)
                //if (_recvThread != null && _recvThread.ThreadState == ThreadState.Running)
                {
                    return _queue[getFirstClient()].Size;
                }
                else
                {
                    return _totalSize[getFirstClient()];// return _server.Available;
                }
                
            }
        }

        public int AvailableEach(int id){
            if (_recvPos == RecvPositions.AfterQueueEntered)
            //if (_recvThread != null && _recvThread.ThreadState == ThreadState.Running)
            {
                return _queue[id].Size;
            }
            else
            {
                return _totalSize[id];// return _server.Available;
            }
        }

        public bool isConnected()
        {
            for (int i = 0; i < _client.Length; i++)
            {
                if (_client != null && _client[i]!=null && _client[i].Client != null)
                {
                    if (_client[i].Client.Connected == true)
                    {
                        return true;
                    };
                }
            }
            return false;
        }

        public Dictionary<int, bool> GetConnectionStates()
        {
            Dictionary<int, bool> conns = new Dictionary<int, bool>();
            for (int i = 0; i < _client.Length; i++)
            {
                if (_client != null && _client[i].Client != null)
                {
                    if (_client[i].Client.Connected == true)
                    {
                        conns[i] = true;
                    }
                    else conns[i] = false;
                }
                else conns[i] = false;
            }
            return conns;
        }
        
        public TcpClient getClient(int id=0)
        {
            if (id >= _client.Length) return null;
            return _client[id];
        }

        protected void ConnectionEvent(ConnType conn, int id)
        {
            if (E_Connection != null)
            {
                
                 E_Connection(this, new ConnectionEventArgs(conn, "", id));
                if (conn == ConnType.Connected)//접속이 되었다면, 해당 id와 함게 conn을 보냄.
                {
                    int conns=0;
                    int connects=0;
                    for (int i = 0; i < _client.Length; i++)
                    {
                        if (_client[i] != null)
                        {
                            conns++;
                            if (_client != null && _client[i].Client != null)
                            {
                                if (_client[i].Client.Connected == true)
                                {
                                    connects++;
                                }
                            }
                        }
                    }
                    if(connects==0) E_Connection(this, new ConnectionEventArgs(ConnType.Disconnected , "모두연결됨.", -1));
                    else if (conns == connects) E_Connection(this, new ConnectionEventArgs(ConnType.Connected, "연결안됨", -1));
                    else E_Connection(this, new ConnectionEventArgs(ConnType.Connecting, "일부만 연결됨", -1));
                }
                
            }
        }

        /// <summary>
        /// 0번의 destIp를 가져온다.
        /// </summary>
        /// <returns></returns>
        public string getRemoteIp()
        {
            return getRemoteIp(0);
        }

        public string getRemoteIp(int id)
        {
            try
            {
                IPEndPoint ep = (IPEndPoint)(getClient(id).Client.RemoteEndPoint);
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


        public void ServerReady(string server, int port, int readTimeout = 0, int writeTimeout = 0)
        {
            setServerInfo(server, port, readTimeout, writeTimeout);
            ReadyForClients();

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

            //            setServer(server, port, null, readTimeout, writeTimeout);
        }

        public void startReceiving()
        {
            ReadyForClients();
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

        public void runListener(){
            if (_listenerThread == null || _listenerThread.IsAlive == false)
            {
                _listenerThread = new Thread(new ThreadStart(this.serverLoop));
                _listenerThread.Start();
            }
            else
            {
                
                if (_listenerThread.ThreadState == ThreadState.Aborted || _listenerThread.ThreadState == ThreadState.AbortRequested)
                {
                    _listenerThread = new Thread(new ThreadStart(this.serverLoop));
                }
                if(_listenerThread.ThreadState!= ThreadState.Running) _listenerThread.Start();
            }
        }

        public void Disconnect(Func<int> runBeforeCloseSocket=null)
        {
            
            _isEndServer = true;
            for (int i = 0; i < _client.Length; i++)
            {
                endThisClient(i);
                if (_client[i] != null)
                {
                    try
                    {
                        if (_client[i].Connected)
                        {
                            if (runBeforeCloseSocket != null) runBeforeCloseSocket.Invoke();
                            _client[i].Client.Blocking = false;
                            _client[i].Client.ReceiveTimeout = 10;
                            _client[i].Client.SendTimeout = 10;
                            _client[i].Close();

                        }
                    }
                    catch { }
                    finally
                    {
                        _queue[i].Clear();
                    }
                }
                _client[i] = null;

            }
            Clear();

        }

        public void Clear()
        {
            for (int i = 0; i < _queue.Length; i++)
            {
                _queue[i].Clear();
            }
        }

        public void Close()
        {
            
            _isEndServer = true;
            for (int i = 0; i < _maxClientNum; i++)
            {
                _isEndConnection[i] = true;

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
            Disconnect();
            
            if (_listenerThread != null && _listenerThread.IsAlive)
            {
                
                Thread tt = _listenerThread;
                _listenerThread = null;
                try
                {
                    tt.Abort();
                }
                catch { }
            }
            _listenerThread = null;
        }

        public void endThisClient(int id=0)
        {
            //if(_sh!=null) _sh.closeStream();
            _isEndConnection[id] = true;
            //_isRecvQueueUsing = false;
            if (_client != null && _client[id]!=null && _client[id].Client!=null)
            {
                try
                {
                    _client[id].Client.Close();
                }
                catch { }
                
                //FinishAClient();
                
            }
            _client[id] = null;

        }


        private int _headerSize = 32; //headerSize는 데이터가 도착한 이후 available이 0일 때 기본으로 받을크기를 정하는데 사용된다.
        public void setHeaderSize(int byteSize)
        {
            _headerSize = byteSize;
        }

        int getBlankClient()
        {
            for (int i = 0; i < _client.Length; i++)
            {
                if (_client[i] == null) return i;
            }
            return -1;
        }

        int getFirstClient()
        {
            
                for (int i = 0; i < _client.Length; i++)
                {
                    if (_client[i] != null) return i;
                }
            
            return -1;
        }

        
        


        Socket _runningSocket = null;
        private void serverLoop(){
                
                while (!_isEndServer)
                {
                    int id=-1;
                    Console.WriteLine("server ready...");
                    try
                    {
                        
                        TcpClient client = _listener.AcceptTcpClient();
                        if (_isEndServer) break;
                        id = getBlankClient();
                        if (id >= 0) _client[id] = client;
                        else
                        {
                            Win32APIs.SendMsgData("Server Listener: 한계크기"+_maxClientNum+"보다 큰 접속 시도. 끊음..", "LogWindow");
                            client.Close();//그냥 끊어버림..
                            Thread.Sleep(1000);//1초 뒤에 다시 받음..
                            continue;
                        }
                        _isEndConnection[id] = false;
                        _queue[id].Clear();//초기화
                        ConnectionEvent(ConnType.Connected, id);
                    }
                    catch
                    {
                        this.Disconnect();
                        OnConnectionFailed();
                        break;
                    }
                    if (_isRunRecvThreadWhenConnected)
                    {
                        makeRecvThread(id);
                    }
                    
                }
                Console.WriteLine("exit from while in Server...");
                _listener = null;
           

        }

        int _currentId = -1;
        void makeRecvThread(int id)
        {
            _recvThreads[id] = new Thread(new ThreadStart(recvLoop));
            _currentId = id;
            _recvThreads[id].Start();
        }

        enum RecvPositions { AfterRecved, AfterQueueEntered };
        RecvPositions _recvPos = RecvPositions.AfterRecved;
        //bool _isRecvThreadRunning = true;
        int loopZero = 3;
        private void recvLoop()
        {
            int id = _currentId;
            Console.WriteLine("server ready...");


            //_isEndConnection = false;
            Console.WriteLine("accept client!");
            //_sh.setStream(_client.GetStream(),_client.Client);
            BeginAClient();

            //_isRecvThreadRunning = true;
            _totalSize[id] = 0;
            int errorCount = 0;
            try
            {
                while (!_isEndConnection[id])
                {

                    funcRunningInServerLoopForClient();
                    if (_callBackFunc != null) _callBackFunc();
                    int size = 0;
                    try
                    {
                        size = _client[id].Client.Receive(_tempRecvBuffer[id], _totalSize[id], 1, SocketFlags.None);//일단 1개를 받음..
                        if (_client[id].Available > 0)
                        {
                            size += _client[id].Client.Receive(_tempRecvBuffer[id], _totalSize[id] + 1, _client[id].Available, SocketFlags.None);//나머지를 받음.
                        }
                        _recvPos = RecvPositions.AfterRecved;
                        if (size <= 0)
                        {
                            errorCount++; //접속된 직후 이 경우가 있을 수 있으므로..
                            if (errorCount > 3)
                            {

                                if (_client[id].Client == null || _client[id].Client.Blocking == false || _client[id].Client.Connected == false)
                                {
                                    //루프에서 나간다.
                                    break;
                                }
                                else
                                {
                                    try
                                    {
                                        _client[id].Close();
                                    }
                                    catch { }
                                    ConnectionEvent(ConnType.Disconnected,id);
                                    _client[id] = null;
                                }
                            }
                            continue;
                        }
                        else
                        {
                            errorCount = 0;
                        }
                    }
                    catch
                    {
                        if (E_NetError != null) E_NetError(this, new NetworkErrorEventArgs(id, NetworkErrorEventArgs.NetErrorMsg.FAIL_WHEN_RECV_MSG));
                        break;
                    }
                    
                    _totalSize[id] += size;
                    TransferEventArgs args = new TransferEventArgs(id, TransferEventArgs.TransferMode.Receive, size, _totalSize[id], "");
                    args.TempBuffer = _tempRecvBuffer[id];
                    if (E_OnCanReceive != null) E_OnCanReceive(this, args);

                    if (args.IsHandled)
                    {
                        if (args.UsedSize > 0 && _totalSize[id] > args.UsedSize)//직접처리했는데 버퍼에 있는 내용을 다 소비하지 않았다면..
                        {
                            //남은 부분을 앞으로 가져옴..
                            Buffer.BlockCopy(_tempRecvBuffer[id], args.UsedSize, _tempRecvBuffer[id], 0, _totalSize[id] - args.UsedSize);
                            _totalSize[id] -= args.UsedSize;
                        }
                    }
                    else if(E_OnReceivedInQueue != null) //직접처리하지 않았으므로, queue에 넣어줌..
                    {
                        _queue[id].enqueueFrom(_tempRecvBuffer[id], 0, _totalSize[id]);
                        _recvPos = RecvPositions.AfterQueueEntered;
                        _totalSize[id] = 0;
                        args.totalSize = _queue[id].Size;

                        if (E_OnReceivedInQueue != null) E_OnReceivedInQueue(this, args);
                    }
                    
                }
            }
            catch(Exception e)
            {
                try
                {
                    _client[id].Close();
                }
                catch { }
            }
           // _isRecvThreadRunning = false;
            
            //_isEndConnection = false;
            _client[id] = null;
            FinishAClient();
            if (E_NetError != null) E_NetError(this, new NetworkErrorEventArgs(id, NetworkErrorEventArgs.NetErrorMsg.RECV_THREAD_FINISHED));
            //ConnectionEvent(ConnType.Disconnected);
            //close();
            //OnConnectionFailed("Connection closed..");
            ConnectionEvent(ConnType.Disconnected, id);
            //close();
            //OnConnectionFailed("Connection closed..");
        }

        public virtual void OnReadTimeout(){}
        public virtual void OnConnectionFailed() { }

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

        public void ReadyForClients()
        {
            runListener();
        }

        /// <summary>
        /// 특정 id를 가진 client에게 보낸다. id에 -1을 넣으면 모두에게 보낸다.
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public int write(byte[] buff, int offset, int size, int id)
        {
            if (_sendType == SendType.SendAllClients)
            {
                int sent = -1;
                for (int i = 0; i < _client.Length; i++)
                {
                    if(_client[i]!=null) sent = _client[i].Client.Send(buff, offset, size, SocketFlags.None);
                }
                return sent;
            }
            else
            {
                return _client[id].Client.Send(buff, offset, size, SocketFlags.None);
            }
        }

        public int write(Byte[] buff, int size)
        {
            return write(buff, 0, size);
        }

        public int readFromNet(Byte[] buff, int offset, int size, int id)
        {
            return _client[id].Client.Send(buff, offset, size, SocketFlags.None);
        }

        public int readFromNet(Byte[] buff, int offset, int size)
        {
            return readFromNet(buff, offset, size, 0);
        }

        public int write(Byte[] buff, int offset, int size)
        {
            return write(buff, offset, size, getFirstClient());
        }

        

        /// <summary>
        /// 특정 id를 가진 client에게 보낸다. id에 -1을 넣으면 모두에게 보낸다.
        /// </summary>
        /// <param name="buff">보낼 내용</param>
        /// <param name="offset">시작점</param>
        /// <param name="size">크기</param>
        /// <param name="ret">각 id별 return size. id,size 의 쌍이다.</param>
        /// <returns>보낸 총 량을 리턴한다.</returns>
        public int writeToAll(byte[] buff, int offset, int size, out Dictionary<int,int> ret)
        {
            ret = new Dictionary<int, int>();

            int sent = -1;
            int total = 0;
            for (int i = 0; i < _client.Length; i++)
            {
                if (_client[i] != null)
                {
                    sent = _client[i].Client.Send(buff, offset, size, SocketFlags.None);
                    ret[i] = sent;
                    total += sent;
                }
                else
                {
                    ret[i] = 0;
                }
            }
            return total;
            
        }

        public void Dispose()
        {
            Close();
        }
    }
}
