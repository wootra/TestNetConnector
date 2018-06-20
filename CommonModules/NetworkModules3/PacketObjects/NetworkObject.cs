using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using DataHandling;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
namespace NetworkModules3
{
    [Serializable]
    public class NetworkObject<T> : NetworkObjectBase, INetPacket, ICloneable
    {
        protected T[] _initBuffer;

        private Dictionary<int, int> _index = new Dictionary<int,int>();
        private List<Array> _children = new List<Array>();

        #region INetPacket implements
        public override Array ArrayBuffer { get { return _initBuffer; } set { _initBuffer = (T[])value; } }
        public virtual List<Array> Children { get { return _children; } set { _children = value; } }
        public virtual int ChildOffset { get { return -1; } }

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

        /// <summary>
        /// 버퍼를 포인터로 리턴한다. unsafe 코드..
        /// </summary>
        [Browsable(false)]
        public IntPtr buffPtr
        {
            get
            {
                return Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
            }
        }
        
        /// <summary>
        /// 버퍼가 string의 내용을 담고 있는데 기본버퍼의 타입이 byte가 아니라면 
        /// String은 swap이 되어 제대로된 값을 보여주지 못할 것이다.
        /// 이를 위해서 이 함수를 사용하여 swap이 이전에 되었으면 다시 스왑하여
        /// UTF-8형식으로 encoding하여 리턴한다.
        /// </summary>
        /// <param name="startIndex">소스 버퍼상에서의 가져올 String의 위치 </param>
        /// <param name="size">가져올 크기</param>
        /// <param name="isSwappedBefore">이전에 전체적으로 swap되었는지 체크</param>
        /// <returns>버퍼안에 있던 스트링(UTF-8)</returns>
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

        /// <summary>
        /// 두 패킷이 같은 것인지 비교. 정확히는 두 패킷이 가진 버퍼의 크기가 같은 것인지 비교한다.
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public override Boolean isSamePacket(NetworkObjectBase packet)
        {
            NetworkObject<T> obj;
            try
            {
                obj = packet as NetworkObject<T>;
            }
            catch
            {
                return false;
            }
            //null이라도 같으면 true.
            if (buffer == null && obj.buffer == null) return true;
            else if (buffer == null || obj.buffer == null) return false; //둘 중 하나가 null이면 false
            else if (buffer.Length != obj.buffer.Length) return false; //버퍼의 크기가 달라도 false
            for (int i = 0; i < buffer.Length; i++)
            {
                if (Buffer.GetByte(obj.buffer, i) != Buffer.GetByte(buffer, i)) return false;
            }
            
            return true;
        }

        /// <summary>
        /// 버퍼상의 위치를 포인터로 가져옴.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
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
                return Buffer.ByteLength(buffer);
            }
        }

        /// <summary>
        /// 기본 item T의 타입.
        /// </summary>
        /// <returns>T의 타입</returns>
        public Type getItemType()
        {
            return typeof(T);
        }

        /// <summary>
        /// byte타입의 버퍼로 바꾸어 버퍼의 내용을 복사하여 리턴함.
        /// </summary>
        /// <returns></returns>
        public override Byte[] getByteBufferCopied()
        {
            Byte[] buff = new Byte[bufferByteSize];
            Buffer.BlockCopy(buffer, 0, buff, 0, bufferByteSize);
            return buff;
        }

        /// <summary>
        /// 버퍼의 내용을 swapBaseSize로 swap하여 byte버퍼에 복사하여 리턴.
        /// </summary>
        /// <param name="swapBaseSize">swap할 때 이 크기를 하나의 타입으로 보고 swap한다.</param>
        /// <returns>값이 swap되어 복사된 byte버퍼</returns>
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

        #endregion

        #region set properties and methods *******************************************
        /// <summary>
        /// 처음
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected T getValue(int index) { return buffer[_index[index]]; }
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
            
            int di=dstOffset;
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

        /// <summary>
        /// 이 버퍼에 있는 내용을 dst배열에 복사해 준다.
        /// </summary>
        /// <param name="dst">내용이 복사될 배열</param>
        /// <param name="dstOffset">어디서부터 저장될지</param>
        /// <returns>복사된 크기가 리턴된다. 대상버퍼(dst)의 크기가 복사될 데이터크기보다 작으면 out of range exception이 리턴된다.</returns>
        public override int copyBufferToArray(Array dst, int dstOffset)
        {
            Buffer.BlockCopy(buffer, 0, dst, dstOffset, bufferByteSize);
            return bufferByteSize;
        }

        /// <summary>
        /// 이 버퍼에 있는 내용을 T타입으로 swap하여 dst배열에 복사해 준다.
        /// </summary>
        /// <param name="dst">내용이 복사될 배열</param>
        /// <param name="dstOffset">어디서부터 저장될지</param>
        /// <returns>복사된 크기가 리턴된다. 대상버퍼(dst)의 크기가 복사될 데이터크기보다 작으면 out of range exception이 리턴된다.</returns>
        public override int copyBufferToSwapArray(Array dst, int dstOffset)
        {
            int typeSize = Marshal.SizeOf(typeof(T));
            int di = dstOffset;
            int offset = 0;
            for (int i = 0; i < buffer.Length; i++)
            {
                for (int j = typeSize - 1; j >= 0; j--)
                {
                    Buffer.SetByte(dst, di + j, Buffer.GetByte(buffer, offset++));
                }
                di += typeSize;
            }
            return bufferByteSize;
        }

        /// <summary>
        /// 같은타입의NetworkObject형식의 buffer에 내부buffer의 내용을 복사해 준다.
        /// </summary>
        /// <param name="dst">자료가 복사될 NetworkObject인스턴스</param>
        public void CopyTo(NetworkObject<T> dst)
        {
            this.buffer.CopyTo(dst.buffer, 0);
        }

        /// <summary>
        /// 같은타입의NetworkObject형식의 buffer에 내부buffer의 내용을 numOfItems만큼만 복사해 준다. 
        /// </summary>
        /// <param name="dst">자료가 복사될 NetworkObject인스턴스</param>
        /// <param name="numOfItems">복사할 item들의 개수</param>
        /// <param name="offset">복사가 시작될 offset</param>
        public void CopyTo(NetworkObject<T> dst, int numOfItems, int offset = 0)
        {
            for (int i = offset; i < numOfItems; i++) dst.buffer[i] = this.buffer[i];
        }

        /// <summary>
        /// src의 buffer에서 자료를 복사해온다. 내부적으로는 src에서 CopyTo를 호출한다.
        /// </summary>
        /// <param name="src">복사해올 자료가 있는 NetworkObject인스턴스</param>
        public void CopyFrom(NetworkObject<T> src)
        {
            src.buffer.CopyTo(this.buffer, 0);
        }

        /// <summary>
        /// src의 buffer에서 자료를 복사해온다. 내부적으로는 src에서 CopyTo를 호출한다.
        /// </summary>
        /// <param name="src">복사해올 자료가 있는 NetworkObject인스턴스</param>
        /// <param name="numOfUnits">복사할 item들의 개수. T형식 자료의 개수이다.</param>
        /// <param name="offset">어느 index에서 시작할 것인지</param>
        public void CopyFrom(NetworkObject<T> src, int numOfUnits, int offset = 0)
        {
            src.CopyTo(this, numOfUnits, offset);
            //for (int i = 0; i < unitsInSrc; i++) this.buffer[i] = src.buffer[i];
        }

        /// <summary>
        /// 배열에서 자료를 복사해 온다.(타입형식을 무시하고 버퍼복사한다)
        /// </summary>
        /// <param name="src">복사할 자료가 있는 배열</param>
        /// <param name="byteSize">복사할 바이트크기. -1이면 이 NeworkObject의 끝까지 복사한다.</param>
        /// <param name="byteOffset">배열에서 복사를 시작할 위치. 기본값 0</param>
        /// <param name="dstByteOffset">이 NetworkObject의 버퍼에서 덮어씌울 위치의 시작점</param>
        public void CopyFrom(Array src, int byteSize=-1, int byteOffset=0, int dstByteOffset=0)
        {
            if (byteSize < 0) byteSize = bufferByteSize-dstByteOffset;
            Buffer.BlockCopy(src, byteOffset, this.buffer, dstByteOffset, byteSize);
        }

        /// <summary>
        /// 배열에서 T형식으로 swap하여 값을 가져온다.
        /// </summary>
        /// <param name="src">복사할 배열</param>
        /// <param name="byteSize">복사될 자료의 바이트 크기</param>
        /// <param name="srcByteOffset">c</param>
        /// <param name="dstByteOffset"></param>
        /// <returns>총 복사된 크기.</returns>
        public int CopyFromSwapped(Array src, int srcByteOffset = 0, int dstByteOffset = 0, int byteSize = -1)
        {
            if (byteSize < 0) byteSize = bufferByteSize;
            int typeSize = Marshal.SizeOf(typeof(T));
            int di = dstByteOffset;
            int srcOffset = srcByteOffset;
            for (int i = 0; i < buffer.Length; i++)
            {
                for (int j = typeSize - 1; j >= 0; j--)
                {
                    Buffer.SetByte(buffer, di + j, Buffer.GetByte(src, srcOffset++));
                }
                di += typeSize;
            }
            return bufferByteSize-dstByteOffset;
        }

        #endregion

        #region UnitsTo functions
        /// <summary>
        /// 여러개의 배열인자를 모아서 하나의 변수로 출력한다.
        /// 예를 들어 기본배열형식이 byte라면 int로 출력할 때 2개의 인자를 모으면 크기가 작은
        /// int형식의 값이 나온다. Little Endian형식으로 출력되므로, 0x01,0x02...형식으로 배열안에
        /// 값이 들어있을 때, 0,1,2,3 으로 입력하면, 출력값은 0x04030201 과 같이 된다.
        /// </summary>
        /// <typeparam name="U">출력될 변수의 타입</typeparam>
        /// <param name="unitNum">Unit의 index를 나열한다. 예> 0,1,2,3</param>
        /// <returns>인자들이 통합된 하나의 숫자.</returns>
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

        protected U UnitsToWithSize<U>(int startIndex, int size, int swapBase=-1)
        {
            int typeSize = (swapBase<0) ? Marshal.SizeOf(typeof(T)) : swapBase;
            U[] dVal = new U[1];

            for (int i = 0; i < size; i++)
            {
                Buffer.BlockCopy(buffer, startIndex+ i * typeSize, dVal, i * typeSize, typeSize);
            }

            return dVal[0];
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
        
        protected U UnitsSwapToWithSize<U>(int startIndex, int size)
        {
            int typeSize = Marshal.SizeOf(typeof(T));
            //U[] dVal = new U[1];
            return TypeArrayConverter.UnitSwapTo<U>(this.buffer, startIndex, false);
            /*
            <U>(this.buffer, startIndex * typeSize, true, typeSize);
            
            int unitsSize = size;
            

            if (unitsSize == 2)
            {
                return UnitsTo<U>(startIndex+1, startIndex);
            }
            else if (unitsSize == 4)
            {
                return UnitsTo<U>(startIndex+3, startIndex+2, startIndex+1, startIndex+0);
            }
            else if (unitsSize == 8)
            {
                return UnitsTo<U>(startIndex+7, startIndex+6, startIndex+5, startIndex+4, startIndex+3, startIndex+2, startIndex+1, startIndex+0);
            }
            else
            {
                throw new Exception("you need insert argument over 2.");
            }
             */
            /*
            for (int i = 0; i < unitNum.Length; i++)
            {
                Buffer.BlockCopy(buffer, unitNum[unitsSize-i] * typeSize, dVal, i * typeSize, typeSize);
            }

            return dVal[0];*/
           
        }
        #endregion
        // constructor
        public NetworkObject(int bufferSize)
        {
            if (bufferSize == 0)
            {
            }
            else
            {
                _initBuffer = new T[bufferSize];
                ArrayBuffer = _initBuffer;
                _index = new Dictionary<int, int>(bufferSize);
                setUnits(0, bufferSize);
            }
        }
        protected override void Dispose()
        {
            _index.Clear();
            _children.Clear();
        }
        /*
        public virtual void ClonedBy(IClonable cloneBase)
        {
            base.ClonedBy(cloneBase);
            try
            {//캐스팅 가능시 하위개체..
                NetworkObject<T> obj = cloneBase as NetworkObject<T>;
                this._index = new Dictionary<int, int>(obj._index);
                this._children = new List<Array>(obj._children);
            }
            catch (Exception e)
            {
                throw new Exception("NetworkObjectBase:: 내부버퍼(ArrayBuffer) copy시 오류발생" + e);
            }
        }
        */
        
    }
}
