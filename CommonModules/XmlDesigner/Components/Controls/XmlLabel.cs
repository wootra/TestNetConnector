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
    public class XmlLabel : Label, IXmlComponent, IXmlItem
    {
        
        String[] _textAlignModes = new String[] {"Right", "Left", "NumberOnlyRight", "Center", "NumberRightTextCenter", "None" };
        String[] _imageLayouts = new String[] { "None","Tile","Center","Stretch","Zoom"};
        Control _control;
        Type _type;
        XmlDocument _xDoc;
        XmlItemTypes _comType = XmlItemTypes.Label;
        XmlEvents _events = new XmlEvents();
        public XmlEvents Events { get { return _events; } }

        public event ValidationEventHandler E_XmlSchemaValidation;

        public XmlLabel()
            : base()
        {
            
        }

        public XmlItemTypes XmlItemType { get { return _comType; } }

        public Control RealControl { get { return this; } }

        public Type Type { get { return typeof(Label); } }
        
        String _filePath = "";
        public string FilePath
        {
            get { return _filePath; }
        }

        public void LoadXml(String xmlFile, Boolean refLoad=false)
        {
            if (XmlLayoutCollection.NowLoadingPath.Length > 0) _filePath = XmlLayoutCollection.NowLoadingPath +  XmlLayoutCollection.PathSeperator + xmlFile;
            else _filePath = xmlFile;

            XmlNode xNode = XmlGetter.RootNode(out _xDoc,_filePath, "./ComponentSchemas/LabelSchema.xsd", XmlScenarioTable_E_XmlSchemaValidation);
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
                MessageBox.Show("XmlLabel.LoadXml:" + e.Message + ":" + xmlFile);
            }
        }

 

        void XmlScenarioTable_E_XmlSchemaValidation(object sender, ValidationEventArgs e)
        {
            MessageBox.Show("XmlLabel.XmlScenarioTable_E_XmlSchemaValidation:" + e.Message);
        }


        public void SaveXml(String xmlFile)
        {

        }



        public void LoadXml(XmlDocument xDoc, XmlNode rootNode, Boolean refLoad=false)
        {
            if (rootNode == null) return;
            XmlControlHandler.GetDefaultControlAttributes(rootNode, xDoc,   this);


            if (rootNode.Attributes == null)
            {
                throw new Exception("Label에 최소한 Text 속성은 있어야 합니다.");
            }

            

            this.TextAlign = GlobalVars.ContentAlignment(XmlGetter.Attribute(rootNode, "TextAlign"));

           
        }


        public XmlNode GetXml(XmlDocument xDoc, XmlNode parent = null)
        {
            XmlNode root = XmlAdder.Element(xDoc, "Label", parent);
            XmlAdder.Attribute(xDoc, "Text", this.Text, parent);
            //XmlAdder.Attribute(xDoc, "Enabled", this.Enabled ? "true" : "false", parent);
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
