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
using FormAdders.EasyGridViewCollections;

namespace TestNgineData.PacketDatas
{
    public class XmlTableMatchInfo: IXmlMatchInfo
    {
        
        public XmlTableMatchInfo(String targetName, IXmlComponent component)
        {
            _targetName = targetName;
            _component = component;
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


        public void LoadXml(XmlDocument xDoc, XmlNode rootNode, Boolean refLoad = false)
        {
            if (rootNode == null) return;
            _xDoc = xDoc;  XmlControlHandler.GetDefaultXmlItemAttributes(rootNode, xDoc, this);

            _columnName = XmlGetter.Attribute(rootNode, "ColumnName");
            if (_columnName.Length == 0) throw new Exception("ScenarioTable 태그에 ColumnName이 없습니다.");

            


        }

        string _columnName;
        public string ColumnName { get { return _columnName; } }


        ItemTypes _itemType = ItemTypes.TextBox;
        public ItemTypes ItemType { get { return _itemType; } }

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

        string _targetName;
        public string TargetName
        {
            get { return _targetName; }
        }

        IXmlComponent _component;
        public IXmlComponent Component
        {
            get { return _component; }
        }


        public object GetValue(params object[] args)
        {
            XmlScenarioTable table = _component as XmlScenarioTable;
            int rowIndex = (int)args[0];
            int colIndex = table.Columns(_columnName).Index;
            return table.Cell(rowIndex, colIndex).Value;
        }


        public void SetValue(params object[] args)
        {
            XmlScenarioTable table = _component as XmlScenarioTable;
            
            int rowIndex = XmlScenarioTable.ActiveRowIndex[table];// (int)args[0];
            int colIndex = table.Columns(_columnName).Index;

            if (args.Length > 0) table.Cell(rowIndex, colIndex).Value = args[0];
            else throw new Exception("XmlTableMatchInfo.SetValue(value) 에 value가 없습니다.");
        }
    }
}
