using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace NetworkModules5
{
    public class StreamHandler
    {
        private NetworkStream _nStream;
        private StreamReader _sReader;
        private StreamWriter _sWriter;
        private int _readTimeout;
        private int _writeTimeout;
        private Encoding _streamEncoding;
        private Socket _socket;
        
        public StreamHandler(Encoding encoding=null)
        {
            initVars();
            _readTimeout = 0;
            _writeTimeout = 0;
            _streamEncoding = encoding;
        }
        private void initVars()
        {
            _streamEncoding = null;
            _nStream = null;
            _sReader = null;
            _sWriter = null;
            _socket = null;

        }
        public Socket getSocket() { return _socket; }

        public void closeStream()
        {
            if (_nStream != null)
            {
                _sReader.Close();
                _sWriter.Close();
                _nStream.Close();
                if(_socket!=null) _socket.Close();
            }
            initVars();
        }

        public void setStream(NetworkStream ns, Socket socket, Encoding encoding = null)
        {
            _nStream = ns;
            _socket = socket;
            _streamEncoding = encoding;
            if (_streamEncoding == null)
            {
                _sReader = new StreamReader(_nStream);
                _sWriter = new StreamWriter(_nStream);
            }
            else
            {
                _sReader = new StreamReader(_nStream, _streamEncoding);
                _sWriter = new StreamWriter(_nStream, _streamEncoding);
            }
            if (_readTimeout != 0 || _writeTimeout != 0) setStreamTimeout();
        }


        ~StreamHandler()
        {

            if (_nStream != null)
            {
                if (_nStream != null)
                {
                    _nStream.Close();
                    _nStream.Dispose();
                }
                if(_socket!=null) _socket.Close();
                Console.WriteLine("StreamHandler disposed..");
            }
            
        }

        public StreamReader getReader() { return _sReader; }
        public StreamWriter getWriter() { return _sWriter; }
        public NetworkStream getStream() { return _nStream; }

        public void setStreamTimeout(int readTimeout = 0, int writeTimeout = 0)
        {
            _readTimeout = readTimeout;
            _writeTimeout = writeTimeout;
            if (_nStream != null) setStreamTimeout();
        }

        public int readBuff(Byte[] buff, int size = 0)
        {
            return readBuff(buff, 0, size);
        }

        public int readBuff(Byte[] buff, int offset, int size = -1)
        {
            if (size > 0)
            {
                return _nStream.Read(buff, offset, size);
            }
            else if (size == 0)
            {
                return 0;
            }
            else
            {
                return _nStream.Read(buff, offset, buff.Length);
            }

        }

        public void writeBuff(Byte[] buff, int size = -1)
        {
            if (size < 0) size = buff.Length;
            _nStream.Write(buff, 0, size);
        }

        public void writeBuff(Byte[] buff, int offset, int size)
        {
            _nStream.Write(buff, offset, size);
        }

        private void setStreamTimeout()
        {
            if (_nStream.CanTimeout)
            {
                if (_writeTimeout > 0) _nStream.WriteTimeout = _writeTimeout;
                if (_readTimeout > 0) _nStream.ReadTimeout = _readTimeout;
            }
        }
    }
}
