using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

//마지막 수정 : 2011/6/14 - Dns.Resolve는 더이상 사용되지 않으므로 Dns.GetHostEntry로 변경.
namespace NetworkModules3.Samples
{
    public class MyUdpClient
    {
        private Socket _uSocket;
        private EndPoint _localEP;
        private EndPoint _remoteEP;

        public MyUdpClient()
        {
                _localEP = new IPEndPoint(Dns.Resolve("192.168.1.127").AddressList[0], IPEndPoint.MinPort + 1);
            //_localEP = new IPEndPoint(IPAddress.Loopback, IPEndPoint.MinPort + 1);
            _uSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _uSocket.Bind(_localEP);
            
        }
        public void setServer(string IP, int port)
        {
            _remoteEP = new IPEndPoint((Dns.Resolve(IP)).AddressList[0], port);
            //_remoteEP = new IPEndPoint(IPAddress.Broadcast, port);
        }
        
		private const int ServerPortNumber = 5432;

		public void EchoTest()
		{
				int i = 0;
				while( i++ < 10 )
				{
					Console.WriteLine("보낼 메세지를 입력하세요 ({0}/10)", i);
					byte[] sendBuffer = Encoding.UTF8.GetBytes(Console.ReadLine());
					_uSocket.SendTo(sendBuffer, _remoteEP);

					byte[] receiveBuffer = new byte[512];
					int receivedSize = _uSocket.ReceiveFrom(receiveBuffer, ref _remoteEP);
					Console.Write("UDP 에코 메세지 : ");
					Console.WriteLine(Encoding.UTF8.GetString(receiveBuffer, 0, receivedSize));
				}				
				_uSocket.Close();
			//}
			//catch( SocketException se )
			//{
			//	Console.WriteLine(se.Message);
			//}
		}
    }
}
