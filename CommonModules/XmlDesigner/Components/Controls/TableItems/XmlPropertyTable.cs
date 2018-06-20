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
using XmlDesigner.ForEvents;

namespace XmlDesigner
{
    public class XmlPropertyTable : EasyGridView, IXmlComponent, IXmlItem
    {
        XmlNode _xRoot;
        XmlDocument _xDoc;
        String[] _itemTypes = new String[] { "TextBox", "CheckBox", "ComboBox", "CheckBoxGroup", "Button", "Image", "ImageButton", "ImageCheckBox", "CloseButton", "Header", "RadioButton", "KeyValue", "Various", "BlankBack", "FileOpenBox" };
        String[] _actions = new String[] { "Modify", "CheckBoxChecked", "ContextMenu", "Nothing", "Auto", "CommonAction", "CopyToClipBoard" };
        String[] _textAlignModes = new String[] {"Right", "Left", "NumberOnlyRight", "Center", "NumberRightTextCenter", "None" };
        String[] _imageLayouts = new String[] { "None","Tile","Center","Stretch","Zoom"};

        Dictionary<String, EasyGridRow> _rows = new Dictionary<string, EasyGridRow>();

        List<ItemTypes> _columnTypes = new List<ItemTypes>();
        
        XmlEvents _events = new XmlEvents();
        public XmlEvents Events { get { return _events; } }

        public event ValidationEventHandler E_XmlSchemaValidation;

        public XmlPropertyTable()
            : base()
        {
            
        }

        public XmlItemTypes XmlItemType { get { return XmlItemTypes.ScenarioTable; } }

        public Control RealControl { get { return this; } }

        public Type Type { get { return typeof(EasyGridView); } }

        public EasyGridRow Row(String name)
        {
            if (_rows.ContainsKey(name)) return _rows[name];
            else return null;
        }
        String _filePath = "";
        public string FilePath
        {
            get { return _filePath; }
        }

        public void LoadXml(String xmlFile, bool refLoad = false)
        {
            if (XmlLayoutCollection.NowLoadingPath.Length > 0) _filePath = XmlLayoutCollection.NowLoadingPath +  XmlLayoutCollection.PathSeperator + xmlFile;
            else _filePath = xmlFile;

            //DataSet dt = new DataSet();
            //dt.ReadXmlSchema(Properties.Resources.TableSchema);
            //dt.ReadXml(xmlFile); //schema를 불러와서 체크하기 위하여..
            /*
            XmlDocument xDoc;
            
            xDoc = new XmlDocument();
            xDoc.PreserveWhitespace = false;
            xDoc.Schemas = new System.Xml.Schema.XmlSchemaSet();


            XmlSchema schema = XmlSchema.Read(File.OpenRead("./ComponentSchemas/TableSchema.xsd"), XmlScenarioTable_E_XmlSchemaValidation);

            xDoc.Schemas.Add(schema);
            xDoc.Load(xmlFile);
            LoadXml( xDoc.SelectSingleNode("//Table") );
            */
            XmlNode root = XmlGetter.RootNode(out _xDoc, _filePath,"./ComponentSchemas/ScenarioTableSchema.xsd", XmlScenarioTable_E_XmlSchemaValidation);
            LoadXml(_xDoc, root, refLoad);
            
        }
        public void LoadXml(XmlDocument xDoc, XmlNode rootNode, bool refLoad = false)
        {
            if (rootNode == null) return;
            XmlControlHandler.GetDefaultControlAttributes(rootNode, xDoc,   this);

            XmlNode xNode = XmlGetter.Child(rootNode,"TableInfo/BaseRowHeight");

            if (xNode != null) this.BaseRowHeight = int.Parse(xNode.InnerText);
            
            #region getTableColumn

            XmlNodeList xNodeList = XmlGetter.Children(rootNode,"Columns/Column");

            for (int i = 0; i < xNodeList.Count; i++)
            {
                XmlNode colNode = xNodeList[i];
                if (colNode.NodeType == XmlNodeType.Comment) continue;

                int wid = int.Parse(colNode.Attributes["Width"].Value);
                String name = colNode.Attributes["Name"].Value;
                ItemTypes itemType = (ItemTypes)(_itemTypes.ToList().IndexOf(colNode.Attributes["ItemType"].Value));
                XmlNodeList colItems = colNode.ChildNodes;
                Actions onClickAction = Actions.Auto;
                Actions onDoubleClickAction = Actions.Auto;
                Actions onRightClickAction = Actions.Auto;

                XmlNode properties = null;
                for (int ci = 0; ci < colItems.Count; ci++)
                {
                    if (colItems[ci].NodeType == XmlNodeType.Comment) continue;

                    if (colItems[ci].Name.Equals("Actions"))
                    {
                        XmlNodeList xActions = colItems[ci].ChildNodes;
                        for (int ai = 0; ai < xActions.Count; ai++)
                        {
                            if (xActions[ai].NodeType == XmlNodeType.Comment) continue;

                            switch (xActions[ai].Name)
                            {
                                case "OnClick":
                                    onClickAction = (Actions)(_actions.ToList().IndexOf(xActions[ai].InnerText));
                                    break;
                                case "OnDoubleClick":
                                    onDoubleClickAction = (Actions)(_actions.ToList().IndexOf(xActions[ai].InnerText));
                                    break;
                                case "OnRightClick":
                                    onRightClickAction = (Actions)(_actions.ToList().IndexOf(xActions[ai].InnerText));
                                    break;
                            }
                        }
                    }
                    else
                    {
                        properties = colItems[ci];
                    }
                }

                MakeAColumn(wid, itemType, name, properties, onClickAction, onDoubleClickAction, onRightClickAction);

            }
            #endregion

            #region getRows

            xNodeList = XmlGetter.Children(rootNode,"Rows/Row");
            //GetRows(xNodeList);

            #endregion
        }

        void GetRows(XmlNodeList xRows)
        {

            for (int i = 0; i < xRows.Count; i++)//Row 들
            {
                XmlNode rowNode = xRows[i];
                String name = rowNode.Attributes["Name"].Value;
                if (rowNode.NodeType == XmlNodeType.Comment) continue;

                XmlNodeList rowChildren = xRows[i].ChildNodes;
                int rowHeight = -1;
                Dictionary<String, object> relObjs = new Dictionary<string, object>();
                object[] args = new object[_columnTypes.Count];
                String[] tooltips = new string[_columnTypes.Count];
                for (int chi = 0; chi < rowNode.ChildNodes.Count; chi++)
                {
                    XmlNode child = rowChildren[chi];
                    if (child.NodeType == XmlNodeType.Comment) continue;

                    if (child.Name.Equals("RowInfo"))
                    {
                        for (int ri = 0; ri < child.ChildNodes.Count; ri++)
                        {

                            XmlNode info = child.ChildNodes[ri];
                            if (info.NodeType == XmlNodeType.Comment) continue;

                            if (info.Name.Equals("Height"))
                            {
                                rowHeight = int.Parse(info.InnerText);
                            }
                            else if (info.Name.Equals("RelativeObjects"))
                            {
                                for (int roi = 0; roi < info.ChildNodes.Count; roi++)
                                {
                                    XmlNode obj = info.ChildNodes[roi];
                                    if (obj.NodeType == XmlNodeType.Comment) continue;

                                    relObjs[obj.Attributes["Name"].Value] = obj.Attributes["Value"].Value;
                                }
                            }
                        }
                    }
                    else if (child.Name.Equals("Cells")) //cells
                    {
                        XmlNodeList cells = child.ChildNodes;
                        if (cells.Count < _columnTypes.Count)
                        {
                            MessageBox.Show("ParsingError: Table/Rows/Row/Cells/ 아래의 Cell 태그의 수가 Column의 개수보다 적습니다. Row:" + i);
                            return;
                        }
                        int count = 0;
                        for (int ci = 0; ci < cells.Count; ci++) //Cell
                        {
                            XmlNode cell = cells[ci];
                            if (cell.NodeType == XmlNodeType.Comment) continue;
                            else if (cell.Name.Equals("Cell") == false) continue;

                            XmlAttribute attr = null;

                            if (cell.Attributes != null) attr = cell.Attributes["Tooltip"];
                            if (attr != null) tooltips[count] = attr.Value;
                            else tooltips[count] = "";

                            XmlNode firstChild = XmlGetter.FirstChild(cell);

                            if (firstChild != null && firstChild.Name.Equals("ItemInfo")) //itemInfo
                            {

                                EasyGridCellInfo info = GetCellInfo(count, firstChild.ChildNodes);
                                if (firstChild.Attributes != null)
                                {
                                    XmlAttribute xItemType = firstChild.Attributes["ItemType"];
                                    if (xItemType != null) info.ItemType = (ItemTypes)(_itemTypes.ToList().IndexOf(xItemType.Value));
                                }
                                args[count] = info;

                            }
                            else //simple value
                            {
                                if (cell.InnerText == null) args[count] = GetSimpleValue(count, "");
                                else args[count] = GetSimpleValue(count, cell.InnerText);
                            }
                            count++;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Parsing Error: /Table/Rows/Row/ 아래에는 RowInfo 나 Cells 만 올 수 있습니다.");
                        return;
                    }
                }//Row's Children

                EasyGridRow row;
                if (relObjs.Count == 0) row = AddARow(args);
                else row = AddARow(relObjs, args, tooltips);
                _rows[name] = row;
            }
        }

        EasyGridCellInfo GetCellInfo(int colIndex, XmlNodeList InfoList)
        {
            ItemTypes type = _columnTypes[colIndex];
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

                                    imgs.Add(Image.FromFile(xImgs[ii].Attributes["URL"].Value));
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
                        info.SelectedIndex = int.Parse(xInfo.Attributes["Value"].Value);
                        break;
                    case "Text":
                        info.Text = xInfo.Attributes["Value"].Value;
                        break;
                    case "ImageLayout":
                        info.ImageLayout = (ImageLayout)(_imageLayouts.ToList().IndexOf(xInfo.Attributes["Value"].Value));
                        break;
                    case "KeyValueColorCollection":
                        {
                            Dictionary<String, string> keyValue = new Dictionary<string, string>();
                            Dictionary<String, Brush> keyColor = new Dictionary<string, Brush>();
                            for (int kvi = 0; kvi < xInfo.ChildNodes.Count; kvi++)
                            {
                                XmlNode xItem = xInfo.ChildNodes[kvi]; //<Item ..
                                if (xItem.NodeType == XmlNodeType.Comment) continue;

                                String key = xItem.Attributes["Key"].Value;
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
                            XmlAttribute attr = xInfo.Attributes["Value"];
                            info.CheckInt = int.Parse(attr.Value);
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
                                if (xItems[xi].Attributes!=null && xItems[xi].Attributes["Checked"] != null && xItems[xi].Attributes["Checked"].Value.Equals("true")) selectedIndices.Add(count);
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
            ItemTypes itemType = _columnTypes[colIndex];
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
                        return int.Parse(value);
                    }
                case ItemTypes.FileOpenBox:
                    {
                        return value;
                    }
                case ItemTypes.Image:
                    {
                        int val;
                        if (int.TryParse(value, out val)) return val;
                        else
                        {
                            try
                            {
                                return Image.FromFile(value);
                            }
                            catch(Exception e)
                            {
                                MessageBox.Show(" Image Cell을 위해 [" + value + "] 를 읽을 수 없습니다.\r\n" + e.Message);
                                return -1;
                            }
                        }
                    }
                case ItemTypes.ImageButton:
                    {
                        int val;
                        if (int.TryParse(value, out val)) return val;
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
                        if (int.TryParse(value, out val)) return val;
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
                        if (int.TryParse(value, out val)) return val;
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

        void XmlScenarioTable_E_XmlSchemaValidation(object sender, ValidationEventArgs e)
        {
            MessageBox.Show(e.Message);
        }

        void MakeAColumn(int wid, ItemTypes itemType, String name, XmlNode prop, Actions ac1, Actions ac2, Actions ac3)
        {
            _columnTypes.Add(itemType);
                
            switch (itemType)
            {
                case ItemTypes.Button:
                    {
                        String headerText = "";
                        headerText = prop.Attributes["HeaderText"].Value;
                        String baseText = prop.Attributes["BaseText"].Value;
                        AddTitleButtonColumn(wid, name, headerText, baseText, ac1, ac2, ac3);
                        break;
                    }
                case ItemTypes.CheckBox:
                    {
                    bool isTriState = prop.Attributes["TriState"].Value.Equals("true");
                    int initValue = int.Parse(prop.Attributes["InitValue"].Value);
                    AddTitleCheckBoxColumn(wid, name, isTriState, ac1, ac2, ac3);
                    break;
                    }
                case ItemTypes.CheckBoxGroup:
                    {
                        String headerText = prop.Attributes["HeaderText"].Value;
                        if (prop.HasChildNodes)
                        {
                            List<String> items = new List<String>();
                            XmlNodeList xItems = prop.ChildNodes[0].ChildNodes;//ComboBoxProperties/SingleSelItems/SingleSelItem
                            List<int> selected = new List<int>();
                            for (int i = 0; i < xItems.Count; i++)
                            {
                                if (xItems[i].NodeType == XmlNodeType.Comment) continue;

                                if (xItems[i].Attributes["Checked"].Value.Equals("true")) selected.Add(i);
                                items.Add(xItems[i].InnerText);
                            }
                            if (selected.Count == 0) selected = null;
                            AddTitleCheckBoxGroupColumn(wid, name, headerText, items.ToArray(), selected.ToArray(), ac1, ac2, ac3);
                        }
                        else
                        {
                            AddTitleCheckBoxGroupColumn(wid, name, headerText, null, null, ac1, ac2, ac3);
                        }
                    }
                    break;
                case ItemTypes.CloseButton:
                    {
                        String headerText = prop.Attributes["HeaderText"].Value;
                        String baseText = prop.Attributes["BaseText"].Value;
                        AddTitleCloseButtonColumn(wid, name, headerText, baseText, ac1, ac2, ac3);
                        break;
                    }
                case ItemTypes.ComboBox:
                    {
                        String headerText = prop.Attributes["HeaderText"].Value;
                        int selIndex = int.Parse(prop.Attributes["SelectedIndex"].Value);
                        if (prop.HasChildNodes)
                        {
                            List<String> items = new List<String>();
                            XmlNodeList xItems = prop.ChildNodes[0].ChildNodes;//ComboBoxProperties/SingleSelItems/SingleSelItem
                            for (int i = 0; i < xItems.Count; i++)
                            {
                                if (xItems[i].NodeType == XmlNodeType.Comment) continue;

                                items.Add(xItems[i].InnerText);
                            }
                            AddTitleComboBoxColumn(wid, name, headerText, items.ToArray(), selIndex, ac1, ac2, ac3);
                        }
                        else
                        {
                            AddTitleComboBoxColumn(wid, name, headerText, null, -1, ac1, ac2, ac3);
                        }
                    }
                    break;
                case ItemTypes.FileOpenBox:
                    {
                        String headerText = prop.Attributes["HeaderText"].Value;
                        bool editable = prop.Attributes["Editable"].Value.Equals("true");
                       // bool autoSort = prop.Attributes["IsAutoSort"].Value.Equals("true");

                        TextAlignModes mode = (TextAlignModes)(_textAlignModes.ToList().IndexOf(prop.Attributes["TextAlignMode"].Value));

                        AddTitleFileOpenBoxColumn(wid, name, headerText, editable, mode,  ac1, ac2, ac3);
                        break;
                    }
                case ItemTypes.Image:
                    {
                        String headerText = prop.Attributes["HeaderText"].Value;
                        int initValue = int.Parse(prop.Attributes["InitValue"].Value);
                        int titleShowImage = int.Parse(prop.Attributes["TitleShowImage"].Value);

                        if (prop.HasChildNodes)
                        {
                            try
                            {
                                XmlNode xImgs = XmlGetter.Child(prop, "ImageList");// prop.SelectNodes("ImageList");
                                if (xImgs.ChildNodes.Count > 0)
                                {
                                    XmlImageList imgs = new XmlImageList("ImageList");
                                    imgs.LoadXml(_xDoc, xImgs);
                                    AddTitleImageColumn(wid, name, headerText, imgs, titleShowImage, ac1, ac2, ac3);
                                }
                                else
                                {
                                    AddTitleImageColumn(wid, name, headerText, (Image[])null, -1, ac1, ac2, ac3);
                                }
                                
                                /*
                               // List<Image> imgs = new List<Image>();
                                //XmlNodeList xImgs = prop.ChildNodes[0].ChildNodes;//ImageCheckBoxProperties/CheckImageList/
                                for (int i = 0; i < xImgs.Count; i++)
                                {
                                    if (xImgs[i].NodeType == XmlNodeType.Comment) continue;

                                    imgs.Add(Image.FromFile(xImgs[i].Attributes["URL"].Value));
                                }
                                AddTitleImageColumn(wid, name, headerText, imgs.ToArray(), titleShowImage, ac1, ac2, ac3);
                                 */
                            }
                            catch
                            {
                                AddTitleImageColumn(wid, name, headerText, (Image[])null, -1, ac1, ac2, ac3);
                            }
                        }
                        else
                        {
                            AddTitleImageColumn(wid, name, headerText, (Image[])null, -1, ac1, ac2, ac3);
                        }
                        break;
                    }
                case ItemTypes.ImageButton:
                    {
                        String headerText = prop.Attributes["HeaderText"].Value;
                        //int initValue = int.Parse(prop.Attributes["InitValue"].Value);
                        int titleShowImage = int.Parse(prop.Attributes["TitleShowImage"].Value);
                        bool showTitleText = (prop.Attributes["ShowTitleText"].Value.Equals("true"));
                        bool useColumnTextForButtonValue = (prop.Attributes["UseColumnTextForButtonValue"].Value.Equals("true"));
                        if (prop.HasChildNodes)
                        {
                            XmlNode xImgs = XmlGetter.Child(prop,"ImageList");// prop.SelectNodes("ImageList");
                            if (xImgs.ChildNodes.Count > 0)
                            {
                                XmlImageList imgs = new XmlImageList("ImageList");
                                try
                                {
                                    imgs.LoadXml(_xDoc, xImgs);
                                    AddTitleImageButtonColumn(wid, name, headerText, imgs, titleShowImage, useColumnTextForButtonValue, showTitleText, ac1, ac2, ac3);
                                }
                                catch (Exception e)
                                {
                                    MessageBox.Show(e.Message);
                                    AddTitleImageButtonColumn(wid, name, headerText, (Image[])null, titleShowImage, useColumnTextForButtonValue, showTitleText, ac1, ac2, ac3);
                                }
                                
                            }
                            else
                            {
                                AddTitleImageButtonColumn(wid, name, headerText, (Image[])null, titleShowImage, useColumnTextForButtonValue, showTitleText, ac1, ac2, ac3);
                            }
                            /*
                            try
                            {
                                List<Image> imgs = new List<Image>();
                                XmlNodeList xImgs = prop.ChildNodes[0].ChildNodes;//ImageCheckBoxProperties/CheckImageList/
                                for (int i = 0; i < xImgs.Count; i++)
                                {
                                    if (xImgs[i].NodeType == XmlNodeType.Comment) continue;

                                    imgs.Add(Image.FromFile(xImgs[i].Attributes["URL"].Value));
                                }
                                AddTitleImageButtonColumn(wid, name, headerText, imgs.ToArray(), titleShowImage, useColumnTextForButtonValue, showTitleText, ac1, ac2, ac3);
                            }
                            catch
                            {
                                AddTitleImageButtonColumn(wid, name, headerText, (Image[])null, titleShowImage, useColumnTextForButtonValue, showTitleText, ac1, ac2, ac3);
                            }
                             */
                        }
                        else
                        {
                            AddTitleImageButtonColumn(wid, name, headerText, (Image[])null, titleShowImage, useColumnTextForButtonValue, showTitleText, ac1, ac2, ac3);
                        }
                        break;
                    }
                case ItemTypes.ImageCheckBox:
                    {
                        String headerText = prop.Attributes["HeaderText"].Value;
                        //bool isTriState = prop.Attributes["TriState"].Value.Equals("true");
                        int initValue = int.Parse(prop.Attributes["InitValue"].Value);

                        if (prop.HasChildNodes)
                        {
                            XmlNode xImgs = XmlGetter.Child(prop, "CheckImageList");// prop.SelectNodes("CheckImageList");
                            if (xImgs.ChildNodes.Count > 0)
                            {
                                XmlCheckImageList imgs = new XmlCheckImageList("CheckImageList");
                                try
                                {
                                    imgs.LoadXml(_xDoc, xImgs);
                                    AddTitleImageCheckColumn(wid, name, headerText, imgs, ac1, ac2, ac3);
                                }
                                catch(Exception e)
                                {
                                    MessageBox.Show("XmlLogTable: MakeAColumn() : "+e.Message);
                                    AddTitleImageCheckColumn(wid, name, headerText, CheckBoxColors.Red, ac1, ac2, ac3);
                                }
                            }
                            else
                            {
                                AddTitleImageCheckColumn(wid, name, headerText, CheckBoxColors.Red, ac1, ac2, ac3);
                            }
                            /*
                            List<Image> imgs = new List<Image>();
                            XmlNodeList xImgs = prop.ChildNodes[0].ChildNodes;//ImageCheckBoxProperties/CheckImageList/
                            try
                            {
                                for (int i = 0; i < xImgs.Count; i++)
                                {
                                    if (xImgs[i].NodeType == XmlNodeType.Comment) continue;

                                    imgs.Add(Image.FromFile(xImgs[i].Attributes["URL"].Value));
                                }
                                AddTitleImageCheckColumn(wid, name, headerText, imgs, ac1, ac2, ac3);
                            }
                            catch (Exception e)
                            {
                                AddTitleImageCheckColumn(wid, name, headerText, CheckBoxColors.Red, ac1, ac2, ac3);
                            }
                             */
                        }
                        else
                        {
                            XmlAttribute attr = prop.Attributes["CheckColor"];
                            if (attr == null) AddTitleImageCheckColumn(wid, name, headerText, CheckBoxColors.Red, ac1, ac2, ac3);
                            else if (attr.Value.Equals("Blue")) AddTitleImageCheckColumn(wid, name, headerText, CheckBoxColors.Blue, ac1, ac2, ac3);
                            else AddTitleImageCheckColumn(wid, name, headerText, CheckBoxColors.Red, ac1, ac2, ac3);
                        }

                        break;
                    }
                case ItemTypes.KeyValue:
                    {
                        String headerText = prop.Attributes["HeaderText"].Value;
                        TextAlignModes mode = (TextAlignModes)(_textAlignModes.ToList().IndexOf(prop.Attributes["TextAlignMode"].Value));
                        AddTitleKeyValueColumn(wid, name, headerText, mode , ac1, ac2, ac3);
                        break;
                    }
                case ItemTypes.RadioButton:
                    {
                        String headerText = prop.Attributes["HeaderText"].Value;
                        int selIndex = int.Parse(prop.Attributes["SelectedIndex"].Value);
                        if (prop.HasChildNodes)
                        {
                            List<String> items = new List<String>();
                            XmlNodeList xItems = prop.ChildNodes[0].ChildNodes;//ComboBoxProperties/SingleSelItems/SingleSelItem
                            for (int i = 0; i < xItems.Count; i++)
                            {
                                if (xItems[i].NodeType == XmlNodeType.Comment) continue;

                                items.Add(xItems[i].InnerText);
                            }
                            AddTitleRadioButtonColumn(wid, name, headerText, items.ToArray(), selIndex, ac1, ac2, ac3);
                        }
                        else
                        {
                            AddTitleRadioButtonColumn(wid, name, headerText, null, -1, ac1, ac2, ac3);
                        }
                    }
                    break;
                case ItemTypes.TextBox:
                    {
                        String headerText = prop.Attributes["HeaderText"].Value;
                        bool editable = prop.Attributes["Editable"].Value.Equals("true");
                        bool autoSort = prop.Attributes["IsAutoSort"].Value.Equals("true");

                        //String initValue = prop.Attributes["InitValue"].Value;
                        TextAlignModes mode = (TextAlignModes)(_textAlignModes.ToList().IndexOf(prop.Attributes["TextAlignMode"].Value));
                        EasyGridTextBoxColumn col = AddTitleTextBoxColumn(wid, name, headerText, editable, autoSort, mode, ac1, ac2, ac3);
                        
                        break;
                    }
                case ItemTypes.Various:
                    {
                        String headerText = prop.Attributes["HeaderText"].Value;
                        
                        AddTitleVariousColumn(wid, name, headerText,ac1, ac2, ac3);
                        break;
                    }
            }
        }

        public void SaveXml(String xmlFile)
        {

        }


        public XmlNode GetXml(XmlDocument xDoc, XmlNode parent = null)
        {
            XmlNode xTableNode = XmlAdder.Element(xDoc, "Table", parent);
            
            XmlNode xTableInfo = XmlAdder.Element(xDoc, "TableInfo", xTableNode);
            XmlNode xBaseRowHeight = XmlAdder.Element(xDoc, "BaseRowHeight", this.BaseRowHeight.ToString(), xTableInfo);

            XmlNode xColumns = XmlAdder.Element(xDoc, "Columns", xTableNode);
            for (int colIndex = 0; colIndex < this.ColumnCount; colIndex++)
            {
                XmlNode xColumn = XmlAdder.Element(xDoc, "Column", xColumns);

                IEasyGridColumn col = Columns(colIndex);
                ItemTypes itemType = col.ItemType;

                switch (itemType)
                {
                    case ItemTypes.Button:
                        {
                            EasyGridButtonColumn c = col as EasyGridButtonColumn;
                            XmlNode xProperties = XmlAdder.Element(xDoc, itemType.ToString() + "Properties", xColumn);
                            XmlAdder.Attribute(xDoc, "HeaderText", c.HeaderText, xProperties);
                            XmlAdder.Attribute(xDoc, "BaseText", c.Text, xProperties);
                        }
                        break;
                    case ItemTypes.CheckBox:
                        {
                            EasyGridCheckBoxColumn c = col as EasyGridCheckBoxColumn;
                            XmlNode xProperties = XmlAdder.Element(xDoc, itemType.ToString() + "Properties", xColumn);
                            XmlAdder.Attribute(xDoc, "TriState", c.ThreeState ? "true" : "false", xProperties);
                        }
                        break;
                    case ItemTypes.CheckBoxGroup:
                        {
                            EasyGridCheckBoxGroupColumn c = col as EasyGridCheckBoxGroupColumn;
                            XmlNode xProperties = XmlAdder.Element(xDoc, itemType.ToString() + "Properties", xColumn);
                            XmlAdder.Attribute(xDoc, "HeaderText", c.HeaderText, xProperties);
                            if (c.Items.Count > 0)
                            {
                                XmlNode multiSelItems = XmlAdder.Element(xDoc, "MultiSelItems", xProperties);
                                for (int i = 0; i < c.Items.Count; i++)
                                {
                                    XmlNode multiSelItem = XmlAdder.Element(xDoc, "MultiSelItem", c.Items[i].Text, multiSelItems);
                                    XmlAdder.Attribute(xDoc, "Checked", c.Items[i].Checked ? "true" : "false", multiSelItem);
                                }
                            }
                        }
                        break;
                    case ItemTypes.CloseButton:
                        {
                            EasyGridCloseButtonColumn c = col as EasyGridCloseButtonColumn;
                            XmlNode xProperties = XmlAdder.Element(xDoc, itemType.ToString() + "Properties", xColumn);
                            XmlAdder.Attribute(xDoc, "HeaderText", c.HeaderText, xProperties);
                            XmlAdder.Attribute(xDoc, "BaseText", c.Text, xProperties);
                        }
                        break;
                    case ItemTypes.ComboBox:
                        {
                            EasyGridComboBoxColumn c = col as EasyGridComboBoxColumn;
                            XmlNode xProperties = XmlAdder.Element(xDoc, itemType.ToString() + "Properties", xColumn);
                            XmlAdder.Attribute(xDoc, "HeaderText", c.HeaderText, xProperties);
                            if (c.Items.Count > 0)
                            {
                                XmlNode singleSelItems = XmlAdder.Element(xDoc, "SingleSelItems", xProperties);
                                for (int i = 0; i < c.Items.Count; i++)
                                {
                                    XmlAdder.Element(xDoc, "SingleSelItem", c.Items.ElementAt(i), singleSelItems);
                                }
                            }
                        }
                        break;
                    case ItemTypes.FileOpenBox:
                        {
                            EasyGridFileOpenBoxColumn c = col as EasyGridFileOpenBoxColumn;
                            
                            XmlNode xProperties = XmlAdder.Element(xDoc, itemType.ToString() + "Properties", xColumn);
                            XmlAdder.Attribute(xDoc, "HeaderText", c.HeaderText, xProperties);
                            XmlAdder.Attribute(xDoc, "Editable", c.IsEditable?"true":"false", xProperties);
                            XmlAdder.Attribute(xDoc, "TextAlignMode", c.ColumnTextAlignMode.ToString(), xProperties);
                        }
                        break;
                    case ItemTypes.Image:
                        {
                            EasyGridImageColumn c = col as EasyGridImageColumn;
                            XmlNode xProperties = XmlAdder.Element(xDoc, itemType.ToString() + "Properties", xColumn);
                            XmlAdder.Attribute(xDoc, "HeaderText", c.HeaderText, xProperties);
                            XmlAdder.Attribute(xDoc, "TitleShowImage", c.SelectedIndex.ToString(), xProperties);
                            XmlImageList imgs = c.Images as XmlImageList;
                            if(imgs!=null && imgs.Count>0){
                                imgs.GetXml(xDoc, xProperties);
                            }
                        }
                        break;
                    case ItemTypes.ImageButton:
                        {
                            EasyGridImageButtonColumn c = col as EasyGridImageButtonColumn;
                            XmlNode xProperties = XmlAdder.Element(xDoc, itemType.ToString() + "Properties", xColumn);
                            XmlAdder.Attribute(xDoc, "HeaderText", c.HeaderText, xProperties);
                            XmlAdder.Attribute(xDoc, "TitleShowImage", c.SelectedIndex.ToString(), xProperties);
                            XmlAdder.Attribute(xDoc, "ShowTitleText", c.HeaderText.Length>0 ? "true" : "false", xProperties);
                            XmlAdder.Attribute(xDoc, "UseColumnTextForButtonValue", c.UseColumnTextForButtonValue.ToString(), xProperties);
                            XmlImageList imgs = c.Images as XmlImageList;
                            if (imgs != null && imgs.Count > 0)
                            {
                                imgs.GetXml(xDoc, xProperties);
                            }
                        }
                        break;
                    case ItemTypes.ImageCheckBox:
                        {
                            EasyGridImageCheckBoxColumn c = col as EasyGridImageCheckBoxColumn;
                            XmlNode xProperties = XmlAdder.Element(xDoc, itemType.ToString() + "Properties", xColumn);
                            XmlAdder.Attribute(xDoc, "HeaderText", c.HeaderText, xProperties);
                            XmlAdder.Attribute(xDoc, "InitValue", c.SelectedIndex.ToString(), xProperties);
                            
                            XmlCheckImageList imgs = c.Images as XmlCheckImageList;
                            if (imgs != null && imgs.Count > 0)
                            {
                                imgs.GetXml(xDoc, xProperties);
                            }else{
                                if (c.Images.Count > 1)
                                {
                                    if (c.Images.ElementAt(1).Equals(FormAdders.Properties.Resources.check_red))
                                    {
                                        XmlAdder.Attribute(xDoc, "CheckColor", CheckBoxColors.Red.ToString(), xProperties);
                                    }
                                    else
                                    {
                                        XmlAdder.Attribute(xDoc, "CheckColor", CheckBoxColors.Blue.ToString(), xProperties);
                                    }
                                }
                                else
                                {
                                    XmlAdder.Attribute(xDoc, "CheckColor", CheckBoxColors.Red.ToString(), xProperties);
                                
                                }
                            }
                        }
                        break;
                    case ItemTypes.KeyValue:
                        {
                            EasyGridKeyValueColumn c = col as EasyGridKeyValueColumn;
                            XmlNode xProperties = XmlAdder.Element(xDoc, itemType.ToString() + "Properties", xColumn);
                            XmlAdder.Attribute(xDoc, "HeaderText", c.HeaderText, xProperties);
                            XmlAdder.Attribute(xDoc, "TextAlignMode", c.ColumnTextAlignMode.ToString(), xProperties);
                            
                        }
                        break;
                    case ItemTypes.RadioButton:
                        {
                            EasyGridRadioButtonColumn c = col as EasyGridRadioButtonColumn;
                            XmlNode xProperties = XmlAdder.Element(xDoc, itemType.ToString() + "Properties", xColumn);
                            XmlAdder.Attribute(xDoc, "HeaderText", c.HeaderText, xProperties);
                            if (c.Items.Count > 0)
                            {
                                XmlNode singleSelItems = XmlAdder.Element(xDoc, "SingleSelItems", xProperties);
                                for (int i = 0; i < c.Items.Count; i++)
                                {
                                    XmlAdder.Element(xDoc, "SingleSelItem", c.Items[i].Text, singleSelItems);
                                }
                            }
                        }
                        break;
                    case ItemTypes.TextBox:
                        {
                            EasyGridTextBoxColumn c = col as EasyGridTextBoxColumn;
                            XmlNode xProperties = XmlAdder.Element(xDoc, itemType.ToString() + "Properties", xColumn);
                            XmlAdder.Attribute(xDoc, "HeaderText", c.HeaderText, xProperties);
                            XmlAdder.Attribute(xDoc, "TextAlignMode", c.ColumnTextAlignMode.ToString(), xProperties);
                            XmlAdder.Attribute(xDoc, "Editable", c.IsEditable ? "true" : "false", xProperties);
                            XmlAdder.Attribute(xDoc, "IsAutoSort", c.SortMode == DataGridViewColumnSortMode.Automatic ? "true" : "false", xProperties);
                        }
                        break;
                    case ItemTypes.Various:
                        {
                            EasyGridVariousColumn c = col as EasyGridVariousColumn;
                            XmlNode xProperties = XmlAdder.Element(xDoc, itemType.ToString() + "Properties", xColumn);
                            XmlAdder.Attribute(xDoc, "HeaderText", c.HeaderText, xProperties);
                        }
                        break;
                }
            }


            return xTableNode;
        }
        XmlItemInterface _interface;
        public XmlItemInterface Interface { get { return _interface; } set { _interface = value; } }
    }
}
