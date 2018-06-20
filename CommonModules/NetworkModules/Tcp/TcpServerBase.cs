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
    public class TcpServerBase:IServerBase,INetConnector
    {
        #region properties
        public enum SendType { Normal = 0, Multicast = 1, Broadcast = 2 };
        private TcpListener _listener;
        private TcpClient _client;
        //private StreamHandler _sh;
        private Thread _thread;
        protected bool _isRecvEventEnabled = true;
        private Boolean _isEndServer;
        private Boolean _isEndConnection;

        public delegate void CallBackFunc();
        private CallBackFunc _callBackFunc;

        private Timer _readTimer;
        private Timer _writeTimer;

        protected BufferQueue _queue;
        private Boolean _isRecvQueueUsing = false;
        private FileStream _fileForSave = null;
        SendType _sendType;
        #endregion

        public TcpServerBase(SendType type = SendType.Normal)
        {
            _listener = null;
            _sendType = type;
            //_sh = null;
            _client = null;
            _isEndServer = true;
            _isEndConnection = true;
            _callBackFunc = null;
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
                else return _client.Available+_totalSize;
            }
        }
        protected virtual void OnRead(int size, int totalSize) {  }
        
        public void stopUsingReceiveQueue()
        {
            _queue.Clear();
            _isRecvQueueUsing = false;
        }
        public Boolean IsReceiveQueueUsing() { return _isRecvQueueUsing; }

        public void SuspendRecvEvent()
        {
            _isRecvEventEnabled = false;
        }
        public void ResumeRecvEvent()
        {
            _isRecvEventEnabled = true;
        }

        public Boolean isEndConnection()
        {
            return _isEndServer;
        }

        public bool isConnected()
        {
            return (_client == null || _client.Client == null) ? false : _client.Connected;
        }
        /*
        public StreamHandler getStreamHandler()
        {
            return _sh;
        }
        */

        public TcpClient getClient()
        {
            return _client;
        }

        public string getSourceIp()
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
        public void setServer(string server, int port, Boolean isUsingReceiveQueue=false, FileStream saveFile=null, int readTimeout = 0, int writeTimeout = 0)
        {
            
            if (_listener != null) return;

            _isEndServer = false;

            IPEndPoint ipe;
            if(server!=null && server.Length>7) ipe = new IPEndPoint(NetFunctions.getIP4Address(server), port);
            else ipe = new IPEndPoint(NetFunctions.getMyIP4Address(), port);
            _listener = new TcpListener(ipe);
            _listener.Start();
            if (isUsingReceiveQueue)
            {
                useReceiveQueue(saveFile);
                
            }
            _readTimeout = readTimeout;
            _writeTimeout = writeTimeout;
            runRecvThread();

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
        public void setServerInfo(string server, int port, Boolean isUsingReceiveQueue = false, FileStream saveFile = null, int readTimeout = 0, int writeTimeout = 0)
        {

            if (_listener != null) return;

            _isEndServer = false;

            IPEndPoint ipe;
            if (server != null && server.Length > 7) ipe = new IPEndPoint(NetFunctions.getIP4Address(server), port);
            else ipe = new IPEndPoint(NetFunctions.getMyIP4Address(), port);
            _listener = new TcpListener(ipe);
            _listener.Start();
            if (isUsingReceiveQueue)
            {
                useReceiveQueue(saveFile);

            }
            _readTimeout = readTimeout;
            _writeTimeout = writeTimeout;
            

            //            setServer(server, port, null, readTimeout, writeTimeout);
        }

        public void startReceiving()
        {
            runRecvThread();
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
                if(_thread.ThreadState!= ThreadState.Running) _thread.Start();
            }
        }

        public void close()
        {
            if (_isEndServer == true) return;
            endThisClient();
            _isEndServer = true;
                
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
        protected virtual void OnReadFails() { }
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
            size = _client.Client.Receive(_tempBuff, 0, unit, SocketFlags.None);

            if (size > 0)
            {
                _queue.enqueueFrom(_tempBuff, 0, size);
            }


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
                        this.close();
                        OnConnectionFailed();
                        break;
                    }
                    _isEndConnection = false;
                    Console.WriteLine("accept client!");
                    //_sh.setStream(_client.GetStream(),_client.Client);
                    BeginAClient();
                    

                    int unit=0;
                    int blankCount = 200;
                    while (_client!=null && _client.Connected && !_isEndConnection)
                    {
                        funcRunningInServerLoopForClient();
                        if (_callBackFunc != null) _callBackFunc();

                        if (_isRecvQueueUsing)
                        {
                            try
                            {
                                if (_client == null || _client.Client == null || _client.Connected == false) unit = 1;
                                else unit = _client.Client.Available;
                            }
                            catch
                            {
                                unit = 1;
                            }
                            try
                            {
                                _totalSize = 0;
                                _runningSocket = _client.Client;
                                _totalSize = _client.Client.Receive(_tempBuff, 0, unit, SocketFlags.None);

                            }
                            catch { }
                            if (_client == null || _client.Client == null || _client.Connected == false) blankCount = 0;
                            if ((unit != 0 && _totalSize <= 0) || blankCount == 0)
                            {
                                endThisClient();
                                OnReadFails();
                                OnConnectionFailed();


                                break;
                            }
                            if (_totalSize > 0)
                            {
                                _queue.enqueueFrom(_tempBuff, 0, _totalSize);
                                OnRead(_totalSize, _queue.Size);
                                blankCount = 200;
                            }
                            else
                            {
                                blankCount--;
                            }
                        }
                        
                    }
                    Console.WriteLine("wait other Client...");
                    _client = null;

                    FinishAClient();
                }
                Console.WriteLine("exit from while in Server...");
                _listener = null;
           

        }
        public virtual void OnReadTimeout(){}
        public virtual void OnConnectionFailed() { }
        Byte[] tempBuff = new Byte[8192];

        public void setReceiveTimeout(int timeout){ _readTimeout = timeout;}
        public void setSendTimeout(int timeout){_writeTimeout = timeout;}


        public int receiveData(Array buff, int offset=-1, int size=-1)
        {
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
                        _readTimer = new Timer(onTimeoutTick, TimeoutKind.Read, _readTimeout, _readTimeout);
                    }
                    
                    while (totalSize < size && count-- > 0)
                    {
                        dataSize = _client.Client.Receive(tempBuff, 0, size - totalSize, SocketFlags.None);

                        Buffer.BlockCopy(tempBuff, 0, buff, offset + totalSize, dataSize);

                        if (dataSize <= 0)
                        {
                            OnConnectionFailed();
                            endThisClient();
                            return -1;
                        }
                        else
                        {
                            totalSize += dataSize;
                        }
                    }
                    if(_readTimer!=null) _readTimer.Dispose();
                    Console.WriteLine("all data received:" + totalSize);
                }
                catch (SocketException )
                {
                    endThisClient();
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

                int rest = receiveData(buff, offset + _totalSize, size);//다음에 남은 데이터를 저장한다.
                if (rest < 0)
                {
                    OnConnectionFailed();
                    return -1;
                }
                int all =  rest + _totalSize;
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
                    _writeTimer = new Timer(onTimeoutTick, TimeoutKind.Write, _writeTimeout, _writeTimeout);
                }
                if (_client != null && _client.Client != null)
                {
                    int dataSize = _client.Client.Send(buff, offset, size, SocketFlags.None);
                    if (_writeTimer != null) _writeTimer.Dispose();
                    return dataSize;
                }
                else
                {
                    if (_writeTimer != null) _writeTimer.Dispose();
                    return -1;
                }
                
                
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
