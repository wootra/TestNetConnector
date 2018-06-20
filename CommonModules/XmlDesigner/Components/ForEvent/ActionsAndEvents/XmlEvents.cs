using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FormAdders;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Linq;
using System.Xml.Schema;

using XmlHandlers;
using FormAdders.EasyGridViewCollections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.IO;
using System.Windows.Forms;

using XmlDesigner.ForEvents.Conditions;

namespace XmlDesigner.ForEvents
{
    public class XmlEvents : Dictionary<String, XmlEvent>, IXmlItem
    {
       
        XmlDocument _xDoc;
        //String[] _imageLayouts = new String[] { "None","Tile","Center","Stretch","Zoom"};


        public event ValidationEventHandler E_XmlSchemaValidation;
        
        public XmlEvents()
            : base()
        {
            
        }

      
          public void LoadXml(String xmlFile, Boolean refLoad=false)
        {
            //DataSet dt = new DataSet();
            //dt.ReadXmlSchema(Properties.Resources.TableSchema);
            //dt.ReadXml(xmlFile); //schema를 불러와서 체크하기 위하여..
            if (XmlLayoutCollection.NowLoadingPath.Length > 0) _filePath = XmlLayoutCollection.NowLoadingPath +  XmlLayoutCollection.PathSeperator + xmlFile;
            else _filePath = xmlFile;

            XmlNode xNode = XmlGetter.RootNode(out _xDoc, _filePath, "./ComponentSchemas/Rules/GlobalTypeRuleSchema.xsd", XmlSchemaValidation);
            /*
            _xDoc = new XmlDocument();
            _xDoc.PreserveWhitespace = false;
            _xDoc.Schemas = new System.Xml.Schema.XmlSchemaSet();
            XmlSchema schema = XmlSchema.Read(File.OpenRead("./ComponentSchemas/LabelSchema.xsd"), XmlScenarioTable_E_XmlSchemaValidation);
            _xDoc.Schemas.Add(schema);

            _xDoc.Load(xmlFile);

                        
            xNode = _xDoc.SelectSingleNode("//Label");
             */
            try
            {
                LoadXml(_xDoc, xNode, refLoad);
            }
            catch (Exception e)
            {
                MessageBox.Show("XmlEvents.LoadXml:" + e.Message + ":" + xmlFile);
            }
        }

 

        void XmlSchemaValidation(object sender, ValidationEventArgs e)
        {
            MessageBox.Show("XmlEvents.XmlSchemaValidation:" + e.Message);
        }


        public void SaveXml(String xmlFile)
        {

        }



        public void LoadXml(XmlDocument xDoc, XmlNode rootNode, Boolean refLoad=false)
        {
            //this.Clear();
            if (rootNode == null) return;
            XmlControlHandler.GetDefaultXmlItemAttributes(rootNode, xDoc, this);

            if (rootNode == null || rootNode.Name.Equals("Events")==false)
            {
                throw new Exception("올바른 Events Tag가 아닙니다. 루트가 Events 가 아닙니다. ");
            }
            
            for (int i = 0; i < rootNode.ChildNodes.Count; i++)
            {
                XmlNode xEvent = rootNode.FirstChild; //OnClick, OnDoubleClick....등등
                String eType = xEvent.Name;// (EventTypes)(GlobalVars.EventTypes.ToList().IndexOf(xEvent.Name));
                if (eType.Equals("CustomEvent")) eType = XmlGetter.Attribute(xEvent, "Name");
                //if ((int)eType < 0) throw new Exception(xEvent.Name + "는 올바른 EventType 태그가 아닙니다." + String.Concat(GlobalVars.EventTypes));

                XmlEvent xe = new XmlEvent();
                xe.LoadXml(xDoc, xEvent);
                this[eType] = xe;
                
            }
        }

        String _filePath = "";
        public string FilePath
        {
            get { return _filePath; }
        }


        public XmlItemTypes XmlItemType
        {
            get { return XmlItemTypes.EventList; }
        }

        public Type Type
        {
            get { return typeof(XmlEvents); }
        }

        void IXmlItem.LoadXml(string xmlFile, bool refLoad)
        {
            LoadXml(xmlFile, refLoad);
        }

        public XmlNode GetXml(XmlDocument xDoc, XmlNode parent = null)
        {
            XmlNode eventsNode = XmlAdder.Element(xDoc, "Events", parent);
            for (int i = 0; i < this.Count; i++)
            {
                this.Values.ElementAt(i).GetXml(xDoc, eventsNode);
            }
            return eventsNode;
        }
        XmlItemInterface _interface;
        public XmlItemInterface Interface { get { return _interface; } set { _interface = value; } }
    }
}
