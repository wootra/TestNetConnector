using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Globalization;


namespace DataHandling
{
    public static class TypeHandling
    {
        public enum TypeName{ HexString=0, Integer, Char, String, Float, OctString, Bin}

        public enum IntegerType
        {
            Int64, Int32, Int16, SByte, UInt64, UInt32, UInt16, Byte
        }
        public static Boolean isIntType(Type type)
        {
            return isSameTypeInList(type, typeof(Byte), typeof(short), typeof(int), typeof(Int64),typeof(ushort), typeof(uint), typeof(UInt64));
        }
        public static Boolean isFloatType(Type type)
        {
            return isSameTypeInList(type, typeof(Single), typeof(Double));
        }


        public static U TypeConvert<T, U>(T src)
        {
            T[] srcArr = new T[1];
            srcArr[0] = src;
            U[] dstArr = new U[1];
            int byteSize = (Marshal.SizeOf(typeof(T)) > Marshal.SizeOf(typeof(U)))? Marshal.SizeOf(typeof(U)) : Marshal.SizeOf(typeof(T));
            Buffer.BlockCopy(srcArr, 0, dstArr, 0, byteSize);
            return dstArr[0];
        }
        
        /// <summary>
        /// someType의 타입이 뒤에 열거된 타입 중 같은 타입이 있으면 true이다.
        /// </summary>
        /// <param name="someType"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static Boolean isSameTypeInList(Type someType, params Type[] types)
        {
            for (int i = 0; i < types.Length; i++)
            {
                if (someType == types[i]) return true;
            }
            return false;
        }
        /// <summary>
        /// 타입이 unsignedType인지 판단한다.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Boolean isTypeUnsigned(Type type)
        {
            return isSameTypeInList(type, typeof(UInt16), typeof(UInt32), typeof(UInt64), typeof(Byte));
        }

        /// <summary>
        /// 제일 처음이나 제일 뒤에 B로 시작하고 0과 1로만 되어있는 문자열은 Bin형식이다.<br/>
        /// 0x로 시작하고, 0~9,A~F,a~f로 이루어진 문자열은 HexSTring이다.<br/>
        /// 0으로 시작하고 0~7까지의 숫자로 이루이진 문자열은 OctString이다.(8비트)<br/>
        /// .이 붙거나 1.2e-10과같은 부동소수점 문자열은 Float형식이다.<br/>
        /// 모두 숫자로 이루어져있거나 -숫자 형식이면 Integer이다.<br/>
        /// 그 이외에 문자열 중 한글자이면 Char, 한글자 이상이면 String이다.
        /// </summary>
        /// <param name="numberUnit"></param>
        /// <returns></returns>
        public static TypeName getTypeKind(string numberUnit)
        {
            Int64 num;
            UInt64 unum;
            if (numberUnit.IndexOf("B") == 0 || numberUnit.IndexOf("b") == 0)//앞에 b가 오는 bin형식
            {
                Char[] unit = null;
                for (int i = 1; i < numberUnit.Length; i++)
                {
                    Char c = numberUnit[i];
                    if (c.Equals('0') == false && c.Equals('1') == false)
                    {
                        if (Char.IsDigit(c)) return TypeName.String;//B다음에 오는 숫자가 1이나 0이 아니면 무조건 String취급
                        else if (unit==null || unit.Length == 0)
                        {
                            unit = new Char[] { c }; //숫자가 아닌 기호 한 종류라면 seperator라고 간주한다.
                        }
                        else if(unit!=null && unit[0]!=c)
                        {
                            return TypeName.String; //기호가 1종류를 넘어가면 string이다.
                        }
                    }
                }
                return TypeName.Bin;
            }

            else if (numberUnit.IndexOf("0X") == 0 || numberUnit.IndexOf("0x") == 0)
            {
                for (int i = 2; i < numberUnit.Length; i++)
                {
                    if (numberUnit[i].CompareTo('0') >= 0 && numberUnit[i].CompareTo('9') <= 0) continue;
                    else if (numberUnit[i].CompareTo('a') >= 0 && numberUnit[i].CompareTo('f') <= 0) continue;
                    else if (numberUnit[i].CompareTo('A') >= 0 && numberUnit[i].CompareTo('F') <= 0) continue;
                    else
                    {
                        if (numberUnit.Length == 1) return TypeName.Char;
                        else return TypeName.String;
                    }
                }
                return TypeName.HexString;
            }
            else if (numberUnit.IndexOf("B") == numberUnit.Length - 1 || numberUnit.IndexOf("b") == numberUnit.Length - 1)//뒤에 B가 오는 bin형식
            {
                Char[] unit = null;
                for (int i = 0; i < numberUnit.Length - 1; i++)
                {
                    Char c = numberUnit[i];
                    if (c.Equals('0') == false && c.Equals('1') == false)
                    {
                        if (Char.IsDigit(c)) return TypeName.String;//B다음에 오는 숫자가 1이나 0이 아니면 무조건 String취급
                        else if (unit == null || unit.Length == 0)
                        {
                            unit = new Char[] { c }; //숫자가 아닌 기호 한 종류라면 seperator라고 간주한다.
                        }
                        else if (unit != null && unit[0] != c)
                        {
                            return TypeName.String; //기호가 1종류를 넘어가면 string이다.
                        }
                    }
                }
                return TypeName.Bin;
            }
            else if (numberUnit[0].Equals('0') && numberUnit.IndexOf('.')<0)
            {
                if (numberUnit.Length < 2) return TypeName.Integer; //크기가 1이고 0이라면 당연히 integer이다.
                
                else if (numberUnit[1].Equals('0')) return TypeName.String; //oct형식은 제일 첫번째만 0이다. 두번째도 0이라면 문자열취급한다.
                /*
                else if (numberUnit.IndexOf('.') > 0)
                {
                    double d;
                    if (Double.TryParse(numberUnit, out d)) return TypeName.Float;
                }
                 */
                for (int i = 1; i < numberUnit.Length; i++)
                {
                    
                    if (numberUnit[i].CompareTo('0') < 0 || numberUnit[i].CompareTo('7') > 0) return TypeName.String;
                }
                return TypeName.OctString;
            }
            else if (numberUnit.IndexOf('.') >= 0)
            {
                //int dot = 0;
                //int e = 0;
                char lastChar = numberUnit[numberUnit.Length - 1];

                if (lastChar == 'f' || lastChar == 'F')
                {
                    numberUnit=numberUnit.Substring(0, numberUnit.Length - 1); //맨 마지막 글자 떼어준다.
                }
                float a;
                double b;
                if (float.TryParse(numberUnit, out a))
                {
                    return TypeName.Float;
                }else if (double.TryParse(numberUnit, out b)){
                    return TypeName.Float;
                }else{
                    return TypeName.String;
                }
                /*
                for (int i = 0; i < numberUnit.Length; i++)
                {
                    if (numberUnit[i].CompareTo('0') >= 0 && numberUnit[i].CompareTo('9') <= 0) continue;
                    else if (dot == 0 && numberUnit[i].Equals('.'))
                    {
                        dot = 1; //.은 하나만 있어야 한다.
                        continue;
                    }
                    else if (e == 0 && (numberUnit[i].Equals('e') || numberUnit[i].Equals('E')))
                    {
                        e++; //e는 있더라도 하나만 있어야 한다.
                    }
                    else if (e == 1)
                    {
                        if (numberUnit[i].Equals('+') || numberUnit[i].Equals('-'))
                        {
                            continue;
                        }
                        else
                        {
                            if (numberUnit.Length == 1) return TypeName.Char;
                            else return TypeName.String;
                        }
                    }
                    else
                    {
                        if (numberUnit.Length == 1) return TypeName.Char;
                        else return TypeName.String;
                    }
                }
                 return TypeName.Float;
                */

            }
            else if (Int64.TryParse(numberUnit, out num))
            {
                return TypeName.Integer;
            }
            else if (UInt64.TryParse(numberUnit, out unum))
            {
                return TypeName.Integer;
            }
            else
            {
                if (numberUnit.Length == 1) return TypeName.Char;
                else return TypeName.String;

            }

        }

        public static IntegerType getIntType(string numStr,bool unsigned)
        {
            if (unsigned)
            {
                UInt64 num = UInt64.Parse(numStr);
                if (num <= Byte.MaxValue) return IntegerType.Byte;
                else if (num <= UInt16.MaxValue) return IntegerType.UInt16;
                else if (num <= UInt32.MaxValue) return IntegerType.UInt32;
                else return IntegerType.UInt64;
            }
            else
            {
                Int64 num = Int64.Parse(numStr);
                if (num <= SByte.MaxValue) return IntegerType.SByte;
                else if (num <= Int16.MaxValue) return IntegerType.Int16;
                else if (num <= Int32.MaxValue) return IntegerType.Int32;
                else return IntegerType.Int64;
            }
        }

        public static int getIntBytes<T>(T num) where T:IComparable<T>
        {
            String hex = String.Format("{0:X}", num);
            if (hex.Length > 8) return 8;
            else if (hex.Length > 4) return 4;
            else if (hex.Length > 2) return 2;
            else return 1;
        }

        public static int parseNumberToByteArray(string num, Array dest, int offset, Type type, Boolean isSwap = false)
        {
            if (num == null || num.Length == 0) num = "0";
            //parseTypeSize 는 string을 숫자로 변환시킬 때, 기준이 될 타입의 크기를 정하는 것이다.
            //예를 들어, 1은 byte에도 들어갈 수 있는 숫자이지만, parseTypeSize를 4로 하면 int에 들어가게 된다.

            //Byte[] buff = new Byte[8]; //숫자가 가질 수 있는 최대 크기.
            //int parseTypeSize = Marshal.SizeOf(type);
            Array data = null;
            bool isHexNumber = false;
            if (num.Length>=3 && num.Substring(0, 2).ToLower().Equals("0x"))
            {
                isHexNumber = true;
                num = num.Substring(2);//앞에 0x를 뺌..
            }

            if (type == typeof(byte))
            {
                data = new byte[1];
                if(isHexNumber) data.SetValue(byte.Parse(num, NumberStyles.HexNumber, CultureInfo.CurrentCulture), 0);
                else data.SetValue(byte.Parse(num, NumberStyles.Integer, CultureInfo.CurrentCulture), 0);
                
            }
            else if (type == typeof(sbyte))
            {
                data = new sbyte[1];
                if (isHexNumber) data.SetValue(sbyte.Parse(num, NumberStyles.HexNumber, CultureInfo.CurrentCulture), 0);
                else data.SetValue(sbyte.Parse(num, NumberStyles.Integer, CultureInfo.CurrentCulture), 0);

            }
            else if (type == typeof(short))
            {
                data = new short[1];
                if (isHexNumber) data.SetValue(short.Parse(num, NumberStyles.HexNumber, CultureInfo.CurrentCulture), 0);
                else data.SetValue(short.Parse(num, NumberStyles.Integer, CultureInfo.CurrentCulture), 0);

            }
            else if (type == typeof(ushort))
            {
                data = new ushort[1];
                if (isHexNumber) data.SetValue(ushort.Parse(num, NumberStyles.HexNumber, CultureInfo.CurrentCulture), 0);
                else data.SetValue(ushort.Parse(num, NumberStyles.Integer, CultureInfo.CurrentCulture), 0);

            }
            else if (type == typeof(int))
            {
                data = new int[1];
                if(isHexNumber) data.SetValue(int.Parse(num, NumberStyles.HexNumber, CultureInfo.CurrentCulture), 0);
                else data.SetValue(int.Parse(num, NumberStyles.Integer, CultureInfo.CurrentCulture), 0);

            }
            else if (type == typeof(uint))
            {
                data = new uint[1];
                if(isHexNumber) data.SetValue(uint.Parse(num, NumberStyles.HexNumber, CultureInfo.CurrentCulture), 0);
                else data.SetValue(uint.Parse(num, NumberStyles.Integer, CultureInfo.CurrentCulture), 0);

            }
            else if (type == typeof(long))
            {
                data = new long[1];
                if(isHexNumber) data.SetValue(long.Parse(num, NumberStyles.HexNumber, CultureInfo.CurrentCulture), 0);
                else data.SetValue(long.Parse(num, NumberStyles.Integer, CultureInfo.CurrentCulture), 0);

            }
            else if (type == typeof(ulong))
            {
                data = new ulong[1];
                if(isHexNumber) data.SetValue(ulong.Parse(num, NumberStyles.HexNumber, CultureInfo.CurrentCulture), 0);
                else data.SetValue(ulong.Parse(num, NumberStyles.Integer, CultureInfo.CurrentCulture), 0);

            }
            else
            {
                data = new uint[1];
                if(isHexNumber) data.SetValue(uint.Parse(num, NumberStyles.HexNumber, CultureInfo.CurrentCulture), 0);
                else data.SetValue(uint.Parse(num, NumberStyles.Integer, CultureInfo.CurrentCulture), 0);

            }
            int size = Marshal.SizeOf(type);
            if (isSwap) Swaper.swapWithSize(data, dest, size, size, 0, offset);
            else Buffer.BlockCopy(data, 0, dest, offset,size);
            return size;

            /*
            if (getTypeKind(num) == TypeName.HexString)
            {
                int size;
                if (isSwap)
                {
                    size = getHexNumber(num, buff, 0);
                    if (size < Marshal.SizeOf(type)) size = Marshal.SizeOf(type);
                    Buffer.BlockCopy(NetUsable<byte>.getTArrayFromArraySwapped(buff, parseTypeSize, 0, size), 0, dest, offset, size);
                }
                else
                {
                    size = getHexNumber(num, dest, offset);
                    if (size < Marshal.SizeOf(type)) size = Marshal.SizeOf(type);
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
             */
        }

        /// <summary>
        /// num이 0x형식이 아니라면 rate를 곱한 값을 array로 보낸다.
        /// </summary>
        /// <param name="num"></param>
        /// <param name="dest"></param>
        /// <param name="offset"></param>
        /// <param name="type"></param>
        /// <param name="rate"></param>
        /// <param name="isSwap"></param>
        /// <returns></returns>
        public static int parseNumberToByteArray(string num, Array dest, int offset, Type type, double rate, Boolean isSwap = false)
        {
            if (num == null || num.Length == 0) num = "0";
            //parseTypeSize 는 string을 숫자로 변환시킬 때, 기준이 될 타입의 크기를 정하는 것이다.
            //예를 들어, 1은 byte에도 들어갈 수 있는 숫자이지만, parseTypeSize를 4로 하면 int에 들어가게 된다.

            //Byte[] buff = new Byte[8]; //숫자가 가질 수 있는 최대 크기.
            //int parseTypeSize = Marshal.SizeOf(type);
            Array data = null;
            bool isHexNumber = false;
            
            if (num.Length >= 3 && num.Substring(0, 2).ToLower().Equals("0x"))
            {
                isHexNumber = true;
                num = num.Substring(2);//앞에 0x를 뺌..
            }

            if (isHexNumber == false)
            {
                double outVar;
                string oldNum="";
                if (double.TryParse(num, out outVar))
                {
                    oldNum = num;
                    num = (outVar * rate).ToString("0.##########");//소수점 10자리까지만..
                }
                long aa;
                if ( type != typeof(double) && type != typeof(float) && long.TryParse(num, out aa)==false)
                {
                    
                    throw new Exception(num + "<-" + oldNum + " is not valid format...");
                }
            }

            if (type == typeof(byte))
            {
                data = new byte[1];
                if (isHexNumber) data.SetValue(byte.Parse(num, NumberStyles.HexNumber, CultureInfo.CurrentCulture), 0);
                else data.SetValue((byte)(byte.Parse(num)), 0);

            }
            else if (type == typeof(sbyte))
            {
                data = new sbyte[1];
                if (isHexNumber) data.SetValue(sbyte.Parse(num, NumberStyles.HexNumber, CultureInfo.CurrentCulture), 0);
                else data.SetValue((sbyte)(sbyte.Parse(num)), 0);

            }
            else if (type == typeof(short))
            {
                data = new short[1];
                if (isHexNumber) data.SetValue(short.Parse(num, NumberStyles.HexNumber, CultureInfo.CurrentCulture), 0);
                else data.SetValue((short)(short.Parse(num)), 0);

            }
            else if (type == typeof(ushort))
            {
                data = new ushort[1];
                if (isHexNumber) data.SetValue(ushort.Parse(num, NumberStyles.HexNumber, CultureInfo.CurrentCulture), 0);
                else data.SetValue((ushort)(ushort.Parse(num)), 0);

            }
            else if (type == typeof(int))
            {
                data = new int[1];
                if (isHexNumber) data.SetValue(int.Parse(num, NumberStyles.HexNumber, CultureInfo.CurrentCulture), 0);
                else data.SetValue((int)(int.Parse(num)), 0);

            }
            else if (type == typeof(uint))
            {
                data = new uint[1];
                if (isHexNumber) data.SetValue(uint.Parse(num, NumberStyles.HexNumber, CultureInfo.CurrentCulture), 0);
                else data.SetValue((uint)(uint.Parse(num)), 0);

            }
            else if (type == typeof(long))
            {
                data = new long[1];
                if (isHexNumber) data.SetValue(long.Parse(num, NumberStyles.HexNumber, CultureInfo.CurrentCulture), 0);
                else data.SetValue((long)(long.Parse(num)), 0);

            }
            else if (type == typeof(ulong))
            {
                data = new ulong[1];
                if (isHexNumber) data.SetValue(ulong.Parse(num, NumberStyles.HexNumber, CultureInfo.CurrentCulture), 0);
                else data.SetValue((ulong)(ulong.Parse(num)), 0);

            }
            else
            {
                data = new uint[1];
                if (isHexNumber) data.SetValue(uint.Parse(num, NumberStyles.HexNumber, CultureInfo.CurrentCulture), 0);
                else data.SetValue((uint)(uint.Parse(num)), 0);

            }
            int size = Marshal.SizeOf(type);
            if (isSwap) Swaper.swapWithSize(data, dest, size, size, 0, offset);
            else Buffer.BlockCopy(data, 0, dest, offset, size);
            return size;

           
        }


        public static int parseNumberToByteArray(string num, Array dest, int offset, int parseTypeSize, Boolean isSwap = false)
        {
            if (num == null || num.Length == 0) num = "0";
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

        private static int getIntNumber(string num, Array dest, int offset, int baseSizeInByte, Boolean isSwap = false)
        {
            if (num == null || num.Length == 0) return 0;
            Int64 tempNum = Int64.Parse(num);

            //baseSizeInByte = (getIntBytes(tempNum) > baseSizeInByte) ? getIntBytes(tempNum) : baseSizeInByte;
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
            UInt64[] hexNumber = new UInt64[1];
            hexNumber[0] = getHexNumber<UInt64>(hexString); //일단 hex에서 숫자 추출하여
            int numberTypeSize = getIntBytes(hexNumber[0]); //순수한 숫자의 byte크기를 가져온다.
            Buffer.BlockCopy(hexNumber, 0, dest, destOffset, numberTypeSize); //앞에서 부터 채우면 작은 부분부터 들어간다.
            return numberTypeSize;
        }

        public static void getHexNumber(string hexString, Array dest, int destOffset, int byteSize)
        {
            UInt64[] hexNumber = new UInt64[1];
            hexNumber[0] = getHexNumber<UInt64>(hexString); //일단 hex에서 숫자 추출하여
            Buffer.BlockCopy(hexNumber, 0, dest, destOffset, byteSize); //앞에서 부터 채우면 작은 부분부터 들어간다.
        }

        public static T getHexNumber<T>(string hexString) where T:IComparable<T>
        {
            T[] value = new T[1];

            if (hexString == null || hexString.Length == 0) return value[0];
            
            if (hexString.IndexOf("0X") == 0 || hexString.IndexOf("0x") == 0) hexString = hexString.Substring(2);

            int size = hexString.Length;
            UInt64[] temp = new UInt64[1];
            temp[0] = UInt64.Parse(hexString, NumberStyles.HexNumber);
            Buffer.BlockCopy(temp, 0, value, 0, Buffer.ByteLength(value));
            /*
            hexString = hexString.ToUpper();
            for (int i = 0; i < size; i++)
            {
                temp <<= 4;

                if (hexString[i] - '0' < 10) temp += (hexString[i] - '0');
                else temp += (hexString[i] - 'A' + 10);
            }
            */
            return value[0];
        }

        public static Int64 getOctNumber(string octString)
        {
            if (octString == null || octString.Length == 0) return 0;
            int size = octString.Length;
            Int64 temp = 0;
            int num;
            for (int i = 0; i < size; i++)
            {
                temp <<= 3;
                num = (octString[i] - '0');
                temp += num;
                if (num > 7 || num < 0) throw new Exception("oct형식이 아닙니다:" + octString);
            }

            return temp;
        }

        public static int getOctNumber(string octString, Array dest, int offset, int baseSizeInByte = 4, Boolean isSwap = false)
        {
            if (octString == null || octString.Length == 0) return 0;

            if (octString[0].Equals('0')) octString = octString.Substring(1);
            Int64 octNum = getOctNumber(octString);
            int octSize = getIntBytes(octNum);
            if (octSize > baseSizeInByte) baseSizeInByte = octSize;

            if (isSwap)
            {
                NetUsable<byte>.arrayToOtherArraySwappedBySize(
                    BitConverter.GetBytes(octNum), 0, dest, offset, baseSizeInByte, sizeof(Int64));
            }
            else
            {
                Buffer.BlockCopy(BitConverter.GetBytes(octNum), sizeof(Int64) - baseSizeInByte, dest, offset, baseSizeInByte);
            }

            return baseSizeInByte;
        }

        public static Int64 getBinNumber(String binStr)
        {
            if (binStr == null || binStr.Length == 0) return 0;
            if (binStr[0] == 'B' || binStr[0] == 'b') binStr = binStr.Substring(1); //앞에 b가 있으면 없애줌.
            else if (binStr[binStr.Length - 1] == 'B' || binStr[binStr.Length - 1] == 'b') binStr = binStr.Substring(0, binStr.Length - 1);
            int num = 0;
            Char[] sep = null;
            for (int i = 0; i < binStr.Length; i++)
            {
                num <<= 1;
                if (binStr[i] == '1')
                    num = num | 1;
                else if (binStr[i] != '0')
                {
                    if(Char.IsDigit(binStr[i])) throw new Exception("2진 형식 숫자가 아닙니다:" + binStr); //0과 1이 아닌 숫자나오면, 에러.
                    else if (sep == null)
                    {
                        sep = new Char[] { binStr[i] }; //문자1개까진 seperator로 인식함.
                        num = num >> 1; //처음에 비트이동한 것을 되돌림.
                    }
                    else if(sep[0] != binStr[i])
                    {
                        throw new Exception("2진 형식 숫자가 아닙니다:" + binStr); //seperator종류가 2개일 수는 없으므로 에러
                    }
                    else if (sep != null && binStr[i] == sep[0])
                    {
                        num = num >> 1;//처음에 비트이동한 것을 되돌림.
                    }
                }
                

            }
            return num;
        }

        public static String getBinNumber(object numFrom, int bits, String seperator = "", int sepSizeOrPos1 = 4, params int[] sepSizeOrPos)
        {
            Int64 num=0;
            if (numFrom.GetType().Equals(typeof(int))) num = (int)(numFrom);
            else if (numFrom.GetType().Equals(typeof(short))) num = (short)(numFrom);
            else if (numFrom.GetType().Equals(typeof(byte))) num = (byte)(numFrom);
            else if (numFrom.GetType().Equals(typeof(Int64))) num = (Int64)(numFrom);
            else if (numFrom.GetType().Equals(typeof(float))) num = (Int64)(float)(numFrom);
            else if (numFrom.GetType().Equals(typeof(double))) num = (Int64)(double)(numFrom);
            else return "NaN";

            String bin = "";
            Int64 mask = 1;
            for (int i = 0; i < bits; i++)
            {
                if (i!=0){
                    if (sepSizeOrPos != null && sepSizeOrPos.Length > 0)
                    {
                        for (int pos = 0; pos < sepSizeOrPos.Length; pos++)
                        {
                            if (i == sepSizeOrPos[pos] || i == sepSizeOrPos1) bin = seperator + bin;
                        }
                    }
                    else if (i % sepSizeOrPos1 == 0) bin = seperator + bin;
                }
                bin = (((num & mask)>0)?"1":"0") + bin;
                mask <<=1;
                
            }
            return bin;
        }
        
        /// <summary>
        ///  value에서 bit를 가져옴..
        ///  4,3,2 번째 bit를 가져오려면 getBits(value, 1,3)을 한다.
        /// </summary>
        /// <param name="value">value to get bit</param>
        /// <param name="startBitOffset">시작 bit offset from 0. </param>
        /// <param name="size">size of bit from 1</param>
        /// <returns></returns>
        static int getBits(uint value, int startBitOffset, int size)
        {
            if (size < 1) throw new Exception("size must be more than 1");
            int mask = ((1 << size) - 1) << startBitOffset;
            int val = (int)((value & mask) >> (startBitOffset));
            return val;
        }
        
        //for compatabliity of old version
        public static TypeName getNumberByType(String numStr, ref Int64 i, ref double d, ref string str)
        {
            String a="";
            return getValueAndType(numStr, ref i, ref d, ref a);
        }
        

        // <summary>
        // Just return Integer, Float, String
        // </summary>
        public static TypeName getValueAndType(String numStr, ref Int64 i, ref double d, ref string str )
        {
            TypeName t = getTypeKind(numStr);
            
            switch (t)
            {
                case TypeName.Bin:
                    i = getBinNumber(numStr);
                    d = 0;
                    str = "";
                    return TypeName.Integer;
                case TypeName.Float:
                    d = Double.Parse(numStr);
                    i = 0;
                    str = "";
                    return TypeName.Float;
                case TypeName.HexString:
                    i = getHexNumber<Int64>(numStr);
                    d = 0;
                    str = "";
                    return TypeName.Integer;
                case TypeName.Integer:
                    i = Int64.Parse(numStr);
                    d = 0;
                    str = "";
                    return TypeName.Integer;
                case TypeName.OctString:
                    i = getOctNumber(numStr);
                    d = 0;
                    str = "";
                    return TypeName.Integer;
                case TypeName.Char:
                case TypeName.String:
                    i = 0;
                    d = 0;
                    str = numStr;
                    return TypeName.String;
            }
            return t;
        }


        public static Boolean isValidType(string param, string type, Platforms platform = Platforms.C32Bit)
        {
            param = param.Trim();
            if (param == null || param.Length == 0) return false; //비어있으면 false
            type = type.ToLower();
            //UInt32 nodw;
            //UInt16 now;
            UInt64 now64;
            Int64 noi64;
            Int32 noi;
            Int16 nos;
            Byte nob;
            double nod;
            float nof;
            char noc;
            
            TypeName typeName = getTypeKind(param) ;
            String paramType="";
            if (typeName == TypeName.HexString){
                
                if(type.Equals("hex") || type.Equals("hexstring"))
                    return true;
                else
                {
                    String p = param.ToLower();
                    if (p.IndexOf("0x") == 0)
                        p = p.Substring(2);
                    if(p.Length<=2) paramType = "byte";
                    else if(p.Length<=4) paramType = "ushort";
                    else if(p.Length<=8) paramType = "uint";
                    else if(p.Length<=16) paramType = "uint64";
                }
            }

            if (platform == Platforms.C32Bit) //32비트 플랫폼이면 long이 32비트이다.
            {
                if (type.Equals("long")) type = "int";
                else if(type.Equals("unsigned long")) type = "uint";
                
            }
            
            
            if (type.Equals("char")) type = "byte";
            
            if(paramType.Equals(type)) return true;
            

            switch (type)
            {
                case "bin":
                case "binary":
                case "bit":
                    return (typeName == TypeName.Bin);
                case "nonumber":
                    return !findNumber(param);

                case "hasnumber":
                    return findNumber(param);
                case "word64":
                case "ddword":
                case "uint64":
                case "u_long":
                case "unsigned long long":
                case "double double word":
                case "8byte":
                case "8bytes":
                case "int64":
                case "long long":
                case "long":
                case "ulong":
                    if (typeName == TypeName.HexString) return true;
                    if (typeName == TypeName.OctString) return true;
                    if (typeName == TypeName.Integer)
                    {
                        switch (type)
                        {
                            case "int64":
                            case "long long":
                            case "long":
                                return Int64.TryParse(param, out noi64);
                            default:
                                return UInt64.TryParse(param, out now64);
                        }
                    }
                    return false;
                    //
                case "unsigned int":
                case "uint32":
                case "uint":
                case "double word":
                case "dword":
                case "4byte":
                case "4bytes":
                case "int32":
                case "int":
                    if (typeName == TypeName.HexString)
                    {
                        if (param.IndexOf("0x") == 0 || param.IndexOf("0X") == 0)
                        {
                            if (param.Length > 10) throw new ArgumentOutOfRangeException("param", param, "4byte의 범위를 넘어서는 값입니다.");
                            //else return Int64.TryParse(param, System.Globalization.NumberStyles.HexNumber,new CultureInfo("ko-KR"), out noi64);
                        }
                        else
                        {
                            if (param.Length > 8) throw new ArgumentOutOfRangeException("param", param, "4byte의 범위를 넘어서는 값입니다.");
                            //else return Int64.TryParse(param, System.Globalization.NumberStyles.HexNumber, new CultureInfo("ko-KR"), out noi64);
                        }
                        return true;
                    }
                    else if (typeName == TypeName.OctString)
                    {
                        if (param.Length > 13) throw new ArgumentOutOfRangeException("param", param, "4byte의 범위를 넘어서는 값입니다.");
                        else return true;
                    }
                    else if (typeName == TypeName.Integer)
                    {
                        uint outInt;
                        switch(type){
                            case "int32":
                            case "int":
                                return Int32.TryParse(param, out noi);
                            default:
                                return UInt32.TryParse(param, out outInt);
                        }
                    }else return false;
                    //return UInt32.TryParse(param, out nodw);
                case "word":
                case "unit16":
                case "unsigned short":
                case "ushort":
                case "u_short":
                case "2byte:":
                case "2bytes":
                case "int16":
                case "short":
                    if (typeName == TypeName.HexString)
                    {
                        if (param.IndexOf("0x") == 0 || param.IndexOf("0X") == 0)
                        {
                            if (param.Length > 6) throw new ArgumentOutOfRangeException("param", param, "2byte의 범위를 넘어서는 값입니다.");
                            //else return short.TryParse(param, System.Globalization.NumberStyles.HexNumber, new CultureInfo("ko-KR"), out nos);
                        }
                        else
                        {
                            if (param.Length > 4) throw new ArgumentOutOfRangeException("param", param, "2byte의 범위를 넘어서는 값입니다.");
                            //else return short.TryParse(param, System.Globalization.NumberStyles.HexNumber, new CultureInfo("ko-KR"), out nos);
                        }
                        return true;
                    }
                    else if (typeName == TypeName.OctString)
                    {
                        if (param.Length > 7) throw new ArgumentOutOfRangeException("param", param, "2byte의 범위를 넘어서는 값입니다.");
                        else return true;

                    }
                    else if (typeName == TypeName.Integer)
                    {
                        UInt16 outInt;
                        switch (type)
                        {
                            case "int16":
                            case "short":
                                return Int16.TryParse(param, out nos);
                            default:
                                return UInt16.TryParse(param, out outInt);
                        }
                    }
                    else return false;
                    //return UInt16.TryParse(param, out now);
                case "byte":
                case "1byte":
                case "uchar":
                case "unsigned char":
                    if (typeName == TypeName.HexString)
                    {
                        if (param.IndexOf("0x") == 0 || param.IndexOf("0X") == 0)
                        {
                            if (param.Length > 4) throw new ArgumentOutOfRangeException("param", param, "1byte의 범위를 넘어서는 값입니다.");
                            //else return true;// return Byte.TryParse(param, System.Globalization.NumberStyles.HexNumber, new CultureInfo("ko-KR"), out nob);
                        }
                        else
                        {
                            if (param.Length > 2) throw new ArgumentOutOfRangeException("param", param, "1byte의 범위를 넘어서는 값입니다.");
                            //else return true;// return Byte.TryParse(param, System.Globalization.NumberStyles.HexNumber, new CultureInfo("ko-KR"), out nob);
                        }
                        return true;
                    }
                    else if (typeName == TypeName.OctString)
                    {
                        if (param.Length > 4) throw new ArgumentOutOfRangeException("param", param, "1byte의 범위를 넘어서는 값입니다.");
                        else return true;
                    }
                    else if (typeName == TypeName.Integer)
                    {
                        return Byte.TryParse(param, out nob);
                    }
                    return false;
                case "int8":
                case "sbyte":
                    if (typeName == TypeName.HexString)
                    {
                        return false;
                    }
                    else if (typeName == TypeName.OctString)
                    {
                        return false;
                    }
                    else if (typeName == TypeName.Integer)
                    {
                        sbyte nosb;
                        return SByte.TryParse(param, out nosb);
                    }
                    return false;
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
                case "char":
                    return Char.TryParse(param, out noc);
                default:
                    return false;
            }
        }

        public enum VariableTypeForms { C64BitShort, C64Bit };

        public static String[] GetVariableTypeList(VariableTypeForms plaform = VariableTypeForms.C64BitShort)
        {
            if (plaform == VariableTypeForms.C64BitShort)
            {
                return new String[]{
                    "char",
                    "uchar",
                    "short",
                    "ushort",
                    "int",
                    "uint",
                    "long",
                    "ulong",
                    "float",
                    "double"
                };
            }
            else// if (plaform == VariableTypeForms.C64Bit)
            {
                return new String[]{
                    "char",
                    "unsigned char",
                    "short",
                    "unsigned short",
                    "int",
                    "unsigned int",
                    "long",
                    "unsigned long",
                    "float",
                    "double"
                };
            }
        }

        public static Type[] GetVariableRealTypeList()
        {
            return new Type[]{
                    typeof(sbyte),
                    typeof(byte),
                    typeof(short),
                    typeof(ushort),
                    typeof(int),
                    typeof(uint),
                    typeof(long),
                    typeof(ulong),
                    typeof(float),
                    typeof(double)
                };
        }

        /// <summary>
        /// get Type from the name of type as string. if it doens't exist, return null.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="platform"></param>
        /// <returns></returns>
        public static Type getTypeFromTypeName(string type, Platforms platform= Platforms.C32Bit)
        {
            type = type.ToLower();

            switch (type)
            {
                case "word64":
                case "ddword":
                case "uint64":
                case "unsigned long long":
                case "double double word":
                case "8byte":
                case "8bytes":
                    return typeof(UInt64);
                case "ulong":
                case "u_long":
                    if( platform== Platforms.C32Bit || platform == Platforms.CS32Bit) return typeof(UInt32);
                    else return typeof(UInt64);
                case "unsigned long":
                case "unsigned int":
                case "uint32":
                case "uint":
                case "u_int":
                case "double word":
                case "dword":
                case "4byte":
                case "4bytes":
                    return typeof(UInt32);
                case "word":
                case "uint16":
                case "u_short":
                case "unsigned short":
                case "ushort":
                case "2byte:":
                case "2bytes":
                    return typeof(UInt16);
                case "int64":
                case "long long":
                    return typeof(Int64);
                case "long":
                    if (platform == Platforms.C32Bit || platform == Platforms.CS32Bit)
                        return typeof(Int32);
                    else return typeof(Int64);
                case "int32":
                case "int":
                    return typeof(Int32);
                case "int16":
                case "short":
                    return typeof(Int16);
                case "byte":
                case "1byte":
                case "u_char":
                case "uchar":
                case "unsigned char":
                    return typeof(byte);
                case "int8":
                case "sbyte":
                    return typeof(sbyte);
                case "double":
                    return typeof(Double);
                case "float":
                case "single":
                    return typeof(float);
                case "string":
                case "char*":
                case "char[]":
                case "char array":
                case "chararray":
                case "cstring":
                    return typeof(String);
                case "char":
                    if (platform == Platforms.C32Bit || platform == Platforms.C64Bit) return typeof(Byte);
                    else return typeof(Char); //CS
                default:
                    return null;
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

        public enum ViewFormat{X=0,D,E,F,x,d,e,f};
        public static String getArrUnits(Array arr, String seperator, ViewFormat viewFormat )
        {
            int arrLen = arr.Length;
            int arrTypeSize = Marshal.SizeOf(arr.GetValue(0));
            String str = "";
            String format = "{0:" + viewFormat.ToString() + (arrTypeSize * 2) + "},";

            for (int i = 0; i < arrLen; i++)
            {
                str+=String.Format(format, arr.GetValue(i));
            }
            return str;
        }
        public enum Platforms{C32Bit=0, C64Bit, CS32Bit, CS64Bit};

        public static int getTypeSize(string type, Platforms platform= Platforms.C32Bit)
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
                case "uint":
                case "u_int":
                case "unsigned int":
                    return 4;
                case "word":
                case "int16":
                case "short":
                case "u_short":
                case "ushort":
                case "unsigned short":
                    return 2;
                case "int64":
                case "double":
                case "long long":
                case "u_longlong":
                case "unsigned long long":
                case "ulonglong":
                case "int8":
                    return 8;
                case "long":
                case "unsigned long":
                case "ulong":
                    if (platform == Platforms.C32Bit || platform == Platforms.CS32Bit) return 4;
                    else return 8;
                case "byte":
                    return 1;
                case "char":
                    if (platform == Platforms.C32Bit || platform == Platforms.C64Bit) return 1;
                    else return 2; //c#
                case "uchar":
                case "u_char":
                case "unsigned char":
                    return 1;
                default:
                    return -1;
            }

        }

        public static Array GetArrayByTypeName(string type, int arrSize, out int typeSize, out Boolean unsigned, bool IsStringWithNullEnd = false)
        {
            Array packets = null;
            unsigned = false;
            type = type.ToLower();
            switch (type)
            {
                case "word64":
                case "ddword":
                case "uint64":
                case "unsigned long long":
                case "double double word":
                case "8byte":
                case "8bytes":
                case "u_long":
                case "ulong":
                    packets = new UInt64[arrSize];
                    typeSize = 8;
                    unsigned = true;
                    break;
                case "unsigned int":
                case "uint32":
                case "uint":
                case "u_int":
                case "double word":
                case "dword":
                case "4byte":
                case "4bytes":
                    packets = new UInt32[arrSize];
                    typeSize = 4;
                    unsigned = true;
                    break;
                case "word":
                case "uint16":
                case "u_short":
                case "unsigned short":
                case "ushort":
                case "2byte:":
                case "2bytes":
                    packets = new UInt16[arrSize];
                    typeSize = 2;
                    unsigned = true;
                    break;
                case "byte":
                case "1byte":
                case "u_char":
                case "ubyte":
                case "uchar":
                case "unsigned char":
                    packets = new Byte[arrSize];
                    typeSize = 1;
                    unsigned = true;
                    break;
                case "int64":
                case "long long":
                case "long":
                    packets = new Int64[arrSize];
                    typeSize = 8;
                    unsigned = false;
                    break;
                case "int32":
                case "int":
                    packets = new Int32[arrSize];
                    typeSize = 4;
                    unsigned = false;
                    break;
                case "int16":
                case "short":
                    packets = new Int16[arrSize];
                    typeSize = 2;
                    unsigned = false;
                    break;
                case "int8":
                case "sbyte":
                case "char":
                    packets = new SByte[arrSize];
                    typeSize = 1;
                    unsigned = false;
                    break;
                case "double":
                    packets = new double[arrSize];
                    typeSize = 8;
                    unsigned = false;
                    break;
                case "float":
                case "single":
                    packets = new Single[arrSize];
                    typeSize = 4;
                    unsigned = false;
                    break;
                case "string":
                case "char*":
                case "char[]":
                case "char array":
                case "chararray":
                case "cstring":
                   if (IsStringWithNullEnd)
                    {
                        packets = new byte[arrSize + 1];
                        Buffer.SetByte(packets, arrSize, (byte)'\0');
                    }
                    else packets = new byte[arrSize];
                    typeSize = 1;
                    //packets.SetValue(packets.Length - 1, 0);
                    break;
                default:
                    packets = null;
                    typeSize = -1;
                    unsigned = false;
                    break;
            }
 

            return packets;
        }

        private static Boolean findNumber(String str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (Char.IsDigit(str, i)) return true;
            }
            return false;
        }

        public enum LanguageTypes { C, CS };
        public static string GetTypeString(Type realType, LanguageTypes lt= LanguageTypes.C)
        {
            Type[] types = new Type[]{typeof(byte),typeof(sbyte),typeof(short),typeof(ushort),typeof(int),typeof(uint),typeof(long),typeof(ulong),typeof(float),typeof(double),typeof(string)};
            String[] typeStr;
            if (lt == LanguageTypes.C)
            {
                typeStr = new String[] { "byte", "signed byte", "short", "unsigned short", "int", "unsigned int", "long", "unsigned long", "float", "double", "string" };
            }
            else//CS
            {
                typeStr = new String[] { "byte", "sbyte", "short", "ushort", "int", "uint", "long", "ulong", "float", "double", "string" };
            }
            for (int i = 0; i < types.Length; i++)
            {
                if (types[i] == realType) return typeStr[i];
            }
            throw new Exception("TypeHandling.GetTypeString()::"+ realType.ToString() + " doesn't have valid name for this program..");
        }
    }
}
