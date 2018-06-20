using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Net;

namespace NetworkModules
{
    
    public class NetHandlerBase
    {
        private StreamHandler _sh; 
        /* stream in Streamhandler is changable up to the client connected but streamhandler itself is not changable. */
        private Byte[] _readBuff;
        private Byte[] _writeBuff;
        protected Byte[] ReadBuffer { get { return _readBuff; } }
        protected Byte[] WriteBuffer { get { return _writeBuff; } }

        public delegate int getDataSizeFunc(int header);
        public event NetworkErrorEventHandler ErrorEvent;
        
        
        public NetHandlerBase(StreamHandler sh)
        {
            _sh = sh;
            _readBuff = new Byte[4096];
            _writeBuff = new Byte[4096];
        }

        protected NetworkStream netStream() { return _sh.getStream(); }

        public Socket getSocket() { return _sh.getSocket(); }
       
        public void writePacketHeaderSwapped(NetworkObjectBase header, Byte[] dataBuff,int dataSize=-1)
        {

            int headerSize = header.bufferByteSize;
            dataSize = (dataSize < 0) ? header.__dataSize : dataSize;
            
            try
            {
                
                netStream().Write(header.getByteBufferSwapCopied(), 0, header.bufferByteSize);
                if(dataSize > 0) netStream().Write(dataBuff, 0, dataSize);
            }
            catch (Exception e)
            {
                Console.WriteLine("NetHandlerBase::writePacketHeaderSwapped - Error when write packet..." + e);
                
            }
        }

        public void writePacket(NetworkObjectBase header, Byte[] dataBuff, int dataSize = -1)
        {
            int headerSize = header.bufferByteSize;
            dataSize = (dataSize < 0) ? header.__dataSize : dataSize;

            try
            {
                netStream().Write(header.getByteBufferCopied(), 0, header.bufferByteSize);
                if (dataSize > 0) netStream().Write(dataBuff, 0, dataSize);
            }
            catch (Exception e)
            {
                Console.WriteLine("NetHandlerBase::writePacketHeaderSwapped - Error when write packet..." + e);

            }
        }
        public int readPacketSwapped(NetworkObjectBase header, ref Byte[] data, int dataSize = -1)
        {
            int gotSize = tryRead(header.bufferByteSize, ref _readBuff);
            if (gotSize == header.bufferByteSize) header.copyBufferSwapFromArray(_readBuff);
            else
            {
                return 0;
            }

            dataSize = (dataSize > 0) ? dataSize : header.__dataSize;
            if (data == null) data = new Byte[dataSize];
            if (dataSize > 0)
            {
                int retSize = tryRead(dataSize, ref data); // data should now swapped. it would swap in the application level.
                return retSize;
            }
            else
                return 0;
        }

        public int readPacket(NetworkObjectBase header, ref Byte[] data, int dataSize = -1)
        {

            int gotSize = tryRead(header.bufferByteSize, ref _readBuff);
            if (gotSize == header.bufferByteSize) header.copyBufferFromArray(_readBuff);
            else
            {
                netStream().Flush(); //제대로 읽어오지 못했다면 메시지큐의 모든 내용을 한번에 소비한다..
                return 0;
            }
            dataSize = (dataSize > 0) ? dataSize : header.__dataSize;
            if (data == null) data = new Byte[dataSize];
            if (dataSize > 0)
                return tryRead(dataSize, ref data);
            else
                return 0;
        }

        protected int tryRead(int size, ref byte[] readBuff)
        {
            int gotSize = 0;
            int loopLimit = 100; // 한 번에 1byte씩은 읽어온다고 가정하고 최대 100번 돈다.
            int nowGet;
            //if (readBuff == null) readBuff = getReadBuff();

            try
            {
                do
                {
                    nowGet = netStream().Read(readBuff, gotSize, size - gotSize);
                    if (nowGet < 0)
                    {
                        ErrorEvent(this, new NetworkErrorEventArgs(-1, NetworkErrorEventArgs.NetErrorMsg.DISCONNECTED));
                        return -1;
                    }
                    gotSize += nowGet;
                    if (netStream() == null || loopLimit-- == 0)
                    {
                        ErrorEvent(this, new NetworkErrorEventArgs(-1, NetworkErrorEventArgs.NetErrorMsg.DISCONNECTED));
                        return -1;
                    }
                    Console.WriteLine("trying read from buffer..(" + gotSize + "/" + size + ")");
                } while (gotSize < size);
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine("NullReferenceException..." + e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("NetHandlerBase::TryRead() - socket 연결 끊김." + e.Message);
                Console.WriteLine("size: " + size + "/buffer size:" + readBuff.Length + " gotSize:" + gotSize);
                ErrorEvent(this, new NetworkErrorEventArgs(-1, NetworkErrorEventArgs.NetErrorMsg.DISCONNECTED));
                return -1;
            }
            return gotSize;
        }


    }
}
