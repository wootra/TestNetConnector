using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using DataHandling;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace NetworkModules3
{
    [Serializable]
    public class ResizableNetworkObject<T> : NetworkObjectBase, INetPacket, ICloneable
    {
        protected T[] _initBuffer;

        private Dictionary<int, int> _index;
        protected List<Array> _children = new List<Array>();

        #region INetPacket implements
        [Browsable(false)]
        public override Array ArrayBuffer { get { return _initBuffer; } set { _initBuffer = (T[])value; } }

        [Browsable(false)]
        public virtual List<Array> Children { get { return _children; } set { _children = value; } }

        [Browsable(false)]
        public virtual int ChildOffset { get { return -1; } }
        
 
        #endregion

        #region IClonable implements
        /*
        public override void ClonedBy(Object cloneBase)
        {
            base.ClonedBy(cloneBase);
            try
            {//캐스팅 가능시 하위개체..
                ResizableNetworkObject<T> obj = cloneBase as ResizableNetworkObject<T>;
                this._index = new Dictionary<int, int>(obj._index);
                this._children = new List<Array>(obj._children);
                //_index = obj._index.Take(obj._index.Count) as Dictionary<int,int>;
            }
            catch (Exception e)
            {
                throw new Exception("NetworkObjectBase:: 내부버퍼(ArrayBuffer) copy시 오류발생" + e);
            }
        }
        */
        #endregion

        #region get properties and methods *******************************************

        /// <summary>
        /// NetworkObject에 종속.
        /// protected T[] _initBuffer를 리턴한다. _initBuffer는 T형식으로 만들어진 기본버퍼이다.
        /// </summary>
        [Browsable(false)]
        public virtual T[] buffer
        {
            get
            {
                return _initBuffer;
            }
        }

        [Browsable(false)]
        public IntPtr buffPtr
        {
            get
            {
                return Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
            }
        }
        
        public String getUnitsToString(int startIndex, int size, Boolean isSwappedBefore)
        {
            
                byte[] charbuff = new byte[size * Marshal.SizeOf(typeof(T))];
                if (isSwappedBefore)
                {

                    Swaper.swapWithSize(buffer, charbuff, Marshal.SizeOf(typeof(T)), size * Marshal.SizeOf(typeof(T)), startIndex, 0);
                }
                else
                {
                    Buffer.BlockCopy(buffer, startIndex, charbuff, 0, size * Marshal.SizeOf(typeof(T)));
                }
                return Encoding.UTF8.GetString(charbuff);
            
        }

        public override Boolean isSamePacket(NetworkObjectBase packet)
        {
            NetworkObject<T> obj;
            try
            {
                obj = (NetworkObject<T>)packet;
            }
            catch
            {
                return false;
            }
            if (buffer == null && obj.buffer == null) return true;
            else if (buffer == null || obj.buffer == null) return false;

            for (int i = 0; i < buffer.Length; i++)
            {
                
                if (Buffer.GetByte(obj.buffer,i)!=Buffer.GetByte(buffer,i)) return false;
            }
            return true;
        }
        public IntPtr getBuffPtr(int index)
        {
            return Marshal.UnsafeAddrOfPinnedArrayElement(buffer, index);
        }


        /// <summary>
        /// 버퍼의 T type unit의 개수입니다.
        /// </summary>
        [Browsable(false)]
        [Description("버퍼의 해당클래스의 기본 type unit의 개수입니다.")]
        public int bufferLength
        {
            get
            {
                return buffer.Length;
            }
        }

        /// <summary>
        /// 버퍼의 바이트 크기입니다.
        /// </summary>
        [Browsable(false)]
        [Description("버퍼의 Byte 크기입니다.")]
        public override int bufferByteSize
        {
            get
            {
                if (buffer == null) return 0;
                else return Buffer.ByteLength(buffer);
            }
        }
        public Type getType()
        {
            return typeof(T);
        }
        public override Byte[] getByteBufferCopied()
        {
            Byte[] buff = new Byte[bufferByteSize];
            Buffer.BlockCopy(buffer, 0, buff, 0, bufferByteSize);
            return buff;
        }
        public override Byte[] getByteBufferSwapCopied(int swapBaseSize = -1)
        {
            if (swapBaseSize < 0) swapBaseSize = Marshal.SizeOf(typeof(T));//this.buffer.GetValue(0));
            int typeSize = swapBaseSize ;
            byte[] dst = new byte[this.bufferByteSize];
            int bi = 0;
            int offset = 0;
            for (int i = 0; i < buffer.Length; i++)
            {
                for (int j = typeSize - 1; j >= 0; j--)
                {
                    Buffer.SetByte(dst, j+bi, Buffer.GetByte(buffer, offset++));
                }
                bi += typeSize;
            }
            return dst;
        }
        protected T getValue(int index) { return buffer[_index[index]]; }
        #endregion

        #region set properties and methods *******************************************
        protected void setAllBy(T unit, int startIndex=0)
        {
            for (int i = startIndex; i < buffer.Length; i++)
            {
                buffer[i] = unit;
            }
        }
        protected void setValue(int index, T value)
        {
            buffer[_index[index]] = value;
        }

        private void setUnits(int startIndex, int size=1)
        {
            for (int i = 0; i < size; i++)
            {
                _index.Add(startIndex+i, _index.Count);
            }
        }

        public override void copyBufferFromArray(Array srcBuffer, int offset=0, int size = -1)
        {
            if (size < 0)
            {
                if (Buffer.ByteLength(srcBuffer) > Buffer.ByteLength(this.buffer)) size = Buffer.ByteLength(this.buffer);
                else size = Buffer.ByteLength(srcBuffer);
            }
            Buffer.BlockCopy(srcBuffer, offset, this.buffer, 0, size);
        }

        public override void copyBufferSwapFromArray(Array srcBuffer, int srcOffset = 0, int size = -1, int swapSize=-1, int dstOffset=0)
        {
            if (size < 0)
            {
                if (Buffer.ByteLength(srcBuffer) > Buffer.ByteLength(this.buffer)) size = Buffer.ByteLength(this.buffer);
                else size = Buffer.ByteLength(srcBuffer);
            }
            
            int typeSize = swapSize;
            if (swapSize < 0) typeSize = Marshal.SizeOf(typeof(T));

            int di = dstOffset;
            int si = srcOffset;
            int i, j;
           
            for (i = 0; i < size / typeSize; i++)
            {
                for (j = typeSize - 1; j >= 0; j--)
                {
                    if (si > srcBuffer.Length - 1) Buffer.SetByte(buffer, di + j, 0);//모자랄 경우 0으로 채움.
                    else Buffer.SetByte(buffer, di + j, Buffer.GetByte(srcBuffer, si++));
                }
                di += typeSize;
            }
        }
        /// <summary>
        /// swapBase를 기준으로 Swap을 하지만 unswapBase를 최소유닛으로 Swap하여 버퍼에 복사한다.
        /// 예를 들어 swapBase가 8이고 unswapBase가 2이면, 2바이트를 최소유닛으로 생각하여 8바이트를
        /// Swap한다. 각각 바이트로 01,02,03,04,05,06,07,08 이 들어있는 버퍼라면 결과는
        /// 07,08,05,06,03,04,01,02 가 나오게 된다.
        /// </summary>
        /// <param name="srcBuffer">복사할 데이터가 들어있는 버퍼</param>
        /// <param name="offset">복사될 버퍼의 offset</param>
        /// <param name="size">복사할 크기</param>
        /// <param name="swapBase">swap할 단위. -1이면 initBuffer의 타입사이즈가 된다.</param>
        /// <param name="unSwapBase">swap할 때 swap되지 않을 최소단위. 기본값은 1로서 모두 swap된다.</param>
        /// <param name="srcOffset">소스의 offset. 기본값은 0이다.</param>
        public void copyBufferUnSwapBasedSwapFromArray(Array srcBuffer, int offset, int size,int swapBase=-1, int unSwapBase = 1, int srcOffset = 0)
        {
            if (swapBase < 0) swapBase = Marshal.SizeOf(typeof(T));

            Swaper.swapWithUnswapBase(srcBuffer, srcOffset, _initBuffer, offset, size, unSwapBase);
        }

        public void ToEachBufferFrom<U>(U value, params int[] buffIndex)
        {

            U[] val = new U[1];
            val[0] = value;
            int typeSize = Marshal.SizeOf(typeof(T));

            for (int i = 0; i < buffIndex.Length; i++)
            {
                Buffer.BlockCopy(val, i * typeSize, buffer, buffIndex[i] * typeSize, typeSize);
            }
        }

        public void ToEachBufferSwapFrom<U>(U value, params int[] buffIndex)
        {

            U[] val = new U[1];
            val[0] = value;
            int typeSize = Marshal.SizeOf(typeof(T));
            int buffSize = buffIndex.Length-1;
            for (int i = 0; i < buffIndex.Length; i++)
            {
                Buffer.BlockCopy(val, i * typeSize, buffer, buffIndex[buffSize-i] * typeSize, typeSize);
            }
        }

        #endregion

        #region copy methods ************************************************************
        /*
        public NetworkObject<T> Clone()
        {
            NetworkObject<T> clone = new NetworkObject<T>(this.bufferLength);
            this.buffer.CopyTo(clone.buffer, 0);
            return clone;
        }
        */

        public override int copyBufferToArray(Array dst, int dstOffset)
        {
            Buffer.BlockCopy(buffer, 0, dst, dstOffset, bufferByteSize);
            return bufferByteSize;
        }
        public override int copyBufferToSwapArray(Array dst, int dstOffset)
        {
            int typeSize = Marshal.SizeOf(typeof(T));
            int bi = dstOffset;
            int offset = 0;
            for (int i = 0; i < buffer.Length; i++)
            {
                for (int j = typeSize - 1; j >= 0; j--)
                {
                    Buffer.SetByte(dst, bi + j, Buffer.GetByte(buffer, offset++));
                }
                bi += typeSize;
            }
            return bufferByteSize;
        }

        public void CopyTo(NetworkObject<T> dst)
        {
            this.buffer.CopyTo(dst.buffer, 0);
        }

        public void CopyTo(NetworkObject<T> dst, int unitsInSrc)
        {
            for (int i = 0; i < unitsInSrc; i++) dst.buffer[i] = this.buffer[i];
        }

        public void CopyFrom(NetworkObject<T> src)
        {
            src.buffer.CopyTo(this.buffer, 0);
        }

        public void CopyFrom(NetworkObject<T> src, int unitsInSrc)
        {
            for (int i = 0; i < unitsInSrc; i++) this.buffer[i] = src.buffer[i];
        }

        #endregion

        #region UnitTo methods
        protected U UnitsTo<U>(params int[] unitNum)
        {
            int typeSize = Marshal.SizeOf(typeof(T));
            U[] dVal = new U[1];

            for (int i = 0; i < unitNum.Length; i++)
            {
                Buffer.BlockCopy(buffer, unitNum[i] * typeSize, dVal, i * typeSize, typeSize);
            }

            return dVal[0];
        }

        protected Int64 UnitsToWithSize(int startIndex, int size, int notSwapBase=-1, Boolean isSwap=false)
        {
            int typeSize = (notSwapBase<0) ? Marshal.SizeOf(typeof(T)): notSwapBase;
            Array dVal;
            switch (size)
            {
                case 1:
                    dVal = new byte[1];
                    break;
                case 2:
                     dVal = new short[1];
                     break;
                case 4:
                    dVal = new int[1];
                     break;
                default:
                    dVal = new Int64[1];
                     break;
            }
            //Int64[] dVal = new Int64[1];
            //dVal[0] = 0;//초기화
            int start = (isSwap)? size / typeSize-1:0;
            int end = (isSwap) ? 0 : size / typeSize-1;
            if (start < end)
            {
                for (int i = start; i <= end; i++)
                {
                    Buffer.BlockCopy(buffer, startIndex + i * typeSize, dVal, i * typeSize, typeSize);
                }
            }
            else
            {
                for (int i = start; i >= end; i--)
                {
                    Buffer.BlockCopy(buffer, startIndex + i * typeSize, dVal, i * typeSize, typeSize);
                }
            }
            switch (size)
            {
                case 1:
                    return (byte)dVal.GetValue(0);
                case 2:
                    return (short)dVal.GetValue(0);
                case 4:
                    return (int)dVal.GetValue(0);
                default:
                    return (Int64)dVal.GetValue(0);
            }
        }

        protected U UnitsSwapTo<U>(params int[] unitNum)
        {
            //int typeSize = Marshal.SizeOf(typeof(T));
            //U[] dVal = new U[1];
            int unitsSize = unitNum.Length;
            if (unitsSize == 2)
            {
                return UnitsTo<U>(unitNum[1], unitNum[0]);
            }else if( unitsSize == 4){
                return UnitsTo<U>(unitNum[3], unitNum[2], unitNum[1], unitNum[0]);
            }
            else if (unitsSize == 8)
            {
                return UnitsTo<U>(unitNum[7], unitNum[6], unitNum[5], unitNum[4], unitNum[3], unitNum[2], unitNum[1], unitNum[0]);
            }
            else
            {
                throw new Exception("you need insert argument over 2.");
            }
            /*
            for (int i = 0; i < unitNum.Length; i++)
            {
                Buffer.BlockCopy(buffer, unitNum[unitsSize-i] * typeSize, dVal, i * typeSize, typeSize);
            }

            return dVal[0];*/
        }

        protected Int64 UnitsSwapToWithSize(int startIndex, int size, int notSwapBase=-1)
        {
            //int typeSize = Marshal.SizeOf(typeof(T));
            //U[] dVal = new U[1];
            return UnitsToWithSize(startIndex, size, notSwapBase, true);

            /*
            for (int i = 0; i < unitNum.Length; i++)
            {
                Buffer.BlockCopy(buffer, unitNum[unitsSize-i] * typeSize, dVal, i * typeSize, typeSize);
            }

            return dVal[0];*/
        }
        #endregion

        // constructor
        public ResizableNetworkObject()
        {
        }
        public ResizableNetworkObject(int buffSize)
        {
            setBuffSize(buffSize);
        }
        public virtual void setBuffSize(int bufferSize)
        {
            if (bufferSize == 0)
            {
                _initBuffer = null;
                ArrayBuffer = null;
                _index = new Dictionary<int, int>();
                setUnits(0, 0);
            }
            else
            {
                _initBuffer = new T[bufferSize];
                ArrayBuffer = _initBuffer;
                _index = new Dictionary<int, int>(bufferSize);
                setUnits(0, bufferSize);
            }
        }

    }
}
