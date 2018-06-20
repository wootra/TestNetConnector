using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using System.ComponentModel;
using IOHandling;
using FormAdders.EasyGridViewCollections;

namespace FormAdders
{
    public partial class EasyGridView : UserControl
    {
         
       

        #region events

        public event EasyGridMenuClickHandler E_ContextMenuClicked = null;
        public event CellClickEventHandler E_CellClicked;
        public event CellClickEventHandler E_CellDoubleClicked;
        public event CellClickEventHandler E_CellRightClicked;
        public event CellClickEventHandler E_ListRowRemoving;
        public event CellClickEventHandler E_ListRowRemoved;
        public event CellCheckedEventHandler E_CheckBoxChanged;
        public event CellRadioButtonSelectedEventHandler E_RadioButtonSelected;
        public event CellCheckBoxGroupSelectedEventHandler E_CheckBoxGroupSelected;
        public event CellTextChangedEventHandler E_TextChanged;
        public event CellTextChangedEventHandler E_TextEditFinished;
        public event CellComboBoxEventHandler E_ComboBoxChanged;
        public event RowPositionChangedHandler E_RowPositionChanged;
        /// <summary>
        /// Click, 버튼으로 인한 선택변화에 대응하는 Event이다.
        /// </summary>
        public event CellClickEventHandler E_CellSelected;
        #endregion

        #region member variables

        #region member Actions
        Actions _actionOnClicked = Actions.Nothing;
        Actions _actionOnDoubleClicked = Actions.Nothing;
        Actions _actionOnRightClicked = Actions.Nothing;

        #endregion 

        #region member bool
        bool _isAutoScrolling = true;
        #endregion

        #region member int
        int ClickedCol = -1;
        int ClickedRow = -1;
        int BeforeClickedRow = -1;
        int BeforeClickedCol = -1;
        #endregion

        #region member Lists
        List<object[]> _data = new List<object[]>();
        List<ItemTypes> _itemTypes = new List<ItemTypes>();
        protected List<bool?> _editables = new List<bool?>();
        protected List<Boolean> _isThreeState = new List<bool>();
        protected List<Object> _titleInitData = new List<object>();
        List<Image> _checkBoxImageList = new List<Image>();
        #endregion

        #endregion

        public EasyGridView()
            : base()
        {
            InitializeComponent();

            ActionOnDoubleClicked = Actions.CheckBoxChecked;
            ActionOnClicked = Actions.Nothing;
            ActionOnRightClicked = Actions.ContextMenu;
            

            V_Data.CellMouseDown += new DataGridViewCellMouseEventHandler(V_Data_CellMouseDown);
            V_Data.CellMouseUp += new DataGridViewCellMouseEventHandler(V_Data_CellMouseUp);
            //V_Data.PreviewKeyDown += new PreviewKeyDownEventHandler(V_Data_PreviewKeyDown);
            V_Data.CellDoubleClick += new DataGridViewCellEventHandler(V_Data_CellDoubleClick);


            V_Data.E_TextEditFinished += new CellTextChangedEventHandler(V_Data_E_TextEditFinished);
            //V_Data.CellBeginEdit += new DataGridViewCellCancelEventHandler(V_Data_CellBeginEdit);
            V_Data.E_TextChanged +=new CellTextChangedEventHandler(V_Data_E_TextChanged);
            V_Data.CellEnter += new DataGridViewCellEventHandler(V_Data_CellEnter);
            V_Data.KeyUp += new KeyEventHandler(V_Data_KeyUp);
            V_Data.CellValueChanged += new DataGridViewCellEventHandler(V_Data_CellValueChanged);
            V_Data.E_CellSelected += new CellClickEventHandler(V_Data_E_CellSelected);
            V_Data.Click += new EventHandler(V_Data_Click);
            DataGridViewRowsAddedEventHandler rowAddEventHandler = new DataGridViewRowsAddedEventHandler(RowsAdded);
            V_Data.RowsAdded += rowAddEventHandler;
            V_Data.ContextMenu = new System.Windows.Forms.ContextMenu();
            V_Data.DataError += new DataGridViewDataErrorEventHandler(V_Data_DataError);
            V_Data.CellPainting += new DataGridViewCellPaintingEventHandler(V_Data_CellPainting);
            // T_Modify.Visible = false;

            //V_Data.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            //V_Data.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
            V_Data.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            V_Data.RowHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            V_Data.EditMode = DataGridViewEditMode.EditProgrammatically;
            V_Data.RowHeadersWidth = 50;

            V_Data.setEditables(_editables, _itemTypes);

            _checkBoxImageList.Add(Properties.Resources.check_normal);
            _checkBoxImageList.Add(Properties.Resources.check_red);
            _checkBoxImageList.Add(Properties.Resources.check_inter);
        }





        



        #region Actions 

        ActionsOnCtrl_MoveKey _actionOnCtrlMove = ActionsOnCtrl_MoveKey.SelectRowsMove;
        /// <summary>
        /// 컨트롤키와 Up,Down,PageUp, PageDown키를 눌렀을 때의 액션을 정의.
        /// </summary>
        public ActionsOnCtrl_MoveKey ActionOnCtrlMoveKey
        {
            get
            {
                return _actionOnCtrlMove;
            }
            set
            {
                _actionOnCtrlMove = value;
            }
        }

        /// <summary>
        /// 컨트롤에서 마우스를 두번 연속 눌렀을 때 동작할 내용입니다. Column별로 Action을 지정했다면 무시됩니다.
        /// 주의: Auto 나 CommonAction 속성으로 설정하면 무시됩니다.
        /// </summary>
        public Actions ActionOnDoubleClicked { 
            get { return _actionOnClicked; } 
            set {
                if (value == Actions.Auto || value == Actions.CommonAction)
                {
                    _actionOnClicked = Actions.Nothing;
                }
                else _actionOnClicked = value;
            } 
        }
        
        /// <summary>
        /// 컨트롤에서 마우스 왼쪽 버튼을 눌렀을 때 동작할 내용입니다. Column별로 Action을 지정했다면 무시됩니다.
        /// 주의: Auto 나 CommonAction 속성으로 설정하면 무시됩니다.
        /// </summary>
        public Actions ActionOnClicked {
            get { return _actionOnDoubleClicked; }
            set
            {
                if (value == Actions.Auto || value == Actions.CommonAction)
                {
                    _actionOnDoubleClicked = Actions.Nothing;
                }
                else _actionOnDoubleClicked = value;
            }
        }

        /// <summary>
        /// 편집모드에서 엔터를 쳤을 때의 액션..기본값은 EnterActions.EditNextRow이다.
        /// </summary>
        public EnterActions ActionOnEnterInEditMode
        {
            get
            {
                return V_Data.ActionOnEnterInEditMode;
            }
            set
            {
                V_Data.ActionOnEnterInEditMode = value;
            }
        }

        /// <summary>
        /// 컨트롤에서 마우스 오른쪽 버튼을 눌렀을 때 동작할 내용입니다. Column별로 Action을 지정했다면 무시됩니다.
        /// 주의: Auto 나 CommonAction 속성으로 설정하면 무시됩니다.
        /// </summary>
        public Actions ActionOnRightClicked {
            get
            {
                return _actionOnRightClicked;
            }
            set
            {
                if (value == Actions.Auto || value == Actions.CommonAction)
                {
                    _actionOnRightClicked = Actions.Nothing;
                }
                else _actionOnRightClicked = value;
            }
        }

        List<Actions> _columnActionOnClicked = new List<Actions>();
        List<Actions> _columnActionOnDoubleClicked = new List<Actions>();
        List<Actions> _columnActionOnRightClicked = new List<Actions>();

        public void setColumnAction(int columnIndex, Actions action, ActionConditions actionCondition)
        {
            if (columnIndex < _columnActionOnClicked.Count)
            {
                if (actionCondition == ActionConditions.Clicked) _columnActionOnClicked[columnIndex] = action;
                else if (actionCondition == ActionConditions.DoubleClicked) _columnActionOnDoubleClicked[columnIndex] = action;
                else  _columnActionOnRightClicked[columnIndex] = action; //rightClicked
            }
            else
            {
                //out of range
            }
        }
        #endregion

        #region Properties
        public DataGridViewHeaderCell TopLeftCell
        {
            get
            {
                return V_Data.TopLeftHeaderCell;
            }
        }

        public List<int> SelectedRowsIndice
        {
            get
            {
                List<int> list = new List<int>();
                DataGridViewSelectedCellCollection cells = V_Data.SelectedCells;
                DataGridViewSelectedRowCollection rows = V_Data.SelectedRows;

                for (int i = 0; i < rows.Count; i++)
                {
                    list.Add(rows[i].Index);
                }

                for (int i = 0; i < cells.Count; i++)
                {
                    if (list.Contains(V_Data.Rows[cells[i].RowIndex].Index) == false) list.Add(V_Data.Rows[cells[i].RowIndex].Index);
                }

                return list;
            }
            set
            {
                for (int i = 0; i < value.Count; i++)
                {
                    V_Data.Rows[i].Selected = true;
                }
            }
        }



        

        /// <summary>
        /// get, SelectedItems와 같다. 현재 선택된 Row의 리스트를 가져온다.
        /// </summary>
        public List<EasyGridRow> SelectedRows
        {
            get
            {
                List<EasyGridRow> list = new List<EasyGridRow>();
                DataGridViewSelectedCellCollection cells = V_Data.SelectedCells;
                DataGridViewSelectedRowCollection rows = V_Data.SelectedRows;

                for (int i = 0; i < rows.Count; i++)
                {
                    list.Add(rows[i] as EasyGridRow);
                }

                for (int i = 0; i < cells.Count; i++)
                {
                    if (list.Contains(V_Data.Rows[cells[i].RowIndex] as EasyGridRow) == false) list.Add(V_Data.Rows[cells[i].RowIndex] as EasyGridRow);
                }

                return list;
            }
        }

        /// <summary>
        /// get, SelectedRows와 같다. 현재 선택된 Row의 리스트를 가져온다.
        /// </summary>
        public List<EasyGridRow> SelectedItems
        {
            get
            {
                List<EasyGridRow> list = new List<EasyGridRow>();
                DataGridViewSelectedRowCollection rows = V_Data.SelectedRows;
                for (int i = 0; i < rows.Count; i++)
                {
                    list.Add(rows[i] as EasyGridRow);
                }
                return list;
            }
        }


        /// <summary>
        /// 이 EasyGridView와 연관되는 데이터를 지정할 수 있습니다. Dictionary형 변수이므로 key로 접근할 수 있습니다.
        /// </summary>
        public Dictionary<String, Object> RelativeObject = new Dictionary<string, object>();

        bool _isTitleVisible = true;
        public Boolean IsTitleVisible
        {
            get
            {
                return V_Data.ColumnHeadersVisible;
            }
            set
            {
                V_Data.ColumnHeadersVisible = value;
            }
        }

        int _indexWidth = 30;
        public int U_IndexWidth
        {
            get
            {
                return _indexWidth;
            }
            set
            {
                V_Data.RowHeadersWidth = value;
                _indexWidth = value;
            }
        }

        public int RowCount
        {
            get
            {
                return V_Data.Rows.Count;
            }
        }

        public int ColumnCount
        {
            get
            {
                return V_Data.Columns.Count;
            }
        }

        public ContextMenu U_ContextMenu
        {
            get { return _contextMenu; }
        }

        bool _showIndex = false;
        public bool U_ShowIndex
        {
            get
            {
                return _showIndex;
            }
            set
            {
                _showIndex = value;

                ShowIndex(value);
            }
        }

        public DataGridViewColumnCollection Header
        {
            get { return V_Data.Columns; }
        }

        public List<EasyGridRow> Rows
        {
            get
            {
                List<EasyGridRow> list = new List<EasyGridRow>();
                foreach (EasyGridRow row in V_Data.Rows)
                {
                    list.Add(row);
                }
                return list;
            }
        }

        public DataGridView ListView
        {
            get { return V_Data; }
        }

        public List<int> CheckedIndices
        {
            get
            {
                List<int> indices = new List<int>();
                int checkCol = FindCheckBoxColumn();

                if (checkCol < 0) return indices; //빈 리스트

                for (int i = 0; i < V_Data.Rows.Count; i++)
                {
                    if ((bool)GetValue(i, checkCol) == true)
                    {
                        indices.Add(i);
                    }
                }
                return indices;
            }
        }

        public List<EasyGridRow> CheckedRows
        {
            get
            {
                List<EasyGridRow> rows = new List<EasyGridRow>();
                int checkCol = FindCheckBoxColumn();

                if (checkCol < 0) return rows; //빈 리스트

                for (int i = 0; i < V_Data.Rows.Count; i++)
                {
                    if (GetValue(i, checkCol).Equals(true)) rows.Add(V_Data.Rows[i] as EasyGridRow);
                    /*
                    if (_itemTypes[ V_Data.Rows[i].Cells[checkCol] is DataGridViewCheckBoxCell)
                    {
                        DataGridViewCheckBoxCell cell = V_Data.Rows[i].Cells[checkCol] as DataGridViewCheckBoxCell;
                        if ((bool)cell.Value == true) rows.Add(V_Data.Rows[i] as EasyGridRow);
                    }else{

                    }
                     */

                }
                return rows;
            }
        }


        [Browsable(true)]
        [EditorBrowsable]
        public Color U_BackColor
        {
            get
            {
                return V_Data.BackgroundColor;

            }
            set
            {
                V_Data.BackgroundColor = value;
            }
        }

        public Color U_GridColor
        {
            get
            {
                return V_Data.GridColor;
            }
            set
            {
                V_Data.GridColor = value;
            }
        }

        public String ColumnName(int colIndex)
        {
            return V_Data.Columns[colIndex].Name;
        }

        public List<String> ColumnNames
        {
            get
            {
                List<String> list = new List<string>();
                for (int i = 0; i < V_Data.Columns.Count; i++) list.Add(V_Data.Columns[i].Name);
                return list;
            }
        }

        public String TitleText(int colIndex)
        {
            return V_Data.Columns[colIndex].HeaderText;
        }

        public int ColIndex(String colName)
        {
            for (int i = 0; i < V_Data.Columns.Count; i++)
            {
                if (V_Data.Columns[i].Name.Equals(colName)) return i;
            }
            return -1;
        }

        public List<String> TitleTexts
        {
            get
            {
                List<String> list = new List<string>();
                for (int i = 0; i < V_Data.Columns.Count; i++) list.Add(V_Data.Columns[i].HeaderText);
                return list;
            }
        }


        public DataGridViewHeaderCell ColumnHeader(int colIndex)
        {
            return V_Data.Columns[colIndex].HeaderCell;
        }

        public DataGridViewHeaderCell RowHeader(int rowIndex)
        {
            return V_Data.Rows[rowIndex].HeaderCell;
        }

        public int RowHeaderWidth
        {
            get
            {
                return V_Data.RowHeadersWidth;
            }
            set
            {
                V_Data.RowHeadersWidth = value;
            }
        }

        /// <summary>
        /// 각 column의 Width의 목록입니다.
        /// </summary>
        public List<int> Wid { 
            get {
                List<int> wid = new List<int>();
                for (int i = 0; i < V_Data.Columns.Count; i++)
                {
                    wid.Add(V_Data.Columns[i].Width);
                }
                return wid;
            }
        }

        #endregion

        #region EventHandlers

        void V_Data_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // e.ThrowException = true;

            //  MessageBox.Show(e.Exception.ToString());
            e.Cancel = true;
        }

        void V_Data_Click(object sender, EventArgs e)
        {
            OnClick(e);
        }

        void V_Data_E_CellSelected(object sender, CellClickEventArgs e)
        {
            IEasyGridCell cell = (Cell(e.RowIndex, e.ColIndex) as IEasyGridCell);
            if (cell!=null && cell.ItemType == ItemTypes.ComboBox)
            {
                ClickedComboBox.Set(e.RowIndex, e.ColIndex);
            }
            if (E_CellSelected != null) E_CellSelected(this, e);
        }

        void V_Data_KeyUp(object sender, KeyEventArgs e)
        {
            if (V_Data.CurrentCell == null) return;
            int col = V_Data.CurrentCell.ColumnIndex;
            int row = V_Data.CurrentCell.RowIndex;

            if (e.KeyData == Keys.Space)
            {
                if (_itemTypes[col] == ItemTypes.ComboBox)
                {
                    bool isTrue = (bool)GetValue(row, col);
                    SetValueInCell(row, col, !isTrue);
                }
                else if (_itemTypes[col] == ItemTypes.ImageCheckBox)
                {
                    bool isTrue = (bool)GetValue(row, col);
                    SetValueInCell(row, col, !isTrue);
                }
            }
            else if (Control.ModifierKeys == Keys.Control)
            {
                if (_actionOnCtrlMove == ActionsOnCtrl_MoveKey.None)
                {
                }
                else
                {
                    List<EasyGridRow> rows;
                    if (_actionOnCtrlMove == ActionsOnCtrl_MoveKey.SelectRowsMove) rows = SelectedRows;
                    else rows = CheckedRows;

                    if (e.KeyData == (Keys.Up | Keys.Control))
                    {
                        
                        MoveRowsUp(SelectedRows, 1);
                        ReleaseSelection();
                        SelectRow(rows, true);
                    }
                    else if (e.KeyData == (Keys.Control | Keys.Down))
                    {
                        
                        MoveRowsDown(SelectedRows, 1);
                        ReleaseSelection();
                        SelectRow(rows, true);
                    }
                    else if (e.KeyData == (Keys.Control | Keys.PageUp))
                    {
                        
                        MoveRowsTo(SelectedRows, 0);
                        ReleaseSelection();
                        SelectRow(rows, true);
                    }
                    else if (e.KeyData == (Keys.Control | Keys.PageDown))
                    {
                        
                        MoveRowsTo(SelectedRows, Rows.Count - 1);
                        ReleaseSelection();
                        SelectRow(rows, true);
                    }
                }
            }
            else if (Control.ModifierKeys == Keys.Alt)
            {
                if (ColumnItemType(V_Data.CurrentCell.ColumnIndex) == ItemTypes.ComboBox)
                {
                    DataGridViewComboBoxCell cell = V_Data.CurrentCell as DataGridViewComboBoxCell;

                    if (e.KeyData == (Keys.Up | Keys.Alt))
                    {
                        ShowComboBoxDropDown(cell);
                    }
                    else if (e.KeyData == (Keys.Alt | Keys.Down))
                    {
                        ShowComboBoxDropDown(cell);
                    }
                    else if (e.KeyData == (Keys.Alt | Keys.PageUp))
                    {
                        
                    }
                    else if (e.KeyData == (Keys.Alt | Keys.PageDown))
                    {
                        
                    }
                }
            }
            
        }

        void V_Data_E_TextChanged(object sender, CellTextChangedEventArgs e)
        {
            if (E_TextChanged != null) E_TextChanged(this, e);
        }

        void V_Data_E_TextEditFinished(object sender, CellTextChangedEventArgs e)
        {
            if (E_TextEditFinished != null) E_TextEditFinished(this, e);
        }

        void V_Data_CellEnter(object sender, DataGridViewCellEventArgs e)
        {

            //if(_showPosition) V_Data.TopLeftHeaderCell.Value = "(" + (e.RowIndex + 1) + "," + (e.ColumnIndex + 1) + ")";
            ClickedCol = e.ColumnIndex;
            ClickedRow = e.RowIndex;
        }

        void V_Data_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            V_Data.TopLeftHeaderCell.Value = "";
        }

        void RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            DataGridView dv = (DataGridView)sender;
            if (dv.RowCount > 10 && _isAutoScrolling) dv.FirstDisplayedScrollingRowIndex = dv.RowCount - 10;
        }

        public int FirstDisplayedScrollingRowIndex
        {
            get
            {
                return V_Data.FirstDisplayedScrollingRowIndex;
            }
            set
            {
                if (value >= 0)
                {
                    V_Data.FirstDisplayedScrollingRowIndex = value;
                }
            }
        }

        public int FirstDisplayedScrollingColIndex
        {
            get { return V_Data.FirstDisplayedScrollingColumnIndex; }
            set { 
                if(value>=0)
                    V_Data.FirstDisplayedScrollingColumnIndex = value; 
            }
        }

        public int DisplayedRowCount
        {
            get
            {
                return V_Data.DisplayedRowCount(true);
            }
        }

        public int DisplayedColCount
        {
            get
            {
                return V_Data.DisplayedColumnCount(true);
            }
        }

        public DataGridViewCell FirstDisplayedCell
        {
            get { return V_Data.FirstDisplayedCell; }
        }

        protected void OnCheckBoxClicked(int rowIndex, int colIndex)
        {

            /*
            ListBoxItem item = WpfFinder.getParentFromTemplate(cb, DependencyObjectType.FromSystemType(typeof(ListBoxItem))) as ListBoxItem;
            //ListBox view = WpfFinder.getParentFromTemplate(item, DependencyObjectType.FromSystemType(typeof(ListBox))) as ListBox;
            if (item == null) return;
            ListRow row = item.Content as ListRow;
            */
            // DataGridViewCheckBoxCell cell = V_Data.Rows[rowIndex].Cells[colIndex] as DataGridViewCheckBoxCell;

            bool? boolState = (bool?)GetValue(rowIndex, colIndex);// (int)cell.Value;
            int checkState = (boolState == true) ? 1 : (boolState == false) ? 0 : 2;

            

            List<int> added = new List<int>();
            List<int> removed = new List<int>();
            List<int> checkedList = new List<int>();


            if (Control.ModifierKeys == Keys.Shift)
            {
                addChecked(rowIndex, colIndex, ref added, ref removed);
            }
            else if (Control.ModifierKeys == Keys.Alt)
            {
                removeChecked(rowIndex, colIndex, ref added, ref removed);
            }
            else
            {
                // DataGridViewCheckBoxColumn col = V_Data.Columns[colIndex] as DataGridViewCheckBoxColumn;

                if (_isThreeState[colIndex]) checkState = (checkState + 2) % 3;
                else checkState = (checkState + 1) % 2;
                SetValueInCell(rowIndex, colIndex, checkState);// cell.Value = 2;
                boolState = (checkState == 1) ? true : (checkState == 0) ? false : (bool?)null;
                if (boolState == true) added.Add(rowIndex);
                else if (boolState == false) removed.Add(rowIndex);
                if (E_CheckBoxChanged != null) E_CheckBoxChanged(V_Data, new CellCheckedEventArgs((bool?)boolState, rowIndex, colIndex, added, removed));
            }

        }
   
        void V_Data_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            DataGridViewRow row = V_Data.Rows[e.RowIndex];
            DataGridViewCell cell = row.Cells[e.ColumnIndex];

            Actions action;
            if (_columnActionOnDoubleClicked[e.ColumnIndex] == Actions.CommonAction)
            {
                action = ActionOnDoubleClicked;
            }
            else
            {
                action = _columnActionOnDoubleClicked[e.ColumnIndex];
            }

            if (_itemTypes[e.ColumnIndex] == ItemTypes.CloseButton)
            {
                if (action == Actions.Auto)
                {
                    //do nohting..
                }
                else
                {
                    DoAction(action, e.RowIndex, e.ColumnIndex);
                }
            }
            else if (_itemTypes[e.ColumnIndex] == ItemTypes.TextBox)
            {
                if (action == Actions.Auto)
                {
                    if (_editables[e.ColumnIndex]==true) BeginModifyMode(e.RowIndex, e.ColumnIndex);

                    //else do nothing..
                }
                else
                {
                    if (action == Actions.Modify) //modify mode라고 해도 _editable로 등록되어있지 않으면 수정할 수 없다.
                    {
                        if (_editables[e.ColumnIndex]==true) BeginModifyMode(e.RowIndex, e.ColumnIndex);
                        else
                        {
                            if (_columnActionOnDoubleClicked[e.ColumnIndex] == Actions.Modify)
                            {
                                BeginModifyMode(e.RowIndex, e.ColumnIndex); //그러나 강제로 할 수는 있다.
                            }
                        }
                    }
                    else
                    {
                        DoAction(action, e.RowIndex, e.ColumnIndex);
                    }
                }

                if (E_CellDoubleClicked != null) E_CellDoubleClicked(sender, new CellClickEventArgs(e.RowIndex, e.ColumnIndex, row,_itemTypes[e.ColumnIndex], cell, V_Data, e));
            }
            else if (_itemTypes[e.ColumnIndex] == ItemTypes.Image ||
               _itemTypes[e.ColumnIndex] == ItemTypes.Button)
            {
                if (action == Actions.Auto)
                {
                    //do nothing..
                }
                else
                {
                    DoAction(action, e.RowIndex, e.ColumnIndex);
                }
                if (E_CellDoubleClicked != null) E_CellDoubleClicked(sender, new CellClickEventArgs(e.RowIndex, e.ColumnIndex, row, _itemTypes[e.ColumnIndex], cell, V_Data, e));
            }
            else if (_itemTypes[e.ColumnIndex] == ItemTypes.ComboBox)
            {
                if (action == Actions.Auto)
                {
                    //do nothing..
                }
                else
                {
                    DoAction(action, e.RowIndex, e.ColumnIndex);
                }
                if (E_CellDoubleClicked != null) E_CellDoubleClicked(sender, new CellClickEventArgs(e.RowIndex, e.ColumnIndex, row, _itemTypes[e.ColumnIndex], cell, V_Data, e));
            }
        }

        //bool _isEditing = false;
        void CloseBtnClicked(int RowIndex, int ColumnIndex, EventArgs e)
        {

            DataGridViewRow deletedRow = V_Data.Rows[RowIndex];
            EasyGridButtonCell cell = deletedRow.Cells[ColumnIndex] as EasyGridButtonCell;
            CellClickEventArgs args = new CellClickEventArgs(RowIndex, ColumnIndex, deletedRow, _itemTypes[ColumnIndex], cell, V_Data, e);

            if (E_ListRowRemoving != null) E_ListRowRemoving(deletedRow, args);

            if (args.IsCancel != null)
            {
                if (args.IsCancel is bool && (bool)(args.IsCancel) == false)
                {
                    return;
                }
            }
            RemoveARow(RowIndex);

            if (E_ListRowRemoved != null) E_ListRowRemoved(deletedRow, args);
        }

        void V_Data_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            /*
            if (Control.ModifierKeys == Keys.Control || Control.ModifierKeys == Keys.Shift)
            {
                BeforeClickedCol = ClickedCol;
                BeforeClickedRow = ClickedRow;
            }
            else
            {
                BeforeClickedCol = -1;
                BeforeClickedRow = -1;
            }
             */
            ClickedCol = e.ColumnIndex;
            ClickedRow = e.RowIndex;

        }

        /*
        void V_Data_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            if (_itemTypes[e.ColumnIndex] == ItemTypes.TextBox)
            {
                //cell의 내용 바뀌었을 때.
                OnTextChanged(e);
            }
            
        }
        
        void V_Data_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (_itemTypes[e.ColumnIndex] == ItemTypes.TextBox)
            {
                OnTextEditFinished(e);
            }
            _isEditing = false;
        }

        void OnTextEditFinished(DataGridViewCellEventArgs e)
        {
            DataGridViewTextBoxCell cell = V_Data.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewTextBoxCell;
            DataGridViewRow row = V_Data.Rows[e.RowIndex];
            String newText = (cell.Value != null) ? cell.Value.ToString() : "";
            CellTextChangedEventArgs arg = new CellTextChangedEventArgs(_originalText, newText, e.ColumnIndex, e.RowIndex, row, cell);
            if (E_TextEditFinished != null) E_TextEditFinished(V_Data, arg);
        }
        
        void OnTextChanged(DataGridViewCellParsingEventArgs e)
        {
            DataGridViewTextBoxCell cell = V_Data.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewTextBoxCell;
            DataGridViewRow row = V_Data.Rows[e.RowIndex];
            CellTextChangedEventArgs arg = new CellTextChangedEventArgs(_originalText, cell.EditedFormattedValue.ToString(), e.ColumnIndex, e.RowIndex, row, cell);
            if (E_TextChanged != null) E_TextChanged(V_Data, arg);
        }
        */

        PositionOnList BeforeComboBox = new PositionOnList(-1, -1);
        PositionOnList ClickedComboBox = new PositionOnList(-1, -1);
        void V_Data_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {

            int iRow = e.RowIndex;
            int iCell = e.ColumnIndex;
            //bool isHeader = false;
            EasyGridRow row = null;
            DataGridViewCell cell;
            DataGridViewColumn col = null;
            ItemTypes itemType;
            if (e.RowIndex < 0 && e.ColumnIndex < 0) return;

            if (e.ColumnIndex >= 0) col = V_Data.Columns[e.ColumnIndex];

            if (iRow < 0)
            {

                //if (e.X <= 5 || e.X >= col.Width - 5) return; //크기변경구역
                DoTitleActions(iCell);
                return;
            }
            else
            {
                row = V_Data.Rows[iRow] as EasyGridRow;
            }

            if (iCell < 0)
            {
                //isHeader = true;
                cell = row.HeaderCell;
                itemType = ItemTypes.Header;

            }
            else
            {
                cell = row.Cells[iCell];
                itemType = _itemTypes[iCell];

            }




            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {

                doLeftClick(row, cell, iRow, iCell, itemType,e);


            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                Actions action;
                if (iCell < 0) action = ActionOnRightClicked;
                else if (_columnActionOnRightClicked[iCell] != Actions.CommonAction)
                {
                    action = _columnActionOnRightClicked[iCell];
                }
                else action = ActionOnRightClicked;

                if (itemType == ItemTypes.TextBox)
                {
                   // DataGridViewTextBoxCell myCell = V_Data.Rows[iRow].Cells[iCell] as DataGridViewTextBoxCell;

                    if (action == Actions.Auto) DoAction(Actions.ContextMenu, iRow, iCell);
                    else DoAction(action, iRow, iCell);
                    if (E_CellRightClicked != null) E_CellRightClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e));
                }
                else if (itemType == ItemTypes.CheckBox)
                {
                    if (action == Actions.Auto) DoAction(Actions.ContextMenu, iRow, iCell);
                    else DoAction(action, iRow, iCell);
                    if (E_CellRightClicked != null) E_CellRightClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e));
                }
                else if (itemType == ItemTypes.ComboBox)
                {
                    if (action == Actions.Auto) DoAction(Actions.ContextMenu, iRow, iCell);
                    else DoAction(action, iRow, iCell);
                    if (E_CellRightClicked != null) E_CellRightClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e));
                }
                else if (itemType == ItemTypes.Button)
                {
                    if (action == Actions.Auto) DoAction(Actions.ContextMenu, iRow, iCell);
                    else DoAction(action, iRow, iCell);
                    if (E_CellRightClicked != null) E_CellRightClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e));
                }
                else if (itemType == ItemTypes.CloseButton)
                {
                    if (action == Actions.Auto)
                    {
                        DoAction(Actions.ContextMenu, iRow, iCell);
                        if (E_CellRightClicked != null) E_CellRightClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e));
                    }
                    else if (action == Actions.Nothing)
                    {
                        CloseBtnClicked(iRow, iCell, e);
                    }
                    else
                    {
                        DoAction(action, iRow, iCell);
                        if (E_CellRightClicked != null) E_CellRightClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e));
                    }
                }
                else if (itemType == ItemTypes.Image)
                {
                    if (action == Actions.Auto) DoAction(Actions.ContextMenu, iRow, iCell);
                    else DoAction(action, iRow, iCell);
                    if (E_CellRightClicked != null) E_CellRightClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e));
                }
                else //header
                {
                    if (action == Actions.Auto) DoAction(Actions.ContextMenu, iRow, iCell);
                    else DoAction(action, iRow, -1);
                    if (E_CellRightClicked != null) E_CellRightClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e));
                }

            }

        }
        void doLeftClick(EasyGridRow row, DataGridViewCell cell, int iRow, int iCell, ItemTypes itemType, EventArgs e)
        {
            Actions action;

            if (iCell < 0)
            {
                action = ActionOnClicked;
            }
            else if (_columnActionOnClicked[iCell] == Actions.CommonAction)
            {
                action = ActionOnClicked;
            }
            else
            {
                action = _columnActionOnClicked[iCell];
            }
            if (itemType == ItemTypes.ImageCheckBox)
            {
                //DataGridViewCheckBoxCell myCell = V_Data.Rows[iRow].Cells[iCell] as DataGridViewCheckBoxCell;

                if (action == Actions.Auto) OnCheckBoxClicked(iRow, iCell);
                else if (action == Actions.CheckBoxChecked) OnCheckBoxClicked(iRow, iCell);
                else if (action == Actions.Nothing) OnCheckBoxClicked(iRow, iCell);
                else DoAction(action, iRow, iCell);
                if(_itemTypes[iCell] != ItemTypes.Various) checkIfAllChecked(iCell);
                if (E_CellClicked != null) E_CellClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e));
            }
            else if (itemType == ItemTypes.TextBox)
            {
                //DataGridViewTextBoxCell myCell = V_Data.Rows[iRow].Cells[iCell] as DataGridViewTextBoxCell;
                if (action == Actions.Auto)
                {
                    //do nothing..
                    if (E_CellClicked != null) E_CellClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e));
                }
                else if (action == Actions.Modify)
                {
                    if (_editables[iCell] == true) BeginModifyMode(iRow, iCell);
                    else if (E_CellClicked != null) E_CellClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e));
                }
                else
                {
                    DoAction(action, iRow, iCell);
                    if (E_CellClicked != null) E_CellClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e));
                }

                //if (E_ListRowClicked != null) E_ListRowClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, cell, V_Data, e));
            }
            else if (itemType == ItemTypes.KeyValue)
            {
                //DataGridViewTextBoxCell myCell = V_Data.Rows[iRow].Cells[iCell] as DataGridViewTextBoxCell;
                if (action == Actions.Auto)
                {
                    //do nothing..
                    if (E_CellClicked != null) E_CellClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e));
                }
                else
                {
                    DoAction(action, iRow, iCell);
                    if (E_CellClicked != null) E_CellClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e));
                }

                //if (E_ListRowClicked != null) E_ListRowClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, cell, V_Data, e));
            }
            else if (itemType == ItemTypes.KeyColor)
            {
                //DataGridViewTextBoxCell myCell = V_Data.Rows[iRow].Cells[iCell] as DataGridViewTextBoxCell;
                if (action == Actions.Auto)
                {
                    //do nothing..
                    if (E_CellClicked != null) E_CellClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e));
                }
                else
                {
                    DoAction(action, iRow, iCell);
                    if (E_CellClicked != null) E_CellClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e));
                }

                //if (E_ListRowClicked != null) E_ListRowClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, cell, V_Data, e));
            }
            else if (itemType == ItemTypes.CheckBox)
            {
                // DataGridViewCheckBoxCell myCell = V_Data.Rows[iRow].Cells[iCell] as DataGridViewCheckBoxCell;

                if (action == Actions.Auto) OnCheckBoxClicked(iRow, iCell);
                else if (action == Actions.CheckBoxChecked) OnCheckBoxClicked(iRow, iCell);
                else if (action == Actions.Nothing) OnCheckBoxClicked(iRow, iCell);
                else DoAction(action, iRow, iCell);

                if (E_CellClicked != null) E_CellClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e));
            }
            else if (itemType == ItemTypes.ComboBox)
            {
                //comboBox는 leftclick시에 정해진 작업이 있으므로 다른 일을 하지 않는다.
                DataGridViewComboBoxCell myCell = V_Data.Rows[iRow].Cells[iCell] as DataGridViewComboBoxCell;
                //ClickedComboBox.Set(myCell.RowIndex, myCell.ColumnIndex);
                //ComboSelectedRow = myCell.RowIndex;
                //ComboSelectedCol = myCell.ColumnIndex;
                ClickedComboBox.Set(iRow, iCell);
                V_Data.CurrentCell = myCell;
                /*
                V_Data.BeginEdit(true);
                DataGridViewComboBoxEditingControl comboboxEdit = (DataGridViewComboBoxEditingControl)V_Data.EditingControl;
                comboboxEdit.DroppedDown = true;
                V_Data.EndEdit();
                 */


                if (action != Actions.ContextMenu)
                {
                    ShowComboBoxDropDown(myCell);
                }
                BeforeComboBox.Set(ClickedComboBox);
            }
            else if (itemType == ItemTypes.RadioButton)
            {
                EasyGridRadioButtonCell myCell = V_Data.Rows[iRow].Cells[iCell] as EasyGridRadioButtonCell;
                Point pt = V_Data.PointToClient(Control.MousePosition);
                int selected = myCell.ClickOnCell(pt.X, pt.Y);
                if (selected >= 0)
                {
                    if (E_RadioButtonSelected != null) E_RadioButtonSelected(this, new CellRadioButtonSelectedEventArgs(myCell.Value, iRow, iCell, myCell.Items[myCell.Value].Text, myCell));
                    if (E_CellClicked != null) E_CellClicked(this, new CellClickEventArgs(iRow, iCell, row, itemType, myCell, V_Data, e));
                }
            }
            else if (itemType == ItemTypes.CheckBoxGroup)
            {
                EasyGridCheckBoxGroupCell myCell = V_Data.Rows[iRow].Cells[iCell] as EasyGridCheckBoxGroupCell;
                Point pt = V_Data.PointToClient(Control.MousePosition);
                int selected = myCell.ClickOnCell(pt.X, pt.Y);
                if (selected >= 0)
                {
                    if (E_CheckBoxGroupSelected != null) E_CheckBoxGroupSelected(this, new CellCheckBoxGroupSelectedEventArgs(selected, iRow, iCell, myCell.Value, myCell));
                    if (E_CellClicked != null) E_CellClicked(this, new CellClickEventArgs(iRow, iCell, row, itemType, myCell, V_Data, e));
                }
            }
            else if (itemType == ItemTypes.Button)
            {
                //DataGridViewButtonCell myCell = V_Data.Rows[iRow].Cells[iCell] as DataGridViewButtonCell;

                if (action == Actions.Auto)
                {
                    //do nothing
                }
                else DoAction(action, iRow, iCell);
                //if (E_ListRowClicked != null) E_ListRowClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, cell, V_Data, e));
                if (E_CellClicked != null) E_CellClicked(this, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e));
            }
            else if (itemType == ItemTypes.CloseButton)
            {
                // DataGridViewButtonCell myCell = V_Data.Rows[iRow].Cells[iCell] as DataGridViewButtonCell;
                //if (action == Actions.Auto || action == Actions.Nothing)
                {
                    //close Button은 오작동의 우려때문에 다른 이벤트를 처리하지 않는다.

                    CloseBtnClicked(iRow, iCell, e); //close Button은 오작동의 우려때문에 다른 이벤트를 처리하지 않는다.
                }
                //else
                {
                    //    DoAction(action, iRow, iCell);
                    //                        if (E_CellClicked != null) E_CellClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, cell, V_Data, e));
                }

            }
            else if (itemType == ItemTypes.Image)
            {
                // DataGridViewImageCell myCell = V_Data.Rows[iRow].Cells[iCell] as DataGridViewImageCell;

                if (action == Actions.Auto) DoAction(Actions.Nothing, iRow, iCell);
                else DoAction(action, iRow, iCell);
                if (E_CellClicked != null) E_CellClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e));
            }

            else if (itemType == ItemTypes.Header)// header
            {
                if (action == Actions.Auto) DoAction(Actions.Nothing, iRow, iCell);
                else DoAction(action, iRow, -1);
                if (E_CellClicked != null) E_CellClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e));
            }
            else //variaous
            {
                IEasyGridCell myCell = cell as IEasyGridCell;
                doLeftClick(row, cell, iRow, iCell, myCell.ItemType,e);
            }
        }

        public void ShowComboBoxDropDown(DataGridViewComboBoxCell myCell)
        {
            bool isSameBox = (BeforeComboBox == ClickedComboBox) ? true : false;
                    
            if (myCell.ContextMenuStrip != null && myCell.ContextMenuStrip.IsDisposed == false && isSameBox)
            {
                myCell.ContextMenuStrip.Dispose();
                return;
            }
            int x = V_Data.RectangleToScreen(V_Data.Bounds).Left;
            int startCol = V_Data.FirstDisplayedScrollingColumnIndex;
            if (V_Data.RowHeadersVisible) x += V_Data.RowHeadersWidth;
            for (int i = startCol; i <= myCell.ColumnIndex; i++)
            {
                x += Wid[i];
            }
            x -= 20;
            int y = V_Data.RectangleToScreen(V_Data.Bounds).Top;
            if (V_Data.ColumnHeadersVisible) y += V_Data.ColumnHeadersHeight;
            int startRow = V_Data.FirstDisplayedScrollingRowIndex;
            for (int i = startRow; i < myCell.RowIndex; i++)
            {
                y += V_Data.Rows[i].Height;
            }

            int width = Wid[myCell.ColumnIndex];

            if (myCell.ContextMenuStrip == null) myCell.ContextMenuStrip = new ContextMenuStrip();
            myCell.ContextMenuStrip.ShowCheckMargin = false;
            myCell.ContextMenuStrip.ShowImageMargin = false;

            if (myCell.ContextMenuStrip.Items.Count > 0)
            {
                myCell.ContextMenuStrip.Items.Clear();
            }

            for (int i = 0; i < myCell.Items.Count; i++)
            {
                myCell.ContextMenuStrip.Items.Add(myCell.Items[i] as String);
            }
            if (myCell.ContextMenuStrip.Width < width) myCell.ContextMenuStrip.Width = width;
            myCell.ContextMenuStrip.ItemClicked += new ToolStripItemClickedEventHandler(ComboBoxItemContextClicked);

            myCell.ContextMenuStrip.Show(new Point(x, y));//Control.MousePosition);
        }


        public DataGridViewCell Cell(PositionOnList pos)
        {
            return V_Data.Rows[pos.Row].Cells[pos.Col];
        }

        public DataGridViewCell Cell(int row, int col)
        {
            return V_Data.Rows[row].Cells[col];
        }

        void ComboBoxItemContextClicked(object sender, ToolStripItemClickedEventArgs e)
        {

            //MessageBox.Show(ClickedRow + "," + ClickedCol + "/"+e.ClickedItem.Text);
            if (ClickedComboBox.Row < 0 || ClickedComboBox.Col < 0) return;
            DataGridViewComboBoxCell cell = Cell(ClickedComboBox) as DataGridViewComboBoxCell;
            
            int index = cell.Items.IndexOf(e.ClickedItem.Text);//.MergeIndex;
            
            if(cell.Items.Contains(e.ClickedItem.Text)){
                //cell.Value = e.ClickedItem.Text;// cell.Items.IndexOf(value);
                cell.ValueMember = e.ClickedItem.Text;
                cell.Value = e.ClickedItem.Text;

                if (E_ComboBoxChanged != null)
                {
                    //DataGridViewComboBoxCell cell = V_Data.Rows[ComboSelectedRow].Cells[ComboSelectedCol] as DataGridViewComboBoxCell;
                    int selectedIndex = cell.Items.IndexOf(cell.ValueMember);
                    DataGridViewRow row = V_Data.Rows[ClickedComboBox.Row];
                    CellComboBoxEventArgs arg = new CellComboBoxEventArgs(selectedIndex, cell.ValueMember, ClickedComboBox.Row, ClickedComboBox.Col, V_Data, row, cell);
                    E_ComboBoxChanged(this, arg);
                }
            }
            //SetValueInCell(ComboSelectedRow, ComboSelectedCol, e.ClickedItem.Text);
            //cell.ValueMember = e.ClickedItem.Text;
            //cell.Value = e.ClickedItem.Text;

        }


        enum CellRepaintOptions { ImageCheckBoxHeaderChanged = 0, None };
        CellRepaintOptions _cellRePaintOption = CellRepaintOptions.None;
        void V_Data_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            ImageCheckBoxHeaderChanged(e);
            
            if (_cellRePaintOption != CellRepaintOptions.None)
            {
                if (e.ColumnIndex == ClickedCol && e.RowIndex == ClickedRow)
                {
                    switch (_cellRePaintOption)
                    {
                        case CellRepaintOptions.ImageCheckBoxHeaderChanged:
                            try
                            {
                                ImageCheckBoxHeaderChanged(e);
                            }
                            catch { }
                            break;
                    }
                   // _cellRePaintOption = CellRepaintOptions.None;
                }
            }
        }

        void SetCellRepaint(CellRepaintOptions option)
        {
            _cellRePaintOption = option;
            V_Data.Refresh();
        }

        void ImageCheckBoxHeaderChanged(DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex>=0) return;
            if (_itemTypes.Count <= e.ColumnIndex) return;

            if (_itemTypes[e.ColumnIndex] == ItemTypes.ImageCheckBox)
            {
                e.PaintBackground(e.ClipBounds, false);

                DataGridViewImageColumn col = V_Data.Columns[e.ColumnIndex] as DataGridViewImageColumn;
                Image[] imgs = _titleInitData[e.ColumnIndex] as Image[];
                
                int checkState = (int)col.Tag;
                Image img = imgs[checkState];

                Point pt = new Point(((e.CellBounds.Width - img.Width) / 2) + e.CellBounds.Location.X,((e.CellBounds.Height - img.Height)/2)+ e.CellBounds.Location.Y);
                 
                e.Graphics.DrawImage(img,pt);
                e.Handled = true;
            }
        }

        /*
        void V_Data_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (_itemTypes[e.ColumnIndex] == ItemTypes.TextBox)
            {
                if (e.Cancel == false)
                {
                    _originalText = V_Data.Rows[e.RowIndex].Cells[e.ColumnIndex].Value as String;
                    if (_originalText == null) _originalText = "";
                }
            }
        }
         */

        void V_Data_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0) return;
            /*
            if (_itemTypes[e.ColumnIndex] == ItemTypes.ComboBox)
            {
                ComboBoxChanged(e);
            }
             */
        }
        /*
        void ComboBoxChanged(DataGridViewCellEventArgs e)
        {
            if (E_ComboBoxChanged != null)
            {
                DataGridViewComboBoxCell cell = V_Data.Rows[ComboSelectedRow].Cells[ComboSelectedCol] as DataGridViewComboBoxCell;
                int selectedIndex = cell.Items.IndexOf(cell.ValueMember);
                DataGridViewRow row = V_Data.Rows[e.RowIndex];
                CellComboBoxEventArgs arg = new CellComboBoxEventArgs(selectedIndex, cell.ValueMember, e.RowIndex, e.ColumnIndex, V_Data, row, cell);
                E_ComboBoxChanged(this, arg);
            }
        }
         */
        #endregion


        #region /////////////////// public functions////////////////////

        /*
        public void AddTitleColumn(String key, String text, double width, Boolean editable = true)
        {
            _editables.Add(editable);
            _titles.Add(text);
            _wid.Add(width);
            _titleDic.Add(key, text);
            
            DataGridViewColumn col = new DataGridViewTextBoxColumn();
            col.Name = key;
            col.HeaderText = text;

            V_Data.Columns.Add(col);
        }

        public void AddTitleColumn(String textAndName, double width, Boolean editable=true)
        {
            AddTitleColumn(textAndName, textAndName, width, editable);
        }
        */

        #region Suspend, Resume
        public void SuspendLayout()
        {
            V_Data.SuspendLayout();
            base.SuspendLayout();
        }

        public void ResumeLayout()
        {
            V_Data.ResumeLayout();
            base.ResumeLayout();
        }

        public void SuspendAutoScrolling()
        {
            _isAutoScrolling = false;
        }

        public void ResumeAutoScrolling()
        {
            _isAutoScrolling = true;
        }
        #endregion


        #region Adders

        /*
        /// <summary>
        /// 새로운 Row를 추가합니다. 더 많은 기능을 위해서는 AddDataRow나 AddDataRowNow를 사용하십시오.
        /// </summary>
        /// <param name="relativeObjName">이 줄에 연관된 데이터의 이름입니다. 사용할 때는 RowRelativeObject(rowIndex)["데이터이름"] 으로 가져갑니다.</param>
        /// <param name="relativeObj">이 줄에 연관된 데이터입니다. </param>
        /// <param name="isArray">데이터가 배열로 들어갈지를 정합니다. 그러나 동작하지 않으므로 true,false중 아무것이나 넣으십시오.</param>
        /// <param name="values">실제 줄에 추가되는 데이터로서, 각 column에 맞는 형식의 값을 지정해야 합니다.</param>
        public void AddItemWithRelative(String relativeObjName, object relativeObj, params object[] values)
        {
            /*
            if (values == null || values.Length == 0) return;
            if (values != null && values[0] is object[]) values = values[0] as object[];
            if (values != null && values[0] is List<object>) values = (values[0] as List<object>).ToArray();
            
            AddARow(relativeObjName, relativeObj, values, null);
        }
         */

        /// <summary>
        /// 대상 row에 cell을 추가합니다. 주로 새로이 row를 만들어 추가할 때 사용됩니다.
        /// </summary>
        /// <param name="row">대상이 되는 DataGridViewRow</param>
        /// <param name="value">넣어 줄 값</param>
        public void AddItemInRow(DataGridViewRow row, object value, String tooltip = null)
        {
            int colIndex = row.Cells.Count;
            ItemTypes itemType = _itemTypes[colIndex];
            IEasyGridCell cell;
            #region TextBox
            if (itemType == ItemTypes.TextBox)
            {
                cell = new EasyGridTextBoxCell(V_Data);
                (cell as EasyGridTextBoxCell).Value = value;
            }
            #endregion
            #region KeyValue
            if (itemType == ItemTypes.KeyValue)
            {
                cell = new EasyGridKeyValueCell(V_Data);
                (cell as EasyGridKeyValueCell).Value.Clear();
                (cell as EasyGridKeyValueCell).Value.Add(value as Dictionary<String, String>);
            }
            #endregion
            #region CheckBox
            else if (itemType == ItemTypes.CheckBox)
            {
                cell = new EasyGridTextBoxCell(V_Data);
                EasyGridTextBoxCell myCell = cell as EasyGridTextBoxCell;
                if (value.Equals(true)) myCell.Value = 1;
                else if (value.Equals(false)) myCell.Value = 0;
                else if (value.Equals(null)) myCell.Value = 2;
                else if (value.Equals(0) || value.Equals(1) || value.Equals(2))
                {
                    myCell.Value = value;
                }
                else myCell.Value = 0;
            }
            #endregion
            #region ImageCheckBox
            else if (itemType == ItemTypes.ImageCheckBox)
            {
                cell = new EasyGridImageCheckBoxCell(V_Data);
                EasyGridImageCheckBoxCell myCell = cell as EasyGridImageCheckBoxCell;
                if (value is int)
                {
                    DataGridViewImageColumn col = V_Data.Columns[colIndex] as DataGridViewImageColumn;
                    Image[] imageList = _titleInitData[colIndex] as Image[];
                    int intValue = (int)value;

                    if (intValue > imageList.Count())
                    {
                        intValue = intValue % 3; //아무리 높은 값을 넣어도 3으로 나머지해준다.
                    }

                    myCell.Value = imageList[intValue];
                    myCell.Tag = intValue;

                }
                else if (value is bool?)
                {
                    DataGridViewImageColumn col = V_Data.Columns[colIndex] as DataGridViewImageColumn;
                    Image[] imageList = _titleInitData[colIndex] as Image[];
                    bool? boolValue = (bool?)value;
                    int intValue = (boolValue.Equals(true)) ? 1 : (boolValue.Equals(false)) ? 0 : 2;
                    myCell.Value = imageList[intValue];
                    myCell.Tag = intValue;
                }
                else
                {
                    throw new Exception("테이블 [" + V_Data.Name + "]의 입력 멤버타입이 틀립니다. row:" + row.Index + ", cell:" + colIndex + ", trying value:" + (value as String) + "/ 원래 타입:int, bool");
                }

            }
            #endregion
            #region ComboBox
            else if (itemType == ItemTypes.ComboBox)
            {
                cell = new EasyGridComboBoxCell(V_Data);
                //ContextMenuStrip menu = cell.ContextMenuStrip;
                DataGridViewComboBoxCell cCell = cell as DataGridViewComboBoxCell;
                if (value is ICollection<object>)
                {
                    ICollection<object> arr = value as ICollection<object>;
                    int selIndex = -1;
                    //menu.Items.Clear();
                    cCell.Items.Clear();
                    if (arr != null && arr.Count > 1 && arr.ElementAt(0) is int)
                    {
                        selIndex = (int)(arr.ElementAt(0));
                        for (int i = 1; i < arr.Count; i++)
                        {
                            cCell.Items.Add(arr.ElementAt(i));
                            //menu.Items.Add(arr[i].ToString());
                        }
                    }
                    else
                    {
                        foreach (object aLine in arr)
                        {
                            cCell.Items.Add(aLine);
                            //menu.Items.Add(aLine.ToString());
                        }
                    }

                    //cCell.Items.AddRange(value as object[]);
                    if (selIndex >= 0)
                    {
                        cCell.ValueMember = cCell.Items[(int)selIndex] as String;
                        cCell.Value = cCell.Items[(int)selIndex] as String;
                    }
                }
                else if (value is ICollection<String>)
                {
                    ICollection<String> arr = value as ICollection<String>;
                    foreach (String aLine in arr)
                    {
                        cCell.Items.Add(aLine);
                    }
                }
                else if (value is int)
                {
                    DataGridViewComboBoxColumn col = V_Data.Columns[colIndex] as DataGridViewComboBoxColumn;

                    cCell.Items.AddRange(col.Items);
                    try
                    {
                        if ((int)value >= 0)
                        {
                            cCell.ValueMember = cCell.Items[(int)value] as String;
                            cCell.Value = cCell.Items[(int)value] as String;
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception("테이블 [" + V_Data.Name + "]의 입력 멤버타입이 틀립니다. \r\n 이렇게 값을 정의하려면 title 정의시 기본 항목을 지정해야 합니다.\r\n row:" + row.Index + ", cell:" + colIndex + ", trying value:" + (value as String) + "/ 원래 타입:String[],int,string", e);
                    }
                }
                else if (value is String)
                {
                    DataGridViewComboBoxColumn col = V_Data.Columns[colIndex] as DataGridViewComboBoxColumn;

                    cCell.Items.AddRange(col.Items);
                    try
                    {
                        cCell.ValueMember = value as String;
                        cCell.Value = value as String;
                    }
                    catch (Exception e)
                    {
                        throw new Exception("테이블 [" + V_Data.Name + "]의 입력 멤버타입이 틀립니다. \r\n 이렇게 값을 정의하려면 title 정의시 기본 항목을 지정해야 합니다.\r\n row:" + row.Index + ", cell:" + colIndex + ", trying value:" + (value as String) + "/ 원래 타입:String[],int,string", e);
                    }
                }
                else
                {
                    throw new Exception("테이블 [" + V_Data.Name + "]의 입력 멤버타입이 틀립니다. row:" + row.Index + ", cell:" + colIndex + ", trying value:" + (value as String) + "/ 원래 타입:String[],int,string");
                }
            }
            #endregion
            #region RadioButton
            else if (itemType == ItemTypes.RadioButton)
            {
                cell = new EasyGridRadioButtonCell(V_Data);

                //ContextMenuStrip menu = cell.ContextMenuStrip;
                EasyGridRadioButtonCell cCell = cell as EasyGridRadioButtonCell;
                int refCount = cCell.Items.RefCount;
                if (value is String[])
                {
                    String[] arr = value as String[];
                    foreach (String aLine in arr)
                    {
                        cCell.Items.Add(aLine);
                    }
                    cCell.Value = 0;
                }
                else if (value is int)
                {
                    EasyGridRadioButtonColumn col = V_Data.Columns[colIndex] as EasyGridRadioButtonColumn;

                    cCell.Items.Add(col.Items);
                    try
                    {
                        if ((int)value >= 0)
                        {
                            cCell.Value = (int)value;
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception("테이블 [" + V_Data.Name + "]의 입력 멤버타입이 틀립니다. \r\n 이렇게 값을 정의하려면 title 정의시 기본 항목을 지정해야 합니다.\r\n row:" + row.Index + ", cell:" + colIndex + ", trying value:" + (value as String) + "/ 원래 타입:String[],int,string", e);
                    }
                }
                else if (value is String)
                {
                    EasyGridRadioButtonColumn col = V_Data.Columns[colIndex] as EasyGridRadioButtonColumn;

                    cCell.Items.Add(col.Items);

                    for (int i = 0; i < cCell.Items.Count; i++)
                    {
                        if (cCell.Items[i].Text.Equals(value as String))
                        {
                            cCell.Value = i;
                            break;
                        }
                    }

                }
                else
                {
                    throw new Exception("테이블 [" + V_Data.Name + "]의 입력 멤버타입이 틀립니다. row:" + row.Index + ", cell:" + colIndex + ", trying value:" + (value as String) + "/ 원래 타입:String[],int,string");
                }
            }
            #endregion
            #region CheckBoxGroup
            else if (itemType == ItemTypes.CheckBoxGroup)
            {
                cell = new EasyGridCheckBoxGroupCell(V_Data);

                //ContextMenuStrip menu = cell.ContextMenuStrip;
                EasyGridCheckBoxGroupCell cCell = cell as EasyGridCheckBoxGroupCell;
                int refCount = cCell.Items.RefCount;
                if (value is ICollection<String>)
                {
                    ICollection<String> arr = value as ICollection<String>;
                    foreach (String aLine in arr)
                    {
                        cCell.Items.Add(aLine);
                    }
                    cCell.Value = null;
                }
                else if (value is ICollection<int>)
                {
                    EasyGridCheckBoxGroupColumn col = V_Data.Columns[colIndex] as EasyGridCheckBoxGroupColumn;

                    cCell.Items.Add(col.Items);

                    if ((int)value >= 0)
                    {
                        cCell.Value = value as ICollection<int>;
                    }

                }
                else if (value == null)
                {
                    EasyGridCheckBoxGroupColumn col = V_Data.Columns[colIndex] as EasyGridCheckBoxGroupColumn;

                    cCell.Items.Add(col.Items);
                    cCell.Value = null;
                }
                else
                {
                    throw new Exception("테이블 [" + V_Data.Name + "]의 입력 멤버타입이 틀립니다. row:" + row.Index + ", cell:" + colIndex + ", trying value:" + (value as String) + "/ 원래 타입:String[],int[], null");
                }
            }
            #endregion
            #region Button
            else if (itemType == ItemTypes.Button)
            {
                cell = new EasyGridButtonCell(V_Data);
                EasyGridButtonCell myCell = cell as EasyGridButtonCell;
                DataGridViewButtonColumn col = V_Data.Columns[colIndex] as DataGridViewButtonColumn;
                if (value == null)
                {
                    if (col.Text == null)
                    {
                        myCell.Value = "";
                    }
                    else
                    {
                        myCell.Value = col.Text;
                    }
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
                cell = new EasyGridCloseButtonCell(V_Data);
                DataGridViewButtonColumn col = V_Data.Columns[colIndex] as DataGridViewButtonColumn;
                EasyGridCloseButtonCell myCell = cell as EasyGridCloseButtonCell;
                if (value == null)
                {
                    if (col.Text == null)
                    {
                        myCell.Value = "X";
                    }
                    else
                    {
                        myCell.Value = col.Text;
                    }
                }
                else
                {
                    myCell.Value = value;
                }
            }
            #endregion
            #region Image
            else if (itemType == ItemTypes.Image)
            {
                cell = new EasyGridImageCell(V_Data);
                EasyGridImageCell myCell = cell as EasyGridImageCell;
                if (value is Image)
                {
                    myCell.Value = value; //그림을 지정 시 직접 그림을 넣어준다.
                    myCell.Tag = -1;
                }
                else if (value is int)
                {
                    DataGridViewImageColumn col = V_Data.Columns[colIndex] as DataGridViewImageColumn;

                    Image[] imageList = _titleInitData[colIndex] as Image[];
                    try
                    {
                        myCell.Value = imageList[(int)value];
                        myCell.Tag = (int)value;
                    }
                    catch (Exception e)
                    {
                        throw new Exception("테이블 [" + V_Data.Name + "]의 입력 멤버타입이 틀립니다. \r\n 이렇게 값을 정의하려면 title 정의시 Column정의 시 기본 항목을 지정해야 합니다.\r\n row:" + row.Index + ", cell:" + colIndex + ", trying value:" + (value as String) + "/저장된 Image개수:" + imageList.Length + "/ 원래 타입:int, Image", e);
                    }
                }
                else
                {
                    throw new Exception("테이블 [" + V_Data.Name + "]의 입력 멤버타입이 틀립니다. row:" + row.Index + ", cell:" + colIndex + ", trying value:" + (value as String) + "/ 원래 타입:int, Image");
                }
            }
            #endregion
            else //Variables
            {

                if (value is EasyGridVariousTypeCellInfo)
                {
                    cell = getVariousCell(value as EasyGridVariousTypeCellInfo, tooltip);
                }
                else
                {
                    throw new Exception("테이블 [" + V_Data.Name + "]의 입력 멤버타입이 틀립니다. row:" + row.Index + ", cell:" + colIndex + ", trying value:" + (value as String) + "/ 원래 타입:EasyGridVariousTypeCellInfo");
                }
            }
            if (tooltip != null && tooltip.Length > 0) (cell as DataGridViewCell).ToolTipText = tooltip;
            row.Cells.Add(cell as DataGridViewCell);


        }
        public IEasyGridCell getVariousCell(EasyGridVariousTypeCellInfo info, String tooltip = null)
        {
            IEasyGridCell cell;
            ItemTypes itemType = info.ItemType;

            #region TextBox
            if (itemType == ItemTypes.TextBox)
            {
                cell = new EasyGridTextBoxCell(V_Data);
                (cell as EasyGridTextBoxCell).Value = info.Text;
            }
            #endregion
            #region KeyValue
            if (itemType == ItemTypes.KeyValue)
            {
                cell = new EasyGridKeyValueCell(V_Data);
                (cell as EasyGridKeyValueCell).Value.Clear();
                (cell as EasyGridKeyValueCell).Value.Add(info.KeyValue as Dictionary<String, String>);
            }
            #endregion
            #region CheckBox
            else if (itemType == ItemTypes.CheckBox)
            {
                cell = new EasyGridCheckBoxCell(V_Data);
                EasyGridCheckBoxCell myCell = cell as EasyGridCheckBoxCell;
                myCell.Value = info.CheckInt;
            }
            #endregion
            #region ImageCheckBox
            else if (itemType == ItemTypes.ImageCheckBox)
            {
                cell = new EasyGridImageCheckBoxCell(V_Data);
                EasyGridImageCheckBoxCell myCell = cell as EasyGridImageCheckBoxCell;
                if (info.Images != null) myCell.Images = info.Images;
                else myCell.Images = _checkBoxImageList;
                myCell.Value = info.CheckInt;
                
            }
            #endregion
            #region ComboBox
            else if (itemType == ItemTypes.ComboBox)
            {
                cell = new EasyGridComboBoxCell(V_Data);
                //ContextMenuStrip menu = cell.ContextMenuStrip;
                EasyGridComboBoxCell cCell = cell as EasyGridComboBoxCell;
                cCell.Items.AddRange(info.Items);
                cCell.Value = info.SelectedText;
            }
            #endregion
            #region RadioButton
            else if (itemType == ItemTypes.RadioButton)
            {
                cell = new EasyGridRadioButtonCell(V_Data);

                //ContextMenuStrip menu = cell.ContextMenuStrip;
                EasyGridRadioButtonCell cCell = cell as EasyGridRadioButtonCell;
                cCell.Items.Add(info.Items);
                cCell.Value = info.SelectedIndex;
            }
            #endregion
            #region CheckBoxGroup
            else if (itemType == ItemTypes.CheckBoxGroup)
            {
                cell = new EasyGridCheckBoxGroupCell(V_Data);

                //ContextMenuStrip menu = cell.ContextMenuStrip;
                EasyGridCheckBoxGroupCell cCell = cell as EasyGridCheckBoxGroupCell;
                cCell.Items.Add(info.Items);
                cCell.Value = info.SelectedIndices;
            }
            #endregion
            #region Button
            else if (itemType == ItemTypes.Button)
            {
                cell = new EasyGridButtonCell(V_Data);
                EasyGridButtonCell myCell = cell as EasyGridButtonCell;
                myCell.Value = info.Text;
            }
            #endregion
            #region CloseButton
            else if (itemType == ItemTypes.CloseButton)
            {
                cell = new EasyGridCloseButtonCell(V_Data);
                EasyGridCloseButtonCell myCell = cell as EasyGridCloseButtonCell;
                if (info.Text == null || info.Text.Length == 0) myCell.Value = "X";
                else myCell.Value = info.Text;
            }
            #endregion
            #region Image
            else// if (itemType == ItemTypes.Image)
            {
                cell = new EasyGridImageCell(V_Data);
                EasyGridImageCell myCell = cell as EasyGridImageCell;
                myCell.Images = info.Images;
                myCell.Value = info.SelectedIndex;
            }
            #endregion
            return cell;
        }

        /// <summary>
        /// 새로운 Row를 추가합니다. 호환성을 위해서 제공하는 function입니다. AddItem보다 약간 더 빠릅니다.
        /// 툴팁을 추가하는 방법은, values와 tooltip의 항목을 같게 하여 추가하는 것입니다. 그렇지 않으면 제대로 추가되지 않습니다.
        /// 만일 하나의 cell에 여러 항목을 적어넣는 ComboBox나 CheckBox같은 경우에는 values[]의 각 항목은 배열이나
        /// List의 형태를 가지게 하면 됩니다.
        /// </summary>
        /// <param name="relativeObjName">이 줄에 연관된 데이터의 이름입니다. 사용할 때는 RowRelativeObject(rowIndex)["데이터이름"] 으로 가져갑니다.</param>
        /// <param name="relativeObject">이 줄에 연관된 데이터입니다. </param>
        /// <param name="values">실제 줄에 추가되는 데이터로서, 각 column에 맞는 형식의 값을 지정해야 합니다.</param>
        public void AddARow(String relativeObjName, Object relativeObject, ICollection<object> values, ICollection<String> tooltips = null)
        {
            EasyGridRow row = makeRow(values, tooltips);
            
            row.HeaderCell.Value = V_Data.Rows.Count.ToString();//index추가.

            if (relativeObjName != null)
            {
                row.RelativeObject[relativeObjName] = relativeObject;
            }

            V_Data.Rows.Add(row);

        }
 
        /// <summary>
        /// 새로운 Row를 추가합니다. Array형식의 데이터를 차례대로 추가합니다.
        /// </summary>
        /// <param name="relativeObjs"></param>
        /// <param name="values"></param>
        /// <param name="tooltips"></param>
        public void AddARow(Dictionary<String, Object> relativeObjs, ICollection<object> values, ICollection<String> tooltips = null)
        {
            EasyGridRow row = makeRow(values, tooltips);
            
            row.HeaderCell.Value = V_Data.Rows.Count.ToString();//index추가.
            V_Data.Rows.Add(row);

            if (relativeObjs != null)
            {
                for (int i = 0; i < relativeObjs.Count; i++)
                {
                    row.RelativeObject[relativeObjs.Keys.ElementAt(i)] = relativeObjs.Values.ElementAt(i);
                }
            }
        }

        EasyGridRow makeRow(ICollection<object> values, ICollection<String> tooltips = null)
        {
            EasyGridRow row = new EasyGridRow();
            for (int grp = 0; grp < values.Count; grp++)
            {
                if (values.ElementAt(grp) is Array)
                {
                    Array arr = values.ElementAt(grp) as Array;
                    if (tooltips == null) AddItemInRow(row, values.ElementAt(grp));
                    else
                    {
                        if (tooltips.Count > 0) AddItemInRow(row, values.ElementAt(grp), tooltips.ElementAt(grp));
                    }
                    
                }
                else if (values.ElementAt(grp) is ICollection<object>)
                {
                    ICollection<object> list = values.ElementAt(grp) as ICollection<object>;
                    if (tooltips == null) AddItemInRow(row, values.ElementAt(grp));
                    else
                    {
                        if (tooltips.Count > 0) AddItemInRow(row, values.ElementAt(grp), tooltips.ElementAt(grp));
                    }
                   
                }
                else //obj
                {
                    if (tooltips == null) AddItemInRow(row, values.ElementAt(grp));
                    else AddItemInRow(row, values.ElementAt(grp), tooltips.ElementAt(grp));
                }
            }
            return row;
        }

        /// <summary>
        /// 새로운 Row를 추가합니다.
        /// </summary>
        /// <param name="values">실제 줄에 추가되는 데이터로서, 각 column에 맞는 형식의 값을 지정해야 합니다.</param>
        public void AddItem(params object[] values)
        {
            AddARow(null, values, null);
            //AddItemWithRelative(null, null, values);
        }
        

        public void AddARow(ICollection<object> values)
        {
            AddARow(null, values,null);
        }

        #endregion

        #region Getters

        public List<Object> GetAColumnData(int index)
        {
            List<Object> aCol = new List<object>();
            foreach (DataGridViewRow row in Rows)
            {
                aCol.Add(row.Cells[index].Value);
            }

            return aCol;
        }

        public List<Object> GetAColumnData(String name)
        {
            int index = V_Data.Columns[name].Index;
            return GetAColumnData(index);
        }

        public List<Object> GetRowRelativeObject(String name)
        {
            List<Object> objs = new List<object>();
            foreach (EasyGridRow row in V_Data.Rows)
            {
                if (row.RelativeObject.ContainsKey(name) == false) throw new Exception(this.Name+"에는 RowRelativeObject "+name+"이 없습니다.");
                else objs.Add(row.RelativeObject[name]);
            }
            return objs;
        }


        /// <summary>
        /// Index로 접근하여 Row와 연관되어있는 데이터를 가져가고 설정할 수 있습니다.
        /// </summary>
        /// <param name="index">대상 Row의 index</param>
        /// <returns>대상 Row와 연관된 Dictionary형식의 데이터. key로 접근하여 set,get합니다.</returns>
        public Dictionary<String, Object> RowRelativeObject(int index)
        {
            EasyGridRow row = V_Data.Rows[index] as EasyGridRow;
            return row.RelativeObject;
        }

        public bool? isRowChecked(int index)
        {
            int col = FindCheckBoxColumn();
            if (V_Data.Rows[index].Cells[col] is DataGridViewCheckBoxCell)
            {
                DataGridViewCheckBoxCell cell = V_Data.Rows[index].Cells[col] as DataGridViewCheckBoxCell;
                if ((int)cell.Value == 1) return true;
                else if ((int)cell.Value == 0) return false;
            }

            return null;
        }

        public ItemTypes ColumnItemType(int colIndex)
        {
            return _itemTypes[colIndex];
        }

        public int FindCheckBoxColumn()
        {
            for (int i = 0; i < V_Data.Columns.Count; i++)
            { //checkbox인 col 찾기..가장 먼저 나온 column만 유효함.
                if (_itemTypes[i] == ItemTypes.CheckBox || _itemTypes[i] == ItemTypes.ImageCheckBox)
                {
                    return i;
                }

            }
            return -1;
        }

        public String GetTitleText(int colIndex)
        {
            return V_Data.Columns[colIndex].HeaderText;
        }

        public String GetTitleName(int colIndex)
        {
            return V_Data.Columns[colIndex].Name;
        }

        public EasyGridRow Row(int index)
        {
            return V_Data.Rows[index] as EasyGridRow;
        }

        public DataGridViewCellStyle GetCellStyle(int row, int col)
        {
            return V_Data.Rows[row].Cells[col].Style;
        }

        public DataGridViewCell GetCell(int rowIndex, int cellIndex)
        {
            return Rows[rowIndex].Cells[cellIndex];
        }

        public DataGridViewCell GetCell(int rowIndex, String cellName)
        {
            int col = V_Data.Columns[cellName as String].Index;
            return Rows[rowIndex].Cells[col];
        }

        public Object GetValue(int iRow, int iCell)
        {
            DataGridViewRow row = V_Data.Rows[iRow];
            IEasyGridCell thisCell = Cell(iRow, iCell) as IEasyGridCell;
            if (thisCell == null) return Cell(iRow,iCell).Value;

            ItemTypes itemType = (thisCell).ItemType;// _itemTypes[iCell];

            if (itemType == ItemTypes.TextBox)
            {
                EasyGridTextBoxCell cell = row.Cells[iCell] as EasyGridTextBoxCell;
                return cell.Value;
            }
            else if (itemType == ItemTypes.CheckBox)
            {
                EasyGridCheckBoxCell cell = row.Cells[iCell] as EasyGridCheckBoxCell;
                if (cell.Value is int)
                {
                    //return (int)cell.Value;
                    int intValue = (int)cell.Value;
                    return (intValue == 0) ? false : (intValue == 1) ? true : (bool?)null;
                }
                else return cell.Value;
            }
            else if (itemType == ItemTypes.ImageCheckBox)
            {
                EasyGridImageCheckBoxCell cell = row.Cells[iCell] as EasyGridImageCheckBoxCell;
                return cell.IsChecked;
                //return (intValue == 0) ? false : (intValue == 1) ? true : (bool?)null;

            }
            else if (itemType == ItemTypes.ComboBox)
            {
                EasyGridComboBoxCell cell = row.Cells[iCell] as EasyGridComboBoxCell;
                return cell.Value;
            }
            else if (itemType == ItemTypes.RadioButton)
            {
                EasyGridRadioButtonCell cell = row.Cells[iCell] as EasyGridRadioButtonCell;
                return cell.Value;
            }
            else if (itemType == ItemTypes.CheckBoxGroup)
            {
                EasyGridCheckBoxGroupCell cell = row.Cells[iCell] as EasyGridCheckBoxGroupCell;
                return cell.Value;
            }
            else if (itemType == ItemTypes.Button)
            {
                EasyGridButtonCell cell = row.Cells[iCell] as EasyGridButtonCell;
                return cell.Value;
            }
            else if (itemType == ItemTypes.CloseButton)
            {
                EasyGridCloseButtonCell cell = row.Cells[iCell] as EasyGridCloseButtonCell;
                return cell.Value;
            }
            else if (itemType == ItemTypes.Image)
            {
                EasyGridImageCell cell = row.Cells[iCell] as EasyGridImageCell;
                return cell.Value;
                /*
                DataGridViewImageColumn col = V_Data.Columns[iCell] as DataGridViewImageColumn;
                Image[] imageList = _titleInitData[iCell] as Image[];
                int index = -1;
                try
                {
                    index = (int)cell.Tag;
                }
                catch
                {
                    index = -1;
                }
                if (imageList == null) return cell.Value;
                else return index;
                 */
            }
            else
            {
                return null;
            }

        }

        public int GetIndexFromComboBox(int rowIndex, int colIndex)
        {
            DataGridViewComboBoxCell cell = Rows[rowIndex].Cells[colIndex] as DataGridViewComboBoxCell;
            DataGridViewComboBoxColumn col = V_Data.Columns[colIndex] as DataGridViewComboBoxColumn;
            if (cell.Value != null) return cell.Items.IndexOf(cell.Value);
            else return -1;
            //return cell.Value;
        }

        public Dictionary<String, Object> GetValuesDic(int rowIndex)
        {
            Dictionary<String, Object> dic = new Dictionary<string, object>();
            for (int i = 0; i < V_Data.Columns.Count; i++)
            {
                dic[V_Data.Columns[i].Name] = GetValue(rowIndex, i);
            }
            return dic;
        }
        #endregion

        #region Setters

        public void HideColumn(int columnIndex)
        {
            V_Data.Columns[columnIndex].Visible = false;
        }
        public void ShowColumn(int columnIndex)
        {
            V_Data.Columns[columnIndex].Visible = true;
        }

        public void SelectRow(int rowIndex, bool selected)
        {
            V_Data.Rows[rowIndex].Selected = selected;
        }

        public void SelectRow(List<EasyGridRow> rows, bool selected)
        {
            foreach (EasyGridRow row in rows)
            {
                row.Selected = selected;
            }
            //if (rows.Count > 0) V_Data.CurrentCell = rows[0].Cells[V_Data.CurrentCell.ColumnIndex];
        }

        public void SelectCell(int rowIndex, int colIndex, bool selected)
        {
            V_Data.Rows[rowIndex].Cells[colIndex].Selected = selected;
        }

        public void SelectCell(List<DataGridViewCell> cells, bool selected)
        {
            foreach (DataGridViewCell cell in cells)
            {
                cell.Selected = selected;
            }
            //if (cells.Count > 0) V_Data.CurrentCell = cells[0];
        }


        /// <summary>
        /// 대상 cell에 값을 설정합니다.
        /// </summary>
        /// <param name="iRow">대상이 되는 row의 index</param>
        /// <param name="iCell">대상이 되는 cell의 index</param>
        /// <param name="value">cell에 넣어줄 값</param>
        public void SetValueInCell(int iRow, int iCell, object value, String tooltip = null)
        {
            DataGridViewRow row = V_Data.Rows[iRow];
            SetValueInCell(row, iCell, value, tooltip);
        }

        /// <summary>
        /// 대상 cell에 값을 설정합니다.
        /// </summary>
        /// <param name="row">대상이 되는 DataGridViewRow 입니다</param>
        /// <param name="colIndex">Row안에 있는 cell의 index</param>
        /// <param name="value">Cell에 지정해 줄 value입니다.</param>
        public void SetValueInCell(DataGridViewRow row, int colIndex, object value, String tooltip = null)
        {
            ItemTypes itemType = _itemTypes[colIndex];
            SetValueInCell(row, colIndex, value, itemType, tooltip);
        }

        public void SetValueInCell(DataGridViewRow row, int colIndex, object value, ItemTypes itemType, String tooltip = null)
        {
            if (itemType == ItemTypes.TextBox)
            {
                DataGridViewTextBoxCell cell = row.Cells[colIndex] as DataGridViewTextBoxCell;
                cell.Value = value;
            }
            else if (itemType == ItemTypes.CheckBox)
            {
                DataGridViewCheckBoxCell cell = row.Cells[colIndex] as DataGridViewCheckBoxCell;
                if (value is bool?)
                {
                    if (value.Equals(true)) cell.Value = 1;
                    else if (value.Equals(false)) cell.Value = 0;
                    else if (value.Equals(null)) cell.Value = 2;
                }
                else if (value is int)
                {
                    cell.Value = ((int)value) % 3;
                }
                else cell.Value = 0;
            }
            else if (itemType == ItemTypes.ImageCheckBox)
            {

                EasyGridImageCheckBoxCell cell = row.Cells[colIndex] as EasyGridImageCheckBoxCell;
                ICollection<Image> images = cell.Images;

                if (value is int)
                {
                    DataGridViewImageColumn col = V_Data.Columns[colIndex] as DataGridViewImageColumn;
                    ICollection<Image> imageList = (images!=null)? images : _titleInitData[colIndex] as Image[];
                    int intValue = (int)value;

                    if (intValue > imageList.Count())
                    {
                        intValue = intValue % 3; //아무리 높은 값을 넣어도 3으로 나머지해준다.
                    }

                    cell.Value = imageList.ElementAt(intValue);
                    cell.Tag = intValue;

                }
                else if (value is bool?)
                {
                    DataGridViewImageColumn col = V_Data.Columns[colIndex] as DataGridViewImageColumn;
                    Image[] imageList = _titleInitData[colIndex] as Image[];
                    bool? boolValue = (bool?)value;
                    int intValue = (boolValue.Equals(true)) ? 1 : (boolValue.Equals(false)) ? 0 : 2;
                    cell.Value = imageList[intValue];
                    cell.Tag = intValue;
                }
                else
                {
                    throw new Exception("테이블 [" + V_Data.Name + "]의 입력 멤버타입이 틀립니다. row:" + row.Index + ", cell:" + cell.ColumnIndex + ", trying value:" + (value as String) + "/ 원래 타입:int, bool");
                }
            }
            else if (itemType == ItemTypes.ComboBox)
            {
                EasyGridComboBoxCell cell = row.Cells[colIndex] as EasyGridComboBoxCell;
                DataGridViewComboBoxColumn col = V_Data.Columns[colIndex] as DataGridViewComboBoxColumn;

                if (value is object[])
                {
                    object[] arr = value as object[];
                    int selIndex = -1;
                    //ContextMenuStrip menu = cell.ContextMenuStrip;
                    //menu.Items.Clear();
                    cell.Items.Clear(); //기존 것을 지운다.
                    if (arr != null && arr.Length > 0 && arr[0] is int)
                    {
                        for (int i = 1; i < arr.Length; i++)
                        {
                            cell.Items.Add(arr[i]);
                            // menu.Items.Add(arr[i].ToString());
                        }
                        selIndex = (int)arr[0];

                    }
                    else
                    {
                        foreach (object aLine in arr)
                        {
                            cell.Items.Add(aLine);
                            // menu.Items.Add(aLine.ToString());
                        }
                        selIndex = -1;
                    }

                    if (selIndex >= 0 && arr.Length > selIndex)
                    {
                        if (cell.Items[selIndex] != null)
                        {
                            cell.ValueMember = cell.Items[selIndex].ToString();
                            cell.Value = cell.Items[selIndex].ToString();
                        }
                    }
                    //cell.Items.AddRange(value);
                }
                else if (value is String[])
                {
                    String[] arr = value as String[];

                    ContextMenuStrip menu = cell.ContextMenuStrip;
                    menu.Items.Clear();
                    cell.Items.Clear(); //기존 것을 지운다.

                    foreach (String aLine in arr)
                    {
                        cell.Items.Add(aLine);
                        menu.Items.Add(aLine);
                    }
                    if (cell.Items.Count > 0) cell.Value = cell.Items[0];
                }
                else if (value is int)
                {
                    if (cell.Items == null || cell.Items.Count == 0) cell.Items.AddRange(col.Items);

                    try
                    {
                        cell.ValueMember = cell.Items[(int)value] as String;
                        cell.Value = value;// cell.Items[(int)value];
                        V_Data.Refresh();
                    }
                    catch (Exception e)
                    {
                        throw new Exception("테이블 [" + V_Data.Name + "]의 입력 멤버타입이 틀립니다. \r\n 이렇게 값을 정의하려면 title 정의시 기본 항목을 지정해야 합니다.\r\n row:" + row.Index + ", cell:" + cell.ColumnIndex + ", trying value:" + (value as String) + "/ 원래 타입:String[],int,string", e);
                    }
                }
                else if (value is String)
                {
                    if (cell.Items == null || cell.Items.Count == 0) cell.Items.AddRange(col.Items);

                    try
                    {
                        if (cell.Items.Contains(value))
                        {
                            cell.ValueMember = value as String;
                            cell.Value = value;// cell.Items.IndexOf(value);
                            V_Data.Refresh();
                        }
                        else
                        {
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception("테이블 [" + V_Data.Name + "]의 입력 멤버타입이 틀립니다. \r\n 이렇게 값을 정의하려면 title 정의시 기본 항목을 지정해야 합니다.\r\n row:" + row.Index + ", cell:" + cell.ColumnIndex + ", trying value:" + (value as String) + "/ 원래 타입:String[],int,string", e);
                    }
                }
                else
                {
                    throw new Exception("테이블 [" + V_Data.Name + "]의 입력 멤버타입이 틀립니다. row:" + row.Index + ", cell:" + cell.ColumnIndex + ", trying value:" + (value as String) + "/ 원래 타입:String[],int,string");
                }
            }
            else if (itemType == ItemTypes.RadioButton)
            {
                EasyGridRadioButtonCell cell = row.Cells[colIndex] as EasyGridRadioButtonCell;
                EasyGridRadioButtonColumn col = V_Data.Columns[colIndex] as EasyGridRadioButtonColumn;

                if (value is ICollection<String>)
                {
                    String[] arr = value as String[];
                    cell.Items.Clear(); //기존 것을 지운다.

                    foreach (String aLine in arr)
                    {
                        cell.Items.Add(aLine);
                    }
                    if (cell.Items.Count > 0) cell.Value = 0;
                }
                else if (value is int)
                {
                    if (cell.Items == null || cell.Items.Count == 0) cell.Items.Add(col.Items);
                    cell.Value = (int)value;// cell.Items[(int)value];
                }
                else if (value is String)
                {
                    if (cell.Items == null || cell.Items.Count == 0) cell.Items.Add(col.Items);
                    cell.Value = cell.Items.IndexOf(value as String);
                }
                else
                {
                    throw new Exception("테이블 [" + V_Data.Name + "]의 입력 멤버타입이 틀립니다. row:" + row.Index + ", cell:" + cell.ColumnIndex + ", trying value:" + (value as String) + "/ 원래 타입:String[],int,string");
                }
            }
            else if (itemType == ItemTypes.CheckBoxGroup)
            {
                EasyGridCheckBoxGroupCell cell = row.Cells[colIndex] as EasyGridCheckBoxGroupCell;
                EasyGridCheckBoxGroupColumn col = V_Data.Columns[colIndex] as EasyGridCheckBoxGroupColumn;

                if (value is ICollection<String>)
                {
                    String[] arr = value as String[];
                    cell.Items.Clear(); //기존 것을 지운다.

                    foreach (String aLine in arr)
                    {
                        cell.Items.Add(aLine);
                    }
                    if (cell.Items.Count > 0) cell.Value = null;
                }
                else if (value is ICollection<int>)
                {
                    if (cell.Items == null || cell.Items.Count == 0) cell.Items.Add(col.Items);
                    cell.Value = value as ICollection<int>;// cell.Items[(int)value];
                }
                else
                {
                    throw new Exception("테이블 [" + V_Data.Name + "]의 입력 멤버타입이 틀립니다. row:" + row.Index + ", cell:" + cell.ColumnIndex + ", trying value:" + (value as String) + "/ 원래 타입:String[],int[]");
                }
            }
            else if (itemType == ItemTypes.Various)
            {

                IEasyGridCell mycell = row.Cells[colIndex] as IEasyGridCell;
                SetValueInCell(row, colIndex, value, mycell.ItemType, tooltip);
            }
            else if (itemType == ItemTypes.Button)
            {
                EasyGridButtonCell cell = row.Cells[colIndex] as EasyGridButtonCell;
                DataGridViewButtonColumn col = V_Data.Columns[colIndex] as DataGridViewButtonColumn;
                if (value == null)
                {
                    cell.Value = col.Text;
                }
                else
                {
                    cell.Value = value;
                }
            }
            else if (itemType == ItemTypes.CloseButton)
            {
                EasyGridCloseButtonCell cell = row.Cells[colIndex] as EasyGridCloseButtonCell;
                cell.Value = (value != null) ? value : "X";
            }
            else if (itemType == ItemTypes.Image)
            {
                EasyGridImageCell cell = row.Cells[colIndex] as EasyGridImageCell;
                if (value is Image)
                {
                    cell.Value = value; //그림을 지정 시 직접 그림을 넣어준다.
                }
                else if (value is int)
                {
                    DataGridViewImageColumn col = V_Data.Columns[colIndex] as DataGridViewImageColumn;
                    Image[] imageList = _titleInitData[colIndex] as Image[];
                    try
                    {
                        cell.Value = imageList[(int)value];
                        cell.Tag = (int)value;
                    }
                    catch (Exception e)
                    {
                        throw new Exception("테이블 [" + V_Data.Name + "]의 입력 멤버타입이 틀립니다. \r\n 이렇게 값을 정의하려면 title 정의시 Column정의 시 기본 항목을 지정해야 합니다.\r\n row:" + row.Index + ", cell:" + cell.ColumnIndex + ", trying value:" + (value as String) + "/저장된 Image개수:" + imageList.Length + "/ 원래 타입:int, Image", e);
                    }
                }
                else
                {
                    throw new Exception("테이블 [" + V_Data.Name + "]의 입력 멤버타입이 틀립니다. row:" + row.Index + ", cell:" + cell.ColumnIndex + ", trying value:" + (value as String) + "/ 원래 타입:int, Image");
                }
            }

            if (tooltip != null && tooltip.Length > 0)
            {
                DataGridViewCell cell = row.Cells[colIndex];
                cell.ToolTipText = tooltip;
            }
        }
        public void SetCellStyle(int row, int col, DataGridViewCellStyle style)
        {
            V_Data.Rows[row].Cells[col].Style = style;
        }

        #endregion

        #region AddTitleFunctions

        public void AddTitleCheckBoxColumn(int wid, String columnName, Boolean threeState = false, Actions actionOnClick = Actions.CommonAction, Actions actionOnDoubleClick = Actions.Auto, Actions actionOnRightClick = Actions.Auto)
        {
            DataGridViewCheckBoxColumn col = new DataGridViewCheckBoxColumn(threeState);
            if (wid < 0)
            {
                wid = 15;
            }
            col.Name = columnName;
            col.HeaderText = "";

            col.Width = wid;
            _isThreeState.Add(threeState);
            col.ThreeState = threeState;

            col.TrueValue = 1;
            col.FalseValue = 0;
            col.IndeterminateValue = 2;
            col.Tag = 0;

            V_Data.Columns.Add(col);

            _itemTypes.Add(ItemTypes.CheckBox);
            _editables.Add(false);
            _titleInitData.Add(0);

            _columnActionOnClicked.Add(actionOnClick);
            _columnActionOnDoubleClicked.Add(actionOnDoubleClick);
            _columnActionOnRightClicked.Add(actionOnRightClick);

        }

        public enum CheckStyle { Red, Blue };

        public void AddTitleImageCheckColumn(int wid, String columnName, Boolean threeState, Image[] imageLists, Actions actionOnClick = Actions.CommonAction, Actions actionOnDoubleClick = Actions.Auto, Actions actionOnRightClick = Actions.Auto)
        {

            DataGridViewImageColumn col = new DataGridViewImageColumn();
            if (wid < 0)
            {
                if (imageLists != null && imageLists.Length > 0)
                {
                    wid = imageLists[0].Width;
                }
                else
                {
                    wid = 2;
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
            }
            col.Name = columnName;
            //col.HeaderText = TitleText;
            col.Width = wid;

            if (imageLists == null || imageLists.Length < 3) //만일 리스트의 Image가 잘못되었거나 list가 null일때..
            {
                List<Image> newList = new List<Image>();
                newList.Add((imageLists == null || imageLists.Length < 1 || imageLists[0] == null) ? Properties.Resources.check_normal : imageLists[0]);
                newList.Add((imageLists == null || imageLists.Length < 2 || imageLists[1] == null) ? Properties.Resources.check_red : imageLists[1]);
                newList.Add((imageLists == null || imageLists.Length < 3 || imageLists[2] == null) ? Properties.Resources.check_inter : imageLists[2]);
                imageLists = newList.ToArray(); //새로 만든 리스트와 교체..
            }

            col.HeaderCell.Value = imageLists[0]; //기본 이미지로 title 이미지를 교체..
            _titleInitData.Add(imageLists);
            col.Tag = 0;
            //col.HeaderText = "";
            col.Image = imageLists[0];
            Bitmap btm = new Bitmap(imageLists[0]);

            col.Image = btm;// Icon.FromHandle(btm.GetHicon());
            V_Data.Columns.Add(col);

            _itemTypes.Add(ItemTypes.ImageCheckBox);
            _isThreeState.Add(threeState);
            _editables.Add(false);

            _columnActionOnClicked.Add(actionOnClick);
            _columnActionOnDoubleClicked.Add(actionOnDoubleClick);
            _columnActionOnRightClicked.Add(actionOnRightClick);

            SetCellRepaint(CellRepaintOptions.ImageCheckBoxHeaderChanged);
        }

        public void AddTitleImageCheckColumn(int wid, String columnName, Boolean threeState = false, CheckStyle checkStyle = CheckStyle.Red, Actions actionOnClick = Actions.CommonAction, Actions actionOnDoubleClick = Actions.Auto, Actions actionOnRightClick = Actions.Auto)
        {
            Image[] list;
            if (checkStyle == CheckStyle.Blue)
            {
                list = new Image[]{
                    Properties.Resources.check_normal,
                    Properties.Resources.check_blue,
                    Properties.Resources.check_inter_press};
            }
            else
            {
                list = new Image[]{
                    Properties.Resources.check_normal,
                    Properties.Resources.check_red,
                    Properties.Resources.check_inter_press};
            }

            AddTitleImageCheckColumn(wid, columnName, threeState, list, actionOnClick, actionOnDoubleClick, actionOnRightClick);
        }

        public void AddTitleComboBoxColumn(int wid, String columnName, String TitleText, object[] items, Actions actionOnClick = Actions.CommonAction, Actions actionOnDoubleClick = Actions.Auto, Actions actionOnRightClick = Actions.Auto)
        {
            DataGridViewComboBoxColumn col = new DataGridViewComboBoxColumn();
            col.FlatStyle = FlatStyle.Popup;
            col.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            if (wid < 0)
            {
                wid = 2;
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            col.Name = columnName;
            col.HeaderText = TitleText;
            col.Width = wid;
            if (items != null)
            {
                foreach (String txt in items)
                {
                    col.Items.Add(txt);
                }
            }
            col.MaxDropDownItems = 100;

            col.DropDownWidth = 100;
            V_Data.Columns.Add(col);

            _itemTypes.Add(ItemTypes.ComboBox);
            _editables.Add(false);
            _titleInitData.Add(0);
            _isThreeState.Add(false);
            _columnActionOnClicked.Add(actionOnClick);
            _columnActionOnDoubleClicked.Add(actionOnDoubleClick);
            _columnActionOnRightClicked.Add(actionOnRightClick);

        }

        public void AddTitleRadioButtonColumn(int wid, String columnName, String TitleText, String[] items, int initSelected=0, Actions actionOnClick = Actions.CommonAction, Actions actionOnDoubleClick = Actions.Auto, Actions actionOnRightClick = Actions.Auto)
        {
            EasyGridRadioButtonColumn col = new EasyGridRadioButtonColumn();
            
            if (wid < 0)
            {
                wid = 2;
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            col.Name = columnName;
            col.HeaderText = TitleText;
            col.Width = wid;
            
            if (items != null)
            {
                col.Items.Add(items);
            }
            col.Items.CheckItem(initSelected);

            V_Data.Columns.Add(col);

            _itemTypes.Add(ItemTypes.RadioButton);
            _editables.Add(null);
            _titleInitData.Add(initSelected);
            _isThreeState.Add(false);
            
            _columnActionOnClicked.Add(actionOnClick);
            _columnActionOnDoubleClicked.Add(actionOnDoubleClick);
            _columnActionOnRightClicked.Add(actionOnRightClick);

        }

        public void AddTitleCheckBoxGroupColumn(int wid, String columnName, String TitleText, String[] items, int[] initSelected = null, Actions actionOnClick = Actions.CommonAction, Actions actionOnDoubleClick = Actions.Auto, Actions actionOnRightClick = Actions.Auto)
        {
            EasyGridCheckBoxGroupColumn col = new EasyGridCheckBoxGroupColumn();

            if (wid < 0)
            {
                wid = 2;
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            col.Name = columnName;
            col.HeaderText = TitleText;
            col.Width = wid;

            if (items != null)
            {
                col.Items.Add(items);
            }
            col.Items.CheckItem(initSelected);

            V_Data.Columns.Add(col);

            _itemTypes.Add(ItemTypes.CheckBoxGroup);
            _editables.Add(null);
            _titleInitData.Add(initSelected);
            _isThreeState.Add(false);

            _columnActionOnClicked.Add(actionOnClick);
            _columnActionOnDoubleClicked.Add(actionOnDoubleClick);
            _columnActionOnRightClicked.Add(actionOnRightClick);

        }

        public void AddTitleImageColumn(int wid, String columnName, String TitleText, String[] imagePaths, int titleShowImage = -1, Actions actionOnClick = Actions.CommonAction, Actions actionOnDoubleClick = Actions.CommonAction, Actions actionOnRightClick = Actions.CommonAction)
        {
            List<Image> imageLists = new List<Image>();
            for (int i = 0; i < imagePaths.Length; i++)
            {
                Image image = Image.FromFile(imagePaths[i]);
                imageLists.Add(image);
            }
            AddTitleImageColumn(wid, columnName, TitleText, imageLists.ToArray(), titleShowImage, actionOnClick, actionOnDoubleClick, actionOnRightClick);
        }

        public void AddTitleImageColumn(int wid, String columnName, String TitleText, ImageList.ImageCollection imageCollection, int titleShowImage = -1, Actions actionOnClick = Actions.CommonAction, Actions actionOnDoubleClick = Actions.CommonAction, Actions actionOnRightClick = Actions.CommonAction)
        {
            List<Image> imageLists = new List<Image>();
            foreach (Image image in imageCollection)
            {
                imageLists.Add(image);
            }
            AddTitleImageColumn(wid, columnName, TitleText, imageLists.ToArray(), titleShowImage, actionOnClick, actionOnDoubleClick, actionOnRightClick);
        }

        public void AddTitleImageColumn(int wid, String columnName, String TitleText, Image[] imageLists = null, int titleShowImage = -1, Actions actionOnClick = Actions.CommonAction, Actions actionOnDoubleClick = Actions.CommonAction, Actions actionOnRightClick = Actions.CommonAction)
        {
            DataGridViewImageColumn col = new DataGridViewImageColumn();
            if (wid < 0)
            {
                if (imageLists != null && imageLists.Length > 0)
                {
                    wid = imageLists[0].Width;
                }
                else
                {
                    wid = 2;
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }

            }
            col.Name = columnName;
            col.HeaderText = TitleText;
            col.Width = wid;
            col.Tag = (imageLists == null || imageLists.Length == 0 || titleShowImage < 0) ? -1 : 0;
            if (titleShowImage >= 0 && imageLists.Length > titleShowImage) col.HeaderCell.Value = imageLists[titleShowImage];
            _titleInitData.Add(imageLists);

            V_Data.Columns.Add(col);

            _itemTypes.Add(ItemTypes.Image);

            _editables.Add(false);
            _isThreeState.Add(false);
            _columnActionOnClicked.Add(actionOnClick);
            _columnActionOnDoubleClicked.Add(actionOnDoubleClick);
            _columnActionOnRightClicked.Add(actionOnRightClick);

        }

        public void AddTitleButtonColumn(int wid, String columnName, String TitleText, bool useTitleText = false, String baseText = "", Actions actionOnClick = Actions.CommonAction, Actions actionOnDoubleClick = Actions.CommonAction, Actions actionOnRightClick = Actions.CommonAction)
        {
            DataGridViewButtonColumn col = new DataGridViewButtonColumn();
            if (wid < 0)
            {
                wid = 2;
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            col.Name = columnName;
            col.HeaderText = TitleText;
            col.Width = wid;
            col.UseColumnTextForButtonValue = useTitleText;
            col.Text = baseText;

            col.SortMode = DataGridViewColumnSortMode.NotSortable;

            V_Data.Columns.Add(col);

            _itemTypes.Add(ItemTypes.Button);
            _editables.Add(false);
            _titleInitData.Add(TitleText);
            _isThreeState.Add(false);
            _columnActionOnClicked.Add(actionOnClick);
            _columnActionOnDoubleClicked.Add(actionOnDoubleClick);
            _columnActionOnRightClicked.Add(actionOnRightClick);

        }

        public void AddTitleCloseButtonColumn(int wid, String columnName, String TitleText, bool useTitleText = false, String baseText = "X", Actions actionOnClick = Actions.CommonAction, Actions actionOnDoubleClick = Actions.CommonAction, Actions actionOnRightClick = Actions.CommonAction)
        {
            DataGridViewButtonColumn col = new DataGridViewButtonColumn();
            if (wid < 0)
            {
                wid = 2;
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            col.Name = columnName;
            col.HeaderText = TitleText;
            col.Width = wid;
            col.UseColumnTextForButtonValue = useTitleText;
            col.Text = baseText;
            col.SortMode = DataGridViewColumnSortMode.NotSortable;


            V_Data.Columns.Add(col);

            _itemTypes.Add(ItemTypes.CloseButton);
            _titleInitData.Add(TitleText);
            _editables.Add(false);
            _isThreeState.Add(false);
            _columnActionOnClicked.Add(actionOnClick);
            _columnActionOnDoubleClicked.Add(actionOnDoubleClick);
            _columnActionOnRightClicked.Add(actionOnRightClick);

        }
        /*
        public void AddTitleIndexColumn(int wid)
        {
            DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();

            if (wid < 0)
            {
                wid = 2;
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            col.SortMode = DataGridViewColumnSortMode.Programmatic;
            col.Name = "row_index";
            col.HeaderText = "no";
            col.Width = wid;
            V_Data.Columns.Add(col);
        }
        */

        public void AddTitleVariousColumn(int wid, String columnName, String TitleText, Actions actionOnClick = Actions.CommonAction, Actions actionOnDoubleClick = Actions.CommonAction, Actions actionOnRightClick = Actions.CommonAction)
        {

            DataGridViewColumn col = new DataGridViewColumn();

            if (wid < 0)
            {
                wid = 2;
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            else if (wid == 0)
            {
                col.Visible = false;
            }
            col.SortMode = DataGridViewColumnSortMode.Programmatic;

            col.Name = columnName;
            col.HeaderText = TitleText;
            col.Width = wid;
            V_Data.Columns.Add(col);

            _itemTypes.Add(ItemTypes.Various);
            _titleInitData.Add(TitleText);
            _editables.Add(null);//null은 cell의 설정에 따라 바뀜을 의미함..
            _isThreeState.Add(false);
            _columnActionOnClicked.Add(actionOnClick);
            if ( actionOnDoubleClick == Actions.CommonAction) _columnActionOnDoubleClicked.Add(Actions.Modify);
            else _columnActionOnDoubleClicked.Add(actionOnDoubleClick);
            _columnActionOnRightClicked.Add(actionOnRightClick);
        }

         public void AddTitleTextBoxColumn(int wid, String columnName, String TitleText, Boolean editable, bool isAutoSort=false, Actions actionOnClick = Actions.CommonAction, Actions actionOnDoubleClick = Actions.CommonAction, Actions actionOnRightClick = Actions.CommonAction)
        {

            DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();

            if (wid < 0)
            {
                wid = 2;
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            else if (wid == 0)
            {
                col.Visible = false;
            }
            col.SortMode = (isAutoSort)? DataGridViewColumnSortMode.Automatic : DataGridViewColumnSortMode.Programmatic;

            col.Name = columnName;
            col.HeaderText = TitleText;
            col.Width = wid;
            V_Data.Columns.Add(col);

            _itemTypes.Add(ItemTypes.TextBox);
            _titleInitData.Add(TitleText);
            _editables.Add(editable);
            _isThreeState.Add(false);
            _columnActionOnClicked.Add(actionOnClick);
            if (editable == true && actionOnDoubleClick == Actions.CommonAction) _columnActionOnDoubleClicked.Add(Actions.Modify);
            else _columnActionOnDoubleClicked.Add(actionOnDoubleClick);
            _columnActionOnRightClicked.Add(actionOnRightClick);
        }
        #endregion

        #region Property Changers
        public void HideTitleBar()
        {
            V_Data.ColumnHeadersVisible = false;
        }

        public void ShowTitleBar()
        {
            V_Data.ColumnHeadersVisible = true;
        }


        public DataGridViewSelectionMode SelectionMode
        {
            get
            {
                return V_Data.SelectionMode;
            }
            set
            {
                V_Data.SelectionMode = value;
            }
        }
        #endregion

        #region Doing Action, Remover

        public void ClearTitles()
        {
            Header.Clear();
            _columnActionOnClicked.Clear();
            _columnActionOnDoubleClicked.Clear();
            _columnActionOnRightClicked.Clear();
        }

        public void ClearData()
        {
            V_Data.Rows.Clear();
            //_rowRelativeObject.Clear();

        }


        /// <summary>
        /// 한 줄을 지웁니다. refreshNow에 true를 적으면 index가 바로 갱신됩니다. 나중에 갱신하려면 RefreshList()를 호출하십시오.
        /// </summary>
        /// <param name="rowIndex">지울 row의 index</param>
        /// <param name="refreshNow">지금 당장 index를 갱신할 것인지를 묻습니다.</param>
        public void RemoveARow(int rowIndex, bool refreshNow = false)
        {
            //_rowRelativeObject.RemoveAt(rowIndex);
            V_Data.Rows.RemoveAt(rowIndex);

            if (refreshNow)
            {
                RefreshList();
            }
        }

        public void RemoveARow(DataGridViewRow row, bool refreshNow = false)
        {
            //_rowRelativeObject.RemoveAt(rowIndex);

            V_Data.Rows.Remove(row);


            if (refreshNow)
            {
                RefreshList();
            }
        }

        public void BeginModifyMode(int rowIndex, int colIndex)
        {
            DataGridViewTextBoxCell cell = V_Data.Rows[rowIndex].Cells[colIndex] as DataGridViewTextBoxCell;
            if (cell != null)
            {
                V_Data.CurrentCell = cell;
                V_Data.BeginEdit(true);

            }
        }
         
        public void DoAction(Actions action, int rowIndex, int colIndex)
        {
            if (rowIndex < 0) return;

            DataGridViewRow row = V_Data.Rows[rowIndex];

            switch (action)
            {
                case Actions.CheckBoxChecked:
                    int col = FindCheckBoxColumn();
                    if (col < 0) return;//라인 내에 체크박스가 없다.
                    //DataGridViewCheckBoxCell checkCell = V_Data.Rows[rowIndex].Cells[col] as DataGridViewCheckBoxCell;
                    //DataGridViewCheckBoxColumn column = V_Data.Columns[col] as DataGridViewCheckBoxColumn;
                    bool now = (bool)GetValue(row.Index, col);
                    SetValueInCell(row, col, !now);
                    break;
                case Actions.ContextMenu:

                    U_ContextMenu.Show(this, this.PointToClient(Control.MousePosition));

                    break;
                case Actions.Modify:
                    if (colIndex < 0) return;
                    BeginModifyMode(rowIndex, colIndex);
                    break;
                case Actions.Nothing:
                    break;
                case Actions.CopyToClipBoard:
                    if (_itemTypes[colIndex] == ItemTypes.TextBox)
                    {
                        DataGridViewTextBoxCell cell = V_Data.Rows[rowIndex].Cells[colIndex] as DataGridViewTextBoxCell;
                        if(cell!=null) Clipboard.SetText(cell.Value as String);
                        
                    }
                    break;
            }
        }

        public void ReleaseSelection()
        {
            V_Data.ClearSelection();
        }

        public void ReleaseAllChecked(int checkBoxColumn)
        {
            if (_itemTypes[checkBoxColumn] == ItemTypes.CheckBox || _itemTypes[checkBoxColumn] == ItemTypes.ImageCheckBox)
            {
                SuspendLayout();
                for (int i = 0; i < V_Data.Rows.Count; i++)
                {
                    SetValueInCell(i, checkBoxColumn, false);
                }
                ResumeLayout();
                RefreshList(false);
            }
            else
            {
                throw new Exception("해당 열은 ImageCheckBox나 CheckBox가 아닙니다. 열:"+checkBoxColumn +" EasyGridView:"+this.Name);
            }
        }

        /// <summary>
        /// 지금 당장 List를 갱신합니다. index가 재조정되고, selection이 사라집니다.
        /// </summary>
        /// <param name="releaseSelection">selection을 사라지게 할 것인지 묻습니다.</param>
        public void RefreshList(bool releaseSelection = false)
        {
            if (releaseSelection) ReleaseSelection();

            resetIndex();
            for (int i = 0; i < _itemTypes.Count; i++)
            {
                if (_itemTypes[i] == ItemTypes.ImageCheckBox)
                {
                    checkIfAllChecked(i);
                }
            }

            V_Data.Refresh();
        }


        delegate void VoidFunc();

        /// <summary>
        /// 리스트의 index를 다시 설정합니다.
        /// </summary>
        public void resetIndex()
        {
            if (InvokeRequired)
            {
                VoidFunc func = resetIndex;
                this.Invoke(func);
            }
            else
            {
                int i = 1;
                foreach (DataGridViewRow row in V_Data.Rows)
                {
                    if (row.HeaderCell != null && row.HeaderCell.Value != null)
                    {
                        if (row.HeaderCell.Value.Equals(i.ToString()) == false) row.HeaderCell.Value = i.ToString();
                    }
                    i++;
                }
            }
        }


        #endregion

        #region ContextMenu
        ContextMenu _contextMenu = new ContextMenu();
        //public List<MenuItem> ContextMenuItems = new List<MenuItem>();
        
        Dictionary<EasyGridMenuItem, EasyGridMenuClickHandler> _contextMenuClickHandlers = new Dictionary<EasyGridMenuItem, EasyGridMenuClickHandler>();

        public MenuItem AddContextMenuItem(String text, EasyGridMenuClickHandler eventHandler = null, ContextMenu parent = null)
        {
            if (parent == null) parent = U_ContextMenu;
            EasyGridMenuItem item = new EasyGridMenuItem(text, new EventHandler(item_Click));
            //ContextMenuItems.Add(item);
            //item.Text = text;
            
            //item.Select = new EventHandler(item_Click);
            if (eventHandler != null && E_ContextMenuClicked != null) _contextMenuClickHandlers.Add(item, eventHandler);

            parent.MenuItems.Add(item);
            return item;
        }
        public void AddContextMenuItem(EasyGridMenuItem item, EasyGridMenuClickHandler eventHandler = null, ContextMenu parent = null)
        {
            if (parent == null) parent = U_ContextMenu;
            item.SetClickedEvent(new EventHandler(item_Click), true);
            if (eventHandler != null && E_ContextMenuClicked != null) _contextMenuClickHandlers.Add(item, eventHandler);
            parent.MenuItems.Add(item);
        }

        public MenuItem AddContextMenuItem(String text, EasyGridMenuItem parent)
        {
            EasyGridMenuItem item = new EasyGridMenuItem(text, new EventHandler(item_Click));
            //ContextMenuItems.Add(item);
            if (parent == null)
            {
                U_ContextMenu.MenuItems.Add(item);
            }
            else
            {
                parent.MenuItems.Add(item);
            }
            return item;
        }

        public void AddContextMenuItem(String[] itemText)
        {
            U_ContextMenu.MenuItems.Clear();
            for (int i = 0; i < itemText.Length; i++)
            {
                AddContextMenuItem(itemText[i], null);
            }
        }

        public void ClearContextMenuItems()
        {
            U_ContextMenu.MenuItems.Clear();

        }

        void item_Click(object sender, EventArgs e)
        {
            EasyGridMenuItem item = sender as EasyGridMenuItem;
            int index;
            if (item.Depth == 0)
                index = U_ContextMenu.MenuItems.IndexOf(item);// ContextMenuItems.IndexOf(item);
            else
                index = item.Parent.MenuItems.IndexOf(item);

            if (_contextMenuClickHandlers.ContainsKey(item)) _contextMenuClickHandlers[item].Invoke(this, new EasyGridMenuClickArgs( item.Text.ToString(), index, ClickedRow, ClickedCol, item, item.Depth));
            else E_ContextMenuClicked(this, new EasyGridMenuClickArgs(item.Text.ToString(), index, ClickedRow, ClickedCol, item, item.Depth));
            
        }
        #endregion

        #region RowMove functions
        public void MoveRowUp(EasyGridRow row, int howManyLines = 1)
        {

            int index = row.Index;
            int gotoIndex = index - howManyLines;

            MoveRowTo(row, gotoIndex);
        }

        public void MoveRowsUp(List<EasyGridRow> rows, int howManyLines = 1, bool goTogether = true)
        {
            if (rows.Count == 0) return;
            if (howManyLines < 0)
            {
                MoveRowsDown(rows, howManyLines * -1, goTogether);
                return;
            }
            if (rows.Count == 0) return;
            int ColumnIndex = V_Data.CurrentCell.ColumnIndex;
            //V_Data.ClearSelection();

            if (goTogether) //가장 첫 열과 함께 이동한다.
            {
                Comparison<EasyGridRow> com = new Comparison<EasyGridRow>(comp);
                rows.Sort(com);
                int index = rows[0].Index;
                int gotoIndex = index - howManyLines;
                MoveRowsTo(rows, gotoIndex);
            }
            else //각자 위로 이동하다가 맨 마지막에 합쳐진다.
            {
                Comparison<EasyGridRow> com = new Comparison<EasyGridRow>(comp);
                rows.Sort(com);
                int index = rows[0].Index;
                int gotoIndex = index - howManyLines;

                int beforeScrollOffset = index - V_Data.FirstDisplayedScrollingRowIndex;
                int nextScroll = gotoIndex - beforeScrollOffset;
                nextScroll = (nextScroll < 0) ? 0 : (nextScroll > V_Data.Rows.Count - 10) ? V_Data.Rows.Count - 10 : nextScroll;
                V_Data.FirstDisplayedScrollingRowIndex = nextScroll;

                
                SuspendAutoScrolling();
                Dictionary<EasyGridRow, bool> isSelected = new Dictionary<EasyGridRow, bool>();
                
                int rowIndex = -1;
                
                V_Data.suspendCellSelectedEvent();
                for (int i = 0; i < rows.Count; i++)
                {
                    index = rows[i].Index;
                    gotoIndex = index - howManyLines;
                    if (gotoIndex < i) gotoIndex = i; //가장 위로 가면 더이상 갈 수 없다.i열 이상 갈 수도 없다.
                    if (index == gotoIndex) continue; //같은 라인에서 움직일 수 없다.

                    
                    List<DataGridViewCell> cells = new List<DataGridViewCell>();
                    for (int c = 0; c < V_Data.SelectedCells.Count; c++)
                    {
                        if (rows[i].Cells.Contains(V_Data.SelectedCells[c]))
                        {//선택된 셀을 deselect하는 동시에 저장.
                            cells.Add(V_Data.SelectedCells[c]);//.Selected = false;
                            isSelected[rows[i]] = true;
                            //rows[i].HeaderCell.Selected = false;
                        }
                    }

                    foreach (DataGridViewCell cell in cells) cell.Selected = false;

                    if (rows[i].Selected)
                    {
                        isSelected[rows[i]] = true;
                        //rows[i].HeaderCell.Selected = false;
                        if(rowIndex<0) rowIndex = i;
                    }
                    
                    V_Data.Rows.Remove(rows[i]);
                    V_Data.Rows.Insert(gotoIndex, rows[i]);//밑에있는 것들은 제일 첫 줄의 다음열로 간다.
                    
                    //rows[i].Selected = true;
                }
           
                V_Data.resumeCellSelectedEvent();
                if (E_CellSelected != null && rowIndex > 0) E_CellSelected(this, new CellClickEventArgs(rows[rowIndex].Index, V_Data.CurrentCell.ColumnIndex, rows[rowIndex], _itemTypes[ColumnIndex], rows[rowIndex].Cells[ColumnIndex], V_Data, new EventArgs()));
           
                V_Data.ClearSelection();

                foreach (EasyGridRow row in isSelected.Keys)
                {
                    row.Selected = true;
                    V_Data.InvalidateRow(row.Index);
                }

                
                
                //if(isSelected.Count>0)
                    //isSelected.Keys.ElementAt(0).HeaderCell.Selected = true;

                ResumeAutoScrolling();
                if(E_RowPositionChanged!=null) E_RowPositionChanged(this, new RowPositionChangedArgs(-1, gotoIndex, rows));
            }
            //if(rows.Count>0) V_Data.CurrentCell = rows[0].Cells[V_Data.CurrentCell.ColumnIndex];
            resetIndex();
        }
        public void MoveRowsUp(List<int> rowIndexList, int goLine = 0, bool goTogether = true)
        {
            List<EasyGridRow> rows = new List<EasyGridRow>();
            for (int i = 0; i < rowIndexList.Count; i++)
            {
                rows.Add(V_Data.Rows[rowIndexList[i]] as EasyGridRow);
            }
            MoveRowsUp(rows, goLine, goTogether);
            //V_Data.SelectionMode = DataGridViewSelectionMode.CellSelect;
        }

        public void MoveRowTo(EasyGridRow row, int gotoIndex = 0)
        {
            if (row == null) return;
            int index = row.Index;
            if (gotoIndex < 0) gotoIndex = 0; //가장 위로 가면 더이상 갈 수 없다.
            if (gotoIndex > V_Data.Rows.Count - 1) gotoIndex = V_Data.Rows.Count - 1;

            //V_Data.ClearSelection();
            if (index == gotoIndex) return; //같은 라인에서 움직일 수 없다.
            bool isSelected = row.Selected;
            row.Selected = false;
            V_Data.Rows.Remove(row);
            V_Data.Rows.Insert(gotoIndex, row);

            row.Selected = isSelected;
            //V_Data.CurrentCell = row.Cells[V_Data.CurrentCell.ColumnIndex];
            resetIndex();
            if (E_RowPositionChanged != null) E_RowPositionChanged(this, new RowPositionChangedArgs(-1, gotoIndex, (new EasyGridRow[] { row }).ToList()));
        }

        public void MoveRowsTo(List<EasyGridRow> rows, int goLine = 0)
        {

            if (rows.Count == 0) return;
            Comparison<EasyGridRow> com = new Comparison<EasyGridRow>(comp);
            rows.Sort(com);
            int ColumnIndex = V_Data.CurrentCell.ColumnIndex;
            int index = rows[0].Index;/// 0;
            //if(goLine<=rows[0].Index) index = rows[0].Index;
            //else index = rows[rows.Count-1].Index;

            int gotoIndex = goLine;// (goLine < index) ? goLine : goLine - 1;// (rows[0].Index <= goLine && rows[rows.Count - 1].Index >= goLine) ? rows[rows.Count].Index : goLine;

            if (gotoIndex < 0) gotoIndex = 0; //가장 위로 가면 더이상 갈 수 없다.
            if (gotoIndex > V_Data.Rows.Count - rows.Count)
                gotoIndex = V_Data.Rows.Count - rows.Count; //첫열은 아래열보다 내려갈 수 없다.
            //if (index == gotoIndex) return; //같은 라인에서 움직일 수 없다.
            //V_Data.ClearSelection();
            //V_Data.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            //V_Data.MultiSelect = false;
            //V_Data.MultiSelect = true;
            int offset = gotoIndex - index;

            SuspendAutoScrolling();

            Dictionary<DataGridViewRow, bool> isSelected = new Dictionary<DataGridViewRow, bool>();

            V_Data.suspendCellSelectedEvent();
            
            for (int i = 0; i < rows.Count; i++)
            {
                isSelected[rows[i]] = rows[i].Selected;
                rows[i].Selected = false;
            }//선택된 row 저장
            List<DataGridViewCell> cells = new List<DataGridViewCell>();
            
            
            for (int i = 0; i < V_Data.SelectedCells.Count; i++)
            {
                EasyGridRow row = V_Data.SelectedCells[i].OwningRow as EasyGridRow;
                if (rows.Contains(row))
                {
                    isSelected[row] = true;
                    cells.Add(V_Data.SelectedCells[i]);
                    //.Selected = false;
                }
            }//선택된 cell을 가진 row 저장

            foreach (DataGridViewCell cell in cells) cell.Selected = false;

            for (int i = 0; i < rows.Count; i++)
            {
                
                V_Data.Rows.Remove(rows[i]); //일단 모두 뺀다.
                
            }
            for (int i = rows.Count-1; i >= 0; i--)
            {
                V_Data.Rows.Insert(gotoIndex, rows[i]);
            }
            V_Data.resumeCellSelectedEvent();
            if (E_CellSelected != null && rows.Count > 0) E_CellSelected(this, new CellClickEventArgs(rows[0].Index, V_Data.CurrentCell.ColumnIndex, rows[0], _itemTypes[ColumnIndex], rows[0].Cells[V_Data.CurrentCell.ColumnIndex], V_Data, new EventArgs()));
            /*
            for (int i = 0; i < rows.Count; i++)
            {
                if (i == 0)// && rows.Count>1)
                {
                    gotoIndex = index + offset;

                    V_Data.Rows.Insert(gotoIndex, rows[i]);//밑에있는 것들은 제일 첫 줄의 다음열로 간다.
                }
                else
                {
                    V_Data.Rows.Insert(rows[i - 1].Index + 1, rows[i]);//밑에있는 것들은 제일 첫 줄의 다음열로 간다.
                }
            }
            */
            foreach (DataGridViewRow row in isSelected.Keys)
            {
                row.Selected = isSelected[row];//selection 복구
            }

            int beforeScrollOffset = index - V_Data.FirstDisplayedScrollingRowIndex;
            int nextScroll = gotoIndex - beforeScrollOffset;
            nextScroll = (nextScroll < 0) ? 0 : (nextScroll > V_Data.Rows.Count - 10) ? V_Data.Rows.Count - 10 : nextScroll;
            if (nextScroll < 0) nextScroll = 0;
            V_Data.FirstDisplayedScrollingRowIndex = nextScroll;

            ResumeAutoScrolling();
            resetIndex();

            if (E_RowPositionChanged != null) E_RowPositionChanged(this, new RowPositionChangedArgs(gotoIndex-index, gotoIndex, rows));
            //V_Data.SelectionMode = DataGridViewSelectionMode.CellSelect;
        }

        public void MoveRowsTo(List<int> rowIndexList, int goLine = 0)
        {
            List<EasyGridRow> rows = new List<EasyGridRow>();
            for (int i = 0; i < rowIndexList.Count; i++)
            {
                rows.Add(V_Data.Rows[rowIndexList[i]] as EasyGridRow);
            }
            MoveRowsTo(rows, goLine);
            //V_Data.SelectionMode = DataGridViewSelectionMode.CellSelect;
        }


        int comp(DataGridViewRow row1, DataGridViewRow row2)
        {
            return row1.Index - row2.Index;
        }


        public void MoveRowDown(EasyGridRow row, int howManyLines = 1)
        {

            int index = row.Index;
            int gotoIndex = index + howManyLines;
            MoveRowTo(row, gotoIndex);
        }

        public void MoveRowsDown(List<EasyGridRow> rows, int howManyLines = 1, bool goTogether = true)
        {
            if (rows.Count == 0) return;
            if (howManyLines < 0)
            {
                MoveRowsUp(rows, howManyLines * -1, goTogether);
                return;
            }
            int ColumnIndex = V_Data.CurrentCell.ColumnIndex;
            //V_Data.ClearSelection();
            if (goTogether) //가장 마지막 열과 함께 이동한다.
            {
                Comparison<EasyGridRow> com = new Comparison<EasyGridRow>(comp);
                rows.Sort(com);
                int index = rows[0].Index;
                int gotoIndex = index + howManyLines;
                MoveRowsTo(rows, gotoIndex);
            }
            else //각자 위로 이동하다가 맨 마지막에 합쳐진다.
            {
                Comparison<EasyGridRow> com = new Comparison<EasyGridRow>(comp);
                rows.Sort(com);
                int index = rows[0].Index;
                int gotoIndex = index + howManyLines;

                int beforeScrollOffset = index - V_Data.FirstDisplayedScrollingRowIndex;
                int nextScroll = gotoIndex - beforeScrollOffset;
                nextScroll = (nextScroll < 0) ? 0 : (nextScroll > V_Data.Rows.Count - 10) ? V_Data.Rows.Count - 10 : nextScroll;
                V_Data.FirstDisplayedScrollingRowIndex = nextScroll;

                SuspendAutoScrolling();
                bool isSelected = false;
                int rowIndex = -1;
                V_Data.suspendCellSelectedEvent();
                for (int i = rows.Count - 1; i >= 0; i--)
                {
                    index = rows[i].Index;
                    gotoIndex = index + howManyLines;
                    if (gotoIndex < i) gotoIndex = i; //가장 위로 가면 더이상 갈 수 없다.i열 이상 갈 수도 없다.
                    if (index == gotoIndex) continue; //같은 라인에서 움직일 수 없다.
                    isSelected = rows[i].Selected;
                    List<DataGridViewCell> cells = new List<DataGridViewCell>();

                    for (int c = 0; c < V_Data.SelectedCells.Count; c++)
                    {
                        if (rows[i].Cells.Contains(V_Data.SelectedCells[c]))
                        {//선택된 셀을 deselect하는 동시에 저장.
                            cells.Add(V_Data.SelectedCells[c]);
                            //.Selected = false;
                            isSelected = true;
                        }
                    }

                    foreach (DataGridViewCell cell in cells) cell.Selected = false;

                    rows[i].Selected = false;
                    V_Data.Rows.Remove(rows[i]);
                    V_Data.Rows.Insert(gotoIndex, rows[i]);//밑에있는 것들은 제일 첫 줄의 다음열로 간다.

                    rows[i].Selected = isSelected;
                    if (rows[i].Selected == true && rowIndex < 0) rowIndex = i;
                    //rows[i].Selected = true;
                }

                V_Data.resumeCellSelectedEvent();
                if (E_CellSelected != null && rowIndex > 0) E_CellSelected(this, new CellClickEventArgs(rows[rowIndex].Index, V_Data.CurrentCell.ColumnIndex, rows[rowIndex], _itemTypes[ColumnIndex], rows[rowIndex].Cells[V_Data.CurrentCell.ColumnIndex], V_Data, new EventArgs()));
           


                ResumeAutoScrolling();
                if (E_RowPositionChanged != null) E_RowPositionChanged(this, new RowPositionChangedArgs(gotoIndex - index, gotoIndex, rows));
            
            }
            resetIndex();
        }

        public void MoveRowsDown(List<int> rowIndexList, int goLine = 0, bool goTogether = true)
        {
            List<EasyGridRow> rows = new List<EasyGridRow>();
            for (int i = 0; i < rowIndexList.Count; i++)
            {
                rows.Add(V_Data.Rows[rowIndexList[i]] as EasyGridRow);
            }
            MoveRowsDown(rows, goLine, goTogether);
            //V_Data.SelectionMode = DataGridViewSelectionMode.CellSelect;
        }

        #endregion


        #endregion ////////////////////////////////////////////////////////

        #region ///////////////private functions //////////////////////////

        /// <summary>
        /// 오직 ItemType이 ImageCheckBox일 경우에만 사용. 모든 체크박스가 체크되어있는지를
        /// 검사하고 헤더에 Checked, Intermediate, NotChecked상태의 체크박스로 표시함.
        /// </summary>
        /// <param name="colIndex"></param>
        void checkIfAllChecked(int colIndex)
        {
            bool isAllFalse = true;
            int trueCount = 0;
            for (int i = 0; i < Rows.Count; i++)
            {
                if (GetValue(i, colIndex).Equals(true))
                {
                    isAllFalse = false;
                    trueCount++;
                }
            }
            if (isAllFalse) V_Data.Columns[colIndex].Tag = 0;
            else if (trueCount == Rows.Count) V_Data.Columns[colIndex].Tag = 1;
            else V_Data.Columns[colIndex].Tag = 2;

            SetCellRepaint(CellRepaintOptions.ImageCheckBoxHeaderChanged);
        }


        void DoTitleActions(int colIndex)
        {
            switch (_itemTypes[colIndex])
            {
                case ItemTypes.ImageCheckBox:
                    ToggleAllCheckBoxes(colIndex);
                    SetCellRepaint(CellRepaintOptions.ImageCheckBoxHeaderChanged);
                    break;
                case ItemTypes.Button:
                    break;
                case ItemTypes.CheckBox:
                    ToggleAllCheckBoxes(colIndex);
                    break;
                case ItemTypes.CloseButton:
                    break;
                case ItemTypes.ComboBox:
                    break;
                case ItemTypes.TextBox:
                    break;
                case ItemTypes.Image:
                    break;
            }
        }

        void ToggleAllCheckBoxes(int colIndex)
        {
            int state = (int)V_Data.Columns[colIndex].Tag;

            if (_isThreeState[colIndex])
                state = (state + 2) % 3; //1,3,2 순서
            else
                state = (state + 1) % 2;

            V_Data.Columns[colIndex].Tag = state;

            Image[] imageList = _titleInitData[colIndex] as Image[];
            
            bool? beforeState=false;
            bool? nowState = (state == 1) ? true : (state == 0) ? false : (bool?)null;
            List<int> added = new List<int>();
            List<int> removed = new List<int>();
            for (int i = 0; i < Rows.Count; i++)
            {
                beforeState = (bool?)GetValue(i, colIndex);

                V_Data.Rows[i].Cells[colIndex].Value = imageList[state];
                V_Data.Rows[i].Cells[colIndex].Tag = state;

                //SetValueInCell(i, colIndex, state);
                if(beforeState==true){
                    if(state==0) removed.Add(i);

                }else{
                    if(state==1) added.Add(i);
                }
            }

            if (E_CheckBoxChanged != null) E_CheckBoxChanged(this, new CellCheckedEventArgs(nowState, 0, colIndex, added, removed));
            
        }

        void DoActionEachType(int rowIndex, int colIndex, Actions action, EventArgs e)
        {
            int iRow = rowIndex;
            int iCell = colIndex;
            if (iRow < 0 || iCell < 0) return;
            DataGridViewRow row = V_Data.Rows[iRow];
            DataGridViewCell cell = row.Cells[iCell];
            //MouseEventArgs arg = new MouseEventArgs(e.Button, e.Clicks, e.X, e.Y, e.Delta);
            

        }
        void addChecked(int rowIndex, int colIndex, ref List<int> added, ref List<int> removed)
        {
            /*
                //일단 checkbox의 state를 복구한다.
                
                if (isChecked==true) cell.Value = 0;
                else if (isChecked == false)
                {
                    DataGridViewCheckBoxColumn col = V_Data.Columns[colIndex] as DataGridViewCheckBoxColumn;
                    if (col.ThreeState) cell.Value = 2;
                    else     cell.Value = 1;
                }
                else if (isChecked == null) cell.Value = true; //three state모드에서 state는 true 다음이 null이다.
                */
            if (BeforeClickedRow >= 0)
            {
                int min = (BeforeClickedRow < rowIndex) ? BeforeClickedRow : rowIndex;
                int max = (BeforeClickedRow < rowIndex) ? rowIndex : BeforeClickedRow;

                for (int i = min; i <= max; i++)
                {

                    //DataGridViewCheckBoxCell aCell = V_Data.Rows[i].Cells[BeforeClickedCol] as DataGridViewCheckBoxCell;

                    if (GetValue(i, BeforeClickedCol).Equals(0)) added.Add(V_Data.Rows[i].Index);
                    SetValueInCell(i, BeforeClickedCol, 1);
                }
                
                if (E_CheckBoxChanged != null) E_CheckBoxChanged(V_Data, new CellCheckedEventArgs(true, rowIndex, colIndex, added, removed));
                BeforeClickedRow = -1;
                BeforeClickedCol = -1;
            }
            else
            {
                BeforeClickedRow = ClickedRow;
                BeforeClickedCol = ClickedCol;
            }
            checkIfAllChecked(colIndex);
            V_Data.ClearSelection();
        }
        void removeChecked(int rowIndex, int colIndex, ref List<int> added, ref List<int> removed)
        {
            //일단 checkbox의 state를 복구한다.
            /*
            if (cell.Value.Equals(true)) cell.Value = false;
            else if (cell.Value.Equals(false)) cell.Value = true;
            else if (cell.Value.Equals(null)) cell.Value = true; //three state모드에서 state는 true 다음이 null이다.
            */
            if (BeforeClickedRow >= 0)
            {
                int min = (BeforeClickedRow < rowIndex) ? BeforeClickedRow : rowIndex;
                int max = (BeforeClickedRow < rowIndex) ? rowIndex : BeforeClickedRow;

                for (int i = min; i <= max; i++)
                {
                    //DataGridViewCheckBoxCell aCell = V_Data.Rows[i].Cells[BeforeClickedCol] as DataGridViewCheckBoxCell;
                    if (GetValue(i, BeforeClickedCol).Equals(1)) removed.Add(V_Data.Rows[i].Index);
                    SetValueInCell(i, BeforeClickedCol, 0);
                    //if (aCell.Value.Equals(1)) removed.Add(V_Data.Rows[i].Index);
                    //aCell.Value = 0;

                    if (E_CheckBoxChanged != null) E_CheckBoxChanged(V_Data, new CellCheckedEventArgs(false, rowIndex, colIndex, added, removed));
                }
                BeforeClickedRow = -1;
                BeforeClickedCol = -1;
            }
            else
            {
                BeforeClickedRow = ClickedRow;
                BeforeClickedCol = ClickedCol;
            }
            V_Data.ClearSelection();
        }


        void ShowIndex(bool isVisible)
        {
            ShowIndex(isVisible, Color.White, Color.DarkGray);
        }

        void ShowIndex(bool isVisible, Color foreColor, Color backColor)
        {
            V_Data.RowHeadersVisible = isVisible;
            
            if (isVisible)
            {
                V_Data.RowHeadersDefaultCellStyle.ForeColor = Color.White; //test
                V_Data.RowHeadersDefaultCellStyle.BackColor = Color.DarkGray; //test

                for (int i = 0; i < V_Data.Rows.Count; i++)
                {
                    V_Data.Rows[0].HeaderCell.Value = i.ToString();
                }
            }
        }

        #endregion///////////////////////////////////////////////////////

        
    }

 





}
