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
using XmlDesigner.ForEvents;


namespace XmlDesigner
{
    public class IXmlComponentBase : Control, IXmlComponent, IXmlItem
    {
        
        Control _control;
        Type _type;
        XmlDocument _xDoc;
        XmlItemTypes _comType;
        XmlEvents _events = new XmlEvents();
        public XmlEvents Events { get { return _events; } }

        public event ValidationEventHandler E_XmlSchemaValidation;

        public IXmlComponentBase()
            : base()
        {
            
        }

        public XmlItemTypes XmlItemType { get { return _comType; } }

        public Control RealControl { get { return this; } }

        public Type Type { get { return this.GetType(); } }
        
        String _filePath = "";
        public string FilePath
        {
            get { return _filePath; }
        }

        public void LoadXml(String xmlFile, Boolean refLoad=false)
        {
            if (XmlLayoutCollection.NowLoadingPath.Length > 0) _filePath = XmlLayoutCollection.NowLoadingPath +  XmlLayoutCollection.PathSeperator + xmlFile;
            else _filePath = xmlFile;

            XmlNode xNode = XmlGetter.RootNode(out _xDoc,_filePath, null, XmlScenarioTable_E_XmlSchemaValidation);
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
                MessageBox.Show("IXmlComponentBase.LoadXml1:" + e.Message + ":" + xmlFile);
            }
        }

 

        void XmlScenarioTable_E_XmlSchemaValidation(object sender, ValidationEventArgs e)
        {
            MessageBox.Show("IXmlComponentBase.XmlScenarioTable_E_XmlSchemaValidation:"+ e.Message);
        }


        public void SaveXml(String xmlFile)
        {

        }



        public void LoadXml(XmlDocument xDoc, XmlNode rootNode, Boolean refLoad=false)
        {
            if (rootNode == null) return;
            XmlControlHandler.GetDefaultControlAttributes(rootNode, xDoc,   this);


           
        }


        public XmlNode GetXml(XmlDocument xDoc, XmlNode parent = null)
        {
           
            return null;
        }
        XmlItemInterface _interface;
        public XmlItemInterface Interface { get { return _interface; } set { _interface = value; } }
    }
}
