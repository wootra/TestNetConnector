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
    public class XmlCommandHeader : NetworkPacketWith<byte>, IXmlItem
    {
        public XmlCommandHeader()
        {
        }

        XmlDocument _xDoc;
        public void LoadXml(String xmlFile, Boolean refLoad = false)
        {
            if (XmlScenario.NowLoadingPath.Length > 0) _filePath = XmlScenario.NowLoadingPath + XmlScenario.PathSeperator + xmlFile;
            else _filePath = xmlFile;

            XmlNode xNode = XmlGetter.RootNode(out _xDoc, _filePath, null, XmlSchemaValidation);

            //try
            {
                LoadXml(_xDoc, xNode, refLoad);
            }
            //catch (Exception e)
            {
                //MessageBox.Show("XmlPacketData.LoadXml:" + e.Message + ":" + xmlFile);
            }
        }

        public int GetDataToBuffer(Array targetBuffer, int nowOffset)
        {
            foreach (IPacketItem item in FieldList.Values)
            {
                XmlCommandField field = item as XmlCommandField;
                nowOffset = field.GetDataToBuffer(targetBuffer, nowOffset);
            }
            return nowOffset;
        }

        void XmlSchemaValidation(object sender, ValidationEventArgs e)
        {
            MessageBox.Show(e.Message);
        }


        public void SaveXml(String xmlFile)
        {

        }

        XmlCommandField _dataSizeField;

        public XmlCommandField DataSizeField
        {
            get { return _dataSizeField; }
        }
        XmlCommandField _idField;
        XmlCommandField _tagField;

        public XmlCommandField IdField
        {
            get { return _idField; }
        }

        public string Id
        {
            get { return _idField.Data; }
        }

        public XmlCommandField TagField
        {
            get { return _tagField; }
        }

        public XmlCommandHeader Clone()
        {
            XmlCommandHeader newHeader = new XmlCommandHeader();
            int count = 0; 
            foreach (XmlCommandField field in FieldList.Values)
            {
                XmlCommandField newField = field.Clone();
                newHeader.FieldList.Add(field.Name, newField);
                newHeader.addItem(count++, field.DataSize, true);
                if (field.FieldType == FieldTypes.DataSize)
                {
                    newHeader._dataSizeField = newField;
                }
                else if (field.Name.ToLower().Equals("id"))
                {
                    newHeader._idField = newField;
                }
                else if (field.Name.ToLower().Equals("tag"))
                {
                    newHeader._tagField = newField;
                }
            }
            newHeader.setBuffSize();
            int offset = 0;
            foreach (XmlCommandField field in newHeader.FieldList.Values)
            {
                field.SetTargetBuffer(newHeader.buffer, offset);
                offset+=field.DataSize;
                field.Data = FieldList[field.Name].Data;
            }

            return newHeader;
        }

        public void LoadXml(XmlDocument xDoc, XmlNode rootNode, Boolean refLoad = false)
        {
            if (rootNode == null) return;
            _xDoc = xDoc; XmlControlHandler.GetDefaultXmlItemAttributes(rootNode, xDoc, this);


            XmlNodeList fields = XmlGetter.Children(rootNode, "Field");
            int count = 0;
            foreach (XmlNode field in fields)
            {
                XmlCommandField xField = new XmlCommandField(null);
                xField.LoadXml(xDoc, field, refLoad);

                FieldList.Add(xField.Name, xField);
                if (xField.FieldType == FieldTypes.DataSize)
                {
                    _dataSizeField = xField;
                }
                else if (xField.Name.ToLower().Equals("id"))
                {
                    _idField = xField;
                }
                else if (xField.Name.ToLower().Equals("tag"))
                {
                    _tagField = xField;
                }

                addItem(count++, xField.DataSize, true);
            }
            setBuffSize();
            int offset = 0;
            foreach (XmlCommandField field in FieldList.Values)
            {
                field.SetTargetBuffer(this.buffer, offset);
                field.Data = field.Data;
                offset += field.DataSize;
            }

            /*
            XmlCommandFields xmlFields = new XmlCommandFields(null);
            xmlFields.LoadXml(xDoc, fields, refLoad);
            */
        }

        ListDic<String, XmlCommandField> _fields = new ListDic<string, XmlCommandField>();

        public XmlCommandField Field(String Name, bool ignoreCase=true)
        {
            if (ignoreCase)
            {
                foreach (String name in _fields.Keys)
                {
                    if (name.ToLower().Equals(Name.ToLower())) return _fields[name];
                }
                return null;
            }
            else
            {
                if (_fields.ContainsKey(Name)) return _fields[Name];
                else return null;
            }
        }

        public ListDic<String, XmlCommandField> FieldList
        {
            get { return _fields; }
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

            return null;
        }

        XmlItemInterface _interface;
        public XmlItemInterface Interface { get { return _interface; } set { _interface = value; } }
    }
}
