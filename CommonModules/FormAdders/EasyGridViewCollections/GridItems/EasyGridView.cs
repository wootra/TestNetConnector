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
//using System.ComponentModel;
using IOHandling;
using FormAdders.EasyGridViewCollections;
using DataHandling;

namespace FormAdders
{
    public partial class EasyGridView : UserControl
    {
         
       

        #region events

        public event EasyGridMenuClickHandler E_ContextMenuClicked = null;
        public event ColumnWidthChangedEventHandler E_ColumnWidthChanged;
        public event ColumnWidthChangingEventHandler E_ColumnWidthChanging;
        public event CellClickEventHandler E_CellClicked;
        public event CellClickEventHandler E_HeaderCellClicked;
        public event CellClickEventHandler E_HeaderCellRightClicked;
        
        public event CellClickEventHandler E_CellDoubleClicked;
        public event CellClickEventHandler E_CellRightClicked;
        public event CellClickEventHandler E_ListRowRemoving;
        public event CellClickEventHandler E_ListRowRemoved;
        public event CellCheckedEventHandler E_CheckBoxChanged;
        public event CellRadioButtonSelectedEventHandler E_RadioButtonSelected;
        public event CellCheckBoxGroupSelectedEventHandler E_CheckBoxGroupSelected;
        public event CellTextChangedEventHandler E_TextChanged;
        public event DataGridViewRowEventHandler E_UserAddedRow;
        public event DataGridViewRowEventHandler E_UserDeleteRow;
        public event CellTextChangedEventHandler E_TextEditFinished;
        public event CellComboBoxEventHandler E_ComboBoxChanged;
        public event RowPositionChangedHandler E_RowPositionChanged;
        public new event ScrollEventHandler Scroll;
        public event LineRemovedByMaxLinesLimitEvent E_LineRemovedByMaxLinesLimit;
        /// <summary>
        /// Click, 버튼으로 인한 선택변화에 대응하는 Event이다.
        /// </summary>
        public event CellClickEventHandler E_CellSelected;
        public event EventHandler E_SelectionChanged;

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
        //protected List<bool?> _editables = new List<bool?>();
        //protected List<Boolean> _isThreeState = new List<bool>();
        //protected List<Object> _titleInitData = new List<object>();
        //List<Image> _checkBoxImageList = new List<Image>();
        #endregion

        #endregion

        public EasyGridView()
            : base()
        {
            InitializeComponent();

            ActionOnDoubleClicked = Actions.CheckBoxChecked;
            ActionOnClicked = Actions.Nothing;
            ActionOnRightClicked = Actions.ContextMenu;

            V_Data.ColumnWidthChanged += V_Data_ColumnWidthChanged;
            
            V_Data.CellMouseDown += new DataGridViewCellMouseEventHandler(V_Data_CellMouseDown);
            V_Data.CellMouseUp += new DataGridViewCellMouseEventHandler(V_Data_CellMouseUp);
            //V_Data.PreviewKeyDown += new PreviewKeyDownEventHandler(V_Data_PreviewKeyDown);
            V_Data.CellDoubleClick += new DataGridViewCellEventHandler(V_Data_CellDoubleClick);
            
            V_Data.MouseClick += new MouseEventHandler(V_Data_MouseClick);
            V_Data.E_TextEditFinished += new CellTextChangedEventHandler(V_Data_E_TextEditFinished);
            //V_Data.CellBeginEdit += new DataGridViewCellCancelEventHandler(V_Data_CellBeginEdit);
            V_Data.E_TextChanged +=new CellTextChangedEventHandler(V_Data_E_TextChanged);
            V_Data.CellEnter += new DataGridViewCellEventHandler(V_Data_CellEnter);
            V_Data.CellLeave += new DataGridViewCellEventHandler(V_Data_CellLeave);
            V_Data.KeyUp += new KeyEventHandler(V_Data_KeyUp);
            V_Data.CellValueChanged += new DataGridViewCellEventHandler(V_Data_CellValueChanged);
            V_Data.E_CellSelected += new CellClickEventHandler(V_Data_E_CellSelected);
            V_Data.Click += new EventHandler(V_Data_Click);
            DataGridViewRowsAddedEventHandler rowAddEventHandler = new DataGridViewRowsAddedEventHandler(RowsAdded);
            V_Data.RowsAdded += rowAddEventHandler;
            V_Data.ContextMenu = new System.Windows.Forms.ContextMenu();
            V_Data.DataError += new DataGridViewDataErrorEventHandler(V_Data_DataError);
            V_Data.CellPainting += new DataGridViewCellPaintingEventHandler(V_Data_CellPainting);
            V_Data.CellMouseEnter += new DataGridViewCellEventHandler(V_Data_CellMouseEnter);
            V_Data.CellMouseLeave += new DataGridViewCellEventHandler(V_Data_CellMouseLeave);
            // T_Modify.Visible = false;
            //V_Data.RowsAdded += new DataGridViewRowsAddedEventHandler(V_Data_RowsAdded);
            //V_Data.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            //V_Data.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
            V_Data.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            V_Data.RowHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            V_Data.EditMode = DataGridViewEditMode.EditProgrammatically;
            V_Data.UserAddedRow += V_Data_UserAddedRow;
            V_Data.UserDeletedRow += V_Data_UserDeletedRow;
            V_Data.RowHeadersWidth = 50;
            //V_Data.Scroll += new ScrollEventHandler(V_Data_Scroll);
            V_Data.SelectionChanged += new EventHandler(V_Data_SelectionChanged);
            //V_Data.setEditables( _itemTypes);
            V_Data.Scroll += new ScrollEventHandler(V_Data_Scroll);
            V_Data.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(V_Data_EditingControlShowing);
            V_Data.AlternatingRowsDefaultCellStyle.BackColor = Color.WhiteSmoke;
            V_Data.DefaultCellStyle.BackColor = Color.White;
            V_Data.DefaultCellStyle.SelectionBackColor = Color.LightSkyBlue;
            //V_Data.VirtualMode = true;
            //_checkBoxImageList.Add(Properties.Resources.check_normal);
           // _checkBoxImageList.Add(Properties.Resources.check_red);
            //_checkBoxImageList.Add(Properties.Resources.check_inter);
            /*
            Panel p = new Panel();
            p.SetBounds(0, 0, 100, 100);
            p.Size = new Size(100, 100);
            p.BackColor = Color.White;
            p.Location = new Point(0,0);
            
            this.Controls.Add(p);
            p.BringToFront();
             */
        }

        ~EasyGridView()
        {
            int a;
        }

        void V_Data_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            if (E_UserDeleteRow != null) E_UserDeleteRow(this, e);
        }

        void V_Data_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            if (E_UserAddedRow != null) E_UserAddedRow(this, e);
        }

        public bool AllowUserToDeleteRows
        {
            get
            {
                return V_Data.AllowUserToDeleteRows;
            }

            set { V_Data.AllowUserToDeleteRows = value; }
        }

        public bool AllowUserToAddRows
        {
            get
            {
                return V_Data.AllowUserToAddRows;
            }

            set
            {
                V_Data.AllowUserToAddRows = value;
            }
        }


        void V_Data_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            if (E_ColumnWidthChanging != null) E_ColumnWidthChanging(this, new ColumnWidthChangingEventArgs(e.Column.Index, e.Column.Width));

            if (E_ColumnWidthChanged != null) E_ColumnWidthChanged(this, new ColumnWidthChangedEventArgs(e.Column.Index));
            
        }

        void V_Data_Scroll(object sender, ScrollEventArgs e)
        {

            List<IEasyGridCell> baseCells = new List<IEasyGridCell>();
            /*
            Rectangle rect;

            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                if (e.NewValue < e.OldValue)//left
                {
                    int size = (e.OldValue - e.NewValue)+10;
                    rect = new Rectangle(new Point(0, 0), new Size(size, V_Data.Height));   
                }
                else
                {
                    int size = e.NewValue - e.OldValue + 10;
                    rect = new Rectangle(new Point(V_Data.Width - size, 0), new Size(size, V_Data.Height));
                    
                }
            }
            else //vertical
            {
                if (e.NewValue < e.OldValue)//up
                {
                    int size = (e.OldValue - e.NewValue) + 10;
                    rect = new Rectangle(new Point(0, 0), new Size(V_Data.Width, size));
                }
                else //down
                {
                    int size = e.NewValue - e.OldValue + 10;
                    rect = new Rectangle(new Point(0, V_Data.Height - size), new Size(V_Data.Width, size));
                    
                }
            }
            V_Data.Invalidate(rect, true);
             */
            /*
            for (int i = 0; i < V_Data.Columns.Count; i++)
            {
                IEasyGridColumn column = V_Data.Columns[i] as IEasyGridColumn;
                for (int ci = 0; ci < column.Span.Spans.Count; ci++)
                {
                    column.Span.Spans[ci].BaseCell.RePaint();
                }
                //if(column.Span.Spans.Count>0) RefreshColumn(column.Index);
            }
             */
            int firstCol = V_Data.FirstDisplayedScrollingColumnIndex;
            int lastCol = V_Data.DisplayedColumnCount(true) + firstCol - 1;
            if (lastCol >= V_Data.Columns.Count) lastCol = V_Data.Columns.Count - 1;
            int firstRow = V_Data.FirstDisplayedScrollingRowIndex;
            int lastRow = firstRow + V_Data.DisplayedRowCount(true) - 1;
            if (lastRow >= V_Data.Rows.Count) lastRow = V_Data.Rows.Count - 1;
            for (int i = firstRow; i <= lastRow; i++)
            {

                for (int col = 0; col <= lastCol; col++)
                {
                    IEasyGridCell cell = V_Data.Rows[i].EasyCells[col];
                    
                    if (cell.ItemType == ItemTypes.TextBox)
                    {
                        if (cell.Span.SpanPos == SpanPosition.SpanBase) CellSpanInfo.RedrawSpanCell = true;
                        if (cell.Span.SpanPos != SpanPosition.NoSpan)
                            (cell as EasyGridTextBoxCell).RePaint();
                    }
                }

            }
            if (Scroll != null) Scroll(this, e);
            
            //V_Data.GetSpanBaseCells();
            /*
            Graphics g = V_Data.CreateGraphics();
            foreach (IEasyGridCell cell in baseCells)
            {
                cell.RePaint();
            }
             */
            //V_Data.Refresh();
        }

        void V_Data_MouseClick(object sender, MouseEventArgs e)
        {
            
            int firstRow = V_Data.FirstDisplayedScrollingRowIndex;
            if (firstRow >= 0)
            {
                Rectangle rect1 = V_Data.GetRowDisplayRectangle(firstRow, true);
                Rectangle rect2 = V_Data.GetRowDisplayRectangle(firstRow + V_Data.DisplayedRowCount(true) - 1, true);
                Rectangle rowRect = new Rectangle(rect1.X, rect1.Y, rect1.Width, rect2.Y + rect2.Height);

                if (rowRect.Contains(new Point(e.X, e.Y)))
                {
                    //CellClicked에서 처리한다.
                    return;
                }
            }
            //아래는 cell이 존재하지 않는 영역을 클릭했을 때에 활성화되는 루틴이다.
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {

                if (E_CellClicked != null) E_CellClicked(V_Data, new CellClickEventArgs(-2, -2, null, ItemTypes.BlankBack, null, V_Data, e, null));
                
                //doLeftClick(null, null, -1, -1, ItemTypes.BlankBack, e);

                OnCellClicked(V_Data, new CellClickEventArgs(-2, -2, null, ItemTypes.BlankBack, null, V_Data, e,null));
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                Actions action;
                action = ActionOnRightClicked;
                
                if (E_CellRightClicked != null) E_CellRightClicked(V_Data, new CellClickEventArgs(-2, -2, null, ItemTypes.BlankBack, null, V_Data, e,null));
                
                OnRightClicked(V_Data, new CellClickEventArgs(-2, -2, null, ItemTypes.BlankBack, null, V_Data, e, null));

                if (action == Actions.ContextMenu) U_ContextMenu.Show(this, this.PointToClient(Control.MousePosition));

                //DoAction(action, -1, -1);
            }
        }
        protected virtual void OnCellClicked(DataGridView sender, CellClickEventArgs args) { }
        protected virtual void OnRightClicked(DataGridView sender, CellClickEventArgs args) { }
/*
        void V_Data_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if(_displayLastRow) 
                this.FirstDisplayedScrollingRowIndex = this.RowCount - this.DisplayedRowCount;
        }
 */
 

        #region TableActions.. ActionOn...

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
        public object DataSource
        {
            get
            {
                return V_Data.DataSource;
            }
            set
            {
                V_Data.DataSource = value;
            }
        }

        bool _displayLastRow = true;
        public bool ScrollToLastRowWhenAddNew
        {
            get
            {
                return _displayLastRow;
            }
            set
            {
                _displayLastRow = value;
            }
        }



        int _maxLines = 100;
        /// <summary>
        /// 0이상이면 그 라인에 도달하면 ClearLinesWhenMaxLines 만큼 지운다.
        /// </summary>
        public int MaxLines
        {
            get { return _maxLines; }
            set { _maxLines = value; }
        }

        /// <summary>
        /// 화면에 최대한 보일 수 있는 row의 갯수이다.
        /// rowHeight의 기준은 BaseRowHeight이다.
        /// </summary>
        public int VisibleMaxLines
        {
            get
            {
                return ((Height - ColumnHeaderHeight) / BaseRowHeight);
            }
        }

        int _clearLinesWhenMaxLines = 10;
        /// <summary>
        /// 0이상이면 MaxLines에 도달하면 정한만큼 지운다.
        /// </summary>
        public int ClearLinesWhenMaxLines
        {
            get { return _clearLinesWhenMaxLines; }
            set { _clearLinesWhenMaxLines = value; }
        }

        /// <summary>
        /// Text표시 시, 크기를 원래크기로 유지할 것인지 모두 보여주기 위해 크기를 조정할 것인지를 정합니다.
        /// </summary>
        public TextViewModes TextViewMode
        {
            get { return V_Data.TextViewMode; }
            set { V_Data.TextViewMode = value; }
        }

        /// <summary>
        /// TextCell의 내용을 정렬할 위치를 정한다.
        /// </summary>
        public TextAlignModes TextAlignMode
        {
            get { return V_Data.TextAlignMode; }
            set { V_Data.TextAlignMode = value; }
        }

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
                if (value != null)
                {
                    for (int i = 0; i < value.Count; i++)
                    {
                        V_Data.Rows[i].Selected = true;
                    }
                }
            }
        }

        public new bool Enabled
        {
            get { return base.Enabled; }
            set
            {
                V_Data.Enabled = value;
                base.Enabled = value;
                foreach(EasyGridRow row in Rows){
                    row.Enabled = value;
                }
                Refresh();
                //V_Data.InvalidateRow(0);
                //V_Data.Invalidate();
                //V_Data.Update();
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
        /// 
        public CustomDictionary<String, Object> RelativeObject { get { return V_Data.RelativeObject; } }

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
                V_Data.Invalidate();
                V_Data.Update();
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

        
        public EasyGridRowCollection Rows
        {
            get
            {
                return (V_Data as EasyGridViewParent).Rows as EasyGridRowCollection;
                /*
                List<EasyGridRow> list = new List<EasyGridRow>();
                foreach (EasyGridRow row in V_Data.Rows)
                {
                    list.Add(row);
                }
                return list;
                 */
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
        
        public List<String> ColumnTexts
        {
            get
            {
                List<String> list = new List<string>();
                for (int i = 0; i < V_Data.Columns.Count; i++) list.Add(V_Data.Columns[i].HeaderText);
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

        public int ColumnHeaderHeight
        {
            get
            {
                return V_Data.ColumnHeadersHeight;
            }
            set
            {
                V_Data.ColumnHeadersHeight = value;
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
                    if (V_Data.Columns[i].Visible == false) wid.Add(0);
                    else wid.Add(V_Data.Columns[i].Width);
                }
                return wid;
            }
        }

        #endregion

        #region EventHandlers
        void V_Data_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                OnCellMouseLeave(V_Data, e);
                return;
            }
            if (V_Data.Rows[e.RowIndex] == null)
            {
                OnCellMouseLeave(V_Data, e);
                return;
            }
            if (V_Data.Rows[e.RowIndex].Cells[e.ColumnIndex] == null)
            {
                OnCellMouseLeave(V_Data, e);
                return;
            }
            IEasyGridCell cell = V_Data.Rows[e.RowIndex].Cells[e.ColumnIndex] as IEasyGridCell;
            if (cell == null || cell.Enabled == false) return;
            if (cell.ItemType == ItemTypes.ImageButton)
            {
                EasyGridImageButtonCell ib = cell as EasyGridImageButtonCell;
                ib.ButtonState = EasyGridImageButtonCell.ButtonStates.Out;
                V_Data.InvalidateCell(cell as DataGridViewCell);
            }
            else if (cell.ItemType == ItemTypes.TextBox)
            {

            }
            OnCellMouseLeave(V_Data, e);
        }
        



        void V_Data_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                OnCellMouseEnter(V_Data, e);
                return;
            }
            if (V_Data.Rows[e.RowIndex] == null)
            {
                OnCellMouseEnter(V_Data, e);
                return;
            }
            if (V_Data.Rows[e.RowIndex].Cells[e.ColumnIndex] == null)
            {
                OnCellMouseEnter(V_Data, e);
                return;
            }
            IEasyGridCell cell = V_Data.Rows[e.RowIndex].Cells[e.ColumnIndex] as IEasyGridCell;
            if (cell == null || cell.Enabled == false)
            {
                OnCellMouseEnter(V_Data, e);
                return;
            }
            if (cell.ItemType == ItemTypes.ImageButton)
            {
                EasyGridImageButtonCell ib = cell as EasyGridImageButtonCell;
                if (ib.Visible == false)
                {
                    return;
                }
                ib.ButtonState = EasyGridImageButtonCell.ButtonStates.Over;
                V_Data.InvalidateCell(cell as DataGridViewCell);
            }
            else if (cell.ItemType == ItemTypes.TextBox)
            {
                //V_Data.ShowCellToolTips = true;
            }
            OnCellMouseEnter(V_Data, e);

        }

        protected virtual void OnCellMouseEnter(DataGridView sender, DataGridViewCellEventArgs args)
        {
        }

        protected virtual void OnCellMouseLeave(DataGridView sender, DataGridViewCellEventArgs args)
        {
        }

        void V_Data_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            IEasyGridCell cell = V_Data.CurrentCell as IEasyGridCell;
            if (cell != null && cell.Enabled == false) V_Data.EndEdit();
        }
        

        public DataGridViewSelectedCellCollection SelectedCells
        {
            get { return V_Data.SelectedCells; }
        }

        public DataGridViewSelectedColumnCollection SelectedColumns
        {
            get
            {
                return V_Data.SelectedColumns;
            }
        }

        void V_Data_SelectionChanged(object sender, EventArgs e)
        {
            DataGridViewSelectedCellCollection _selection = null;
            DataGridViewSelectedCellCollection nowCells = V_Data.SelectedCells;
            if (_selection == null)
            {
                _selection = V_Data.SelectedCells;
                
            }
            else if (_selection.Equals(nowCells) == false)
            {
                //List<DataGridViewCell> cells = new List<DataGridViewCell>();

                foreach (DataGridViewCell cell in _selection)
                {
                    if (nowCells.Contains(cell) == false)
                    {
                        //if (cells.Contains(cell) == false) cells.Add(cell);
                        try
                        {
                            V_Data.InvalidateCell(cell);
                        }
                        catch { }
                    }
                }
                /*
                foreach (DataGridViewCell cell in cells)
                {
                    V_Data.InvalidateCell(cell);
                }
                */
                _selection = V_Data.SelectedCells;

            }
            if (E_SelectionChanged != null) E_SelectionChanged(this, e);

        }
        /*
        void V_Data_Scroll(object sender, ScrollEventArgs e)
        {

            List<IEasyGridCell> baseCells = V_Data.GetSpanBaseCells();
            Graphics g = V_Data.CreateGraphics();
            foreach (IEasyGridCell cell in baseCells)
            {
                cell.RePaint();
            }
            //V_Data.Refresh();
        }
        */
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
            
            OnCellSelected(this, e);
        }

        protected virtual void OnCellSelected(EasyGridView sender, CellClickEventArgs args) { }

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
                    if (e.KeyData == (Keys.Right | Keys.Control))
                    {
                        V_Data.Columns[col].Width += 10;
                    }
                    if (e.KeyData == (Keys.Left | Keys.Control))
                    {
                        if (V_Data.Columns[col].Width >= 20)//최소 10
                        {
                            V_Data.Columns[col].Width -= 10;
                        }
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
                    EasyGridComboBoxCell cell = V_Data.CurrentCell as EasyGridComboBoxCell;

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
            if (E_TextChanged != null && _isClearing==false) E_TextChanged(this, e);
            OnTextChanged(this, e);
        }

        protected virtual void OnTextChanged(EasyGridView sender, CellTextChangedEventArgs args) { }

        void V_Data_E_TextEditFinished(object sender, CellTextChangedEventArgs e)
        {
            if (E_TextEditFinished != null && _isClearing==false) E_TextEditFinished(this, e);
        }

        protected virtual void OnTextEditFinished(EasyGridView sender, CellTextChangedEventArgs args) { }

        void V_Data_CellEnter(object sender, DataGridViewCellEventArgs e)
        {

            //if(_showPosition) V_Data.TopLeftHeaderCell.Value = "(" + (e.RowIndex + 1) + "," + (e.ColumnIndex + 1) + ")";
            ClickedCol = e.ColumnIndex;
            ClickedRow = e.RowIndex;
            OnCellEnter(V_Data, e);
        }
        void V_Data_CellLeave(object sender, DataGridViewCellEventArgs e)
        {

        }

        protected virtual void OnCellEnter(DataGridView sender, DataGridViewCellEventArgs args)
        {
        }

        protected virtual void OnCellLeave(DataGridView sender, DataGridViewCellEventArgs args)
        {
        }


        void RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (_isAutoScrolling)
            {
                DataGridView dv = (DataGridView)sender;
                int firstRow = dv.RowCount - dv.DisplayedRowCount(false)-1;
                if (firstRow > 0)
                {
                    
                    //if (dv.RowCount > dv.DisplayedRowCount(false)) dv.FirstDisplayedScrollingRowIndex = firstRow;// dv.RowCount - 10;
                }
            }
        }

        public int FirstDisplayedScrollingRowIndex
        {
            get
            {
                return V_Data.FirstDisplayedScrollingRowIndex;
            }
            set
            {
                if (value >= 0 && value < V_Data.Rows.Count)
                {
                    V_Data.FirstDisplayedScrollingRowIndex = value;
                }
                else if(V_Data.Rows.Count>0)
                {
                    V_Data.FirstDisplayedScrollingRowIndex = 0;
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

        protected virtual void OnCheckBoxClicked(int rowIndex, int colIndex)
        {

            /*
            ListBoxItem item = WpfFinder.getParentFromTemplate(cb, DependencyObjectType.FromSystemType(typeof(ListBoxItem))) as ListBoxItem;
            //ListBox view = WpfFinder.getParentFromTemplate(item, DependencyObjectType.FromSystemType(typeof(ListBox))) as ListBox;
            if (item == null) return;
            ListRow row = item.Content as ListRow;
            */
            // DataGridViewCheckBoxCell cell = V_Data.Rows[rowIndex].Cells[colIndex] as DataGridViewCheckBoxCell;
            IEasyGridCheckBoxCell checkCell = Cell(rowIndex, colIndex) as IEasyGridCheckBoxCell;
            if (checkCell == null) 
                return;

            bool? boolState = checkCell.IsChecked;// (bool?)GetValue(rowIndex, colIndex);// (int)cell.Value;
            //int checkState = (boolState == true) ? 1 : (boolState == false) ? 0 : 2;

            boolState = (boolState == false) ? true : false;
            
            List<int> added = new List<int>();
            List<int> removed = new List<int>();
            //List<int> checkedList = new List<int>();


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

                //if (_isThreeState[colIndex]) checkState = (checkState + 2) % 3;
                //else checkState = (checkState + 1) % 2;
                //IEasyGridCheckBoxCell checkCell = Cell(rowIndex, colIndex) as IEasyGridCheckBoxCell;
                
                //SetValueInCell(rowIndex, colIndex, checkState);// cell.Value = 2;
                //boolState = (checkState == 1) ? true : (checkState == 0) ? false : (bool?)null;
                if (boolState == true) added.Add(rowIndex);
                else if (boolState == false) removed.Add(rowIndex);

                CellCheckedEventArgs args = new CellCheckedEventArgs(boolState, colIndex, rowIndex, rowIndex, added, removed);
                
                if (E_CheckBoxChanged != null) E_CheckBoxChanged(this, args);

                if (args.IsCancel == false)
                {
                    checkCell.IsChecked = boolState;
                }
            }
            _clickedKey = Control.ModifierKeys;
        }
   
        void V_Data_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            
            DataGridViewRow row = V_Data.Rows[e.RowIndex];
            DataGridViewCell cell = row.Cells[e.ColumnIndex];
            IEasyGridCell easyCell = cell as IEasyGridCell;
            if (easyCell != null && easyCell.Enabled == false)
            {
                
                if (E_CellDoubleClicked != null) E_CellDoubleClicked(sender, new CellClickEventArgs(e.RowIndex, e.ColumnIndex, row, _itemTypes[e.ColumnIndex], cell, V_Data, e, easyCell.Value));
                return;
            }

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
                EasyGridTextBoxCell myCell = V_Data.Rows[e.RowIndex].Cells[e.ColumnIndex] as EasyGridTextBoxCell;

                if (action == Actions.Auto)
                {
                    if (myCell.IsEditable)
                    {
                        BeginModifyMode(e.RowIndex, e.ColumnIndex);
                    }

                    //else do nothing..
                }
                else
                {
                    if (action == Actions.Modify) //modify mode라고 해도 _editable로 등록되어있지 않으면 수정할 수 없다.
                    {
                        if (myCell.IsEditable) BeginModifyMode(e.RowIndex, e.ColumnIndex);
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
                if (myCell != null)
                {
                    if (E_CellDoubleClicked != null) E_CellDoubleClicked(sender, new CellClickEventArgs(e.RowIndex, e.ColumnIndex, row, _itemTypes[e.ColumnIndex], cell, V_Data, e, myCell.Value));
                }
                else
                {

                }
            }
            else if (_itemTypes[e.ColumnIndex] == ItemTypes.FileOpenBox)
            {
                EasyGridFileOpenBoxCell myCell = V_Data.Rows[e.RowIndex].Cells[e.ColumnIndex] as EasyGridFileOpenBoxCell;

                if (action == Actions.Auto)
                {
                    if (myCell.IsEditable)
                    {
                        BeginModifyMode(e.RowIndex, e.ColumnIndex);
                    }

                    //else do nothing..
                }
                else
                {
                    if (action == Actions.Modify) //modify mode라고 해도 _editable로 등록되어있지 않으면 수정할 수 없다.
                    {
                        if (myCell.IsEditable) BeginModifyMode(e.RowIndex, e.ColumnIndex);
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

                if (E_CellDoubleClicked != null) E_CellDoubleClicked(sender, new CellClickEventArgs(e.RowIndex, e.ColumnIndex, row, _itemTypes[e.ColumnIndex], cell, V_Data, e, myCell.Value));
            }
            else if (_itemTypes[e.ColumnIndex] == ItemTypes.Image || _itemTypes[e.ColumnIndex] == ItemTypes.ImageButton ||
               _itemTypes[e.ColumnIndex] == ItemTypes.Button)
            {
                IEasyGridCell myCell = V_Data.Rows[e.RowIndex].Cells[e.ColumnIndex] as IEasyGridCell;

                if (action == Actions.Auto)
                {
                    //do nothing..
                }
                else
                {
                    DoAction(action, e.RowIndex, e.ColumnIndex);
                }
                if (E_CellDoubleClicked != null) E_CellDoubleClicked(sender, new CellClickEventArgs(e.RowIndex, e.ColumnIndex, row, _itemTypes[e.ColumnIndex], cell, V_Data, e, myCell.Value));
            }
            else if (_itemTypes[e.ColumnIndex] == ItemTypes.ComboBox)
            {
                IEasyGridCell myCell = V_Data.Rows[e.RowIndex].Cells[e.ColumnIndex] as IEasyGridCell;

                if (action == Actions.Auto)
                {
                    //do nothing..
                }
                else
                {
                    DoAction(action, e.RowIndex, e.ColumnIndex);
                }
                if (E_CellDoubleClicked != null) E_CellDoubleClicked(sender, new CellClickEventArgs(e.RowIndex, e.ColumnIndex, row, _itemTypes[e.ColumnIndex], cell, V_Data, e, myCell.Value));
            }
            
            IEasyGridCell selectedCell = V_Data.Rows[e.RowIndex].Cells[e.ColumnIndex] as IEasyGridCell;

            OnCellDoubleClicked(V_Data, new CellClickEventArgs(e.RowIndex, e.ColumnIndex, row, _itemTypes[e.ColumnIndex], cell, V_Data, e, selectedCell.Value));
        }

        protected virtual void OnCellDoubleClicked(DataGridView sender, CellClickEventArgs args){}

        //bool _isEditing = false;
        void CloseBtnClicked(int RowIndex, int ColumnIndex, EventArgs e)
        {

            DataGridViewRow deletedRow = V_Data.Rows[RowIndex];
            EasyGridCloseButtonCell cell = deletedRow.Cells[ColumnIndex] as EasyGridCloseButtonCell;
            CellClickEventArgs args = new CellClickEventArgs(RowIndex, ColumnIndex, deletedRow, _itemTypes[ColumnIndex], cell, V_Data, e, cell.Value);

            if (E_ListRowRemoving != null) E_ListRowRemoving(deletedRow, args);

            if (args.IsCancel ==true)
            {
                
                    return;
                
            }
            RemoveARow(RowIndex);

            if (E_ListRowRemoved != null) E_ListRowRemoved(deletedRow, args);
        }

        bool _isTitlePressed=false;
        
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


            if (e.ColumnIndex >= 0 && e.RowIndex < 0)
            {
                _isTitlePressed = true;
                V_Data._pressedPoint = Control.MousePosition;
            }
            else
            {
                _isTitlePressed = false;
            }
            BeforeClickedCol = ClickedCol;
            BeforeClickedRow = ClickedRow;

            ClickedCol = e.ColumnIndex;
            ClickedRow = e.RowIndex;
            V_Data._pressedCell = Cell(ClickedRow, ClickedCol);
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
            DataGridViewCell cell = null;
            DataGridViewColumn col = null;
            ItemTypes itemType;

            if (e.ColumnIndex >= 0 && e.RowIndex < 0)
            {
                if (_isTitlePressed)
                {
                    Point pt = Control.MousePosition;
                    int deltaX = pt.X - V_Data._pressedPoint.X;
                    int wid = V_Data.Columns[e.ColumnIndex].Width;
                    if (deltaX < -wid) deltaX = -(wid - 2);
                    if (V_Data.Columns[e.ColumnIndex].Width + deltaX < 10) { 
                        // 10이하의 크기로는 줄일 수 없다.
                    }
                    else
                    {


                        if (Math.Abs(deltaX) > 10)
                        {
                            ColumnWidthChangingEventArgs args = new ColumnWidthChangingEventArgs(V_Data._pressedCell.ColumnIndex, V_Data.Columns[V_Data._pressedCell.ColumnIndex].Width + deltaX);
                            if (E_ColumnWidthChanging != null) E_ColumnWidthChanging(this, args);
                            if (args.Cancel != true)
                            {
                                Point clientPt = V_Data.PointToClient(V_Data._pressedPoint);
                                int colX = 0;
                                for (int i = 0; i < V_Data._pressedCell.ColumnIndex; i++)
                                {
                                    colX+=V_Data.Columns[i].Width;
                                }
                                int clientHalfX = colX + V_Data.Columns[V_Data._pressedCell.ColumnIndex].Width / 2;
                                
                                if (clientPt.X < clientHalfX)
                                {
                                    V_Data.Columns[V_Data._pressedCell.ColumnIndex].Width -= deltaX;
                                }
                                else
                                {
                                    V_Data.Columns[V_Data._pressedCell.ColumnIndex].Width += deltaX;
                                }
                                
                                if (E_ColumnWidthChanged != null) E_ColumnWidthChanged(this,
                              new ColumnWidthChangedEventArgs(V_Data._pressedCell.ColumnIndex));
                            }
                            _isTitlePressed = false;
                            V_Data._pressedPoint = Control.MousePosition;
                            return;
                        }
                        //else if (deltaX < -10) V_Data.Columns[_pressedCell.ColumnIndex].Width += deltaX;
                       
                    }
                }
                
                
            }
            else
            {
            }
            V_Data._pressedPoint = Control.MousePosition;
            _isTitlePressed = false;

            if (e.RowIndex < 0 && e.ColumnIndex < 0) return;
            if (V_Data._pressedCell == null) 
                return;
            if (e.ColumnIndex != V_Data._pressedCell.ColumnIndex
                || e.RowIndex != V_Data._pressedCell.RowIndex) 
                    return;//다른 셀에서 MouseUp하면 return

            if (e.ColumnIndex >= 0){
                col = V_Data.Columns[e.ColumnIndex];
                itemType = (col as IEasyGridColumn).ItemType;
            }else{
                itemType = ItemTypes.Header;
            }
            
            


            IEasyGridCell myCell = (e.RowIndex<0 || e.ColumnIndex<0)? null : V_Data.Rows[e.RowIndex].Cells[e.ColumnIndex] as IEasyGridCell;

            if (iRow < 0)
            {
                if(iCell<0){
                    if (E_HeaderCellClicked != null) E_HeaderCellClicked(this, new CellClickEventArgs(-1, -1, null, itemType, V_Data.TopLeftHeaderCell, V_Data, e, myCell.Value));
                }else{
                    cell = col.HeaderCell;
                    //if (e.X <= 5 || e.X >= col.Width - 5) return; //크기변경구역
                    CellClickEventArgs args = new CellClickEventArgs(-1, iCell, null, itemType, cell, V_Data, e, Column(iCell).HeaderText);
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        if (E_HeaderCellClicked != null)
                        {
                            E_HeaderCellClicked(this, args);
                        }
                    }
                    else if (e.Button == System.Windows.Forms.MouseButtons.Right)
                    {
                        if (E_HeaderCellRightClicked != null)
                        {
                            E_HeaderCellRightClicked(this, args);
                        }
                    }
                    if(args.IsCancel!=true) DoTitleActions(iCell);
                }
                return;
            }
            else
            {
                if (iCell < 0)
                {
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        if (E_HeaderCellClicked != null) E_HeaderCellClicked(this, new CellClickEventArgs(iRow, -1, null, itemType, V_Data.TopLeftHeaderCell, V_Data, e, Rows[iRow].HeaderCell.Value));
                    }
                    else if (e.Button == System.Windows.Forms.MouseButtons.Right)
                    {
                        if (E_HeaderCellRightClicked != null) E_HeaderCellRightClicked(this, new CellClickEventArgs(iRow, -1, null, itemType, V_Data.TopLeftHeaderCell, V_Data, e, Rows[iRow].HeaderCell.Value));
                    }
                    
                    return;
                }
                
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
            if (row.Cells.Count <= iCell) return;
            
            if (row[iCell] == null) return;
            /*
            if (row[iCell].Span.SpanPos != SpanPosition.Spanned)
            {
                //if (row[iCell].Enabled == false) return;
            }
            else //spanned cell이면 치환해 준다.
            {
                
                iRow = row[iCell].Span.SpanBaseCell.RowIndex;
                iCell = row[iCell].Span.SpanBaseCell.ColumnIndex;
                row = V_Data.Rows[iRow];
            }
            */

            if (Cell(iRow, iCell) != null && (Cell(iRow,iCell) as IEasyGridCell)!=null)
            {
                if ((Cell(iRow, iCell) as IEasyGridCell).Enabled == false && EventEnabledWhenCellDisabled==false) return;
            }

            


            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                doLeftClick(row, cell, iRow, iCell, itemType, e);
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
                    if (E_CellRightClicked != null) E_CellRightClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e, cell.Value));
                }
                else if (itemType == ItemTypes.FileOpenBox)
                {
                    // DataGridViewTextBoxCell myCell = V_Data.Rows[iRow].Cells[iCell] as DataGridViewTextBoxCell;

                    if (action == Actions.Auto) DoAction(Actions.ContextMenu, iRow, iCell);
                    else DoAction(action, iRow, iCell);
                    if (E_CellRightClicked != null) E_CellRightClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e, cell.Value));
                }
                else if (itemType == ItemTypes.CheckBox)
                {
                    if (action == Actions.Auto) DoAction(Actions.ContextMenu, iRow, iCell);
                    else DoAction(action, iRow, iCell);
                    if (E_CellRightClicked != null) E_CellRightClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e, cell.Value));
                }
                else if (itemType == ItemTypes.ComboBox)
                {
                    if (action == Actions.Auto) DoAction(Actions.ContextMenu, iRow, iCell);
                    else DoAction(action, iRow, iCell);
                    if (E_CellRightClicked != null) E_CellRightClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e, cell.Value));
                }
                else if (itemType == ItemTypes.Button)
                {
                    if (action == Actions.Auto) DoAction(Actions.ContextMenu, iRow, iCell);
                    else DoAction(action, iRow, iCell);
                    if (E_CellRightClicked != null) E_CellRightClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e, cell.Value));
                }
                else if (itemType == ItemTypes.CloseButton)
                {
                    if (action == Actions.Auto)
                    {
                        DoAction(Actions.ContextMenu, iRow, iCell);
                        if (E_CellRightClicked != null) E_CellRightClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e, cell.Value));
                    }
                    else if (action == Actions.Nothing)
                    {
                        CloseBtnClicked(iRow, iCell, e);
                    }
                    else
                    {
                        DoAction(action, iRow, iCell);
                        if (E_CellRightClicked != null) E_CellRightClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e, cell.Value));
                    }
                }
                else if (itemType == ItemTypes.Image)
                {
                    if (action == Actions.Auto) DoAction(Actions.ContextMenu, iRow, iCell);
                    else DoAction(action, iRow, iCell);
                    if (E_CellRightClicked != null) E_CellRightClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e, cell.Value));
                }
                else if (itemType == ItemTypes.ImageButton)
                {
                    if (action == Actions.Auto) DoAction(Actions.ContextMenu, iRow, iCell);
                    else DoAction(action, iRow, iCell);
                    if (E_CellRightClicked != null) E_CellRightClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e, cell.Value));
                }
                else //header
                {
                    if (action == Actions.Auto) DoAction(Actions.ContextMenu, iRow, iCell);
                    else DoAction(action, iRow, -1);
                    if (E_CellRightClicked != null) E_CellRightClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e, cell.Value));
                }
                OnRightClicked(V_Data, new CellClickEventArgs(-1, -1, null, itemType, null, V_Data, e, cell.Value));

            }

            

        }

        /// <summary>
        /// 셀의 속성이 Enabled==false일 때에서 이벤트를 발생시킴..
        /// </summary>
        public bool EventEnabledWhenCellDisabled = true;

        void doLeftClick(EasyGridRow row, DataGridViewCell cell, int iRow, int iCell, ItemTypes itemType, MouseEventArgs e)
        {
            if (row.Cells.Count <= iCell) return;
            if (row[iCell]==null || (row[iCell].Enabled == false && EventEnabledWhenCellDisabled==false)) return;
            Point posInCell = new Point();
            if (cell != null && iRow >= 0 && iCell >= 0)
            {
                int x = 0;
                //for(int i=0; i<iCell; i++) x+=Wid[i];
                posInCell.X = e.X - x;
                posInCell.Y = e.Y;
            }
            Actions action;
            try
            {
                cell.Selected = true;
            }
            catch
            {
                return;
            }

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
            IEasyGridCell eCell = cell as IEasyGridCell;

            if (itemType == ItemTypes.ImageCheckBox)
            {
                //DataGridViewCheckBoxCell myCell = V_Data.Rows[iRow].Cells[iCell] as DataGridViewCheckBoxCell;

                if (action == Actions.Auto) OnCheckBoxClicked(iRow, iCell);
                else if (action == Actions.CheckBoxChecked) OnCheckBoxClicked(iRow, iCell);
                else if (action == Actions.Nothing) OnCheckBoxClicked(iRow, iCell);
                else DoAction(action, iRow, iCell);
                if(_itemTypes[iCell] != ItemTypes.Various) checkIfAllChecked(iCell);
                if (E_CellClicked != null) E_CellClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e, eCell));
                
            }
            else if (itemType == ItemTypes.TextBox)
            {
                //DataGridViewTextBoxCell myCell = V_Data.Rows[iRow].Cells[iCell] as DataGridViewTextBoxCell;
                EasyGridTextBoxCell myCell = V_Data.Rows[iRow].Cells[iCell] as EasyGridTextBoxCell;
                if (action == Actions.Auto)
                {
                    //do nothing..
                    if (E_CellClicked != null) E_CellClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e, myCell.Value));
                }
                else if (action == Actions.Modify)
                {
                    if ( myCell.IsEditable) BeginModifyMode(iRow, iCell);
                    else if (E_CellClicked != null) E_CellClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e, myCell.Value));
                }
                else
                {
                    DoAction(action, iRow, iCell);
                    if (E_CellClicked != null) E_CellClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e, myCell.Value));
                }

                //if (E_ListRowClicked != null) E_ListRowClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, cell, V_Data, e));
            }
            else if (itemType == ItemTypes.FileOpenBox)
            {
                //DataGridViewTextBoxCell myCell = V_Data.Rows[iRow].Cells[iCell] as DataGridViewTextBoxCell;
                EasyGridFileOpenBoxCell myCell = V_Data.Rows[iRow].Cells[iCell] as EasyGridFileOpenBoxCell;
                string before = myCell.Text;
                bool isChanged = myCell.MouseClick(posInCell);
                if (isChanged)
                {
                    if (E_TextChanged != null) E_TextChanged(this, new CellTextChangedEventArgs(before, myCell.Text, iCell, iRow, myCell.OwningRow, myCell));
                    if (E_TextEditFinished != null) E_TextEditFinished(this, new CellTextChangedEventArgs(before, myCell.Text, iCell, iRow, myCell.OwningRow, myCell));
                }
                else
                {

                    if (action == Actions.Auto)
                    {
                        //do nothing..
                        if (E_CellClicked != null) E_CellClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e, myCell.Value));
                    }
                    else if (action == Actions.Modify)
                    {
                        if (myCell.IsEditable) BeginModifyMode(iRow, iCell);
                        else if (E_CellClicked != null) E_CellClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e, myCell.Value));
                    }
                    else
                    {
                        DoAction(action, iRow, iCell);
                        if (E_CellClicked != null) E_CellClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e, myCell.Value));
                    }
                }
                //if (E_ListRowClicked != null) E_ListRowClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, cell, V_Data, e));
            }
            else if (itemType == ItemTypes.KeyValue)
            {
                //DataGridViewTextBoxCell myCell = V_Data.Rows[iRow].Cells[iCell] as DataGridViewTextBoxCell;
                if (action == Actions.Auto)
                {
                    //do nothing..
                    if (E_CellClicked != null) E_CellClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e, eCell));
                }
                else
                {
                    DoAction(action, iRow, iCell);
                    if (E_CellClicked != null) E_CellClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e, eCell));
                }

                //if (E_ListRowClicked != null) E_ListRowClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, cell, V_Data, e));
            }
                /*
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
                 */
            else if (itemType == ItemTypes.CheckBox)
            {
                // DataGridViewCheckBoxCell myCell = V_Data.Rows[iRow].Cells[iCell] as DataGridViewCheckBoxCell;

                if (action == Actions.Auto) OnCheckBoxClicked(iRow, iCell);
                else if (action == Actions.CheckBoxChecked) OnCheckBoxClicked(iRow, iCell);
                else if (action == Actions.Nothing) OnCheckBoxClicked(iRow, iCell);
                else DoAction(action, iRow, iCell);

                if (E_CellClicked != null) E_CellClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e, eCell));
            }
            else if (itemType == ItemTypes.ComboBox)
            {
                //comboBox는 leftclick시에 정해진 작업이 있으므로 다른 일을 하지 않는다.
                EasyGridComboBoxCell myCell = V_Data.Rows[iRow].Cells[iCell] as EasyGridComboBoxCell;
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
                    if (E_RadioButtonSelected != null) E_RadioButtonSelected(this, new CellRadioButtonSelectedEventArgs(myCell.GetValue(), iRow, iCell, myCell.Items[myCell.GetValue()].Text, myCell));
                    OnRadioButtonChanged(this, new CellRadioButtonSelectedEventArgs(myCell.GetValue(), iRow, iCell, myCell.Items[myCell.GetValue()].Text, myCell));

                    if (E_CellClicked != null) E_CellClicked(this, new CellClickEventArgs(iRow, iCell, row, itemType, myCell, V_Data, e, eCell));
                }
            }
            else if (itemType == ItemTypes.CheckBoxGroup)
            {
                EasyGridCheckBoxGroupCell myCell = V_Data.Rows[iRow].Cells[iCell] as EasyGridCheckBoxGroupCell;
                Point pt = V_Data.PointToClient(Control.MousePosition);
                int selected = myCell.ClickOnCell(pt.X, pt.Y);
                if (selected >= 0)
                {
                    if (E_CheckBoxGroupSelected != null) E_CheckBoxGroupSelected(this, new CellCheckBoxGroupSelectedEventArgs(selected, iRow, iCell, myCell.GetValue(), myCell));
                    OnCheckBoxGroupChanged(this, new CellCheckBoxGroupSelectedEventArgs(selected, iRow, iCell, myCell.GetValue(), myCell));

                    if (E_CellClicked != null) E_CellClicked(this, new CellClickEventArgs(iRow, iCell, row, itemType, myCell, V_Data, e, eCell));
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
                if (E_CellClicked != null) E_CellClicked(this, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e, eCell));
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
                if (E_CellClicked != null) E_CellClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e, eCell));
            }
            else if (itemType == ItemTypes.ImageButton)
            {
                // DataGridViewImageCell myCell = V_Data.Rows[iRow].Cells[iCell] as DataGridViewImageCell;

                if (action == Actions.Auto) DoAction(Actions.Nothing, iRow, iCell);
                else DoAction(action, iRow, iCell);
                if (E_CellClicked != null) E_CellClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e, eCell));
            }
            else if (itemType == ItemTypes.Header)// header
            {
                if (action == Actions.Auto) DoAction(Actions.Nothing, iRow, iCell);
                else DoAction(action, iRow, -1);
                if (E_CellClicked != null) E_CellClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e, eCell));
            }
            else //variaous
            {
                IEasyGridCell myCell = cell as IEasyGridCell;
                doLeftClick(row, cell, iRow, iCell, myCell.ItemType,e);
            }

            OnCellClicked(V_Data, new CellClickEventArgs(iRow, iCell, row, itemType, cell, V_Data, e, eCell));
        }

        protected virtual void OnRadioButtonChanged(EasyGridView sender, CellRadioButtonSelectedEventArgs args) { }
        protected virtual void OnCheckBoxGroupChanged(EasyGridView sender, CellCheckBoxGroupSelectedEventArgs args) { }

        public new void Dispose()
        {
            V_Data.Dispose();
            V_Data = null;
            
            GC.Collect(0, GCCollectionMode.Forced);
            
            GC.WaitForFullGCApproach();
            GC.WaitForFullGCComplete();
            GC.WaitForPendingFinalizers();
            this.Dispose(true);

        }

        public void ShowComboBoxDropDown(EasyGridComboBoxCell myCell)
        {
            bool isSameBox = (BeforeComboBox == ClickedComboBox) ? true : false;
            
            if (myCell.ContextMenuStrip != null && myCell.ContextMenuStrip.IsDisposed == false)
            {
                myCell.ContextMenuStrip.Dispose();
                 if(isSameBox) return;
            }
            
            int x = V_Data.RectangleToScreen(V_Data.Bounds).Left;
            int startCol = V_Data.FirstDisplayedScrollingColumnIndex;
            if (V_Data.RowHeadersVisible) x += V_Data.RowHeadersWidth;
            for (int i = startCol; i < myCell.ColumnIndex; i++)
            {
                x += Wid[i];
            }
            //x -= 20;
            int y = V_Data.RectangleToScreen(V_Data.Bounds).Top;
            if (V_Data.ColumnHeadersVisible) y += V_Data.ColumnHeadersHeight;
            int startRow = V_Data.FirstDisplayedScrollingRowIndex;
            for (int i = startRow; i <= myCell.RowIndex; i++)
            {
                y += V_Data.Rows[i].Height;
            }

            int width = Wid[myCell.ColumnIndex];

            if (myCell.ContextMenuStrip == null) myCell.ContextMenuStrip = new ContextMenuStrip(new Container());
            myCell.ContextMenuStrip.ShowCheckMargin = false;
            myCell.ContextMenuStrip.ShowImageMargin = false;
            myCell.ContextMenuStrip.AutoSize = true;     

            if (myCell.ContextMenuStrip.Items.Count > 0)
            {
                myCell.ContextMenuStrip.Items.Clear();
            }

            for (int i = 0; i < myCell.Items.Count; i++)
            {
                myCell.ContextMenuStrip.Items.Add(myCell.Items.ElementAt(i) as String);
            }

            if (myCell.ContextMenuStrip.Width < width)
            {
                int height = myCell.ContextMenuStrip.Height;// +2 * myCell.ContextMenuStrip.Items.Count;
                //myCell.ContextMenuStrip.AutoSize = false;
                myCell.ContextMenuStrip.Width = width;
                myCell.ContextMenuStrip.AllowTransparency = true;
                myCell.ContextMenuStrip.BackColor = Color.FromArgb(0x55ffffff);
                //myCell.ContextMenuStrip.SetBounds(0, 0, width, height, BoundsSpecified.Size);
                
            }
            myCell.ContextMenuStrip.ItemClicked += new ToolStripItemClickedEventHandler(ComboBoxItemContextClicked);
            
            try
            {
                myCell.ContextMenuStrip.Show(new Point(x, y));//Control.MousePosition);
            }catch(Exception e){
                throw e;
            }
            
        }


        public DataGridViewCell Cell(PositionOnList pos)
        {
            if (pos.Row >= 0 && pos.Col >= 0)
                return V_Data.Rows[pos.Row].Cells[pos.Col];
            else return null;
        }

        public IEasyGridCell Cell(int row, int col)
        {
            if (V_Data.Rows.Count>row && row>=0){
                if (V_Data.Rows[row] == null)
                {
                    return null;
                }
                else if(V_Data.Rows[row].Cells.Count>0 && col>=0)
                {
                    if (V_Data.Rows[row].Cells[col] == null)
                    {
                        return null;
                    }
                }
            }


            if (row >= 0 && col >= 0)
                return V_Data.Rows[row].Cells[col] as IEasyGridCell;
            else if (row < 0 && col>=0)
            {
                return this.Column(col).HeaderCell;//.Columns[col].HeaderCell;
            }
            else return null;
        }

        void ComboBoxItemContextClicked(object sender, ToolStripItemClickedEventArgs e)
        {

            //MessageBox.Show(ClickedRow + "," + ClickedCol + "/"+e.ClickedItem.Text);
            if (ClickedComboBox.Row < 0 || ClickedComboBox.Col < 0) return;
            EasyGridComboBoxCell cell = Cell(ClickedComboBox) as EasyGridComboBoxCell;
            
            int index = cell.Items.ToList().IndexOf(e.ClickedItem.Text);//.MergeIndex;
            
            if(cell.Items.Contains(e.ClickedItem.Text)){
                //cell.Value = e.ClickedItem.Text;// cell.Items.IndexOf(value);
                cell.ValueMember = e.ClickedItem.Text;
                cell.Value = e.ClickedItem.Text;

                if (E_ComboBoxChanged != null)
                {
                    //DataGridViewComboBoxCell cell = V_Data.Rows[ComboSelectedRow].Cells[ComboSelectedCol] as DataGridViewComboBoxCell;
                    int selectedIndex = cell.Items.ToList().IndexOf(cell.ValueMember);
                    DataGridViewRow row = V_Data.Rows[ClickedComboBox.Row];
                    CellComboBoxEventArgs arg = new CellComboBoxEventArgs(selectedIndex, cell.ValueMember, ClickedComboBox.Row, ClickedComboBox.Col, V_Data, row, cell);
                    E_ComboBoxChanged(this, arg);
                }
            }
            //SetValueInCell(ComboSelectedRow, ComboSelectedCol, e.ClickedItem.Text);
            //cell.ValueMember = e.ClickedItem.Text;
            //cell.Value = e.ClickedItem.Text;
            BeforeComboBox.Set(-1, -1);
        }
        void DrawRowHeader(DataGridViewCellPaintingEventArgs e)
        {
            Graphics g = e.Graphics;
            DataGridViewHeaderCell cell = V_Data.Rows[e.RowIndex].HeaderCell;
            Point pt = Control.MousePosition;
            pt = V_Data.PointToClient(pt);
            if(e.CellBounds.Contains(pt)) CellFunctions.DrawPlainHeaderBack(e.CellBounds, g, Color.WhiteSmoke, true);
            else CellFunctions.DrawPlainHeaderBack(e.CellBounds, g, Color.WhiteSmoke, cell.Selected);
            SizeF size = CellFunctions.TextSize(cell.Value as String, g, V_Data.Font);
            float y = CellFunctions.TextCenterYInRact(e.CellBounds, size.Height);
            g.DrawString(cell.Value as String, V_Data.Font, Brushes.Black, new PointF(e.CellBounds.X + (e.CellBounds.Width - size.Width), y));
            e.Handled = true;
            
        }

        //enum CellRePaintOptions { ImageCheckBoxHeaderChanged = 0, None };
        //CellRePaintOptions _cellRePaintOption = CellRepaintOptions.None;
        Point _oldButton = new Point(-1, -1);
        Point _oldCell = new Point(-1, -1);
        void V_Data_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            //ImageCheckBoxHeaderChanged(e);
            bool isChanged = false;
            if (e.ColumnIndex >= 0 && e.RowIndex < 0)
            {
                IEasyGridColumn col = V_Data.Columns[e.ColumnIndex] as IEasyGridColumn;
                if(col!=null)  col.OnPaint(e);
                return;
            }
            
            if (e.ColumnIndex < 0 && e.RowIndex >= 0)
            {
                DrawRowHeader(e);
                return;
            }
            else if(e.ColumnIndex>=0 && e.RowIndex>=0)//일반 cell
            {

                
            }
            
            /*
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
             */
        }

        /*
        void SetCellRepaint(CellRepaintOptions option)
        {
            _cellRePaintOption = option;
            V_Data.Refresh();
        }
         */
        /*
        void ImageCheckBoxHeaderChanged(DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex>=0) return;
            if (_itemTypes.Count <= e.ColumnIndex) return;

            if (_itemTypes[e.ColumnIndex] == ItemTypes.ImageCheckBox)
            {
                e.PaintBackground(e.ClipBounds, false);

                EasyGridImageCheckBoxColumn col = V_Data.Columns[e.ColumnIndex] as EasyGridImageCheckBoxColumn;
                //Image[] imgs = _titleInitData[e.ColumnIndex] as Image[];
                
                int checkState = col.Checked;
                Image img = imgs[checkState];

                Point pt = new Point(((e.CellBounds.Width - img.Width) / 2) + e.CellBounds.Location.X,((e.CellBounds.Height - img.Height)/2)+ e.CellBounds.Location.Y);
                 
                e.Graphics.DrawImage(img,pt);
                e.Handled = true;
            }
        }
        */
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
        public new void SuspendLayout()
        {
            V_Data.SuspendLayout();
            base.SuspendLayout();
        }

        public new void ResumeLayout()
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
        /// 새로운 Row를 추가합니다. 호환성을 위해서 제공하는 function입니다. AddItem보다 약간 더 빠릅니다.
        /// 툴팁을 추가하는 방법은, values와 tooltip의 항목을 같게 하여 추가하는 것입니다. 그렇지 않으면 제대로 추가되지 않습니다.
        /// 만일 하나의 cell에 여러 항목을 적어넣는 ComboBox나 CheckBox같은 경우에는 values[]의 각 항목은 배열이나
        /// List의 형태를 가지게 하면 됩니다.
        /// </summary>
        /// <param name="relativeObjName">이 줄에 연관된 데이터의 이름입니다. 사용할 때는 RowRelativeObject(rowIndex)["데이터이름"] 으로 가져갑니다.</param>
        /// <param name="relativeObject">이 줄에 연관된 데이터입니다. </param>
        /// <param name="values">실제 줄에 추가되는 데이터로서, 각 column에 맞는 형식의 값을 지정해야 합니다.</param>
        public EasyGridRow AddARow(String relativeObjName, Object relativeObject, ICollection<object> values, ICollection<String> tooltips = null)
        {
            EasyGridRow row = makeRow(values, tooltips);
            
            row.HeaderCell.Value = V_Data.Rows.Count.ToString();//index추가.

            if (relativeObjName != null)
            {
                row.RelativeObject[relativeObjName] = relativeObject;
            }

            V_Data.Rows.Add(row);
            CheckMaxLines();
            return row;
        }

        /// <summary>
        /// 일부의 값만으로 row를 추가할 수 있다.
        /// </summary>
        /// <param name="relativeObjName"></param>
        /// <param name="relativeObject"></param>
        /// <param name="values"></param>
        /// <param name="tooltips"></param>
        /// <returns></returns>
        public EasyGridRow AddARow(String relativeObjName, Object relativeObject, Dictionary<String, object> values, ICollection<String> tooltips = null)
        {
            List<object> listValue = new List<object>();
            for (int i = 0; i < ColumnCount; i++)
            {
                String name = ColumnNames[i];
                if (values.Keys.Contains(name))
                {
                    listValue.Add(values[name]);
                }
                else
                {
                    listValue.Add(null);
                }
            }

            return AddARow(relativeObjName, relativeObject, listValue, tooltips);
        }

        /// <summary>
        /// 일부의 값만으로 row를 추가할 수 있다.
        /// </summary>
        /// <param name="relativeObjName"></param>
        /// <param name="relativeObject"></param>
        /// <param name="values"></param>
        /// <param name="tooltips"></param>
        /// <returns></returns>
        public EasyGridRow AddARow(String relativeObjName, Object relativeObject, Dictionary<int, object> values, ICollection<String> tooltips = null)
        {
            List<object> listValue = new List<object>();
            for (int i = 0; i < ColumnCount; i++)
            {
                if (values.Keys.Contains(i))
                {
                    listValue.Add(values[i]);
                }
                else
                {
                    listValue.Add(null);
                }
            }

            return AddARow(relativeObjName, relativeObject, listValue, tooltips);
        }

        void CheckMaxLines()
        {
            if(MaxLines>0 &&  _clearLinesWhenMaxLines>0){
            if (RowCount > _maxLines)
            {

                int lines = _clearLinesWhenMaxLines;
                while (lines-- > 0)
                {
                    if (Rows.Count > 0) Rows.RemoveAt(0);// V_Data.Rows.RemoveAt(0);
                    else
                    {
                        lines++;//지워지지 않았다.
                        break;
                    }
                }
                RefreshList();
                if (E_LineRemovedByMaxLinesLimit != null)
                {
                    E_LineRemovedByMaxLinesLimit(this, _clearLinesWhenMaxLines - lines);
                }
            }
            }
            
        }
        /// <summary>
        /// 새로운 Row를 추가합니다. Array형식의 데이터를 차례대로 추가합니다.
        /// </summary>
        /// <param name="relativeObjs"></param>
        /// <param name="values"></param>
        /// <param name="tooltips"></param>
        public EasyGridRow AddARow(Dictionary<String, Object> relativeObjs, ICollection<object> values, ICollection<String> tooltips = null)
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
            CheckMaxLines();
            
            return row;
        }

        /// <summary>
        /// 새로운 Row를 추가합니다. Array형식의 데이터를 차례대로 추가합니다.
        /// </summary>
        /// <param name="rowIndex">해당 index의 뒤에 새 Row가 삽입됩니다. -1을 적으면 가장 앞에 삽입됩니다.</param>
        /// <param name="relativeObjs"></param>
        /// <param name="values"></param>
        /// <param name="tooltips"></param>
        public EasyGridRow InsertARow(int rowIndex, Dictionary<String, Object> relativeObjs, ICollection<object> values, ICollection<String> tooltips = null)
        {
            EasyGridRow row = makeRow(values, tooltips);

            row.HeaderCell.Value = V_Data.Rows.Count.ToString();//index추가.

            if (rowIndex >= V_Data.RowCount - 1) V_Data.Rows.Add(row);
            else if (rowIndex < 0) rowIndex = -1;
            else V_Data.Rows.Insert(rowIndex + 1, row);//.Add(row);

            if (relativeObjs != null)
            {
                for (int i = 0; i < relativeObjs.Count; i++)
                {
                    row.RelativeObject[relativeObjs.Keys.ElementAt(i)] = relativeObjs.Values.ElementAt(i);
                }
            }
            CheckMaxLines();

            return row;
        }

        EasyGridRow makeRow(ICollection<object> values, ICollection<String> tooltips = null)
        {
            EasyGridRow row = new EasyGridRow(this);
            row.MakeCells(values, tooltips);
            if (row.Cells.Count < V_Data.Columns.Count)
            {
                int start = row.Cells.Count;
                for(int i=start; i<V_Data.Columns.Count; i++){
                    row.AddItemInRow(row, null , null);
                }
            }
            row.Height = BaseRowHeight;
            
            return row;
        }
        public int BaseRowHeight
        {
            get { return V_Data.BaseRowHeight; }
            set { V_Data.BaseRowHeight = value;}// _baseRowHeight = value; }
        }
        /// <summary>
        /// 새로운 Row를 추가합니다.
        /// </summary>
        /// <param name="values">실제 줄에 추가되는 데이터로서, 각 column에 맞는 형식의 값을 지정해야 합니다.</param>
        public EasyGridRow AddItem(params object[] values)
        {
            return AddARow(null, values, null);
            //AddItemWithRelative(null, null, values);
        }


        public EasyGridRow AddARowWithInfo(params EasyGridCellInfo[] infos)
        {
            return AddARow(null, infos, null);
        }

        public EasyGridRow AddARow(ICollection<object> values)
        {
            return AddARow(null, values,null);
        }

        #endregion

        #region Getters

        public List<Object> GetAColumnData(int index)
        {
            List<Object> aCol = new List<object>();
            foreach (EasyGridRow row in Rows)
            {
                aCol.Add(row[index].Value);
            }

            return aCol;
        }

        public List<Object> GetAColumnData(String name)
        {
            int index = V_Data.Columns[name].Index;
            return GetAColumnData(index);
        }

        public List<String> GetAColumnDataWithString(int index)
        {
            List<String> aCol = new List<String>();
            foreach (EasyGridRow row in Rows)
            {
                aCol.Add(row[index].Value as String);
            }

            return aCol;
        }
        
        
        public List<String> GetAColumnDataWithString(String name)
        {
            int index = V_Data.Columns[name].Index;
            return GetAColumnDataWithString(index);
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
        public CustomDictionary<String, Object> RowRelativeObject(int index)
        {
            if (index >= 0 && V_Data.Rows.Count>index)
            {
                EasyGridRow row = V_Data.Rows[index] as EasyGridRow;
                return row.RelativeObject;
            }
            else return new CustomDictionary<string, object>();
        }

        public bool? isRowChecked(int index)
        {
            int col = FindCheckBoxColumn();
            if (V_Data.Rows[index].Cells[col] is EasyGridCheckBoxCell)
            {
                EasyGridCheckBoxCell cell = V_Data.Rows[index].Cells[col] as EasyGridCheckBoxCell;
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
            else if (itemType == ItemTypes.FileOpenBox)
            {
                EasyGridFileOpenBoxCell cell = row.Cells[iCell] as EasyGridFileOpenBoxCell;
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
                return cell.GetValueInt();//.Value;
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
            else if (itemType == ItemTypes.ImageButton)
            {
                EasyGridImageButtonCell cell = row.Cells[iCell] as EasyGridImageButtonCell;
                return cell.GetValueInt();//.Value;
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
            EasyGridComboBoxCell cell = Rows[rowIndex].Cells[colIndex] as EasyGridComboBoxCell;
            //EasyGridComboBoxColumn col = V_Data.Columns[colIndex] as EasyGridComboBoxColumn;
            return cell.SelectedIndex;
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
            try
            {
                V_Data.Rows[rowIndex].Cells[colIndex].Selected = selected;
            }
            catch { }
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
            IEasyGridCell cell = row.Cells[colIndex] as IEasyGridCell;
            #region 전처리
            if (cell.ItemType == ItemTypes.Button)
            {
            }
            else if (cell.ItemType == ItemTypes.CheckBox)
            {
            }
            else if (cell.ItemType == ItemTypes.CheckBoxGroup)
            {
            }
            else if (cell.ItemType == ItemTypes.CloseButton)
            {
            }
            else if (cell.ItemType == ItemTypes.ComboBox)
            {
                EasyGridComboBoxCell myCell = cell as EasyGridComboBoxCell;
                EasyGridComboBoxColumn col = V_Data.Columns[colIndex] as EasyGridComboBoxColumn;
                if (myCell.Items.Count == 0) myCell.Items = col.Items;
                
            }
            else if (cell.ItemType == ItemTypes.Header)
            {
            }
            else if (cell.ItemType == ItemTypes.Image)
            {
                EasyGridImageColumn col = V_Data.Columns[colIndex] as EasyGridImageColumn;
                EasyGridImageCell myCell = cell as EasyGridImageCell;
                if (myCell.Images == null || myCell.Images.Count == 0)
                {
                    if (col.Images.Count > 0) myCell.Images = col.Images;
                }
            }
            else if (cell.ItemType == ItemTypes.ImageButton)
            {
                EasyGridImageButtonColumn col = V_Data.Columns[colIndex] as EasyGridImageButtonColumn;
                EasyGridImageButtonCell myCell = cell as EasyGridImageButtonCell;
                if (myCell.Images == null || myCell.Images.Count == 0)
                {
                    if (col.Images.Count > 0) myCell.Images = col.Images;
                    if (col.UseColumnTextForButtonValue) myCell.Text = col.Text;
                }
            }
            else if (cell.ItemType == ItemTypes.ImageCheckBox)
            {
            }
                /*
            else if (cell.ItemType == ItemTypes.KeyColor)
            {
            }
                 */
            else if (cell.ItemType == ItemTypes.KeyValue)
            {
            }
            else if (cell.ItemType == ItemTypes.RadioButton)
            {
            }
            else if (cell.ItemType == ItemTypes.TextBox)
            {
            }
            else if (cell.ItemType == ItemTypes.FileOpenBox)
            {
            }
            else if (cell.ItemType == ItemTypes.Various)
            {
            }
            #endregion

            cell.Value = value;

            #region 후처리
            if (cell.ItemType == ItemTypes.Button)
            {
            }
            else if (cell.ItemType == ItemTypes.CheckBox)
            {
            }
            else if (cell.ItemType == ItemTypes.CheckBoxGroup)
            {
            }
            else if (cell.ItemType == ItemTypes.CloseButton)
            {
            }
            else if (cell.ItemType == ItemTypes.ComboBox)
            {

            }
            else if (cell.ItemType == ItemTypes.Header)
            {
            }
            else if (cell.ItemType == ItemTypes.Image)
            {
               
            }
            else if (cell.ItemType == ItemTypes.ImageButton)
            {

            }
            else if (cell.ItemType == ItemTypes.ImageCheckBox)
            {
                int checkCount = 0;
                for (int i = 0; i < V_Data.Rows.Count; i++)
                {
                    EasyGridImageCheckBoxCell myCell = Cell(i, colIndex) as EasyGridImageCheckBoxCell;
                    if (myCell.IsChecked == true) checkCount++;
                }
                EasyGridImageCheckBoxColumn col = V_Data.Columns[colIndex] as EasyGridImageCheckBoxColumn;
                if (checkCount == 0) col.IsChecked = false;
                else if (checkCount == V_Data.Rows.Count) col.IsChecked = true;
                else col.IsChecked = null;
            }
                /*
            else if (cell.ItemType == ItemTypes.KeyColor)
            {
            }
                 */
            else if (cell.ItemType == ItemTypes.KeyValue)
            {
            }
            else if (cell.ItemType == ItemTypes.RadioButton)
            {
            }
            else if (cell.ItemType == ItemTypes.TextBox)
            {
            }
            else if (cell.ItemType == ItemTypes.FileOpenBox)
            {
            }
            else if (cell.ItemType == ItemTypes.Various)
            {
            }
            #endregion

            #region before
            /*
             * 
             * 
            if (itemType == ItemTypes.TextBox)
            {
                EasyGridTextBoxCell cell = row.Cells[colIndex] as EasyGridTextBoxCell;
                cell.Value = value;
            }
            else if (itemType == ItemTypes.KeyValue)
            {

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
                    ICollection<Image> imageList = (images != null) ? images : _titleInitData[colIndex] as Image[];
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
            */
            #endregion

            if (tooltip != null && tooltip.Length > 0)
            {
                DataGridViewCell tcell = row.Cells[colIndex];
                tcell.ToolTipText = tooltip;
                
            }
        }
        public void SetCellStyle(int row, int col, DataGridViewCellStyle style)
        {
            V_Data.Rows[row].Cells[col].Style = style;
        }

        #endregion

        #region AddTitleFunctions

        public EasyGridCheckBoxColumn AddTitleCheckBoxColumn(int wid, String columnName, Boolean threeState = false, Actions actionOnClick = Actions.CommonAction, Actions actionOnDoubleClick = Actions.Auto, Actions actionOnRightClick = Actions.Auto)
        {
            EasyGridCheckBoxColumn col = new EasyGridCheckBoxColumn(V_Data, threeState);
            if (wid < 0)
            {
                wid = 15;
            }
            col.Name = columnName;
            col.HeaderText = "";

            col.Width = wid;
            //_isThreeState.Add(threeState);
            col.ThreeState = threeState;

            col.TrueValue = 1;
            col.FalseValue = 0;
            col.IndeterminateValue = 2;
            //col.Tag = 0;
            DataGridViewColumn dc = col as DataGridViewColumn;
            dc.DataPropertyName = columnName;

            V_Data.Columns.Add(col);

            _itemTypes.Add(ItemTypes.CheckBox);
            
            //_editables.Add(false);
           // _titleInitData.Add(0);

            _columnActionOnClicked.Add(actionOnClick);
            _columnActionOnDoubleClicked.Add(actionOnDoubleClick);
            _columnActionOnRightClicked.Add(actionOnRightClick);
            return col;
        }


        public EasyGridImageCheckBoxColumn AddTitleImageCheckColumn(int wid, String columnName, String headerText, ICollection<Image> imageLists, Actions actionOnClick = Actions.CommonAction, Actions actionOnDoubleClick = Actions.Auto, Actions actionOnRightClick = Actions.Auto)
        {

            EasyGridImageCheckBoxColumn col = new EasyGridImageCheckBoxColumn();
            if (wid < 0)
            {
                if (imageLists != null && imageLists.Count > 0)
                {
                    wid = imageLists.ElementAt(0).Width;
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
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
            DataGridViewColumn dc = col as DataGridViewColumn;
            dc.DataPropertyName = columnName;

            if (imageLists != null && imageLists.Count > 0) col.Images = imageLists;

            col.HeaderCell.Value = col.Image;// imageLists[0]; //기본 이미지로 title 이미지를 교체..
            //_titleInitData.Add(imageLists);
            //col.Tag = 0;
            //col.HeaderText = "";
            //col.Image = imageLists[0];
            //Bitmap btm = new Bitmap(imageLists[0]);

            //col.Image = btm;// Icon.FromHandle(btm.GetHicon());
            V_Data.Columns.Add(col);

            _itemTypes.Add(ItemTypes.ImageCheckBox);
            //_isThreeState.Add(threeState);
            //_editables.Add(false);

            _columnActionOnClicked.Add(actionOnClick);
            _columnActionOnDoubleClicked.Add(actionOnDoubleClick);
            _columnActionOnRightClicked.Add(actionOnRightClick);

            col.Text = headerText;
            //SetCellRepaint(CellRepaintOptions.ImageCheckBoxHeaderChanged);
            return col;
        }
        public EasyGridImageCheckBoxColumn AddTitleImageCheckColumn(int wid, String columnName, Boolean threeState, ICollection<Image> imageLists, Actions actionOnClick = Actions.CommonAction, Actions actionOnDoubleClick = Actions.Auto, Actions actionOnRightClick = Actions.Auto)
        {

            EasyGridImageCheckBoxColumn col = new EasyGridImageCheckBoxColumn();
            if (wid < 0)
            {
                if (imageLists != null && imageLists.Count > 0)
                {
                    wid = imageLists.ElementAt(0).Width;
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
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
            DataGridViewColumn dc = col as DataGridViewColumn;
            dc.DataPropertyName = columnName;

            if(imageLists!=null && imageLists.Count>0) col.Images = imageLists;

            col.HeaderCell.Value = col.Image;// imageLists[0]; //기본 이미지로 title 이미지를 교체..
            //_titleInitData.Add(imageLists);
            //col.Tag = 0;
            //col.HeaderText = "";
            //col.Image = imageLists[0];
            //Bitmap btm = new Bitmap(imageLists[0]);

            //col.Image = btm;// Icon.FromHandle(btm.GetHicon());
            V_Data.Columns.Add(col);

            _itemTypes.Add(ItemTypes.ImageCheckBox);
            //_isThreeState.Add(threeState);
            //_editables.Add(false);

            _columnActionOnClicked.Add(actionOnClick);
            _columnActionOnDoubleClicked.Add(actionOnDoubleClick);
            _columnActionOnRightClicked.Add(actionOnRightClick);

            //SetCellRepaint(CellRepaintOptions.ImageCheckBoxHeaderChanged);
            return col;
        }

        public EasyGridImageCheckBoxColumn AddTitleImageCheckColumn(int wid, String columnName, Boolean threeState = false, CheckBoxColors checkBoxColor = CheckBoxColors.Red, Actions actionOnClick = Actions.CommonAction, Actions actionOnDoubleClick = Actions.Auto, Actions actionOnRightClick = Actions.Auto)
        {
            Image[] list;
            if (checkBoxColor == CheckBoxColors.Blue)
            {
                list = new Image[]{
                    Properties.Resources.check_normal,
                    Properties.Resources.check_blue,
                    Properties.Resources.check_inter,
                    Properties.Resources.check_normal_press
                };
            }
            else
            {
                list = new Image[]{
                    Properties.Resources.check_normal,
                    Properties.Resources.check_red,
                    Properties.Resources.check_inter,
                    Properties.Resources.check_normal_press
                };
            }

            return AddTitleImageCheckColumn(wid, columnName, threeState, list, actionOnClick, actionOnDoubleClick, actionOnRightClick);
        }

        public EasyGridImageCheckBoxColumn AddTitleImageCheckColumn(int wid, String columnName, String headerText, CheckBoxColors checkBoxColor = CheckBoxColors.Red, Actions actionOnClick = Actions.CommonAction, Actions actionOnDoubleClick = Actions.Auto, Actions actionOnRightClick = Actions.Auto)
        {
            Image[] list;
            if (checkBoxColor == CheckBoxColors.Blue)
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

            EasyGridImageCheckBoxColumn col= AddTitleImageCheckColumn(wid, columnName, false, list, actionOnClick, actionOnDoubleClick, actionOnRightClick);
            col.Text = headerText;
            return col;
        }

        public EasyGridComboBoxColumn AddTitleComboBoxColumn(int wid, String columnName, String TitleText, String[] items, int SelectedIndex=0, Actions actionOnClick = Actions.CommonAction, Actions actionOnDoubleClick = Actions.Auto, Actions actionOnRightClick = Actions.Auto)
        {
            EasyGridComboBoxColumn col = new EasyGridComboBoxColumn(V_Data);
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
            DataGridViewColumn dc = col as DataGridViewColumn;
            dc.DataPropertyName = columnName;

            if (items != null)
            {
                foreach (String txt in items)
                {
                    col.Items.Add(txt);
                }
            }
            col.MaxDropDownItems = 100;
            col.SelectedIndex = SelectedIndex;

            col.DropDownWidth = 100;
            V_Data.Columns.Add(col);

            _itemTypes.Add(ItemTypes.ComboBox);
            //_editables.Add(false);
            //_titleInitData.Add(0);
            //_isThreeState.Add(false);
            _columnActionOnClicked.Add(actionOnClick);
            _columnActionOnDoubleClicked.Add(actionOnDoubleClick);
            _columnActionOnRightClicked.Add(actionOnRightClick);
            return col;
        }

        public EasyGridRadioButtonColumn AddTitleRadioButtonColumn(int wid, String columnName, String TitleText, String[] items, int initSelected = 0, Actions actionOnClick = Actions.CommonAction, Actions actionOnDoubleClick = Actions.Auto, Actions actionOnRightClick = Actions.Auto)
        {
            EasyGridRadioButtonColumn col = new EasyGridRadioButtonColumn(V_Data);
            
            if (wid < 0)
            {
                wid = 2;
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            col.Name = columnName;
            col.HeaderText = TitleText;
            col.Width = wid;
            DataGridViewColumn dc = col as DataGridViewColumn;
            dc.DataPropertyName = columnName;

            if (items != null)
            {
                col.Items.Add(items);
            }
            col.Items.CheckItem(initSelected);
            col.SelectedIndex = initSelected;
            
            V_Data.Columns.Add(col);

            _itemTypes.Add(ItemTypes.RadioButton);
            //_editables.Add(null);
            //_titleInitData.Add(initSelected);

            //_isThreeState.Add(false);
            
            _columnActionOnClicked.Add(actionOnClick);
            _columnActionOnDoubleClicked.Add(actionOnDoubleClick);
            _columnActionOnRightClicked.Add(actionOnRightClick);
            return col;
        }

        public EasyGridCheckBoxGroupColumn AddTitleCheckBoxGroupColumn(int wid, String columnName, String TitleText, String[] items, int[] initSelected = null, Actions actionOnClick = Actions.CommonAction, Actions actionOnDoubleClick = Actions.Auto, Actions actionOnRightClick = Actions.Auto)
        {
            EasyGridCheckBoxGroupColumn col = new EasyGridCheckBoxGroupColumn(V_Data);

            if (wid < 0)
            {
                wid = 2;
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            col.Name = columnName;
            col.HeaderText = TitleText;
            col.Width = wid;
            DataGridViewColumn dc = col as DataGridViewColumn;
            dc.DataPropertyName = columnName;

            if (items != null)
            {
                col.Items.Add(items);
            }
            col.Items.SetCheckedItems(initSelected);

            V_Data.Columns.Add(col);

            _itemTypes.Add(ItemTypes.CheckBoxGroup);
            //_editables.Add(null);
            //_titleInitData.Add(initSelected);
            //_isThreeState.Add(false);

            _columnActionOnClicked.Add(actionOnClick);
            _columnActionOnDoubleClicked.Add(actionOnDoubleClick);
            _columnActionOnRightClicked.Add(actionOnRightClick);

            return col;
        }

        public EasyGridImageColumn AddTitleImageColumn(int wid, String columnName, String TitleText, String[] imagePaths, int titleShowImage = -1, Actions actionOnClick = Actions.CommonAction, Actions actionOnDoubleClick = Actions.CommonAction, Actions actionOnRightClick = Actions.CommonAction)
        {
            List<Image> imageLists = new List<Image>();
            for (int i = 0; i < imagePaths.Length; i++)
            {
                Image image = Image.FromFile(imagePaths[i]);
                imageLists.Add(image);
            }
            return AddTitleImageColumn(wid, columnName, TitleText, imageLists.ToArray(), titleShowImage, actionOnClick, actionOnDoubleClick, actionOnRightClick);
        }

        public EasyGridImageColumn AddTitleImageColumn(int wid, String columnName, String TitleText, ImageList.ImageCollection imageCollection, int titleShowImage = -1, Actions actionOnClick = Actions.CommonAction, Actions actionOnDoubleClick = Actions.CommonAction, Actions actionOnRightClick = Actions.CommonAction)
        {
            List<Image> imageLists = new List<Image>();
            foreach (Image image in imageCollection)
            {
                imageLists.Add(image);
            }
            return AddTitleImageColumn(wid, columnName, TitleText, imageLists.ToArray(), titleShowImage, actionOnClick, actionOnDoubleClick, actionOnRightClick);
        }

        public EasyGridImageColumn AddTitleImageColumn(int wid, String columnName, String TitleText, ICollection<Image> imageLists = null, int titleShowImage = -1, Actions actionOnClick = Actions.CommonAction, Actions actionOnDoubleClick = Actions.CommonAction, Actions actionOnRightClick = Actions.CommonAction)
        {
            EasyGridImageColumn col = new EasyGridImageColumn(V_Data);
            if (wid < 0)
            {
                if (imageLists != null && imageLists.Count > 0)
                {
                    wid = imageLists.ElementAt(0).Width;
                }
                else
                {
                    wid = 2;
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }

            }
            if (imageLists != null) col.Images = imageLists;
            if (titleShowImage >= 0) col.SelectedIndex = titleShowImage;
            else col.SelectedIndex = -1;

            col.Name = columnName;
            col.HeaderText = TitleText;
            col.Width = wid;
            DataGridViewColumn dc = col as DataGridViewColumn;
            dc.DataPropertyName = columnName;

            //if (titleShowImage >= 0 && imageLists.Length > titleShowImage) col.HeaderCell.Value = imageLists[titleShowImage];
            
            //_titleInitData.Add(imageLists);

            V_Data.Columns.Add(col);

            _itemTypes.Add(ItemTypes.Image);

            //_editables.Add(false);
            //_isThreeState.Add(false);
            _columnActionOnClicked.Add(actionOnClick);
            _columnActionOnDoubleClicked.Add(actionOnDoubleClick);
            _columnActionOnRightClicked.Add(actionOnRightClick);
            return col;
        }

        public EasyGridImageButtonColumn AddTitleImageButtonColumn(int wid, String columnName, String TitleText, String[] imagePaths, int titleShowImage = -1, bool UseColumnTextForButtonValue = false, bool showTitleText = false, Actions actionOnClick = Actions.CommonAction, Actions actionOnDoubleClick = Actions.CommonAction, Actions actionOnRightClick = Actions.CommonAction)
        {
            List<Image> imageLists = new List<Image>();
            for (int i = 0; i < imagePaths.Length; i++)
            {
                Image image = Image.FromFile(imagePaths[i]);
                imageLists.Add(image);
            }
            return AddTitleImageButtonColumn(wid, columnName, TitleText, imageLists.ToArray(), titleShowImage, UseColumnTextForButtonValue, showTitleText, actionOnClick, actionOnDoubleClick, actionOnRightClick);
        }

        public EasyGridImageButtonColumn AddTitleImageButtonColumn(int wid, String columnName, String TitleText, ImageList.ImageCollection imageCollection, int titleShowImage = -1, bool UseColumnTextForButtonValue = false, bool showTitleText = false, Actions actionOnClick = Actions.CommonAction, Actions actionOnDoubleClick = Actions.CommonAction, Actions actionOnRightClick = Actions.CommonAction)
        {
            List<Image> imageLists = new List<Image>();
            foreach (Image image in imageCollection)
            {
                imageLists.Add(image);
            }
            return AddTitleImageButtonColumn(wid, columnName, TitleText, imageLists.ToArray(), titleShowImage, UseColumnTextForButtonValue, showTitleText, actionOnClick, actionOnDoubleClick, actionOnRightClick);
        }

        public EasyGridImageButtonColumn AddTitleImageButtonColumn(int wid, String columnName, String TitleText, ICollection<Image> imageLists = null, int titleShowImage = -1, bool UseColumnTextForButtonValue = false, bool showTitleText = false, Actions actionOnClick = Actions.CommonAction, Actions actionOnDoubleClick = Actions.CommonAction, Actions actionOnRightClick = Actions.CommonAction)
        {
            EasyGridImageButtonColumn col = new EasyGridImageButtonColumn(V_Data);
            if (wid < 0)
            {
                if (imageLists != null && imageLists.Count > 0)
                {
                    wid = imageLists.ElementAt(0).Width;
                }
                else
                {
                    wid = 2;
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }

            }
            if (imageLists != null) col.Images = imageLists;
            if (titleShowImage >= 0) col.SelectedIndex = titleShowImage;
            else col.SelectedIndex = -1;

            col.Name = columnName;
            col.HeaderText = (showTitleText)? TitleText : "";
            col.Text = TitleText;
            col.Width = wid;
            col.UseColumnTextForButtonValue = UseColumnTextForButtonValue;
            
            DataGridViewColumn dc = col as DataGridViewColumn;
            dc.DataPropertyName = columnName;

            //if (titleShowImage >= 0 && imageLists.Length > titleShowImage) col.HeaderCell.Value = imageLists[titleShowImage];

            //_titleInitData.Add(imageLists);

            V_Data.Columns.Add(col);

            _itemTypes.Add(ItemTypes.ImageButton);

            //_editables.Add(false);
            //_isThreeState.Add(false);
            _columnActionOnClicked.Add(actionOnClick);
            _columnActionOnDoubleClicked.Add(actionOnDoubleClick);
            _columnActionOnRightClicked.Add(actionOnRightClick);
            return col;
        }

        public EasyGridButtonColumn AddTitleButtonColumn(int wid, String columnName, String TitleText, String baseText = "", Actions actionOnClick = Actions.CommonAction, Actions actionOnDoubleClick = Actions.CommonAction, Actions actionOnRightClick = Actions.CommonAction)
        {
            EasyGridButtonColumn col = new EasyGridButtonColumn(V_Data);
            if (wid < 0)
            {
                wid = 2;
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            col.Name = columnName;
            col.HeaderText = TitleText;
            col.Width = wid;
            //col.UseColumnTextForButtonValue = useTitleText;
            col.Text = baseText;
            DataGridViewColumn dc = col as DataGridViewColumn;
            dc.DataPropertyName = columnName;


            col.SortMode = DataGridViewColumnSortMode.NotSortable;
            
            V_Data.Columns.Add(col);

            _itemTypes.Add(ItemTypes.Button);
            //_editables.Add(false);
            //_titleInitData.Add(TitleText);
            //_isThreeState.Add(false);
            _columnActionOnClicked.Add(actionOnClick);
            _columnActionOnDoubleClicked.Add(actionOnDoubleClick);
            _columnActionOnRightClicked.Add(actionOnRightClick);
            return col;
        }

        public EasyGridCloseButtonColumn AddTitleCloseButtonColumn(int wid, String columnName, String TitleText, String baseText = "X", Actions actionOnClick = Actions.CommonAction, Actions actionOnDoubleClick = Actions.CommonAction, Actions actionOnRightClick = Actions.CommonAction)
        {
            EasyGridCloseButtonColumn col = new EasyGridCloseButtonColumn(V_Data);
            if (wid < 0)
            {
                wid = 2;
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            col.Name = columnName;
            col.HeaderText = TitleText;
            col.Width = wid;
            //col.UseColumnTextForButtonValue = useTitleText;
            col.Text = baseText;
            col.SortMode = DataGridViewColumnSortMode.NotSortable;
            DataGridViewColumn dc = col as DataGridViewColumn;
            dc.DataPropertyName = columnName;


            V_Data.Columns.Add(col);

            _itemTypes.Add(ItemTypes.CloseButton);
            //_titleInitData.Add(TitleText);
            //_editables.Add(false);
            //_isThreeState.Add(false);
            _columnActionOnClicked.Add(actionOnClick);
            _columnActionOnDoubleClicked.Add(actionOnDoubleClick);
            _columnActionOnRightClicked.Add(actionOnRightClick);
            return col;
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

        public EasyGridVariousColumn AddTitleVariousColumn(int wid, String columnName, String TitleText, Actions actionOnClick = Actions.CommonAction, Actions actionOnDoubleClick = Actions.CommonAction, Actions actionOnRightClick = Actions.CommonAction)
        {

            EasyGridVariousColumn col = new EasyGridVariousColumn(V_Data);

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
            DataGridViewColumn dc = col as DataGridViewColumn;
            dc.DataPropertyName = columnName;

            V_Data.Columns.Add(col);

            _itemTypes.Add(ItemTypes.Various);
            //_titleInitData.Add(TitleText);
            //_editables.Add(null);//null은 cell의 설정에 따라 바뀜을 의미함..
            //_isThreeState.Add(false);
            _columnActionOnClicked.Add(actionOnClick);
            if ( actionOnDoubleClick == Actions.CommonAction) _columnActionOnDoubleClicked.Add(Actions.Modify);
            else _columnActionOnDoubleClicked.Add(actionOnDoubleClick);
            _columnActionOnRightClicked.Add(actionOnRightClick);

            return col;
        }

        public EasyGridTextBoxColumn AddTitleTextBoxColumn(int wid, String columnName, String TitleText, Boolean editable, bool isAutoSort = false, TextAlignModes textAlignMode = TextAlignModes.None, Actions actionOnClick = Actions.CommonAction, Actions actionOnDoubleClick = Actions.CommonAction, Actions actionOnRightClick = Actions.CommonAction)
        {
            EasyGridTextBoxColumn col = new EasyGridTextBoxColumn(V_Data);

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
            col.CellTextAlignMode = textAlignMode;
            DataGridViewColumn dc = col as DataGridViewColumn;
            dc.DataPropertyName = columnName;

            V_Data.Columns.Add(col);

            _itemTypes.Add(ItemTypes.TextBox);
             
            //_titleInitData.Add(TitleText);
            col.IsEditable = editable;
            //_editables.Add(editable);
            //_isThreeState.Add(false);
            _columnActionOnClicked.Add(actionOnClick);
            if (editable == true && actionOnDoubleClick == Actions.CommonAction) _columnActionOnDoubleClicked.Add(Actions.Modify);
            else _columnActionOnDoubleClicked.Add(actionOnDoubleClick);
            _columnActionOnRightClicked.Add(actionOnRightClick);

            return col;
        }

        public EasyGridKeyValueColumn AddTitleKeyValueColumn(int wid, String columnName, String TitleText, TextAlignModes textAlignMode = TextAlignModes.None, Actions actionOnClick = Actions.CommonAction, Actions actionOnDoubleClick = Actions.CommonAction, Actions actionOnRightClick = Actions.CommonAction)
        {
            EasyGridKeyValueColumn col = new EasyGridKeyValueColumn(V_Data);

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
            
            DataGridViewColumn dc = col as DataGridViewColumn;
            dc.DataPropertyName = columnName;

            V_Data.Columns.Add(col);

            _itemTypes.Add(ItemTypes.KeyValue);

            //_titleInitData.Add(TitleText);
            
            //_editables.Add(editable);
            //_isThreeState.Add(false);
            _columnActionOnClicked.Add(actionOnClick);
            _columnActionOnDoubleClicked.Add(actionOnDoubleClick);
            _columnActionOnRightClicked.Add(actionOnRightClick);

            return col;
        }

        public EasyGridFileOpenBoxColumn AddTitleFileOpenBoxColumn(int wid, String columnName, String TitleText, Boolean editable, TextAlignModes textAlignMode = TextAlignModes.None, Actions actionOnClick = Actions.CommonAction, Actions actionOnDoubleClick = Actions.CommonAction, Actions actionOnRightClick = Actions.CommonAction)
        {
            EasyGridFileOpenBoxColumn col = new EasyGridFileOpenBoxColumn(V_Data);

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
            col.CellTextAlignMode = textAlignMode;
            DataGridViewColumn dc = col as DataGridViewColumn;
            dc.DataPropertyName = columnName;

            V_Data.Columns.Add(col);

            _itemTypes.Add(ItemTypes.FileOpenBox);

            //_titleInitData.Add(TitleText);
            col.IsEditable = editable;
            //_editables.Add(editable);
            //_isThreeState.Add(false);
            _columnActionOnClicked.Add(actionOnClick);
            if (editable == true && actionOnDoubleClick == Actions.CommonAction) _columnActionOnDoubleClicked.Add(Actions.Modify);
            else _columnActionOnDoubleClicked.Add(actionOnDoubleClick);
            _columnActionOnRightClicked.Add(actionOnRightClick);

            return col;
        }
        #endregion

        #region Property Changers
        public void HideTitleBar()
        {
            V_Data.ColumnHeadersVisible = false;
            V_Data.Invalidate();
            V_Data.Update();
        }

        public void ShowTitleBar()
        {
            V_Data.ColumnHeadersVisible = true;
            V_Data.Invalidate();
            V_Data.Update();    
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


        public void RefreshCell(int row, int col)
        {
            V_Data.InvalidateCell(col, row);
            V_Data.Update();
        }

        public void RefreshCell(IEasyGridCell cell)
        {
            V_Data.InvalidateCell(cell.ColumnIndex, cell.RowIndex);
            V_Data.Update();
        }

        public void RefreshRow(int row)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    V_Data.InvalidateRow(row);
                    V_Data.Update();
                }));
            }
            else
            {
                V_Data.InvalidateRow(row);
                V_Data.Update();
            }
            
        }

        public void RefreshColumn(int col)
        {

            if (_itemTypes[col] == ItemTypes.ImageCheckBox)
            {
                checkIfAllChecked(col);
            }

            V_Data.InvalidateColumn(col);
            V_Data.Update();

        }
        


        public void ClearTitles()
        {
            if (Rows.Count > 0)
            {
                Rows.Clear();
            }
            if (Header.Count > 0)
            {
                Header.Clear();
            }
            _columnActionOnClicked.Clear();
            _columnActionOnDoubleClicked.Clear();
            _columnActionOnRightClicked.Clear();
            _itemTypes.Clear();
            V_Data.ClearSpanInfo(false, true);
        }

        bool _isClearing = false;
        public void ClearData()
        {
            _isClearing = true;
            Rows.Clear();
            //_rowRelativeObject.Clear();
            V_Data.ClearSpanInfo(true, false);
            _isClearing = false;
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
            
            EasyGridTextBoxCell cell = V_Data.Rows[rowIndex].Cells[colIndex] as EasyGridTextBoxCell;
            if (cell == null) return;
            cell = cell.Span.SpanBaseCell as EasyGridTextBoxCell;// GetSpanBaseCell(cell) as EasyGridTextBoxCell;
            if (cell != null)
            {
                V_Data.IsEditActivated = true;
                V_Data.CurrentCell = cell;
                V_Data.BeginEdit(true, true);

            }
        }

        public override void Refresh()
        {
            V_Data.Invalidate();
            V_Data.Update();
        }


        public DataGridViewColumnCollection Columns
        {
            get { return V_Data.Columns; }
        }

        public IEasyGridColumn Column(int index)
        {
            return V_Data.Columns[index] as IEasyGridColumn;
        }

        public bool MultiSelect
        {
            get { return V_Data.MultiSelect; }
            set { V_Data.MultiSelect = value; }
        }

        public IEasyGridColumn Column(String name)
        {
            return V_Data.Columns[name] as IEasyGridColumn;
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
                    if (Cell(rowIndex, colIndex) == null)
                    {
                        if(V_Data.ContextMenu!=null) U_ContextMenu.Show(this, this.PointToClient(Control.MousePosition));
                    }else if((Cell(rowIndex, colIndex) as DataGridViewCell).ContextMenuStrip==null) U_ContextMenu.Show(this, this.PointToClient(Control.MousePosition));

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
                        EasyGridTextBoxCell cell = V_Data.Rows[rowIndex].Cells[colIndex] as EasyGridTextBoxCell;
                        if(cell!=null) Clipboard.SetText(cell.Value as String);
                        
                    }
                    break;
            }
        }

        public void ReleaseSelection()
        {
            V_Data.ClearSelection();
        }

        public void ReleaseAllRowColors()
        {
            for (int i = 0; i < RowCount; i++)
            {
                if(Rows[i].RowBackMode != RowBackModes.None) Rows[i].RowBackMode = RowBackModes.None;
            }
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
            Refresh();
            //V_Data.Refresh();
            
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
            if (item != null)
            {
                int index;
                if (item.Depth == 0)
                    index = U_ContextMenu.MenuItems.IndexOf(item);// ContextMenuItems.IndexOf(item);
                else
                    index = item.Parent.MenuItems.IndexOf(item);

                if (_contextMenuClickHandlers.ContainsKey(item)) _contextMenuClickHandlers[item].Invoke(this, new EasyGridMenuClickArgs(item.Text.ToString(), index, ClickedRow, ClickedCol, item, item.Depth));
                else if (E_ContextMenuClicked != null) E_ContextMenuClicked(this, new EasyGridMenuClickArgs(item.Text.ToString(), index, ClickedRow, ClickedCol, item, item.Depth));
                
                OnContextMenuClicked(this, new EasyGridMenuClickArgs(item.Text.ToString(), index, ClickedRow, ClickedCol, item, item.Depth));
            }
            else
            {
               
                
            }
            
        }
        protected virtual void OnContextMenuClicked(EasyGridView sender, EasyGridMenuClickArgs args){}
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
                IEasyGridCell eCell = rows[rowIndex].Cells[ColumnIndex] as IEasyGridCell;
                if (E_CellSelected != null && rowIndex > 0) E_CellSelected(this, new CellClickEventArgs(rows[rowIndex].Index, V_Data.CurrentCell.ColumnIndex, rows[rowIndex], _itemTypes[ColumnIndex], rows[rowIndex].Cells[ColumnIndex], V_Data, new EventArgs(), eCell.Value));
           
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
            if (E_CellSelected != null && rows.Count > 0) E_CellSelected(this, new CellClickEventArgs(rows[0].Index, V_Data.CurrentCell.ColumnIndex, rows[0], _itemTypes[ColumnIndex], rows[0].Cells[V_Data.CurrentCell.ColumnIndex], V_Data, new EventArgs(), V_Data.CurrentCell.Value));
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
                if (E_CellSelected != null && rowIndex > 0) E_CellSelected(this, new CellClickEventArgs(rows[rowIndex].Index, V_Data.CurrentCell.ColumnIndex, rows[rowIndex], _itemTypes[ColumnIndex], rows[rowIndex].Cells[V_Data.CurrentCell.ColumnIndex], V_Data, new EventArgs(), V_Data.CurrentCell.Value));
           


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
            //bool isAllFalse = true;
            int trueCount = 0;
            int allCheckBoxes = 0;
            IEasyGridCheckBoxCell checkCol = V_Data.Columns[colIndex] as IEasyGridCheckBoxCell;
            if (checkCol == null) return; //column이 checkbox 컬럼이 아니다.

            for (int i = 0; i < Rows.Count; i++)
            {
                IEasyGridCheckBoxCell cell = Rows[i].EasyCells[colIndex] as IEasyGridCheckBoxCell;// Cell(i, colIndex) as IEasyGridCheckBoxCell;
                if (cell == null) continue;
                allCheckBoxes++;
                if (cell.IsChecked == true) trueCount++;
            }
            if (allCheckBoxes == trueCount) checkCol.IsChecked = true;
            else if (trueCount == 0) checkCol.IsChecked = false;
            else checkCol.IsChecked = (bool?)null;
            /*
            if (isAllFalse) V_Data.Columns[colIndex].Tag = 0;
            else if (trueCount == Rows.Count) V_Data.Columns[colIndex].Tag = 1;
            else V_Data.Columns[colIndex].Tag = 2;
            */
            //SetCellRepaint(CellRepaintOptions.ImageCheckBoxHeaderChanged);
        }
        

        void DoTitleActions(int colIndex)
        {
            IEasyGridCheckBoxCell checkedCell;
            bool isChecked;

            switch (_itemTypes[colIndex])
            {
                case ItemTypes.ImageCheckBox:
                    checkedCell = V_Data.Columns[colIndex] as IEasyGridCheckBoxCell;
                    isChecked = (checkedCell.IsChecked==false)? true:false;
                    ToggleAllCheckBoxes(colIndex, isChecked);
                    checkedCell.IsChecked = isChecked;
                    //SetCellRepaint(CellRepaintOptions.ImageCheckBoxHeaderChanged);
                    break;
                case ItemTypes.Button:
                    break;
                case ItemTypes.CheckBox:
                    checkedCell = V_Data.Columns[colIndex] as IEasyGridCheckBoxCell;
                    isChecked = (checkedCell.IsChecked==false)? true:false;
                    ToggleAllCheckBoxes(colIndex, isChecked);
                    ToggleAllCheckBoxes(colIndex, isChecked);
                    checkedCell.IsChecked = isChecked;
                    break;
                case ItemTypes.CloseButton:
                    break;
                case ItemTypes.ComboBox:
                    break;
                case ItemTypes.TextBox:
                    break;
                case ItemTypes.Image:
                    break;
                case ItemTypes.ImageButton:
                    break;
                default:
                    break;
            }
        }

        void ToggleAllCheckBoxes(int colIndex, bool isChecked, int startRow = 0, int endRow=-1)
        {
            bool? toState = isChecked;
            if(endRow<0) endRow = V_Data.Rows.Count-1;
            
            //Image[] imageList = _titleInitData[colIndex] as Image[];
            
            List<int> added = new List<int>();
            List<int> removed = new List<int>();
            for (int i = 0; i < Rows.Count; i++)
            {
                IEasyGridCell icell = Rows[i].EasyCells[colIndex];// Cell(i, colIndex);
                IEasyGridCheckBoxCell cell = icell as IEasyGridCheckBoxCell;
                if (cell != null) //cell이 checkbox가 아닐 수도 있다.
                {
                    //beforeState = (bool?)GetValue(i, colIndex);
                    bool? orgState = cell.IsChecked;
                    if (orgState != toState)
                    {
                        if (orgState == true) removed.Add(i);
                        else if (orgState == false) added.Add(i);
                        //cell.IsChecked = isChecked;
                    }
                }
            }
            CellCheckedEventArgs args = new CellCheckedEventArgs(isChecked, colIndex, startRow, endRow, added, removed);
            if (E_CheckBoxChanged != null) E_CheckBoxChanged(this, args);

            if (args.IsCancel == false)
            {
                for (int i = 0; i < Rows.Count; i++)
                {
                    IEasyGridCheckBoxCell cell = Cell(i, colIndex) as IEasyGridCheckBoxCell;
                    if (cell != null)
                    {
                        //beforeState = (bool?)GetValue(i, colIndex);
                        bool? orgState = cell.IsChecked;
                        if (orgState != toState)
                        {
                            cell.IsChecked = isChecked;
                        }
                    }
                }
                RefreshList();
            }
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
        Keys _clickedKey = Keys.None;
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
            if (BeforeClickedRow >= 0 && (_clickedKey== Keys.None || _clickedKey == Control.ModifierKeys))
            {
                int min = (BeforeClickedRow < rowIndex) ? BeforeClickedRow : rowIndex;
                int max = (BeforeClickedRow < rowIndex) ? rowIndex : BeforeClickedRow;
                List<int> orgAdded = new List<int>(added);
                for (int i = min; i <= max; i++)
                {

                    //DataGridViewCheckBoxCell aCell = V_Data.Rows[i].Cells[BeforeClickedCol] as DataGridViewCheckBoxCell;

                    if (GetValue(i, BeforeClickedCol).Equals(false))
                    {
                        added.Add(V_Data.Rows[i].Index);
                        
                    }
                }
                
                CellCheckedEventArgs args = new CellCheckedEventArgs(true, colIndex, min, max, added, removed);

                if (E_CheckBoxChanged != null) E_CheckBoxChanged(this, args);
                OnCheckBoxChanged(V_Data, args);

                if (args.IsCancel == false)
                {
                    for (int i = min; i <= max; i++)
                    {

                        //DataGridViewCheckBoxCell aCell = V_Data.Rows[i].Cells[BeforeClickedCol] as DataGridViewCheckBoxCell;

                        if (GetValue(i, BeforeClickedCol).Equals(false))
                        {

                            SetValueInCell(i, BeforeClickedCol, true);
                        }
                    }
                }
                else
                {
                    added = orgAdded;
                }
                //BeforeClickedRow = -1;
                //BeforeClickedCol = -1;
            }
            else
            {
                //BeforeClickedRow = ClickedRow;
                //BeforeClickedCol = ClickedCol;
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
            if (BeforeClickedRow >= 0 && (_clickedKey == Keys.None || _clickedKey == Control.ModifierKeys))
            {
                int min = (BeforeClickedRow < rowIndex) ? BeforeClickedRow : rowIndex;
                int max = (BeforeClickedRow < rowIndex) ? rowIndex : BeforeClickedRow;
                List<int> removedOrg = new List<int>(removed);

                for (int i = min; i <= max; i++)
                {
                    if (GetValue(i, BeforeClickedCol).Equals(true))
                    {
                        removed.Add(V_Data.Rows[i].Index);
                        
                    }
                }
                CellCheckedEventArgs args = new CellCheckedEventArgs(false, colIndex, min, max, added, removed);
                if (E_CheckBoxChanged != null) E_CheckBoxChanged(this, args);
                OnCheckBoxChanged(V_Data, args);
                
                if (args.IsCancel == false)
                {
                    for (int i = min; i <= max; i++)
                    {
                        if (GetValue(i, BeforeClickedCol).Equals(true))
                        {
                            SetValueInCell(i, BeforeClickedCol, 0);
                        }
                    }
                }
            }
            else
            {
                //BeforeClickedRow = ClickedRow;
                //BeforeClickedCol = ClickedCol;
            }

            V_Data.ClearSelection();
        }
        protected virtual void OnCheckBoxChanged(DataGridView sender, CellCheckedEventArgs args) { }


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

    public delegate void LineRemovedByMaxLinesLimitEvent(object sender, int removedLines);






}
