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
    public class XmlPacketData : NetworkPacketWith<byte>, IXmlItem
    {
        public XmlPacketData()
        {
        }

        XmlDocument _xDoc;
        public void LoadXml(String xmlFile, Boolean refLoad = false)
        {
            if (XmlScenario.NowLoadingPath.Length > 0) _filePath = XmlScenario.NowLoadingPath + XmlScenario.PathSeperator + xmlFile;
            else _filePath = xmlFile;

            XmlNode xNode = XmlGetter.RootNode(out _xDoc, _filePath, null, XmlSchemaValidation);

            try
            {
                LoadXml(_xDoc, xNode, refLoad);
            }
            catch (Exception e)
            {
                MessageBox.Show("XmlPacketData.LoadXml:" + e.Message + ":" + xmlFile);
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
            _xDoc = xDoc; XmlControlHandler.GetDefaultXmlItemAttributes(rootNode, xDoc, this);

            XmlNodeList fieldNodeList = XmlGetter.Children(rootNode, "Fields/Field");
            int count = 0;
            int index = 0;
            foreach (XmlNode fieldNode in fieldNodeList)
            {

                XmlField field = new XmlField(new byte[1], count);
                
                field.LoadXml(xDoc, fieldNode);

                _fields.Add(field.Name, field);
                if (field.Count > 1)
                {
                    for (int i = 0; i < field.Count; i++)
                    {
                        addItem(index++, field.DataTypeSize, true);
                    }
                }
                else
                {
                    addItem(index++, field.DataTypeSize, true);
                }
                
                count += field.Count * field.DataTypeSize;
            }
            setBuffSize(); //size를 fix함..
            for (int i = 0; i < _fields.Values.Count; i++)
            {
                _fields.Values.ElementAt(i).TargetBuffer = this.buffer;
                _fields.Values.ElementAt(i).Data = _fields.Values.ElementAt(i).Data;//실제 값을 넣어준다.
            }
        }

        CustomDictionary<String, XmlField> _fields = new CustomDictionary<string, XmlField>();

        public XmlField Field(String Name)
        {
            if (_fields.ContainsKey(Name)) return _fields[Name];
            else return null;
        }

        public CustomDictionary<String, XmlField> Fields
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
