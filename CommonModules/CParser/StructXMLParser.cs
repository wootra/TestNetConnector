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

namespace CustomParser
{
    public class StructXMLParser
    {
        public static Dictionary<String, VariableInfo> VariablesList = new Dictionary<string, VariableInfo>();
        public static Dictionary<String, FunctionInfo> FunctionsList = new Dictionary<string, FunctionInfo>();

        public static void ItemsToXml(IList<CPacketItem> items, String xmlFile, CStructInfo info)
        {
            XmlDocument xDoc = new XmlDocument();
            XmlElement xRoot = xDoc.CreateElement("Packet");
            xDoc.AppendChild(xRoot);
            ItemsToXml(items, xDoc, xRoot, info);
            xDoc.Save(xmlFile);
            //List<CPacketItem> items = cstr.Items;
        }
        public static void ItemsToXml(IList<CPacketItem> items, XmlDocument xDoc, XmlNode xRoot=null, CStructInfo info=null)
        {
            
            #region addInfo
            
            if (info != null)
            {
                XmlElement xInfos = XmlAdder.Element(xDoc, "Infos", xRoot);
                XmlElement xInfo;
                xInfo = XmlAdder.Element(xDoc, "Info", xInfos);
                XmlAdder.Attribute(xDoc, "Comment", info.Comment, xInfos);
                xInfo = XmlAdder.Element(xDoc, "Info", xInfos);
                XmlAdder.Attribute(xDoc, "LastModified", info.LastModified.ToString(), xInfos);

            }
            #endregion
            XmlElement xItems = XmlAdder.Element(xDoc, "Items", xRoot);

            for (int i = 0; i < items.Count; i++)
            {
                XmlElement item = XmlAdder.Element(xDoc, "Item", xItems);// xDoc.CreateElement("Item");
                XmlAdder.Attribute(xDoc, "Name", items[i].Name, item);
                XmlAdder.Attribute(xDoc, "Type", items[i].TypeString, item);
                XmlAdder.Attribute(xDoc, "IsSwap", (items[i].IsSwap)?"True":"False", item);
                //XmlAdder.Attribute(xDoc, "Visible", (items[i].Visible) ? "True" : "False", item);
                XmlAdder.Attribute(xDoc, "ShowOnReport", (items[i].ShowOnReport) ? "True" : "False", item);
                if (items[i].PassCondition.Length > 0) XmlAdder.Attribute(xDoc, "PassCondition", items[i].PassCondition.Replace("<", "&lt;").Replace(">", "&gt;"), item);

                if(items[i].Description.Length>0) XmlAdder.Attribute(xDoc, "Comment", (items[i].Description), item);
                int size = items[i].Length;
                if (items[i].Var != null)
                {
                    XmlAdder.Attribute(xDoc, "Var", (items[i].Var.Name), item);//Var는 변수이므로 이름만 저장하고, 있으면 복구하고, 없으면 비워놓는다.
                }
                /*
                if (items[i].Function != null)
                {
                    XmlNode function = XmlAdder.Element(xDoc, "Function", items[i].Function.Name, item);
                    if (items[i].Function.Args!=null && items[i].Function.Args.Length > 0)
                    {
                        foreach (String arg in items[i].Function.Args)
                        {
                            XmlAdder.Element(xDoc, "Arg", arg, function);
                        }
                    }
                }
                else if (items[i].Var != null)
                {
                    XmlAdder.Attribute(xDoc, "Var", (items[i].Var.Name), item);//Var는 변수이므로 이름만 저장하고, 있으면 복구하고, 없으면 비워놓는다.
                }
                else if (items[i].InitValues != null && items[i].InitValues.Length > 0)
                {
                    if (size == 0) size = items[i].InitValues.Length;

                    for (int itemCount = 0; itemCount < size; itemCount++)
                    {
                        XmlAdder.Element(xDoc, "InitValue", items[i].InitValues[itemCount].ToString(), item);
                    }
                }
                */
                if(items[i].PacketData.Count>0){
                    
                    foreach (String dataName in items[i].PacketData.Keys)
                    {
                        XmlElement packetData = XmlAdder.Element(xDoc, "PacketData",item);
                        String value = items[i].PacketData[dataName].Value;
                        XmlAdder.Attribute(xDoc, "Name", dataName, packetData);
                        XmlAdder.Attribute(xDoc, "Value", value, packetData);
                    }
                
                }
                
                if (items[i].Function!=null && items[i].Function.Exists)
                {
                    XmlElement func = XmlAdder.Element(xDoc, "Function", item);
                    XmlAdder.Attribute(xDoc, "Name", items[i].Function.Name, func);
                    if (items[i].Function.Args != null && items[i].Function.Args.Length > 0)
                    {
                        for (int argc = 0; argc < items[i].Function.Args.Length; argc++)
                        {
                            XmlAdder.Element(xDoc, "Arg", items[i].Function.Args[argc].ToString(), func);
                        }
                    }
                }
                else if (items[i].InitValues!=null && items[i].InitValues.Length > 0)
                {
                    if(size==0) size = items[i].InitValues.Length;
                    
                    for(int itemCount=0; itemCount<size; itemCount++){
                        
                        XmlAdder.Element(xDoc, "InitValue", items[i].InitValues[itemCount].ToString(), item);
                    }
                }
                
                XmlAdder.Attribute(xDoc, "Size", size.ToString(), item);

                if (items[i].BitItems.Count > 0)
                {
                    XmlNode bitItems = XmlAdder.Element(xDoc, "BitItems", item);
                    foreach (BitItem bItem in items[i].BitItems)
                    {
                        bItem.GetXml(xDoc, bitItems);
                    }
                }

                //xItems.AppendChild(item);
            }
            //
        }

        /// <summary>
        /// 리스트에서 크기를 가져와서 offset을 계산에 offsetDic에 넣어준다.
        /// 총 크기를 리턴한다.
        /// </summary>
        /// <param name="offsetDic"></param>
        /// <param name="autoFields"></param>
        /// <returns></returns>
        public static int CalculateOffsets(IList<CPacketItem> autoFields, Dictionary<CPacketItem, int> offsetDic)
        {
            offsetDic.Clear();
            int totalSize = 0;
            if (autoFields.Count > 0)
            {
                int offset = 0;
                foreach (CPacketItem packetItem in autoFields) //row
                {
                    offsetDic.Add(packetItem, offset);
                    offset += packetItem.Length * DataHandling.TypeHandling.getTypeSize(packetItem.TypeString);
                }
                totalSize = offset;
            }
            return totalSize;
        }

        public static void CodeToXml(String code, String xmlFile, CStructInfo info=null)
        {
            List<CPacketItem> items = StructXMLParser.CodeToItems(code, null);

            StructXMLParser.ItemsToXml(items, xmlFile, info);
        }
        
        /// <summary>
        /// Xml에서 Item들을 가져올 때 참조할 변수 리스트이다.
        /// </summary>
        

        public static IList<CPacketItem> XmlToItems(String xmlFile, CPacketStruct parser, CStructInfo info=null)
        {
            
            XmlDocument xDoc;// = new XmlDocument();
            //xDoc.Load(xmlFile);
            XmlNode root = XmlGetter.RootNode(out xDoc, xmlFile);
            return XmlToItems(root, parser, info);

        }

        public static IList<CPacketItem> XmlToItems(XmlNode root, CPacketStruct parser, CStructInfo info=null)
        {
            XmlNodeList xNodeList;
            PacketItemCollection items = new PacketItemCollection(parser);

            if (parser != null)
            {
                parser.Items = items;
                parser.IsDynamicPacket = false;
            }

            if (info != null)
            {
                xNodeList = XmlGetter.Children(root, "Infos/Info");// xDoc.SelectNodes("//Packet//Infos//Info");
                 //for (int i = 0; i < xNodeList.Count; i++)
                if (xNodeList != null)
                {
                    foreach (XmlNode child in xNodeList)
                    {

                        String name = XmlGetter.Attribute(child, "Name");// xNodeList.Item(i).Attributes["Name"].Value;
                        String value = XmlGetter.InnerText(child);// xNodeList.Item(i).InnerText;
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
            }

            xNodeList = XmlGetter.Children(root, "Items/Item");// xDoc.SelectNodes("//Packet//Items//Item");
            
            //for (int i = 0; i < xNodeList.Count; i++)
            foreach (XmlNode child in xNodeList)  //Item
            {
                CPacketItem item = new CPacketItem();
                #region attributes of Item
                item.Name = XmlGetter.Attribute(child, "Name").Trim();// xNodeList.Item(i).Attributes["Name"].Value;
                item.SetType(XmlGetter.Attribute(child, "Type").Trim(), false);// xNodeList.Item(i).Attributes["Type"].Value;
                //item.Visible = XmlGetter.Attribute(child, "Visible").Trim().ToLower().Equals("false") == false;//기본값 true
                String varName = XmlGetter.Attribute(child, "Var").Trim();
                item.ShowOnReport = XmlGetter.Attribute(child, "ShowOnReport").ToLower().Equals("false") == false;//기본값 true
                item.PassCondition = XmlGetter.Attribute(child, "PassCondition").Replace("&lt;","<").Replace("&gt;",">");//기본값 true
                if (varName.Length > 0)
                {
                    if (parser == null)
                    {
                        if (VariablesList != null && VariablesList.ContainsKey(varName))
                        {
                            item.InitValues = VariablesList[varName].Values;
                            //if (parser != null) parser.IsDynamicPacket = true;
                        }
                        else
                        {

                            item.InitValues = new String[item.Length];
                            //item.Var = null;
                        }
                    }
                    else
                    {
                        if (VariablesList != null && VariablesList.ContainsKey(varName))
                        {
                            item.Var = VariablesList[varName];
                            if (parser != null) parser.IsDynamicPacket = true;
                        }
                        else
                        {
                            item.Var = null;
                        }
                    }
                }
                item.IsSwap = (XmlGetter.Attribute(child, "IsSwap").ToLower().Equals("false") 
                    == false);// xNodeList.Item(i).Attributes["IsSwap"].Value.ToLower().Equals("true");
                //isSwap == false is default.
                item.Description = XmlGetter.Attribute(child, "Comment");

                int _size = 0; //function일 때는 의미 없으므로 일단 보류
                try
                {
                    _size = Convert.ToInt32(XmlGetter.Attribute(child, "Size"));//xNodeList.Item(i).Attributes["Size"].Value);
                }
                catch { }
                #endregion
                #region PacketData
                XmlNodeList packetData = XmlGetter.Children(child, "PacketData");
                if(item.PacketData==null) item.PacketData = new Dictionary<string,PacketData>();
                

                foreach (XmlNode packetDatum in packetData)
                {
                    String name = XmlGetter.Attribute(packetDatum, "Name");
                    String val = XmlGetter.Attribute(packetDatum, "Value");
                    string funcName;
                    string[] tempArgs;

                    if (isFunction(val, out funcName, out tempArgs)) parser.IsDynamicPacket = true;
                    PacketData pd = new PacketData() { Value = val };
                    item.PacketData.Add(name, pd);
                }
                #endregion
                #region InitValue
                XmlNodeList initValues = XmlGetter.Children(child, "InitValue");// child.ChildNodes;// xNodeList.Item(i).ChildNodes;

                if (initValues.Count >= 1)
                {
                    List<String> _initObjValues = new List<String>();
                    int count = 0;
                    foreach(XmlNode initValue in initValues)
                    //for (int initCount = 0; initCount < initValues.Count; initCount++) //초기설정을 넣는다.
                    {
                        //if (initValues.Item(initCount).Name.Equals("InitValue"))
                        //{
                        //    _initObjValues.Add(initValues.Item(initCount).InnerText);
                        //}

                        _initObjValues.Add(initValue.InnerText);
                        string[] values = initValue.InnerText.Split(", ".ToCharArray(),  StringSplitOptions.RemoveEmptyEntries);
                        foreach (string val in values)
                        {
                            string funcName;
                            string[] tempArgs;
                            if (isFunction(val, out funcName, out tempArgs))
                            {
                                parser.IsDynamicPacket = true;
                                break;
                            }
                        }
                        count++;
                        if (count >= _size) break;
                    }

                    //initValue가 모자라면 그만큼 0으로 채움..
                    for (int i = count; i < _size; i++)
                    {
                        _initObjValues.Add("0");
                    }

                    item.Length = _size;
                    
                    item.InitValues = _initObjValues.ToArray();

                    
                }
                #endregion
                #region BitItem
                XmlNodeList bitItems = XmlGetter.Children(child, "BitItems/BitItem");// child.ChildNodes;// xNodeList.Item(i).ChildNodes;
                foreach(XmlNode bitItem in bitItems){
                    BitItem bItem = new BitItem(item);
                    bItem.LoadXml(bitItem);
                    item.BitItems.Insert(bItem);//(bItem.StartOffset, bItem.BitSize, bItem.BitName);
                }
                #endregion
                #region function
                FunctionInfo func;
                XmlNode function = XmlGetter.Child(child, "Function");// child.ChildNodes;// xNodeList.Item(i).ChildNodes;
                if(function!=null)
                {
                    //func = new FunctionInfo();

                    XmlNodeList args = XmlGetter.Children(function, "Arg");// initValues.Item(0).ChildNodes;
                    List<String> argList = new List<string>();
                    string funcName = XmlGetter.Attribute(function, "Name");// initValues.Item(0).Attributes["Name"].Value;
                    String[] argArray = null;
                    if (parser != null)
                    {
                        for (int argc = 0; argc < args.Count; argc++)
                        {
                            argList.Add(args[argc].InnerText);
                        }
                        if (argList.Count > 0) argArray = argList.ToArray();

                        if (FunctionsList.ContainsKey(funcName))
                        {

                            func = new FunctionInfo(FunctionsList[funcName], argArray);//.setFunction(FunctionsList[funcName], argList.ToArray());

                        }
                        else
                        {
                            func = new FunctionInfo(funcName, null, argArray);
                        }

                        if (func.Exists)
                        {
                            item.Function = func;
                            if (parser != null) parser.IsDynamicPacket = true;
                        }
                    }
                    else //parser==null
                    {
                        if (FunctionsList.ContainsKey(funcName))
                        {
                            object ret = FunctionsList[funcName].Invoke();
                            if (ret is String[]) item.InitValues = ret as String[];
                            else if (ret is object[])
                            {
                                item.InitValues = new String[(ret as object[]).Length];
                                for (int ri = 0; ri < (ret as object[]).Length; ri++)
                                {
                                    item.InitValues[ri] = (ret as object[]).ElementAt(ri).ToString();
                                }
                            }
                            else
                            {
                                item.InitString = ret.ToString();
                            }
                        }
                        else
                        {
                            item.InitString = "";
                        }
                    }
                }
                #endregion
                items.Add(item);
            }
            
            return items;

        }

        public static String XmlToCode(String xmlFile, CStructInfo info=null)
        {
            IList<CPacketItem> list = XmlToItems(xmlFile, null, info);
            return ItemsToCode(list);
        }

        public static String ItemsToCode(IList<CPacketItem> items)
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
                if (items[i].Function != null)
                {
                    code += items[i].Function.Name+"(";//.InitString;
                    object[] args = items[i].Function.GetArgs();
                    if (args != null)
                    {
                        for (int ai = 0; ai < args.Length; ai++)
                        {
                            if (ai != 0) code += ",";
                            code += args[ai];
                        }
                    }
                    code+=")";

                }
                else if (items[i].Var != null)
                {
                    code += items[i].Var.Name;

                }else if (items[i].Length == 1)
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
                code += ";";
                if(items[i].Description.Length>0) code+=" //"+items[i].Description;
                code+="\r\n";
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

        enum tokenNodes { type = 0, name, arrNum,
            simple_func,//변수명 뒤에 @이름 식으로 나타나는 간단함수. imbeded된 함수로, 사용자함수가 아님.
            func_args,
             //=뒤에 함수명(함수args)식으로 나타나는 함수.
            equal, value, outArray, inArray, afterLine, inComment };
        public static List<CPacketItem> CodeToItems(String codeStr, CPacketStruct packetStruct)
        {
            List<CPacketItem> items = new List<CPacketItem>();
            /*
            names.Clear();
            types.Clear();
            sizes.Clear();
            initStr.Clear();
            initValues.Clear();
             */
            if(packetStruct!=null)  packetStruct.IsDynamicPacket = false;
            
            String[] lines = codeStr.Split("\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries); //각 라인으로 나누어서
            String[] initValues;
            for (int line_i = 0; line_i < lines.Length; line_i++)
            {//각 라인들
                String line = lines[line_i];
                line = removeOuterWhiteSpace(line)+"\n";//마지막을 \n으로 검출함..
                line = line.Replace('\r',' ').Replace('\t',' ');//다양한 공백문자를 스페이스로 치환한다.
                
                if(line.Length==0) continue;

                string typeStr="";
                Type type = null;
                string name = "";

                string equal = "";
                List<String> values = new List<string>();
                string func = "";
                string arrNumStr = "";
                string tempToken = "";
                string comment="";
                uint arrNum = 1;
                string errStr = "";
                tokenNodes mode = tokenNodes.type;
                bool isArray = false;
                for (int ci = 0; ci < line.Length; ci++)
                {
                    char c = line[ci];
                    if(mode== tokenNodes.afterLine){//모든 token분석이 끝났다. 이후는 comment취급한다.
                        if(c=='\n'){
                            if(tempToken.Length>0){
                                throw new Exception("StructXmlParser: line [" + line_i + "] - wrong format. // would be comment. your input:"+tempToken+c+"\r\n"+errStr+" <<");
                            }
                            break;
                        }
                        else if(c=='/'){
                            if(tempToken.Length==0){
                                tempToken+='/';
                            }
                            else if(tempToken.Equals("/")){
                                mode = tokenNodes.inComment;
                                errStr+=tempToken+'/';
                                tempToken = "";
                            }else{
                                //can't occur..
                            }
                        }else if(c==' '){//;이후에 나온 공백은 모두 무시
                            continue;
                        }else{
                            throw new Exception("StructXmlParser: line [" + line_i + "] - wrong format. // would be comment. your input:"+tempToken+c+"\r\n"+errStr+" <<");
                        }
                    }else if(mode== tokenNodes.inComment){
                        if(c=='\n'){
                            comment = tempToken;
                            tempToken = "";
                            break;
                        }else{
                            tempToken+=c;
                        }
                    }else if( c == ' '){
                        if (tempToken.Length == 0) continue;//공백이 두개 이상나왔다.
                         switch (mode)
                        {
                            case tokenNodes.type:
                                typeStr = tempToken;
                                
                                type = TypeHandling.getTypeFromTypeName(typeStr, TypeHandling.Platforms.C32Bit);
                                if (type == null)
                                {
                                    throw new Exception("StructXmlParser: line [" + line_i + "] - type["+typeStr+"] is not a valid type."+"\r\n"+errStr+" <<");
                                }
                                mode = tokenNodes.name;
                                break;
                            case tokenNodes.name:
                                
                                if (tempToken.Length == 0) continue;//이름의 앞뒤에 공백 나와도 그냥 넘어간다. @ 나 = 또는 [가 나와야 끝남.
                                else
                                {
                                    tempToken += ' ';//중간에 들어가는공백은 이름에 포함됨..
                                    continue;
                                }
                                
                                break;
                            case tokenNodes.arrNum:
                                 continue;
                            case tokenNodes.simple_func:
                                if (tempToken.Length == 0) continue;//이름의 앞뒤에 공백 나와도 그냥 넘어간다. @ 나 = 또는 [가 나와야 끝남.
                                else
                                {
                                    tempToken += ' ';//중간에 들어가는공백은 이름에 포함됨..
                                    continue;
                                }
                                break;
                            case tokenNodes.equal:
                                continue;//=의 앞뒤에 공백 나와도 그냥 넘어간다.
                            case tokenNodes.value:
                            case tokenNodes.inArray:
                                
                                if (tempToken.Length == 0) continue;//배열 정의값의 앞뒤에 공백 나와도 그냥 넘어간다.
                                else
                                {
                                    tempToken += ' ';//값 중간에 있는 공백은 포함시킨다.
                                    continue;
                                }
                                break;
                            case tokenNodes.outArray://배열 정의 끝나고 나서..
                                continue;//이후 나오는 공백은 모두 무시한다.

                        }
                        errStr+=tempToken+' ';
                        tempToken = "";
                    }
                    else if (c == '(')
                    {
                        if (mode == tokenNodes.value)
                        {
                            string funcName = tempToken.Trim();
                            if (funcName.Length > 0)
                            {
                                mode = tokenNodes.func_args; //함수정의로 들어감..
                            }
                        }
                        else
                        {
                            throw new Exception("StructXmlParser: line [" + line_i + "] - '(' is on the invalid position. should be after function name" + "\r\n" + errStr + " <<");
                        }
                        tempToken += c;
                        errStr+=tempToken+'(';
                    }
                    else if (c == ')' && mode == tokenNodes.func_args)
                    {
                        if (packetStruct != null) packetStruct.IsDynamicPacket = true;//함수정의이므로..
                        mode = tokenNodes.value;
                        tempToken += c;
                    }
                    else if (c == ';')
                    {


                        switch (mode)
                        {
                            case tokenNodes.type:
                                typeStr = tempToken;

                                type = TypeHandling.getTypeFromTypeName(typeStr, TypeHandling.Platforms.C32Bit);
                                if (type == null)
                                {
                                    throw new Exception("StructXmlParser: line [" + line_i + "] - type[" + typeStr + "] is not a valid type" + "\r\n" + errStr + " <<");
                                }
                                mode = tokenNodes.name;
                                break;
                        }

                        if (mode == tokenNodes.type && typeStr.Length == 0)
                        {
                            mode = tokenNodes.afterLine;
                            tempToken = "";
                            continue;
                        }
                        else if (typeStr.Length == 0)//type은 필수임..끝낼 수 없슴.
                        {
                            throw new Exception("StructXmlParser: line [" + line_i + "] doesn't have type defination.." + "\r\n" + errStr + " <<");
                        }
                        if (tempToken.Length > 0)
                        {
                            if (mode == tokenNodes.value && values.Count == 0)//value에 처음값이 들어온 뒤(배열되기전) 줄이 끝나면 //values는 값의 배열이다.
                            {
                                values.Add(tempToken);
                            }
                            else if (mode == tokenNodes.name)
                            {
                                name = tempToken;
                            }
                            else if (mode == tokenNodes.simple_func)
                            {
                                func = tempToken;
                            }
                            else if (mode == tokenNodes.func_args)
                            {
                                throw new Exception("StructXmlParser: line [" + line_i + "] can't parsed.. function definition isn't closed.." + "\r\n" + errStr + " <<");
                            }
                        }
                        tempToken = "";
                        mode = tokenNodes.afterLine;

                        continue;
                        errStr += tempToken + ';';
                        tempToken = "";
                    }
                    else if (c == '@')
                    {
                        if (mode == tokenNodes.name)
                        {
                            name = tempToken.Trim();
                            mode = tokenNodes.simple_func;
                            tempToken = "";
                        }
                        else if (mode == tokenNodes.equal)
                        {
                            mode = tokenNodes.value;
                            tempToken = "@";//value의 시작..
                            packetStruct.IsDynamicPacket = true;
                        }
                        else if (mode == tokenNodes.value)
                        {
                            if (tempToken.Trim().Length == 0) //@가 처음에 나오면 함수나 변수이다.
                            {
                                packetStruct.IsDynamicPacket = true;
                            }
                            //끝문자가 나오기 전에는 계속 추가만 할 뿐..
                            tempToken += c;
                        }
                        else
                        {
                            throw new Exception("StructXmlParser: line [" + line_i + "] : wrong format - '@' have to be place after name for a function" + "\r\n" + errStr + " <<");
                        }
                        errStr += tempToken + '@';

                    }
                    else if (c == '[')
                    {
                        if (mode == tokenNodes.name)
                        {
                            name = tempToken.Trim();
                            mode = tokenNodes.arrNum;
                        }
                        else if (mode == tokenNodes.simple_func)
                        {
                            func = tempToken;
                            mode = tokenNodes.arrNum;
                        }
                        else
                        {
                            throw new Exception("StructXmlParser: line [" + line_i + "] : wrong format - '[' have to be place after name or function name for array size" + "\r\n" + errStr + " <<");
                        }
                        errStr += tempToken + '[';
                        tempToken = "";
                    }
                    else if (c == ']')
                    {
                        if (mode == tokenNodes.arrNum)
                        {
                            arrNumStr = tempToken.Trim();
                            if (arrNumStr.Length > 0)
                            {
                                if (uint.TryParse(arrNumStr, out arrNum) == false)
                                {
                                    throw new Exception("StructXmlParser: line [" + line_i + "] : wrong format - it must be unsigned number in '[' and ']' : your input:" + tempToken + "\r\n" + errStr + " <<");
                                }
                            }
                            else
                            {//array의 크기가 정해져있지 않다. 나중에 정해질 것임..
                                arrNum = 0;
                            }
                            isArray = true;
                            mode = tokenNodes.equal;
                        }
                        else
                        {
                            throw new Exception("StructXmlParser: line [" + line_i + "] : wrong format - ']' have to be placed after [ and number for array size" + "\r\n" + errStr + " <<");
                        }
                        errStr += tempToken + ']';
                        tempToken = "";
                    }
                    else if (c == '=')
                    {
                        if ((mode == tokenNodes.name || mode == tokenNodes.simple_func || mode == tokenNodes.equal) && c == '=')//이름뒤에 =이 나오면
                        {
                            if (mode == tokenNodes.name)
                                name = tempToken.Trim();
                            else
                            {//func
                                func = tempToken;
                            }
                            equal += c;
                            mode = tokenNodes.value;
                            tempToken = "";
                        }
                        else
                        {
                            throw new Exception("StructXmlParser: line [" + line_i + "] : wrong format - '=' have to be placed after name,[num] or function name for assign values" + "\r\n" + errStr + " <<");
                        }
                        errStr += tempToken + '=';
                        tempToken = "";
                    }
                    else if (c == ',' || c == '}')
                    {
                        if (mode == tokenNodes.inArray)
                        {
                            if (tempToken.Length == 0) values.Add("0");
                            else values.Add(tempToken.Trim());
                        }
                        else
                        {
                            throw new Exception("StructXmlParser: line [" + line_i + "] : wrong format - ',' have to be placed in the '{' and '}' when this variable is array." + "\r\n" + errStr + " <<");
                        }
                        if (c == '}')
                        {
                            string str = "";
                            for (int vi = 0; vi < values.Count; vi++)
                            {
                                str += values[vi];
                                if (TypeHandling.isValidType(values[vi], typeStr) == false)
                                {
                                    //String error = "초기값이 타입과 다릅니다.";
                                    //if(TypeHandling.getTypeKind(values[i])== TypeHandling.TypeName.HexString) error+=" hex값을 넣어주실 때는 unsigned 타입으로 지정하십시오.";
                                    throw new Exception("StructXmlParser: line [" + line_i + "] : wrong value type of array. type:"
                                        + typeStr + "/ value:" + values[vi] + "..\r\n tip: if you want use hex as value, use unsigned type. \r\n" + errStr + str + " <<");
                                }
                            }
                            mode = tokenNodes.outArray;
                        }
                        errStr += tempToken;
                        tempToken = "";
                    }
                    else if (c == '{')
                    {
                        if (isArray == false)
                        {
                            throw new Exception("StructXmlParser: line [" + line_i + "] : wrong format - '{' have to be placed if '[' and ']' exist after name or function.");
                        }
                        else if (mode == tokenNodes.value)
                        {
                            mode = tokenNodes.inArray;
                        }
                        else
                        {
                            throw new Exception("StructXmlParser: line [" + line_i + "] : wrong format - '{' have to be placed in the value area - after '='");
                        }
                        //do nothing..
                        errStr += "{";//tempToken;
                        //tempToken = "";
                    }
                    else
                    {
                        tempToken += c;
                    }

                }
                #region old
                /*
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
                //wootra for now.,String type = line.Substring(0, firstSpace);

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
                
                rest = rest.Replace(" ", "");
                //변수명검사
                for (int j = 0; j < rest.Length; j++) //변수명 검사
                {
                    if ((Char.IsLetterOrDigit(rest[j]) == false) && rest[j].Equals('_') == false)
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
                CPacketItem item;
                
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
                       
                       
                        
                        bool isVariable = false;
                        
                        if (token[j].Substring(0, 1).Equals("@"))//변수일 때..
                        {
                            setError("배열에서는 변수를 사용할 수 없습니다.  value:" + token[j], i, line);
                            return null;
                            
                        }
                        if (isVariable == false)
                        {
                            switch (TypeHandling.getValueAndType(token[j], ref intValue, ref doubleValue, ref strValue))
                            {//hex나 oct형식등을 모두 숫자형으로 먼저 치환한다.
                                case TypeHandling.TypeName.Integer:
                                    if (isSwap)
                                    {
                                        if (TypeHandling.getTypeFromTypeName(typeStr) == typeof(byte)) initValues[j] = Swaper.swap<byte>((byte)intValue).ToString();
                                        else if (TypeHandling.getTypeFromTypeName(typeStr) == typeof(short)) initValues[j] = Swaper.swap<short>((short)intValue).ToString();
                                        else if (TypeHandling.getTypeFromTypeName(typeStr) == typeof(int)) initValues[j] = Swaper.swap<int>((int)intValue).ToString();
                                        else if (TypeHandling.getTypeFromTypeName(typeStr) == typeof(long)) initValues[j] = Swaper.swap<long>((long)intValue).ToString();
                                        else if (TypeHandling.getTypeFromTypeName(typeStr) == typeof(ushort)) initValues[j] = Swaper.swap<ushort>((ushort)intValue).ToString();
                                        else if (TypeHandling.getTypeFromTypeName(typeStr) == typeof(uint)) initValues[j] = Swaper.swap<uint>((uint)intValue).ToString();
                                        else if (TypeHandling.getTypeFromTypeName(typeStr) == typeof(ulong)) initValues[j] = Swaper.swap<ulong>((ulong)intValue).ToString();
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
                            if (j != 0) newInitStr += ",";
                            newInitStr += initValues[j].ToString();
                            if (TypeHandling.isValidType(initValues[j].ToString(), typeStr) == false)
                            {
                                setError("초기값이 타입과 다릅니다.Type:" + typeStr + "  value:" + token[j], i, line);
                                return null;
                            }
                        }
                    }
                    initValue = newInitStr;
                    
                }else if(initValue!=null && initValue.Length>0) //배열형식이 아니라 단순값일 때
                {
                    if (initValue[0]!='-' && Char.IsDigit(initValue[0])==false && initValue[0]!='\"')//첫글자가 문자로 시작하고, 따옴표(")로시작하지 않으면 함수나 변수이거나 문자열이다. 
                    {
                        int argstart = initValue.IndexOf("(");
                        int argend = initValue.LastIndexOf(")");
                        bool isVarOrFunc = false;

                        if(argstart<0 && argend<0 && initValue.IndexOf("@")==0){//변수임.
                            String varName = initValue.Substring(1);
                            if (VariablesList.ContainsKey(varName) && VariablesList[varName].ValueExists && VariablesList[varName].Values.Length>0)
                            {
                                isVarOrFunc = true;
                                if (packetStruct == null)
                                {
                                    if (VariablesList[varName].Values.Length == 1)
                                    {
                                        initValue = VariablesList[varName].Values.ElementAt(0);
                                        item = new CPacketItem(rest, typeStr, arrSize, initValue);
                                    }
                                    else
                                    {
                                        item = new CPacketItem(rest, typeStr, arrSize, VariablesList[varName].Values);
                                    }
                                    //packetStruct.IsDynamicPacket = true;
                                }
                                else
                                {
                                    item = new CPacketItem(rest, typeStr, arrSize, initValue);
                                    packetStruct.IsDynamicPacket = true;
                                }
                                
                                item.IsSwap = isSwap;
                                //item.InitString = initValue;
                                
                                item.Var = VariablesList[varName];// new VariableInfo(initValue);
                                items.Add(item);
                                
                                continue;
                            }
                        }else if(argstart>1 && argend==initValue.Length-1)
                        {//함수임.
                            String funcName = initValue.Substring(0, argstart);
                            if (FunctionsList.ContainsKey(funcName))
                            {
                                isVarOrFunc = true;
                                String[] args = initValue.Substring(argstart + 1, argend - 1 - argstart).Split(",".ToCharArray(),  StringSplitOptions.RemoveEmptyEntries);

                                FunctionInfo funcInfo = new FunctionInfo(FunctionsList[funcName],args);
                                if (packetStruct == null)
                                {
                                    initValue = FunctionsList[funcName].Invoke(args).ToString();
                                    item = new CPacketItem(rest, typeStr, arrSize, initValue);
                                }
                                else
                                {
                                    item = new CPacketItem(rest, typeStr, arrSize, funcInfo);
                                    packetStruct.IsDynamicPacket = true;
                                }
                                item.IsSwap = isSwap;
                                //item.InitString = initValue;
                                //items[i].Function.setFunction( FunctionsList[funcName],args);// new FunctionInfo(funcName, args);
                                items.Add(item);
                                
                                continue;
                            }
                        }
                        if (isVarOrFunc == false)
                        {
                            if (typeStr.Equals("string"))
                            {
                                initValue = initValue.Replace("\\\"", "@'aAiIOo~{|\\]~");//\"를 구분하기 위해 모두 특수한 문자로 바꾸어준다.
                                initValue = initValue.Replace("\"", "");
                                initValue = initValue.Replace("@'aAiIOo~{|\\]~", "\\\"");//\"를 다시 복구한다.

                                initValues[0] = initValue;//따옴표를 지우고 넣어준다.
                            }
                            else
                            {
                                setError("함수정의가 맞지 않습니다. 괄호가 완성되지 않았습니다.", i, line);
                                return null;
                            }
                        }
                    }
                    else
                    {
                        
                        if(typeStr.Equals("string")){
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
                 */
                    
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
                
                //초기값 검사 끝
                if(name.Length==0) name = "var_"+(items.Count+1);
                if (arrNum != values.Count) throw new Exception("array size is different to values");
                
                CPacketItem item = new CPacketItem(name, typeStr, values.ToArray(), packetStruct, comment);
                
                if(func.ToLower().Equals("swap")) item.IsSwap = true;
                else item.IsSwap = false;
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
            if (packetStruct != null) packetStruct.Items.CopyFrom(items);
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
            return ItemsToPacket(XmlToItems(xml, null), swap, sendBuff, out totalSendSize, false);
        }

        public static bool CodeToPacket(String msg, bool swap, Byte[] sendBuff, out int totalSendSize)
        {

            return ItemsToPacket(CodeToItems(msg, null), swap, sendBuff, out totalSendSize, false);
        }

        public static bool ItemsToPacket(IList<CPacketItem> items, bool swap, Byte[] sendBuff, out int totalSendSize, bool isDynamic)
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
                /*
                string initString;
                
                if (items[i].Function != null)
                {
                    object ret = items[i].Function.Invoke();
                    if (ret != null) initString = ret.ToString();
                    else initString = "0";
                }
                else if (items[i].Var != null && items[i].Var.ValueExists && items[i].Var.Values.Length>0)
                {
                    initString = items[i].Var.Values[0];
                }
                else
                {
                    initString = items[i].InitString;
                }
                */
                if (type.Equals("char") || type.Equals("string"))
                {
                    if (items[i].InitString == null || items[i].InitString.Length == 0) continue;
                    packets = Encoding.UTF8.GetBytes(items[i].InitString);
                    Buffer.BlockCopy(packets, 0, sendBuff, totalSendSize, packets.Length);
                    totalSendSize += packets.Length;
                }
                else
                {

                    String[] units;
                    string funcName;
                    string[] args;

                    if(items[i].InitValues.Length==1 && isFunction(items[i].InitValues[0], out funcName, out args)){
                        if (FunctionsList.ContainsKey(funcName))
                        {
                            FunctionInfo funcInfo = new FunctionInfo(FunctionsList[funcName], args);
                            object ret = FunctionsList[funcName].Invoke(args);
                            units = getUnitFromFuncReturn(ret);
                        }
                        else
                        {
                            units = new string[] { "0" };
                            //initString = "0";
                        }
                    }
                    else if (items[i].Function != null )
                    {
                        object ret = items[i].Function.Invoke();
                        units = getUnitFromFuncReturn(ret);
                    }
                    else if (items[i].Var != null && items[i].Var.ValueExists && items[i].Var.Values.Length>0)
                    {
                        //initString = items[i].InitString;//.Var.Values[0];
                        //units = new String[1];
                        units = items[i].Var.Values;// = initString;// ret.ToString();
                    }
                    else if (items[i].InitValues != null && items[i].InitValues.Length > 0)
                    {
                        units = new string[ items[i].InitValues.Length];
                        items[i].InitValues.CopyTo(units, 0);
                    }
                    else
                    {
                        units = new String[1]{items[i].InitString};
                        //initString = items[i].InitString;
                        //units[0] = initString;

                    }

                    if (units.Length == 0 || units[0].Length == 0)//비어있으면 모두 0으로 채움..
                    {
                        for(int k=0; k< Buffer.ByteLength(packets); k++){
                            Buffer.SetByte(packets, k, 0);
                        }
                    }
                    else
                    {

                        //    = items[i].InitValues;

                        for (int k = 0; k < units.Length; k++)
                        {
                            if (isDynamic)
                            {
                                if (units[k].Length > 0 && units[k][0] == '@')
                                {
                                    units[k] = getFuncOrValue(units[k]);
                                }
                            }
                            TypeHandling.TypeName typeName = TypeHandling.getTypeKind(units[k]);
                            try
                            {
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
                            catch (Exception e)
                            {
                                throw new Exception("변환중 에러 발생: " + typeName + " " + items[i].Name + "[" + k + "] = " + units[k] + " to " + packets.ToString());

                                break;
                            }
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

        private static string[] getUnitFromFuncReturn(object ret)
        {
            string[] units;
            if (ret == null)
            {
                units = new String[1];
                units[0] = "0";
            }
            else if (ret is String[]) units = ret as String[];
            else if (ret is object[])
            {
                units = new String[(ret as object[]).Length];
                for (int ui = 0; ui < units.Length; ui++)
                {
                    units[ui] = (ret as object[])[ui].ToString();
                }
            }
            else if (ret is Array)
            {
                units = new String[(ret as Array).Length];
                int ui = 0;
                foreach (object obj in (ret as Array))
                {
                    units[ui] = obj.ToString();
                    ui++;
                }
            }
            else
            {
                units = new String[1];
                units[0] = ret.ToString();
            }
            return units;
        }

        private static string getFuncOrValue(string initValue)
        {
            int argstart = initValue.IndexOf('(');
            int argend = initValue.IndexOf(')');

            if (argstart < 0 && argend < 0 && initValue.IndexOf("@") == 0)
            {//변수임.
                String varName = initValue.Substring(1);
                if (VariablesList.ContainsKey(varName) && VariablesList[varName].ValueExists && VariablesList[varName].Values.Length > 0)
                {
                    if (VariablesList[varName].Values.Length > 0)
                    {
                        initValue = VariablesList[varName].Values.ElementAt(0);
                        return initValue;
                    }
                }
            }
            else if (argstart > 1 && argend == initValue.Length - 1)
            {//함수임.
                String funcName = initValue.Substring(0, argstart);
                if (FunctionsList.ContainsKey(funcName))
                {
                    String[] args = initValue.Substring(argstart + 1, argend - 1 - argstart).Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                    FunctionInfo funcInfo = new FunctionInfo(FunctionsList[funcName], args);

                    initValue = FunctionsList[funcName].Invoke(args).ToString();
                    
                    return initValue;
                    //item.InitString = initValue;
                    //items[i].Function.setFunction( FunctionsList[funcName],args);// new FunctionInfo(funcName, args);
                }
            }
            
            return "0";
        }

        static bool isFunction(string initValue, out string funcName, out string[] args){
            int argstart = initValue.IndexOf('(');
            int argend = initValue.IndexOf(')');

            if (argstart > 1 && argend == initValue.Length - 1)
            {//함수임.
                funcName = initValue.Substring(0, argstart);
                args = new string[0];
                if (FunctionsList.ContainsKey(funcName))
                {
                    args = initValue.Substring(argstart + 1, argend - 1 - argstart).Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);



                    //item.InitString = initValue;
                    //items[i].Function.setFunction( FunctionsList[funcName],args);// new FunctionInfo(funcName, args);
                }
                return true;
            }
            else
            {
                funcName = "";
                args = new string[0];
                return false;
            }
        }

        
    
    }
}
