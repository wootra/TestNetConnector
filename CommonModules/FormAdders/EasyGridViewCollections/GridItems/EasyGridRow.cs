using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FormAdders; using DataHandling;
using System.Drawing;
using DataHandling;

namespace FormAdders.EasyGridViewCollections
{
    public class EasyGridRow : DataGridViewRow
    {
        CustomDictionary<String, object> _relativeObject = new CustomDictionary<string, object>();
        public CustomDictionary<String, object> RelativeObject { get { return _relativeObject; } }
        public RowSpan Span;
        public EasyGridView _view;
        
        public EasyGridRow(EasyGridView view)
            : base()
        {
            base.Resizable = DataGridViewTriState.NotSet;
            _view = view;
            Span = new RowSpan(this);
        }


        bool _isSetting = false;
        public override bool Selected
        {
            get
            {
                return base.Selected;
            }
            set
            {
                if (_isSetting == false)
                {
                    _isSetting = true;
                    base.Selected = value;
                    _isSetting = false;
                }
            }
        }

        bool _enabled = true;
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                foreach (IEasyGridCell cell in Cells)
                {
                    cell.Enabled = value;
                }
            }
        }

        public int SetRowHeight()
        {
            int max = 0;
            foreach (DataGridViewCell cell in Cells)
            {
                EasyGridTextBoxCell textCell = cell as EasyGridTextBoxCell;
                if (textCell != null)
                {
                    int height = _view.BaseRowHeight;// textCell.GetCellHeight();
                    if (max < height) max = height;
                }
            }
            if (max > 0 && this.Height!=max)
            {
                this.Height = max;
                DataGridView.UpdateRowHeightInfo(this.Index, true);
                //DataGridView.Update();
            }
            return max;
        }

        public void MakeCells(ICollection<object> values, ICollection<String> tooltips = null)
        {
            for (int grp = 0; grp < values.Count; grp++)
            {
                
                if (values.ElementAt(grp) is Array)
                {
                    Array arr = values.ElementAt(grp) as Array;
                    if (tooltips == null) AddItemInRow(this, values.ElementAt(grp));
                    else
                    {
                        if (tooltips.Count > 0) AddItemInRow(this, values.ElementAt(grp), tooltips.ElementAt(grp));
                    }

                }
                else if (values.ElementAt(grp) is ICollection<object>)
                {
                    ICollection<object> list = values.ElementAt(grp) as ICollection<object>;
                    if (tooltips == null) AddItemInRow(this, values.ElementAt(grp));
                    else
                    {
                        if (tooltips.Count > 0) AddItemInRow(this, values.ElementAt(grp), tooltips.ElementAt(grp));
                    }

                }
                else if (values.ElementAt(grp) is EasyGridCellInfo)
                {
                    EasyGridCellInfo info = values.ElementAt(grp) as EasyGridCellInfo;
                    if (tooltips == null){
                        IEasyGridCell cell = SetVariousCell(info);
                        this.Cells.Add(cell as DataGridViewCell);
                    }
                    else{
                        IEasyGridCell cell = SetVariousCell(info, tooltips.ElementAt(grp));
                        this.Cells.Add(cell as DataGridViewCell);
                        //AddItemInRow(this, values.ElementAt(grp), tooltips.ElementAt(grp));
                    }
                }
                else //obj
                {
                    if (tooltips == null) AddItemInRow(this, values.ElementAt(grp));
                    else AddItemInRow(this, values.ElementAt(grp), tooltips.ElementAt(grp));
                }
            }
            
        }

        public void MakeCells(IDictionary<String,object> values, IDictionary<String,String> tooltips = null)
        {
            //List<object> args = new List<object>();
            //List<String> tooltip = new List<string>();
            for (int i = 0; i < _view.ListView.Columns.Count; i++)
            {
                String colName = _view.ListView.Columns[i].Name;
                if (values!=null && values.Keys.Contains(colName))
                {
                    if(tooltips!=null && tooltips.Keys.Contains(colName)) AddItemInRow(this, values[colName], tooltips[colName]);
                    else AddItemInRow(this, values[colName], null);
                }
                else
                {
                    if (tooltips!=null && tooltips.Keys.Contains(colName)) AddItemInRow(this, null, tooltips[colName]);
                    else AddItemInRow(this, null, null);
                }
            }
        }

        /// <summary>
        /// 대상 row에 cell을 추가합니다. 주로 새로이 row를 만들어 추가할 때 사용됩니다.
        /// </summary>
        /// <param name="row">대상이 되는 DataGridViewRow</param>
        /// <param name="value">넣어 줄 값</param>
        public void AddItemInRow(DataGridViewRow row, object value, String tooltip = null)
        {
            int colIndex = row.Cells.Count;
            ItemTypes itemType = (_view.ListView.Columns[colIndex] as IEasyGridColumn).ItemType;// _itemTypes[colIndex];
            IEasyGridCell cell;
            #region TextBox
            if (itemType == ItemTypes.TextBox)
            {
                cell = new EasyGridTextBoxCell(_view.ListView as EasyGridViewParent);
                EasyGridTextBoxCell myCell = cell as EasyGridTextBoxCell;
                EasyGridTextBoxColumn col = _view.ListView.Columns[colIndex] as EasyGridTextBoxColumn;
                if (value == null)
                {
                    myCell.Value = "";
                }
                else
                {
                    if ((value is EasyGridCellInfo) == false)
                    {
                        myCell.IsEditable = col.IsEditable;
                        if (col.CellTextAlignMode == TextAlignModes.None)
                        {
                            myCell.TextAlignMode = _view.TextAlignMode;
                            
                        }
                        else
                        {
                            myCell.TextAlignMode = col.CellTextAlignMode;
                        }

                        if (col.TextViewMode == TextViewModes.Default)
                        {
                            myCell.TextViewMode = _view.TextViewMode;
                        }
                        else
                        {
                            myCell.TextViewMode = col.TextViewMode;
                        }
                    }

                    (cell as EasyGridTextBoxCell).Value = value.ToString();
                }
            }
            #endregion
            #region FileOPen
            else if (itemType == ItemTypes.FileOpenBox)
            {
                cell = new EasyGridFileOpenBoxCell(_view.ListView as EasyGridViewParent);
                EasyGridFileOpenBoxCell myCell = cell as EasyGridFileOpenBoxCell;
                EasyGridFileOpenBoxColumn col = _view.ListView.Columns[colIndex] as EasyGridFileOpenBoxColumn;
                if (value == null)
                {
                    myCell.Value = "";
                }
                else
                {
                    if ((value is EasyGridCellInfo) == false)
                    {
                        myCell.IsEditable = col.IsEditable;
                        myCell.TextAlignMode = col.CellTextAlignMode;
                    }
                    (cell as EasyGridFileOpenBoxCell).Value = value.ToString();
                }
            }
            #endregion
            #region KeyValue
            else if (itemType == ItemTypes.KeyValue)
            {
                cell = new EasyGridKeyValueCell(_view.ListView);
                EasyGridKeyValueCell myCell = cell as EasyGridKeyValueCell;
                if (value == null)
                {
                    myCell.Value = "";
                }
                else
                {
                    myCell.Value = value;
                }
            }
            #endregion
            #region CheckBox
            else if (itemType == ItemTypes.CheckBox)
            {
                cell = new EasyGridCheckBoxCell(_view.ListView);
                EasyGridCheckBoxCell myCell = cell as EasyGridCheckBoxCell;
                if (value == null)
                {
                    myCell.Value = false;
                }
                else
                {
                    myCell.Value = value;
                }
            }
            #endregion
            #region ImageCheckBox
            else if (itemType == ItemTypes.ImageCheckBox)
            {

                cell = new EasyGridImageCheckBoxCell();
                EasyGridImageCheckBoxCell myCell = cell as EasyGridImageCheckBoxCell;

                EasyGridImageColumn col = _view.ListView.Columns[colIndex] as EasyGridImageColumn;
                if (col != null && col.Images != null && col.Images.Count > 0)
                {
                    ICollection<Image> imageList = col.Images;// _titleInitData[colIndex] as ICollection<Image>;
                    if (imageList.Count != 0) myCell.Images = imageList;
                }
                if (value == null)
                {
                    myCell.Value = 0;
                }
                else if (value is string)
                {
                    int intval;
                    if (int.TryParse(value as String, out intval))
                    {
                        myCell.Value = intval;
                    }
                    else
                    {
                        myCell.Value = 0;
                    }
                }
                else
                {
                    myCell.Value = value;
                }

            }
            #endregion
            #region ComboBox
            else if (itemType == ItemTypes.ComboBox)
            {
                cell = new EasyGridComboBoxCell(_view.ListView);
                //ContextMenuStrip menu = cell.ContextMenuStrip;
                EasyGridComboBoxCell myCell = cell as EasyGridComboBoxCell;
                EasyGridComboBoxColumn col = _view.ListView.Columns[colIndex] as EasyGridComboBoxColumn;

                if (myCell.Items == null || myCell.Items.Count == 0)
                {
                    if (col.Items != null && col.Items.Count > 0)
                    {
                        myCell.Items = col.Items;
                        myCell.SelectedIndex = col.SelectedIndex;
                    }
                }
                /*
            if (_titleInitData[colIndex] != null && _titleInitData[colIndex] is ICollection<String>)
            {
                if (myCell.Items.Count == 0) myCell.Items = _titleInitData[colIndex] as ICollection<String>;
            }
            EasyGridComboBoxColumn col = _view.ListView.Columns[colIndex] as EasyGridComboBoxColumn;
                 */
                if (value == null)
                {
                    myCell.Value = -1;
                }
                else if (value is string)
                {
                    int intval;
                    if (int.TryParse(value as String, out intval))
                    {
                        myCell.Value = intval;
                    }
                    else if (col.Items != null && col.Items.Count > 0 && col.Items.Contains(value as string))
                    {
                        myCell.Value = col.Items.ToList().IndexOf(value as string);
                    }
                    else
                    {
                        myCell.Value = -1;
                    }
                }
                else
                {
                    myCell.Value = value;
                }
            }
            #endregion
            #region RadioButton
            else if (itemType == ItemTypes.RadioButton)
            {
                cell = new EasyGridRadioButtonCell(_view.ListView);

                //ContextMenuStrip menu = cell.ContextMenuStrip;
                EasyGridRadioButtonCell myCell = cell as EasyGridRadioButtonCell;
                EasyGridRadioButtonColumn col = _view.ListView.Columns[colIndex] as EasyGridRadioButtonColumn;

                if (myCell.Items == null || myCell.Items.Count == 0)
                {
                    if (col.Items != null && col.Items.Count > 0)
                    {
                        myCell.Items.Add(col.Items);
                        myCell.SelectedIndex = col.SelectedIndex;
                    }
                }
                /*
                if (_titleInitData[colIndex] != null && _titleInitData[colIndex] is ICollection<String>)
                {
                    if (cCell.Items.Count == 0) cCell.Items.Add(_titleInitData[colIndex] as ICollection<String>);
                }
                int refCount = cCell.Items.RefCount;
                 */
                if (value == null)
                {
                    myCell.Value = myCell.StartIndex;
                }
                else if (value is string)
                {
                    int intval;
                    if (int.TryParse(value as String, out intval))
                    {
                        myCell.Value = intval;
                    }
                    else
                    {
                        myCell.Value = myCell.StartIndex;
                    }
                }else
                {
                    myCell.Value = value;
                }
            }
            #endregion
            #region CheckBoxGroup
            else if (itemType == ItemTypes.CheckBoxGroup)
            {
                cell = new EasyGridCheckBoxGroupCell(_view.ListView);

                //ContextMenuStrip menu = cell.ContextMenuStrip;
                EasyGridCheckBoxGroupCell myCell = cell as EasyGridCheckBoxGroupCell;
                EasyGridCheckBoxGroupColumn col = _view.ListView.Columns[colIndex] as EasyGridCheckBoxGroupColumn;

                if (myCell.Items == null || myCell.Items.Count == 0)
                {
                    if (col.Items != null && col.Items.Count > 0)
                    {
                        myCell.Items.Add(col.Items);
                        if (col.Items.Count > 0) myCell.SetValue(col.Items.GetValue());
                    }
                }
                /*
                if (_titleInitData[colIndex] != null && _titleInitData[colIndex] is ICollection<String>)
                {
                    if(cCell.Items.Count == 0) cCell.Items.Add(_titleInitData[colIndex] as ICollection<String>);
                }
                */
                if (value == null)
                {
                    //myCell.Value = "";
                }
                else if (value is string)
                {
                    int intval;
                    if (int.TryParse(value as String, out intval))
                    {
                        myCell.Value = intval;
                    }
                    else
                    {
                        myCell.Value = value;
                    }
                }
                else
                {
                    myCell.Value = value;
                }
            }
            #endregion
            #region Button
            else if (itemType == ItemTypes.Button)
            {
                cell = new EasyGridButtonCell(_view.ListView);
                EasyGridButtonCell myCell = cell as EasyGridButtonCell;
                EasyGridButtonColumn col = _view.ListView.Columns[colIndex] as EasyGridButtonColumn;

                myCell.BaseText = col.Text;
                if (value == null)
                {
                    myCell.Value = col.Text;
                }
                else
                {
                    myCell.Value = value;
                }
            }
            #endregion
            #region CloseButton
            else if (itemType == ItemTypes.CloseButton)
            {
                cell = new EasyGridCloseButtonCell(_view.ListView);
                EasyGridCloseButtonCell myCell = cell as EasyGridCloseButtonCell;
                EasyGridCloseButtonColumn col = _view.ListView.Columns[colIndex] as EasyGridCloseButtonColumn;

                myCell.BaseText = (col.Text==null)? "X" : col.Text;
                myCell.Value = value;
                /*
                myCell.BaseText = _titleInitData[colIndex] as String;
                myCell.Value = value;
                 */
            }
            #endregion
            #region Image
            else if (itemType == ItemTypes.Image)
            {
                cell = new EasyGridImageCell(_view.ListView);
                EasyGridImageColumn col = _view.ListView.Columns[colIndex] as EasyGridImageColumn;
                EasyGridImageCell myCell = cell as EasyGridImageCell;
                if (myCell.Images == null || myCell.Images.Count == 0)
                {
                    if (col.Images != null && col.Images.Count > 0) myCell.Images = col.Images;
                }
                //if (_titleInitData[colIndex] != null && _titleInitData[colIndex] is ICollection<Image>) myCell.Images = (_titleInitData as ICollection<Image>);

                if (value == null)
                {
                    if (myCell.Images != null && myCell.Images.Count > 0) myCell.Value = 0;
                    //myCell.Value = "";
                }
                else if (value is string)
                {
                    int intval;
                    if (int.TryParse(value as String, out intval))
                    {
                        myCell.Value = intval;
                    }
                    
                    else
                    {
                        if (myCell.Images != null && myCell.Images.Count > 0) myCell.Value = 0;
                    }
                }
                else
                {
                    myCell.Value = value;
                }
            }
            #endregion
            #region ImageButton
            else if (itemType == ItemTypes.ImageButton)
            {
                cell = new EasyGridImageButtonCell(_view.ListView);
                EasyGridImageButtonColumn col = _view.ListView.Columns[colIndex] as EasyGridImageButtonColumn;
                EasyGridImageButtonCell myCell = cell as EasyGridImageButtonCell;
                if (myCell.Images == null || myCell.Images.Count == 0)
                {
                    if (col.Images != null && col.Images.Count > 0) myCell.Images = col.Images;
                }
                //if (_titleInitData[colIndex] != null && _titleInitData[colIndex] is ICollection<Image>) myCell.Images = (_titleInitData as ICollection<Image>);
                if (value == null)
                {
                    //myCell.Value = "";
                    if (myCell.Images != null && myCell.Images.Count > 0) myCell.Value = 0;
                }
                else if (value is string)
                {
                    int intval;
                    if (int.TryParse(value as String, out intval))
                    {
                        myCell.Value = intval;
                    }
                    else
                    {
                        myCell.Value = value;
                    }
                }
                else
                {
                    myCell.Value = value;
                }
            }
            #endregion
            else //Variables
            {
                /*
                if (value is EasyGridVariousTypeCellInfo)
                {
                    cell = getVariousCell(value as EasyGridVariousTypeCellInfo, tooltip);
                }
                 */
                if (value is EasyGridCellInfo)
                {
                    cell = SetVariousCell(value as EasyGridCellInfo, tooltip);
                }
                else
                {
                    EasyGridCellInfo info = new EasyGridCellInfo();
                    info.ItemType = ItemTypes.TextBox;
                    info.Text = (value!=null)? value.ToString() : "";
                    info.IsEditable = true;
                    cell = SetVariousCell(info, tooltip);
                    //throw new Exception("테이블 [" + _view.Name + "]의 입력 멤버타입이 틀립니다. row:" + row.Index + ", cell:" + colIndex + ", trying value:" + (value as String) + "/ 원래 타입:EasyGridVariousTypeCellInfo");
                }
            }
            if (tooltip != null && tooltip.Length > 0) (cell as DataGridViewCell).ToolTipText = tooltip;
            row.Cells.Add(cell as DataGridViewCell);


        }

        //public IEasyGridCell getVariousCell(EasyGridVariousTypeCellInfo info, String tooltip = null)
        public IEasyGridCell SetVariousCell(EasyGridCellInfo info, String tooltip = null)
        {
            IEasyGridCell cell;
            ItemTypes itemType = info.ItemType;

            #region TextBox
            if (itemType == ItemTypes.TextBox)
            {
                cell = new EasyGridTextBoxCell(_view.ListView as EasyGridViewParent);
                (cell as EasyGridTextBoxCell).SetValue(info);//.Value = info;//.Text;
                
            }
            #endregion
            #region FileOpenBox
            else if (itemType == ItemTypes.FileOpenBox)
            {
                cell = new EasyGridFileOpenBoxCell(_view.ListView as EasyGridViewParent);
                (cell as EasyGridFileOpenBoxCell).SetValue(info);//.Value = info;//.Text;
            }
            #endregion
            #region KeyValue
            else if (itemType == ItemTypes.KeyValue)
            {
                cell = new EasyGridKeyValueCell(_view.ListView);

                cell.SetValue(info);
            }
            #endregion
            #region CheckBox
            else if (itemType == ItemTypes.CheckBox)
            {
                cell = new EasyGridCheckBoxCell(_view.ListView);
                cell.SetValue(info);
            }
            #endregion
            #region ImageCheckBox
            else if (itemType == ItemTypes.ImageCheckBox)
            {
                cell = new EasyGridImageCheckBoxCell();
                cell.SetValue(info);

            }
            #endregion
            #region ComboBox
            else if (itemType == ItemTypes.ComboBox)
            {
                cell = new EasyGridComboBoxCell(_view.ListView);
                cell.SetValue(info);
            }
            #endregion
            #region RadioButton
            else if (itemType == ItemTypes.RadioButton)
            {
                cell = new EasyGridRadioButtonCell(_view.ListView);
                cell.SetValue(info);
            }
            #endregion
            #region CheckBoxGroup
            else if (itemType == ItemTypes.CheckBoxGroup)
            {
                cell = new EasyGridCheckBoxGroupCell(_view.ListView);
                cell.SetValue(info);
            }
            #endregion
            #region Button
            else if (itemType == ItemTypes.Button)
            {
                cell = new EasyGridButtonCell(_view.ListView);
                cell.SetValue(info);
            }
            #endregion
            #region CloseButton
            else if (itemType == ItemTypes.CloseButton)
            {
                cell = new EasyGridCloseButtonCell(_view.ListView);
                cell.SetValue(info);
            }
            #endregion
            #region Image
            else if (itemType == ItemTypes.Image)
            {
                cell = new EasyGridImageCell(_view.ListView);
                cell.SetValue(info);
            }
            #endregion
            #region ImageButton
            else if (itemType == ItemTypes.ImageButton)
            {
                cell = new EasyGridImageButtonCell(_view.ListView);
                if (info.Images == null)
                {
                    IEasyGridColumn col = _view.Column(this.Cells.Count);
                    if (col is EasyGridImageButtonColumn)
                    {
                        info.Images = (col as EasyGridImageButtonColumn).Images;
                    }
                    else if (col is EasyGridImageColumn)
                    {
                        info.Images = (col as EasyGridImageColumn).Images;
                    }

                    cell.SetValue(info);
                }
                else
                {
                    cell.SetValue(info);
                }
                
                
            }
            #endregion
            else//default
            {
                cell = new EasyGridTextBoxCell(_view.ListView as EasyGridViewParent);
                cell.SetValue(info);
            }

            return cell;
        }

        //int _tempHeight = 32;
        /*
        public void SetRowHeight(int height, RowHeightSettingModes mode = RowHeightSettingModes.SetForMax)
        {
            switch(mode){
                case RowHeightSettingModes.UpdateMaxNow:
                    if(_tempHeight<height) _tempHeight = height;
                    if (this.Height != _tempHeight)
                    {
                        this.Height = _tempHeight;
                        //DataGridView.InvalidateRow(this.Index);
                        //DataGridView.Refresh();
                        //DataGridView.Update();
                        base.PaintCells(DataGridView.CreateGraphics(),
                            DataGridView.GetRowDisplayRectangle(this.Index, true),
                            DataGridView.GetRowDisplayRectangle(this.Index, true),
                            this.Index,
                             DataGridViewElementStates.Resizable,
                             false, false, DataGridViewPaintParts.All);
                        DataGridView.UpdateRowHeightInfo(this.Index, true);
                        
                    }
                    break;
                case RowHeightSettingModes.UpdateMinNow:
                    if(_tempHeight>height) _tempHeight = height;
                    if (this.Height != _tempHeight)
                    {
                        this.Height = _tempHeight;
                        //DataGridView.InvalidateRow(this.Index);
                        DataGridView.UpdateRowHeightInfo(this.Index, true);
                        //DataGridView.Update();
                    }
                    break;
                case RowHeightSettingModes.SetForMax:
                    if (_tempHeight < height)
                    {
                        _tempHeight = height;
                    }
                    break;
                case RowHeightSettingModes.SetForMin:
                    if (_tempHeight < height) _tempHeight = height;
                    break;
                case RowHeightSettingModes.SetWithThis:
                    _tempHeight = height;
                    break;
                case RowHeightSettingModes.UpdateWithThisNow:
                default:
                    _tempHeight = height;
                    if (this.Height != _tempHeight)
                    {
                        this.Height = height;
                       // DataGridView.InvalidateRow(this.Index);
                        DataGridView.UpdateRowHeightInfo(this.Index, true);
                        //DataGridView.Update();
                    }
                    break;
            }

        }
        */
        List<Color> _orgCellColors = new List<Color>();
        RowBackModes _rowBackMode = RowBackModes.None;
        /// <summary>
        /// RowBackMode가 CustomColor이면 RowBackCustomColor 속성을 셋팅하는 대로 배경색이 바뀐다.
        /// </summary>
        public RowBackModes RowBackMode
        {
            get { return _rowBackMode; }
            set {
                if (_rowBackMode == RowBackModes.None || _rowBackMode == RowBackModes.CellColor)
                {
                    _orgCellColors.Clear();
                    for (int i = 0; i < Cells.Count; i++)
                    {
                        _orgCellColors.Add(Cells[i].Style.BackColor);
                    }
                }

                switch (value)
                {
                    case RowBackModes.Blue:
                        for (int i = 0; i < Cells.Count; i++)
                        {
                            Cells[i].Style.BackColor = Color.LightSkyBlue;
                        }
                        break;
                    case RowBackModes.CustomColor:
                        for (int i = 0; i < Cells.Count; i++)
                        {
                            Cells[i].Style.BackColor = RowBackCustomColor;
                        }
                        break;
                    case RowBackModes.Gray:
                        for (int i = 0; i < Cells.Count; i++)
                        {
                            Cells[i].Style.BackColor = Color.LightGray;
                        }
                        break;
                    case RowBackModes.Red:
                        for (int i = 0; i < Cells.Count; i++)
                        {
                            Cells[i].Style.BackColor = Color.LightPink;
                        }
                        break;
                    case RowBackModes.CellColor:
                    case RowBackModes.None:
                        if (_orgCellColors.Count > 0)
                        {
                            for (int i = 0; i < Cells.Count; i++)
                            {
                                Cells[i].Style.BackColor = _orgCellColors[i];
                            }
                        }
                        break;
                }
               // if (this.Index >= 0 && this._rowBackMode!=value)
               //     DataGridView.InvalidateRow(this.Index);
                _rowBackMode = value;

            }
        }

        Color _rowBackCustom = Color.FromArgb(200, 210, 255);
        /// <summary>
        /// RowBackMode가 RowBackModes.CustomColor 일 때 지정하는 색깔..
        /// </summary>
        public Color RowBackCustomColor
        {
            get { return _rowBackCustom; }
            set { 
                _rowBackCustom = value;
                for (int i = 0; i < Cells.Count; i++)
                {
                    Cells[i].Style.BackColor = value;
                }
                if (this.Index >= 0 && this._rowBackCustom != value && _rowBackMode== RowBackModes.CustomColor)
                    DataGridView.InvalidateRow(this.Index);
            }
        }

        List<DataGridViewCell> _rowCustomColorCells = new List<DataGridViewCell>();
        public List<DataGridViewCell> CustomColorCells
        {
            get { return _rowCustomColorCells; }
        }

        public IEasyGridCell this[int index]{
            get{
                return Cells[index] as IEasyGridCell;
            }
        }

        /// <summary>
        /// 해당 cell을 가져온다. 단, IEasyGridCell 인터페이스 타입으로 가져온다.
        /// column 이름이 String으로서 일치해야 한다.
        /// </summary>
        /// <param name="numEnum"></param>
        /// <returns></returns>
        public IEasyGridCell this[Enum numEnum]
        {
            get
            {
                return Cells[numEnum.ToString()] as IEasyGridCell;
            }
        }

        public IEasyGridCell this[string col_name]
        {
            get
            {
                return Cells[col_name] as IEasyGridCell;
            }
        }

        public List<IEasyGridCell> EasyCells
        {
            get
            {
                List<IEasyGridCell> cells = new List<IEasyGridCell>();
                foreach (IEasyGridCell cell in this.Cells)
                {
                    cells.Add(cell);
                }
                return cells;
            }
        }


        
    }

    public class RowSpan
    {
        EasyGridRow _targetRow;
        public List<RowSpanInfo> Spans = new List<RowSpanInfo>();

        internal RowSpan(EasyGridRow targetRow)
        {
            _targetRow = targetRow;
        }

        public void RemoveSpans()
        {
            foreach (RowSpanInfo info in Spans)
            {
                for (int i = info.SpanStart; i < info.SpanStart + info.SpanSize; i++)
                {
                    IEasyGridCell cell = _targetRow.EasyCells[i];
                    cell.Span.SetSpanBaseInRow(cell, 1);
                }
            }
        }

        
        public void AddSpan(int startIndex, int size)
        {
            Spans.Add(new RowSpanInfo(_targetRow, startIndex, size));
            IEasyGridCell baseCell = _targetRow.EasyCells[startIndex];
            for (int i = startIndex; i < startIndex + size; i++)
            {
                _targetRow.EasyCells[i].Span.SetSpanBaseInRow(baseCell, size);
            }
        }

        public SpanPosition GetSpanPosition(int colIndex)
        {
            foreach (RowSpanInfo info in Spans)
            {
                if (colIndex == info.SpanStart) return SpanPosition.SpanBase;
                else if (colIndex <= info.SpanStart+info.SpanSize) return SpanPosition.Spanned;
            }
            return SpanPosition.NoSpan;
        }
    }

    public class RowSpanInfo
    {
        public int SpanStart;
        public int SpanSize;

        EasyGridRow _targetRow;
        public RowSpanInfo(EasyGridRow targetRow, int startIndex, int spanSize)
        {
            _targetRow = targetRow;
            SpanStart = startIndex;
            SpanSize = spanSize;
        }
        public EasyGridRow TargetRow { get { return _targetRow; } }
        
    }

}
