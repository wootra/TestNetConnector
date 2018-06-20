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

using XmlDesigner.ForEvents.Conditions;

namespace XmlDesigner.ForEvents
{
    public class XmlEvent : IXmlItem
    {
        public String _eventType;

        public List<XmlActionList> Actions = new List<XmlActionList>();// new XmlActionList();


        public XmlEvent()
        {
            XmlCondition.New(XmlConditionTypes.And);
        }

        public XmlEvent(String eventType, List<XmlActionList> actions)
        {
            _eventType = eventType;
            
            Actions = actions;
        }

        public String EventType { get { return _eventType; } }
      
        XmlDocument _xDoc;  
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
                MessageBox.Show("XmlEvent.LoadXml:" + e.Message + ":" + xmlFile);
            }
        }

 

        void XmlSchemaValidation(object sender, ValidationEventArgs e)
        {
            MessageBox.Show("XmlEvent.XmlSchemaValidation:" + e.Message);
        }


        public void SaveXml(String xmlFile)
        {

        }

        /// <summary>
        /// OnClick, OnDoubleClick등의 Tag에 대해 분석한다.
        /// </summary>
        /// <param name="rootNode"></param>
        public void LoadXml(XmlDocument xDoc, XmlNode rootNode, bool refLoad=false){
            if (rootNode == null) return;
            Actions.Clear();

            XmlControlHandler.GetDefaultXmlItemAttributes(rootNode, xDoc, this);

            XmlCondition cCond = null;
            _eventType = rootNode.Name;// (EventTypes)GlobalVars.EventTypes.ToList().IndexOf(rootNode.Name);

            List<XmlActionList> cActionGroup = new List<XmlActionList>();// XmlActionList();
            for (int i = 0; i < rootNode.ChildNodes.Count; i++)
            {
                
                XmlNode xChild = rootNode.ChildNodes[i];
                XmlActionList cActions = new XmlActionList();
                if(xChild.Name.Equals("Actions")){
                    cActions.LoadXml(xDoc, xChild);
                    Actions.Add(cActions);
                }

            }
            if (cCond == null) cCond = XmlCondition.New(XmlConditionTypes.True);
            
            
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
            XmlNode eventNode = XmlAdder.Element(xDoc, _eventType.ToString(), parent);
            for (int i = 0; i < Actions.Count; i++)
            {

                Actions[i].GetXml(xDoc, eventNode);
            }
            return eventNode;
        }
        XmlItemInterface _interface;
        public XmlItemInterface Interface { get { return _interface; } set { _interface = value; } }
    }
}
