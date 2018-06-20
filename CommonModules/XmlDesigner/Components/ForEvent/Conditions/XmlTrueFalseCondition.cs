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
    public class XmlCondTrueFalse : XmlCondition
    {

        internal XmlCondTrueFalse(XmlConditionTypes condType)
        {
            _conditionType = condType;
        }

        public override String getConditionText()
        {
            return _conditionType.ToString();
        }

        public override bool GetCondition()
        {
            if (_conditionType == XmlConditionTypes.True) return true;
            else return false;
        }



        public override void SaveXml(string xmlFile)
        {
            
        }

        public override void LoadXml(XmlDocument xDoc, XmlNode rootNode, Boolean refLoad=false)
        {
            //do not anything.
        }

        public override XmlNode GetXml(XmlDocument xDoc, XmlNode parent = null)
        {
            return XmlAdder.Element(xDoc, _conditionType.ToString(), parent);
        }
        XmlItemInterface _interface;
        public XmlItemInterface Interface { get { return _interface; } set { _interface = value; } }
    }

}
