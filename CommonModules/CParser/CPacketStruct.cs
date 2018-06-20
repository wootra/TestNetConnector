using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataHandling;
using RtwEnums.Network;
using System.Runtime.InteropServices;

namespace CustomParser
{
    [Serializable]
    public class CPacketStruct
    {
        public PacketItemCollection Items;
        public CStructInfo Infos = new CStructInfo();
        /*
        public List<string> names = new List<string>();
        public List<string> types = new List<string>();
        public List<int> sizes = new List<int>();
        public List<string> initStr = new List<string>();
        public List<Boolean> isSwap = new List<bool>();
        public List<object[]> initValues = new List<object[]>();
         */
        public String errStr = "";
        public String NativeText = "";
        //public String simpleText = "";
        public Byte[] PacketBuffer;
        public int PacketDataSize=0;
        public Encoding StringEncoding = Encoding.UTF8;
        public bool IsStringWithNullEnd = true;
        public Endians Endian = Endians.Big;

        public int Count
        {
            get
            {
                return Items.Count;// names.Count; 
            }
        }
        

        public CPacketStruct(Byte[] initPacketBuff = null)
        {
            if (initPacketBuff == null) PacketBuffer = new Byte[4096];
            else PacketBuffer = initPacketBuff;
            Items = new PacketItemCollection(this);
        }

        /// <summary>
        /// 같은 이름이 존재하는지 알아본다. 없으면 true를 리턴한다.
        /// item을 넣는 이유는 이 아이템을 제외하고 검색해야 하기 때문이다.
        /// </summary>
        /// <param name="item">이름을 셋팅할 item. null이면 모든 item에서 같은 이름을 찾음..</param>
        /// <param name="name">setting할 이름</param>
        /// <returns>같은 이름 없을 시 true리턴.</returns>
        public bool IsNewName(CPacketItem item, String name)
        {
            foreach (CPacketItem citem in this.Items)
            {
                if (item != null && citem.Equals(item)) continue;//같은 item 제외
                if (citem.Name.Equals(name))
                {
                    return false;//같은 이름이 있으므로 newName이 아니다.
                }
            }
            return true;
        }


        public CPacketItem getItem(int num){
            if (Items.Count < num + 1) return null;
            else return Items[num];
            //return new CStructItem(names[num], types[num], sizes[num], initValues[num]);
        }

        public void setNativeText(string str, bool swapWhenMakePacket)
        {
            NativeText = str;
            //Items = 
            StructXMLParser.CodeToItems(str, this);
            if(this.IsDynamicPacket==false) MakePacket(swapWhenMakePacket);
        }

        public String SimpleText
        {
            get
            {
                String str = "";
                string type = "";
                bool typeChanged = false;
                for (int i = 0; i < Items.Count; i++)
                {
                    if (type.Equals(Items[i].TypeString) == false)
                    {
                        if(i!=0) str+="/";
                        str += Items[i].TypeString + ">>";
                        typeChanged = true;
                    }
                    for (int v = 0; v < Items[i].InitValues.Length; v++)
                    {
                        if(typeChanged==false || v!=0){
                            str+=",";
                        }
                        str+=Items[i].InitValues[v];
                    }
                    type = Items[i].TypeString;
                    typeChanged = false;
                }
                return str;
            }
            
        }

        /// <summary>
        /// 내부의 NativeText에 지정된 C문법의 text를 분석하여
        /// 패킷을 생성하고, 그 패킷에 해당하는 string을 리턴한다.
        /// 만일 에러가 발생하면 exception을 발생시킨다.
        /// exeption의 원인은 exeption.Message에 있다.
        /// 이 메시지를 호출하고 난 뒤에, PacketBuffer에 보면 메시지가 생성되어 있고,
        /// PacketDataSize안에는 그 사이즈가 있다.
        /// </summary>
        /// <param name="str">분석을 원하는 c문법의 text</param>
        /// <returns>패킷에 해당하는 string. type1,값1,값2/type2,값3,값4...식의 포멧이다.</returns>
        public void MakePacket(String str, bool swap)
        {
            if (str == null || str.Length == 0)
            {
                str = NativeText;
            }
            else
            {
                NativeText = str;
            }
            Items.Clear();
            Items.CopyFrom(StructXMLParser.CodeToItems(str, null));
            MakePacket(swap);
            
            #region old
            /*
            
            CStructParser ns = new CStructParser();
            
            String[] lines = str.Split(";".ToCharArray()); //각 라인으로 나누어서
            object[] initValues;
            for (int i = 0; i < lines.Length; i++)
            {//각 라인들
                String line = lines[i];
                line = removeOuterWhiteSpace(line);

                if(line.Length==0) continue;

                int firstSpace = -1;
                int secondSpace = -1;
                int thirdSpace = -1;
                int typeSpace = -1;
                try
                {
                    firstSpace = line.IndexOf(' ');
                    secondSpace = line.IndexOf(' ', firstSpace+1);
                    thirdSpace = line.IndexOf(' ', secondSpace+1);
                }
                catch { }
                if (firstSpace < 0) firstSpace = line.IndexOf('[');
                if (firstSpace < 0) firstSpace = line.IndexOf('=');
                if (firstSpace < 0) firstSpace = line.Length;
                String type = line.Substring(0, firstSpace);

                if (TypeHandling.getTypeFromTypeName(type) == null)
                {//첫번째 토큰에서 제대로된 타입이 검출안되면
                    if (secondSpace > 0) type = line.Substring(0, secondSpace); //두번째 검색
                    else
                    {
                        setError("타입정의가 맞지 않습니다.", i, line); //두번째 스페이스가 없으면 에러
                        return null;
                    }

                    if (TypeHandling.getTypeFromTypeName(type) == null)
                    {//두번째 토큰에서 타입틀리면
                        if (thirdSpace > 0) type = line.Substring(0, thirdSpace);//세번째 검색
                        else
                        {
                            setError("타입정의가 맞지 않습니다.", i, line);//세번째 스페이스 없으면 에러
                            return null;
                        }

                        if (TypeHandling.getTypeFromTypeName(type) == null)
                        { //세번째 토큰에서도 타입이 틀리다면
                            setError("타입정의가 맞지 않습니다.", i, line); //무조건 에러
                            return null;
                        }
                        else
                        {
                            typeSpace = thirdSpace;
                        }
                    }
                    else
                    {
                        typeSpace = secondSpace;
                    }
                }
                else
                {
                    typeSpace = firstSpace;
                }
                type = type.ToLower();
                String rest = line.Substring(typeSpace);

                if (type.ToLower().Equals("string") == false) rest = rest.Trim();// rest.Replace(" ", ""); //string이 아니라면 나머지에서는 빈칸이 필요없다.

                //초기값 입력받기
                bool isSwap = false;
                if(rest.Length>5 && rest.Substring(0,5).ToLower().Equals("swap@")){//swap명령어.
                    rest = rest.Substring(5);//명령어 삭제.
                    isSwap = true;//값을 핸들링할 때 swap한다.
                }
                String initValue = "0"; //기본값은 0이다.
                String[] token = rest.Split("=".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                int openBracket;
                int closeBracket;
                if (token.Length == 2)
                {
                    token[0] = token[0].Trim();
                    token[1] = token[1].Trim();
                    initValue = token[1];

                }
                else if (token.Length > 2)
                {
                    setError("= 이 2번 이상 들어갑니다.", i, line);
                    return null;
                }
                
                rest = token[0]; //=오른쪽의 값은 잘라버림.
                //배열검사
                openBracket = rest.IndexOf('[');
                closeBracket = rest.IndexOf(']');

                int arrSize = 1;

                if (openBracket > 0 || closeBracket > 0) //배열인지 검사하여
                {
                    if (openBracket < 0 || closeBracket < 0)
                    {
                        setError("배열을 나타내는 [,] 기호 둘 중 하나가 없습니다", i, line);
                        return null;
                    }
                    String numStr = rest.Substring(openBracket + 1, closeBracket - openBracket - 1);
                    
                    if (numStr.Length == 0)
                    {
                        arrSize = initValue.Split(",".ToCharArray()).Length;
                        rest = rest.Substring(0, openBracket); //배열 크기를 가져왔으므로 배열기호그룹 삭제
                    }
                    else
                    {
                        int num = -1;
                        if (Int32.TryParse(numStr, out num) == false)
                        {
                            setError("배열기호[] 안에는 정수가 와야합니다.", i, line);
                            return null;
                        }
                        else
                        {
                            if (num <= 0)
                            {
                                setError("배열의 크기는 1보다 작을 수 없습니다.", i, line);
                                return null;
                            }
                            else
                            {
                                arrSize = num;
                                rest = rest.Substring(0, openBracket); //배열 크기를 가져왔으므로 배열기호그룹 삭제
                            }
                        }
                    }
                } //배열검사 끝.
                
                //초기값 검사
                openBracket = initValue.IndexOf('{');
                closeBracket = initValue.IndexOf('}');
                //initValues.Add(new object[arrSize]);//값 배열을 만들어줌
                initValues = new object[arrSize];
                if (openBracket >= 0 || closeBracket >= 0 || initValue.IndexOf(',')>=0) //배열형식의 초기값이라면
                {
                    if (openBracket < 0 || closeBracket < 0)
                    {
                        setError("{ 나 } 중에서 하나가 없습니다.", i, line);
                        return null;
                    }
                    String numStr = initValue.Substring(openBracket + 1, closeBracket - openBracket - 1); //브래킷 내부의 내용 가져옴
                    token = numStr.Split(",".ToCharArray());
                    
                    

                    if (token.Length > arrSize) //배열의 크기보다 클 때
                    {
                        setError("배열의 크기를 넘어서 초기화를 시도했습니다. size:" + arrSize + "  this:" + token.Length, i, line);
                        return null;
                    }
                    string newInitStr = "";
                    for (int j = 0; j < token.Length; j++) //초기값들의 타입을 검사한다.
                    {
                        Int64 intValue = 0;
                        Double doubleValue = 0.0;
                        String strValue = "";
                        if (type.Equals("string") == false && token[j].Length == 0) initValues[j] = 0;// initValues[i][j] = 0;

                        switch (TypeHandling.getValueAndType(token[j], ref intValue, ref doubleValue, ref strValue))
                        {//hex나 oct형식등을 모두 숫자형으로 먼저 치환한다.
                            case TypeHandling.TypeName.Integer:
                                if (isSwap)
                                {
                                    if (TypeHandling.getTypeFromTypeName(type) == typeof(byte)) initValues[j] = Swaper.swap<byte>((byte)intValue);
                                    else if (TypeHandling.getTypeFromTypeName(type) == typeof(short)) initValues[j] = Swaper.swap<short>((short)intValue);
                                    else if (TypeHandling.getTypeFromTypeName(type) == typeof(int)) initValues[j] = Swaper.swap<int>((int)intValue);
                                    else if (TypeHandling.getTypeFromTypeName(type) == typeof(long)) initValues[j] = Swaper.swap<long>((long)intValue);
                                    else if (TypeHandling.getTypeFromTypeName(type) == typeof(ushort)) initValues[j] = Swaper.swap<ushort>((ushort)intValue);
                                    else if (TypeHandling.getTypeFromTypeName(type) == typeof(uint)) initValues[j] = Swaper.swap<uint>((uint)intValue);
                                    else if (TypeHandling.getTypeFromTypeName(type) == typeof(ulong)) initValues[j] = Swaper.swap<ulong>((ulong)intValue);
                                }
                                else
                                {
                                    initValues[j] = intValue;
                                }
                                break;
                            case TypeHandling.TypeName.Float:
                                initValues[j] = doubleValue;
                                break;
                            case TypeHandling.TypeName.String:
                                initValues[j] = strValue;
                                break;
                        }
                        if (j != 0) newInitStr += ",";
                        newInitStr += initValues[j].ToString();
                        if (TypeHandling.isValidType(initValues[j].ToString(), type) == false)
                        {
                            setError("초기값이 타입과 다릅니다.Type:" + type + "  value:" + token[j], i, line);
                            return null;
                        }
                        
                    }
                    initValue = newInitStr;
                    
                }else //배열형식이 아니라 단순값일 때
                {
                    Int64 intValue = 0;
                    Double doubleValue = 0.0;
                    String strValue = "";
                    if (type.Equals("string") == false && initValue.Length == 0) initValues[0] = 0;


                    
                    
                    switch (TypeHandling.getValueAndType(initValue, ref intValue, ref doubleValue, ref strValue))
                    {//hex나 oct형식등을 모두 숫자형으로 먼저 치환한다.
                        case TypeHandling.TypeName.Integer:
                            //initValues[i][0] = intValue;
                            if (isSwap)
                            {
                                if (TypeHandling.getTypeFromTypeName(type) == typeof(byte)) initValues[0] = Swaper.swap<byte>((byte)intValue);
                                else if (TypeHandling.getTypeFromTypeName(type) == typeof(short)) initValues[0] = Swaper.swap<short>((short)intValue);
                                else if (TypeHandling.getTypeFromTypeName(type) == typeof(int)) initValues[0] = Swaper.swap<int>((int)intValue);
                                else if (TypeHandling.getTypeFromTypeName(type) == typeof(long)) initValues[0] = Swaper.swap<long>((long)intValue);
                                else if (TypeHandling.getTypeFromTypeName(type) == typeof(ushort)) initValues[0] = Swaper.swap<ushort>((ushort)intValue);
                                else if (TypeHandling.getTypeFromTypeName(type) == typeof(uint)) initValues[0] = Swaper.swap<uint>((uint)intValue);
                                else if (TypeHandling.getTypeFromTypeName(type) == typeof(ulong)) initValues[0] = Swaper.swap<ulong>((ulong)intValue);
                            }
                            else
                            {
                                initValues[0] = intValue;
                            }
                            break;
                        case TypeHandling.TypeName.Float:
                            initValues[0] = doubleValue;
                            break;
                        case TypeHandling.TypeName.String:
                            initValues[0] = strValue;
                            break;
                    }
                    
                    if (TypeHandling.isValidType(initValues[0].ToString(), type) == false)
                    {
                        String error = "초기값이 타입과 다릅니다.";
                        if(TypeHandling.getTypeKind(initValue)== TypeHandling.TypeName.HexString) error+=" hex값을 넣어주실 때는 unsigned 타입으로 지정하십시오.";
                        setError(error, i, line);
                        return null;
                    }
                    

                    initValue = initValues[0].ToString();
                    
                }
                //초기값 검사 끝
                rest = rest.Replace(" ", "");
                //변수명검사
                for (int j = 0; j < rest.Length; j++) //변수명 검사
                {
                    if ((Char.IsLetterOrDigit(rest[j]) == false) && rest[j].Equals('_')==false)
                    {
                        setError("변수명에는 기호가 들어갈 수 없습니다.", i, line);
                        return null;
                    }
                    else if (j == 0 && Char.IsDigit(rest[j]))
                    {
                        setError("변수명의 첫번째에는 숫자가 들어갈 수 없습니다.", i, line);
                        return null;
                    }
                }//변수명 검사 끝
                if (rest.Length == 0) rest = "var" + i;
                CStructItem item = new CStructItem(rest, type, arrSize, initValues);
                item.IsSwap = isSwap;
                item.InitString = initValue;
                ns.Items.Add(item);


            }//각 라인들 검색 끝
            this.Items.Clear();
            this.Items = ns.Items;
            */
            #endregion
            //simpleText = MakeMsg();
            //return simpleText;
        }
        public void MakePacket(bool swap)
        {
            String err;
            StructXMLParser.ItemsToPacket(this.Items, swap, PacketBuffer, out PacketDataSize, IsDynamicPacket);
            //if (err != null && err.Length > 0) throw new Exception(err);
        }

        void setError(string errMsg, int line, String lineStr)
        {
            this.errStr = errMsg + " Line(" + line + ") in \r\n" + lineStr;
            throw new Exception(this.errStr);
        }
        public void MakeMsgText()
        {
           NativeText = StructXMLParser.ItemsToCode(Items);
        }
        /*
String removeOuterWhiteSpace(String line)
{
    line = line.Replace("\n", "");
    line = line.Replace("\r", "");
    line = line.Replace("\0", "");
    line = line.Trim();//빈칸과 라인피드를 모두 삭제
    return line;
}
*/
        /*
/// <summary>
/// 내부의 NativeText에 지정된 text를 분석하여
/// 패킷을 생성하고, 그 패킷에 해당하는 string을 리턴한다.
/// 만일 에러가 발생하면 exception을 발생시킨다.
/// exeption의 원인은 exeption.Message에 있다.
/// 이 메시지를 호출하고 난 뒤에, PacketBuffer에 보면 메시지가 생성되어 있고,
/// PacketDataSize안에는 그 사이즈가 있다.
/// </summary>
public String MakeMsg()
{
    String msg = "";
    int openBracket = -1;
    int closeBracket = -1;

    String numStr;
    String[] token;

    for (int i = 0; i < this.Count; i++)
    {
        int size = this.Items[i].size;//.sizes[i];
        if (i != 0) msg += "/";
        msg += this.Items[i].type;//.types[i];
        openBracket = this.Items[i].InitString.IndexOf("{");//.initStr[i].IndexOf("{");
        closeBracket = this.Items[i].InitString.IndexOf("}");

        if (openBracket >= 0)
        {
            numStr = this.Items[i].InitString.Substring(openBracket + 1, closeBracket - openBracket - 1); //브래킷 내부의 내용 가져옴
        }
        else
        {
            numStr = this.Items[i].InitString;
        }
        token = numStr.Split(",".ToCharArray());


        for (int j = 0; j < size; j++)
        {
            if (token.Length > j) numStr = token[j];
            msg += ",";
            msg += numStr;
        }
    }
    String error = "";

    if (BuildMsg(msg, PacketBuffer, out PacketDataSize, out error) == false)
    {
        setError(error, -1, "");
    }
    return msg;
}
*/


        /*
        /// <summary>
        /// 패킷을 생성한다. 입력이 되는 msg는 int,1,2,3/short,1,2,3,4,5 와 같은 형식으로 나가는 arg 리스트이다.
        /// </summary>
        /// <param name="msg">타입이 달라지면 /로 구분하고, 그 전에는 , 로 구분하여 각 항목의 처음에는 타입을 명시하는 argument리스트.</param>
        /// <param name="_sendBuff">만들어진 패킷을 담을 버퍼</param>
        /// <param name="size">만들어진 패킷의 실제 사이즈</param>
        /// <param name="error">에러가 나면 에러의 메시지</param>
        /// <returns>성공이면 true, 실패면 false를 리턴하고 error에 원인을 담는다.</returns>
        public bool BuildMsg(String msg, Byte[] _sendBuff, out int totalSendSize, out String error)
        {
         
            error = "";

            String[] kinds = msg.Split("/".ToCharArray());
            totalSendSize = 0;
            Boolean unsigned = false;
            for (int i = 0; i < kinds.Length; i++)
            {
                String[] units;
                units = kinds[i].Split(",".ToCharArray());//형식은 type,값,값/type,값,값...형식으로 이루어짐.

                int typeSize = 4;
                if (units[units.Length - 1].Length == 0) units[units.Length - 1] = "0";
                String type = units[0].Trim().ToLower();
                Array packets = getArray(type, units, out typeSize, out unsigned);

                if (packets == null || packets.Length < 1 || typeSize < 0)
                {
                    error = "타입명세실패. 타입명세 후 값을 적으십시오.\r\n 형식: 타입,데이터,데이터/타입,데이터,데이터,데이터...\r\n ex> int,1,2,3/short,1,2,3";
                    return false;
                }
                //Byte[] buff;

                if (type.Equals("char") || type.Equals("string"))
                {
                    Buffer.BlockCopy(packets, 0, _sendBuff, totalSendSize, packets.Length);
                    totalSendSize += packets.Length;

                }
                else
                {
                    for (int k = 1; k < units.Length; k++)
                    {
                        TypeHandling.TypeName typeName = TypeHandling.getTypeKind(units[k]);

                        if (typeName == TypeHandling.TypeName.Integer)
                        {

                            if (unsigned == false)
                            {
                                if (typeSize == 1)
                                    packets.SetValue(SByte.Parse(units[k]), k - 1);
                                else if (typeSize == 2)
                                    packets.SetValue(Int16.Parse(units[k]), k - 1);
                                else if (typeSize == 4)
                                    packets.SetValue(Int32.Parse(units[k]), k - 1);
                                else
                                    packets.SetValue(Int64.Parse(units[k]), k - 1);
                            }
                            else
                            {
                                if (typeSize == 1)
                                    packets.SetValue(Byte.Parse(units[k]), k - 1);
                                else if (typeSize == 2)
                                    packets.SetValue(UInt16.Parse(units[k]), k - 1);
                                else if (typeSize == 4)
                                    packets.SetValue(UInt32.Parse(units[k]), k - 1);
                                else
                                    packets.SetValue(UInt64.Parse(units[k]), k - 1);
                            }
                        }
                        else if (typeName == TypeHandling.TypeName.HexString)
                        {

                            if (typeSize == 1) packets.SetValue((byte)TypeHandling.getHexNumber<byte>(units[k]), k - 1);
                            else if (typeSize == 2) packets.SetValue((ushort)TypeHandling.getHexNumber<ushort>(units[k]), k - 1);
                            else if (typeSize == 4) packets.SetValue((uint)TypeHandling.getHexNumber<uint>(units[k]), k - 1);
                            else packets.SetValue((UInt64)TypeHandling.getHexNumber<ulong>(units[k]), k - 1);
                        }
                        else if (typeName == TypeHandling.TypeName.Float)
                        {
                            if (typeSize == 4) packets.SetValue(float.Parse(units[k]), k - 1);
                            else packets.SetValue(Double.Parse(units[k]), k - 1);
                        }
                        else if (typeName == TypeHandling.TypeName.Bin)
                        {
                            if (typeSize == 1) packets.SetValue((byte)TypeHandling.getBinNumber(units[k]), k - 1);
                            else if (typeSize == 2) packets.SetValue((short)TypeHandling.getBinNumber(units[k]), k - 1);
                            else if (typeSize == 4) packets.SetValue((int)TypeHandling.getBinNumber(units[k]), k - 1);
                            else packets.SetValue((Int64)TypeHandling.getBinNumber(units[k]), k - 1);
                        }

                    }
                    //buff = new Byte[Marshal.SizeOf(packets.GetValue(0)) * packets.Length];
                    if (Endian == Endians.Big) Swaper.swapWithSize(packets, _sendBuff, typeSize, Buffer.ByteLength(packets), 0, totalSendSize);
                    else Buffer.BlockCopy(packets, 0, _sendBuff, totalSendSize, Buffer.ByteLength(packets));
                    totalSendSize += Buffer.ByteLength(packets);

                }
            }

            return true;
        }
*/
       
        /*
        /// <summary>
        /// int,1,2,3/short,1,2,3  형태의 메시지를 보고 알맞는 array를 생성해 돌려줌..
        /// </summary>
        /// <param name="type"></param>
        /// <param name="units"></param>
        /// <param name="typeSize"></param>
        /// <param name="unsigned"></param>
        /// <returns></returns>
        private Array getArray(string type, String[] units, out int typeSize, out Boolean unsigned)
        {
            Array packets = null;
            unsigned = false;
            if (type.Equals("int64"))
            {
                packets = new Int64[units.Length - 1];
                typeSize = 8;
            }
            else if (type.Equals("uint64"))
            {
                packets = new UInt64[units.Length - 1];
                typeSize = 8;
                unsigned = true;
            }
            if (type.Equals("uint"))
            {
                packets = new uint[units.Length - 1];
                typeSize = 4;
                unsigned = true;
            }
            else if (type.Equals("ushort"))
            {
                packets = new ushort[units.Length - 1];
                typeSize = 2;
                unsigned = true;
            }
            else if (type.Equals("int"))
            {
                packets = new int[units.Length - 1];
                typeSize = 4;
            }
            else if (type.Equals("short"))
            {
                packets = new short[units.Length - 1];
                typeSize = 2;
            }
            else if (type.Equals("byte") || type.Equals("uchar") || type.Equals("unsigned char") || type.Equals("1byte"))
            {
                packets = new byte[units.Length - 1];
                typeSize = 1;
                unsigned = true;
            }
            else if (type.Equals("sbyte"))
            {
                packets = new sbyte[units.Length - 1];
                typeSize = 1;
                unsigned = false;
            }
            else if (type.Equals("char") || type.Equals("string"))
            {
                if(IsStringWithNullEnd) units[1] += "\0";
                packets = StringEncoding.GetBytes(units[1]);
                typeSize = packets.Length;
                //packets.SetValue(packets.Length - 1, 0);
            }
            else if (type.Equals("long"))
            {
                packets = new Int64[units.Length - 1];
                typeSize = 8;
            }
            else if (type.Equals("float"))
            {
                packets = new float[units.Length - 1];
                typeSize = 4;
            }
            else if (type.Equals("double"))
            {
                packets = new Double[units.Length - 1];
                typeSize = 8;
            }
            else
            {
                typeSize = -1;
            }

            return packets;
        }
        */
        bool _isDynamicPacket = false;
        /// <summary>
        /// 패킷이 실시간으로 만들어지는 패킷인지 나타냄..
        /// </summary>
        public bool IsDynamicPacket
        {
            get { return _isDynamicPacket; }
            set { _isDynamicPacket = value; }
        }

        /// <summary>
        /// value의 string을 리턴하고, 그 값이 들어간 int 배열을 리턴한다.
        /// </summary>
        /// <param name="cPacketItem"></param>
        /// <param name="bufferToGetData"></param>
        /// <param name="values">값이 들어간 value들을 리턴한다.</param>
        /// <returns></returns>
        public static string GetValueString(CPacketItem cPacketItem, Byte[] bufferToGetData, int startOffset, int limitSize, out int[] values, string spliter=",")
        {
            string valueString;
            int typeSize = Marshal.SizeOf(cPacketItem.RealType);
            values = new int[cPacketItem.Length];

            if (cPacketItem.IsSwap)
            {
                valueString = "";
                
                int value;
                int len = 0;
                for (int i = 0; i < cPacketItem.Length; i++)
                {
                    if (i != 0) valueString += spliter;
                    int start = cPacketItem.ByteOffset + i * typeSize;
                    int offset = startOffset + start;
                    value = TypeArrayConverter.GetVariableSwapFromBuffer<int>(bufferToGetData, offset, cPacketItem.IsSwap, typeSize, typeSize);
                    values[i] = value;
                    valueString += value;
                }
                
            }
            else
            {
                valueString = "";
                int value;
                for (int i = 0; i < cPacketItem.Length; i++)
                {
                    if (i != 0) valueString += spliter;
                    int offset = startOffset + cPacketItem.ByteOffset + i * typeSize;
                    value = TypeArrayConverter.CopyBufferToVariable<int>(bufferToGetData, offset, typeSize, true, cPacketItem.IsSwap);  // TypeArrayConverter.GetVariableSwapFromBuffer<int>(bufferToGetData, cPacketItem.ByteOffset + i * typeSize, cPacketItem.IsSwap, typeSize, typeSize);
                    values[i] = value;
                    valueString += value;
                }
            }
            return valueString;
        }

        /// <summary>
        /// value의 string을 Hex로 리턴하고, 그 값이 들어간 int 배열을 리턴한다.
        /// </summary>
        /// <param name="cPacketItem"></param>
        /// <param name="bufferToGetData"></param>
        /// <param name="values">값이 들어간 value들을 리턴한다.</param>
        /// <returns></returns>
        public static string GetValueHex(CPacketItem cPacketItem, Byte[] bufferToGetData, int startOffset, int limitSize, out int[] values, string spliter = ",")
        {
            string valueString;
            int typeSize = Marshal.SizeOf(cPacketItem.RealType);
            values = new int[cPacketItem.Length];

            if (cPacketItem.IsSwap)
            {
                valueString = "";

                int value;
                int len = 0;
                for (int i = 0; i < cPacketItem.Length; i++)
                {
                    if (i != 0) valueString += spliter;
                    int start = cPacketItem.ByteOffset + i * typeSize;
                    int offset = startOffset + start;
                    value = TypeArrayConverter.GetVariableSwapFromBuffer<int>(bufferToGetData, offset, cPacketItem.IsSwap, typeSize, typeSize);
                    values[i] = value;
                    valueString += value.ToString("X"+(typeSize*2)+"");
                }

            }
            else
            {
                valueString = "";
                int value;
                for (int i = 0; i < cPacketItem.Length; i++)
                {
                    if (i != 0) valueString += spliter;
                    int offset = startOffset + cPacketItem.ByteOffset + i * typeSize;
                    value = TypeArrayConverter.CopyBufferToVariable<int>(bufferToGetData, offset, typeSize, true, cPacketItem.IsSwap);  // TypeArrayConverter.GetVariableSwapFromBuffer<int>(bufferToGetData, cPacketItem.ByteOffset + i * typeSize, cPacketItem.IsSwap, typeSize, typeSize);
                    values[i] = value;
                    valueString += value.ToString("X" + (typeSize * 2) + ""); ;
                }
            }
            return valueString;
        }


        public CPacketStruct Clone()
        {
            CPacketStruct clone = new CPacketStruct(new byte[this.PacketBuffer.Length]);
            foreach (CPacketItem item in Items)
            {
                clone.Items.Add(item.Clone());
            }
            return clone;
        }
    }

    public class PacketItemCollection : IList<CPacketItem>
    {
        CPacketStruct _parent;
        List<CPacketItem> _items = new List<CPacketItem>();
        int _totalSize = 0;

        internal PacketItemCollection(CPacketStruct parent)
        {
            _parent = parent;
            _totalSize = 0;
        }
        public int IndexOf(CPacketItem item)
        {
            return _items.IndexOf(item);
        }

        public void Insert(int index, CPacketItem item)
        {
            item.SetParent(_parent);
            _items.Insert(index, item);
            SortFromStart();
        }

        public void SortFromStart(ICollection<CPacketItem> items=null)
        {
            int offset = 0;
            _totalSize = 0;
            if (items == null) items = _items;//null은 기본값..

            foreach (CPacketItem item in items)
            {
                if (item == null) break; //끊어지면 offset이 의미가 없다.
                item.ByteOffset = offset;
                offset += item.TotalSize;
                _totalSize = offset;
            }
        }

        public void RemoveAt(int index)
        {
            _items[index].ByteOffset = -1;
            _items.RemoveAt(index);
            SortFromStart();
        }

        public CPacketItem this[int index]
        {
            get
            {
                return _items[index];
            }
            set
            {
                _items[index].ByteOffset = -1;
                _items[index] = value;
                _items[index].SetParent(_parent);
                SortFromStart();
            }
        }

        public void Add(CPacketItem item)
        {
            _items.Add(item);
            item.SetParent(_parent);
            item.ByteOffset = _totalSize;
            _totalSize += item.TotalSize;
            //SortFromStart();
                
        }

        public void Clear()
        {
            foreach (CPacketItem item in _items)
            {
                item.ByteOffset = -1;//초기화.
            }
            _items.Clear();
        }

        public bool Contains(CPacketItem item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(CPacketItem[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
            SortFromStart(array);
        }

        public int Count
        {
            get { return _items.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(CPacketItem item)
        {
            return _items.Remove(item);
        }

        public IEnumerator<CPacketItem> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        /// <summary>
        /// 기존의 모든 값을 지우고 list에서 가져와서 넣어준다.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="?"></param>
        public void CopyFrom(IList<CPacketItem> list)
        {
            if (list is PacketItemCollection)
            {
                if ((list as PacketItemCollection)._items.Equals(_items) == false)
                {
                    _items.Clear();
                    foreach (CPacketItem item in list)
                    {
                        _items.Add(item);
                        item.SetParent(_parent);
                    }
                }
            }
            else
            {
                _items.Clear();
                foreach (CPacketItem item in list)
                {
                    _items.Add(item);
                    item.SetParent(_parent);
                }
            }

            
            SortFromStart();
        }
    }

}
