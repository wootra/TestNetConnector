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
using DataHandling;
using XmlDesigner.ForEvents;

namespace XmlDesigner.PacketDatas
{
    public class XmlScenario : IXmlItem, IScenarioItem
    {
        XmlScenarioInfo _info = new XmlScenarioInfo();
        List<IScenarioItem> _itemList = new List<IScenarioItem>();
        public static String NowLoadingPath = "";
        public static String PathSeperator = "/";
        public static String NowLoadingFile = "";
        public static XmlScenario ActiveScenario;
        public Stack<XmlDocument> UndoStack = new Stack<XmlDocument>(10);
        public Stack<XmlDocument> RedoStack = new Stack<XmlDocument>(10);

        public XmlScenario()
        {
            ActiveScenario = this;
        }

        public XmlScenario(String name, String ScenarioPath)
        {
            _filePath = ScenarioPath + "\\" + name + ".xml";
            _baseDir = ScenarioPath;
            _name = name;
            if (File.Exists(_filePath))
            {
                throw new Exception("Scenario " + name + " already exists in the directory[" + ScenarioPath + "]");
            }
            this.Interface.RegisterEvent(Events.OnScenarioListChanged.ToString(), EventHandlers.RefreshScenarioList.ToString(), "@XmlScenario");
        }
        
        public ScenarioItemTypes ScenarioItemType { get { return ScenarioItemTypes.Scenario; } }

        String _baseDir="";
        public String BaseDir
        {
            get { return _baseDir; }
        }

        public String FilePath
        {
            get { return _filePath; }
        }

        String _name = "";
        public String Name
        {
            get{
                return _name;
                //string fileName = _filePath.Substring(_filePath.LastIndexOf("\\")+1);
                //return fileName.Substring(0, fileName.LastIndexOf("."));
            }

            set
            {
                //string fileDir = _filePath.Substring(0,_filePath.LastIndexOf("\\"));
                //string fileName = _filePath.Substring(_filePath.LastIndexOf("\\")+1);
                String newPath = _baseDir+"\\"+value+".xml";
                if (File.Exists(newPath)) throw new Exception("" + value + " exists already..");

                File.Move(_filePath, newPath);
                _filePath = newPath;
                _name = value;
                //_filePath = _baseDir + "\\" + _name + ".xml";
            }
        }

        public IScenarioItem GetItem(String name)
        {
            foreach (IScenarioItem item in _itemList)
            {
                if (item.Name.Equals(name)) return item;
            }
            return null;
        }

        XmlDocument _xDoc; 

        public void LoadXml(String xmlFile, Boolean refLoad = false)
        {
            XmlLayoutCollection.PathSeperator = "\\";

            if (xmlFile.Equals(_filePath))
            {
                throw new Exception("같은 시나리오를 재귀적으로 Ref했습니다. 시나리오파일명:" + _filePath);
            }
            _filePath = xmlFile.Replace("/", "\\");
            
            string fileDir = _filePath.Substring(0, _filePath.LastIndexOf("\\"));
            string fileName = _filePath.Substring(_filePath.LastIndexOf("\\") + 1);
            _name = fileName.Substring(0, fileName.LastIndexOf(".")); //xml을 뺌..

            _baseDir = fileDir;

            NowLoadingFile = xmlFile;
            //_filePath = _filePath.Replace("\\", "/").Replace(Directory.GetCurrentDirectory().Replace("\\", "/"), "");

            XmlNode rootNode = XmlGetter.RootNode(out _xDoc,_filePath, ".\\ComponentSchemas\\LayoutCollectionSchema.xsd", XmlSchemaValidation);
            //try
            {
                LoadXml(_xDoc, rootNode);
            }
            //catch (Exception e)
            {
                //    throw new Exception(e.Message + ":" + xmlFile);
            }
        }

        public void Undo()
        {
            if (UndoStack.Count > 0)
            {
                //SaveXml();//현재상태 저장

                //XmlDocument doc = UndoStack.Pop();
                RedoStack.Push(_xDoc);//현재상태는 일단 Redo stack으로..
            
                XmlDocument doc = UndoStack.Pop();
                _xDoc = doc;//현재상태 갱신..
                XmlNode rootNode = XmlGetter.FirstChild(doc);// XmlGetter.RootNode(out _xDoc, _filePath, ".\\ComponentSchemas\\LayoutCollectionSchema.xsd", XmlSchemaValidation);
                //try
                {
                    LoadXml(_xDoc, rootNode);
                }
                this.Interface.RunEvent(
            }
        }
        public void Redo()
        {
            if (RedoStack.Count > 0)
            {

            }
        }

        public XmlScenarioInfo Info
        {
            get { return _info; }
        }

        /// <summary>
        /// AllPacket은 시나리오가 있을 시 시나리오의 내용까지 모두포함하는 패킷입니다.
        /// 읽기전용이고, 수정해도 내부의 값이 변하지 않습니다.
        /// </summary>
        public List<IScenarioItem> AllPackets
        {
            get
            {
                List<IScenarioItem> list = new List<IScenarioItem>();
                for (int i = 0; i < _itemList.Count; i++)
                {
                    if (_itemList[i] is XmlScenario)
                    {
                        List<IScenarioItem> childList = (_itemList[i] as XmlScenario).AllPackets;
                        for (int c = 0; c < childList.Count; c++)
                        {
                            
                            list.Add(childList[c]);
                        }
                    }
                    else if (_itemList[i] is XmlCommand)
                    {
                        list.Add(_itemList[i] as XmlCommand);
                    }
                }
                return list;
            }
        }

        public XmlScenario getSubScenario(String name)
        {
            for (int i = 0; i < _itemList.Count; i++)
            {
                if (_itemList[i] is XmlScenario)
                {
                    if (_itemList[i].Name.Equals(name)) return _itemList[i] as XmlScenario;
                }
                
            }
            return null;
        }

        void XmlSchemaValidation(object sender, ValidationEventArgs e)
        {
            MessageBox.Show(e.Message);
        }


        public void SaveXml(String xmlFile=null)
        {
            UndoStack.Push(_xDoc);
            XmlDocument xDoc = new XmlDocument();
            _xDoc = xDoc;
            XmlNode root = GetXml(xDoc, null);
            //xDoc.AppendChild(root);
            this.Interface = new XmlItemInterface(root, xDoc, this);

            if (xmlFile == null)
            {
                if (_filePath != null)
                {
                    try
                    {
                        xDoc.Save(_filePath);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);

                    }
                }
                else
                {

                }
            }
            else
            {
                _filePath = xmlFile;
                NowLoadingFile = xmlFile;
                xDoc.Save(xmlFile);
            }
        }

        public bool ContainsName(String name)
        {
            foreach (IScenarioItem item in _itemList)
            {
                if (item.Name.ToLower().Equals(name.ToLower())) return true;
            }
            return false;
        }

        //ListDic<String, XmlCommand> _packetList = new ListDic<string, XmlCommand>();
        //public ListDic<String, XmlCommand> PacketList { get { return _packetList; } }

        public void LoadXml(XmlDocument xDoc, XmlNode rootNode, Boolean refLoad = false)
        {
            if (rootNode == null) return;
            XmlControlHandler.GetDefaultXmlItemAttributes(rootNode, xDoc, this);

            XmlNode info = XmlGetter.Child(rootNode, Properties.Resources.Scenario_InfoNodeName);
            _info.LoadXml(_xDoc, info);
            
            if(refLoad==false) _itemList.Clear();//초기화
            
            XmlNode xScenItemList = XmlGetter.Child(rootNode, Properties.Resources.Scenario_CommandListNodeName);
            int packetCount = 0;
            int scenCount = 0;
            foreach (XmlNode xScenItem in xScenItemList.ChildNodes)
            {

                if (xScenItem.Name.Equals(Properties.Resources.Scenario_ItemName_ScenarioNodeName))//"Scenario"))
                {
                    scenCount++;
                    XmlScenario scen = new XmlScenario();
                    scen.LoadXml(_xDoc, xScenItem);
                    _itemList.Add(scen);
                }
                else if (xScenItem.Name.Equals(Properties.Resources.Scenario_ItemName_CommandGroupNodeName))//"CommandGroup"))
                {
                    packetCount++;
                    String name = XmlGetter.Attribute(xScenItem, "Name");
                    if (name.Length == 0) throw new Exception("시나리오파일 [" + NowLoadingFile + "]," + packetCount + "번째 Packet 태그에 Name속성이 없습니다.");

                    String template = XmlGetter.Attribute(xScenItem, "Template");
                    if (template.Contains("."))
                    {
                        template = template.Substring(template.LastIndexOf(".")+1);//.의 가장 맨 뒤의 이름만 가져옴..
                    }
                    if (template.Length == 0) throw new Exception("시나리오파일 [" + NowLoadingFile + "]," + packetCount + "번째 Packet 태그(Name=" + name + ")에 Template속성이 없습니다.");

                    XmlTemplate xTemplate = TemplateInfos.This.GetTemplate(template);//._totalTemplates[template];

                    XmlCommand packet = new XmlCommand(name, xTemplate, this);
                    packet.LoadXml(_xDoc, xScenItem);
                    _itemList.Add(packet);
                }
                else if (xScenItem.Name.Equals("Command"))
                {
                    packetCount++;
                    String name = XmlGetter.Attribute(xScenItem, "Name");
                    if (name.Length == 0) throw new Exception("시나리오파일 [" + NowLoadingFile + "]," + packetCount + "번째 Packet 태그에 Name속성이 없습니다.");

                    String template = XmlGetter.Attribute(xScenItem, "Template");
                    if (template.Length == 0) throw new Exception("시나리오파일 [" + NowLoadingFile + "]," + packetCount + "번째 Packet 태그(Name=" + name + ")에 Template속성이 없습니다.");
                    if (template.Contains("."))
                    {
                        template = template.Substring(template.LastIndexOf(".") + 1);
                    }
                    XmlTemplate xTemplate;
                    if ((xTemplate = TemplateInfos.This.GetTemplate(template))!=null)
                    {
                        
                        XmlCommand packet = new XmlCommand(name, xTemplate, this);
                        try
                        {
                            packet.LoadXml(_xDoc, xScenItem);
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Error on Loading Command [" + packet.Name + "]\r\n"+e.Message);
                        }
                        _itemList.Add(packet);
                    }
                    else
                    {
                        throw new Exception("Template " + template + " doesn't exist which should be in Scenario File[" + _filePath + "]");
                    }
                    
                }
            }

        }

        String _filePath = "";


        public Type Type
        {
            get { return typeof(XmlScenario); }
        }

        public XmlNode GetXml(XmlDocument xDoc, XmlNode parent = null)
        {
            XmlNode root = XmlHandlers.XmlAdder.Element(xDoc, "Scenario");

            XmlNode infoNode = _info.GetXml(xDoc, root);

            XmlElement commandListNode = XmlHandlers.XmlAdder.Element(xDoc, "CommandList", root);

            foreach (XmlCommand command in _itemList)
            {
                command.GetXml(xDoc, commandListNode);
            }
            return root;
        }
        /*
        public void AddCommand(XmlCommand command)
        {
            _itemList.Add(command);
            //XmlNode commandList = XmlGetter.Child(Interface.Node, "CommandList");
            //commandList.AppendChild(command.GetXml(Interface.Document, commandList));
        }
        */

        public List<IScenarioItem> Items
        {
            get { return _itemList; }
        }
        



        public void RemoveElement(IScenarioItem command)
        {
            _itemList.Remove(command);
        }

        XmlItemInterface _interface;
        public XmlItemInterface Interface { get { return _interface; } set { _interface = value; } }
    }
}
