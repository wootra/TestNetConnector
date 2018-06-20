using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;


namespace DataHandling
{
    /* 요약:
     * NeUsable은 Network프로그래밍을 하면서 자주 쓰일 함수들을 미리 정의해 놓은 것이다.
     * 주로 Big Endian방식과 Small Endian방식을 바꾸어주는 Swap 기능들과
     * 서로 다른 종류의 타입과 버퍼를 복사하는 기능들로 이루어져 있다.
     * 또한 패킷을 쉽게 만들 수 있는 API를 제공한다.
    */
    public unsafe class NetUsable<T>
    {
        //private volatile static byte[] _tempBuff = new byte[4096];
        /*
    public static int SetPacketWithUintData(uint uCommand, uint uData, ref byte[] btBuf)
    {
    private static UInt32 _startCode = 0;
    private static UInt32 _sessionNo = 0;
    private static UInt32 _encode = 0;

        int uLen = 0;
        BitConverter.GetBytes(_startCode).CopyTo(btBuf, uLen);
        uLen += sizeof(uint);
        BitConverter.GetBytes(_sessionNo).CopyTo(btBuf, uLen);
        uLen += sizeof(uint);

        const uint dataLen = sizeof(uint) + sizeof(uint);
        BitConverter.GetBytes(dataLen).CopyTo(btBuf, uLen);
        uLen += sizeof(uint);
        BitConverter.GetBytes(uCommand).CopyTo(btBuf, uLen);
        uLen += sizeof(uint);
        BitConverter.GetBytes(uData).CopyTo(btBuf, uLen);
        uLen += sizeof(uint);
        BitConverter.GetBytes(_encode).CopyTo(btBuf, uLen);
        uLen += sizeof(uint);

        return uLen;
    }
        */

        #region Array to Array;
        public static void SwapArrayFromOtherArrayOf<U>(U[] source, Array dest, int typeSize = 4, int srcOffset = 0, int dstOffset = 0, int byteSize = -1)
        {
            byteSize = (byteSize >= 0) ? byteSize : (Buffer.ByteLength(source) > Buffer.ByteLength(dest)) ? Buffer.ByteLength(dest) : Buffer.ByteLength(source);

            int srcSize = byteSize / typeSize;

            int sPt = srcOffset * Marshal.SizeOf(typeof(U));
            int dPt = dstOffset * Marshal.SizeOf(typeof(T));

            for (int i = 0; i < srcSize; i++)
            {
                for (int j = typeSize - 1; j >= 0; j--)
                {
                    Buffer.SetByte(dest, dPt++, Buffer.GetByte(source, sPt + j));
                }
                sPt += typeSize;
            }
        }

        public static T[] getTArrayFromArraySwapBy<U>(Array source, int byteSize = -1, int offset = 0)
        {
            if (byteSize < 0) byteSize = Buffer.ByteLength(source);
            int typeSize = Marshal.SizeOf(typeof(U));
            int numOfUnits = byteSize / typeSize;
            offset *= (offset == 0) ? 0 : Marshal.SizeOf(source.GetValue(0));
            int destSize = byteSize / Marshal.SizeOf(typeof(T));
            T[] dest = new T[destSize];

            for (int i = 0; i < numOfUnits; i++)
            {
                for (int j = typeSize - 1; j >= 0; j--)
                {
                    Buffer.SetByte(dest, j + i * typeSize, Buffer.GetByte(source, offset++));
                }
            }
            return dest;
        }
        
        public static void arrayToOtherArraySwappedByT(Array source, int srcOffset, Array dest, int destOffset, int byteSize)
        {
            int typeSize = Marshal.SizeOf(typeof(T));
            arrayToOtherArraySwappedBySize(source, srcOffset, dest, destOffset, byteSize, typeSize);
        }
        public static void arrayToOtherArraySwappedBySize(Array source, int srcOffset, Array dest, int destOffset, int byteSize, int swapBaseSize)
        {
            int buffSize = (byteSize > Buffer.ByteLength(dest)) ? Buffer.ByteLength(dest) : byteSize;
            int swapOffset = swapBaseSize - 1;
            int buffBase = srcOffset;
            int swapPos;
            for (int i = 0; i < buffSize; i++)
            {
                swapPos = (swapOffset + swapOffset * i) % swapBaseSize + (i / swapBaseSize) * swapBaseSize;
                buffBase = srcOffset + swapPos;
                Buffer.SetByte(dest, i + destOffset, Buffer.GetByte(source, buffBase));
            }
        }

        public static T[] getTArrayFromArraySwapped(Array source, int swapBase=-1, int srcStartIndex = 0, int byteSize = -1)
        {
            if (byteSize < 0) byteSize = Buffer.ByteLength(source);
            int tSize = Marshal.SizeOf(typeof(T));
            int typeSize = (swapBase<0)? tSize : swapBase;
            
            int destSize = byteSize / tSize;
            int srcOffset = srcStartIndex * Marshal.SizeOf(source.GetValue(0));
            T[] dest = new T[destSize];
            arrayToOtherArraySwappedBySize(source, srcOffset, dest, 0, byteSize, typeSize);
            return dest;
        }

        public static T getTFromArray(Array source, int offset, int unitByteSize, Boolean isSwap=false)
        {
            T[] dest = new T[1];
            int tSize = Marshal.SizeOf(typeof(T));
            int swapIndex = unitByteSize - 1;
            int swapBase = swapIndex;
            int loopSize = (tSize>=unitByteSize)? unitByteSize :tSize;
            for (int i = 0; i < loopSize; i++)
            {
                if (isSwap == false) swapBase = i;
                else swapBase = swapBase % unitByteSize;
                Buffer.SetByte(dest, i, Buffer.GetByte(source, offset + swapBase));
                swapBase += unitByteSize-1;
            }
            return dest[0];
        }

        public static T[] getTUnitArrayFromArray(Array source, int offset, int unitByteSize, int countOfUnit, Boolean isSwap=false){
            int tSize = Marshal.SizeOf(typeof(T));
            int sourceTypeSize = Marshal.SizeOf(source.GetValue(0));
            T[] dest = new T[countOfUnit];
            for(int i=0; i<countOfUnit; i++){
                dest[i] = getTFromArray(source, offset, unitByteSize, isSwap);
            }
            return dest;
        }
 
        #endregion

 /*
        #region Object To Array

        public static byte[] objectToByteArr(object obj)
        {
            int rawSize = Marshal.SizeOf(obj);
            byte[] rawdata = new byte[rawSize];
            GCHandle handle = GCHandle.Alloc(rawdata, GCHandleType.Pinned);
            IntPtr buffer = handle.AddrOfPinnedObject();
            Marshal.StructureToPtr(obj, buffer, false);
            handle.Free();
            return rawdata;
        }

        public static int ObjectToByteArray(object anything, byte[] arr)
        {
            int rawsize = Marshal.SizeOf(anything);
            IntPtr buffer = Marshal.AllocHGlobal(rawsize);
            Marshal.StructureToPtr(anything, buffer, false);

            Marshal.Copy(buffer, arr, 0, rawsize);
            Marshal.FreeHGlobal(buffer);
            return rawsize;
        }

        public static int ObjectToArray(object anything, T[] arr, int offsetInByte)
        {
            int rawsize = Marshal.SizeOf(anything);
            
            IntPtr buffer = Marshal.AllocHGlobal(rawsize);
            Marshal.StructureToPtr(anything, buffer, false);
            byte* pt = (byte*)buffer.ToPointer();
            for (int i = 0; i < rawsize; i++)
            {
                Buffer.SetByte(arr,i+offsetInByte, *pt);
            }
            
            Marshal.FreeHGlobal(buffer);
            return rawsize;
        }

        public static int ObjectToSwappedArrayByTypeSize(object anything, T[] arr,int offsetInByte,int typeSize)
        {
            int rawsize = Marshal.SizeOf(anything);
            int arrSize = rawsize / typeSize;

            IntPtr buffer = Marshal.AllocHGlobal(rawsize);
            Marshal.StructureToPtr(anything, buffer, false);
            byte* pt = (byte*)buffer.ToPointer();
            for (int i = 0; i < arrSize; i++)
            {

                for (int j = typeSize-1; j >=0; j--)
                {
                    Buffer.SetByte(arr, offsetInByte+j+(i*typeSize), *pt);
                    pt++;
                }
            }

            //Marshal.Copy(buffer, arr, 0, rawsize);
            Marshal.FreeHGlobal(buffer);
            return rawsize;
        }

        public static int ObjectToIntArray(object anything, int[] arr)
        {

            int rawsize = Marshal.SizeOf(anything);
            int arrSize = rawsize / Marshal.SizeOf(typeof(T));

            IntPtr buffer = Marshal.AllocHGlobal(rawsize);
            Marshal.StructureToPtr(anything, buffer, false);

            int* ipt = (int*)buffer.ToPointer();

            for (int i = 0; i < arrSize; i++)
            {
                arr[i] = (int)(*ipt);
            }
            Marshal.Copy(buffer, arr, 0, rawsize);
            Marshal.FreeHGlobal(buffer);
            return rawsize;
        }

        #endregion

        #region Object To Object

        public static  object ObjectToOtherObject(object source, int srcSize, Type destObjType)
        {//need test
            int destObjSize = Marshal.SizeOf(destObjType);
            if (srcSize > destObjSize ) return null;

            byte[] outArr = new byte[destObjSize];


            IntPtr sBuffer = Marshal.AllocHGlobal(srcSize);
            Marshal.StructureToPtr(source, sBuffer, false) ;
            
            byte* sPt = (byte*)sBuffer.ToPointer();

            for (int i = 0; i < srcSize; i++)
            {
                    outArr[i] = *sPt;
                    sPt++;
            }

            GCHandle oHandle = GCHandle.Alloc(outArr, GCHandleType.Pinned);
            IntPtr oBuffer = oHandle.AddrOfPinnedObject();

            object retObj = Marshal.PtrToStructure(oBuffer, destObjType);
            oHandle.Free();
            Marshal.FreeHGlobal(sBuffer);
            return retObj;
        }


        #endregion 
        */

        public static Byte[] getPacketArrayFromArgs(params object[] args)
        {
            Byte[] ret;
            Type t;
            int totalSize = getTotalSize(args);
            ret = new Byte[totalSize];
            int offset = 0;

            for (int i = 0; i < args.Length; i++)
            {
                t = args[i].GetType();
                
                if ((t.Equals(typeof(Array))))
                {
                    int count = Buffer.ByteLength((Array)(args[i]));
                    Buffer.BlockCopy(((Array)(args[i])), 0, ret, offset, count);
                    offset+=count;
                }else{
                    int typeSize = Marshal.SizeOf(t);
                    Type childType = args[i].GetType();
                    if (childType == typeof(Int32)) Buffer.BlockCopy(BitConverter.GetBytes((Int32)args[i]), 0, ret, offset, typeSize);
                    else if (childType == typeof(Int64)) Buffer.BlockCopy(BitConverter.GetBytes((Int64)args[i]), 0, ret, offset, typeSize);
                    else if (childType == typeof(Int16)) Buffer.BlockCopy(BitConverter.GetBytes((Int16)args[i]), 0, ret, offset, typeSize);
                    else if (childType == typeof(Byte)) Buffer.BlockCopy(BitConverter.GetBytes((Byte)args[i]), 0, ret, offset, typeSize);
                    else if (childType == typeof(UInt32)) Buffer.BlockCopy(BitConverter.GetBytes((UInt32)args[i]), 0, ret, offset, typeSize);
                    else if (childType == typeof(UInt64)) Buffer.BlockCopy(BitConverter.GetBytes((UInt64)args[i]), 0, ret, offset, typeSize);
                    else if (childType == typeof(UInt16)) Buffer.BlockCopy(BitConverter.GetBytes((UInt16)args[i]), 0, ret, offset, typeSize);
                    else if (childType == typeof(Boolean)) Buffer.BlockCopy(BitConverter.GetBytes((Boolean)args[i]), 0, ret, offset, typeSize);
                    else if (childType == typeof(Char)) Buffer.BlockCopy(BitConverter.GetBytes((Char)args[i]), 0, ret, offset, typeSize);
                    else if (childType == typeof(float)) Buffer.BlockCopy(BitConverter.GetBytes((float)args[i]), 0, ret, offset, typeSize);

                    offset += typeSize;
                }
            }

            return ret;
        }
        private static int getTotalSize(object[] args){
            Type t;
            int totalSize = 0;
            for (int i = 0; i < args.Length; i++)
            {
                t = args[i].GetType();
                if ((t.Equals(typeof(Array))))
                {
                    int arrSize = Buffer.ByteLength((Array)(args[i]));
                    totalSize += arrSize;
                }
                else totalSize += Marshal.SizeOf(t);
            }
            return totalSize;
        }
    }
}
