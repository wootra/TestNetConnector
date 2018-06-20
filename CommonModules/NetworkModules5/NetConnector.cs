using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using DataHandling;
using RtwEnums.Network;
using IOHandling;

namespace NetworkModules5
{
    public enum ReceiveModes { Header = 0, Data, All };

    public class NetConnector:INetConnector
    {
        public event ConnectionEventHandler E_Connection;
        public event NetworkErrorEventHandler E_NetError;
        public event TransferEventHandler E_OnCanReceive;
        public event TransferEventHandler E_OnReceivedInQueue;
        public delegate bool? CallBackFunc(Byte[] buffer, Socket socket, int id, NetConnector connector);
        protected CallBackFunc _callBackFunc;
        protected bool _isRunRecvThreadWhenConnected = true;
        protected enum RecvPositions { AfterRecved, AfterQueueEntered };
        protected RecvPositions _recvPos = RecvPositions.AfterRecved;
        public int _headerSize;

        public NetConnector(int headerSize )
        {
            _headerSize = headerSize;
        }

        ReceiveModes _recvMode = ReceiveModes.Header;
        int _dataSizeToGet = 0;
        /// <summary>
        /// 루프내부구문이다.
        /// </summary>
        /// <returns>true를 리턴하면 루프에서 빠져나가는 것이고, false를 리턴하면 continue하는 것이며, null을 리턴하면 그냥 진행한다.</returns>
        protected bool? InRecvLoop(Byte[] buffer, Socket socket, int id, ref int totalSize, ref int errorCount, BufferQueue queue)
        {

            if (_callBackFunc != null)
            {
                return _callBackFunc(buffer, socket, id, this);
            }
            int size = 0;
            try
            {
                if (_recvMode == ReceiveModes.Header)
                {
                    size = socket.Receive(buffer, 0, _headerSize, SocketFlags.None);
                }
                else if (_recvMode == ReceiveModes.Data)
                {
                    size = socket.Receive(buffer, 0, _dataSizeToGet, SocketFlags.None);
                }
                else //all
                {
                    size = socket.Receive(buffer, totalSize, 1, SocketFlags.None);//일단 1개를 받음..
                    if (socket.Available > 0)
                    {
                        size += socket.Receive(buffer, totalSize + 1, socket.Available, SocketFlags.None);//나머지를 받음.
                    }
                }

                _recvPos = RecvPositions.AfterRecved;
                #region when size <= 0
                if (size <= 0)
                {
                    errorCount++; //접속된 직후 이 경우가 있을 수 있으므로..
                    if (errorCount > 3)
                    {

                        if (socket == null || socket.Blocking == false || socket.Connected == false)
                        {
                            //루프에서 나간다.
                            return true;
                        }
                        else
                        {
                            try
                            {
                                socket.Close();
                            }
                            catch { }
                            RunConnectionEvent(ConnType.Disconnected, "error count==3", id);
                            socket = null;
                        }
                        errorCount = 0;
                        return true;
                    }
                    return false;
                }
                else
                {
                    errorCount = 0;
                }
                #endregion
            }
            catch
            {
                if (E_NetError != null) E_NetError(this, new NetworkErrorEventArgs(id, NetworkErrorEventArgs.NetErrorMsg.FAIL_WHEN_RECV_MSG));
                return true;
            }
                
            TransferEventArgs args;


            if (_recvMode == ReceiveModes.Header || _recvMode == ReceiveModes.Data)
            {
                args = new TransferEventArgs(id, buffer, TransferEventArgs.TransferMode.Receive, size, size, "");
                args.ReceiveMode = _recvMode;
            }
            else
            //if (_recvMode == ReceiveModes.All)
            {
                totalSize += size;
                
                args = new TransferEventArgs(id, buffer, TransferEventArgs.TransferMode.Receive, size, totalSize, "");
                args.ReceiveMode = _recvMode;
            }
            if (E_OnCanReceive != null) 
                E_OnCanReceive(this, args);


            if (_recvMode == ReceiveModes.All)
            {
                _recvMode = args.ReceiveMode;
                _dataSizeToGet = args.DataSizeToGet;

                if (args.IsHandled)
                {
                    if (args.UsedSize > 0
                        && totalSize > args.UsedSize)
                    //직접처리했는데 버퍼에 있는 내용을 다 소비하지 않았다면..
                    {
                        //남은 부분을 앞으로 가져옴..
                        Buffer.BlockCopy(buffer, args.UsedSize, buffer, 0, totalSize - args.UsedSize);
                        totalSize -= args.UsedSize;
                    }
                    else//이 경우 모든 받은 내용을 처리했다고 가정한다.
                    {
                        totalSize = 0;//버퍼를 초기화한다. 제일 처음부터 받게 된다.
                    }
                }
                else if (E_OnReceivedInQueue != null) //직접처리하지 않았으므로, queue에 넣어줌..
                {
                    queue.enqueueFrom(buffer, 0, totalSize);
                    _recvPos = RecvPositions.AfterQueueEntered;
                    totalSize = 0;
                    args = new TransferEventArgs(id, buffer, TransferEventArgs.TransferMode.Receive, size, queue.Size, "");

                    if (E_OnReceivedInQueue != null) E_OnReceivedInQueue(this, args);
                }
            }
            else if (_recvMode == ReceiveModes.Header)
            {
                _recvMode = args.ReceiveMode;
                _dataSizeToGet = args.DataSizeToGet;

            }
            else if (_recvMode == ReceiveModes.Data)
            {
                _recvMode = args.ReceiveMode;
                _dataSizeToGet = args.DataSizeToGet;

            }

            return null;
        }

        public void setFuncInLoop(CallBackFunc callBackFunc)
        {
            _callBackFunc = callBackFunc;
        }
        public void removeFuncInLoop()
        {
            _callBackFunc = null;
        }

        protected void RunConnectionEvent(ConnType connType, String msg, int id){
            if(E_Connection!=null) E_Connection(this, new ConnectionEventArgs(connType, msg, id));
        }

        protected void RunNetErrorEvent(int id, NetworkErrorEventArgs.NetErrorMsg netErrorMsg, string errorStr)
        {
            if (E_NetError != null) E_NetError(this, new NetworkErrorEventArgs(id, netErrorMsg, errorStr));
        }


        public virtual int Available { get; set; }


        public virtual bool isConnected() { return false; }

        public virtual string getRemoteIp() { return ""; }



        public virtual void Disconnect(Func<int> runFuncBeforeCloseSocket = null) { }

        public virtual int readFromNet(byte[] buff, int offset, int size) { return -1; }

        public virtual int write(byte[] buff, int offset, int size) { return -1; }

        public virtual int write(byte[] buff, int size) { return -1; }

        public virtual InterfaceFunctions Interface { get { return null; } }

        public virtual void Close() {}
    }
}
