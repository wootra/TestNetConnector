using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataHandling;
using XmlDesigner;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using XmlHandlers;
using XmlDesigner.ForEvents;

namespace XmlDesigner.ForEvents.Conditions
{


    public abstract class XmlCondition : IXmlItem, IXmlComponentCondition
    {

        XmlDocument _xDoc;
        protected XmlConditionTypes _conditionType = XmlConditionTypes.And;
        protected static String[] _conditionTexts = new string[]{ "And", "Or", "Equal", "More", "MoreOrEqual", "True", "False"};
        string _filePath = "";
        protected IXmlItem _component;

        public static XmlCondition New(XmlConditionTypes condType)
        {
            XmlCondition condition;
            switch (condType)
            {

                case XmlConditionTypes.Equal:
                case XmlConditionTypes.More:
                case XmlConditionTypes.MoreOrEqual:
                    condition = new XmlCompareCondition(condType);
                    break;
                case XmlConditionTypes.Or:
                case XmlConditionTypes.And:
                    condition= new XmlAndOrCondition(condType);
                    break;
                case XmlConditionTypes.True:
                case XmlConditionTypes.False:
                    condition = new XmlCondTrueFalse(condType);
                    break;
                default:
                    throw new Exception("XmlConditionMaker : 해당 타입 " + condType + "은 사용할 수 없습니다.");
            }
            condition._component = XmlControlHandler.NowEventLoadingXmlItem;

            return condition;
        }

        public static XmlCondition New(string conditionType)
        {
            XmlConditionTypes condType = (XmlConditionTypes)(_conditionTexts.ToList().IndexOf(conditionType));

            return New(condType);
        }

        public XmlConditionTypes ConditionType
        {
            get { return _conditionType; }
        }

        public abstract bool GetCondition();

        public abstract string getConditionText();
        
        public XmlItemTypes XmlItemType
        {
            get { return XmlItemTypes.Condition; }
        }

        public virtual Type Type
        {
            get { return typeof(XmlCondition); }
        }

        public XmlNode LoadXml(String xmlFile, Boolean refLoad=false)
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
                throw new Exception(e.Message + ":" + xmlFile);
            }
            return xNode;
        }



        void XmlSchemaValidation(object sender, ValidationEventArgs e)
        {
            throw new Exception(e.Message);
        }


        public abstract void SaveXml(String xmlFile);



        public abstract void LoadXml(XmlDocument xDoc, XmlNode rootNode, Boolean refLoad=false);

        public string FilePath
        {
            get { return _filePath; }
        }

        public abstract XmlNode GetXml(XmlDocument xDoc, XmlNode parent=null);

        void IXmlItem.LoadXml(string xmlFile, bool refLoad)
        {
            LoadXml(xmlFile, refLoad);
        }
        XmlItemInterface _interface;
        public XmlItemInterface Interface { get { return _interface; } set { _interface = value; } }
    }

    
}
