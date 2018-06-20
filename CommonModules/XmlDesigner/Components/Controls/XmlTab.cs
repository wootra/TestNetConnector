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
    public partial class XmlTab : ImageTabPage, IXmlComponent, IXmlItem, IXmlLayout
    {
        Panel _tabPanel;
        public Dictionary<String, IXmlComponent> _idList = new Dictionary<string, IXmlComponent>();
        public String _namespace = "";
        public Panel _layoutPanel;
        List<XmlComponent> _components = new List<XmlComponent>();
        XmlEvents _events = new XmlEvents();
        public XmlEvents Events { get { return _events; } }

        // 요약:
        //     컨트롤이 도킹되는 위치와 방법을 지정합니다.
        //String[] _dockStyles = new String[]{"None", "Top","Bottom","Left","Right","Fill"};

        public XmlTab(Dictionary<String, IXmlComponent> idList, String Namespace=null)
            : base()
        {
            _idList = idList;
            if (Namespace != null && Namespace.Length > 0) _namespace = Namespace;
        }

        public List<XmlComponent> Components { get { return _components; } }

        public XmlItemTypes XmlItemType
        {
            get { return XmlItemTypes.Tab; }
        }

        String _filePath = "";
        public string FilePath
        {
            get { return _filePath; }
        }

        XmlDocument _xDoc;  public void LoadXml(String xmlFile, bool refLoad = false)
        {

            if (XmlLayoutCollection.NowLoadingPath.Length > 0) _filePath = XmlLayoutCollection.NowLoadingPath +  XmlLayoutCollection.PathSeperator + xmlFile;
            else _filePath = xmlFile;
            XmlNode rootNode = XmlGetter.RootNode(out _xDoc,_filePath, "./ComponentSchemas/TabSchema.xsd", XmlSchemaValidation);
            try
            {
                LoadXml(_xDoc, rootNode, refLoad);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message + ":" + _filePath);
            }
        }

        public void SaveXml(string xmlFile)
        {
            throw new NotImplementedException();
        }


        public Control RealControl
        {
            get { return _tabPanel; }
        }

        public Type Type
        {
            get { return _tabPanel.GetType(); }
        }

        public void LoadXml(XmlDocument xDoc, XmlNode rootNode, bool refLoad=false)
        {
            if (rootNode == null) return;
            XmlControlHandler.GetDefaultControlAttributes(rootNode, xDoc,   this, refLoad);

            if(rootNode.ChildNodes.Count==0){
                throw new Exception("올바른 Tab 태그가 아닙니다. <Panel> 이나 <Flow> 중 하나를 자식으로 가져야 합니다.");
            }

            string ns = XmlGetter.Attribute(rootNode, "NameSpace");
            if (ns.Length > 0)
            {
                if (_namespace != null && _namespace.Length > 0)
                {
                    _namespace +=  GlobalVars.Seperator + ns;
                }
                else
                {
                    _namespace = ns;
                }
            }
            this.Dock = DockStyle.Fill;
            
            _layoutPanel = XmlControlHandler.LoadLayoutChildren(rootNode.FirstChild, this, _idList, _namespace, this);
        }
 
      

        void XmlSchemaValidation(object sender, ValidationEventArgs e)
        {
            MessageBox.Show("XmlTab.XmlSchemaValidation:" + e.Message);
        }


        public XmlNode GetXml(XmlDocument xDoc, XmlNode parent = null)
        {
            XmlNode xLayout = XmlAdder.Element(xDoc, "Tab", parent);
            
            if (_layoutPanel is FlowLayoutPanel)
            {
                XmlNode flow = XmlAdder.Element(xDoc, "Flow", xLayout);

                for (int i = 0; i < _layoutPanel.Controls.Count; i++)
                {
                    XmlNode com = XmlAdder.Element(xDoc, "Component", flow);
                    Control control = _layoutPanel.Controls[i];
                    XmlAdder.Attribute(xDoc, "Width", control.Width.ToString(), com);
                    XmlAdder.Attribute(xDoc, "Height", control.Height.ToString(), com);
                    IXmlComponent xRealCom = control as IXmlComponent;
                    if (xRealCom != null) xRealCom.GetXml(xDoc, com);
                }
            }
            else //Panel
            {
                XmlNode panel = XmlAdder.Element(xDoc, "Panel", xLayout);
                for (int i = 0; i < _layoutPanel.Controls.Count; i++)
                {
                    XmlNode com = XmlAdder.Element(xDoc, "Component", panel);
                    Control control = _layoutPanel.Controls[i];
                    XmlAdder.Attribute(xDoc, "Width", control.Width.ToString(), com);
                    XmlAdder.Attribute(xDoc, "Height", control.Height.ToString(), com);
                    XmlAdder.Attribute(xDoc, "X", control.Location.X.ToString(), com);
                    XmlAdder.Attribute(xDoc, "Y", control.Location.Y.ToString(), com);

                    IXmlComponent xRealCom = control as IXmlComponent;
                    if (xRealCom != null) xRealCom.GetXml(xDoc, com);
                }
            }

            return xLayout;
        }
        XmlItemInterface _interface;
        public XmlItemInterface Interface { get { return _interface; } set { _interface = value; } }
    }
}
