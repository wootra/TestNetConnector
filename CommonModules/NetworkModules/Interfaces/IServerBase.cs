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
    public interface IServerBase
    {
        void useReceiveQueue(FileStream file = null);

        int Available{get;}
        
        void stopUsingReceiveQueue();
        Boolean IsReceiveQueueUsing();

        Boolean isEndConnection();

        bool isConnected();
        /*
        public StreamHandler getStreamHandler()
        {
            return _sh;
        }
        */

        string getSourceIp();

        void setServer(string server, int port, Boolean isUsingReceiveQueue = true, FileStream saveFile = null, int readTimeout = 0, int writeTimeout = 0);
        
      
        void runRecvThread();
        void close();


    }
}
