﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkModules
{
    public delegate void TransferEventHandler(object sender, TransferEventArgs e);
    /*    
        public static class NetErrorMsg
        {
            public static int CONNECTION_FAIL = 0;
            public static int DISCONNECTED = 1;
        }
        */
    public class TransferEventArgs : EventArgs
    {
        public enum TransferMode
        {
            Send=0, Receive
        };
        public TransferMode tmode { get; set; }
        public int id { get; set; }
        public String description;
        public int size;
        public int totalSize;

        /// <summary>
        /// Queue를 사용하지 않고 직접 처리하려면 이 항목을 true로 설정합니다.
        /// </summary>
        public bool IsHandled = false;

        /// <summary>
        /// Queue를 사용하지 않고 직접 처리하였을 경우, 사용한 Byte수를 지정합니다.
        /// </summary>
        public int UsedSize = 0;
        /* errorMsg  는 NetErrorMsg 의 멤버이다. */

        /// <summary>
        /// 임시로 데이터를 받고 있는 
        /// </summary>
        public Byte[] TempBuffer=null;

        public TransferEventArgs(int id, TransferMode transferMode, int size, int totalSize, String description = "")
            : base()
        {
            this.id = id;
            tmode = transferMode;
            this.description = description;
            this.size = size;
            this.totalSize = totalSize;
        }
    }
}
