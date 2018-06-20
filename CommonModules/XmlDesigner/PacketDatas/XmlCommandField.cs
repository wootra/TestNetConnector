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
using System.Runtime.InteropServices;
using System.Globalization;

namespace XmlDesigner.PacketDatas
{
    public class XmlCommandField: IXmlItem, IPacketItem
    {
        public XmlCommandField(XmlCommandFields parentFields) {
            _parentFields = parentFields;
        }

        /// <summary>
        /// Data에서 기본적으로 사용할 buffer를 지정한다.
        /// </summary>
        /// <param name="targetBuffer"></param>
        /// <param name="offset"></param>
        public void SetTargetBuffer(Array targetBuffer, int offset)
        {
            if (FieldType == FieldTypes.Variable)
            {

            }
            else
            {
                Offset = offset;

                TargetBuffer = targetBuffer;
                String str = ""+_dataString;
                if(_dataString!=null && _dataString.Length>0) Data = str;
            }
        }

        XmlCommandFields _parentFields;
        public XmlCommandFields ParentFields
        {
            get { return _parentFields; }
        }

        /// <summary>
        /// targetBuffer에 이 Field의 내용을 복사하고, 다음 offset을 리턴한다.
        /// </summary>
        /// <param name="targetBuffer"></param>
        /// <param name="nowOffset"></param>
        /// <returns></returns>
        public int GetDataToBuffer(Array targetBuffer, int nowOffset)
        {
            if (FieldType == FieldTypes.Variable)
            {
                Buffer.BlockCopy(TargetBuffer, 0, targetBuffer, nowOffset, DataSize);
                
            }
            else
            {
                DataHandling.TypeHandling.parseNumberToByteArray(_dataString, targetBuffer, nowOffset, DataType, true);
                
            }
            return nowOffset + DataSize;
        }
        

        public String Name = "";
        public Type DataType = typeof(int);
        //int _dataSize = 4;
        public int DataSize
        {
            get
            {
                if (FieldType == FieldTypes.Variable)
                {
                    if (TargetBuffer == null) return 0;
                    else return Buffer.ByteLength(TargetBuffer);
                }
                else
                {
                    return Marshal.SizeOf(DataType);// return _dataSize;
                }
            }
        }
        String _dataString = "0";
        public int Offset = 0;
        public Array TargetBuffer;
        public FieldTypes FieldType;
        public DynamicFieldTypes DynamicFieldType;
        public AutoFieldTypes AutoFieldType;

        public XmlCommandField Clone()
        {
            XmlCommandField field = new XmlCommandField(_parentFields);
            field.Name = ""+this.Name;
            field.DataType = this.DataType;
            //field._dataSize = this.DataSize;
            field._dataString = ""+this._dataString;

            field.FieldType = this.FieldType;
            field.DynamicFieldType = this.DynamicFieldType;
            field.AutoFieldType = this.AutoFieldType;
            field.Offset = this.Offset;

            field._parentFields = this._parentFields;
            
            if (FieldType == FieldTypes.Variable)
            {
                if (TargetBuffer == null || TargetBuffer.Length == 0)
                {
                    //do nothing..
                }
                else
                {
                    int dataSize = Buffer.ByteLength(TargetBuffer);
                    field.TargetBuffer = new Byte[dataSize];
                    Buffer.BlockCopy(TargetBuffer, 0, field.TargetBuffer, 0, dataSize);
                }
            }
            //field.TargetBuffer = this.TargetBuffer;
            
            return field;

        }

        /// <summary>
        /// 값을 넣으면 데이터를 버퍼에 넣는다.
        /// </summary>
        public String Data
        {
            get
            {
                //_dataString = DataValue.ToString();
                return _dataString;
            }
            set
            {
                try
                {
                    if (FieldType == FieldTypes.Variable)
                    { //hex로 처리
                        List<Byte> charList = new List<byte>();
                        string token = "";
                        for (int i = 0; i < value.Length; i++)
                        {
                            Char a = value[i];

                            if (a.Equals(' ') || a.Equals(',') || a.Equals('/'))
                            {
                                if (token.Length % 2 == 1) token = '0' + token; //짝수가 아니라면 앞에 0을 붙여서 포멧을 0a식으로 맞춤
                                else continue;//이미 이전에 다 처리되었음..
                            }
                            else
                            {
                                token += a;
                            }

                            byte outVar;

                            if (token.Length % 2 == 0) //1byte의 크기(2개)가 되었다면 일단 hex로 바꿈...
                            {
                                if (token.ToLower().Equals("0x")) //0x라면 무시하고 다음을 읽음..
                                {
                                    token = "";
                                    continue;
                                }

                                if (byte.TryParse(token, System.Globalization.NumberStyles.HexNumber, CultureInfo.CurrentCulture, out outVar))
                                {
                                    charList.Add(outVar);
                                    token = "";
                                }
                                else
                                {
                                    throw new Exception("" + (charList.Count + 1) + "th item is out of format...");
                                }
                            }
                            else
                            {
                                if (i == value.Length - 1)//마지막이라면 넣어야 한다.
                                {
                                    if (byte.TryParse(token, System.Globalization.NumberStyles.HexNumber, CultureInfo.CurrentCulture, out outVar))
                                    {
                                        charList.Add(outVar);
                                        token = "";
                                    }
                                    else
                                    {
                                        throw new Exception("" + (charList.Count + 1) + "th item is out of format...");
                                    }
                                }
                            }
                        }
                        _dataString = value;
                        TargetBuffer = charList.ToArray();
                        
                        #region old
                        /*
                        String[] tokens = value.Split(", ".ToCharArray());
                        Byte[] buff = new byte[tokens.Length];
                        bool isNumArray = (tokens.Length >= 2) ? true : false;
                        if (isNumArray)
                        {

                            for (int i = 0; i < tokens.Length; i++)
                            {
                                string token = tokens[i].Trim();
                                byte outVar;
                                if (token.Length == 0)
                                {
                                    buff[i] = 0;//콤마 사이에 아무것도 없으면 0으로 간주함..
                                }
                                else if (token.Length>2 && token.Substring(0, 2).ToLower().Equals("0x"))//hex
                                {
                                    if (byte.TryParse(token.Substring(2), System.Globalization.NumberStyles.HexNumber, CultureInfo.CurrentCulture, out outVar))
                                    {
                                        buff[i] = outVar;
                                    }
                                    else
                                    {
                                        isNumArray = false;
                                    }
                                }
                                else if (byte.TryParse(token, out outVar))//digit
                                {
                                    buff[i] = outVar;
                                }
                                else //string
                                {
                                    throw new Exception("" + i + "th item is out of format...");
                                    //isNumArray = false;//멈추고 바로 string으로 취급하여 분석한다.
                                   // break;
                                }
                            }
                        }

                        if (isNumArray) TargetBuffer = buff;
                        else TargetBuffer = Encoding.UTF8.GetBytes(value);
                        _dataString = value;
                         */
                        #endregion
                    }
                    else
                    {

                        //if(this.Interface.Node.Attributes["Data"]
                        //this.Interface.Node.Attributes["Data"].Value = _dataString;
                        if (value == null || value.Length == 0)
                        {
                            _dataString = "0";
                        }
                        else
                        {
                            _dataString = "" + value;
                            if (TargetBuffer != null)
                            {
                                DataHandling.TypeHandling.parseNumberToByteArray(value, TargetBuffer, Offset, DataType, true);

                            }
                        }



                    }

                }
                catch (Exception e)
                {
                    throw new Exception("Field[" + Name + "] Load중 에러 :" + e.Message);
                }
            }
        }

        public object DataValue
        {
            get
            {
                if (FieldType == FieldTypes.Variable)
                {
                    if(TargetBuffer==null) return null;

                    Byte[] buff = new Byte[DataSize];

                    Buffer.BlockCopy(TargetBuffer, Offset, buff, 0, DataSize);
                    
                    return buff;

                }else{
                    bool unsigned = (DataType == typeof(byte) || DataType == typeof(ushort) || DataType == typeof(uint) || DataType == typeof(ulong));
                    object value;
                    if (TargetBuffer != null)
                    {
                        if (DataType == typeof(float))
                        {
                            value = TypeArrayConverter.CopyBufferToVariable<float>(TargetBuffer, Offset, DataSize, unsigned, true);
                        }
                        else if (DataType == typeof(double))
                        {
                            value = TypeArrayConverter.CopyBufferToVariable<double>(TargetBuffer, Offset, DataSize, unsigned, true);
                        }
                        else if (DataType == typeof(byte))
                        {
                            value = TypeArrayConverter.CopyBufferToVariable<byte>(TargetBuffer, Offset, DataSize, unsigned, true);
                        }
                        else if (DataType == typeof(short))
                        {
                            value = TypeArrayConverter.CopyBufferToVariable<short>(TargetBuffer, Offset, DataSize, unsigned, true);
                        }
                        else if (DataType == typeof(int))
                        {
                            value = TypeArrayConverter.CopyBufferToVariable<int>(TargetBuffer, Offset, DataSize, unsigned, true);
                        }
                        else if (DataType == typeof(long))
                        {
                            value = TypeArrayConverter.CopyBufferToVariable<long>(TargetBuffer, Offset, DataSize, unsigned, true);
                        }
                        else if (DataType == typeof(sbyte))
                        {
                            value = TypeArrayConverter.CopyBufferToVariable<sbyte>(TargetBuffer, Offset, DataSize, unsigned, true);
                        }
                        else if (DataType == typeof(ushort))
                        {
                            value = TypeArrayConverter.CopyBufferToVariable<ushort>(TargetBuffer, Offset, DataSize, unsigned, true);
                        }
                        else if (DataType == typeof(uint))
                        {
                            value = TypeArrayConverter.CopyBufferToVariable<uint>(TargetBuffer, Offset, DataSize, unsigned, true);
                        }
                        else if (DataType == typeof(ulong))
                        {
                            value = TypeArrayConverter.CopyBufferToVariable<ulong>(TargetBuffer, Offset, DataSize, unsigned, true);
                        }
                        else if (DataType == typeof(float))
                        {
                            value = TypeArrayConverter.CopyBufferToVariable<float>(TargetBuffer, Offset, DataSize, unsigned, true);
                        }
                        else if (DataType == typeof(double))
                        {
                            value = TypeArrayConverter.CopyBufferToVariable<double>(TargetBuffer, Offset, DataSize, unsigned, true);
                        }
                        else
                        {
                            value = TypeArrayConverter.CopyBufferToVariable<int>(TargetBuffer, Offset, DataSize, unsigned, true);
                        }

                        _dataString = value.ToString();
                        return value;
                    }else return 0;
                }
                
                
            }
            set
            {
                if (FieldType == FieldTypes.Variable)
                {
                    if (value is Byte[])
                    {
                        int dataSize = Buffer.ByteLength(value as Byte[]);

                        TargetBuffer = new Byte[dataSize];
                        Buffer.BlockCopy(value as Byte[], 0, TargetBuffer, 0, dataSize);
                        string str = "";
                        for (int i = 0; i < TargetBuffer.Length; i++)
                        {
                            if (i != 0) str += " ";
                            str += string.Format("{0:X2}", (TargetBuffer as Byte[])[i]);
                        }
                        _dataString = str;
                        
                    }
                    else if (value is string)
                    {
                        TargetBuffer = Encoding.UTF8.GetBytes(value as String);
                        _dataString = value as String;
                    }
                }
                else
                {
                    setData(value);
                    _dataString = value.ToString();
                }
            }
        }

        /// <summary>
        /// FieldType이 variable이 아닌 경우 값을 setting하는 함수..
        /// </summary>
        /// <param name="value"></param>
        void setData(object value) //variable이 아닌 경우 값 setting..
        {

            if (TargetBuffer != null)
            {
                if (DataType.Equals(typeof(int))) TypeArrayConverter.FillBufferUnitsFrom<int>(Convert.ToInt32(value), TargetBuffer, Offset, true);
                else if (DataType.Equals(typeof(byte))) TypeArrayConverter.FillBufferUnitsFrom<byte>(Convert.ToByte(value), TargetBuffer, Offset, true);
                else if (DataType.Equals(typeof(short))) TypeArrayConverter.FillBufferUnitsFrom<short>(Convert.ToInt16(value), TargetBuffer, Offset, true);
                else if (DataType.Equals(typeof(long))) TypeArrayConverter.FillBufferUnitsFrom<long>(Convert.ToInt64(value), TargetBuffer, Offset, true);
                else if (DataType.Equals(typeof(uint))) TypeArrayConverter.FillBufferUnitsFrom<uint>(Convert.ToUInt32(value), TargetBuffer, Offset, true);
                else if (DataType.Equals(typeof(sbyte))) TypeArrayConverter.FillBufferUnitsFrom<sbyte>(Convert.ToSByte(value), TargetBuffer, Offset, true);
                else if (DataType.Equals(typeof(ulong))) TypeArrayConverter.FillBufferUnitsFrom<ulong>(Convert.ToUInt64(value), TargetBuffer, Offset, true);
                else if (DataType.Equals(typeof(ushort))) TypeArrayConverter.FillBufferUnitsFrom<ushort>(Convert.ToUInt16(value), TargetBuffer, Offset, true);
                else if (DataType.Equals(typeof(float))) TypeArrayConverter.FillBufferUnitsFrom<float>(Convert.ToSingle(value), TargetBuffer, Offset, true);
                else if (DataType.Equals(typeof(double))) TypeArrayConverter.FillBufferUnitsFrom<double>(Convert.ToDouble(value), TargetBuffer, Offset, true);
                else TypeArrayConverter.FillBufferUnitsFrom<int>(Convert.ToInt32(value), TargetBuffer, Offset, true);
            }
        }
        XmlDocument _xDoc;  
        public void LoadXml(String xmlFile, Boolean refLoad = false)
        {
            if (XmlScenario.NowLoadingPath.Length > 0) _filePath = XmlScenario.NowLoadingPath + XmlScenario.PathSeperator + xmlFile;
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

        
        public void setFieldType(String typeText)
        {
            FieldType = FieldTypes.Dynamic;
            for (int i = 0; i < (int)FieldTypes.NULL; i++)
            {
                if (typeText.ToLower().Equals(((FieldTypes)i).ToString().ToLower())) FieldType = (FieldTypes)i;
            }
        }

/*        public void setDynamicFieldType(String typeText)
        {
            DynamicFieldType = DynamicFieldTypes.BoardID;
            for (int i = 0; i < (int)DynamicFieldTypes.NULL; i++)
            {
                if (typeText.ToLower().Equals(((DynamicFieldTypes)i).ToString().ToLower())) DynamicFieldType = (DynamicFieldTypes)i;
            }
        }

        public void setAutoFieldType(String typeText)
        {
            AutoFieldType = AutoFieldTypes.StartEventID;
            for (int i = 0; i < (int)AutoFieldTypes.NULL; i++)
            {
                if (typeText.ToLower().Equals(((AutoFieldTypes)i).ToString().ToLower())) AutoFieldType = (AutoFieldTypes)i;
            }
        }
        */
        //Encoding _stringEncoding;
        public void LoadXml(XmlDocument xDoc, XmlNode rootNode, Boolean refLoad = false)
        {
            if (rootNode == null) return;
            _xDoc = xDoc;  XmlControlHandler.GetDefaultXmlItemAttributes(rootNode, xDoc, this);

            Name = XmlGetter.Attribute(rootNode, Properties.Resources.Field_Name_Attr);
            if (Name.Length == 0) throw new Exception( Properties.Resources.Field_Name_Attr+ "이 정의되지 않았습니다.");
            
            String typeText = XmlGetter.Attribute(rootNode, Properties.Resources.Field_ValueType_Attr);// "ValueType");
            if (typeText.Length == 0) throw new Exception(Properties.Resources.Field_ValueType_Attr+ "이 정의되지 않았습니다.");
            DataType = TypeHandling.getTypeFromTypeName(typeText);


            setFieldType(XmlGetter.Attribute(rootNode, Properties.Resources.Field_FieldType_Attr));// "FieldType"));

            Data = XmlGetter.Attribute(rootNode, Properties.Resources.Field_Value_Attr);

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
            
            return this.Interface.Node;
        }

        XmlItemInterface _interface;
        public XmlItemInterface Interface { get { return _interface; } set { _interface = value; } }
    }
}
