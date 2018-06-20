using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataHandling;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace NetworkModules4
{
    public class InterfaceFunctions
    {
        BufferQueue _queue;
        INetConnector _conn;

        public InterfaceFunctions(INetConnector connector, BufferQueue queue)
        {
            _queue = queue;
            _conn = connector;
        }
        public int Available
        {
            get
            {
                return _queue.Size;
            }
        }
        public void ClearBuffer()
        {
            _queue.Clear();
            //_isRecvQueueUsing = false;
        }


        public int U_Read(Array buff, int size = -1)
        {
            return U_Read(buff, 0, size);
        }

        public enum ReadOption { ReadForced = 0, ReadWhenSizeAvailable };
        public int U_Read(Array buff, int offset, int size, ReadOption option = ReadOption.ReadWhenSizeAvailable)
        {
            if (option == ReadOption.ReadWhenSizeAvailable)
            {
                if (_queue.Size < size) return 0;
                else
                {
                    _queue.dequeueTo(buff, offset, size);
                    return size;
                }
            }
            else
            {

                int realSize = (_queue.Size > size) ? size : _queue.Size;
                _queue.dequeueTo(buff, offset, realSize);
                return realSize;
            }
        }
        public Array U_Read()
        {
            return _queue.dequeue();
        }

        public int write(Byte[] buff)
        {
            return U_Write(buff, 0, buff.Length);
        }

        public int U_Write(Byte[] buff, int size = -1)
        {
            return U_Write(buff, 0, size);
        }
        public int U_Write(Byte[] buff, int offset, int size)
        {
            if (size < 0) size = buff.Length;

            return _conn.write(buff, offset, size);
        }
        public int BufferSize
        {
            get { return _queue.Size; }
        }
    }
}
