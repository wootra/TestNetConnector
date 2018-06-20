using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataHandling;


namespace XmlDesigner.ForEvents.Conditions
{

    public class XmlConditionValue 
    {
        string _type;
        string _value;
        bool _isVariable;
        bool _isComponent;
        public XmlConditionValue(string type, string value, bool isVariable=false, bool isComponent=false)
        {
            _type = type;
            _value = value;
            _isVariable = isVariable;
            _isComponent = isComponent;
        }

        public String TypeName { get { return _type; } }

        public string Value { get { return _value;}}

        public bool IsVariable { get { return _isVariable; } }

        public bool IsComponent { get { return _isComponent; } }
    }

    
}
