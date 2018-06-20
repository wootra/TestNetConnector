using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using DataHandling;
using NetworkPacket;
namespace NetworkModules4
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
        string getRemoteIp();
        void Disconnect(Func<int> runFuncBeforeCloseSocket=null);

        int readFromNet(Byte[] buff, int offset, int size);
        
        int write(Byte[] buff, int offset, int size);
        int write(Byte[] buff, int size);
        InterfaceFunctions Interface { get; }

        event ConnectionEventHandler E_Connection;
        event NetworkErrorEventHandler E_NetError;
        event TransferEventHandler E_OnCanReceive;
        event TransferEventHandler E_OnReceivedInQueue;

        void Close();
    }
}
