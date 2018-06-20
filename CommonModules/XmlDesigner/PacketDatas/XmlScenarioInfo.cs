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
    public class XmlScenarioInfo: IXmlItem
    {
        public XmlScenarioInfo()
        {
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

        public String Version;
        public String Writer;
        public DateTime LastModified;

        public void LoadXml(XmlDocument xDoc, XmlNode rootNode, Boolean refLoad = false)
        {
            if (rootNode == null) return;
            _xDoc = xDoc;  
            XmlControlHandler.GetDefaultXmlItemAttributes(rootNode, xDoc, this);

            XmlNode node;
            if ((node = XmlGetter.Child(rootNode, "Version")) != null)
            {
                Version = node.InnerText;
            }
            else Version = XmlGetter.Attribute(rootNode, "Version");

            if ((node = XmlGetter.Child(rootNode, "Writer")) != null)
            {
                Writer = node.InnerText;
            }
            else Writer = XmlGetter.Attribute(rootNode, "Writer");

            if ((node = XmlGetter.Child(rootNode, "LastModified")) != null)
            {
                if (DateTime.TryParse(node.InnerText, out LastModified) == false)
                {
                    LastModified = DateTime.Now;
                }
            }
            else
            {
                if (DateTime.TryParse(XmlGetter.Attribute(rootNode, "LastModified"), out LastModified) == false)
                {
                    LastModified = DateTime.Now;
                }
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
            XmlNode root = XmlAdder.Element(xDoc, "Info", parent);
            XmlElement element = XmlAdder.Element(xDoc, "Version", root);
            element.InnerText = Version;

            element = XmlAdder.Element(xDoc, "Writer", root);
            element.InnerText = Writer;

            element = XmlAdder.Element(xDoc, "LastModified", root);
            element.InnerText = LastModified.ToString();
            return root;
        }

        XmlItemInterface _interface;
        public XmlItemInterface Interface { get { return _interface; } set { _interface = value; } }
    }
}
