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

namespace XmlDesigner.PacketDatas
{
    public class XmlCommandStructDefinition : IXmlItem
    {
        string _headerFileName;
        StructType _structType;
        public XmlCommandStructDefinition(String headerFileName, StructType structType)
            : base()
        {
            _structType = structType;

            _header = new XmlCommandHeader();
            _data = new XmlCommandFields(null, _structType);
            
        
            if (HeaderPath == null)
            {
                HeaderPath = Path.GetFullPath(Directory.GetCurrentDirectory() + "\\..\\TestNgineData\\Headers");
            }
            _headerFileName = headerFileName;
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

        public static string HeaderPath;

        public XmlCommandStructDefinition Clone()
        {
            XmlCommandStructDefinition pacektStruct = new XmlCommandStructDefinition(_headerFileName, _structType);
            pacektStruct._header = _header.Clone();
            pacektStruct._data = _data.Clone(_data.Name);
            return pacektStruct;
        }


        
        public XmlCommandHeader Header { get { return _header; } }

        XmlCommandFields _data;
        XmlCommandHeader _header;
        public XmlCommandFields Data { get { return _data; } }

        



        public void LoadXml(XmlDocument xDoc, XmlNode rootNode, Boolean refLoad = false)
        {
            if (rootNode == null) return;
            _xDoc = xDoc;  XmlControlHandler.GetDefaultXmlItemAttributes(rootNode, xDoc, this);

            XmlNode headerNode = XmlGetter.Child(rootNode, Properties.Resources.CommandHeader_Tag); //header 매칭..

            _header.LoadXml(HeaderPath+"\\"+_headerFileName, refLoad);
            if (headerNode != null) //Header 태그가 없으면 그냥 둔다.
            {
                foreach (String fieldName in _header.FieldList.Keys)
                {
                    String attr = XmlGetter.Attribute(headerNode, fieldName);
                    if (attr.Length > 0)
                    {
                        _header.FieldList[fieldName].Data = attr;
                    }
                }
            }
            //_header.LoadXml(_xDoc, headerNode);

            XmlNode dataNode = XmlGetter.Child(rootNode, Properties.Resources.CommandData_Tag);
            if(dataNode!=null) _data.LoadXml(_xDoc, dataNode);

        }

        String _filePath = "";
        public string FilePath
        {
            get { return _filePath; }
        }

        public Type Type
        {
            get { return this.GetType(); }
        }

        public XmlNode GetXml(XmlDocument xDoc, XmlNode parent = null)
        {

            return null;
        }

        /// <summary>
        /// offset부터 시작하여 내부의 내용을 복사하고, 다음으로 넣어야 할 offset을 리턴한다.
        /// </summary>
        /// <param name="targetBuffer">Target이 되는 Buffer</param>
        /// <param name="offset">byte size 버퍼 시작점.</param>
        /// <returns>다음 offset</returns>
        public int GetDataToBuffer(Array targetBuffer, int offset=0)
        {
            Header.DataSizeField.DataValue = _data.ByteSize;
            //offset += Header.GetDataToBuffer(targetBuffer, offset);//.copyBufferToArray(targetBuffer, offset);
            //offset += _data.Fields.GetDataToBuffer(targetBuffer, offset);
            offset += Header.copyBufferToArray(targetBuffer, offset);
            _data.GetDataToBuffer(targetBuffer, offset);
            offset += _data.copyBufferToArray(targetBuffer, offset);

            return offset;
        }

        public Byte[] GetBuffer()
        {
            int offset = 0;
            Byte[] buff = new Byte[Header.bufferByteSize + Data.ByteSize];
            Header.DataSizeField.DataValue = Data.ByteSize;
            if(Header.bufferByteSize>0) offset += Header.copyBufferToArray(buff, offset);
            Data.GetDataToBuffer(buff, offset);
            //if(Data.bufferByteSize>0) offset += Data.copyBufferToArray(buff, offset);
            return buff;
        }

        XmlItemInterface _interface;
        public XmlItemInterface Interface { get { return _interface; } set { _interface = value; } }
    }
}
