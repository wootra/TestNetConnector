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

    public class XmlCompareCondition : XmlCondition
    {
        public List<XmlConditionValue> _values = new List<XmlConditionValue>();
        
        internal XmlCompareCondition(XmlConditionTypes condType)
        {
            _conditionType = condType;
        }

        public override String getConditionText()
        {
            String condText = "(";
            for (int i = 0; i < _values.Count; i++)
            {
                if (i != 0)
                {
                    if (_conditionType == XmlConditionTypes.Equal) condText += " == ";
                    else if (_conditionType == XmlConditionTypes.More) condText += " < ";
                    else if (_conditionType == XmlConditionTypes.MoreOrEqual) condText += " <= ";
                }
                condText += _values[i].TypeName+"("+ _values[i].Value+")";
            }
            condText += ")";
            return condText;
        }

        public void Add(XmlConditionValue cond)
        {
            _values.Add(cond);
        }

        public void Clear()
        {
            _values.Clear();
        }

        public override bool GetCondition()
        {
            if (_values.Count == 0) return true;
            if (_values.Count < 2)
            {
                throw new Exception(((XmlConditionTypes)_conditionType).ToString() + " 요소에는 반드시 Value 요소가 2개이상 들어가야 합니다.");
            }
            IXmlComponent loadingComponent = XmlComponent.NowLoading;
            

            for (int i = 1; i < _values.Count; i++)
            {
                String value1 = (_values[i].IsVariable) ? GetValue(_values[i].Value).ToString() : _values[i].Value;
                String value2 = (_values[i - 1].IsVariable) ? GetValue(_values[i - 1].Value).ToString() : _values[i - 1].Value;
                
                value1 = (_values[i].IsComponent) ? GetComponentValue(_values[i].Value) : _values[i].Value;
                value2 = (_values[i - 1].IsComponent) ? GetComponentValue(_values[i - 1].Value) : _values[i - 1].Value;

                String value1Type = _values[i].TypeName;
                String value2Type = _values[i - 1].TypeName;

                switch (_conditionType)
                {
                    case XmlConditionTypes.Equal:
                        if (value1.Trim().Equals(value2.Trim("\t \n\r".ToCharArray()))==false) return false;
                        break;
                    case XmlConditionTypes.More:
                        if (DataComparer.Minus(value1Type, value1, value2Type, value2) <= 0) return false;
                        break;
                    case XmlConditionTypes.MoreOrEqual:
                        if (DataComparer.Minus(value1Type, value1, value2Type, value2) < 0) return false;
                        break;
                    default:
                        break;
                }
            }
            return true;
        }

        public object GetValue(String valueName)
        {
            return GlobalVars.Var(valueName).Value;
        }

        public String GetComponentValue(String componentPath)
        {
            return _component.Interface.GetComponentValue(componentPath);
        }

        public override void LoadXml(XmlDocument xDoc, XmlNode rootNode, Boolean refLoad=false)
        {
            if (rootNode == null) return;
            XmlControlHandler.GetDefaultXmlItemAttributes(rootNode, xDoc, this);

            for (int i = 0; i < rootNode.ChildNodes.Count; i++)
            {
                XmlNode xValues = rootNode.ChildNodes[i];
                foreach (XmlNode xValue in xValues.ChildNodes)
                {
                    string type = XmlGetter.Attribute(xValue, "Type");
                    bool isVariable = XmlGetter.Attribute(xValue, "IsVariable").Equals("true");
                    bool isComponent = XmlGetter.Attribute(xValue, "IsComponent").Equals("true");

                    string value = xValue.InnerText;
                    XmlConditionValue condValue = new XmlConditionValue(type, value, isVariable, isComponent);
                    _values.Add(condValue);
                }
            }
        }

        public override void SaveXml(string xmlFile)
        {
            
        }


        public override XmlNode GetXml(XmlDocument xDoc, XmlNode parent=null)
        {
            XmlNode root = XmlAdder.Element(xDoc, _conditionType.ToString(), parent as XmlElement);
            for (int i = 0; i < _values.Count; i++)
            {
                XmlNode xValue = XmlAdder.Element(xDoc, "Value", _values[i].Value, root);
                XmlAdder.Attribute(xDoc, "Type", _values[i].TypeName, xValue);
                XmlAdder.Attribute(xDoc, "IsVariable", _values[i].IsVariable ? "true" : "false", xValue);
                root.AppendChild(xValue);
            }
            return root;
        }
        XmlItemInterface _interface;
        public XmlItemInterface Interface { get { return _interface; } set { _interface = value; } }

    }



    
}
