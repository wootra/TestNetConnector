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
using XmlDesigner.Parsers;
using XmlDesigner.ForEvents.Conditions;


namespace TestNgineData.PacketDatas
{
    public class XmlMatchData: IXmlItem
    {
        public static XmlMatchData NowLoading;
        List<XmlMatchItem.ActiveTimes> _activeTimes;
        XmlMatchItem _matchItem;

        public XmlMatchData(List<XmlMatchItem.ActiveTimes> activeTimes, String fieldName)
        {
            _fieldName = fieldName;
            _activeTimes = activeTimes;
        }

        public List<XmlMatchItem.ActiveTimes> ActiveTimes { get { return _activeTimes; } }

        //String _buffField;

        /// <summary>
        /// ToTable방향일 때 유효함.
        /// </summary>
        //public string FromField { get { return _buffField; } } 

        XmlDocument _xDoc;  
        public void LoadXml(String xmlFile, Boolean refLoad = false)
        {
            if (XmlScenario.NowLoadingPath.Length > 0) _filePath = XmlScenario.NowLoadingPath + XmlScenario.PathSeperator + xmlFile;
            else _filePath = xmlFile;

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

        
        
        List<XmlMatchComponent> _components = new List<XmlMatchComponent>();
        public List<XmlMatchComponent> Components { get { return _components; } }

        XmlCondition _condition;
        public XmlCondition Condition { get { return _condition; } }

        String _fieldName;
        public String FieldName { get { return _fieldName; } }

        public void LoadXml(XmlDocument xDoc, XmlNode rootNode, Boolean refLoad = false)
        {
            if (rootNode == null) return;
            _xDoc = xDoc;  XmlControlHandler.GetDefaultXmlItemAttributes(rootNode, xDoc, this);

            NowLoading = this;
            _matchItem = XmlMatchItem.NowLoading;
            
            foreach (XmlNode child in rootNode.ChildNodes)
            {
                if (child.Name.Equals("Condition"))
                {
                    XmlNode condNode = XmlHandlers.XmlGetter.FirstChild(child);
                    XmlCondition cond = XmlCondition.New(condNode.Name);
                    cond.LoadXml(xDoc, condNode, refLoad);
                    _condition = cond;
                }
                else if (child.Name.Equals("Component"))
                {
                    XmlMatchComponent comp = new XmlMatchComponent();
                    comp.LoadXml(xDoc, child);
                    _components.Add(comp);
                }
            }
            if (_condition == null) _condition = XmlCondition.New(XmlConditionTypes.True); //default condition

        }
        /*
        public String FindForInit(XmlCommand packet)
        {
            //String columnName = matchItem.ColumnName(field);
            String fieldPos = "Header";
            String fieldName = "id";
            getFieldUnit(_fieldName, out fieldPos, out fieldName);
            string value = "";
            if (fieldPos.Equals("Command.Header"))
            {
                XmlCommandField field = packet.CommandHeader.Field(fieldName);
                if(field==null) value="";// throw new Exception("TargetTemplate:"+_matchItem.TargetTemplate + " : Command.Data에 Field Name[" + fieldName + "] 이 없는 것 같습니다.");
                else value = field.Data;
            }
            else if (fieldPos.Equals("Command.Data"))
            {
                XmlCommandField field = packet.CommandData.Field(fieldName);
                if (field == null) 
                    value = "";//throw new Exception("TargetTemplate:" + _matchItem.TargetTemplate + " : Command.Data에 Field Name[" + fieldName + "] 이 없는 것 같습니다.");
                else value = field.Data;
            }
            else if (fieldPos.Equals("String")) //사용자가 String. 뒤에 적은 String을 그대로 적어줌..
            {
                value = fieldName;
            }
            else if (fieldPos.Equals("Packet")) //사용자가 String. 뒤에 적은 String을 그대로 적어줌..
            {
                switch (fieldName)
                {
                    case "Port":
                        value = packet.PortName;
                        break;
                    case "TargetTemplate":
                        value = packet.Template;
                        break;
                    default:
                        value = "";
                        break;
                }
                
            }
            else if (fieldPos.Equals("Info"))
            {
                switch (fieldName)
                {
                    case "Name":
                        value = packet.Info.Name;
                        break;
                    case "Description":
                        value = packet.Info.Description;
                        break;
                }
            }
            else
            {
                throw new Exception("TargetTemplate:"+_matchItem.TargetTemplate + " : "+_fieldName + "을 찾을 수 없습니다.");
            }
            return value;
        }

        public String FindForResponse(XmlCommand packet)
        {
            //String columnName = matchItem.ColumnName(field);
            String fieldPos = "Header";
            String fieldName = "id";
            getFieldUnit(_fieldName, out fieldPos, out fieldName);
            string value = "";
            if (fieldPos.Equals("Response.Header"))
            {
                XmlCommandField field = packet.Response.Header.Field(fieldName);
                if (field == null) value = "";// throw new Exception("TargetTemplate:" + _matchItem.TargetTemplate + " : Command.Data에 Field Name[" + fieldName + "] 이 없는 것 같습니다.");
                else value = field.DataValue.ToString();
            }
            else if (fieldPos.Equals("Response.Data"))
            {
                XmlCommandField field = packet.Response.Data.Field(fieldName);
                if (field == null)
                    value = "";//throw new Exception("TargetTemplate:" + _matchItem.TargetTemplate + " : Command.Data에 Field Name[" + fieldName + "] 이 없는 것 같습니다.");
                else value = field.DataValue.ToString();
            }
            else if (fieldPos.Equals("String")) //사용자가 String. 뒤에 적은 String을 그대로 적어줌..
            {
                //do nothing..
            }
            else if (fieldPos.Equals("Info"))
            {
                //do nothing..
            }
            else if (fieldPos.Equals("Packet"))
            {

            }
            else
            {
                throw new Exception("TargetTemplate:" + _matchItem.TargetTemplate + " : " + _fieldName + "을 찾을 수 없습니다.");
            }

            foreach (XmlMatchComponent com in Components)
            {
                for (int i = 0; i < com.MatchInfoItems.Count; i++)
                {
                    com.MatchInfoItems[i].SetValue(value);
                }
            }
            return value;
        }

        public void SetDataToPacket(XmlCommand packet, int rowIndex)
        {
             
            //String columnName = matchItem.ColumnName(field);
            String fieldPos = "Header";
            String fieldName = "id";
            getFieldUnit(_fieldName, out fieldPos, out fieldName);
            
            if (fieldPos.Equals("Command.Header"))
            {
                XmlCommandField field = packet.CommandHeader.Field(fieldName);
                if (field == null) return;//  throw new Exception("Command.Header에 Field Name[" + fieldName + "] 이 없는 것 같습니다.");
                else
                {
                    field.Data = SendingParsers.RunParser(
                        this.Components[0].MatchInfoItems[0].GetValue(rowIndex),
                        this.Components[0].SendingParser,
                        this.Components[0].Args
                        ).ToString();
                }
            }
            else if (fieldPos.Equals("Command.Data"))
            {
                XmlCommandField field = packet.CommandData.Field(fieldName);
                if (field == null) return;// throw new Exception("Command.Data에 Field Name[" + fieldName + "] 이 없는 것 같습니다.");
                else
                {
                    field.Data = SendingParsers.RunParser(
                        this.Components[0].MatchInfoItems[0].GetValue(rowIndex),
                        this.Components[0].SendingParser,
                        this.Components[0].Args
                        ).ToString();
                }
            }
            else if (fieldPos.Equals("String")) //사용자가 String. 뒤에 적은 String을 그대로 적어줌..
            {
                //Send모드시에는 필요없다.
            }
            else if (fieldPos.Equals("Info"))
            {
                //SendMode 시에는 필요없다.
            }
            else if(fieldPos.Equals("Packet")){

                if (fieldName.Equals("Port"))
                {
                    packet.PortName = _components[0].MatchInfoItems[0].GetValue(rowIndex).ToString();
                }
            }
            else
            {
                throw new Exception(_fieldName + "을 찾을 수 없습니다.");
            }
        }
         void getFieldUnit(String fullFieldId, out String fieldPos, out String fieldName)
        {
            int index = fullFieldId.LastIndexOf(".");
            if (index < 0)
            {
                throw new Exception("에러:" + fullFieldId + ":Match 태그의 FieldName은 {위치}.{Field태그의 Name값} 형식이 되어야 합니다. 예>Command.Header.id 또는 Info.Name");
            }

            fieldPos = fullFieldId.Substring(0, index);
            fieldName = fullFieldId.Substring(index + 1);
        }
        */
        String _filePath = "";
        public string FilePath
        {
            get { return _filePath; }
        }

        public Type Type
        {
            get { return typeof(XmlImageList); }
        }

        public XmlNode GetXml(XmlDocument xDoc, XmlNode parent = null)
        {

            return null;
        }

        XmlItemInterface _interface;
        public XmlItemInterface Interface { get { return _interface; } set { _interface = value; } }
    }
}
