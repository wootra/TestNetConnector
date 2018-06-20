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
    public class XmlCommandInfo: IXmlItem
    {
        public XmlCommandInfo()
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
        String _name="";
        public String Name { 
            get { return _name; }
            set
            {
                _name = value;
                this.Interface.Node.Attributes["Name"].Value = value;
            }
        }
        
        String _description;
        public String Description { 
            get { return _description; }
            set
            {
                _description = value;
                this.Interface.Node.Attributes["Description"].Value = value;
            }
        }

        String _comment;
        public String Comment
        {
            get { return _comment; }
            set
            {
                _comment = value;
                this.Interface.Node.Attributes["Comment"].Value = value;
            }
        }
        
        PacketTypes _packetType = PacketTypes.IO;
        public PacketTypes PacketType
        {
            get { return _packetType; }
        }


        public void LoadXml(XmlDocument xDoc, XmlNode rootNode, Boolean refLoad = false)
        {
            if (rootNode == null) return;
            _xDoc = xDoc;  XmlControlHandler.GetDefaultXmlItemAttributes(rootNode, xDoc, this);

            _name = XmlGetter.Attribute(rootNode, "Name");
            //_template = XmlGetter.Attribute(rootNode, "Template");
            _description = XmlGetter.Attribute(rootNode, "Description");

            String packetType = XmlGetter.Attribute(rootNode, "Type");
            _packetType = PacketTypes.IO; //default..
            for(int i=(int)PacketTypes.IO; i<(int)PacketTypes.None; i++){
                if(packetType.Equals(((PacketTypes)i).ToString())) _packetType = (PacketTypes)i;
            }
            
        }

        String _filePath = "";
        public string FilePath
        {
            get { return _filePath; }
        }

        public Type Type
        {
            get { return typeof(XmlCommandInfo); }
        }

        public XmlNode GetXml(XmlDocument xDoc, XmlNode parent = null)
        {

            return null;
        }

        XmlItemInterface _interface;
        public XmlItemInterface Interface { get { return _interface; } set { _interface = value; } }

    }
    
}
