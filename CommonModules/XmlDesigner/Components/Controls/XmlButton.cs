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
    public class XmlButton :Button , IXmlComponent, IXmlItem
    {
        XmlDocument _xDoc;
        
        Control _control;
        Type _type;
        XmlItemTypes _comType = XmlItemTypes.Button;
        XmlEvents _events = new XmlEvents();
        public XmlEvents Events { get { return _events; } }

        public event ValidationEventHandler E_XmlSchemaValidation;

        public XmlButton()
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

          public void LoadXml(String xmlFile, bool refLoad = false)
        {
            if (XmlLayoutCollection.NowLoadingPath.Length > 0) _filePath = XmlLayoutCollection.NowLoadingPath +  XmlLayoutCollection.PathSeperator + xmlFile;
            else _filePath = xmlFile;

            //DataSet dt = new DataSet();
            //dt.ReadXmlSchema(Properties.Resources.TableSchema);
            //dt.ReadXml(xmlFile); //schema를 불러와서 체크하기 위하여..

            XmlNode xNode = XmlGetter.RootNode(out _xDoc, _filePath, "./ComponentSchemas/ButtonSchema.xsd", XmlScenarioTable_E_XmlSchemaValidation);
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
                MessageBox.Show("XmlButton.LoadXml1:" + e.Message + ":" + xmlFile);
            }
        }

 

        void XmlScenarioTable_E_XmlSchemaValidation(object sender, ValidationEventArgs e)
        {
            MessageBox.Show("XmlButton.XmlScenarioTable_E_XmlSchemaValidation:" + e.Message);
        }


        public void SaveXml(String xmlFile)
        {

        }



        public void LoadXml(XmlDocument xDoc, XmlNode rootNode, bool refLoad = false)
        {
            if (rootNode == null) return;
            XmlControlHandler.GetDefaultControlAttributes(rootNode, xDoc,   this);

            this.TextAlign = GlobalVars.ContentAlignment(XmlGetter.Attribute(rootNode, "TextAlign"));

            
            
        }
        protected override void OnClick(EventArgs e)
        {
            XmlControlHandler.RunEvent(this, EventTypes.OnClick.ToString());
            base.OnClick(e);
        }


        public XmlNode GetXml(XmlDocument xDoc, XmlNode parent = null)
        {
            XmlNode root = XmlAdder.Element(xDoc, "Button", parent);
            XmlAdder.Attribute(xDoc, "Text", this.Text, parent);
            XmlAdder.Attribute(xDoc, "Enabled", this.Enabled ? "true" : "false", parent);
            XmlAdder.Attribute(xDoc, "TextAlign", this.TextAlign.ToString(), parent);
            if (this.ForeColor.IsKnownColor) XmlAdder.Attribute(xDoc, "TextColor", this.ForeColor.Name, parent);
            else XmlAdder.Attribute(xDoc, "TextColor", String.Format("#{0:X6}", this.ForeColor.ToArgb()), parent);

            if (this.BackColor.IsKnownColor) XmlAdder.Attribute(xDoc, "BackColor", this.ForeColor.Name, parent);
            else XmlAdder.Attribute(xDoc, "BackColor", String.Format("#{0:X6}", this.ForeColor.ToArgb()), parent);

            return root;
        }
        XmlItemInterface _interface;
        public XmlItemInterface Interface { get { return _interface; } set { _interface = value; } }
    }
}
