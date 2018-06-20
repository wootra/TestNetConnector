using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace NetworkModules
{
    public static class TypeHandling
    {
        public enum TypeName
        {
            HexString, Integer, Char, CharArray, Float, OctString
        }
        public enum IntegerType
        {
            Int64, Int32, Int16, Byte
        }


        public static TypeName getTypeKind(string numberUnit)
        {
            Int64 num;
            if (numberUnit.IndexOf("0X") == 0 || numberUnit.IndexOf("0x") == 0)
            {
                for (int i = 2; i < numberUnit.Length; i++)
                {
                    if (numberUnit[i].CompareTo('0') >= 0 && numberUnit[i].CompareTo('9') <= 0) continue;
                    if (numberUnit[i].CompareTo('a') >= 0 && numberUnit[i].CompareTo('f') <= 0) continue;
                    if (numberUnit[i].CompareTo('A') >= 0 && numberUnit[i].CompareTo('F') <= 0) continue;

                    if (numberUnit.Length == 1) return TypeName.Char;
                    else return TypeName.CharArray;

                }
                return TypeName.HexString;
            }
            else if (numberUnit[0].Equals('0')) return TypeName.OctString;
            else if (numberUnit.IndexOf('.') >= 0) return TypeName.Float;
            else if (Int64.TryParse(numberUnit, out num))
            {
                return TypeName.Integer;
            }
            else
            {
                if (numberUnit.Length == 1) return TypeName.Char;
                else return TypeName.CharArray;
                
            }
        }

        public static IntegerType getIntType(string numStr)
        {
            Int64 num = Int64.Parse(numStr);
            if (num <= Byte.MaxValue) return IntegerType.Byte;
            else if (num <= Int16.MaxValue) return IntegerType.Int16;
            else if (num <= Int32.MaxValue) return IntegerType.Int32;
            else return IntegerType.Int64;
        }

        public static int getIntBytes(Int64 num)
        {
            if (num <= Byte.MaxValue) return 1;
            else if (num <= Int16.MaxValue) return 2;
            else if (num <= Int32.MaxValue) return 4;
            else return 8;
        }
 
        public static int parseNumberToByteArray(string num, Array dest, int offset, int parseTypeSize, Boolean isSwap=false)
        {
            if (num == null || num.Length==0) num = "0";
            //parseTypeSize 는 string을 숫자로 변환시킬 때, 기준이 될 타입의 크기를 정하는 것이다.
            //예를 들어, 1은 byte에도 들어갈 수 있는 숫자이지만, parseTypeSize를 4로 하면 int에 들어가게 된다.

            Byte[] buff = new Byte[8]; //숫자가 가질 수 있는 최대 크기.

            if (getTypeKind(num) == TypeName.HexString)
            {
                int size;
                if (isSwap)
                {
                    size = getHexNumber(num, buff, offset);
                    Buffer.BlockCopy(NetUsable<byte>.getTArrayFromArraySwapped(buff, parseTypeSize, 0, size), 0, dest, offset, size);
                }
                else
                {
                    size = getHexNumber(num, dest, offset);
                }
                return size;
            }
            else if (getTypeKind(num) == TypeName.OctString) return getOctNumber(num, dest, offset, parseTypeSize, isSwap);
            else if (getTypeKind(num) == TypeName.Float)
            {
                Buffer.BlockCopy(
                    NetUsable<byte>.getTArrayFromArraySwapped(BitConverter.GetBytes(float.Parse(num)), sizeof(float), 0, sizeof(float)), 
                    0, 
                    dest, 
                    offset, 
                    sizeof(float));
                return sizeof(float);
            }
            else if (getTypeKind(num) == TypeName.Integer) return getIntNumber(num, dest, offset, parseTypeSize, isSwap);
            else if (getTypeKind(num) == TypeName.Char)
            {
                Buffer.SetByte(dest, offset, (Byte)(num[0]));
                return 1;
            }
            else //CharArray. char array doesn't need swapping.
            {
                for (int i = 0; i < num.Length; i++)
                {
                    Buffer.SetByte(dest, offset++, (Byte)num[i]);
                }
                return num.Length;
            }
        }

        private static int getIntNumber(string num, Array dest, int offset, int baseSizeInByte, Boolean isSwap=false)
        {
            if (num == null || num.Length == 0) return 0;
            Int64 tempNum = Int64.Parse(num);

            baseSizeInByte = (getIntBytes(tempNum) > baseSizeInByte) ? getIntBytes(tempNum) : baseSizeInByte;
            Byte[] numArr;

            if (baseSizeInByte == 1) numArr = BitConverter.GetBytes((byte)tempNum);
            else if (baseSizeInByte == 2) numArr = BitConverter.GetBytes((short)tempNum);
            else if (baseSizeInByte == 4) numArr = BitConverter.GetBytes((int)tempNum);
            else numArr = BitConverter.GetBytes(tempNum);

            if (isSwap) NetUsable<byte>.arrayToOtherArraySwappedBySize(numArr, 0, dest, offset, baseSizeInByte, baseSizeInByte);
            else Buffer.BlockCopy(numArr, 0, dest, offset, baseSizeInByte);
            
            return baseSizeInByte;
        }

        public static int getHexNumber(string hexString, Array dest, int destOffset)
        {
            if (hexString == null || hexString.Length == 0) return 0;
            if (hexString.IndexOf("0X") == 0 || hexString.IndexOf("0x") == 0) hexString = hexString.Substring(2);
            
            Int64[] hexNumber = new Int64[1];
            hexNumber[0] = getHexNumber(hexString);
            int numberTypeSize = getIntBytes(hexNumber[0]);
            Buffer.BlockCopy(hexNumber, 0, dest, destOffset, numberTypeSize);
            return numberTypeSize;
        }

        public static Int64 getHexNumber(string hexString)
        {
            if (hexString == null || hexString.Length == 0) return 0;
            if (hexString.IndexOf("0X") == 0 || hexString.IndexOf("0x") == 0) hexString = hexString.Substring(2);

            int size = hexString.Length;
            Int64 temp = 0;

            hexString = hexString.ToUpper();
            for (int i = 0; i < size; i++)
            {
                temp <<= 4;

                if (hexString[i] - '0' < 10) temp += (hexString[i] - '0');
                else temp += (hexString[i] - 'A' + 10);
            }

            return temp;
        }

        public static Int64 getOctNumber(string octString)
        {
            if (octString == null || octString.Length == 0) return 0;
            int size = octString.Length;
            Int64 temp = 0;

            for (int i = 0; i < size; i++)
            {
                temp <<= 3;
                temp += (octString[i] - '0');
            }

            return temp;
        }
        public static int getOctNumber(string octString, Array dest, int offset, int baseSizeInByte = 4, Boolean isSwap=false)
        {
            if (octString == null || octString.Length == 0) return 0;

            if (octString[0].Equals('0')) octString = octString.Substring(1);
            Int64 octNum = getOctNumber(octString);
            int octSize = getIntBytes(octNum);
            if(octSize > baseSizeInByte) baseSizeInByte = octSize;

           if(isSwap){
                NetUsable<byte>.arrayToOtherArraySwappedBySize(
                    BitConverter.GetBytes(octNum), 0, dest, offset, baseSizeInByte, sizeof(Int64));
           }else{
               Buffer.BlockCopy(BitConverter.GetBytes(octNum), sizeof(Int64)-baseSizeInByte, dest, offset, baseSizeInByte);
           }

            return baseSizeInByte;
        }
        public static Boolean isValidType(string param, string type)
        {

            param = param.Trim();
            type = type.ToLower();
            UInt32 nodw;
            UInt16 now;
            Int64 noi64;
            Int32 noi;
            Int16 nos;
            Byte nob;
            double nod;
            float nof;

            switch (type)
            {
                case "nonumber":
                    return !findNumber(param);
                case "hex":
                case "hexstring":
                    if (getTypeKind(param) == TypeName.HexString)
                        return true;
                    else return false;
                case "hasnumber":
                    return findNumber(param);
                case "dword":

                    return UInt32.TryParse(param, out nodw);
                case "word":
                    
                    return UInt16.TryParse(param, out now);
                case "int64":
                case "8byte":
                case "8bytes":
                case "long long":
                    
                    return Int64.TryParse(param, out noi64);
                case "int32":
                case "int":
                case "4byte":
                case "4bytes":
                    
                    return Int32.TryParse(param, out noi);
                case "int16":
                case "short":
                case "2byte:":
                case "2bytes":
                    
                    return Int16.TryParse(param, out nos);
                case "int8":
                case "byte":
                case "char":
                case "1byte":
                    
                    return Byte.TryParse(param, out nob);
                case "double":
                    
                    return Double.TryParse(param, out nod);
                case "float":
                    
                    return float.TryParse(param, out nof);
                case "string":
                case "char*":
                case "char[]":
                case "char array":
                case "cstring":
                    return true;
                default:
                    return false;
            }

        }

        public static void printArr(Array arr, String name)
        {
            int arrLen = arr.Length;
            int arrTypeSize = Marshal.SizeOf(arr.GetValue(0));
            String format;
            Console.Write(name + ":");
            for (int i = 0; i < arrLen; i++)
            {
                format = "{0:X" + arrTypeSize * 2 + "},";
                Console.Write(format, arr.GetValue(i));
            }
            Console.WriteLine("");
        }

        public static int getTypeSize(string type)
        {

            type = type.Trim();
            type = type.ToLower();

            switch (type)
            {
                case "string":
                case "char*":
                case "char[]":
                case "char array":
                case "cstring":
                case "hasnumber":
                case "nonumber":
                    return -1;
                case "hex":
                case "hexstring":
                case "dword":
                case "int32":
                case "float":
                case "int":
                    return 4;
                case "word":
                case "int16":
                case "short":
                    return 2;
                case "int64":
                case "double":
                case "long long":
                    return 8;
                case "int8":
                case "byte":
                case "char":
                    return 1;
                default:
                    return -1;
            }

        }
        private static Boolean findNumber(String str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (Char.IsDigit(str, i)) return true;
            }
            return false;
        }
    }
}
