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
    public class XmlComponent :  IXmlItem
    {
        XmlDocument _xDoc;
        Type _type;
        Panel _panel;
        Control _realComponent;
        PanelTypes _panelType;
        Dictionary<String, IXmlComponent> _idList = new Dictionary<string, IXmlComponent>();
        public String _namespace = "";
        public static IXmlComponent NowLoading;
        public event ValidationEventHandler E_XmlSchemaValidation;


        public XmlComponent(Panel panel,Dictionary<String, IXmlComponent> idList, String Namespace)
        {
            _panel = panel;
            if (_panel is FlowLayoutPanel) _panelType = PanelTypes.Flow;
            else _panelType = PanelTypes.Panel;
            _namespace = Namespace;
            _idList = idList;
        }


        public Control RealControl { get { return _realComponent; } }

        
        String _filePath = "";
        public string FilePath
        {
            get { return _filePath; }
        }

         public void LoadXml(String xmlFile, bool refLoad=false)
        {
            if (XmlLayoutCollection.NowLoadingPath.Length > 0) _filePath = XmlLayoutCollection.NowLoadingPath +  XmlLayoutCollection.PathSeperator + xmlFile;
            else _filePath = xmlFile;

            XmlNode xNode = XmlGetter.RootNode(out _xDoc, _filePath, "./ComponentSchemas/LabelSchema.xsd", XmlSchemaValidation);
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
                MessageBox.Show("XmlComponent.LoadXml:" + e.Message + ":" + xmlFile);
            }
        }

 

        void XmlSchemaValidation(object sender, ValidationEventArgs e)
        {
            MessageBox.Show("XmlComponent.XmlSchemaValidation:" + e.Message);
        }


        public void SaveXml(String xmlFile)
        {

        }



        public void LoadXml(XmlDocument xDoc, XmlNode rootNode, bool refLoad=false)
        {
            if (rootNode == null) return;
            

            string refPath = XmlGetter.Attribute(rootNode, "Ref");
            bool refExist = refPath.Length > 0;

            XmlNode comNode;
            if (refExist)
            {
                if (XmlLayoutCollection.NowLoadingPath.Length > 0) refPath = XmlLayoutCollection.NowLoadingPath +  XmlLayoutCollection.PathSeperator + refPath;
                
                comNode = XmlGetter.RootNode(out _xDoc,refPath, null, XmlSchemaValidation);
                _filePath = refPath;
            }
            else
            {
                if (rootNode.ChildNodes.Count == 0)
                {
                    throw new Exception("Component 태그의 정의가 완전하지 않습니다. Ref로 xml파일을 불러오거나 직접 내부에 정의해야 합니다.\r\n name:" + XmlGetter.Attribute(rootNode, "Name"));
                }

                comNode = rootNode.ChildNodes[0];

            }
            string nameText;

            nameText = XmlGetter.Attribute(rootNode, "Name");

            XmlControlHandler.LoadInterface(this, rootNode, xDoc);

            
            Control control = XmlControlHandler.AddControl(nameText, xDoc, comNode, _panel, _idList, _namespace); ;
            _realComponent = control;
            NowLoading = control as IXmlComponent;
            //if (txt.Length > 0) control.Name = txt;
            if (XmlGetter.Attribute(rootNode, "Margin").Length > 0) control.Margin = ValueParser.Padding(XmlGetter.Attribute(rootNode, "Margin"));
            if (XmlGetter.Attribute(rootNode, "Padding").Length > 0) control.Padding = ValueParser.Padding(XmlGetter.Attribute(rootNode, "Padding"));
            if (XmlGetter.Attribute(rootNode, "Enabled").Length > 0) control.Enabled = (XmlGetter.Attribute(rootNode, "Enabled").Equals("false") == false);



            String hgt = XmlGetter.Attribute(rootNode, "Height");
            if (hgt.Length != 0) control.Height = int.Parse(hgt);
            String wid = XmlGetter.Attribute(rootNode, "Width");
            if (wid.Length != 0) control.Width = int.Parse(wid);

            if (_panel is FlowLayoutPanel)
            {
            }
            else if (_panel is Panel)
            {
                String x = XmlGetter.Attribute(rootNode, "X");
                Point location = new Point();
                if (x.Length != 0) location.X = int.Parse(x);
                String y = XmlGetter.Attribute(rootNode, "Y");
                if (y.Length != 0) location.Y = int.Parse(y);
                control.Location = location;
            }

            
        }


        public XmlNode GetXml(XmlDocument xDoc, XmlNode parent = null)
        {
            XmlNode root = XmlAdder.Element(xDoc, "Component", parent);
            XmlAdder.Attribute(xDoc, "Width", RealControl.Width.ToString(), root);
            XmlAdder.Attribute(xDoc, "Height", RealControl.Height.ToString(), root);
            XmlAdder.Attribute(xDoc, "Name", RealControl.Name, root);

            if (_panel is FlowLayoutPanel)
            {
            }
            else if (_panel is Panel)
            {
                XmlAdder.Attribute(xDoc, "X", RealControl.Location.X.ToString(), root);
                XmlAdder.Attribute(xDoc, "Y", RealControl.Location.Y.ToString(), root);
            }

            if (_filePath.Length > 0)
            {
                XmlAdder.Attribute(xDoc, "Ref", _filePath, root);
            }
            else
            {
                (_realComponent as IXmlComponent).GetXml(xDoc, root);
            }
            return root;
        }
        XmlItemInterface _interface;
        public XmlItemInterface Interface { get { return _interface; } set { _interface = value; } }
    }
}
