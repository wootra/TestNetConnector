using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace NetworkModules3.Samples
{
    public class MyUdpServer
    {
        private bool _isEnd;
       
        private Socket _uSocket;
        private EndPoint _localEP;
        private EndPoint _remoteEP;

        public MyUdpServer()
        {
            _isEnd=false;
        }
        ~MyUdpServer()
        {
            if (_uSocket != null) _uSocket.Close();
        }
        public void setUdpServerPort(int port)
        {
            try
            {
                _localEP = new IPEndPoint(IPAddress.Any, port); //IPAddress.Any 는 서버전용.
                //_localEP = new IPEndPoint(IPAddress.Broadcast, port);
                _uSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                                
                _uSocket.Bind(_localEP);
                _remoteEP = new IPEndPoint(IPAddress.None, port); //this is blank IPEndPoint yet.
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
        }
        private SocketAsyncEventArgs asyncComplete(SocketAsyncEventArgs arg)
        {
            return arg;
        }

        public void Echo()
        {
            try
            {
                // 원격컴퓨터에서 데이터를 받아올 receiveBuffer 를 선언함
                byte[] receiveBuffer = new byte[2048];
                SocketAsyncEventArgs arg = new SocketAsyncEventArgs();
                arg.AcceptSocket = _uSocket;
                arg.SetBuffer(receiveBuffer, 0, receiveBuffer.Length);
                //arg.Completed += asyncComplete;
                try
                {
                    Console.WriteLine("UDP 에코 서버를 시작합니다");
                    while (!_isEnd)
                    {
                        // 기다리고 있다가 remoteEP 로부터 데이터를 받는다
                        // receivedSize  : 받은 바이트수
                        // receiveBuffer : 받은 데이터가 들어갈 저장소
                        // remoteEP      : 데이터를 받아올 원격컴퓨터의 IP종단점
                        int receivedSize = _uSocket.ReceiveFrom(receiveBuffer, ref _remoteEP);
                        
                        // _uSocket.ReceiveAsync(SocketAsyncEventArgs.Empty);
                        Console.Write(DateTime.Now.ToShortTimeString() + " 메세지 : ");
                        Console.WriteLine(Encoding.UTF8.GetString(receiveBuffer, 0, receivedSize));

                        // 받은 데이터(receiveBuffer)를 remoteEP 로 다시 보낸다
                        
                        Console.WriteLine("client's IP is " + _remoteEP.ToString());
                        _uSocket.SendTo(receiveBuffer, receivedSize, SocketFlags.None, _remoteEP);
                    }

                }
                catch (SocketException se)
                {
                    Console.WriteLine("exception.."+se.Message);
                }
                finally
                {
                    if (_uSocket != null)
                    {
                        _uSocket.Close();
                        _uSocket = null;
                    }
                }
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.Message);

            }           
        
        }
        public void end()
        {
            _uSocket.Close();
            _uSocket = null;
            _isEnd = true;
            
        }
    }
}
