using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.IO;
using DataHandling;

namespace NetworkModules3
{
    [Serializable]
    public abstract class NetworkObjectBase:ICloneable,IDisposable
    {
        [Browsable(false)]
        public virtual Array headerBuffer{get {return null;}}
        public virtual Array ArrayBuffer{get;set;}
        private Boolean _isDataStruct = false;
        #region properties and get functions *******************************************
        public abstract Boolean isSamePacket(NetworkObjectBase packet);
        
        public virtual int bufferByteSize{get {return -1;}}
        public static int AllPacket = 0;
        

        // 설명:
        // tag, dataSize, id는 네트워크에서 command로 사용하는 패킷으로 사용할 때만 유효하다.
        // 그러나 Mux_Item같은 데이터타입의 값을 읽을때는 프로그램이 에러가 날 것이다.

        [Browsable(false)]
        protected int __tag { get { return (!_isDataStruct)? (int)(headerBuffer.GetValue(0)):-1; } }
        [Browsable(false)]
        protected int __dataSize { get { return (!_isDataStruct) ? (int)(headerBuffer.GetValue(1)) : -1; } }
        [Browsable(false)]
        protected int __id { get { return (!_isDataStruct)? (int)(headerBuffer.GetValue(2)):-1; } }
        

        protected void isDataStruct(Boolean dataStruct)
        {
            _isDataStruct = dataStruct;
        }
        #endregion

        #region copy functions *******************************************
        /// <summary>
        /// *** don't use it. it exists for comportability from lower version. ***
        /// </summary>
        /// <param name="src"></param>
        public void copyHeaderFrom(NetworkObjectBase src)
        {
            headerBuffer.SetValue(src.__tag,0);
            headerBuffer.SetValue(src.__dataSize,1);
            headerBuffer.SetValue(src.__id,2);
        }
        public abstract Byte[] getByteBufferCopied();
        public abstract Byte[] getByteBufferSwapCopied(int swapBaseSize = -1);
        public abstract int copyBufferToArray(Array dst, int offset=0);
        public abstract int copyBufferToSwapArray(Array dst, int offset=0);
        public abstract void copyBufferFromArray(Array src, int offset = 0, int size = -1);
        public abstract void copyBufferSwapFromArray(Array buff, int offset = 0, int size = -1, int swapSize=-1, int dstOffset=0);
        #endregion

        #region  Bits method
        public static Int64 trimBit(Int64 num, int index, int size)
        {
            return (num >> index) & ((1 << size) - 1);
        }

        public static int trimBit(int num, int index, int size)
        {
            return (num >> index) & ((1 << size) - 1);
        }
        public static int trimBit(short num, int index, int size)
        {
            return (num >> index) & ((1 << size) - 1);
        }
        public static int trimBit(byte num, int index, int size)
        {
            return (num >> index) & ((1 << size) - 1);
        }

        public static int addBit(int num, int index, int size, int addNum)
        {
            
            int bitMask = (-1) ^ ((1<<(size-1))<<index);
            num = num & bitMask;
            addNum <<= index;
            
            return num | addNum;
        }



        public static Int64 addBit(Int64 num, int index, int size, Int64 addNum)
        {
            Int64 bitMask = (-1) ^ ((1 << (size - 1)) << index);
            num = num & bitMask;
            addNum <<= index;

            return num | addNum;
        }

        public static void addBitToBuffer(Array buff, int byteIndex, int startBitIndex, Int64 value, int bitSize)
        {
            int byteSize = ((startBitIndex + bitSize - 1) / 8) + 1; //8이하는 1, 16이하는 2...
            Byte[] buffCopy = new Byte[byteSize];
            Buffer.BlockCopy(buff, byteIndex, buffCopy, 0, byteSize);
            if (byteSize == 1)
            {
                buffCopy[0] = (Byte)addBit(buffCopy[0], startBitIndex, bitSize, (int)value);
            }
            else if (byteSize == 2)
            {
                Int16 srcNum = BitConverter.ToInt16(buffCopy, 0);
                srcNum = (Int16)addBit(srcNum, startBitIndex, bitSize, (int)value);
            }
            else if (byteSize == 4)
            {
                Int32 srcNum = BitConverter.ToInt32(buffCopy, 0);
                srcNum = addBit(srcNum, startBitIndex, bitSize, (int)value);
            }
            else if (byteSize == 8)
            {
                Int64 srcNum = BitConverter.ToInt64(buffCopy, 0);
                srcNum = addBit(srcNum, startBitIndex, bitSize, value);
            }
            Buffer.BlockCopy(buffCopy, 0, buff, byteIndex, byteSize);
        }
        #endregion

        #region add methods

        public static int BytesToInt(params byte[] args){
            int num = 0;
            for (int i = 0; i < args.Length; i++)
            {
                num <<= 8;
                num |= args[i];
            }
            return num;
        }
        public static int ShortsToInt(params short[] args)
        {
            int num = 0;
            for (int i = 0; i < args.Length; i++)
            {
                num <<= 16;
                num |= (ushort)args[i];
            }
            return num;
        }
        public static long BytesToLong(params byte[] args)
        {
            long num = 0;
            for (int i = 0; i < args.Length; i++)
            {
                num <<= 8;
                num |= args[i];
            }
            return num;
        }
        public static long ShortsToLong(params short[] args)
        {
            long num = 0;
            for (int i = 0; i < args.Length; i++)
            {
                num <<= 16;
                num |= (ushort)args[i];
            }
            return num;
        }
        public static long IntsToLong(params int[] args)
        {
            long num = 0;
            for (int i = 0; i < args.Length; i++)
            {
                num <<= 32;
                num |= (uint)args[i];
            }
            return num;
        }

        #endregion

#region ICloneable 구현

        /*
        public virtual void ClonedBy(Object cloneBase){
            try{//캐스팅 가능시 하위개체..
                NetworkObjectBase obj = cloneBase as NetworkObjectBase;
                copyBufferFromArray(obj.ArrayBuffer);
                _isDataStruct = obj._isDataStruct;
            }catch(Exception e){
                throw new Exception("NetworkObjectBase:: 내부버퍼(ArrayBuffer) copy시 오류발생"+e);
            }
        }
         */
        //Byte[] cloneBuffer = new Byte[10000];
        public virtual Object Clone()
        {
            return SerializeCloner.MemClone(this);
            /*
            FileStream fs = File.Create("TempObj");

            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, this);
            fs.Close();
            fs = File.OpenRead("TempObj");
            INetPacket obj = bf.Deserialize(fs) as INetPacket;
            //INetPacket obj = (this as IClonable).Clone(typeof(ResizableNetworkObject<T>)) as INetPacket;
            fs.Close();
            return obj;
             */
        }
        /*
        public virtual Object Clone(){
            if(this.GetType() == clonedItemClassType){
                throw new Exception("추상클래스의 인스턴스를 만들 수 없슴.");
            }else{
                try{
                    NetworkObjectBase obj =  Activator.CreateInstance(clonedItemClassType) as NetworkObjectBase;
                    obj.copyBufferFromArray(this.ArrayBuffer);
                    obj._isDataStruct = this._isDataStruct;
                    return obj as IClonable;
                }catch{
                    throw new Exception("타입 "+clonedItemClassType.ToString()+ "클래스가 NetworkObjectBase와 연관이 없음");
                }
            }
        }
        */
#endregion

        #region set functions *******************************************

        #endregion

        public NetworkObjectBase() {
            AllPacket++;
        }

        protected virtual void Dispose()
        {
            
        }

        ~NetworkObjectBase()
        {
            Dispose();
            AllPacket--;
        }

        void IDisposable.Dispose()
        {
            Dispose();
        }
    }
}
