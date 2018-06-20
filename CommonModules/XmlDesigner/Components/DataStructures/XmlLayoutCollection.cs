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
    public partial class XmlLayoutCollection: IXmlItem
    {
        
        // 요약:
        //     컨트롤이 도킹되는 위치와 방법을 지정합니다.
        Control _parent;
        Dictionary<String, IXmlComponent> _localComponents = new Dictionary<string,IXmlComponent>();
        public Dictionary<String, IXmlComponent> _globalComponents;
        String _namespace = "";
        public static String NowLoadingPath = "";
        public static String PathSeperator = "/";


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent">이 LayoutCollection이 Fill될 대상 control</param>
        /// <param name="controls">Xml파일에서 불러들여져서 이 LayoutCollection에 컨트롤들이 관리될 Dictionary</param>
        /// <param name="Namespace">null이면 parent의 Name이 namespace가 됨. 이 이름이 앞에 붙어서 namespace/name 식으로 구분됨..</param>
        public XmlLayoutCollection(Control parent, Dictionary<String, IXmlComponent> controls, String Namespace=null)
            : base()
        {
            _parent = parent;
            _globalComponents = controls;
            if (Namespace == null) _namespace = parent.Name;
            else _namespace = Namespace;
        }

        String _filePath = "";

        public string FilePath
        {
            get { return _filePath; }
        }

        XmlDocument _xDoc;  
        public void LoadXml(String xmlFile, Boolean refLoad=false)
        {
            _filePath = xmlFile;
            if (_filePath.LastIndexOf("/") >= 0)
            {
                _filePath = _filePath.Replace("\\", "/");
                XmlLayoutCollection.NowLoadingPath = _filePath.Substring(0, _filePath.LastIndexOf("/"));
                XmlLayoutCollection.PathSeperator = "/";
            }
            else if (_filePath.LastIndexOf("\\") >= 0)
            {
                _filePath = _filePath.Replace("/", "\\");
                XmlLayoutCollection.NowLoadingPath = _filePath.Substring(0, _filePath.LastIndexOf("\\"));
                XmlLayoutCollection.PathSeperator = "\\";
            }
            

            XmlNode rootNode = XmlGetter.RootNode(out _xDoc, _filePath);//"./ComponentSchemas/LayoutCollectionSchema.xsd", XmlSchemaValidation);
            //try
            {
                LoadXml(_xDoc, rootNode);
            }
            //catch (Exception e)
            {
            //    throw new Exception(e.Message + ":" + xmlFile);
            }
        }

        public void Clear()
        {

            foreach (String key in _localComponents.Keys)
            {
                try
                {
                    _parent.Controls.RemoveByKey(key);
                }
                catch { }
                try
                {
                    _globalComponents.Remove(key);
                }
                catch { }
            }
            _localComponents.Clear();
        }
        
        public void SaveXml(string xmlFile)
        {
            throw new NotImplementedException();
        }


        public void LoadXml(XmlDocument xDoc, XmlNode rootNode, Boolean refLoad=false)
        {
            if (rootNode == null) return;
            XmlControlHandler.GetDefaultXmlItemAttributes(rootNode, xDoc, this);

            if(rootNode.ChildNodes.Count==0){
                throw new Exception("올바른 LayoutCollection 태그가 아닙니다. <Layout>이 적어도 하나는 있어야 합니다.");
            }
            
            String xmlNamespace = XmlGetter.Attribute(rootNode, "NameSpace");
            if (xmlNamespace.Length > 0) _namespace = xmlNamespace;//namepace를 덮어씌운다.
            
            for (int i = 0; i < rootNode.ChildNodes.Count; i++)
            {
                XmlNode xLayout = rootNode.ChildNodes[i];
                if (xLayout.NodeType != XmlNodeType.Element) continue; //주석을 거른다.
                if (xLayout.Name.Equals("Layout") == false) continue; //오직 자식으로는 Component만을 가진다.
                
                XmlLayout layout = new XmlLayout(_localComponents, _namespace);
                layout.LoadXml(xDoc, xLayout);
                _parent.Controls.Add(layout);
            }

            foreach (string key in _localComponents.Keys)
            {
                if (_globalComponents.ContainsKey(key))
                {
                    throw new Exception("이미 [" + key + "] 이름으로 추가된 컴포넌트가 존재합니다. ");
                }
                else
                {
                    _globalComponents.Add(key, _localComponents[key]);
                }
                
                /*
                if (_namespace.Length == 0)
                {
                    if (_globalComponents.ContainsKey(key))
                    {
                        throw new Exception("이미 [" + key + "] 이름으로 추가된 컴포넌트가 존재합니다. ");
                    }
                    else
                    {
                        _globalComponents.Add(key, _localComponents[key]);
                    }
                }
                else
                {
                    if (_globalComponents.ContainsKey(_namespace +  GlobalVars.Seperator + key))
                    {
                        throw new Exception("같은 namespace [" + _namespace + "] 가 존재합니다. Namespace를 같이 쓰면 안됩니다.");
                    }
                    else
                    {
                        _globalComponents.Add(_namespace +  GlobalVars.Seperator + key, _localComponents[key]);
                    }
                }
                 */
            }
        }


        void XmlSchemaValidation(object sender, ValidationEventArgs e)
        {
            throw new Exception("XmlLayoutCollection : "+e.Message);
        }

        public XmlItemTypes XmlItemType
        {
            get { return XmlItemTypes.LayoutCollection; }
        }

        public Type Type
        {
            get { return typeof(XmlLayoutCollection); }
        }

        public XmlNode GetXml(XmlDocument xDoc, XmlNode parent = null)
        {
            XmlNode root = XmlAdder.Element(xDoc, "LayoutCollection", parent);
            for (int i = 0; i < _parent.Controls.Count; i++)
            {
                IXmlItem item = _parent.Controls[i] as IXmlItem;
                item.GetXml(xDoc, root);
            }
            return root;
        }
        XmlItemInterface _interface;
        public XmlItemInterface Interface { get { return _interface; } set { _interface = value; } }
    }
}
