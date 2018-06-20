using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetworkPacket;
using XmlDesigner;
using System.Xml;
using XmlHandlers;
using System.Xml.Schema;
using System.IO;
using System.Windows.Forms;
using DataHandling;
namespace XmlDesigner.PacketDatas
{
    public class XmlCommandFields: NetworkPacketWith<byte>, IXmlItem, IPacketItem
    {
        
        StructType _structType;
        public XmlCommandFields(XmlCommandFields parentFields, StructType structType)
        {
            _parentFields = parentFields;
            _structType = structType;
        }

        XmlCommandFields _parentFields;
        public XmlCommandFields ParentFields
        {
            get { return _parentFields; }
        }

         XmlDocument _xDoc;  public void LoadXml(String xmlFile, Boolean refLoad = false)
        {
            if (XmlScenario.NowLoadingPath.Length > 0) _filePath =  xmlFile;
            else _filePath = xmlFile;

            XmlNode xNode = XmlGetter.RootNode(out _xDoc,_filePath, null, XmlSchemaValidation);

            try
            {
                LoadXml(_xDoc, xNode, refLoad);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + ":" + xmlFile);
            }
        }



        void XmlSchemaValidation(object sender, ValidationEventArgs e)
        {
            MessageBox.Show(e.Message);
        }


        public void SaveXml(String xmlFile)
        {

        }

        public int ByteSize
        {
            get
            {
                int size = 0;
                foreach (XmlCommandField field in FieldList.Values)
                {
                    size += field.DataSize;
                }

                foreach (XmlCommandFields fields in FieldsList.Values)
                {
                    size += fields.ByteSize;
                }

                return size;
            }
        }

        public int GetDataToBuffer(Array targetBuffer, int nowOffset)
        {
            if (VariableSizeField != null && VariableField != null)
            {
                VariableSizeField.DataValue = Buffer.ByteLength(VariableField.TargetBuffer); //크기 지정..
            }

            foreach (XmlCommandField item in FieldList.Values)
            {
                XmlCommandField field = item as XmlCommandField;
                nowOffset = field.GetDataToBuffer(targetBuffer, nowOffset);
                
            }

            foreach (XmlCommandFields item in FieldsList.Values)
            {
                XmlCommandFields fields = item as XmlCommandFields;
                nowOffset = fields.GetDataToBuffer(targetBuffer, nowOffset);
            }
            return nowOffset;
        }
        /*
        ListDic<String,IPacketItem> _allItems = new ListDic<String,IPacketItem>();
        public List<IPacketItem> AllItems
        {
            get { return _allItems.Values.ToList(); }
        }
        */

        ListDic<String, XmlCommandField> _fieldList = new ListDic<string, XmlCommandField>();

        public ListDic<String, XmlCommandField> FieldList { get { return _fieldList; } }

        XmlCommandFields _templateFields;
        public XmlCommandFields TemplateFields
        {
            get { return _templateFields; }
            set{_templateFields = value;}
        }

        ListDic<String, XmlCommandFields> _fieldsList = new ListDic<string, XmlCommandFields>();

        public ListDic<String, XmlCommandFields> FieldsList { get { return _fieldsList; } }

        public void FillFieldList(List<XmlCommandField> targetFieldList)
        {
            foreach (XmlCommandField field in FieldList.Values)
            {
                targetFieldList.Add(field);
            }

            foreach (XmlCommandFields fields in FieldsList.Values)
            {
                fields.FillFieldList(targetFieldList);
            }
        }

        public enum NumberChangedModes { CopyTemplate, RemainValues };
        NumberChangedModes _numberChangedMode = NumberChangedModes.CopyTemplate;
        public NumberChangedModes NumberChangedMode { get { return _numberChangedMode; } set { _numberChangedMode = value; } }

        XmlCommandField _loopCountField;
        public XmlCommandField LoopCountField { get{ return _loopCountField;} }

        XmlCommandField _variableSizeField;
        public XmlCommandField VariableSizeField { get { return _variableSizeField; } }

        XmlCommandField _variableField;
        public XmlCommandField VariableField { get { return _variableField; } }

        public int VariableSize
        {
            get
            {
                if (_variableSizeField == null) return 0;
                else return Convert.ToInt32(_variableSizeField.DataValue);
            }
        }

        public int NumberOfFields
        {
            get
            {
                if (LoopCountField == null) return 0;
                else
                {
                    return Convert.ToInt32(LoopCountField.DataValue);
                }
            }
            set
            {
                
                if(LoopCountField!=null){
                    LoopCountField.DataValue = value;
                    if (_numberChangedMode == NumberChangedModes.RemainValues)
                    {
                        if (value > FieldsList.Count)
                        {
                            for (int i = FieldsList.Count; i < value; i++)
                            {
                                String newName = TemplateFields.Name + "[" + i + "]";
                                XmlCommandFields newFields = TemplateFields.Clone(newName);
                                FieldsList.Add(newName, newFields);
                            }
                        }
                        else if (value < FieldsList.Count)
                        {
                            while (FieldsList.Count > value)
                            {
                                FieldsList.RemoveAt(FieldsList.Count - 1);
                            }
                        }
                    }
                    else if(_numberChangedMode == NumberChangedModes.CopyTemplate)
                    {
                        FieldsList.Clear();
                        for (int i = 0; i < value; i++)
                        {
                            String newName = TemplateFields.Name + "[" + i + "]";
                            XmlCommandFields newFields = TemplateFields.Clone(newName);
                            FieldsList.Add(newName, newFields);
                        }
                    }
                }else{
                    throw new Exception("Field중에 FieldType이 LoopCount인 Field가 없습니다.");
                }
            }
        }

        public XmlCommandFields Clone(String newName)
        {
            XmlCommandFields newFields = new XmlCommandFields(_parentFields, _structType);
            
            int count=0;
            foreach(XmlCommandField field in FieldList.Values){
                XmlCommandField newField = field.Clone();
                newFields.FieldList.Add(field.Name, newField);
                newFields.addItem(count++, field.DataSize, true);
                if (field.FieldType == FieldTypes.LoopCount)
                {
                    newFields._loopCountField = newField;
                }
                else if (field.FieldType == FieldTypes.Variable)
                {
                    newFields._variableField = newField;
                }
                else if (field.FieldType == FieldTypes.VariableSize)
                {
                    newFields._variableSizeField = newField;
                }
            }
            newFields.setBuffSize();
            int offset=0;
            foreach (XmlCommandField field in newFields.FieldList.Values)
            {
                field.SetTargetBuffer(newFields.buffer, offset);
                offset += field.DataSize;
                field.Data = FieldList[field.Name].Data;
            }

            foreach(XmlCommandFields fields in FieldsList.Values){

                newFields.FieldsList.Add(fields.Name, fields.Clone(fields.Name));
                
            }
            newFields.Name = newName;
            newFields._templateName = _templateName;
            newFields._templateFields = _templateFields;// (_templateFields != null) ? _templateFields.Clone(_templateFields.Name) : null;
            newFields._numberChangedMode = _numberChangedMode;
            //newFields.SizeDefineField = SizeDefineField;
            return newFields;
        }

        String _name = "Default";
        public String Name { set { _name = value; } get { return _name; } }

        String _templateName = "Default";
        public String TemplateName { get { return _templateName; } set { _templateName = value; } }

        PacketHandlingTypes _pacektHandlingType;
        public PacketHandlingTypes PacketHandlingType
        {
            get { return _pacektHandlingType; }
        }

        public void LoadXml(XmlDocument xDoc, XmlNode rootNode, Boolean refLoad = false)
        {
            if (rootNode == null) return;
            _xDoc = xDoc; XmlControlHandler.GetDefaultXmlItemAttributes(rootNode, xDoc, this);


            //int count = 0;
            //int index = 0;
            int count = 0;
            _pacektHandlingType = PacketHandlingTypes.Static;//default

            foreach (XmlNode fieldNode in rootNode.ChildNodes)
            {
                if (fieldNode.Name.Equals(Properties.Resources.Fields_Field_Tag))
                {
                    XmlCommandField field = new XmlCommandField(this);

                    field.LoadXml(xDoc, fieldNode);

                    addItem(count++, field.DataSize, field.FieldType == FieldTypes.Dynamic, field.DataType);
                    _fieldList.Add(field.Name, field);
                    if (field.FieldType == FieldTypes.LoopCount)
                    {
                        _loopCountField = field;
                        //field.DataValue = 0;
                        _pacektHandlingType = PacketHandlingTypes.Loop;

                    }
                    else if (field.FieldType == FieldTypes.VariableSize)
                    {
                        _variableSizeField = field;
                        _pacektHandlingType = PacketHandlingTypes.Serial;
                    }
                    else if (field.FieldType == FieldTypes.Variable)
                    {
                        _variableField = field;
                        if (_structType == StructType.Command)
                        {
                            if (_variableSizeField == null)
                            {
                                throw new Exception("There's no FieldType[VariableSizeField] for this field["+field.Name+"]");
                                
                            }
                            else
                            {
                                _variableSizeField.DataValue = Buffer.ByteLength(field.TargetBuffer);
                            }
                        }
                    }
                }
                else if (fieldNode.Name.Equals(Properties.Resources.Fields_Loop_Tag))
                {

                    XmlCommandFields fields = new XmlCommandFields(_parentFields, _structType);

                    String name = XmlGetter.Attribute(fieldNode, Properties.Resources.Fields_Loop_Name_Attr);
                    
                    fields.LoadXml(xDoc, fieldNode, refLoad);
                    TemplateFields = fields;
                    fields.Name = name;
                    fields.TemplateName = name;
                    //NumberOfFields = Convert.ToInt32(SizeDefineField.DataValue);//default로 지정된 크기를 만들어준다.

                    //_fieldsList.Add(name, fields);
                }
            }

            setBuffSize(); //size를 fix함..
            int offset = 0;
            for (int i = 0; i < _fieldList.Values.Count; i++)
            {
                _fieldList.ValueList[i].SetTargetBuffer(this.buffer, offset);
                if(_fieldList.ValueList[i].FieldType == FieldTypes.Dynamic)
                    _fieldList.ValueList[i].Data = _fieldList.ValueList[i].Data;//실제 값을 넣어준다.
                else if (_fieldList.ValueList[i].FieldType == FieldTypes.LoopCount)
                {
                    _fieldList.ValueList[i].Data = "0";
                }
                else
                {
                    _fieldList.ValueList[i].Data = _fieldList.ValueList[i].Data;//실제 값을 넣어준다.
                }
                offset += _fieldList.ValueList[i].DataSize;
            }
        }

        String _filePath = "";
        public string FilePath
        {
            get { return _filePath; }
        }

        public Type Type
        {
            get { return typeof(XmlImageList); }
        }

        public XmlNode GetXml(XmlDocument xDoc, XmlNode parent = null)
        {
            XmlNode root = XmlAdder.Element(xDoc, TemplateName, parent);
            
            foreach (XmlCommandField field in FieldList.Values)
            {
                if (field.FieldType == FieldTypes.Dynamic || field.FieldType == FieldTypes.Variable)
                {
                    XmlAdder.Attribute(xDoc, field.Name, field.Data, root);
                }
            }

            foreach (XmlCommandFields fields in FieldsList.Values)
            {
                fields.GetXml(xDoc, root);
            }

            return root;
        }

        XmlItemInterface _interface;
        public XmlItemInterface Interface { get { return _interface; } set { _interface = value; } }


        /// <summary>
        /// 시나리오에서 데이터를 가져와 필드에 값을 넣어주는 함수이다.
        /// </summary>
        /// <param name="xDoc"></param>
        /// <param name="rootNode"></param>
        internal void loadFields(XmlDocument xDoc, XmlNode rootNode)
        {
            foreach (String name in FieldList.Keys)
            {
                string value = XmlGetter.Attribute(rootNode, name);

                XmlCommandField field = FieldList[name];
                if(value.Length>0) field.Data = value; //값 설정.
                /*
                if (field.FieldType == FieldTypes.LoopCount)
                {
                    _loopCountField = field;

                }
                else if (field.FieldType == FieldTypes.VariableSize)
                {
                    _
                }
                 */
            }
            
            if (TemplateFields != null)
            {
                string fieldsName = TemplateFields.Name;
                int count = 0;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Name.Equals(fieldsName)) count++; //이름이 같은 Tag만 가져와 count함..
                }
                NumberOfFields = count; //방의 크기를 조정함..

                int index = 0;
                foreach (XmlNode node in rootNode.ChildNodes) //값을 채워넣음..
                {
                    if (node.Name.Equals(fieldsName))
                    {
                        FieldsList.ValueList[index++].loadFields(xDoc, node);
                    }
                }
            }

        }

        internal XmlCommandField Field(string fieldName, bool ignoreCase=true)
        {
            if (ignoreCase)
            {
                foreach(String key in _fieldList.Keys)// (int i = 0; i < _fieldList.Count; i++)
                {
                    if (fieldName.ToLower().Equals(key.ToLower())) return _fieldList[key];
                }
                throw new Exception("fieldName[" + fieldName + "] is not in this command..");
            }
            else
            {
                return _fieldList[fieldName];
            }
        }

        /// <summary>
        /// sourceBuffer로부터 값을 받아와서 값을 채워준다.
        /// 이 때, 버퍼의 크기가 동적이라면 다시만들어서 채워준다.
        /// 이 함수는 responseData를 채워넣을 때에만 유효하다.
        /// </summary>
        /// <param name="sourceBuffer"></param>
        /// <param name="startIndex"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public int FillDataFromNetworkBuffer(Array sourceBuffer, int startIndex, int size)
        {
            int offset = startIndex;
            int variableSize = 0;
            foreach (XmlCommandField field in FieldList.Values)
            {
                if (field.FieldType == FieldTypes.VariableSize)
                {
                    field.SetTargetBuffer(new Byte[VariableSize], 0); //size만큼 크기를 키움..
                }
                
                Buffer.BlockCopy(sourceBuffer, offset, field.TargetBuffer, 0, field.DataSize);
                
                if (field.FieldType == FieldTypes.LoopCount) //반복만큼 크기를 키움..
                {
                    NumberOfFields = NumberOfFields;
                }
                
                offset += field.DataSize;
            }

            Buffer.BlockCopy(sourceBuffer, startIndex, this.ArrayBuffer, 0, bufferByteSize);
            startIndex+=bufferByteSize;

            int beforeIndex;
            int nextSize = size-bufferByteSize;
            foreach (XmlCommandFields fields in FieldsList.Values)
            {
                beforeIndex = startIndex;
                
                startIndex = fields.FillDataFromNetworkBuffer(sourceBuffer, startIndex, nextSize);
                nextSize -= (beforeIndex - startIndex); 
            }
            return startIndex;
        }
        /*
        public void resetFields()
        {
            int offset = 0;
            foreach (XmlCommandField field in _fieldList.Values)
            {
                field.SetTargetBuffer(this.buffer, offset);
                offset += field.DataSize;
            }
        }
         */
    }
}
