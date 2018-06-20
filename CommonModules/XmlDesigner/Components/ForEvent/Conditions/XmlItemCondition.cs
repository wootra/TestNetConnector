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
using XmlDesigner.ForEvents;

namespace XmlDesigner
{
    public class XmlItemCondition: IXmlItem, IXmlComponentCondition
    {
        IXmlItem _item;
        public XmlItemCondition(IXmlItem item)
        {
            _item = item;
        }

        XmlDocument _xDoc;
        XmlEvents _events = new XmlEvents();
        public XmlEvents Events { get { return _events; } }

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
                MessageBox.Show("XmlItemCondition.LoadXml:" + e.Message + ":" + xmlFile);
            }
        }



        void XmlSchemaValidation(object sender, ValidationEventArgs e)
        {
            MessageBox.Show("XmlItemCondition.XmlSchemaValidation:" + e.Message);
        }


        public void SaveXml(String xmlFile)
        {

        }
        /*
        string _columnName = "";
        public String ColumnName
        {
            get { return _columnName; }
        }

        string _rowName = "";
        public String RowName { get { return _rowName; } }

        String _enabled = "";
        public String Enabled { get{ return _enabled;}}
        */
        Dictionary<String, string> _items = new Dictionary<string, string>();
        public Dictionary<String, String> Items
        {
            get { return _items; }
        }

        public void LoadXml(XmlDocument xDoc, XmlNode rootNode, Boolean refLoad = false)
        {
            if (rootNode == null) return;
            XmlControlHandler.GetDefaultXmlItemAttributes(rootNode, xDoc, this);

            XmlNodeList children;
            children = XmlGetter.Children(rootNode, "Item");
            if (children != null)
            {
                foreach (XmlNode child in children)
                {
                    String name = XmlGetter.Attribute(child, "Name");
                    if (name.Length > 0) _items.Add(name, child.InnerText.Trim());
                    //if (child != null) _items = child.InnerText;
                }
            }
            /*
            child = XmlGetter.Child(rootNode, "ColumnName");
            if (child != null) _columnName = child.InnerText;

            child = XmlGetter.Child(rootNode, "RowName");
            if (child != null) _rowName = child.InnerText;
            
            child = XmlGetter.Child(rootNode, "Enabled");
            if (child != null) _enabled = child.InnerText;
            */

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

        public bool GetCondition()
        {
            bool isTrue = true;
            foreach (String key in _items.Keys)
            {

                if (_item.Interface.GetRealTimeArgs(new String[]{key})[0].ToString().Equals(_items[key]) == false) return false;

            }
            return isTrue;
            /*
            if (_columnName.Length > 0)
            {
                String colName = _item.Columns(_item.CurrentCell.ColumnIndex).Name;
                if (_columnName.Equals(colName) == false) return false;
            }

            if (_rowName.Length > 0)
            {
                String rowName = _item.Rows[_item.CurrentCell.RowIndex].Name;

                if (_rowName.Equals(rowName) == false) return false;
            }

            if (_enabled.Length > 0)
            {
                if((_enabled.ToLower().Equals("true")) != _item.CurrentCell.Enabled) return false;
            }
            return true;
             */
        }
    }
}
