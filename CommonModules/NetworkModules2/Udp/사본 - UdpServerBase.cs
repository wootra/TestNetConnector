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
    public class UdpServerBase:IServerBase,INetConnector
    {
        public enum SendType { Normal = 0, Multicast = 1, Broadcast = 2 };
        #region properties
        //private StreamHandler _sh;
        private Thread _thread;
        protected bool _isRecvEventEnabled = true;
        private Boolean _isEndConnection;
        private String _ip;
        public delegate void CallBackFunc();
        private CallBackFunc _callBackFunc;

        
        protected BufferQueue _queue;
        private Boolean _isRecvQueueUsing = false;
        private FileStream _fileForSave = null;
        SendType _sendType;
        #endregion

        public UdpServerBase(SendType type=SendType.Normal){
            _sendType = type;
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
                if (_queue != null) return _queue.Count;
                else return _uSocket.Available;
            }
        }
        protected virtual void OnRead(int size, int totalSize) {  }


        public void SuspendRecvEvent()
        {
            _isRecvEventEnabled = false;
        }
        public void ResumeRecvEvent()
        {
            _isRecvEventEnabled = true;
        }

        public void stopUsingReceiveQueue()
        {
            _queue.Clear();
            _isRecvQueueUsing = false;
        }
        public Boolean IsReceiveQueueUsing() { return _isRecvQueueUsing; }

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
            return (_uSocket == null) ? false : _uSocket.Connected;
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

        public void setServerInfo(string initIp, int port, FileStream saveFile = null, int readTimeout = 0, int writeTimeout = 0)
        {
            _ip = initIp;
            _port = port;
            _isEndConnection = false;
            _fileForSave = saveFile;
            //if (server != null && server.Length > 7) _localEP = new IPEndPoint(NetFunctions.getIP4Address(server), port);
            //else _localEP = new IPEndPoint(NetFunctions.getMyIP4Address(), port);
            _readTimeout = readTimeout;
            _writeTimeout = writeTimeout;
            


            //            setServer(server, port, null, readTimeout, writeTimeout);
        }
        public void StartServerReady()
        {

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
                try
                {
                    if (_uSocket != null) _uSocket.Close();
                }
                catch { }
                _uSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                _uSocket.Bind(_localEP);


            }
            else if (_sendType == SendType.Multicast)
            {
                _uSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                _uSocket.Bind(_localEP);

                _uSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(IPAddress.Parse(_ip)));
                _uSocket.SetSocketOption(SocketOptionLevel.Udp, SocketOptionName.MulticastTimeToLive, 1);
            }
            else //broadcast
            {
                _uSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                _uSocket.Bind(_localEP);

                _uSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.Broadcast, true);
            }
            if(_readTimeout>0) _uSocket.ReceiveTimeout = _readTimeout;
            if(_writeTimeout>0) _uSocket.SendTimeout = _writeTimeout;

            _remoteEP = new IPEndPoint(IPAddress.None, _port); //this is blank IPEndPoint yet.
            
            useReceiveQueue(null); //udp는 무조건 queue를 쓴다.
            
            runRecvThread();
        }

        public void setServer(string initIp, int port, Boolean isUsingReceiveQueue=true, FileStream saveFile=null, int readTimeout = 0, int writeTimeout = 0)
        {
            _port = port;
            _isEndConnection = false;
            _fileForSave = saveFile;
            //if (server != null && server.Length > 7) _localEP = new IPEndPoint(NetFunctions.getIP4Address(server), port);
            //else _localEP = new IPEndPoint(NetFunctions.getMyIP4Address(), port);
            _readTimeout = readTimeout;
            _writeTimeout = writeTimeout;

            StartServerReady();
//            setServer(server, port, null, readTimeout, writeTimeout);
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
                if (_thread.ThreadState != ThreadState.Running)
                {
                    try {
                        _thread.Abort(); 
                    }
                    catch { }
                    _thread = new Thread(new ThreadStart(this.serverLoop));
                    _thread.Start();
                }

            }
        }

        public void close()
        {
            if (_isEndConnection == true) return;
            endThisClient();
            if (_thread!=null && _thread.IsAlive)
            {
                _isEndConnection = true;
                _thread.Join(100);//.Abort();
                if (_thread!=null && _thread.ThreadState == ThreadState.Running)
                {
                    
                    FinishAClient();
                    Thread tt = _thread;
                    _thread = null;
                    tt.Abort();
                }
                try
                {
                    _uSocket.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                
               // _thread.Join();
            }
            _thread = null;
        }
        public void readMore()
        {
            int size=0;
            

            try
            {
                size = _uSocket.ReceiveFrom(_tempBuff, ref _remoteEP);
                if (size > 0)
                {
                    _queue.enqueueFrom(_tempBuff, 0, size, _fileForSave);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("socket exception" + e.ToString());
                return;
                
            }

            
        }

        public void endThisClient()
        {
            //if(_sh!=null) _sh.closeStream();
            _isEndConnection = true;
           // _isRecvQueueUsing = false;

        }
        protected virtual void OnReadFails() { }
        private byte[] _tempBuff = new byte[8192];
        
        private void serverLoop(){

                BeginAClient();
                int blankCount = 200;
                 
                while (!_isEndConnection)
                {
                    //_sh.setStream(_client.GetStream(),_client.Client);
                    
                    

                    int size = 0;
                    try
                    {
                        size = _uSocket.ReceiveFrom(_tempBuff, ref _remoteEP);
                    }
                    catch(Exception e) {
                        Console.WriteLine("socket exception" + e.ToString());
                        //throw;
                    }
                    funcRunningInServerLoopForClient();
                    if (_callBackFunc != null) _callBackFunc();


                    if (size > 0)
                    {
                        _queue.enqueueFrom(_tempBuff, 0, size, _fileForSave);
                        OnRead(size, _queue.Count);
                        blankCount = 200;
                    }
                    else
                    {
                        blankCount--;
                    }
                    if (blankCount == 0)
                    {
                        Console.WriteLine("blank msg received...: UdpServerBase.cs");
                        break;
                    }


                }
                FinishAClient();
                Console.WriteLine("exit from while in Server...");
                

            

        }
        public int read(Array buff, int size = -1)
        {
            return read(buff, 0, size);
        }
        public virtual void OnReadTimeout(){}
        public virtual void OnConnectionFailed() { }
       // Byte[] tempBuff = new Byte[8192];
        public int read(Array buff, int offset, int size)
        {
            int realSize = (_queue.Count > size) ? size : _queue.Count;
            realSize = (realSize < buff.Length) ? realSize : Buffer.ByteLength(buff);
            _queue.dequeueTo(buff, offset, realSize, null);
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
            try
            {
                 int ret = _uSocket.SendTo(buff, offset, size, SocketFlags.None, _remoteEP);
                 return ret;
            }
            catch(Exception e)
            {
                Console.WriteLine("UdpServerBase:write = 숨겨진 remoteEP에 엑세스 시도..." + e.Message + ":" + e.StackTrace);
                return 0;
            }
        }
 
    }
}
