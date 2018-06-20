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

namespace XmlDesigner.PacketDatas
{
    public class XmlField: IXmlItem
    {
        public XmlField(Array targetBuffer, int offset)
        {
            Offset = offset;
            TargetBuffer = targetBuffer;
        }

        

        public String Name = "";
        public Type DataType = typeof(int);
        public int DataTypeSize = 4;
        String _dataString = "0";
        public int Count = 1;
        public int Offset = 0;
        public Array TargetBuffer;

        public String Data
        {
            get
            {
                //_dataString = DataValue.ToString();
                return _dataString;
            }
            set
            {
                _dataString = value;
                this.Interface.Node.Attributes["Data"].Value = _dataString;
                if (_dataString.Length == 0) _dataString = "0";
                DataHandling.TypeHandling.parseNumberToByteArray(_dataString, TargetBuffer, Offset, DataType, true);

            }
        }

        public object DataValue
        {
            get
            {
                bool unsigned = (DataType == typeof(byte) || DataType == typeof(ushort) || DataType == typeof(uint) || DataType == typeof(ulong));
                object value;
                if (DataType == typeof(float))
                {
                    value = TypeArrayConverter.CopyBufferToVariable<float>(TargetBuffer, Offset, DataTypeSize, unsigned, true);
                }
                else if (DataType == typeof(double))
                {
                    value = TypeArrayConverter.CopyBufferToVariable<double>(TargetBuffer, Offset, DataTypeSize, unsigned, true);
                }
                else if (DataType == typeof(byte))
                {
                    value = TypeArrayConverter.CopyBufferToVariable<byte>(TargetBuffer, Offset, DataTypeSize, unsigned, true);
                }
                else if (DataType == typeof(short))
                {
                    value = TypeArrayConverter.CopyBufferToVariable<short>(TargetBuffer, Offset, DataTypeSize, unsigned, true);
                }
                else if (DataType == typeof(int))
                {
                    value = TypeArrayConverter.CopyBufferToVariable<int>(TargetBuffer, Offset, DataTypeSize, unsigned, true);
                }
                else if (DataType == typeof(long))
                {
                    value = TypeArrayConverter.CopyBufferToVariable<long>(TargetBuffer, Offset, DataTypeSize, unsigned, true);
                }
                else if (DataType == typeof(sbyte))
                {
                    value = TypeArrayConverter.CopyBufferToVariable<sbyte>(TargetBuffer, Offset, DataTypeSize, unsigned, true);
                }
                else if (DataType == typeof(ushort))
                {
                    value = TypeArrayConverter.CopyBufferToVariable<ushort>(TargetBuffer, Offset, DataTypeSize, unsigned, true);
                }
                else if (DataType == typeof(uint))
                {
                    value = TypeArrayConverter.CopyBufferToVariable<uint>(TargetBuffer, Offset, DataTypeSize, unsigned, true);
                }
                else if (DataType == typeof(ulong))
                {
                    value = TypeArrayConverter.CopyBufferToVariable<ulong>(TargetBuffer, Offset, DataTypeSize, unsigned, true);
                }
                else
                {
                    value = TypeArrayConverter.CopyBufferToVariable<int>(TargetBuffer, Offset, DataTypeSize, unsigned, true);
                }
                
                _dataString = value.ToString();
                return value;
            }
            set
            {
                if (value is int) TypeArrayConverter.FillBufferUnitsFrom<int>((int)value, TargetBuffer, Offset, true);
                _dataString = value.ToString();
            }
        }

        XmlDocument _xDoc;  public void LoadXml(String xmlFile, Boolean refLoad = false)
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


        public void LoadXml(XmlDocument xDoc, XmlNode rootNode, Boolean refLoad = false)
        {
            if (rootNode == null) return;
            _xDoc = xDoc;  XmlControlHandler.GetDefaultXmlItemAttributes(rootNode, xDoc, this);

            Name = XmlGetter.Attribute(rootNode, "Name");
            if (Name.Length == 0) throw new Exception("Name이 정의되지 않았습니다.");
            String typeText = XmlGetter.Attribute(rootNode, "Type");
            if (typeText.Length == 0) throw new Exception("Type이 정의되지 않았습니다.");
            DataType = TypeHandling.getTypeFromTypeName(typeText);

            String sizeText = XmlGetter.Attribute(rootNode, "Size");
            if (sizeText.Length == 0 || int.TryParse(sizeText, out DataTypeSize) == false)
            {
                DataTypeSize = Marshal.SizeOf(DataType);
            }

            String countText = XmlGetter.Attribute(rootNode, "Count");
            if (countText.Length == 0 || int.TryParse(countText, out Count) == false)
            {
                Count = 1;
            }
            if (Count == 0) Count = 1;//Count 0은 의미가 없음..

            _dataString = XmlGetter.Attribute(rootNode, "Data");
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
