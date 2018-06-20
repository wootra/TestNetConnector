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
    public class XmlCheckImageList : FormAdders.FineImageList, IXmlItem
    {
        XmlDocument _xDoc;
        List<String> _pathList = new List<string>();
//        List<Image> _imageList = new List<Image>();
        //String[] _imageLayouts = new String[] { "None","Tile","Center","Stretch","Zoom"};

        String _tagName = "ImageList";
        public event ValidationEventHandler E_XmlSchemaValidation;

        public XmlCheckImageList(String tagName = null)
            : base()
        {
            if (tagName != null) _tagName = tagName;
        }

      
         public void LoadXml(String xmlFile, Boolean refLoad=false)
        {
            //DataSet dt = new DataSet();
            //dt.ReadXmlSchema(Properties.Resources.TableSchema);
            //dt.ReadXml(xmlFile); //schema를 불러와서 체크하기 위하여..
            if (XmlLayoutCollection.NowLoadingPath.Length > 0) _filePath = XmlLayoutCollection.NowLoadingPath +  XmlLayoutCollection.PathSeperator + xmlFile;
            else _filePath = xmlFile;

            XmlNode xNode = XmlGetter.RootNode(out _xDoc, _filePath, "./ComponentSchemas/Rules/TableRuleSchema.xsd", XmlSchemaValidation);
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
                MessageBox.Show("XmlCheckImageList.LoadXml:" + e.Message + ":" + xmlFile);
            }
        }

 

        void XmlSchemaValidation(object sender, ValidationEventArgs e)
        {
            MessageBox.Show("XmlCheckImageList.XmlSchemaValidation:" + e.Message);
        }


        public void SaveXml(String xmlFile)
        {

        }

        public void AddImage(String imagePath)
        {
            if (XmlLayoutCollection.NowLoadingPath.Length > 0) imagePath = XmlLayoutCollection.NowLoadingPath +  XmlLayoutCollection.PathSeperator + imagePath;
            if (File.Exists(imagePath) == false) throw new Exception("XmlCheckImageList: " + imagePath + " 파일이 없습니다.");
            try
            {
                Image img = Image.FromFile(imagePath);
                this.Add(img);
                _pathList.Add(imagePath);
            }
            catch (Exception e)
            {
                throw new Exception("XmlCheckImageList : " + imagePath + " 를 Image 형식으로 읽을 수 없습니다.");
            }
        }

        public void LoadXml(XmlDocument xDoc, XmlNode rootNode, Boolean refLoad=false)
        {
            if (rootNode == null) return;
            XmlControlHandler.GetDefaultXmlItemAttributes(rootNode, xDoc, this);

            _tagName = rootNode.Name;
            if (rootNode.ChildNodes.Count > 0)
            {
                XmlNode xImg;
                string url="";

                xImg = XmlGetter.Child(rootNode, "NormalImage");
                //xImg = rootNode.SelectSingleNode("NormalImage");
                url = XmlGetter.Attribute(xImg, "URL");
                AddImage(url);
                xImg = xImg = XmlGetter.Child(rootNode,"CheckImage");
                url = XmlGetter.Attribute(xImg, "URL");
                AddImage(url);
                xImg = xImg = XmlGetter.Child(rootNode,"InterImage");
                url = XmlGetter.Attribute(xImg, "URL");
                AddImage(url);
                xImg = xImg = XmlGetter.Child(rootNode,"DisabledImage");
                url = XmlGetter.Attribute(xImg, "URL");
                AddImage(url);
                
            }
            /*
            for (int i = 0; i < rootNode.ChildNodes.Count; i++)
            {
                XmlNode xImg = rootNode.ChildNodes[i];
                string url = XmlGetter.getAttr(xImg, "URL");
                
                AddImage(url);
            }
             */
        }

        String _filePath = "";
        public string FilePath
        {
            get { return _filePath; }
        }

        public XmlItemTypes XmlItemType
        {
            get { return XmlItemTypes.ImageList; }
        }

        public Type Type
        {
            get { return typeof(XmlImageList); }
        }

        public XmlNode GetXml(XmlDocument xDoc, XmlNode parent = null)
        {
            XmlNode xImgList = XmlAdder.Element(xDoc, _tagName, parent);

            XmlNode xImg = XmlAdder.Element(xDoc, "Image", xImgList);
            XmlAdder.Attribute(xDoc, "URL", _pathList[(int)ImageKind.NormalImage], xImg);
            xImg = XmlAdder.Element(xDoc, "Image", xImgList);
            XmlAdder.Attribute(xDoc, "URL", _pathList[(int)ImageKind.CheckImage], xImg);
            xImg = XmlAdder.Element(xDoc, "Image", xImgList);
            XmlAdder.Attribute(xDoc, "URL", _pathList[(int)ImageKind.InterImage], xImg);
            xImg = XmlAdder.Element(xDoc, "Image", xImgList);
            XmlAdder.Attribute(xDoc, "URL", _pathList[(int)ImageKind.DisabledImage], xImg);
            /*
            for (int i = 0; i < _pathList.Count; i++)
            {
                XmlNode xImg = XmlAdder.Element(xDoc, "Image", xImgList);
                XmlAdder.Attribute(xDoc, "URL", _pathList[i], xImg);
            }
             */
            return xImgList;
        }

        public enum ImageKind { NormalImage = 0, CheckImage, InterImage, DisabledImage };

        XmlItemInterface _interface;
        public XmlItemInterface Interface { get { return _interface; } set { _interface = value; } }
    }
}
