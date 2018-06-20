using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace DataHandling
{

    /* 요약:
     * NeUsable은 Network프로그래밍을 하면서 자주 쓰일 함수들을 미리 정의해 놓은 것이다.
     * 주로 Big Endian방식과 Small Endian방식을 바꾸어주는 Swap 기능들과
     * 서로 다른 종류의 타입과 버퍼를 복사하는 기능들로 이루어져 있다.
     * 또한 패킷을 쉽게 만들 수 있는 API를 제공한다.
    */
    public class NetUsableAPI
    {
        #region Array to Array;
        /* 요약:
         * source 배열을 기준크기(typeSize)로 Swap된 dest 배열로 복사합니다.
         * 기준크기는 int32형(4byte)입니다.
         * offset은 시작할 배열의 번호입니다. 예를 들어, source[2]번부터 시작하려면 offset을 2로 합니다.
         * 단, 복사가 되는 size인 ByteSize는 byte로 하거나 비워두십시오.
         */
        
        public static void ArrayToSwappedOtherArray<U,T>(U[] source, Array dest, int typeSize=4, int srcOffset = 0, int dstOffset = 0, int byteSize = -1)
        {
            byteSize = (byteSize >= 0) ? byteSize : (Buffer.ByteLength(source) > Buffer.ByteLength(dest)) ? Buffer.ByteLength(dest) : Buffer.ByteLength(source);

            int srcSize = byteSize / typeSize;
            
            int sPt = srcOffset * Marshal.SizeOf(typeof(U));
            int dPt = dstOffset *Marshal.SizeOf(typeof(T));

            for (int i = 0; i < srcSize; i++)
            {
                for (int j = typeSize - 1; j >= 0; j--)
                {
                    Buffer.SetByte(dest,dPt++,Buffer.GetByte(source,sPt+j));
                }
                sPt += typeSize;
            }
        }
         
        public static T[] getTArrayFromArraySwapByUFrom<U,T>(Array source, int byteSize = -1, int offset = 0)
        {
            if (byteSize < 0) byteSize = Buffer.ByteLength(source);
            int typeSize = Marshal.SizeOf(typeof(U));
            int numOfUnits = byteSize / typeSize;
            offset *= (offset==0)? 0: Marshal.SizeOf(source.GetValue(0));
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
        
        #endregion

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
