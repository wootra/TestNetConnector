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


namespace TestNgineData.PacketDatas
{
    public class XmlMatchComponent: IXmlItem
    {
        public XmlMatchComponent()
        {
        }

         XmlDocument _xDoc;  public void LoadXml(String xmlFile, Boolean refLoad = false)
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

        
        static Dictionary<String, String> _recentTarget = new Dictionary<string, string>();


        List<IXmlComponent> _targetComponents = new List<IXmlComponent>();
        public List<IXmlComponent> TargetComponents { get { return _targetComponents; } }

        List<IXmlMatchInfo> _matchInfoItems = new List<IXmlMatchInfo>();
        public List<IXmlMatchInfo> MatchInfoItems { get { return _matchInfoItems; } }

        String _sendingParser = "";
        public String SendingParser { get { return _sendingParser; } }

        String[] _args;
        public String[] Args { get { return _args; } }

        public virtual void LoadXml(XmlDocument xDoc, XmlNode rootNode, Boolean refLoad = false)
        {
            if (rootNode == null) return;
            _xDoc = xDoc;  XmlControlHandler.GetDefaultXmlItemAttributes(rootNode, xDoc, this);

            _sendingParser = XmlGetter.Attribute(rootNode, "SendingParser");

            _args = ValueParser.getArgs(XmlGetter.Attribute(rootNode, "Args"));

            _format = new XmlMatchFormat(); //초기값.
            foreach (XmlNode child in rootNode.ChildNodes)
            {
                if (child.Name.Equals("Format"))
                {
                    XmlNode xFormat = child;
                    _format = new XmlMatchFormat();
                    if (xFormat != null)
                    {
                        _format.LoadXml(xDoc, xFormat, refLoad);

                    }
                }
                else{
                    String comName = child.Name;
                    string recentName = null;
                    if (_recentTarget.ContainsKey(comName))
                    {
                        recentName = _recentTarget[comName];
                    }

                    if (comName.Equals("ScenarioTable"))
                    {
                        String targetName = XmlGetter.Attribute(child, "TargetName");
                        if (targetName.Length == 0)
                        {
                            if (recentName != null) targetName = recentName;
                            else throw new Exception("ScenarioTable 을 위한 TargetName이 정의되지 않았습니다.");
                        }

                        IXmlComponent com = XmlControlHandler.GetComponentByName(targetName); //같은 타입에서는 여러번 TargetName을 쓰지 않아도 인식하도록..

                        if (com != null)
                        {

                            XmlScenarioTable table = com as XmlScenarioTable;
                            if (table != null)
                            {
                                if (targetName.Length > 0)
                                {
                                    _recentTarget[comName] = targetName;//모든 속성이 맞으면 등록..
                                }
                                XmlTableMatchInfo info = new XmlTableMatchInfo(targetName, table);
                                info.LoadXml(xDoc, child);
                                _matchInfoItems.Add(info);
                                _targetComponents.Add(com);

                                String column = XmlGetter.Attribute(child, "ColumnName");
                                if(column.Length>0){
                                    if (table.ColumnNames.Contains(column))
                                    {
                                        table.Columns(column).RelativeObject["XmlMatchData"] = XmlMatchData.NowLoading;
                                    }
                                    else
                                    {
                                        throw new Exception("XmlMatchComponent: Table[" + table.Name + "]에 Column[" + column + "] 이 없습니다.");
                                    }
                                }
                                
                            }
                            else
                            {
                                throw new Exception("targetName [" + targetName + "]은 ScenarioTable 이 아닙니다.");
                            }
                            

                        }
                        else
                        {
                            throw new Exception("TargetName [" + targetName + "] 은 배치되지 않았습니다.");
                        }

                    }
                }
            }

        }
        XmlMatchFormat _format;
        public XmlMatchFormat Format { get { return _format; } }


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
