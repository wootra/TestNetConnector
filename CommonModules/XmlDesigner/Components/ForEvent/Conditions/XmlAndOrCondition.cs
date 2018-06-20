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

namespace XmlDesigner.ForEvents.Conditions
{
    public class XmlAndOrCondition: XmlCondition
    {
        protected List<XmlCondition> _conditions = new List<XmlCondition>();

        internal XmlAndOrCondition(XmlConditionTypes condType)
        {
            _conditionType = condType;
        }
        
       
        public override String getConditionText()
        {
            String condText = "(";
            for (int i = 0; i < _conditions.Count; i++)
            {
                if (i != 0) condText += ((XmlConditionTypes)_conditionType).ToString();
                condText += _conditions[i].getConditionText();
            }
            condText += ")";
            return condText;
        }

       
        public override bool GetCondition()
        {
            for (int i = 0; i < _conditions.Count; i++)
            {
                switch (_conditionType)
                {
                    case XmlConditionTypes.Or:
                        if (_conditions[i].GetCondition()) return true; //or
                        break;
                    case XmlConditionTypes.And:
                        if (_conditions[i].GetCondition()==false) return false;
                        break;
                    default:

                        break;

                }
            }
            //loop을 다 돌고 나서도 결정나지 않았으면..
            switch (_conditionType)
            {
                case XmlConditionTypes.Or:
                    return false; //or
                case XmlConditionTypes.And:
                    return true;
                default:
                    return false;
            }
        }

        public void Add(XmlCondition cond)
        {
            _conditions.Add(cond);
        }

        public void Clear()
        {
            _conditions.Clear();
        }

        public override void LoadXml(XmlDocument xDoc, XmlNode rootNode, Boolean refLoad=false)
        {
            if (rootNode == null) return;
            XmlControlHandler.GetDefaultXmlItemAttributes(rootNode, xDoc, this);

            for (int i = 0; i < rootNode.ChildNodes.Count; i++)
            {
                XmlNode xCond = rootNode.ChildNodes[i];
                XmlCondition cond = XmlCondition.New(xCond.Name);
                cond.LoadXml(xDoc, xCond);
            }
        }

        public override void SaveXml(string xmlFile)
        {
            
        }

        public override XmlNode GetXml(XmlDocument xDoc, XmlNode parent)
        {
            XmlNode root = XmlAdder.Element(xDoc, _conditionType.ToString(), parent as XmlElement);
            for (int i = 0; i < _conditions.Count; i++)
            {
                _conditions[i].GetXml(xDoc, root);
            }
            return root;
        }
        XmlItemInterface _interface;
        public XmlItemInterface Interface { get { return _interface; } set { _interface = value; } }
    }



    
}
