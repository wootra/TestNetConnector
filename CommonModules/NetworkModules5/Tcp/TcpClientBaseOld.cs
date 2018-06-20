using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Net.NetworkInformation;


namespace NetworkModules5.Tcp //.Tcp없애지 말것. 구버전과의 호환성을 위해서..
{
    public abstract class TcpClientBase
    {
        public TcpClient _client;
        protected StreamHandler _sh;

        private Boolean _isEndLoop;
        public delegate void CallBackFunc();
        private CallBackFunc _funcRunInRecvLoop;
        private enum ConnType { Disconnected, Connecting, Connected };
        private ConnType _connState;
        protected TcpClient Client { get { return _client; } }
        protected StreamHandler SHandler { get { return _sh; } }
        protected Thread _loopThread;
        private String _server;
        private int _port;
        private int _readTimeout;
        private int _writeTimeout;
        private Encoding _encoding;

        Byte[] errBuffer;
                
        public TcpClientBase() {

            _client = null;
            _isEndLoop = true;
            _funcRunInRecvLoop = null;
            _connState = ConnType.Disconnected;

            errBuffer = new byte[] { 1, 2, 3, 4, 5, 6 };
        }
        ~TcpClientBase(){
            if(_connState == ConnType.Connected || _isEndLoop==false || _client!=null) afterDisconnected();
        }
        public StreamHandler getStreamHandler() { return _sh; }
 

        private void tryConnect()
        {
            try
            {
                Console.WriteLine("now connecting to server (" + _server + ":" + _port + ")");
                
                TryingConnect();
                _connState = ConnType.Connecting;
                if (_client != null)
                {
                    try
                    {
                        _client.Close();
                        _sh.closeStream();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("이전에 만든 client가 끊어지지 않아서 끊는 도중에 에러가 발생했습니다."+e.Message);
                    }
                }
                _client = new TcpClient(_server, _port);
                if (_client.Connected) afterConnection();
                else afterDisconnected();
            }
            catch (Exception e)
            {
                afterDisconnected();
                Console.WriteLine("Failed Connecting To server.. TcpClientBase::connectToServer() - " + e.Message);
            }
        }
        private void afterConnection()
        {

            if (_client != null && _client.Connected)
            {
                Console.WriteLine("now connected to server (" + _server + ":" + _port + ")");
                _sh = new StreamHandler(_encoding);

                _sh.setStreamTimeout(_readTimeout, _writeTimeout);
                _sh.setStream(_client.GetStream(), _client.Client);
                //_loopThread.Abort();
                //_loopThread.Join();
                _isEndLoop = false;
                
                

                _loopThread = new Thread(new ThreadStart(clientLoop));
                _loopThread.Start();
                _connState = ConnType.Connected;
                BeginAClient();
            }
        }
        private void afterDisconnected()
        {
            
        }
        public virtual void connectToServer(string server, int port, int readTimeout = 0, int writeTimeout = 0,Encoding encoding=null)
        {
            _server = server;
            _port = port;
            _readTimeout = readTimeout;
            _writeTimeout = writeTimeout;
            _encoding = encoding;
            
            _loopThread = new Thread(new ThreadStart(tryConnect));
            _loopThread.Start();
        }
        protected abstract void BeginAClient();
        protected abstract void FinishAClient();
        protected abstract void TryingConnect();
        protected abstract void RunInLoop();
        
        public void setStreamTimeout(int readTimeout = 0, int writeTimeout = 0)
        //if timeout is 0, networkStream will use default timeout.
        {
            _sh.setStreamTimeout(readTimeout, writeTimeout);
        }

        public void runClientLoop(){
            if(_loopThread.ThreadState == ThreadState.Stopped) _loopThread.Start();
        }

        public void clientLoop() // public for test. it's originally private function.
        {
            while (!_isEndLoop && _client!=null && _client.Connected)
            {
                Console.Write("receive loop!");
                RunInLoop();
            }
            Console.WriteLine("client is finished from the Loop");
            FinishAClient();
            afterDisconnected();

        }

        public void setRecvFuncRunningInLoop(CallBackFunc callBackFunc)
        {
            _funcRunInRecvLoop = callBackFunc;
        }

        public void disconnect()
        {
            //_loopThread.Abort();
            
            _isEndLoop = true;
            _client.Close();
            _client = null;
            _loopThread.Join(1000);
            if (_loopThread!=null && _loopThread.ThreadState == ThreadState.Running)
            {
                
                FinishAClient();
                afterDisconnected();
                Thread tt = _loopThread;
                _loopThread = null;
                tt.Abort();
            }
            _loopThread = null;
            Console.WriteLine("now disconnected from server (" + _server + ":" + _port + ")");
            _connState = ConnType.Disconnected;

            Console.WriteLine("client loop end...");
        }
        public void join()
        {
            _loopThread.Join();
        }
        /*
        public void Echo()
        {
            Console.Write("tag:");
            String msg = Console.ReadLine();
            String reply = "";
            HEADER_COMMAND header = new HEADER_COMMAND();
            header.m_id = 1;
            header.m_tag = Int32.Parse(msg);
            
            if (msg == null | String.IsNullOrEmpty(msg)) afterDisconnected();
            else
            {
                //_sh.getWriter().WriteLine(msg);
                //_sh.getWriter().Flush();
                Byte[] arr = new Byte[header.bufferByteSize];
                header.copyBufferToSwapArray(arr,0);
                _sh.getWriter().Write(arr);
                Console.WriteLine("write!");
                try
                {
                    //reply = _sh.getReader().ReadLine();
                    

                    int i=0;
                    Console.Write("server replied : ");
                    while ((i = _sh.getStream().ReadByte()) > 0)
                    {
                        Console.Write("{0:X2}", i);
                    }
                    Console.WriteLine("/END");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Echo() said : " + e.Message);
                }
                Console.WriteLine("Server replied : " + reply);
            }

        }
*/
/*
        public void sampleClient(string server, int port)
        {
            NetworkStream ns = null;
            StreamWriter sw = null;
            TcpClient client = null;

            try
            {
                client = new TcpClient(server, port);
                Console.WriteLine("connect to server...");
                ns = client.GetStream();
                sw = new StreamWriter(ns, Encoding.Unicode);
                string ss = String.Empty;
                Console.WriteLine("입력하세요");
                while ((ss = Console.ReadLine()) != null)
                {
                    sw.WriteLine(ss);
                    sw.Flush();
                    
                    Console.WriteLine("write!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if(sw!=null) sw.Close();
                if(client!=null && client.Connected) client.Close();
            }
        }
*/

        
    }
}
