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
    public class XmlControlMatchings: IXmlItem
    {
        public static XmlControlMatchings NowLoading;

        public XmlControlMatchings()
        {
            
        }

         XmlDocument _xDoc; 
        public void LoadXml(String xmlFile, Boolean refLoad = false)
        {
            if (XmlLayoutCollection.NowLoadingPath.Length > 0) _filePath = XmlLayoutCollection.NowLoadingPath + XmlLayoutCollection.PathSeperator + xmlFile;
            else _filePath = xmlFile;

            XmlNode xNode = XmlGetter.RootNode(out _xDoc,_filePath, null, XmlSchemaValidation);

            //try
            {
                LoadXml(_xDoc, xNode, refLoad);
            }
            //catch (Exception e)
            {
             //   MessageBox.Show(e.Message + ":" + xmlFile);
            }
        }



        void XmlSchemaValidation(object sender, ValidationEventArgs e)
        {
            MessageBox.Show(e.Message);
        }


        public void SaveXml(String xmlFile)
        {

        }

        public List<String> TargetTemplates{
            get{
                List<String> list = new List<string>();
                foreach(String key in _matchItems.Keys){
                    list.Add(key);
                }
                return list;
            }
        }
        
        Dictionary<String, XmlMatchItem> _matchItems = new Dictionary<string, XmlMatchItem>();
        
        public XmlMatchItem MatchItem(String targetTemplate)
        {
            if (_matchItems.ContainsKey(targetTemplate))
            {
                return _matchItems[targetTemplate];
            }
            else return null;
        }

        public void LoadXml(XmlDocument xDoc, XmlNode rootNode, Boolean refLoad = false)
        {
            if (rootNode == null) return;
            _xDoc = xDoc;  XmlControlHandler.GetDefaultXmlItemAttributes(rootNode, xDoc, this);
            NowLoading = this;

            XmlNode commonMatchItem = XmlGetter.Child(rootNode, "CommonMatches");

            XmlMatchItem commonMatches = new XmlMatchItem();
            commonMatches.LoadXml(xDoc, commonMatchItem);

            XmlNodeList matchItems = XmlGetter.Children(rootNode, "MatchItem");
            foreach (XmlNode matchItem in matchItems)
            {
                String targetTemplate = XmlGetter.Attribute(matchItem, "TargetTemplate");
                if (targetTemplate.Length == 0) throw new Exception("MatchItem Tag에 TargetTemplate 속성이 없습니다.");
                XmlMatchItem item = new XmlMatchItem();
                item.LoadXml(xDoc, matchItem);
                foreach (XmlMatchData matchData in commonMatches.GetMatchDataList(XmlMatchItem.ActiveTimes.Init))
                {
                    item.GetMatchDataList(XmlMatchItem.ActiveTimes.Init).Add(matchData);
                }
                foreach (XmlMatchData matchData in commonMatches.GetMatchDataList(XmlMatchItem.ActiveTimes.Send))
                {
                    item.GetMatchDataList(XmlMatchItem.ActiveTimes.Send).Add(matchData);
                }
                foreach (XmlMatchData matchData in commonMatches.GetMatchDataList(XmlMatchItem.ActiveTimes.Recv))
                {
                    item.GetMatchDataList(XmlMatchItem.ActiveTimes.Recv).Add(matchData);
                }
                _matchItems.Add(targetTemplate, item);
                
            }
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
