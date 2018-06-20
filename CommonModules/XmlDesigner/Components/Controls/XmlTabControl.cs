using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Schema;
using XmlHandlers;
using FormAdders;
using XmlDesigner.ForEvents;

namespace XmlDesigner
{
    public partial class XmlTabControl : ImageTabControl, IXmlComponent, IXmlItem
    {
        
        // 요약:
        //     컨트롤이 도킹되는 위치와 방법을 지정합니다.
        //Control _parent;
        public Dictionary<String, IXmlComponent> _idList;
        String _namespace = "";
        XmlEvents _events = new XmlEvents();
        public XmlEvents Events { get { return _events; } }

        public XmlTabControl( Dictionary<String, IXmlComponent> controls, String Namespace)
            : base()
        {
            //_parent = parent;
            _idList = controls;
            if (Namespace != null && Namespace.Length > 0) _namespace = Namespace;
        }

        String _filePath = "";

        public string FilePath
        {
            get { return _filePath; }
        }

        XmlDocument _xDoc;  public void LoadXml(String xmlFile, Boolean refLoad=false)
        {
            if (XmlLayoutCollection.NowLoadingPath.Length > 0) _filePath = XmlLayoutCollection.NowLoadingPath +  XmlLayoutCollection.PathSeperator + xmlFile;
            else _filePath = xmlFile;

            XmlNode rootNode = XmlGetter.RootNode(out _xDoc, _filePath, "./ComponentSchemas/TabControlSchema.xsd", XmlSchemaValidation);
            //try
            {
                LoadXml(_xDoc, rootNode);
            }
            //catch (Exception e)
            {
            //    throw new Exception(e.Message + ":" + xmlFile);
            }
        }

        public void SaveXml(string xmlFile)
        {
            throw new NotImplementedException();
        }


        public void LoadXml(XmlDocument xDoc, XmlNode rootNode, Boolean refLoad=false)
        {
            if (rootNode == null) return;
            XmlControlHandler.GetDefaultControlAttributes(rootNode, xDoc,   this);

            String ns = XmlGetter.Attribute(rootNode, "NameSpace");
            if(ns.Length>0){
                if (_namespace != null && _namespace.Length > 0)
                {
                    _namespace +=  GlobalVars.Seperator + ns;
                }
                else
                {
                    _namespace = ns;
                }
            }

            XmlNode xInfo = XmlGetter.Child(rootNode, "TabControlInfo");// rootNode.SelectSingleNode("TabControlInfo");

            XmlNode xTabImages = XmlGetter.Child(rootNode, "TabImages"); //xInfo.SelectSingleNode("TabImages");
            if (xTabImages != null)
            {
                XmlImageList xImgList = new XmlImageList("TabImages");

                xImgList.LoadXml(xDoc, xTabImages);
                this.ImageList = xImgList;
            }

            XmlNodeList xTabs = XmlGetter.Children(rootNode,"Tabs/Tab");
            for (int i = 0; i < xTabs.Count; i++)
            {
                XmlNode xTab = xTabs[i];
                //if (xTab.NodeType != XmlNodeType.Element) continue; //주석을 거른다.
                if (xTab.Name.Equals("Tab") == false) continue; //오직 자식으로는 Component만을 가진다.
                
                XmlTab tab = new XmlTab(_idList, _namespace);
                tab.LoadXml(xDoc, xTab);
                
                this.TabPages.Add(tab);
                //_parent.Controls.Add(tab);
                

                Dictionary<String, IXmlComponent> idList = tab._idList;
                /*
                for (int ids = 0; ids < idList.Count; ids++)
                {
                    string name = idList.Keys.ElementAt(ids);
                    if (_namespace.Length > 0) name = _namespace + "." + name; //namespace가 있으면 붙여서 namepace.name 형식으로 다시 naming..
                    Controls.Add(name, idList.Values.ElementAt(ids));
                }
                 */
            }
        }


        void XmlSchemaValidation(object sender, ValidationEventArgs e)
        {
            MessageBox.Show("XmlTabControl.XmlSchemaValidation:" + e.Message);
        }

        public XmlItemTypes XmlItemType
        {
            get { return XmlItemTypes.TabControl; }
        }

        public Control RealControl
        {
            get { return this; }
        }

        public Type Type { get { return this.GetType(); } }


        public XmlNode GetXml(XmlDocument xDoc, XmlNode parent = null)
        {
            XmlNode root = XmlAdder.Element(xDoc, "TabControl", parent);

            XmlNode xInfo = XmlAdder.Element(xDoc, "TabControlInfo", root);
            XmlImageList xImgList = this.ImageList as XmlImageList;
            if (xImgList != null)
            {
                xImgList.GetXml(xDoc, xInfo);
            }

            XmlNode xTabs = XmlAdder.Element(xDoc, "Tabs", root);
            for (int i = 0; i < this.TabPages.Count; i++)
            {
                XmlTab tab = this.TabPages[i] as XmlTab;
                if (tab != null)
                {
                    tab.GetXml(xDoc, xTabs);
                }
            }

            return root;
        }
        XmlItemInterface _interface;
        public XmlItemInterface Interface { get { return _interface; } set { _interface = value; } }
    }
}
