using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using DataHandling;
using System.Collections;
using System.Runtime.InteropServices;
using NetworkPacket;
using XmlHandlers;
using CustomParser;

namespace XmlDesigner
{
    public class ScenarioXmlParser : StructXMLParser
    {
        public static void ItemsToXml(List<CPacketItem> items, String xmlFile, CStructInfo info)
        {
            XmlDocument xDoc = new XmlDocument();
            
            //List<CPacketItem> items = cstr.Items;

            XmlElement xRoot = xDoc.CreateElement("Packet");
            xDoc.AppendChild(xRoot);
            #region addInfo
            XmlElement xInfos = XmlAdder.Element(xDoc, "Infos", xRoot);
            
            XmlElement xInfo;
            xInfo = XmlAdder.Element(xDoc, "Info", xInfos);
            XmlAdder.Attribute(xDoc, "Comment", info.Comment, xInfos);
            xInfo = XmlAdder.Element(xDoc, "Info", xInfos);
            XmlAdder.Attribute(xDoc, "LastModified", info.LastModified.ToString(), xInfos);
            #endregion

            XmlElement xItems = XmlAdder.Element(xDoc, "Items", xRoot);

            for (int i = 0; i < items.Count; i++)
            {
                XmlElement item = xDoc.CreateElement("Item");
                XmlAdder.Attribute(xDoc, "Name", items[i].Name, item);
                XmlAdder.Attribute(xDoc, "Type", items[i].TypeString, item);
                XmlAdder.Attribute(xDoc, "IsSwap", items[i].IsSwap.ToString(), item);
                int size = items[i].Length;

                if (items[i].Function.Exists)
                {
                    XmlElement func = XmlAdder.Element(xDoc, "Function", item);
                    XmlAdder.Attribute(xDoc, "Name", items[i].Function.Name, func);
                    for(int argc=0; argc<items[i].Function.Args.Length; argc++){
                        XmlAdder.Element(xDoc, "Arg", items[i].Function.Args[argc].ToString(), func);
                    }

                }else if(items[i].InitValues.Length>0){
                    if(size==0) size = items[i].InitValues.Length;
                    
                    for(int itemCount=0; itemCount<size; itemCount++){
                        XmlAdder.Element(xDoc, "InitValue", items[i].InitValues[itemCount].ToString(), item);
                    }
                }
                
                XmlAdder.Attribute(xDoc, "Size", size.ToString(), item);
                xItems.AppendChild(item);
            }
            xDoc.Save(xmlFile);
        }
        public static void CodeToXml(String code, String xmlFile, CStructInfo info=null)
        {
            List<CPacketItem> items = ScenarioXmlParser.CodeToItems(code);

            ScenarioXmlParser.ItemsToXml(items, xmlFile, info);
        }

        public static List<CPacketItem> XmlToItems(String xmlFile, CStructInfo info=null)
        {
            List<CPacketItem> items = new List<CPacketItem>();

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(xmlFile);
            
            XmlNodeList xNodeList;

            if (info != null)
            {
                 xNodeList = xDoc.SelectNodes("//Packet//Infos//Info");
                 for (int i = 0; i < xNodeList.Count; i++)
                 {

                    String name = xNodeList.Item(i).Attributes["Name"].Value;
                    String value = xNodeList.Item(i).InnerText;
                    switch (name)
                    {
                        case "Comment":
                            info.Comment = value;
                            break;
                        case "LastModified":
                            info.SetModified(value);
                            break;
                    }
                 }
            }

            xNodeList = xDoc.SelectNodes("//Packet//Items//Item");
            
            for (int i = 0; i < xNodeList.Count; i++)
            {
                
                String name = xNodeList.Item(i).Attributes["Name"].Value;
                String type = xNodeList.Item(i).Attributes["Type"].Value;
                bool isSwap = false;
                try
                {
                    isSwap = xNodeList.Item(i).Attributes["IsSwap"].Value.ToLower().Equals("true");
                }
                catch { }
               int size = 0;
                try{
                    size = Convert.ToInt32(xNodeList.Item(i).Attributes["Size"].Value);
                }catch{}

                XmlNodeList initValues = XmlGetter.Children(xNodeList.Item(i), "InitValue");// xNodeList.Item(i).ChildNodes;

                CPacketItem item = new CPacketItem(name, type, size, "0", null);//default

                if(initValues.Count>1){
                    if(size==0) size = initValues.Count; //size가 지정되지 않았으면 초기설정개수와 동일하게 맞춘다.
                    List<String> initObjValues = new List<String>();

                    if (initValues.Count <= size)//정해진 배열크기보다 초기값설정이 적거나 일치할 때
                    {
                        for (int initCount = 0; initCount < initValues.Count; initCount++) //초기설정을 넣는다.
                        {
                            if (initValues.Item(initCount).Name.Equals("InitValue"))
                            {
                                initObjValues.Add(initValues.Item(initCount).InnerText);
                            }
                        }
                        for (int initCount = initValues.Count; initCount < size; initCount++)//초기설정이 모자라면 0으로 채운다.
                        {
                            if (initValues.Item(initCount).Name.Equals("InitValue"))
                            {
                                initObjValues.Add("0");
                            }
                        }
                    }
                    else//초기설정값이 더 많을 때..
                    {
                        for (int initCount = 0; initCount < size; initCount++) //초기설정을 size에서 정해진 만큼만 넣는다.
                        {
                            if (initValues.Item(initCount).Name.Equals("InitValue"))
                            {
                                initObjValues.Add(initValues.Item(initCount).InnerText);
                            }
                        }
                    }
                    
                    item = new CPacketItem(name, type, size, initObjValues.ToArray());
                    
                    
                    
                }
                XmlNode function = XmlGetter.Child(xNodeList.Item(i), "Function");// xNodeList.Item(i).ChildNodes;
                if(function!=null)
                {
                    FunctionInfo func = new FunctionInfo();
                    
                    if (initValues.Item(0).Name.Equals("Function"))
                    {
                        XmlNodeList args = initValues.Item(0).ChildNodes;
                        List<String> argList = new List<string>();
                        String funcName = initValues.Item(0).Attributes["Name"].Value;
                        for (int argc = 0; argc < args.Count; argc++)
                        {
                            argList.Add(args[argc].InnerText);
                        }
                        String[] argArray;
                        if (argList.Count > 0) argArray = argList.ToArray();
                        else argArray = null;
                        if (FunctionsList.ContainsKey(funcName))
                        {
                            func = new FunctionInfo(FunctionsList[funcName], argArray);
                        }
                        else func = new FunctionInfo(funcName, null, argArray);//
                    }
                    if (func.Exists)
                    {
                        item = new CPacketItem(name, type, size, func, null);
                    }
                    else
                    {

                        String initObj = initValues.Item(0).InnerText;
                        item = new CPacketItem(name, type, size, initObj, null);
                    }
                }

                item.IsSwap = isSwap;
                items.Add(item);
                
            }
            
            return items;

        }

        public static String XmlToCode(String xmlFile, CStructInfo info=null)
        {
            List<CPacketItem> list = XmlToItems(xmlFile, info);
            return ItemsToCode(list);
        }

        public static String ItemsToCode(List<CPacketItem> items)
        {
            String code = "";
            for (int i = 0; i < items.Count; i++)
            {
                code += items[i].TypeString;
                
                code+=" ";
                if (items[i].IsSwap) code += "swap@";
                code += items[i].Name;
                if (items[i].Length > 1) code += "[" + items[i].Length + "]";
                code+=" = ";
                
                if (items[i].Length == 1)
                {
                    code += items[i].InitString;
                }
                else if (items[i].InitValues.Length > 1)
                {
                    code += " { ";
                    for (int initCount = 0; initCount < items[i].InitValues.Length; initCount++)
                    {
                        if (initCount != 0) code += ",";
                        code += items[i].InitValues[initCount];
                    }
                    code += "}";
                }
                code += ";\r\n";
            }
            return code;
        }
        
        static String removeOuterWhiteSpace(String line)
        {
            line = line.Replace("\n", "");
            line = line.Replace("\r", "");
            line = line.Replace("\0", "");
            line = line.Trim();//빈칸과 라인피드를 모두 삭제
            return line;
        }
        
        static String setError(string errMsg, int line, String lineStr)
        {
            String err = errMsg + " Line(" + line + ") in \r\n" + lineStr;
            throw new Exception(err);
        }

        public static List<CPacketItem> CodeToItems(String str)
        {
            List<CPacketItem> items = new List<CPacketItem>();
            /*
            names.Clear();
            types.Clear();
            sizes.Clear();
            initStr.Clear();
            initValues.Clear();
             */
            
            
            String[] lines = str.Split(";".ToCharArray()); //각 라인으로 나누어서
            String[] initValues;
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

                int arrSize = 1; //arrSize가 1이면 단순값이다.

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
                initValues = new String[arrSize];
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
                        initValues[j] = token[j];
                        TypeHandling.getValueAndType(token[j], ref intValue, ref doubleValue, ref strValue);
                        /*
                        switch (TypeHandling.getValueAndType(token[j], ref intValue, ref doubleValue, ref strValue))
                        {//hex나 oct형식등을 모두 숫자형으로 먼저 치환한다.
                            case TypeHandling.TypeName.Integer:
                                if (isSwap)
                                {
                                    if (TypeHandling.getTypeFromTypeName(type) == typeof(byte)) initValues[j] = Swaper.swap<byte>((byte)intValue).ToString();
                                    else if (TypeHandling.getTypeFromTypeName(type) == typeof(short)) initValues[j] = Swaper.swap<short>((short)intValue).ToString();
                                    else if (TypeHandling.getTypeFromTypeName(type) == typeof(int)) initValues[j] = Swaper.swap<int>((int)intValue).ToString();
                                    else if (TypeHandling.getTypeFromTypeName(type) == typeof(long)) initValues[j] = Swaper.swap<long>((long)intValue).ToString();
                                    else if (TypeHandling.getTypeFromTypeName(type) == typeof(ushort)) initValues[j] = Swaper.swap<ushort>((ushort)intValue).ToString();
                                    else if (TypeHandling.getTypeFromTypeName(type) == typeof(uint)) initValues[j] = Swaper.swap<uint>((uint)intValue).ToString();
                                    else if (TypeHandling.getTypeFromTypeName(type) == typeof(ulong)) initValues[j] = Swaper.swap<ulong>((ulong)intValue).ToString();
                                }
                                else
                                {
                                    initValues[j] = intValue.ToString();
                                }
                                break;
                            case TypeHandling.TypeName.Float:
                                initValues[j] = doubleValue.ToString();
                                break;
                            case TypeHandling.TypeName.String:
                                initValues[j] = strValue;
                                break;
                        }
                         */
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
                    if (initValue[0]!='-' && Char.IsDigit(initValue[0])==false && initValue[0]!='\"')//첫글자가 문자로 시작하고, 따옴표(")로시작하지 않으면 함수나 변수이거나 문자열이다. 
                    {
                        int argstart = initValue.IndexOf("(");
                        int argend = initValue.LastIndexOf(")");
                        
                        if(argstart<0 && argend<0 && initValue.IndexOf("@")==0){//변수임.
                            items[i].Var = new VariableInfo(initValue);
                        
                        }else if(argstart>1 && argend==initValue.Length-1)
                        {//함수임.
                            String funcName = initValue.Substring(0, argstart);
                            String[] args = initValue.Substring(argstart + 1, argend - 1 - argstart).Split(",".ToCharArray());
                            if (FunctionsList.ContainsKey(funcName))
                            {
                                items[i].Function = new FunctionInfo(FunctionsList[funcName], args);
                            }
                            else
                            {
                                items[i].Function = new FunctionInfo(funcName, null, args);
                            }
                        
                        }else if(type.Equals("string")){
                            initValue = initValue.Replace("\\\"", "@'aAiIOo~{|\\]~");//\"를 구분하기 위해 모두 특수한 문자로 바꾸어준다.
                            initValue = initValue.Replace("\"", "");
                            initValue = initValue.Replace("@'aAiIOo~{|\\]~", "\\\"");//\"를 다시 복구한다.

                            initValues[0] = initValue;//따옴표를 지우고 넣어준다.
                        }else{
                            setError("함수정의가 맞지 않습니다. 괄호가 완성되지 않았습니다.", i, line);
                            return null;
                        }
                    }
                    else
                    {
                        
                        if(type.Equals("sring")){
                            initValue = initValue.Replace("\\\"", "@'aAiIOo~{|\\]~");//\"를 구분하기 위해 모두 특수한 문자로 바꾸어준다.
                            initValue = initValue.Replace("\"", "");
                            initValue = initValue.Replace("@'aAiIOo~{|\\]~","\\\"");//\"를 다시 복구한다.
                            
                            initValues[0] = initValue;//따옴표를 지우고 넣어준다.
                        }
                        else if (initValue.Length == 0) initValues = new String[] { "0" };
                        else
                        {
                            for (int initc = 0; initc < arrSize; initc++)
                            {
                                initValues[initc] = initValue; //모두 같은 값으로 채워줌.
                            }
                        }
                    }
                    #region old
                    /*
                    switch (TypeHandling.getValueAndType(initValue, ref intValue, ref doubleValue, ref strValue))
                    {//hex나 oct형식등을 모두 숫자형으로 먼저 치환한다.
                        case TypeHandling.TypeName.Integer:
                            //initValues[i][0] = intValue;
                            if (isSwap)
                            {
                                if (TypeHandling.getTypeFromTypeName(type) == typeof(byte)) initValues[0] = Swaper.swap<byte>((byte)intValue).ToString();
                                else if (TypeHandling.getTypeFromTypeName(type) == typeof(short)) initValues[0] = Swaper.swap<short>((short)intValue).ToString();
                                else if (TypeHandling.getTypeFromTypeName(type) == typeof(int)) initValues[0] = Swaper.swap<int>((int)intValue).ToString();
                                else if (TypeHandling.getTypeFromTypeName(type) == typeof(long)) initValues[0] = Swaper.swap<long>((long)intValue).ToString();
                                else if (TypeHandling.getTypeFromTypeName(type) == typeof(ushort)) initValues[0] = Swaper.swap<ushort>((ushort)intValue).ToString();
                                else if (TypeHandling.getTypeFromTypeName(type) == typeof(uint)) initValues[0] = Swaper.swap<uint>((uint)intValue).ToString();
                                else if (TypeHandling.getTypeFromTypeName(type) == typeof(ulong)) initValues[0] = Swaper.swap<ulong>((ulong)intValue).ToString();
                            }
                            else
                            {
                                initValues[0] = intValue.ToString();
                            }
                            break;
                        case TypeHandling.TypeName.Float:
                            initValues[0] = doubleValue.ToString();
                            break;
                        case TypeHandling.TypeName.String:
                            initValues[0] = strValue;
                            break;
                    }
                     * */
                    #endregion
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
                CPacketItem item = new CPacketItem(rest, type, arrSize, initValues);
                item.IsSwap = isSwap;
                //item.InitString = initValue;
                items.Add(item);
                /*
                ns.names.Add(rest);
                ns.sizes.Add(arrSize);
                ns.types.Add(type);
                ns.isSwap.Add(isSwap);
                ns.initStr.Add(initValue);
                 */

            }//각 라인들 검색 끝
            
            return items;
        }
        /// <summary>
        /// string들의 열거를 param string[]형식으로 받는 함수의 delegate.
        /// string[] 을 리턴한다.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public delegate String[] InitVarFunc(params string[] obj);

        String[] runFunction(Dictionary<String, InitVarFunc> funcList, Dictionary<String,VariableInfo> vars, FunctionInfo funcInfo)
        {
            string[] args = funcInfo.Args;
            List<String> newArg = new List<string>();
            for (int i = 0; i < args.Length; i++) //argument가 변수일때 바꾸어줌..
            {
                String refName = args[i];
                if (vars.Keys.Contains(refName))
                {
                    if (vars[refName].Values != null && vars[refName].Values.Length > 0)
                    {
                        for (int a = 0; a < vars[refName].Values.Length; a++) //여러개의 argument라도 추가가 된다.
                        {
                            newArg.AddRange(vars[refName].Values);
                        }
                    }
                    else
                    {
                        throw new Exception("변수" + refName + "가 존재하지 않거나 값이 설정되지 않았습니다.");
                    }
                }
                else
                {
                    newArg.Add(args[i]);
                }
            }
            return funcList[funcInfo.Name](newArg.ToArray());
        }

        public void ConvertFuncVarsToConst(List<CPacketItem> items, Dictionary<String, InitVarFunc> funcList, Dictionary<String, VariableInfo> varList)
        {
            for (int i = 0; i < items.Count; i++)
            {
                //Byte[] buff;
                if (items[i].Function.Exists)
                {
                    items[i].InitValues = runFunction(funcList, varList, items[i].Function);
                }
                else if (items[i].Var.Exists) //변수의 내용을 사용하도록 셋팅되어있다면
                {
                    if(items[i].Var.ValueExists){//이미 값이 셋팅되었다. ref이기때문에 변수가 변하면 동적으로 변하게 된다.
                        items[i].InitValues = items[i].Var.Values;
                    }else{//값이 셋팅되어있지 않다면 변수들에서 찾아서 셋팅한다.
                        String refName = items[i].Var.Name; //
                        /*
                        for(int itm=0; itm<i; itm++){//먼저 자기 자신의 아이템에서 돌면서 값이 셋팅된 해당 이름의 변수가 있는지 찾아본다. 해당 item의 앞에 셋팅된 것만 가져올 수 있다.
                            if(items[itm].name == refName){//이름이같은 item을 찾는다.
                                if(items[itm].InitValues!=null && items[itm].InitValues.Length>0) //해당 이름의 item요소가 있고, 내용이 존재한다면
                                    items[i].InitValues = items[itm].InitValues;//그 내용을 그대로 가져옴(ref copy)
                                break;
                            }
                        }
                         */
                        if(items[i].InitValues==null || items[i].InitValues.Length==0){ //아직 비어있다는 것은 해당 이름의 item요소를 못찾았다는 뜻이다.
                            //변수에서 찾아온다.
                            if(varList.Keys.Contains(refName)){
                                items[i].InitValues = varList[refName].Values;
                            }
                        }
                        if (items[i].InitValues == null || items[i].InitValues.Length == 0) //아직도 비어있다면 변수에서 찾지 못했다는 뜻이다.
                        {
                            throw new Exception("변수 " + refName + " 의 값을 찾을 수 없습니다.");
                        }

                    }
                }
            }
        }
       

        /// <summary>
        /// Items의 내용을 바꾸어 packet으로 변경한다.
        /// Send할 패킷을 만드는 데 사용된다.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="funcList">사용될 function의 리스트. function은 String들을 argument로 받고, String[]을 리턴하는 function들이다.</param>
        /// <param name="varList">사용될 variables의 리스트. 같은 CPacketItem의 다른 item들을 찾아보고, 찾을 수 없다면 이 리스트에서 찾는다.</param>
        /// <param name="swap">전체적으로 swap을 할 것인지를 묻는다. 이 arguemnt가 true인데 단일 item이 IsSwap에 true가 되어있다면 다른 것과는 반대로 동작한다. 즉, swap되지 않는다</param>
        /// <param name="dataBuff">패킷데이터가 저장될 버퍼</param>
        /// <param name="dataSize">전체 크기</param>
        /// <returns></returns>
        public void ItemsToPacket(List<CPacketItem> items, Dictionary<String, InitVarFunc> funcList, Dictionary<String, VariableInfo> varList, bool swap, Byte[] dataBuff, out int dataSize)
        {

            dataSize = 0;
            Boolean unsigned = false;
            Dictionary<String, VariableInfo> vars = new Dictionary<string, VariableInfo>();// = new Dictionary<string, string[]>(varList);
            for (int i = 0; i < items.Count; i++) //static 하게 정의된 item의 요소는 변수로 사용가능하다.
            {
                if (items[i].InitValues.Length > 0)
                {
                    vars.Add(items[i].Name, new VariableInfo(items[i].Name, items[i].InitValues));
                }
            }
            for (int i = 0; i < varList.Count; i++)
            {
                if (vars.Keys.Contains(varList.Keys.ElementAt(i))) continue;//같은 변수명이면 같은 패킷에 속한 것이 적용된다.
                else vars.Add(varList.Keys.ElementAt(i), varList.Values.ElementAt(i));
            }


            for (int i = 0; i < items.Count; i++)
            {

                int typeSize = 4;

                String type = items[i].TypeString;
                Array packets = TypeHandling.GetArrayByTypeName(type, items[i].Length, out typeSize, out unsigned);

                //Byte[] buff;
                if (items[i].Function.Exists)
                {
                    items[i].InitValues = runFunction(funcList, vars, items[i].Function);
                }
                else if (items[i].Var.Exists)
                {
                    String refName = items[i].Var.Name;
                    if (vars.Keys.Contains(refName))
                    {
                        if (vars[refName].Values != null && vars[refName].Values.Length > 0)
                        {
                            items[i].InitValues = vars[items[i].Var.Name].Values;
                        }
                        else
                        {
                            throw new Exception("변수" + refName + "가 존재하지 않거나 값이 설정되지 않았습니다.");
                        }
                    }
                }
                
                if (type.Equals("char") || type.Equals("string"))
                {
                    if (items[i].InitString == null || items[i].InitString.Length == 0) continue;
                    packets = Encoding.UTF8.GetBytes(items[i].InitString);
                    Buffer.BlockCopy(packets, 0, dataBuff, dataSize, packets.Length);
                    dataSize += packets.Length;
                }
                else
                {
                    for (int k = 0; k < items[i].InitValues.Length; k++)
                    {
                        TypeHandling.TypeName typeName = TypeHandling.getTypeKind(items[i].InitValues[k]);
                        if (typeName == TypeHandling.TypeName.Integer)
                        {

                            if (unsigned == false)
                            {
                                if (typeSize == 1)
                                    packets.SetValue(SByte.Parse(items[i].InitValues[k]), k);
                                else if (typeSize == 2)
                                    packets.SetValue(Int16.Parse(items[i].InitValues[k]), k);
                                else if (typeSize == 4)
                                    packets.SetValue(Int32.Parse(items[i].InitValues[k]), k);
                                else
                                    packets.SetValue(Int64.Parse(items[i].InitValues[k]), k);
                            }
                            else
                            {
                                if (typeSize == 1)
                                    packets.SetValue(Byte.Parse(items[i].InitValues[k]), k);
                                else if (typeSize == 2)
                                    packets.SetValue(UInt16.Parse(items[i].InitValues[k]), k);
                                else if (typeSize == 4)
                                    packets.SetValue(UInt32.Parse(items[i].InitValues[k]), k);
                                else
                                    packets.SetValue(UInt64.Parse(items[i].InitValues[k]), k);
                            }
                        }
                        else if (typeName == TypeHandling.TypeName.HexString)
                        {
                            if (typeSize == 1) packets.SetValue(TypeHandling.getHexNumber<byte>(items[i].InitValues[k]), k);
                            else if (typeSize == 2) packets.SetValue(TypeHandling.getHexNumber<ushort>(items[i].InitValues[k]), k);
                            else if (typeSize == 4) packets.SetValue(TypeHandling.getHexNumber<uint>(items[i].InitValues[k]), k);
                            else packets.SetValue(TypeHandling.getHexNumber<ulong>(items[i].InitValues[k]), k);
                        }
                        else if (typeName == TypeHandling.TypeName.Float)
                        {
                            if (typeSize == 4) packets.SetValue(float.Parse(items[i].InitValues[k]), k);
                            else packets.SetValue(Double.Parse(items[i].InitValues[k]), k);
                        }
                        else if (typeName == TypeHandling.TypeName.Bin)
                        {
                            if (typeSize == 1) packets.SetValue((byte)TypeHandling.getBinNumber(items[i].InitValues[k]), k);
                            else if (typeSize == 2) packets.SetValue((short)TypeHandling.getBinNumber(items[i].InitValues[k]), k);
                            else if (typeSize == 4) packets.SetValue((int)TypeHandling.getBinNumber(items[i].InitValues[k]), k);
                            else packets.SetValue((Int64)TypeHandling.getBinNumber(items[i].InitValues[k]), k);
                        }


                    }
                    //buff = new Byte[Marshal.SizeOf(packets.GetValue(0)) * packets.Length];
                    if (swap)//전체 swap
                    {
                        if (items[i].IsSwap==false) Swaper.swapWithSize(packets, dataBuff, typeSize, Buffer.ByteLength(packets), 0, dataSize);
                        else Buffer.BlockCopy(packets, 0, dataBuff, dataSize, Buffer.ByteLength(packets));
                    }
                    else //각각 swap
                    {
                        if (items[i].IsSwap) Swaper.swapWithSize(packets, dataBuff, typeSize, Buffer.ByteLength(packets), 0, dataSize);
                        else Buffer.BlockCopy(packets, 0, dataBuff, dataSize, Buffer.ByteLength(packets));
                    }
                    dataSize += Buffer.ByteLength(packets);

                }
            }
            /*
            if(_sendBuff.Equals(PacketBuffer)){ //만일 _sendBuff가 Packet버퍼와 같을 때, 사이즈를 조정한다.(초기사이즈->실제사이즈)
                Byte[] buff = new Byte[totalSendSize];
                Buffer.BlockCopy(_sendBuff, 0, buff, 0, totalSendSize);
                PacketBuffer = buff;
            }
            */
        }

        public static bool XmlToPacket(String xml, bool swap, Byte[] sendBuff, out int totalSendSize)
        {
            return ItemsToPacket(XmlToItems(xml), swap, sendBuff, out totalSendSize);
        }

        public static bool CodeToPacket(String msg, bool swap, Byte[] sendBuff, out int totalSendSize)
        {
            return ItemsToPacket(CodeToItems(msg), swap, sendBuff, out totalSendSize);
        }

        public static bool ItemsToPacket(List<CPacketItem> items, bool swap, Byte[] sendBuff, out int totalSendSize)
        {

            

            totalSendSize = 0;
            Boolean unsigned = false;

            for (int i = 0; i < items.Count; i++)
            {
                int typeSize = 4;
                String type = items[i].TypeString;
                Array packets = TypeHandling.GetArrayByTypeName(type, items[i].Length, out typeSize, out unsigned);

                if (packets == null || packets.Length < 1 || typeSize < 0)
                {
                    String error = "타입명세실패";
                    throw new Exception(error);
                }


                if (type.Equals("char") || type.Equals("string"))
                {
                    if (items[i].InitString == null || items[i].InitString.Length == 0) continue;
                    packets = Encoding.UTF8.GetBytes(items[i].InitString);
                    Buffer.BlockCopy(packets, 0, sendBuff, totalSendSize, packets.Length);
                    totalSendSize += packets.Length;
                }
                else
                {
                    String[] units = items[i].InitValues;
                    for (int k = 0; k < units.Length; k++)
                    {
                        TypeHandling.TypeName typeName = TypeHandling.getTypeKind(units[k]);

                        if (typeName == TypeHandling.TypeName.Integer)
                        {

                            if (unsigned == false)
                            {
                                if (typeSize == 1)
                                    packets.SetValue(SByte.Parse(units[k]), k);
                                else if (typeSize == 2)
                                    packets.SetValue(Int16.Parse(units[k]), k);
                                else if (typeSize == 4)
                                    packets.SetValue(Int32.Parse(units[k]), k);
                                else
                                    packets.SetValue(Int64.Parse(units[k]), k);
                            }
                            else
                            {
                                if (typeSize == 1)
                                    packets.SetValue(Byte.Parse(units[k]), k);
                                else if (typeSize == 2)
                                    packets.SetValue(UInt16.Parse(units[k]), k);
                                else if (typeSize == 4)
                                    packets.SetValue(UInt32.Parse(units[k]), k);
                                else
                                    packets.SetValue(UInt64.Parse(units[k]), k);
                            }
                        }
                        else if (typeName == TypeHandling.TypeName.HexString)
                        {
                            if (unsigned)
                            {
                                if (typeSize == 1) packets.SetValue((byte)TypeHandling.getHexNumber<byte>(units[k]), k);
                                else if (typeSize == 2) packets.SetValue((ushort)TypeHandling.getHexNumber<ushort>(units[k]), k);
                                else if (typeSize == 4) packets.SetValue((uint)TypeHandling.getHexNumber<uint>(units[k]), k);
                                else packets.SetValue((UInt64)TypeHandling.getHexNumber<ulong>(units[k]), k);
                            }
                            else
                            {
                                if (typeSize == 1) packets.SetValue((sbyte)TypeHandling.getHexNumber<sbyte>(units[k]), k);
                                else if (typeSize == 2) packets.SetValue((short)TypeHandling.getHexNumber<short>(units[k]), k);
                                else if (typeSize == 4) packets.SetValue((int)TypeHandling.getHexNumber<int>(units[k]), k);
                                else packets.SetValue((Int64)TypeHandling.getHexNumber<long>(units[k]), k);
                            }
                        }
                        else if (typeName == TypeHandling.TypeName.Float)
                        {
                            if (typeSize == 4) packets.SetValue(float.Parse(units[k]), k);
                            else packets.SetValue(Double.Parse(units[k]), k);
                        }
                        else if (typeName == TypeHandling.TypeName.Bin)
                        {
                            if (typeSize == 1) packets.SetValue((byte)TypeHandling.getBinNumber(units[k]), k);
                            else if (typeSize == 2) packets.SetValue((short)TypeHandling.getBinNumber(units[k]), k);
                            else if (typeSize == 4) packets.SetValue((int)TypeHandling.getBinNumber(units[k]), k);
                            else packets.SetValue((Int64)TypeHandling.getBinNumber(units[k]), k);
                        }

                    }
                    //buff = new Byte[Marshal.SizeOf(packets.GetValue(0)) * packets.Length];
                    if (swap) //전체 swap
                    {
                        if (items[i].IsSwap==false) Swaper.swapWithSize(packets, sendBuff, typeSize, Buffer.ByteLength(packets), 0, totalSendSize);
                        else Buffer.BlockCopy(packets, 0, sendBuff, totalSendSize, Buffer.ByteLength(packets));
                    }
                    else //일부만 swap
                    {
                        if (items[i].IsSwap) Swaper.swapWithSize(packets, sendBuff, typeSize, Buffer.ByteLength(packets), 0, totalSendSize);
                        else Buffer.BlockCopy(packets, 0, sendBuff, totalSendSize, Buffer.ByteLength(packets));
                    }
                    totalSendSize += Buffer.ByteLength(packets);

                }
            }
            /*
            if(_sendBuff.Equals(PacketBuffer)){ //만일 _sendBuff가 Packet버퍼와 같을 때, 사이즈를 조정한다.(초기사이즈->실제사이즈)
                Byte[] buff = new Byte[totalSendSize];
                Buffer.BlockCopy(_sendBuff, 0, buff, 0, totalSendSize);
                PacketBuffer = buff;
            }
            */
            return true;
        }

        
    
    }
}
