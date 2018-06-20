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
    public class XmlImageLed : ImageLed, IXmlComponent, IXmlItem
    {
        
        Control _control;
        Type _type;
        XmlDocument _xDoc;
        XmlItemTypes _comType = XmlItemTypes.Led;
        XmlEvents _events = new XmlEvents();
        public XmlEvents Events { get { return _events; } }

        public event ValidationEventHandler E_XmlSchemaValidation;

        public XmlImageLed()
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
                MessageBox.Show("XmlImageLed.LoadXml:" + e.Message + ":" + xmlFile);
            }
        }

 

        void XmlScenarioTable_E_XmlSchemaValidation(object sender, ValidationEventArgs e)
        {
            MessageBox.Show("XmlImageLed.XmlScenarioTable_E_XmlSchemaValidation:" + e.Message);
        }


        public void SaveXml(String xmlFile)
        {
            
        }



        public void LoadXml(XmlDocument xDoc, XmlNode rootNode, Boolean refLoad=false)
        {
            if (rootNode == null) return;
            XmlControlHandler.GetDefaultControlAttributes(rootNode, xDoc,   this);

            XmlNode xImages = XmlGetter.Child(rootNode, "ImageList");
            if (xImages != null)
            {
                this.LedImages.Clear();
                XmlNode xImage;
                String url;
                Image img;
                xImage = XmlGetter.Child(xImages, "OffImage");
                if (xImages == null) throw new Exception("Led/ImageList/OffImage 태그는 필수입니다.");
                url = XmlGetter.Attribute(xImages, "URL");
                img = Image.FromFile(url);
                this.LedImages.Add(img);

                xImage = XmlGetter.Child(xImages, "OnImage");
                if (xImages == null) throw new Exception("Led/ImageList/OnImage 태그는 필수입니다.");
                url = XmlGetter.Attribute(xImages, "URL");
                img = Image.FromFile(url);
                this.LedImages.Add(img);

                xImage = XmlGetter.Child(xImages, "MidStateImage");
                if (xImages == null) throw new Exception("Led/ImageList/MidStateImage 태그는 필수입니다.");
                url = XmlGetter.Attribute(xImages, "URL");
                img = Image.FromFile(url);
                this.LedImages.Add(img);
            }
            else
            {
                this.setImage(
                    FormAdders.Properties.Resources.led_on,
                    FormAdders.Properties.Resources.led_off,
                    FormAdders.Properties.Resources.led_mid);
            }

        }


        public XmlNode GetXml(XmlDocument xDoc, XmlNode parent = null)
        {
           
            return null;
        }
        XmlItemInterface _interface;
        public XmlItemInterface Interface { get { return _interface; } set { _interface = value; } }

    }
}
