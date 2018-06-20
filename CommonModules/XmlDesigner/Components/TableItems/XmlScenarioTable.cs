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
using XmlDesigner.Parsers;
using XmlDesigner.PacketDatas;

namespace XmlDesigner
{
    public class XmlScenarioTable : EasyGridView, IXmlComponent, IXmlItem
    {
        XmlDocument _xDoc;
        XmlNode _xRoot;
        public static String[] ItemTypesText = new String[] { "TextBox", "CheckBox", "ComboBox", "CheckBoxGroup", "Button", "Image", "ImageButton", "ImageCheckBox", "CloseButton", "Header", "RadioButton", "KeyValue", "Various", "BlankBack", "FileOpenBox" };
        public static String[] ActionsText = new String[] { "Modify", "CheckBoxChecked", "ContextMenu", "Nothing", "Auto", "CommonAction", "CopyToClipBoard" };
        public static String[] TextAlignModesText = new String[] { "Right", "Left", "NumberOnlyRight", "Center", "NumberRightTextCenter", "None" };
        public static String[] ImageLayoutsText = new String[] { "None", "Tile", "Center", "Stretch", "Zoom" };
        
        
        public static Dictionary<XmlScenarioTable, int> ActiveRowIndex = new Dictionary<XmlScenarioTable, int>();
        public static XmlScenarioTable ActiveTable;
        

        //Dictionary<String, EasyGridRow> _rows = new Dictionary<string, EasyGridRow>();

        List<ItemTypes> _columnTypes = new List<ItemTypes>();
        
        XmlEvents _events = new XmlEvents();
        public XmlEvents Events { get { return _events; } }

        public event ValidationEventHandler E_XmlSchemaValidation;

        XmlScenarioRows _rows;
        public new XmlScenarioRows Rows { get { return _rows; } }

        public XmlScenarioTable()
            : base()
        {
            _rows = new XmlScenarioRows(this);
            this.E_CellClicked += new CellClickEventHandler(XmlScenarioTable_E_CellClicked);
            this.E_TextChanged += new CellTextChangedEventHandler(XmlScenarioTable_E_TextChanged);
            this.E_CheckBoxChanged += new CellCheckedEventHandler(XmlScenarioTable_E_CheckBoxChanged);
            this.E_CheckBoxGroupSelected += new CellCheckBoxGroupSelectedEventHandler(XmlScenarioTable_E_CheckBoxGroupSelected);
            this.E_CellRightClicked += new CellClickEventHandler(XmlScenarioTable_E_CellRightClicked);
            this.E_CellDoubleClicked += new CellClickEventHandler(XmlScenarioTable_E_CellDoubleClicked);
            this.E_ContextMenuClicked += new EasyGridMenuClickHandler(XmlScenarioTable_E_ContextMenuClicked);
        }
        public object Args(String argName)
        {
            if (_currentCell == null) return null;
            switch (argName)
            {
                case "ColumnIndex":
                    return _currentCell.ColumnIndex;
                case "RowIndex":
                    return _currentCell.RowIndex;
                case "CurrentCell":
                    return _currentCell;
                case "CellValue":
                    return _currentCell.Value;
                case "Text":
                    return _textChangedArgs.Text;
                case "BeforeText":
                    return _textChangedArgs.BeforeText;
                case "TextChangedArgs":
                    return _textChangedArgs;
                case "StartRowIndex":
                    return _checkboxChangedArg.StartRowIndex;
                case "EndRowIndex":
                    return _checkboxChangedArg.EndRowIndex;
                case "IsChecked":
                    return _checkboxChangedArg.Checked;
                case "Packet.Command":
                    {
                        XmlPacket packet = Row(_currentCell.RowIndex).RelativeObject["XmlPacket"] as XmlPacket;
                        return packet.Command;
                    }
                case "Packet.Response":
                    {
                        XmlPacket packet = Row(_currentCell.RowIndex).RelativeObject["XmlPacket"] as XmlPacket;
                        return packet.Response;
                    }
                case "Packet":
                    {
                        XmlPacket packet = Row(_currentCell.RowIndex).RelativeObject["XmlPacket"] as XmlPacket;
                        return packet;
                    }
                default:
                    return null;
            }

        }

        string GetComponentValue(string path)
        {
            switch (path)
            {
                case "Column.Name":
                    return Columns(_currentCell.ColumnIndex).Name;
                case "Row.Name":
                    return Rows[_currentCell.RowIndex].Name;
                case "Cell.Value":
                    return _currentCell.Value.ToString();
                default:
                    return null;
            }
        }

        EasyGridMenuClickArgs _menuClickedArgs;
        void XmlScenarioTable_E_ContextMenuClicked(object sender, EasyGridMenuClickArgs e)
        {
            _menuClickedArgs = e;
            _currentCell = Cell(e.RowIndex, e.ColIndex) as IEasyGridCell;

            XmlControlHandler.RunEvent(this, "OnMenuClicked");
            
            
        }

        void XmlScenarioTable_E_CellDoubleClicked(object sender, CellClickEventArgs e)
        {
            _currentCell = Cell(e.RowIndex, e.ColIndex) as IEasyGridCell;
            if (_currentCell != null) XmlControlHandler.RunEvent(this, "OnCellDoubleClicked");
        }

        void XmlScenarioTable_E_CellRightClicked(object sender, CellClickEventArgs e)
        {
            _currentCell = Cell(e.RowIndex, e.ColIndex) as IEasyGridCell;
            if (_currentCell != null) XmlControlHandler.RunEvent(this, "OnCellRightClicked");
        }

        CellCheckBoxGroupSelectedEventArgs _checkGroupChangedArg;
        void XmlScenarioTable_E_CheckBoxGroupSelected(object sender, CellCheckBoxGroupSelectedEventArgs e)
        {
            _currentCell = Cell(e.RowIndex, e.ColIndex) as IEasyGridCell;
            _checkGroupChangedArg = e;
            if (_currentCell != null) XmlControlHandler.RunEvent(this, "OnCellClicked");
        }

        CellCheckedEventArgs _checkboxChangedArg;
        void XmlScenarioTable_E_CheckBoxChanged(object sender, CellCheckedEventArgs e)
        {
            _currentCell = Cell(e.EndRowIndex, e.ColumnIndex) as IEasyGridCell;
            _checkboxChangedArg = e;
            if (_currentCell != null) XmlControlHandler.RunEvent(this, "OnCheckBoxChanged");
        }

        CellTextChangedEventArgs _textChangedArgs;
        void XmlScenarioTable_E_TextChanged(object sender, CellTextChangedEventArgs e)
        {
            _currentCell = Cell(e.RowIndex, e.ColIndex) as IEasyGridCell;
            _textChangedArgs = e;
            if (_currentCell != null) XmlControlHandler.RunEvent(this, "OnTextChanged");
        }

        public IEasyGridCell CurrentCell { get { return _currentCell; } }
        
        IEasyGridCell _currentCell;

        void XmlScenarioTable_E_CellClicked(object sender, CellClickEventArgs e)
        {
            _currentCell = Cell(e.RowIndex, e.ColIndex) as IEasyGridCell;
            if(_currentCell!=null) XmlControlHandler.RunEvent(this, "OnCellClicked");
        }

        public XmlItemTypes XmlItemType { get { return XmlItemTypes.ScenarioTable; } }

        public Control RealControl { get { return this; } }

        public Type Type { get { return typeof(EasyGridView); } }

        public XmlScenarioRow AddARowByName(String name, Dictionary<String, object> values, Dictionary<String, String> tooltips=null)
        {
            XmlScenarioRow row = new XmlScenarioRow(this, Interface.Document, Interface.Node);
            row.MakeCells(values, tooltips);
            Rows.Add(name, row);
            return row;
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
            XmlControlHandler.GetDefaultControlAttributes(rootNode, xDoc,   this, refLoad, Args, GetComponentValue);
            
            XmlNode xNode = XmlGetter.Child(rootNode,"TableInfo/BaseRowHeight");

            if (xNode != null) this.BaseRowHeight = int.Parse(xNode.InnerText);
            
            #region getTableColumn

            XmlNodeList xNodeList = XmlGetter.Children(rootNode,"Columns/Column");

            for (int i = 0; i < xNodeList.Count; i++)
            {
                XmlNode colNode = xNodeList[i];
                if (colNode.NodeType == XmlNodeType.Comment) continue;

                int wid = int.Parse(XmlGetter.Attribute(colNode, "Width"));
                String name = XmlGetter.Attribute(colNode, "Name");
                ItemTypes itemType = (ItemTypes)(ItemTypesText.ToList().IndexOf(XmlGetter.Attribute(colNode, "ItemType")));
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
                                    onClickAction = (Actions)(ActionsText.ToList().IndexOf(xActions[ai].InnerText));
                                    break;
                                case "OnDoubleClick":
                                    onDoubleClickAction = (Actions)(ActionsText.ToList().IndexOf(xActions[ai].InnerText));
                                    break;
                                case "OnRightClick":
                                    onRightClickAction = (Actions)(ActionsText.ToList().IndexOf(xActions[ai].InnerText));
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
            
            XmlNode xRows = XmlGetter.Child(rootNode, "Rows");

            if (xRows != null && xRows.ChildNodes.Count > 0)
            {
                _rows = new XmlScenarioRows(this);
                Rows.LoadXml(xDoc, xRows);
            }
            else
            {
                _rows = new XmlScenarioRows(this, xDoc, rootNode);
            }
            //xNodeList = XmlGetter.Children(rootNode,"Rows/Row");
            
            #endregion
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
                        headerText = XmlGetter.Attribute(prop,"HeaderText");
                        String baseText = XmlGetter.Attribute(prop,"BaseText");
                        AddTitleButtonColumn(wid, name, headerText, baseText, ac1, ac2, ac3);
                        break;
                    }
                case ItemTypes.CheckBox:
                    {
                    bool isTriState = ValueParser.IsTrue( XmlGetter.Attribute(prop,"TriState"));
                    //string initVal = XmlGetter.Attribute(prop, "InitValue");
                    //int initValue = (initVal.Length > 0) ? int.Parse(initVal) : 0;
                    AddTitleCheckBoxColumn(wid, name, isTriState, ac1, ac2, ac3);
                    break;
                    }
                case ItemTypes.CheckBoxGroup:
                    {
                        String headerText = XmlGetter.Attribute(prop,"HeaderText");
                        if (prop.HasChildNodes)
                        {
                            List<String> items = new List<String>();
                            XmlNodeList xItems = prop.ChildNodes[0].ChildNodes;//ComboBoxProperties/SingleSelItems/SingleSelItem
                            List<int> selected = new List<int>();
                            for (int i = 0; i < xItems.Count; i++)
                            {
                                if (xItems[i].NodeType == XmlNodeType.Comment) continue;

                                if (ValueParser.IsTrue(XmlGetter.Attribute(xItems[i],"Checked"))) selected.Add(i);
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
                        String headerText = XmlGetter.Attribute(prop,"HeaderText");
                        String baseText = XmlGetter.Attribute(prop,"BaseText");
                        AddTitleCloseButtonColumn(wid, name, headerText, baseText, ac1, ac2, ac3);
                        break;
                    }
                case ItemTypes.ComboBox:
                    {
                        String headerText = XmlGetter.Attribute(prop,"HeaderText");
                        String selIndexText = XmlGetter.Attribute(prop, "SelectedIndex");
                        int selIndex = (selIndexText.Length > 0) ? int.Parse(selIndexText) : -1;
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
                        String headerText = XmlGetter.Attribute(prop,"HeaderText");
                        bool editable = XmlGetter.Attribute(prop,"Editable").Equals("true");
                       // bool autoSort = XmlGetter.Attribute("IsAutoSort").Equals("true");

                        TextAlignModes mode = (TextAlignModes)(TextAlignModesText.ToList().IndexOf(XmlGetter.Attribute(prop,"TextAlignMode")));

                        AddTitleFileOpenBoxColumn(wid, name, headerText, editable, mode,  ac1, ac2, ac3);
                        break;
                    }
                case ItemTypes.Image:
                    {
                        String headerText = XmlGetter.Attribute(prop,"HeaderText");
                        //int initValue = int.Parse(XmlGetter.Attribute(prop,"InitValue"));
                        int titleShowImage = int.Parse(XmlGetter.Attribute(prop,"TitleShowImage"));

                        if (prop.HasChildNodes)
                        {
                            try
                            {
                                XmlNode xImgs = XmlGetter.Child(prop, "ImageList");
                                if (xImgs!=null && xImgs.ChildNodes.Count > 0)
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

                                    imgs.Add(Image.FromFile(xImgs[i], "URL")));
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
                        String headerText = XmlGetter.Attribute(prop,"HeaderText");
                        //int initValue = int.Parse(XmlGetter.Attribute("InitValue"));
                        int titleShowImage = int.Parse(XmlGetter.Attribute(prop,"TitleShowImage"));
                        bool showTitleText = (XmlGetter.Attribute(prop,"ShowTitleText").Equals("true"));
                        bool useColumnTextForButtonValue = (XmlGetter.Attribute(prop,"UseColumnTextForButtonValue").Equals("true"));
                        if (prop.HasChildNodes)
                        {
                            XmlNode xImgs = XmlGetter.Child(prop, "ImageList");//( prop.SelectNodes("ImageList");
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

                                    imgs.Add(Image.FromFile(xImgs[i], "URL")));
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
                        String headerText = XmlGetter.Attribute(prop,"HeaderText");
                        //bool isTriState = XmlGetter.Attribute("TriState").Equals("true");
                        //int initValue = int.Parse(XmlGetter.Attribute(prop,"InitValue"));

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
                                    MessageBox.Show("XmlScenarioTable.MakeColumn:"+e.Message);
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

                                    imgs.Add(Image.FromFile(xImgs[i], "URL")));
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
                        String headerText = XmlGetter.Attribute(prop, "HeaderText");
                        TextAlignModes mode = (TextAlignModes)(TextAlignModesText.ToList().IndexOf(XmlGetter.Attribute(prop, "TextAlignMode")));
                        AddTitleKeyValueColumn(wid, name, headerText, mode , ac1, ac2, ac3);
                        break;
                    }
                case ItemTypes.RadioButton:
                    {
                        String headerText = XmlGetter.Attribute(prop, "HeaderText");
                        String selIndexText = XmlGetter.Attribute(prop, "SelectedIndex");
                        int selIndex = (selIndexText.Length>0)? int.Parse(selIndexText) : -1;
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
                        String headerText = XmlGetter.Attribute(prop, "HeaderText");
                        bool editable = XmlGetter.Attribute(prop, "Editable").Equals("true");
                        bool autoSort = XmlGetter.Attribute(prop, "IsAutoSort").Equals("true");

                        //String initValue = XmlGetter.Attribute(prop, "InitValue");
                        TextAlignModes mode = (TextAlignModes)(TextAlignModesText.ToList().IndexOf(XmlGetter.Attribute(prop, "TextAlignMode")));
                        EasyGridTextBoxColumn col = AddTitleTextBoxColumn(wid, name, headerText, editable, autoSort, mode, ac1, ac2, ac3);
                        
                        break;
                    }
                case ItemTypes.Various:
                    {
                        String headerText = XmlGetter.Attribute(prop, "HeaderText");
                        
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

            Rows.GetXml(xDoc, xTableNode);

            return xTableNode;
        }
        XmlItemInterface _interface;
        public XmlItemInterface Interface { get { return _interface; } set { _interface = value; } }
    }
}
