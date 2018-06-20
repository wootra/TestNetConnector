using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace DataHandling
{
    public static class TypeArrayConverter
    {
        /// <summary>
        /// 버퍼에 T형식의 변수의 값을 채워넣음.
        /// </summary>
        /// <typeparam name="T">형식</typeparam>
        /// <param name="src">버퍼에 넣을 값을 가진 변수</param>
        /// <param name="buffer">버퍼</param>
        /// <param name="buffIndex">버퍼위치</param>
        public static void FillBufferUnitsFrom<T>(T src, Array buffer, params int[] buffIndex)
        {

            T[] val = new T[1];
            val[0] = src;
            int typeSize = Marshal.SizeOf(buffer.GetValue(0));

            for (int i = 0; i < buffIndex.Length; i++)
            {
                Buffer.BlockCopy(val, i * typeSize, buffer, buffIndex[i] * typeSize, typeSize);
            }
        }

        public static void setData(object value, Array buff, int offset, bool isSwap) //variable이 아닌 경우 값 setting..
        {
            
            if (buff != null)
            {
                if (buff.GetValue(0) is int) TypeArrayConverter.FillBufferUnitsFrom<int>(Convert.ToInt32(value), buff, offset, isSwap);
                else if (buff.GetValue(0) is byte) TypeArrayConverter.FillBufferUnitsFrom<byte>(Convert.ToByte(value), buff, offset, isSwap);
                else if (buff.GetValue(0) is short) TypeArrayConverter.FillBufferUnitsFrom<short>(Convert.ToInt16(value), buff, offset, isSwap);
                else if (buff.GetValue(0) is long) TypeArrayConverter.FillBufferUnitsFrom<long>(Convert.ToInt64(value), buff, offset, isSwap);
                else if (buff.GetValue(0) is uint) TypeArrayConverter.FillBufferUnitsFrom<uint>(Convert.ToUInt32(value), buff, offset, isSwap);
                else if (buff.GetValue(0) is sbyte) TypeArrayConverter.FillBufferUnitsFrom<sbyte>(Convert.ToSByte(value), buff, offset, isSwap);
                else if (buff.GetValue(0) is ulong) TypeArrayConverter.FillBufferUnitsFrom<ulong>(Convert.ToUInt64(value), buff, offset, isSwap);
                else if (buff.GetValue(0) is ushort) TypeArrayConverter.FillBufferUnitsFrom<ushort>(Convert.ToUInt16(value), buff, offset, isSwap);
                else if (buff.GetValue(0) is float) TypeArrayConverter.FillBufferUnitsFrom<float>(Convert.ToSingle(value), buff, offset, isSwap);
                else if (buff.GetValue(0) is double) TypeArrayConverter.FillBufferUnitsFrom<double>(Convert.ToDouble(value), buff, offset, isSwap);
                else TypeArrayConverter.FillBufferUnitsFrom<int>(Convert.ToInt32(value), buff, offset, isSwap);
            }
        }

        public static void setData<T>(object value, Array buff, int offset, bool isSwap) //variable이 아닌 경우 값 setting..
        {

            if (buff != null)
            {
                if (typeof(T) == typeof(int)) TypeArrayConverter.FillBufferUnitsFrom<int>(Convert.ToInt32(value), buff, offset, isSwap);
                else if (typeof(T) == typeof(byte)) TypeArrayConverter.FillBufferUnitsFrom<byte>(Convert.ToByte(value), buff, offset, isSwap);
                else if (typeof(T) == typeof(short)) TypeArrayConverter.FillBufferUnitsFrom<short>(Convert.ToInt16(value), buff, offset, isSwap);
                else if (typeof(T) == typeof(long)) TypeArrayConverter.FillBufferUnitsFrom<long>(Convert.ToInt64(value), buff, offset, isSwap);
                else if (typeof(T) == typeof(uint)) TypeArrayConverter.FillBufferUnitsFrom<uint>(Convert.ToUInt32(value), buff, offset, isSwap);
                else if (typeof(T) == typeof(sbyte)) TypeArrayConverter.FillBufferUnitsFrom<sbyte>(Convert.ToSByte(value), buff, offset, isSwap);
                else if (typeof(T) == typeof(ulong)) TypeArrayConverter.FillBufferUnitsFrom<ulong>(Convert.ToUInt64(value), buff, offset, isSwap);
                else if (typeof(T) == typeof(ushort)) TypeArrayConverter.FillBufferUnitsFrom<ushort>(Convert.ToUInt16(value), buff, offset, isSwap);
                else if (typeof(T) == typeof(float)) TypeArrayConverter.FillBufferUnitsFrom<float>(Convert.ToSingle(value), buff, offset, isSwap);
                else if (typeof(T) == typeof(double)) TypeArrayConverter.FillBufferUnitsFrom<double>(Convert.ToDouble(value), buff, offset, isSwap);
                else TypeArrayConverter.FillBufferUnitsFrom<int>(Convert.ToInt32(value), buff, offset, isSwap);
            }
        }

        public static void setData(Type t, object value, Array buff, int offset, bool isSwap) //variable이 아닌 경우 값 setting..
        {

            if (buff != null)
            {
                if (t == typeof(int)) TypeArrayConverter.FillBufferUnitsFrom<int>(Convert.ToInt32(value), buff, offset, isSwap);
                else if (t == typeof(byte)) TypeArrayConverter.FillBufferUnitsFrom<byte>(Convert.ToByte(value), buff, offset, isSwap);
                else if (t == typeof(short)) TypeArrayConverter.FillBufferUnitsFrom<short>(Convert.ToInt16(value), buff, offset, isSwap);
                else if (t == typeof(long)) TypeArrayConverter.FillBufferUnitsFrom<long>(Convert.ToInt64(value), buff, offset, isSwap);
                else if (t == typeof(uint)) TypeArrayConverter.FillBufferUnitsFrom<uint>(Convert.ToUInt32(value), buff, offset, isSwap);
                else if (t == typeof(sbyte)) TypeArrayConverter.FillBufferUnitsFrom<sbyte>(Convert.ToSByte(value), buff, offset, isSwap);
                else if (t == typeof(ulong)) TypeArrayConverter.FillBufferUnitsFrom<ulong>(Convert.ToUInt64(value), buff, offset, isSwap);
                else if (t == typeof(ushort)) TypeArrayConverter.FillBufferUnitsFrom<ushort>(Convert.ToUInt16(value), buff, offset, isSwap);
                else if (t == typeof(float)) TypeArrayConverter.FillBufferUnitsFrom<float>(Convert.ToSingle(value), buff, offset, isSwap);
                else if (t == typeof(double)) TypeArrayConverter.FillBufferUnitsFrom<double>(Convert.ToDouble(value), buff, offset, isSwap);
                else TypeArrayConverter.FillBufferUnitsFrom<int>(Convert.ToInt32(value), buff, offset, isSwap);
            }
        }

        public static void setData(Type t, object value, Array buff, int offset, double rate, bool isSwap) //variable이 아닌 경우 값 setting..
        {

            if (buff != null)
            {
                if (t == typeof(int)) TypeArrayConverter.FillBufferUnitsFrom<int>((int)(Convert.ToInt32(value)* rate), buff, offset, isSwap);
                else if (t == typeof(byte)) TypeArrayConverter.FillBufferUnitsFrom<byte>((byte)(Convert.ToByte(value) * rate), buff, offset, isSwap);
                else if (t == typeof(short)) TypeArrayConverter.FillBufferUnitsFrom<short>((short)(Convert.ToInt16(value) * rate), buff, offset, isSwap);
                else if (t == typeof(long)) TypeArrayConverter.FillBufferUnitsFrom<long>((long)(Convert.ToInt64(value) * rate), buff, offset, isSwap);
                else if (t == typeof(uint)) TypeArrayConverter.FillBufferUnitsFrom<uint>((uint)(Convert.ToUInt32(value) * rate), buff, offset, isSwap);
                else if (t == typeof(sbyte)) TypeArrayConverter.FillBufferUnitsFrom<sbyte>((sbyte)(Convert.ToSByte(value) * rate), buff, offset, isSwap);
                else if (t == typeof(ulong)) TypeArrayConverter.FillBufferUnitsFrom<ulong>((ulong)(Convert.ToUInt64(value) * rate), buff, offset, isSwap);
                else if (t == typeof(ushort)) TypeArrayConverter.FillBufferUnitsFrom<ushort>((ushort)(Convert.ToUInt16(value) * rate), buff, offset, isSwap);
                else if (t == typeof(float)) TypeArrayConverter.FillBufferUnitsFrom<float>((float)(Convert.ToSingle(value) * rate), buff, offset, isSwap);
                else if (t == typeof(double)) TypeArrayConverter.FillBufferUnitsFrom<double>((double)(Convert.ToDouble(value) * rate), buff, offset, isSwap);
                else TypeArrayConverter.FillBufferUnitsFrom<int>((int)(Convert.ToInt32(value) * rate), buff, offset, isSwap);
            }
        }



        /// <summary>
        /// 버퍼에 T형식의 변수의 값을 채워넣음.
        /// </summary>
        /// <typeparam name="T">형식</typeparam>
        /// <param name="src">버퍼에 넣을 값을 가진 변수</param>
        /// <param name="buffer">버퍼</param>
        /// <param name="buffIndex">버퍼의 byte위치</param>
        public static void FillBufferUnitsFrom<T>(T src, Array buffer, int startIndex, bool isSwap=false)
        {

            T[] val = new T[1];
            val[0] = src;
            int valueTypeSize = Marshal.SizeOf(val[0]);
            int buffTypeSize = Marshal.SizeOf(buffer.GetValue(0));

            if (isSwap)
            {
                Swaper.swapWithSize(val, buffer, valueTypeSize, valueTypeSize, 0, startIndex);
            }
            else
            {
                if (valueTypeSize < buffTypeSize)
                {
                    Buffer.BlockCopy(val, 0, buffer, startIndex, valueTypeSize);
                }
                else
                {
                    Buffer.BlockCopy(val, 0, buffer, startIndex, buffTypeSize);
                    /*
                    int numOfIndex = buffTypeSize / valueTypeSize;
                    for (int i = 0; i < numOfIndex; i++)
                    {

                        Buffer.BlockCopy(val, i * buffTypeSize, buffer, startIndex + i * buffTypeSize, buffTypeSize);
                    }
                     */
                }
            }
        }
        /// <summary>
        /// T형식의 변수에 있는 내용을 trimSize만큼 잘라서 buffer의 각 항목에 집어넣는다.
        /// 버퍼의 타입이 trimSize보다 크다면 T형식에서 trimSize만큼 잘라서 집어넣고, 타입이 그보다 작다면
        /// trimSize만큼 잘라서 버퍼의 각 항목에 맞게 넣는다. 이 경우 데이터가 잘린다.
        /// </summary>
        /// <typeparam name="T">값을 채워넣을 변수의 형식</typeparam>
        /// <param name="src">값을 채워넣을 변수(source)</param>
        /// <param name="trimSize">버퍼에 값을 채워넣을 크기. 변수 안에 이 크기보다 많은 내용이 있다면 잘라서 버퍼에 채움</param>
        /// <param name="buffer">값을 채워넣을 버퍼</param>
        /// <param name="buffIndex">값을 채워넣을 버퍼상의 위치</param>
        public static void FillBufferUnitsFromTrimed<T>(T src, int trimSize, Array buffer, params int[] buffIndex)
        {
            T[] lng = new T[1];
            lng[0] = src;
            int srcTypeSize = Marshal.SizeOf(lng[0]);
            int buffTypeSize = Marshal.SizeOf(buffer.GetValue(0));
            int maxCount = srcTypeSize / trimSize;
            int sizeInBuffer = buffIndex.Length;
            if (sizeInBuffer > maxCount) sizeInBuffer = maxCount;

            if (trimSize > buffTypeSize)
            {
                for (int i = 0; i < sizeInBuffer; i++)
                {
                    Buffer.BlockCopy(lng, i * trimSize, buffer, buffIndex[i] * buffTypeSize, buffTypeSize);
                }
            }
            else
            {
                for (int i = 0; i < sizeInBuffer; i++)
                {
                    Buffer.BlockCopy(lng, i * trimSize, buffer, buffIndex[i] * buffTypeSize, trimSize);
                }
            }
        }
        
        /// <summary>
        /// T형식의 변수에 있는 내용을 trimSize만큼 잘라서 buffer의 각 항목에 집어넣는다.
        /// 버퍼의 타입이 trimSize보다 크다면 T형식에서 trimSize만큼 잘라서 집어넣고, 타입이 그보다 작다면
        /// trimSize만큼 잘라서 버퍼의 각 항목에 맞게 넣는다. 이 경우 데이터가 잘린다.
        /// </summary>
        /// <typeparam name="T">값을 채워넣을 변수의 형식</typeparam>
        /// <param name="src">값을 채워넣을 변수(source)</param>
        /// <param name="trimSize">버퍼에 값을 채워넣을 크기. 변수 안에 이 크기보다 많은 내용이 있다면 잘라서 버퍼에 채움</param>
        /// <param name="buffer">값을 채워넣을 버퍼</param>
        /// <param name="buffStartIndex">값을 채워넣을 버퍼상의 위치</param>
        /// <param name="sizeInBuffer">버퍼의 기본크기T의 개수</param>
        public static void FillBufferUnitsFromTrimed<T>(T src, int trimSize, Array buffer, int buffStartIndex, int sizeInBuffer)
        {
            T[] lng = new T[1];
            lng[0] = src;
            int srcTypeSize = Marshal.SizeOf(lng[0]);
            int buffTypeSize = Marshal.SizeOf(buffer.GetValue(0));
            int maxCount = srcTypeSize / trimSize;
            if (sizeInBuffer > maxCount) sizeInBuffer = maxCount;

            if (trimSize > buffTypeSize)
            {
                for (int i = 0; i < sizeInBuffer; i++)
                {
                    Buffer.BlockCopy(lng, i * trimSize, buffer, buffStartIndex + buffTypeSize * i, buffTypeSize);
                }
            }
            else
            {
                for (int i = 0; i < sizeInBuffer; i++)
                {
                    Buffer.BlockCopy(lng, i * trimSize, buffer, buffStartIndex + buffTypeSize * i, trimSize);
                }
            }
        }
        
        /// <summary>
        /// 버퍼에서 T크기만큼의 값을 읽어와서 T형식의 변수에 넣어준다.
        /// </summary>
        /// <typeparam name="T">읽어올 값의 형식</typeparam>
        /// <param name="buffer">값을 읽어올 버퍼</param>
        /// <param name="unitIndex">읽어올 위치</param>
        /// <returns>T형식의 변수. 버퍼에서 T크기만큼 복사하여 리턴한다.</returns>
        public static T UnitTo<T>(Array buffer, params int[] unitIndex)
        {
            int typeSize = Marshal.SizeOf(buffer.GetValue(0));
            T[] val = new T[1];

            for (int i = 0; i < unitIndex.Length; i++)
            {
                Buffer.BlockCopy(buffer, unitIndex[i] * typeSize, val, i * typeSize, typeSize);
            }

            return val[0];

        }

        /// <summary>
        /// 버퍼에서 T크기만큼의 값을 읽어와서 T형식의 변수에 넣어준다.
        /// 만일 T의 타입크기가 버퍼의 타입크기보다 작다면 버퍼의 값은 잘려서 들어갈 것이다.
        /// </summary>
        /// <typeparam name="T">읽어올 값의 형식</typeparam>
        /// <param name="buffer">값을 읽어올 버퍼</param>
        /// <param name="unitIndex">읽어올 위치</param>
        /// <param name="isSwap">스왑할 것인지 물음</param>
        /// <param name="swapBaseSize">스왑할 단위를 물음. 기본값-1로 둘 경우 소스버퍼의 크기가 됨.</param>
        /// <returns>T형식의 변수. 버퍼에서 T크기만큼 복사하여 리턴한다.</returns>
        public static T UnitTo<T>(Array buffer, int startByteOffset, Boolean isSwap=false, int swapBaseSize=-1)
        {
            swapBaseSize = (swapBaseSize<0) ? Marshal.SizeOf(buffer.GetValue(0)) : swapBaseSize;

            T[] val = new T[1];
            int valueTypeSize = Marshal.SizeOf(val[0]);

            if (isSwap == false)
            {
                Buffer.BlockCopy(buffer, startByteOffset, val, 0, valueTypeSize);
            }
            else
            {
                if (swapBaseSize > valueTypeSize) //이경우는 버퍼의 타입사이즈가 더 커서 잘라와야 하는 경우이다.
                {//bigEndian기준으로는 앞에서 잘라오면 된다.
                    Buffer.BlockCopy(buffer, startByteOffset, val, 0, valueTypeSize);
                }
                else
                {
                    int num = valueTypeSize / swapBaseSize -1;
                    for (int i = 0; i <= num; i++)
                    {
                        Buffer.BlockCopy(buffer, startByteOffset + (num-i) * swapBaseSize, val, i * swapBaseSize, swapBaseSize);
                    }
                }
            }

            return val[0];

        }

        public static Boolean isMinus(Array buffer, int startByteOffset, int typeByteSize){
            Byte[] signByte = new Byte[1];
            Buffer.BlockCopy(buffer, typeByteSize-1, signByte, 0, 1);
            return (signByte[0] >> 7) > 0;
        }
        public static void CopyIntToArray(Int64 value, Array arr, int byteSize = -1, int dstOffset = 0)
        {
            if (byteSize < 0) byteSize = (Buffer.ByteLength(arr) < sizeof(Int64)) ? Buffer.ByteLength(arr) : sizeof(Int64);
            Int64[] val = new Int64[1];
            val[0] = value;
            Buffer.BlockCopy(val, 0, arr, dstOffset, byteSize);
        }
        public static void CopyDoubleToArray(Double value, Array arr, int byteSize = -1, int dstOffset = 0)
        {
            if (byteSize < 0) byteSize = (Buffer.ByteLength(arr) < sizeof(Int64)) ? Buffer.ByteLength(arr) : sizeof(Int64);
            if (byteSize == 4)
            {
                Single[] val = new Single[1];
                val[0] = (Single)value;
                Buffer.BlockCopy(val, 0, arr, dstOffset, byteSize);
            }
            else //8
            {
                Double[] val = new Double[1];
                val[0] = value;
                Buffer.BlockCopy(val, 0, arr, dstOffset, byteSize);
            }
        }

        /// <summary>
        /// 버퍼에 있는 내용을 그냥 value에 집어넣으면 버퍼의 할당크기와 변수의 바이트크기가 다를 경우
        /// signed와 unsigned를 잡을 수 없다.
        /// 따라서 부호를 인식해 주는 작업과 부호에 따라서 복사를 달리해야 한다.
        /// 이 메서드는 버퍼에 있는 내용을 U타입이 signed인지 판단한 이후에 알맞은 
        /// converting을 통해서 U타입에 넣어준다. float과 double도 이와 같은 과정을 통해
        /// 부호와 타입이 정해진다.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="buffer"></param>
        /// <param name="startByteOffset"></param>
        /// <param name="byteSizes"></param>
        /// <param name="isUnsigned"></param>
        /// <returns></returns>
        public static U CopyBufferToVariable<U>(Array buffer, int startByteOffset, int byteSizes = -1, Boolean isUnsigned = false, Boolean isSwap=false)
        {
            int typeSize = Marshal.SizeOf(typeof(U));
            byteSizes = (byteSizes<0) ? typeSize: byteSizes;

            U[] value = new U[1];

            if ((value[0] is SByte) || (value[0] is Int16) || (value[0] is Int32) || (value[0] is Int64))
            { //signed value
                if (isMinus(buffer, startByteOffset, byteSizes))
                    CopyIntToArray(-1, value, typeSize);
                else CopyIntToArray(0, value, typeSize); //부호를 맞추고

                if(isSwap)Swaper.swapWithSize(buffer, value, byteSizes, byteSizes, startByteOffset, 0);
                else Buffer.BlockCopy(buffer, startByteOffset, value, 0, byteSizes);
                
            }
            else if ((value[0] is Byte) || (value[0] is UInt16) || (value[0] is UInt32) || (value[0] is UInt64))
            {//unsigned values
                CopyIntToArray(0, value, typeSize);
                if (isSwap) Swaper.swapWithSize(buffer, value, byteSizes, byteSizes, startByteOffset, 0);
                else Buffer.BlockCopy(buffer, startByteOffset, value, 0, byteSizes);
            }
            else if ((value[0] is Single) || (value[0] is Double))
            {//floating point
                Single[] sing = new Single[1];
                Double[] doub = new Double[1];

                if (byteSizes == 4)
                {

                    if (typeSize == 8)
                    {
                        Buffer.BlockCopy(buffer, startByteOffset, sing, 0, byteSizes);
                        doub[0] = (Double)sing[0];
                        //버퍼의 내용은 single이고 가져갈 내용은 double이므로 일단 single형식에
                        //buffer copy를 하고, 캐스팅을 통해 값을 넣어준다.
                        
                        if (isSwap) Swaper.swapWithSize(doub, value, byteSizes, byteSizes, 0, 0);
                        else Buffer.BlockCopy(doub, 0, value, 0, typeSize);
                    }
                    else //4 - single: 가져갈 값과 버퍼의 내용 동일하므로 바로 복사해줌.
                    {
                        if (isSwap) Swaper.swapWithSize(buffer, value, typeSize, typeSize, startByteOffset, 0);
                        else Buffer.BlockCopy(buffer, startByteOffset, value, 0, typeSize);
                       // Buffer.BlockCopy(buffer, startByteOffset, value, 0, typeSize);
                    }

                }
                else //8
                {
                    if (typeSize == 4)
                    {
                        Buffer.BlockCopy(buffer, startByteOffset, doub, 0, byteSizes);
                        sing[0] = (Single)doub[0];
                        //버퍼의 내용은 double이고 가져갈 내용은 single이므로 일단 double형식에
                        //buffer copy를 하고, 캐스팅을 통해 값을 넣어준다.
                        //Buffer.BlockCopy(sing, 0, value, 0, typeSize);
                        if (isSwap) Swaper.swapWithSize(doub, value, typeSize, typeSize, 0, 0);
                        else Buffer.BlockCopy(sing, 0, value, 0, typeSize);
                    }
                    else //8 - double: 가져갈 값과 버퍼의 내용 동일하므로 바로 복사해줌.
                    {
                        if (isSwap) Swaper.swapWithSize(buffer, value, typeSize, typeSize, startByteOffset, 0);
                        else Buffer.BlockCopy(buffer, startByteOffset, value, 0, typeSize);
                        //Buffer.BlockCopy(buffer, startByteOffset, value, 0, typeSize);
                    }
                }
            }
            else//boolean
            {
                Int64[] a= new Int64[1];
                CopyIntToArray(0, a);
                Buffer.BlockCopy(buffer, startByteOffset, a, 0, byteSizes);
                if (a[0] != 0)
                {
                    CopyIntToArray(1, value);
                }
                else
                {
                    CopyIntToArray(0, value);
                }
            }
            return value[0];
            
        }

        /// <summary>
        /// buffer에서 일정 크기만큼 가져와서 T형식의 변수로 리턴한다.
        /// </summary>
        /// <typeparam name="T">리턴할 변수타입</typeparam>
        /// <param name="buffer">읽어올 버퍼</param>
        /// <param name="startByteOffset">버퍼상에서 byte offset</param>
        /// <param name="isSwap">Swap할것인지</param>
        /// <param name="swapBaseSize">Swap하는 기준크기. -1이면 T의 크기</param>
        /// <param name="size">버퍼에서 읽어올 크기. 이 크기만큼 읽어와서 Swap하여 T에 넣는다. -1이면 T의크기.</param>
        /// <returns></returns>
        public static T GetVariableSwapFromBuffer<T>(Array buffer, int startByteOffset, Boolean isSwap = false, int swapBaseSize = -1, int size = -1)
        {
            //swapBaseSize = (swapBaseSize < 0) ? Marshal.SizeOf(buffer.GetValue(0)) : swapBaseSize;
            swapBaseSize = (swapBaseSize < 0) ? Marshal.SizeOf(typeof(T)) : swapBaseSize;

            T[] val = new T[1];

            int valueTypeSize = Marshal.SizeOf(val[0]);

            if (size > 0)
            {
                if (valueTypeSize < size) throw new Exception("size of T type must be bigger than 'size'. 'size' is the size to read from buffer");
                else valueTypeSize = size;
            }

            if (swapBaseSize < valueTypeSize) throw new Exception("swapBaseSize must be bigger than size");

            if (isSwap == false)
            {//little Endian기준으로는 앞에서 잘라오면 된다.
                Buffer.BlockCopy(buffer, startByteOffset, val, 0, valueTypeSize);
            }
            else
            {
                Swaper.swapWithSize(buffer, val, swapBaseSize, size, startByteOffset, 0);
            }

            return val[0];

        }

        /// <summary>
        /// buffer에서 일정 크기만큼 가져와서 object 형식으로 가져온다.
        /// </summary>
        /// <typeparam name="T">리턴할 변수타입</typeparam>
        /// <param name="buffer">읽어올 버퍼</param>
        /// <param name="startByteOffset">버퍼상에서 byte offset</param>
        /// <param name="isSwap">Swap할것인지</param>
        /// <param name="swapBaseSize">Swap하는 기준크기. -1이면 T의 크기</param>
        /// <param name="size">버퍼에서 읽어올 크기. 이 크기만큼 읽어와서 Swap하여 T에 넣는다. -1이면 T의크기.</param>
        /// <returns></returns>
        public static object GetVariableSwapFromBuffer(Array buffer, Type type, int startByteOffset, Boolean isSwap = false, int swapBaseSize = -1, int size = -1)
        {
            //swapBaseSize = (swapBaseSize < 0) ? Marshal.SizeOf(buffer.GetValue(0)) : swapBaseSize;
            swapBaseSize = (swapBaseSize < 0) ? Marshal.SizeOf(type) : swapBaseSize;

            Array val = GetBufferFromType(type, 1);
            
                
            int valueTypeSize = Marshal.SizeOf(type);

            if (size > 0)
            {
                if (valueTypeSize < size) throw new Exception("size of T type must be bigger than 'size'. 'size' is the size to read from buffer");
                else valueTypeSize = size;
            }

            if (swapBaseSize < valueTypeSize) throw new Exception("swapBaseSize must be bigger than size");

            if (isSwap == false)
            {//little Endian기준으로는 앞에서 잘라오면 된다.
                Buffer.BlockCopy(buffer, startByteOffset, val, 0, valueTypeSize);
            }
            else
            {
                Swaper.swapWithSize(buffer, val, swapBaseSize, size, startByteOffset, 0);
            }

            return val.GetValue(0);

        }

        private static Array GetBufferFromType(Type type, int size)
        {
            Array arr;
            if (type == typeof(byte))
            {
                arr = new byte[size];
            }
            else if (type == typeof(sbyte))
            {
                arr = new sbyte[size];
            }
            else if (type == typeof(short))
            {
                arr = new short[size];
            }
            else if (type == typeof(ushort))
            {
                arr = new ushort[size];
            }
            else if (type == typeof(int))
            {
                arr = new int[size];
            }
            else if (type == typeof(uint))
            {
                arr = new uint[size];
            }
            else if (type == typeof(long))
            {
                arr = new long[size];
            }
            else if (type == typeof(ulong))
            {
                arr = new ulong[size];
            }
            else if (type == typeof(float))
            {
                arr = new float[size];
            }
            else if (type == typeof(double))
            {
                arr = new double[size];
            }
            else
            {
                throw new Exception("TypeArrayConverter.GetVariableSwapFromBuffer::type [" + type.ToString() + "] is not valid type for this function...");
            }
            return arr;
        }

        /// <summary>
        /// 버퍼에서 T크기만큼의 값을 읽어와서 T형식의 변수에 넣어준다.
        /// 만일 T의 타입크기가 버퍼의 타입크기보다 작다면 버퍼의 값은 잘려서 들어갈 것이다.
        /// 단, 각 unit들은 뒤바뀔 것이다. 예를 들어, swapBase가 2인데, T의 형식이 8이라고 하자.
        /// 이 경우, 0x00,0x01,0x02,0x03,0x04,0x05,...와 같이 순서대로 값이 들어가있는 버퍼에서,
        /// startByteOffset이 0이고, isSwap이 true이고, swapBase가 2, T의 형식이 int이라고 하면,
        /// 실제 리턴되는 값의 버퍼내용은 다음과 같다.
        /// 01000302. 하지만 이 시스템은 LittleEndian 시스템이므로 표시되는 숫자는
        /// 0x02030001이 될 것이다.
        /// </summary>
        /// <typeparam name="T">읽어올 값의 형식</typeparam>
        /// <param name="buffer">값을 읽어올 버퍼</param>
        /// <param name="unitIndex">읽어올 위치</param>
        /// <param name="isSwap">스왑할 것인지 물음</param>
        /// <param name="swapBaseSize">스왑할 단위를 물음. 기본값-1로 둘 경우 소스버퍼의 크기가 됨.</param>
        /// <returns>T형식의 변수. 버퍼에서 T크기만큼 복사하여 리턴한다.</returns>
        public static T UnitSwapTo<T>(Array buffer, int startByteOffset, Boolean isSwap = false, int swapBaseSize = -1)
        {
            //swapBaseSize = (swapBaseSize < 0) ? Marshal.SizeOf(buffer.GetValue(0)) : swapBaseSize;
            swapBaseSize = (swapBaseSize < 0) ? Marshal.SizeOf(buffer.GetValue(0)) : swapBaseSize;

            T[] val = new T[1];
            int valueTypeSize = Marshal.SizeOf(val[0]);

            if (isSwap == false)
            {
                Buffer.BlockCopy(buffer, startByteOffset, val, 0, valueTypeSize);
            }
            else
            {
                if (swapBaseSize > valueTypeSize) //이경우는 버퍼의 타입사이즈가 더 커서 잘라와야 하는 경우이다.
                {//bigEndian기준으로는 앞에서 잘라오면 된다.
                    Buffer.BlockCopy(buffer, startByteOffset, val, 0, valueTypeSize);
                }
                else
                {
                    int num = valueTypeSize / swapBaseSize - 1;
                    for (int i = 0; i <= num; i++)
                    {
                        Buffer.BlockCopy(buffer, startByteOffset + (num - i) * swapBaseSize, val, i * swapBaseSize, swapBaseSize);
                    }
                }
            }

            return val[0];

        }
/*
        /// <summary>
        ///  버퍼에서 하나의 타입을 가진 값으로 복사한다. 이때, 사이즈와 unsigned여부에 따라 내부적으로 해당타입을 만들어
        ///  복사하므로 signed와 unsigned가 고려된다.
        /// </summary>
        /// <param name="srcBuff">복사할 데이터를 가진 버퍼</param>
        /// <param name="startByteOffset">버퍼상의 byte위치</param>
        /// <param name="typeByteSize">복사될 타입의 바이트사이즈</param>
        /// <returns> 변환된 값을 Int64로 리턴함. 필요한 타입으로 캐스팅해 쓰면 된다.</returns>
        public static Double copyBuffToVariableFloat(Array srcBuff, int startByteOffset, int typeByteSize)
        {

            Double value;
            switch (typeByteSize)
            {
                case 4:
                    value = (Int64)TypeArrayConverter.UnitTo<Single>(srcBuff, startByteOffset);
                    return value;
                default://8
                    value = (Int64)TypeArrayConverter.UnitTo<Double>(srcBuff, startByteOffset);
                    return value;
            }

        }
 */

        /// <summary>
        /// 버퍼에서 trimSize만큼 잘라서 T형식의 값에 넣어준다. trimSize는 T의 크기보다 적을 수 있다.
        /// 값을 입력받는 변수는 trimSize만큼 이동하며 저장된다. 
        /// 값을 받아오는 배열에서는 자신의 타입크기만큼 이동하며 불러온다.
        /// 따라서 trimSize를 배열의 타입크기보다 크게 하면 값에는 공백이 들어가게 된다.
        /// 반대의 경우에는 배열의 각 항목에서 trimSize만큼 잘라서 출력변수에 입력된다.
        /// 이 경우, 배열의 내용에서 값의 손실이 발생할 수 있다.
        /// </summary>
        /// <typeparam name="T">받을 값의 형식</typeparam>
        /// <param name="buffer">값을 가져올 버퍼. 타입에 상관없이 byte단위로 끊어서 가져올수 있다.</param>
        /// <param name="trimSize">T형식의 변수에 채워 줄 크기</param>
        /// <param name="unitIndex">array에서 값을 가져올 index.byte가 아닌 index이다.</param>
        /// <returns>T형식의 변수. 버퍼에서 trimSize만큼만 복사하여 채워넣는다.</returns>
        public static T UnitTrimTo<T>(Array buffer, int trimSize,  params int[] unitIndex)
        {
            int buffTypeSize = Marshal.SizeOf(buffer.GetValue(0));
            T[] val = new T[1];
            
            int valueTypeSize = Marshal.SizeOf(val[0]);
            int sizeOfIndex = unitIndex.Length;
            if (valueTypeSize < trimSize * sizeOfIndex) sizeOfIndex = valueTypeSize / trimSize;

            if (trimSize > valueTypeSize)
            {
                for (int i = 0; i < sizeOfIndex; i++)
                {
                    Buffer.BlockCopy(buffer, unitIndex[i] * buffTypeSize, val, i * trimSize, valueTypeSize);
                }
            }
            else
            {
                for (int i = 0; i < sizeOfIndex; i++)
                {
                    Buffer.BlockCopy(buffer, unitIndex[i] * buffTypeSize, val, i * trimSize, trimSize);
                }
            }

            return val[0];

        }
        /// <summary>
        /// 버퍼에서 trimSize만큼 잘라서 T형식의 값에 넣어준다. trimSize는 T의 크기보다 적을 수 있다.
        /// 값을 입력받는 변수는 trimSize만큼 이동하며 저장된다. 
        /// 값을 받아오는 배열에서는 자신의 타입크기만큼 이동하며 불러온다.
        /// 따라서 trimSize를 배열의 타입크기보다 크게 하면 값에는 공백이 들어가게 된다.
        /// 반대의 경우에는 배열의 각 항목에서 trimSize만큼 잘라서 출력변수에 입력된다.
        /// 이 경우, 배열의 내용에서 값의 손실이 발생할 수 있다.
        /// </summary>
        /// <typeparam name="T">받을 값의 형식</typeparam>
        /// <param name="buffer">값을 가져올 버퍼. 타입에 상관없이 byte단위로 끊어서 가져올수 있다.</param>
        /// <param name="trimSize">T형식의 변수에 채워 줄 크기</param>
        /// <param name="unitIndex">array에서 값을 가져올 index.byte가 아닌 index이다.</param>
        /// <returns>T형식의 변수. 버퍼에서 trimSize만큼만 복사하여 채워넣는다.</returns>
        public static T UnitTrimTo<T>(Array buffer, int trimSize, int startIndex, int sizeOfIndex)
        {
            int buffTypeSize = Marshal.SizeOf(buffer.GetValue(0));
            T[] val = new T[1];

            int valueTypeSize = Marshal.SizeOf(val[0]);
            
            if (valueTypeSize < trimSize * sizeOfIndex) sizeOfIndex = valueTypeSize / trimSize;

            if (trimSize > valueTypeSize)
            {
                for (int i = 0; i < sizeOfIndex; i++)
                {
                    Buffer.BlockCopy(buffer, startIndex + i * buffTypeSize, val, i * trimSize, valueTypeSize);
                }
            }
            else
            {
                for (int i = 0; i < sizeOfIndex; i++)
                {
                    Buffer.BlockCopy(buffer, startIndex + i * buffTypeSize, val, i * trimSize, trimSize);
                }
            }

            return val[0];

        }

        public static T[] ConvertToArray<T>(string p, Type type, bool swap=true) where T : IConvertible, IComparable, IFormattable
        {
            TypeCode tCode = Type.GetTypeCode(type);
            int baseTSize = Marshal.SizeOf(type);
            int arrTSize = Marshal.SizeOf(typeof(T));
            T[] arr;
            if(arrTSize>=baseTSize) arr = new T[1];
            else arr = new T[baseTSize / arrTSize];
            
            try
            {
                Array temp;
                switch (tCode)
                {
                    case TypeCode.Byte:
                        temp = new byte[] { byte.Parse(p) };
                        break;
                    case TypeCode.Int16:
                        temp = new Int16[] { Int16.Parse(p) };
                        break;
                    case TypeCode.Int32:
                        temp = new Int32[] { Int32.Parse(p) };
                        break;
                    case TypeCode.Int64:
                        temp = new Int64[] { Int64.Parse(p) };
                        break;
                    case TypeCode.SByte:
                        temp = new SByte[] { SByte.Parse(p) };
                        break;
                    case TypeCode.UInt16:
                        temp = new UInt16[] { UInt16.Parse(p) };
                        break;
                    case TypeCode.UInt32:
                        temp = new UInt32[] { UInt32.Parse(p) };
                        break;
                    case TypeCode.UInt64:
                        temp = new UInt64[] { UInt64.Parse(p) };
                        break;
                    case TypeCode.Double:
                        temp = new Double[] { Double.Parse(p) };
                        break;
                    case TypeCode.Single:
                        temp = new Single[] { Single.Parse(p) };
                        break;
                    default:
                        throw new Exception("DataHandling.TypeArrayConverter.ConvertToArray() - only number can be handled...");
                }
                if (swap)
                {
                    Swaper.swapWithSize(temp, arr, arrTSize, arrTSize, 0, 0);
                    return arr;
                }
                else
                {
                    Buffer.BlockCopy(temp, 0, arr, 0, arrTSize);
                    return arr;
                }
                
            }
            catch (Exception ex)
            {
                throw new Exception("DataHandling.TypeArrayConverter.ConvertToArray() - \r\nvalue: "+p+"/ type:"+ type.ToString()+"\r\n"+ex.Message);
            }
        }
    }
}
