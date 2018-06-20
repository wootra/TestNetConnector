using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FormAdders;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Linq;
using System.Xml.Schema;

using XmlHandlers;
using FormAdders.EasyGridViewCollections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.IO;
using System.Windows.Forms;

using XmlDesigner.ForEvents.Conditions;

namespace XmlDesigner
{
    public class XmlScenarioRows : EasyGridRowCollection, IDictionary<String, XmlScenarioRow>, IXmlItem
    {
        XmlDocument _xDoc;
        XmlNode _xRows;
        XmlScenarioTable _table;
        Dictionary<String, XmlScenarioRow> _rowsDic = new Dictionary<string, XmlScenarioRow>();
        public XmlScenarioRows(XmlScenarioTable table, XmlDocument xDoc=null, XmlNode parentNode=null):base(table.ListView)
        {
            _table = table;
            if (xDoc != null && parentNode != null)
            {
                XmlNode node = XmlAdder.Element(xDoc, "Rows", parentNode);
                this.Interface = new XmlItemInterface(node, xDoc, this);
            }
        }


        public XmlItemTypes XmlItemType
        {
            get { return XmlItemTypes.Action; }
        }

        public Type Type
        {
            get { return typeof(XmlAction); }
        }

        public void LoadXml(String xmlFile, Boolean refLoad=false)
        {
            //DataSet dt = new DataSet();
            //dt.ReadXmlSchema(Properties.Resources.TableSchema);
            //dt.ReadXml(xmlFile); //schema를 불러와서 체크하기 위하여..
            if (XmlLayoutCollection.NowLoadingPath.Length > 0) _filePath = XmlLayoutCollection.NowLoadingPath +  XmlLayoutCollection.PathSeperator + xmlFile;
            else _filePath = xmlFile;

            XmlNode xNode = XmlGetter.RootNode(out _xDoc, _filePath, null, XmlSchemaValidation);// "./ComponentSchemas/ActionSchema.xsd", XmlSchemaValidation);
            /*
            _xDoc = new XmlDocument();
            _xDoc.PreserveWhitespace = false;
            _xDoc.Schemas = new System.Xml.Schema.XmlSchemaSet();
            XmlSchema schema = XmlSchema.Read(File.OpenRead("./ComponentSchemas/LabelSchema.xsd"), XmlScenarioTable_E_XmlSchemaValidation);
            _xDoc.Schemas.Add(schema);

            _xDoc.Load(xmlFile);

                        
            xNode = _xDoc.SelectSingleNode("//Label");
             */
            try
            {
                LoadXml(_xDoc, xNode, refLoad);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + ":" + xmlFile);
            }
        }

        public void SetXmlDocument(XmlDocument xDoc)
        {
            _xDoc = xDoc;
        }

        void XmlSchemaValidation(object sender, ValidationEventArgs e)
        {
            MessageBox.Show(e.Message);
        }

        public void LoadXml(XmlDocument xDoc, XmlNode rootNode, Boolean refLoad=false)
        {
            if (rootNode == null) return;
            XmlControlHandler.GetDefaultXmlItemAttributes(rootNode, xDoc, this);
            _xRows = rootNode;
            Clear();

            for (int i = 0; i < rootNode.ChildNodes.Count; i++)
            {
                XmlNode xRow = rootNode.ChildNodes[i];
                if (xRow.NodeType == XmlNodeType.Comment) continue;

                XmlScenarioRow row = new XmlScenarioRow(_table);
                row.LoadXml(xDoc, xRow);
                Add(row);
            }

        }

        public void SaveXml(string xmlFile)
        {
            throw new NotImplementedException();
        }

        String _filePath = "";
        public string FilePath
        {
            get { return _filePath; }
        }

        public XmlNode GetXml(XmlDocument xDoc,XmlNode parent = null)
        {
            String args = "";
           
            return null;
        }


        void IXmlItem.LoadXml(string xmlFile, bool refLoad)
        {
            LoadXml(xmlFile, refLoad);
        }
        
        public bool Contains(XmlScenarioRow item)
        {
            return base.Contains(item as EasyGridRow);
        }

        public void CopyTo(XmlScenarioRow[] array, int arrayIndex)
        {
            DataGridViewRow[] rows = new DataGridViewRow[array.Length];
            for (int i = 0; i < array.Length; i++) rows[i] = array[i];
            base.CopyTo(rows, arrayIndex);
        }

        public bool Remove(XmlScenarioRow item)
        {
            String name = item.Name;
            return base.Remove(item as EasyGridRow);
            _rowsDic.Remove(name);
        }
        /*
        List<XmlScenarioRow> rows = new List<XmlScenarioRow>();//forEnumerator..
        public IEnumerator<XmlScenarioRow> EasyGridCollection.GetEnumerator()
        {
            rows.Clear();

            for (int i = 0; i < base.Count; i++)
            {
                rows.Add(base[i] as XmlScenarioRow);
            }
            return rows.GetEnumerator();
        }
        */
        XmlItemInterface _interface;
        public XmlItemInterface Interface { get { return _interface; } set { _interface = value; } }

        public void Add(string key, XmlScenarioRow value)
        {
            value.Name = key;
            if (_rowsDic.ContainsKey(key) == false)
            {
                _rowsDic.Add(key, value);
                base.Add(value);
            }
        }

        public bool ContainsKey(string key)
        {
            return _rowsDic.Keys.Contains(key);
        }

        public ICollection<string> Keys
        {
            get { return _rowsDic.Keys; }
        }

        public bool Remove(string key)
        {
            bool success = false;
            try
            {
                XmlScenarioRow row = _rowsDic[key];
                success = _rowsDic.Remove(key);
                base.Remove(row);
            }
            catch { }
            return success;
        }

        public bool TryGetValue(string key, out XmlScenarioRow value)
        {
            return _rowsDic.TryGetValue(key, out value);
        }

        public ICollection<XmlScenarioRow> Values
        {
            get { return _rowsDic.Values; }
        }

        public XmlScenarioRow this[string key]
        {
            get
            {
                return _rowsDic[key];
            }
            set
            {
                XmlScenarioRow row = _rowsDic[key];
                int index = row.Index;
                base.Remove(row);
                value.Name = key;
                base.Insert(index, value);
                _rowsDic[key] = value;
            }
        }

        public new XmlScenarioRow this[int index]
        {
            get
            {
                return base[index] as XmlScenarioRow;
            }
            set
            {
                XmlScenarioRow row = base[index] as XmlScenarioRow;
                String key = row.Name;
                base.Remove(row);
                value.Name = key;
                base.Insert(index, value);
                _rowsDic[key] = value;
            }
        }

        public void Add(KeyValuePair<string, XmlScenarioRow> item)
        {
            item.Value.Name = item.Key;
            _rowsDic.Add(item.Key, item.Value);
            base.Add(item.Value);
        }

        public bool Contains(KeyValuePair<string, XmlScenarioRow> item)
        {
            return _rowsDic.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, XmlScenarioRow>[] array, int arrayIndex)
        {
            int count = 0;
            int index = 0;
            foreach(KeyValuePair<String, XmlScenarioRow> item in _rowsDic){
                if (count < arrayIndex) continue;
                array[index++] = item;
            }
        }

        public bool Remove(KeyValuePair<string, XmlScenarioRow> item)
        {
            if (_rowsDic.ContainsKey(item.Key))
            {
                if (_rowsDic[item.Key].Equals(item.Value)) return _rowsDic.Remove(item.Key);
            }
            return false;
        }

        public new IEnumerator<KeyValuePair<string, XmlScenarioRow>> GetEnumerator()
        {
            return _rowsDic.GetEnumerator();
        }
    }
}
