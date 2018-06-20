using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace DataHandling
{
    public static class Swaper
    {
        public static T swap<T>(T value)
        {//보통 타입 변수들을 swap한다.
            T[] src = new T[] { value };
            Byte temp;
            int typeSize = Marshal.SizeOf(typeof(T));
            for (int i = 0; i < typeSize/2; i++)
            {
                temp = Buffer.GetByte(src, i);
                Buffer.SetByte(src, i, Buffer.GetByte(src, typeSize - i-1));
                Buffer.SetByte(src, typeSize - i-1, temp);
            }
            return src[0];
        }

        public static void swap<T>(T[] src, int srcOffsetIndex=0, int size=-1)
        {//배열의 타입에 따라서 내부의 내용을 swap한다.
            Byte temp;
            int typeSize = Marshal.SizeOf(typeof(T));
            int srcSize = (size < 0) ? src.Length-srcOffsetIndex : size/typeSize;
            int offset;
            for (int arrIndex = 0; arrIndex < srcSize; arrIndex++)
            {
                offset = (arrIndex) * typeSize + srcOffsetIndex;
                for (int i = 0; i < typeSize / 2; i++)
                {
                    temp = Buffer.GetByte(src, i+offset);
                    Buffer.SetByte(src, i+offset, Buffer.GetByte(src, typeSize - i-1+offset));
                    Buffer.SetByte(src, typeSize - i-1+offset, temp);
                }
            }
        }

        public static void swap<T>(Array src, Array dst, int size = -1, int srcOffset = 0, int dstOffset = 0)
        {//src배열의 내용을 dst배열로 T타입을 기준으로 swap하여 복사한다.
            Byte temp;
            int typeSize = Marshal.SizeOf(typeof(T));
            int srcSize = (size < 0) ? (Buffer.ByteLength(src)-srcOffset)/typeSize : size/typeSize;
            int fromOffset, toOffset;
            for (int arrIndex = 0; arrIndex < srcSize; arrIndex++)
            {
                fromOffset = arrIndex * typeSize + srcOffset;
                toOffset = arrIndex * typeSize + dstOffset;

                for (int i = 0; i < typeSize / 2; i++)
                {
                    temp = Buffer.GetByte(src, i + fromOffset);
                    Buffer.SetByte(dst, i + toOffset, Buffer.GetByte(src, typeSize - i-1 + fromOffset));
                    Buffer.SetByte(dst, typeSize - i-1 + toOffset, temp);
                }
            }
        }
        /// <summary>
        /// swapBase를 기준으로 Swap을 하지만 unswapBase를 최소유닛으로 Swap하여 버퍼에 복사한다.
        /// 예를 들어 swapBase가 8이고 unswapBase가 2이면, 2바이트를 최소유닛으로 생각하여 8바이트를
        /// Swap한다. 각각 바이트로 01,02,03,04,05,06,07,08 이 들어있는 버퍼라면 결과는
        /// 07,08,05,06,03,04,01,02 가 나오게 된다.
        /// </summary>
        /// <param name="srcBuff">소스버퍼</param>
        /// <param name="srcStartOffset">소스버퍼가 시작될 offset(Byte)</param>
        /// <param name="dstBuff">복사될 버퍼</param>
        /// <param name="dstStartOffset">복사될 버퍼의 offset(Byte)</param>
        /// <param name="byteSize">복사할 크기(Byte)</param>
        /// <param name="unSwapBase">Swap되지 않는 최소단위</param>
        public static void swapWithUnswapBase(Array srcBuff, int srcStartOffset, Array dstBuff, int dstStartOffset, int byteSize, int unSwapBase)
        {
            Byte[] temp = new Byte[unSwapBase];
            int loopSize = byteSize / unSwapBase;
            int indexBack;
            int index;
            for (int i = 0; i < loopSize; i++)
            {
                index = i * unSwapBase;
                indexBack = ((loopSize - 1 - i) * unSwapBase);
                Buffer.BlockCopy(srcBuff, srcStartOffset + index, temp, 0, unSwapBase);
                Buffer.BlockCopy(srcBuff, srcStartOffset + indexBack, dstBuff, dstStartOffset + index, unSwapBase);
                Buffer.BlockCopy(temp, 0, dstBuff, dstStartOffset + indexBack, unSwapBase);
            }
        }

        /// <summary>
        /// 여기서 size는 bytesize이다.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        /// <param name="swapBaseSize"></param>
        /// <param name="size">number of bytes to copy</param>
        /// <param name="srcOffset"></param>
        /// <param name="dstOffset"></param>
        public static void swapWithSize(Array src, Array dst, int swapBaseSize, int size = -1, int srcOffset = 0, int dstOffset = 0)
        {
            /*
            if (swapBaseSize == 2) swap<Int16>(src, dst, size, srcOffset, dstOffset);
            else if (swapBaseSize == 4) swap<Int32>(src, dst, size, srcOffset, dstOffset);
            else if (swapBaseSize == 8) swap<Int64>(src, dst, size, srcOffset, dstOffset);
            else Buffer.BlockCopy(src, srcOffset, dst, dstOffset, size);
            */
            if (size == 0) return;//do nothing..
            
            if (size < 0)
            {
                int src_size = Buffer.ByteLength(src);
                int dst_size = Buffer.ByteLength(dst);
                size = (src_size < dst_size) ? src_size : dst_size; //둘 중 더 작은 크기를 size로 자동 할당..
            }
            
            
            if (swapBaseSize == 1)
            {
                try
                {
                    Buffer.BlockCopy(src, srcOffset, dst, dstOffset, size);
                }
                catch
                {
                    throw;
                }
                return;
            }
            int typeSize = swapBaseSize;
            int srcSize = (size < 0) ? (Buffer.ByteLength(src) - srcOffset) / typeSize : size / typeSize;
            int fromOffset, toOffset;
            Byte tempByte;
            for (int arrIndex = 0; arrIndex < srcSize; arrIndex++)
            {
                fromOffset = arrIndex * typeSize + srcOffset;
                toOffset = arrIndex * typeSize + dstOffset;

                for (int i = 0; i < typeSize / 2; i++)
                {
                    tempByte = Buffer.GetByte(src, fromOffset + typeSize-i-1);
                    try
                    {
                        Buffer.SetByte(dst, toOffset + typeSize - i - 1, Buffer.GetByte(src, fromOffset + i));
                        Buffer.SetByte(dst, toOffset + i, tempByte);
                    }
                    catch
                    {
                        throw;
                    }
                    /*
                    tempByte = Buffer.GetByte(src, i+fromOffset);//src와 dst가 같을 수도 있으므로 백업을 한다.
                    Buffer.SetByte(dst, i + toOffset, Buffer.GetByte(src, typeSize - i - 1 + fromOffset));
                    Buffer.SetByte(dst, typeSize - i - 1 + toOffset, tempByte);
                     */
                }
            }
        }

        public static T[] swapCopied<T>(Array src, int sourceOffset=0, int byteSize=-1)
        {//src의 내용을 T타입으로 swap된 T의 배열을 생성한다.
            Byte temp;
            int typeSize = Marshal.SizeOf(typeof(T));
            int srcSize = (byteSize < 0) ? (Buffer.ByteLength(src)-sourceOffset)/typeSize : byteSize/typeSize;
            T[] dst = new T[srcSize];
           
            int fromOffset;
            int toOffset;
            for (int arrIndex = 0; arrIndex < srcSize; arrIndex++)
            {
                fromOffset = arrIndex * typeSize + sourceOffset;
                toOffset = arrIndex * typeSize;
                for (int i = 0; i < typeSize / 2; i++)
                {
                    temp = Buffer.GetByte(src, i+fromOffset);
                    Buffer.SetByte(dst, i+toOffset, Buffer.GetByte(src, typeSize - i-1+fromOffset));
                    Buffer.SetByte(dst, typeSize - i-1+toOffset, temp);
                }
            }
            return dst;
        }

        public static T[] swapTrimed<T>(Array src, int sourceOffset=0, int size=-1)
        {
            //소스를 복사하여 swap된 새 T타입의 배열을 만들지만, 작은 크기의 타입에 맞추어
            //내용을 끊어서 채운다. 예를 들어, int배열을 short에 넣는다면, int의 모든
            //내용이 복사되는 것이 아니라 하위2바이트의 내용만이 들어간다. 마찬가지로
            //short배열을 int배열로 넣는다면, int배열의 상위 2비트는 비우게 될 것이다.

            Byte temp;
            int srcTypeSize = Marshal.SizeOf(src.GetValue(0));
            int dstTypeSize = Marshal.SizeOf(typeof(T));
            int srcSize = (size < 0) ? (Buffer.ByteLength(src)-sourceOffset)/srcTypeSize : size/srcTypeSize;
            int swapSize = (srcTypeSize > dstTypeSize) ? dstTypeSize : srcTypeSize; //작은 크기를 swap기준으로 삼는다.

            T[] dst = new T[srcSize];

            int fromOffset;
            int toOffset;
            for (int arrIndex = 0; arrIndex < srcSize; arrIndex++)
            {
                fromOffset = arrIndex * srcTypeSize+sourceOffset; //src는 scrtype의 크기만큼 이동한다.
                toOffset = arrIndex * dstTypeSize; //dst는 dst타입의 크기만큼 이동한다.
                //배열의 offset이동을 다르게 함으로서 상위비트는 무시하고 넘어간다.

                for (int i = 0; i < swapSize / 2; i++)
                {
                    temp = Buffer.GetByte(src, i+fromOffset);
                    Buffer.SetByte(dst, i+toOffset, Buffer.GetByte(src, swapSize - i-1+fromOffset));
                    Buffer.SetByte(dst, swapSize - i-1+toOffset, temp);
                }
            }
            return dst;

        }
    }
}
