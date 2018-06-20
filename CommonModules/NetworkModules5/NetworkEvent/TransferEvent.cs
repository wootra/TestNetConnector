using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkModules5
{
    public delegate void TransferEventHandler(object sender, TransferEventArgs e);
    /*    
        public static class NetErrorMsg
        {
            public static int CONNECTION_FAIL = 0;
            public static int DISCONNECTED = 1;
        }
        */
   
    /// <summary>
    /// Buffer에 내용을 받고 나서 그 값을 받아오고, 다음 받을 처리에 대해 정의한다.
    /// ReceiveMode를 셋팅하면 다음에 어떤 모드로 받을 것인지 지정할 수 있다.
    /// </summary>
    public class TransferEventArgs : EventArgs
    {
        public enum TransferMode
        {
            Send=0, Receive
        };
        TransferMode _tMode = TransferMode.Receive;
        public TransferMode tmode { get { return _tMode; } }

        /// <summary>
        /// 다음에 받을 모드는 어떤 모드인지 지정한다.
        /// ReceiveMode를 Data로 지정할 시, DataSizeToGet을 지정해야 한다.
        /// </summary>
        public ReceiveModes ReceiveMode { get; set; }
        int _dataSizeToGet = 0;
        /// <summary>
        /// ReceiveMode가 Data일 때, 받아올 크기를 지정한다.
        /// </summary>
        public int DataSizeToGet { set { _dataSizeToGet = value; } get { return _dataSizeToGet; } }

        int _id = 0;
        /// <summary>
        /// Network module을 처음 생성할 때 지정한 id이다. 중복처리하지 않는다.
        /// </summary>
        public int id { get { return _id; } }

        public String Description { get; set; }

        int _size = 0;
        /// <summary>
        /// 이번 턴에서 받은 Size
        /// </summary>
        public int Size { get { return _size; } }

        int _totalSize = 0;
        /// <summary>
        /// 처리되지 않고 축적된 총량이다. IsHandled를 true로 하면 다음턴에서는 TotalSize와 Size가 같아진다.
        /// </summary>
        public int TotalSize { get { return _totalSize; } }
        /// <summary>
        /// Queue를 사용하지 않고 직접 처리하려면 이 항목을 true로 설정합니다.
        /// 사용하였다면 반드시 UsedSize에 크기를 지정하여야 합니다.
        /// </summary>
        public bool IsHandled = false;

        /// <summary>
        /// Queue를 사용하지 않고 직접 처리하였을 경우, 사용한 Byte수를 지정합니다.
        /// 이 항목이 유효하려면 IsHandled를 true로 설정하여야 합니다.
        /// 만일 버퍼에 받은 내용이 UsedSize보다 크다면 UsedSize만큼만 삭제하고 다음부터 받습니다.
        /// </summary>
        public int UsedSize = 0;
        /* errorMsg  는 NetErrorMsg 의 멤버이다. */

        Byte[] _tempBuffer;
        /// <summary>
        /// 임시로 데이터를 받고 있는 버퍼이다.
        /// </summary>
        public byte[] TempBuffer { get { return _tempBuffer; } }
        /* errorMsg  는 NetErrorMsg 의 멤버이다. */
        public TransferEventArgs(int id, Byte[] tempBuffer, TransferMode transferMode, int size, int totalSize, String description = "")
            : base()
        {
            _tempBuffer = tempBuffer;
            _id = id;
            _tMode = transferMode;
            Description = description;
            _size = size;
            _totalSize = totalSize;
        }
    }
}
