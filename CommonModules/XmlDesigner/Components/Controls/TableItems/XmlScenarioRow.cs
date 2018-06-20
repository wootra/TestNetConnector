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
using DataHandling;
using XmlDesigner.ForEvents.Conditions;

namespace XmlDesigner
{
    public class XmlScenarioRow : EasyGridRow, IXmlItem
    {
        XmlScenarioTable _table;
        XmlDocument _xDoc;
        public XmlScenarioRow(XmlScenarioTable table, XmlDocument xDoc=null, XmlNode parentNode=null):base(table)
        {
            _table = table;
            if (xDoc != null && parentNode != null)
            {
                XmlNode node = XmlAdder.Element(xDoc, "Row", parentNode);
                this.Interface = new XmlItemInterface(node, xDoc, this);
            }
        }

        public String Name
        {
            get
            {
                if (this.RelativeObject.ContainsKey("name") == false) return null;
                else return this.RelativeObject["name"].ToString();
            }
            set
            {
                this.RelativeObject["name"] = value;
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
                MessageBox.Show("XmlScenarioRow.LoadXml():"+e.Message + ":" + xmlFile);
            }
        }



        void XmlSchemaValidation(object sender, ValidationEventArgs e)
        {
            MessageBox.Show(e.Message);
        }

        public void LoadXml(XmlDocument xDoc, XmlNode rootNode, Boolean refLoad = false)
        {
            if (rootNode == null) return;
            XmlControlHandler.GetDefaultXmlItemAttributes(rootNode, xDoc, this);

            String name = XmlGetter.Attribute(rootNode, "Name");
            //if (rowNode.NodeType == XmlNodeType.Comment) continue;

            XmlNodeList rowChildren = rootNode.ChildNodes;
            //Dictionary<String, object> relObjs = new Dictionary<string, object>();

            for (int chi = 0; chi < rowChildren.Count; chi++)
            {
                XmlNode rowNode = rowChildren[chi];
                //if (child.NodeType == XmlNodeType.Comment) continue;

                if (rowNode.Name.Equals("RowInfo"))
                {
                    for (int ri = 0; ri < rowNode.ChildNodes.Count; ri++)
                    {

                        XmlNode info = rowNode.ChildNodes[ri];
                        if (info.NodeType == XmlNodeType.Comment) continue;

                        if (info.Name.Equals("Height"))
                        {
                            if (info.InnerText != null && info.InnerText.Length > 0)
                            {
                                this.Height = int.Parse(info.InnerText);
                            }
                        }
                        else if (info.Name.Equals("RelativeObjects"))
                        {
                            for (int roi = 0; roi < info.ChildNodes.Count; roi++)
                            {
                                XmlNode obj = info.ChildNodes[roi];
                                if (obj.NodeType == XmlNodeType.Comment) continue;

                                this.RelativeObject[XmlGetter.Attribute(obj, "Name")] = XmlGetter.Attribute(obj, "Value");
                            }
                        }
                    }
                }
                else if (rowNode.Name.Equals("Cells")) //cells
                {
                    XmlNodeList cells = rowNode.ChildNodes;
                    if (cells.Count < _table.ColumnCount)
                    {
                        throw new Exception("ParsingError: Table/Rows/Row/Cells/ 아래의 Cell 태그의 수가 Column의 개수보다 적습니다. Row:" + name);

                    }
                    object[] args = new object[_table.ColumnCount];
                    String[] tooltips = new string[_table.ColumnCount];

                    int count = 0;
                    for (int ci = 0; ci < cells.Count; ci++) //Cell
                    {
                        XmlNode cell = cells[ci];
                        if (cell.NodeType == XmlNodeType.Comment) continue;
                        else if (cell.Name.Equals("Cell") == false) continue;

                        //string value = XmlGetter.Attribute(cell, "Value");
                        string tooltip = XmlGetter.Attribute(cell, "Tooptip");

                        if (tooltip.Length > 0) tooltips[count] = tooltip;

                        string value = XmlGetter.Attribute(cell, "Value");
                        if (value.Length > 0)
                        {
                            //if (value == null) args[count] = GetSimpleValue(count, "");
                            //else 
                            args[count] = GetSimpleValue(count, value);
                        }
                        else
                        {

                            XmlNode firstChild = XmlGetter.FirstChild(cell);

                            if (firstChild != null && firstChild.Name.Equals("ItemInfo")) //itemInfo
                            {

                                EasyGridCellInfo info = GetCellInfo(count, firstChild.ChildNodes);
                                if (firstChild.Attributes != null)
                                {
                                    XmlAttribute xItemType = firstChild.Attributes["ItemType"];
                                    if (xItemType != null) info.ItemType = (ItemTypes)(XmlScenarioTable.ItemTypesText.ToList().IndexOf(xItemType.Value));
                                }
                                args[count] = info;

                            }
                            else //simple value
                            {
                                if (value == null) args[count] = GetSimpleValue(count, "");//빈 셀일때 default값을 받아온다.
                                else args[count] = GetSimpleValue(count, cell.InnerText);
                            }
                        }
                        count++;
                    }
                    this.MakeCells(args, tooltips);
                }
                else if (rowNode.Name.Equals("ChosenCells")) //cells
                {
                    XmlNodeList cells = rowNode.ChildNodes;


                    ListDic<String, String> ttps = new ListDic<string, string>();
                    ListDic<String, object> argDic = new ListDic<string, object>();
                    /*
                    for (int i = 0; i < _table.ColumnCount; i++)
                    {
                        argDic[_table.ColumnName(i)] = GetSimpleValue(i, "");
                        ttps[_table.ColumnName(i)] = "";
                    }
                    */
                    for (int ci = 0; ci < cells.Count; ci++) //Cell
                    {
                        XmlNode cell = cells[ci];
                        String colName = XmlGetter.Attribute(cell, "ColumnName");

                        if (cell.NodeType == XmlNodeType.Comment) continue;
                        else if (cell.Name.Equals("Cell") == false) continue; //반드시 Cell Tag여야 한다.
                        else if (colName.Length == 0) continue;//반드시 columnName이 있어야 한다.


                        string value = XmlGetter.Attribute(cell, "Value");
                        string tooltip = XmlGetter.Attribute(cell, "Tooptip");

                        ttps[colName] = tooltip;
                        if (value.Length > 0)
                        {
                            argDic[colName] = value;
                        }
                        else
                        {
                            XmlNode itemInfo = XmlGetter.FirstChild(cell);

                            if (itemInfo != null && itemInfo.Name.Equals("ItemInfo")) //itemInfo
                            {
                                String itemTypeText = XmlGetter.Attribute(itemInfo, "ItemType");
                                ItemTypes itemType = (ItemTypes)(XmlScenarioTable.ItemTypesText.ToList().IndexOf(itemTypeText));

                                EasyGridCellInfo info = GetCellInfo(itemType, itemInfo.ChildNodes);
                                argDic[colName] = info;
                            }
                            else
                            {
                                throw new Exception("Cell의 내부 Tag로서 유효한 것은 ItemInfo밖에 없습니다. ChosenCell의 값은 반드시 있어야합니다.");
                            }

                        }
                    }
                    this.MakeCells(argDic, ttps);

                }
                else
                {
                    MessageBox.Show("Parsing Error: /Table/Rows/Row/ 아래에는 RowInfo 나 Cells 또는 ChosenCells 만 올 수 있습니다.");
                    return;
                }
            }

        }

        EasyGridCellInfo GetCellInfo(int colIndex, XmlNodeList InfoList)
        {
            ItemTypes type = _table.ColumnItemType(colIndex);// _columnTypes[colIndex];
            return GetCellInfo(type, InfoList);
        }

        EasyGridCellInfo GetCellInfo(ItemTypes type, XmlNodeList InfoList)
        {
            EasyGridCellInfo info = new EasyGridCellInfo();
            info.ItemType = type;
            for (int i = 0; i < InfoList.Count; i++)
            {
                XmlNode xInfo = InfoList[i];
                if (xInfo.NodeType == XmlNodeType.Comment) continue;
                switch (xInfo.Name)
                {
                    case "ImageList":
                        {
                            try
                            {
                                List<Image> imgs = new List<Image>();
                                XmlNodeList xImgs = xInfo.ChildNodes;
                                for (int ii = 0; ii < xImgs.Count; ii++)
                                {
                                    if (xImgs[ii].NodeType == XmlNodeType.Comment) continue;

                                    imgs.Add(Image.FromFile(XmlGetter.Attribute(xImgs[ii],"URL")));
                                }
                                info.Images = imgs;
                            }
                            catch
                            {
                                info.Images = null;
                                //실패시 image를 넣지 않는다.
                            }
                            break;
                        }
                    case "SelectedIndex":
                        info.SelectedIndex = int.Parse(XmlGetter.Attribute(xInfo, "Value"));
                        break;
                    case "Text":
                        info.Text = XmlGetter.Attribute(xInfo, "Value");
                        break;
                    case "ImageLayout":
                        info.ImageLayout = (ImageLayout)(XmlScenarioTable.ImageLayoutsText.ToList().IndexOf(XmlGetter.Attribute(xInfo, "Value")));
                        break;
                    case "KeyValueColorCollection":
                        {
                            Dictionary<String, string> keyValue = new Dictionary<string, string>();
                            Dictionary<String, Brush> keyColor = new Dictionary<string, Brush>();
                            for (int kvi = 0; kvi < xInfo.ChildNodes.Count; kvi++)
                            {
                                XmlNode xItem = xInfo.ChildNodes[kvi]; //<Item ..
                                if (xItem.NodeType == XmlNodeType.Comment) continue;

                                String key = XmlGetter.Attribute(xItem, "Key");
                                XmlAttribute value = null;
                                if(xItem.Attributes!=null ) value = xItem.Attributes["Value"];

                                XmlAttribute color = null;
                                if(xItem.Attributes!=null) color = xItem.Attributes["Color"];
                                if (value != null) keyValue.Add(key, value.Value);
                                else keyValue.Add(key, "");

                                if (color != null)
                                {
                                    String colorString = color.Value;

                                    if (colorString.Contains('#'))
                                    {
                                        try
                                        {
                                            int rr = int.Parse(colorString.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
                                            int gg = int.Parse(colorString.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
                                            int bb = int.Parse(colorString.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
                                            keyColor.Add(key, new SolidBrush(Color.FromArgb(rr, gg, bb)));
                                        }
                                        catch
                                        {
                                            MessageBox.Show("색깔정의를 Parsing할 수 없습니다 : " + colorString + "\r\n 옳은 예> Red 또는 #FF0000");
                                        }
                                    }
                                    else
                                    {
                                        try
                                        {
                                            Color col = Color.FromName(colorString);
                                            keyColor.Add(key, new SolidBrush(col));
                                        }
                                        catch
                                        {
                                            MessageBox.Show("색깔정의를 Parsing할 수 없습니다 : " + colorString + "\r\n 옳은 예> Red 또는 #FF0000");

                                        }
                                    }
                                }
                                else
                                {
                                    //추가하지 않는다.
                                }
                            }
                            info.KeyValue = keyValue;
                            info.KeyColor = keyColor;
                            break;
                        }
                    case "Checked":
                        {
                            XmlAttribute attr = null;
                            if(xInfo.Attributes!=null) attr = xInfo.Attributes["Value"];
                            info.Checked = (attr == null) ? (bool?)null : (attr.Value.Equals("true"));
                            break;
                        }
                    case "CheckInt":
                        {
                            String val = XmlGetter.Attribute(xInfo, "Value");
                            if (val.Length > 0) info.CheckInt = int.Parse(val);
                            else info.CheckInt = 0;
                            //XmlAttribute attr = xInfo.Attributes["Value"];
                            
                            break;
                        }
                    case "MultiSelItems":
                        {
                            List<String> items = new List<String>();
                            List<int> selectedIndices = new List<int>();
                            XmlNodeList xItems = xInfo.ChildNodes;//ComboBoxProperties/SingleSelItems/SingleSelItem
                            int count = 0;
                            for (int xi = 0; xi < xItems.Count; xi++)
                            {
                                if (xItems[xi].NodeType == XmlNodeType.Comment) continue;
                                items.Add(xItems[xi].InnerText);
                                if (xItems[xi].Attributes!=null && xItems[xi].Attributes["Checked"] != null && XmlGetter.Attribute(xItems[xi], "Checked").Equals("true")) selectedIndices.Add(count);
                                count++;
                            }
                            info.Items = items;
                            if (selectedIndices.Count > 0) info.SelectedIndices = selectedIndices;
                            break;
                        }
                    case "SingleSelItems":
                        {
                            List<String> items = new List<String>();
                            XmlNodeList xItems = xInfo.ChildNodes;//ComboBoxProperties/SingleSelItems/SingleSelItem
                            for (int xi = 0; xi < xItems.Count; xi++)
                            {
                                if (xItems[xi].NodeType == XmlNodeType.Comment) continue;
                                items.Add(xItems[xi].InnerText);
                            }
                            info.Items = items;
                            break;
                        }
                }
            }
            return info;
        }

        object GetSimpleValue(int colIndex, String value)
        {
            ItemTypes itemType = _table.ColumnItemType(colIndex);// _columnTypes[colIndex];
            switch (itemType)
            {
                case ItemTypes.Button:
                    {
                        return value;
                    }
                case ItemTypes.CheckBox:
                    {
                        int val;
                        if (int.TryParse(value, out val)) return val;
                        else if (value.Equals("true")) return 1;
                        else return 0;
                    }
                case ItemTypes.CheckBoxGroup:
                    {
                        String[] values = value.Split(" ,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        List<int> selectedIndices = new List<int>();
                        for (int i = 0; i < values.Length; i++)
                        {
                            if (values[i].Equals("1")) selectedIndices.Add(i);
                        }
                        return selectedIndices;
                    }
                case ItemTypes.CloseButton:
                    {
                        return value;
                    }
                case ItemTypes.ComboBox:
                    {
                        if (value.Length == 0) return -1;
                        else return int.Parse(value);
                    }
                case ItemTypes.FileOpenBox:
                    {
                        return value;
                    }
                case ItemTypes.Image:
                    {
                        int val;
                        if (value.Length == 0) return -1;
                        else if (int.TryParse(value, out val)) return val;
                        else
                        {
                            try
                            {
                                return Image.FromFile(value);
                            }
                            catch(Exception e)
                            {
                                MessageBox.Show("XmlScenarioRow:GetSimpleValue() : Image Cell을 위해 [" + value + "] 를 읽을 수 없습니다.\r\n" + e.Message);
                                return -1;
                            }
                        }
                    }
                case ItemTypes.ImageButton:
                    {
                        int val;
                        if (value.Length == 0) return -1;
                        else if (int.TryParse(value, out val)) return val;
                        else
                        {
                            try
                            {
                                return Image.FromFile(value);
                            }
                            catch (Exception e)
                            {
                                return value;//이미지가 아니면 글자로 표시
                            }
                        }
                    }
                case ItemTypes.ImageCheckBox:
                    {
                        int val;
                        if (value.Length == 0) return 0;
                        else if (int.TryParse(value, out val)) return val;
                        else
                        {
                            MessageBox.Show("ImageCheckBox Cell을 위한 argument가 아닙니다. :" + value);
                        }

                        break;
                    }
                case ItemTypes.KeyValue:
                    {
                        if (value.Length == 0) return new Dictionary<String, object>();
                        else MessageBox.Show("Column이 KeyValue형식인 Cell은 <KeyValueColorCollection><Item Key= Value= Color=/></KeyValueColorCollection>로 정의해야 합니다. :"+value);
                        break;
                    }
                case ItemTypes.RadioButton:
                    {
                        int val;
                        if (value.Length == 0) return -1;
                        else if (int.TryParse(value, out val)) return val;
                        else
                        {
                            return value;
                        }
                    }
                case ItemTypes.TextBox:
                    {
                        return value;
                    }
                case ItemTypes.Various:
                    {
                        MessageBox.Show("Column이 Various형식인 Cell은 각각의 고유 타입으로 정의해야 합니다.");
                        break;
                    }
            }
            return null;
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

        XmlItemInterface _interface;
        public XmlItemInterface Interface { get { return _interface; } set { _interface = value; } }
    }
}
