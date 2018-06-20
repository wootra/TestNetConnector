using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetworkPacket;
using XmlDesigner;
using System.Xml;
using XmlHandlers;
using System.Xml.Schema;
using System.IO;
using System.Windows.Forms;

namespace XmlDesigner.PacketDatas
{
    public class XmlCommand : IXmlItem, IScenarioItem
    {

        public static String NowLoadingFile = "";
        public static XmlCommand ActivePacket;
        public Byte[] Buffer;
        
        public XmlCommand(String name, XmlTemplate xmlTemplate, XmlScenario parentScenario)
        {
            _name = name;
            _template = xmlTemplate.Clone();
            _parentScenario = parentScenario;
        }

        public ScenarioItemTypes ScenarioItemType { get { return ScenarioItemTypes.Command; } }

        

        XmlDocument _xDoc;  
        
        public void LoadXml(String xmlFile, Boolean refLoad = false)
        {
            if (XmlScenario.NowLoadingPath.Length > 0) _filePath = XmlScenario.NowLoadingPath + XmlScenario.PathSeperator + xmlFile;
            else _filePath = xmlFile;
            
            NowLoadingFile = _filePath;

            XmlNode xNode = XmlGetter.RootNode(out _xDoc,_filePath, null, XmlSchemaValidation);
            
            try
            {
                LoadXml(_xDoc, xNode, refLoad);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + ":" + xmlFile);
            }


        }



        void XmlSchemaValidation(object sender, ValidationEventArgs e)
        {
            MessageBox.Show(e.Message);
        }


        public void SaveXml(String xmlFile)
        {

        }

        XmlScenario _parentScenario;
        String _name = "";
        public String Name
        {
            get { return _name; }
            set {
                if (_parentScenario != null)
                {
                    if(_parentScenario.ContainsName(value)){
                        throw new Exception("Command ["+value+"] exists in the scenario already...");
                    }
                    
                }
                _name = value; 
            }
        }

        XmlTemplate _template;
        
        public XmlTemplate Template //not using..
        {
            get { return _template; }
            set
            {
                _template = value;
                this.Interface.Node.Attributes["Template"].Value = value.Info.Name;
            }
        }
        
        public void MakeCommandPacket()
        {
            

        }

        public PacketTypes PacketType
        {
            get{
                return _template.Info.PacketType;
            }
        }

        public XmlCommandStructDefinition Command
        {
            get
            {
                return _template.Command;
            }
        }

        public XmlCommandStructDefinition Response
        {
            get { return _template.Response; }
        }

        public XmlCommandStructDefinition AutoResponse
        {
            get { return _template.AutoResponse; }
        }

        public XmlCommandHeader CommandHeader
        {
            get
            {
                return _template.Command.Header;
            }
        }

        public XmlCommandFields CommandData
        {
            get{
                return _template.Command.Data;
            }
        }

        public XmlCommandHeader ResponseHeader
        {
            get
            {
                return _template.Response.Header;
            }
        }

        public XmlCommandFields ResponseData
        {
            get
            {
                return _template.Response.Data;
            }
        }
       
        public void LoadXml(XmlDocument xDoc, XmlNode rootNode, Boolean refLoad = false)
        {
            if (rootNode == null) return;
            
            _xDoc = xDoc;  
            
            XmlControlHandler.GetDefaultXmlItemAttributes(rootNode, xDoc, this);

            XmlCommandFields data = _template.Command.Data;
            data.loadFields(xDoc, rootNode);
        }

        String _filePath = "";
        public string FilePath
        {
            get { return _filePath; }
        }

        public Type Type
        {
            get { return typeof(XmlImageList); }
        }
        IXmlResponseCondition ResponseCondition;

        public XmlNode GetXml(XmlDocument xDoc, XmlNode parent = null)
        {
            XmlNode root = XmlAdder.Element(xDoc, "Command", parent);
            
            XmlAdder.Attribute(xDoc, "Name", Name, root);
            XmlAdder.Attribute(xDoc, "Template", Template.FullPath, root);

            foreach(XmlCommandField field in Template.Command.Data.FieldList.Values){
                if (field.FieldType == FieldTypes.Dynamic)
                {
                    XmlAdder.Attribute(xDoc, field.Name, field.Data, root);
                }
            }
            if (ResponseCondition != null) ResponseCondition.GetXml(xDoc, parent);


            foreach (XmlCommandFields fields in Template.Command.Data.FieldsList.Values)
            {
                XmlNode node = fields.GetXml(xDoc, root);//

            }
            return root;
        }

        public bool? IsPassed()
        {
            if (ResponseCondition == null) //ResponseCondition이 없으면 Header의 status로 판단..
            {
                int passed = ((int)(ResponseHeader.Field("status").DataValue));
                if (passed == 1) return true;
                else if (passed == 0) return false;
                else return (bool?)null;
            }
            else
            {
                return ResponseCondition.GetCondition();
            }
        }

        public void AddCondition(String fieldName, String symbol, String value, String groupingSymbol="")
        {
            
            XmlResponseCondition cond = new XmlResponseCondition(this, fieldName, symbol, value);

            if (ResponseCondition != null)
            {
                XmlResponseConditionGroup.GroupingSymbols groupSymbol = XmlResponseConditionGroup.GetSymbol(groupingSymbol);

                XmlResponseConditionGroup grp = new XmlResponseConditionGroup(this, groupSymbol, ResponseCondition, cond);
                ResponseCondition = grp;
            }
            else
            {
                ResponseCondition = cond;
            }
        }

        XmlItemInterface _interface;
        public XmlItemInterface Interface { get { return _interface; } set { _interface = value; } }
    }

    public class XmlResponseCondition : IXmlResponseCondition
    {
        public XmlCommand _command;
        public XmlCommandField Field;
        public enum CondSymbols { GT=0, LT, EQ, GE, LE, NE };
        CondSymbols CondSymbol;
        public String Value;

        public XmlResponseCondition(XmlCommand command, String fieldName, CondSymbols symbol, String value)
        {
            _command = command;
            Field = _command.ResponseData.Field(fieldName);
            CondSymbol = symbol;
            Value = value;
        }

        public XmlResponseCondition(XmlCommand command, String fieldName, String symbolText, String value)
        {
            _command = command;
            Field = _command.ResponseData.Field(fieldName);
            CondSymbol = GetSymbol(symbolText);
            Value = value;
        }
        
        internal XmlResponseCondition(XmlCommand command)
        {
            _command = command;
        }

        public bool GetCondition()
        {
            XmlCommandField clone = Field.Clone();
            clone.Data = Value;
                
            if (CondSymbol == CondSymbols.EQ)
            {
                if(Field.Data.Equals(clone.Data)) return true;
                else return false;
            }
            else if (CondSymbol == CondSymbols.GE)
            {
                if (Field.DataType == typeof(float))
                {
                    if ((float)Field.DataValue >= (float)clone.DataValue) return true;
                    else return false;
                }
                else
                {
                    if (Convert.ToInt64(Field.DataValue) >= Convert.ToInt64(clone.DataValue)) return true;
                    else return false;
                }
            }
            else if (CondSymbol == CondSymbols.GT)
            {
                if (Field.DataType == typeof(float))
                {
                    if ((float)Field.DataValue > (float)clone.DataValue) return true;
                    else return false;
                }
                else
                {
                    if (Convert.ToInt64(Field.DataValue) > Convert.ToInt64(clone.DataValue)) return true;
                    else return false;
                }
            }
            else if (CondSymbol == CondSymbols.LE)
            {
                if (Field.DataType == typeof(float))
                {
                    if ((float)Field.DataValue <= (float)clone.DataValue) return true;
                    else return false;
                }
                else
                {
                    if (Convert.ToInt64(Field.DataValue) <= Convert.ToInt64(clone.DataValue)) return true;
                    else return false;
                }
            }
            else if (CondSymbol == CondSymbols.LT)
            {
                if (Field.DataType == typeof(float))
                {
                    if ((float)Field.DataValue < (float)clone.DataValue) return true;
                    else return false;
                }
                else
                {
                    if (Convert.ToInt64(Field.DataValue) < Convert.ToInt64(clone.DataValue)) return true;
                    else return false;
                }
            }
            else// if (CondSymbol == CondSymbols.NE)
            {
                if (Field.DataType == typeof(float))
                {
                    if ((float)Field.DataValue != (float)clone.DataValue) return true;
                    else return false;
                }
                else
                {
                    if (Convert.ToInt64(Field.DataValue) != Convert.ToInt64(clone.DataValue)) return true;
                    else return false;
                }
            }
            
        }
        public void LoadXml(XmlNode root)
        {
            string cond = XmlGetter.Attribute(root, "CondText");
            string fieldName = "";
            string symbol = "";
            string value="";

            for (int i = 0; i < cond.Length; i++)
            {
                if (cond[i].Equals('=') || cond[i].Equals('<') || cond[i].Equals('>') || cond[i].Equals('!'))
                {
                    symbol += cond[i];
                }
                else if (symbol.Length == 0)
                {
                    fieldName += cond[i];
                }
                else
                {
                    Value += cond[i];
                }
            }
            Field = _command.ResponseData.Field(fieldName.Trim());
            switch (symbol.Trim())
            {
                case "==":
                    CondSymbol = CondSymbols.EQ;
                    break;
                case ">=":
                    CondSymbol = CondSymbols.GE;
                    break;
                case ">":
                    CondSymbol = CondSymbols.GT;
                    break;
                case "<=":
                    CondSymbol = CondSymbols.LE;
                    break;
                case "<":
                    CondSymbol = CondSymbols.LT;
                    break;
                case "!=":
                    CondSymbol = CondSymbols.NE;
                    break;
            }
            Value = value.Trim();
        }

        public static CondSymbols GetSymbol(String symbolText){
            CondSymbols symbol = CondSymbols.EQ;
             switch (symbolText.Trim())
            {
                case "==":
                    symbol = CondSymbols.EQ;
                    break;
                case ">=":
                    symbol = CondSymbols.GE;
                    break;
                case ">":
                    symbol = CondSymbols.GT;
                    break;
                case "<=":
                    symbol = CondSymbols.LE;
                    break;
                case "<":
                    symbol = CondSymbols.LT;
                    break;
                case "!=":
                    symbol = CondSymbols.NE;
                    break;
            }
            return symbol;
        }

        public void GetXml(XmlDocument xDoc, XmlNode parent)
        {
            XmlNode root = XmlAdder.Element(xDoc, "RespondCond", parent);
            String cond = Field.Name;
            switch (CondSymbol)
            {
                case CondSymbols.EQ:
                    cond += "==";
                    break;
                case CondSymbols.GE:
                    cond += ">=";
                    break;
                case CondSymbols.GT:
                    cond += ">";
                    break;
                case CondSymbols.LE:
                    cond += "<=";
                    break;
                case CondSymbols.LT:
                    cond += "<";
                    break;
                case CondSymbols.NE:
                    cond += "!=";
                    break;
                default:
                    break;
            }
            cond += Value;
            XmlAdder.Attribute(xDoc, "CondText",cond, root);
        }
    }

    public class XmlResponseConditionGroup : IXmlResponseCondition
    {
        public IXmlResponseCondition _condition1;
        public enum GroupingSymbols { And = 0, Or, True, NotTrue, NULL };
        public GroupingSymbols GroupingSymbol = GroupingSymbols.And;
        public IXmlResponseCondition _condition2;
        public XmlCommand _command;

        public XmlNode GetXml(XmlDocument xDoc, XmlNode parent)
        {
            XmlNode root = XmlHandlers.XmlAdder.Element(xDoc, "CondGroup", parent);
            XmlHandlers.XmlAdder.Attribute(xDoc, "Symbol", GroupingSymbol.ToString(), root);
           
            _condition1.GetXml(xDoc, root);
            _condition2.GetXml(xDoc, root);
            return root;
        }

        public static GroupingSymbols GetSymbol(String symbolText)
        {
            switch (symbolText.Trim().ToLower())
            {
                case "and":
                    return GroupingSymbols.And;
                case "or":
                    return GroupingSymbols.Or;
                case "true":
                    return GroupingSymbols.True;
                case "nottrue":
                    return GroupingSymbols.NotTrue;
            }
            return GroupingSymbols.True;
        }

        public void LoadXml( XmlNode rootNode)
        {
            string symbol = XmlGetter.Attribute(rootNode, "Symbol");
            for (int i = 0; i < (int)GroupingSymbols.NULL; i++)
            {
                if (((GroupingSymbols)i).ToString().ToLower().Equals(symbol.ToLower()))
                {
                    GroupingSymbol = (GroupingSymbols)i;
                    break;
                }
            }

             int count = 0;
            foreach (XmlNode respCond in rootNode.ChildNodes)
            {
                if (respCond.Name.Equals("CondGroup"))
                {
                    if (count++ == 0)
                    {
                        _condition1 = new XmlResponseConditionGroup(_command);
                        _condition1.LoadXml(respCond);
                    }
                    else
                    {
                        _condition2 = new XmlResponseConditionGroup(_command);
                        _condition2.LoadXml(respCond);

                    }
                }
                else if (respCond.Name.Equals("Cond"))
                {
                    if (count++ == 0)
                    {
                        _condition1 = new XmlResponseCondition(_command);
                        _condition1.LoadXml(respCond);
                    }
                    else
                    {
                        _condition2 = new XmlResponseCondition(_command);
                        _condition2.LoadXml(respCond);

                    }
                }
            }
        }

        public bool GetCondition()
        {
            if (GroupingSymbol == GroupingSymbols.And)
            {
                return (_condition1.GetCondition() && _condition2.GetCondition());
            }
            else if(GroupingSymbol == GroupingSymbols.Or)
            {
                return (_condition1.GetCondition() || _condition2.GetCondition());
            }
            else if (GroupingSymbol == GroupingSymbols.True)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public XmlResponseConditionGroup(XmlCommand command, GroupingSymbols symbol, IXmlResponseCondition cond1, IXmlResponseCondition cond2)
        {
            _command = command;
            if (symbol == GroupingSymbols.True || symbol == GroupingSymbols.Or)
            {
                throw new Exception("And 와 Or 속성은 XmlResponseConditionGroup(symbol, cond1, cond2)를 사용하십시오");
            }
            _condition1 = cond1;
            _condition2 = cond2;
        }
        public XmlResponseConditionGroup(XmlCommand command, GroupingSymbols symbol)
        {
            _command = command;
            if (symbol == GroupingSymbols.And || symbol == GroupingSymbols.Or)
            {
                throw new Exception("And 와 Or 속성은 XmlResponseConditionGroup(symbol, cond1, cond2)를 사용하십시오");
            }
            else
            {
                GroupingSymbol = symbol;
            }
        }
        private XmlResponseConditionGroup(XmlCommand command)
        {
            _command = command;
        }
    }

    public interface IXmlResponseCondition
    {
        bool GetCondition();
        XmlNode GetXml(XmlDocument xDoc, XmlNode parent);
        void LoadXml( XmlNode rootNode);
    }
}
