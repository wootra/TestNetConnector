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
using XmlDesigner.ForEvents.Conditions;

namespace XmlDesigner
{
    public class XmlActionList :List<XmlAction>, IXmlItem
    {
        XmlDocument _xDoc;
        //String[] _imageLayouts = new String[] { "None","Tile","Center","Stretch","Zoom"};
        public XmlCondition Condition;
        public IXmlComponentCondition ComCondition;

        String[] _conditions = new string[] { "And", "Or", "Equal", "More", "MoreOrEqual", "True", "False" };
        
        public event ValidationEventHandler E_XmlSchemaValidation;

        public XmlActionList()
            : base()
        {
            Condition = XmlCondition.New(XmlConditionTypes.True);//default는 true..
        }

      
          public void LoadXml(String xmlFile, Boolean refLoad=false)
        {
            //DataSet dt = new DataSet();
            //dt.ReadXmlSchema(Properties.Resources.TableSchema);
            //dt.ReadXml(xmlFile); //schema를 불러와서 체크하기 위하여..
            if (XmlLayoutCollection.NowLoadingPath.Length > 0) _filePath = XmlLayoutCollection.NowLoadingPath +  XmlLayoutCollection.PathSeperator + xmlFile;
            else _filePath = xmlFile;

            XmlNode xNode = XmlGetter.RootNode(out _xDoc, _filePath, "./ComponentSchemas/Rules/GlobalTypeRuleSchema.xsd", XmlSchemaValidation);
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
                MessageBox.Show("XmlActions.LoadXml:" + e.Message + ":" + xmlFile);
            }
            
        }

 

        void XmlSchemaValidation(object sender, ValidationEventArgs e)
        {
            MessageBox.Show("XmlActions.XmlSchemaValidation:" + e.Message);
        }


        public void SaveXml(String xmlFile)
        {

        }



        public void LoadXml(XmlDocument xDoc, XmlNode rootNode, Boolean refLoad=false)
        {
            if (rootNode == null) return;
            XmlControlHandler.GetDefaultXmlItemAttributes(rootNode, xDoc, this);

            this.Clear();
            if (rootNode == null)
            {
                throw new Exception("올바른 Actions Tag가 아닙니다. 루트가 Actions 가 아닙니다. ");
            }
            for (int i = 0; i < rootNode.ChildNodes.Count; i++)
            {
                XmlNode xChild = rootNode.ChildNodes[i];
                if (xChild.Name.Equals("Condition"))
                {
                    XmlNode condNode = xChild.FirstChild;
                    Condition = XmlCondition.New(condNode.Name);
                    Condition.LoadXml(xDoc, xChild);
                }
                else if (xChild.Name.Equals("ComponentCondition"))
                {
                    ComCondition = XmlComConditions.New();//현재 loading중인 conmponent의 고유 ComponentCondition을 가져온다.
                    ComCondition.LoadXml(xDoc, xChild);
                }
                else
                {
                    XmlNode xAction = xChild;
                    XmlAction action = new XmlAction();
                    action.LoadXml(xDoc, xAction);
                    this.Add(action);
                }
            }
        }

        String _filePath = "";
        public string FilePath
        {
            get { return _filePath; }
        }

        public XmlItemTypes XmlItemType
        {
            get { throw new NotImplementedException(); }
        }

        public Type Type
        {
            get { throw new NotImplementedException(); }
        }

        void IXmlItem.LoadXml(string xmlFile, bool refLoad)
        {
            LoadXml(xmlFile, refLoad);
        }

        public XmlNode GetXml(XmlDocument xDoc, XmlNode parent = null)
        {
            XmlNode actions = XmlAdder.Element(xDoc, "Actions", parent);
            for (int i = 0; i < this.Count; i++)
            {
                this[i].GetXml(xDoc, actions);
            }
            return actions;
        }
        XmlItemInterface _interface;
        public XmlItemInterface Interface { get { return _interface; } set { _interface = value; } }
    }
}
