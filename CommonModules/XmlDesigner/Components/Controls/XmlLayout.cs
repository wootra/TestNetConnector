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
using XmlDesigner.ForEvents;

namespace XmlDesigner
{
    public partial class XmlLayout : Panel, IXmlComponent, IXmlItem, IXmlLayout
    {
        Panel _layoutPanel;
        Dictionary<String, IXmlComponent> _idList;
        public String _namespace = "";
        String _backgroundImage_Path = null;
        List<XmlComponent> _components = new List<XmlComponent>();
        XmlEvents _events = new XmlEvents();
        public XmlEvents Events { get { return _events; } }

        // 요약:
        //     컨트롤이 도킹되는 위치와 방법을 지정합니다.
        
        public XmlLayout(Dictionary<String, IXmlComponent> idList, String Namespace)
            : base()
        {
            _idList = idList;
            if (Namespace != null && Namespace.Length > 0) _namespace = Namespace;
        }

        public List<XmlComponent> Components { get { return _components; } }

        public XmlItemTypes XmlItemType
        {
            get { return XmlItemTypes.Layout; }
        }

        String _filePath = "";
        public string FilePath
        {
            get { return _filePath; }
        }

        XmlDocument _xDoc;  
        public void LoadXml(String xmlFile, Boolean refLoad=false)
        {

            if (XmlLayoutCollection.NowLoadingPath.Length > 0) _filePath = XmlLayoutCollection.NowLoadingPath +  XmlLayoutCollection.PathSeperator + xmlFile;
            else _filePath = xmlFile;

            XmlNode rootNode = XmlGetter.RootNode(out _xDoc,_filePath, "./ComponentSchemas/LayoutSchema.xsd", XmlSchemaValidation);
            try
            {
                LoadXml(_xDoc, rootNode);
            }
            catch (Exception e)
            {
                throw new Exception("XmlLayout.LoadXml:"+e.Message + ":" + _filePath);
            }
        }

        public void SaveXml(string xmlFile)
        {
            throw new NotImplementedException();
        }


        public Control RealControl
        {
            get { return _layoutPanel; }
        }

        public Type Type { get { return this.GetType(); } }

        public void LoadXml(XmlDocument xDoc, XmlNode rootNode, Boolean refLoad=false)
        {
            if (rootNode == null) return;
            XmlControlHandler.GetDefaultControlAttributes(rootNode, xDoc,   this);

            if(rootNode.ChildNodes.Count==0){
                throw new Exception("올바른 Layout 태그가 아닙니다. <Panel> 이나 <Flow> 중 하나를 자식으로 가져야 합니다.");
            }
            if (_namespace.Length == 0)
            {
                _namespace = XmlGetter.Attribute(rootNode, "NameSpace");
            }
            else
            {
                String Namespace = XmlGetter.Attribute(rootNode, "NameSpace");
                if (Namespace.Length > 0) _namespace = _namespace +  GlobalVars.Seperator + Namespace;
                else { } //do nothing. parent's namesapce will be used for control's namespace.
            }
            
           
            _layoutPanel = XmlControlHandler.LoadLayoutChildren(rootNode.ChildNodes[0], this, _idList, _namespace, this);
            
            try
            {
                string backPath = XmlGetter.Attribute(rootNode, "Background-Image");
                if (XmlLayoutCollection.NowLoadingPath.Length > 0) backPath = XmlLayoutCollection.NowLoadingPath +  XmlLayoutCollection.PathSeperator + backPath;
                if (backPath.Length > 0) _layoutPanel.BackgroundImage = Image.FromFile(backPath);
                string imageLayout = XmlGetter.Attribute(rootNode, "Background-ImageLayout");
                if(imageLayout.Length>0) _layoutPanel.BackgroundImageLayout = (ImageLayout)GlobalVars.ImageLayouts.ToList().IndexOf(imageLayout);
                _backgroundImage_Path = backPath; //이미지 지정에 성공하면 배정함.
            }
            catch { }
            String backColor = XmlGetter.Attribute(rootNode, "Background-Color");
            if (backColor.Length > 0) _layoutPanel.BackColor = XmlHandlers.ValueParser.StringToColor(backColor);
            
        }



        void XmlSchemaValidation(object sender, ValidationEventArgs e)
        {
            MessageBox.Show("XmlLayout.XmlSchemaValidation:" + e.Message);
        }


        public XmlNode GetXml(XmlDocument xDoc, XmlNode parent = null)
        {
            XmlNode xLayout = XmlAdder.Element(xDoc, "Layout", parent);
            XmlAdder.Attribute(xDoc, "Dock", this.Dock.ToString(), xLayout);
            switch (this.Dock)
            {
                case DockStyle.Fill:
                    break;
                case DockStyle.Left:
                case DockStyle.Right:
                    XmlAdder.Attribute(xDoc, "Width", this.Width.ToString(), xLayout);
                    break;
                case DockStyle.Top:
                case DockStyle.Bottom:
                    XmlAdder.Attribute(xDoc, "Height", this.Height.ToString(), xLayout);
                    break;
                case DockStyle.None:
                    XmlAdder.Attribute(xDoc, "Width", this.Width.ToString(), xLayout);
                    XmlAdder.Attribute(xDoc, "Height", this.Height.ToString(), xLayout);
                    XmlAdder.Attribute(xDoc, "X", this.Location.X.ToString(), xLayout);
                    XmlAdder.Attribute(xDoc, "Y", this.Location.Y.ToString(), xLayout);
                    break;
            }
            if(this.BackgroundImage!=null) XmlAdder.Attribute(xDoc, "Background-Image", _backgroundImage_Path, xLayout);
            if(this.BackColor.Equals(Color.Transparent)==false) XmlAdder.Attribute(xDoc, "Background-Color", this.BackColor.ToString(), xLayout);
            
            if (_layoutPanel is FlowLayoutPanel)
            {
                XmlNode flow = XmlAdder.Element(xDoc, "Flow", xLayout);
                
                for (int i = 0; i < _layoutPanel.Controls.Count; i++)
                {
                    //XmlComponent xCom = new XmlComponent(_layoutPanel, _idList, _namespace);
                    XmlNode com = XmlAdder.Element(xDoc, "Component", flow);
                    Control control = _layoutPanel.Controls[i];
                    XmlAdder.Attribute(xDoc, "Width", control.Width.ToString(), com);
                    XmlAdder.Attribute(xDoc, "Height", control.Height.ToString(), com);
                    XmlAdder.Attribute(xDoc, "Name", control.Name, com);
                    XmlAdder.Attribute(xDoc, "Name", control.Name, com);
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
