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
    public interface INetConnector
    {
        int Available{get;}

        bool isConnected();
        /*
        public StreamHandler getStreamHandler()
        {
            return _sh;
        }
        */

        string getDestIp();

        void ConnectToServer(string server, int port, int readTimeout = 0, int writeTimeout = 0);
        void ServerReady(string server, int port, int readTimeout = 0, int writeTimeout = 0);
        void setServerInfo(string server, int port, int readTimeout = 0, int writeTimeout = 0);
        
        void ReadyForClient();
        void Connect(bool runReceiveThreadWhenConnected);
        
        void Disconnect(Func<int> runFuncBeforeCloseSocket=null);

        int readFromNet(Byte[] buff, int offset, int size);
        
        int write(Byte[] buff, int offset, int size);
        InterfaceFunctions Interface { get; }

        event ConnectionEventHandler E_Connection;
        event NetworkErrorEventHandler E_NetError;
        event TransferEventHandler E_OnReceived;
    }
}
