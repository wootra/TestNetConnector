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
    public class XmlMatchItem: IXmlItem
    {
        public enum ActiveTimes { Init=0, Send, Recv };
        string[] _activeTimeTexts = new string[] { "Init", "Send", "Recv" };
        public static XmlMatchItem NowLoading;

        Dictionary<ActiveTimes, List<XmlMatchData>> _activeTimeToMatchData = new Dictionary<ActiveTimes, List<XmlMatchData>>();
        Dictionary<String, XmlMatchData> _fieldToMatchData = new Dictionary<string, XmlMatchData>();
        public XmlMatchItem()
        {
            _activeTimeToMatchData[ActiveTimes.Init] = new List<XmlMatchData>();
            _activeTimeToMatchData[ActiveTimes.Send] = new List<XmlMatchData>();
            _activeTimeToMatchData[ActiveTimes.Recv] = new List<XmlMatchData>();
        }

        XmlDocument _xDoc;  
        public void LoadXml(String xmlFile, Boolean refLoad = false)
        {
            if (XmlLayoutCollection.NowLoadingPath.Length > 0) _filePath = XmlLayoutCollection.NowLoadingPath + XmlLayoutCollection.PathSeperator + xmlFile;
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

        String _targetTemplate;
        public String TargetTemplate { get { return _targetTemplate; } }

        public List<XmlMatchData> GetMatchDataList(ActiveTimes active)
        {
            return _activeTimeToMatchData[active];
        }

        public List<XmlMatchData> this[ActiveTimes active]
        {
            get { return _activeTimeToMatchData[active]; }
        }

        public XmlMatchData GetMatchData(String fieldName)
        {
            if (_fieldToMatchData.ContainsKey(fieldName)) return _fieldToMatchData[fieldName];
            return null;
        }

        public XmlMatchData this[String fieldName]
        {
            get {
                if(_fieldToMatchData.ContainsKey(fieldName)) return _fieldToMatchData[fieldName];
                return null;
            }
        }

        public List<String> FieldNames
        {
            get
            {
                return _fieldToMatchData.Keys.ToList();
            }
        }

        public void LoadXml(XmlDocument xDoc, XmlNode rootNode, Boolean refLoad = false)
        {
            if (rootNode == null) return;
            _xDoc = xDoc;  XmlControlHandler.GetDefaultXmlItemAttributes(rootNode, xDoc, this);
            NowLoading = this;

            _targetTemplate = XmlGetter.Attribute(rootNode, "TargetTemplate");

            
            foreach (XmlNode match in rootNode.ChildNodes)
            {
                String fieldName = XmlGetter.Attribute(match, "FieldName");
                if (fieldName.Length == 0) throw new Exception(rootNode.Name+"MatchItem [TargetTemplate:"+_targetTemplate+"]/Match 태그에는 FieldName이 반드시 들어가야 합니다. 형식예: Info.Name 혹은 Command.Header.id");
                
                String activeTimesText = XmlGetter.Attribute(match, "ActiveTimes");
                List<ActiveTimes> activeTimes = getActiveTimes(activeTimesText);
                if (activeTimes.Count == 0) activeTimes.Add(ActiveTimes.Init);

                XmlMatchData data = new XmlMatchData(activeTimes, fieldName);
                data.LoadXml(xDoc, match, refLoad);

                _fieldToMatchData.Add(fieldName, data); //field->match 란에 추가

                for (int i = 0; i < activeTimes.Count; i++)
                {
                    _activeTimeToMatchData[activeTimes[i]].Add(data); //activeTime->Match 란에 추가.
                }

            }
        }

        List<ActiveTimes> getActiveTimes(string activeTimesText)
        {
            String[] token = activeTimesText.Split(" ".ToCharArray(),  StringSplitOptions.RemoveEmptyEntries);
            List<ActiveTimes> list = new List<ActiveTimes>();
            for (int i = 0; i < token.Length; i++)
            {
                int index = _activeTimeTexts.ToList().IndexOf(token[i]);
                if (index >= 0) list.Add((ActiveTimes)index);
                else throw new Exception(token[i] + " is not an option for ActiveTimes");
            }
            return list;
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

        public XmlNode GetXml(XmlDocument xDoc, XmlNode parent = null)
        {
            
            return null;
        }

        XmlItemInterface _interface;
        public XmlItemInterface Interface { get { return _interface; } set { _interface = value; } }
    }
}
