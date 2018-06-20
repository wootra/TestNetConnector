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
    public class XmlImageButton :FormAdders.ImgButton2 , IXmlComponent, IXmlItem
    {
        XmlDocument _xDoc;
        String[] _imageLayouts = new String[] { "None","Tile","Center","Stretch","Zoom"};
        Control _control;
        Type _type;
        XmlItemTypes _comType = XmlItemTypes.ImageButton;
        XmlImageList _imageList;

        XmlEvents _events = new XmlEvents();
        public XmlEvents Events { get { return _events; } }

        public event ValidationEventHandler E_XmlSchemaValidation;

        public XmlImageButton()
            : base()
        {
            
            _imageList = new XmlImageList();
            U_ImageList = _imageList;
        }

        public XmlItemTypes XmlItemType { get { return _comType; } }

        public Control RealControl { get { return this; } }

        public Type Type { get { return this.GetType(); } }

        public void LoadXml(String xmlFile, Boolean refLoad=false)
        {
            //DataSet dt = new DataSet();
            //dt.ReadXmlSchema(Properties.Resources.TableSchema);
            //dt.ReadXml(xmlFile); //schema를 불러와서 체크하기 위하여..
            if (XmlLayoutCollection.NowLoadingPath.Length > 0) _filePath = XmlLayoutCollection.NowLoadingPath +  XmlLayoutCollection.PathSeperator + xmlFile;
            else _filePath = xmlFile;

            XmlNode xNode = XmlGetter.RootNode(out _xDoc, _filePath, "./ComponentSchemas/ImageButtonSchema.xsd", XmlScenarioTable_E_XmlSchemaValidation);
            /*
            _xDoc = new XmlDocument();
            _xDoc.PreserveWhitespace = false;
            _xDoc.Schemas = new System.Xml.Schema.XmlSchemaSet();
            XmlSchema schema = XmlSchema.Read(File.OpenRead("./ComponentSchemas/ImageButtonSchema.xsd"), XmlScenarioTable_E_XmlSchemaValidation);
            _xDoc.Schemas.Add(schema);

            _xDoc.Load(xmlFile);

                        
            xNode = _xDoc.SelectSingleNode("//ImageButton");
             */
            try
            {
                LoadXml(_xDoc, xNode, refLoad);
            }
            catch (Exception e)
            {
                MessageBox.Show("XmlImageButton.LoadXml:" + e.Message + ":" + xmlFile);
            }
        }

 

        void XmlScenarioTable_E_XmlSchemaValidation(object sender, ValidationEventArgs e)
        {
            MessageBox.Show("XmlImageButton.XmlScenarioTable_E_XmlSchemaValidation:" + e.Message);
        }


        public void SaveXml(String xmlFile)
        {

        }



        public void LoadXml(XmlDocument xDoc, XmlNode rootNode, Boolean refLoad=false)
        {
            if (rootNode == null) return;
            XmlControlHandler.GetDefaultControlAttributes(rootNode, xDoc,   this);

            this.TextAlign = GlobalVars.ContentAlignment(XmlGetter.Attribute(rootNode, "TextAlign"));
            

            string disIndex = XmlGetter.Attribute(rootNode, "DisabledImageIndex");
            if(disIndex.Length>0) this.U_ButtonDisabledType = int.Parse(disIndex);

            string color = XmlGetter.Attribute(rootNode, "ActivationColor");
            if(color.Length>0) this.ActiveBackColor = ValueParser.StringToColor(color);

            


            string imageLayout = XmlGetter.Attribute(rootNode, "ImageLayout");
            if (imageLayout.Length > 0) this.BackgroundImageLayout = (ImageLayout)GlobalVars.ImageLayouts.ToList().IndexOf(imageLayout);
            else this.BackgroundImageLayout = ImageLayout.Zoom;
            

            //this.BackgroundImageLayout = ImageLayout.Stretch;

            XmlNode XImages = XmlGetter.Child(rootNode,"Images");// rootNode.SelectSingleNode("Images");

            _imageList.LoadXml(xDoc, XImages);
            /*

            for (int i = 0; i < rootNode.ChildNodes.Count; i++)
            {
                XmlNode xChild = rootNode.ChildNodes[i];
                if (xChild.Name.Equals("Events"))
                {
                    _events.LoadXml(xDoc, xChild);
                }
                else if (xChild.Name.Equals("Images"))
                {
                    _imageList.LoadXml(xDoc, xChild);
                    //_imageList = new XmlImageList();
                    this.U_ImageList = _imageList;
                    
                }
            }
             */
        }

        String _filePath = "";
        public string FilePath
        {
            get { return _filePath; }
        }

                    
    
    
        protected override void OnClick(EventArgs e)
        {
            XmlControlHandler.RunEvent(this, EventTypes.OnClick.ToString());
            base.OnClick(e);
        }

        public XmlNode GetXml(XmlDocument xDoc, XmlNode parent = null)
        {
            XmlNode root = XmlAdder.Element(xDoc, "ImageButton", parent);
            XmlAdder.Attribute(xDoc, "Enabled", this.Enabled ? "true" : "false", parent);
            XmlAdder.Attribute(xDoc, "Text", this.Text, parent);
            if (this.ForeColor.IsKnownColor) XmlAdder.Attribute(xDoc, "TextColor", this.ForeColor.Name, parent);
            else XmlAdder.Attribute(xDoc, "TextColor", String.Format("#{0:X6}", this.ForeColor.ToArgb()), parent);
            if (this.BackColor.IsKnownColor) XmlAdder.Attribute(xDoc, "BackColor", this.ForeColor.Name, parent);
            else XmlAdder.Attribute(xDoc, "BackColor", String.Format("#{0:X6}", this.ForeColor.ToArgb()), parent);
            if (this.BackColor.IsKnownColor) XmlAdder.Attribute(xDoc, "ActivationColor", this.ForeColor.Name, parent);
            else XmlAdder.Attribute(xDoc, "ActivationColor", String.Format("#{0:X6}", this.ForeColor.ToArgb()), parent);

            XmlAdder.Attribute(xDoc, "DisabeldImageIndex", this.U_ButtonDisabledType.ToString(), parent);
            
            _events.GetXml(xDoc, root);
            return root;
        }
        XmlItemInterface _interface;
        public XmlItemInterface Interface { get { return _interface; } set { _interface = value; } }
    }
}
