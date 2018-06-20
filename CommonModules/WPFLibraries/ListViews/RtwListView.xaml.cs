using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPF_Handler;
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
namespace RtwWpfControls
{
    public partial class RtwListView : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public enum RtwListChildControl { CloseButton = 0, CheckBox };
        protected List<double> _wid = new List<double>();
        protected List<Boolean> _editables = new List<bool>();
        protected ClickCounter _clickCounter;
        protected Rectangle _sep = new Rectangle();
        protected ResourceHandler _rh;
        protected double _height = 20;
        protected HorizontalAlignment _dataHorizonAlign = HorizontalAlignment.Left;
        protected bool _checkBoxActivated = true;
        protected List<RtwListRow> _selectedItems = new List<RtwListRow>();

        public List<Object> _titles = new List<Object>();
        public Dictionary<Object, Object> _titleDic = new Dictionary<object, object>();
        
        
        public event ListRowClickEventHandler E_ListRowClicked;
        public event ListRowClickEventHandler E_ListRowDoubleClicked;
        public event ListRowClickEventHandler E_ListRowRemoving;
        public event ListRowClickEventHandler E_ListRowRemoved;
        public event ListCheckedEventHandler E_CheckBoxChanged;
        public event ListRowTextChangedEventHandler E_TextChanged;
        public event ListComboBoxEventHandler E_ComboBoxChanged;
        
        public enum Actions { Modify = 0, CheckBoxChecked, ContextMenu, Nothing };

        public Actions ActionOnDoubleClicked { get; set; }
        public Actions ActionOnClicked { get; set; }
        public Actions ActionOnRightClicked { get; set; }
        public List<double> Wid { get { return _wid; } set { _wid = value; } }
        
        
        public List<int> CheckedIndices{
            get{
                List<int> list = new List<int>();
             
                foreach (RtwListRow row in Rows)
                {
                    if (isRowChecked(row)==true) list.Add(row.Index);
                }
                return list;
            }
        }

        public bool? isRowChecked(RtwListRow aRow)
        {
            for (int i = 0; i < aRow.Count; i++)
            {
                UIElement col = aRow[i];
                if (col is CheckBox)
                {
                    Boolean isChecked = (Boolean)((col as CheckBox).IsChecked == true);
                    if (isChecked == true) return true;
                    else if (isChecked == false) return false;
                    else return null;
                }
            }
            return null;
        }
         
        public List<RtwListRow> CheckedRows = new List<RtwListRow>();
        List<RtwListRow> _rows = new List<RtwListRow>();
        ListBox _listView;
        ContextMenu _contextMenu = new ContextMenu();
        public bool CheckIfOtherCheckBoxExist = true;
        public bool CheckAllCheckBox = true;
        
        public List<RtwListRow> Rows {
            get{ return new List<RtwListRow>(_rows);}
            set {
                bool reset = false;
                if (_rows.Count > value.Count) reset = true;
                _rows = value;
                if(reset) resetIndex();
                NotifyPropertyChanged("Rows");
            }
        }

        RtwListRow _header = new RtwListRow();
        public RtwListRow Header {
            get { return _header; }
            set {
                _header = value;
                NotifyPropertyChanged("Header");
            } 
        }
        public HorizontalAlignment DataHorizonAlignment
        {
            get { return _dataHorizonAlign; }
            set
            {
                _dataHorizonAlign = value;
                if (Rows != null)
                {
                    for (int i = 0; i < Rows.Count; i++)
                    {
                        Rows[i].ItemHorizontalAlignment = value;
                    }
                }
                
            }
        }

        public void resetIndex()
        {
            int i=0;
            foreach (RtwListRow row in Rows)
            {
                _closeButtons[i].RowIndex = i;
                row.Index = i++;
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
                Header.ShowIndex = value;
                foreach (RtwListRow row in Rows)
                {
                    row.ShowIndex = value;
                }
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
                Header.IndexWidth = value;
                foreach (RtwListRow row in Rows)
                {
                    row.IndexWidth = value;
                }
            }
        }


        public List<MenuItem> ContextMenuItems = new List<MenuItem>();
        Dictionary<MenuItem,ContextMenuClickHandler> _contextMenuClickHandlers = new Dictionary<MenuItem,ContextMenuClickHandler>();
        public delegate void ContextMenuClickHandler(Object sender, String text, int index, int selectedRow, object MenuItem);
        public event ContextMenuClickHandler E_ContextMenuClicked = null;
        
        public void AddContextMenuItem(String text, ContextMenuClickHandler eventHandler=null, ContextMenu parent=null, String tooltip=null, String inputGesture=null)
        {
            if(parent==null) parent = U_ContextMenu;
            MenuItem item = new MenuItem();
            ContextMenuItems.Add(item);
            item.Header = text;
            item.Click +=new RoutedEventHandler(item_Click);
            if (tooltip != null && tooltip.Length > 0) item.ToolTip = tooltip;
            if (inputGesture != null && inputGesture.Length > 0) item.InputGestureText = inputGesture;
            if (eventHandler != null && E_ContextMenuClicked!=null) _contextMenuClickHandlers.Add(item, eventHandler);
            
            parent.Items.Add(item);
            
        }

        void item_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            int index = ContextMenuItems.IndexOf(item);
            if (_contextMenuClickHandlers.ContainsKey(item)) _contextMenuClickHandlers[item].Invoke(this, item.Header.ToString(), index, ClickedRow, item);
            else E_ContextMenuClicked(this, item.Header.ToString(), index, ClickedRow, item);
        }

        public double RowHeight
        {
            get { return _height; }
            set
            {
                _height = value;
                if (Rows == null) return;
                for (int i = 0; i < Rows.Count; i++)
                {
                    Rows[i].Height = value;
                }
            }
        }

        bool _isTitleVisible = true;
        public Boolean IsTitleVisible
        {
            get
            {
                return _isTitleVisible;
            }
            set
            {
                _isTitleVisible = value;

            }
        }

        public RtwListView()
            : base()
        {
            InitializeComponent();

            Rows = new List<RtwListRow>();
            Header.E_CellWidthChanged += new ListRowCellWidthChangedHandler(OnCellWidthChanged);
            //DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Normal, Dispatcher);
            //timer.Interval = new TimeSpan(2000);
            _clickCounter = new ClickCounter(200, this);
            _clickCounter.OnCountedClick += new CountedClickEventHandler(_clickCounter_OnCountedClick);
            ActionOnDoubleClicked = Actions.CheckBoxChecked;
            ActionOnClicked = Actions.Modify;
            ActionOnRightClicked = Actions.Nothing;
            _rh = new ResourceHandler(this.Resources);
            _rh.setControl(typeof(CheckBox), "CheckBoxTemplate");
            _rh.setControl(typeof(Rectangle), "SeperatorLine");
            _rh.SetStyle(_sep);
            _sep.Width = 1;
            RowHeight = 20;
            _sep.Height = 16;
            Header.ItemHorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            Header.setSeperator(_sep);
            Header.Height = RowHeight;
            HeaderPlace.Children.Add(Header);
            Header.IndexBackColor = Colors.Transparent;
            
            //E_ListRowClicked += new ListRowClickEventHandler(ListTable_E_ListRowClicked);
            //TestData();
            //this.ContentTemplate = this.Resources["dTemplate"] as DataTemplate;
            //this.ContentTemplate = this.Resources["noTitleTemplate"] as DataTemplate;
            /*
            UIElementCollection uilist = ((this.ContentTemplate as DataTemplate).LoadContent() as Grid).Children;
            
            foreach (UIElement uie in uilist)
            {
                if (uie is ListView)
                {
                    _listView = uie as ListView;
                    break;
                }
            }
            */
            _listView = MyListBox;
            MyListBox.MouseUp += new MouseButtonEventHandler(MyListBox_MouseUp);
            MyListBox.PreviewKeyUp += new KeyEventHandler(MyListBox_PreviewKeyUp);
            //this.focus += new RoutedEventHandler(List_LostFocus);
            //_listView.ItemsSource = Rows;
        }

        void List_LostFocus(object sender, RoutedEventArgs e)
        {
            RtwListRow.DeActivate();
        }

        void MyListBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if(System.Windows.Input.Keyboard.FocusedElement ==null) return;
            if (e.Key == Key.ImeProcessed) return;
            if (ClickedRow >=0)
            {
                //if (e.Key == Key.ImeProcessed) return;
                
                 if (e.Key == Key.Tab) //next col then next row
                {
                    int nextEdit = ClickedCol;
                    Rows[ClickedRow].Unfocus();
                    if (_editables.Count <= ClickedRow + 1)
                    { //go next row
                        if (ClickedRow + 1 < Rows.Count)
                        {
                            ClickedRow++;
                            Rows[ClickedRow].setModifyMode(ClickedRow);
                        }
                        else
                        {
                            ClickedRow = -1;
                        }
                    }
                    else
                    {
                        for (int i = ClickedCol + 1; i < _editables.Count; i++)
                        {
                            if (_editables[i])
                            {
                                Rows[ClickedRow].setModifyMode(i);
                                break;
                            }
                        }
                    }
                }
                 else if (e.Key == Key.Return) //next row
                 {
                     int row = ClickedRow;
                     if (RtwListRow.ActivatedRow != null)
                     {
                         Rows[ClickedRow].Unfocus();
                         ClickedRow++;
                     }
                     if (row < Rows.Count-1)
                     {
                         Rows[ClickedRow].setModifyMode(ClickedCol);
                     }
                     else
                     {
                         ClickedRow = -1;
                     }
                 }
                 else if (e.Key == Key.Escape)
                 {
                     if (e.KeyStates == KeyStates.Toggled)
                     {
                         Rows[ClickedRow].CancelModify();
                     }
                 }
                 else //다른 키일때
                 {
                     if (RtwListRow.ActivatedRow == null)
                     {
                         if (ClickedCol >= 0)
                         {
                             if (_editables[ClickedCol])
                             {
                                 Rows[ClickedRow].setModifyMode(ClickedCol);
                                 if ((int)e.Key >= (int)Key.A && (int)e.Key <= (int)Key.Z)
                                 {
                                     Rows[ClickedRow].setModifyingText(e.Key.ToString());
                                 }
                                 else if ((int)e.Key >= (int)Key.D0 && (int)e.Key <= (int)Key.D9)
                                 {
                                     Rows[ClickedRow].setModifyingText(((int)e.Key - (int)Key.D0).ToString());
                                 }
                             }
                         }
                     }
                 }
            }
            //e.Handled = true;
        }

        void MyListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _clickCounter.CountClick(MyListBox, e);
            
        }

        void OnCellWidthChanged(object sender, ListRowCellWidthChangedArgs e)
        {
            if(e.RowIndex>=0) Header.setCellWidth(e.CellIndex, e.AfterWidth);
            for (int i = 0; i < Rows.Count; i++)
            {
                if (i != e.RowIndex) Rows[i].setCellWidth(e.CellIndex, e.AfterWidth);
            }
        }

        void _clickCounter_OnCountedClick(CountedClickEventArgs e)
        {
            if (e.Sender.Equals(MyListBox)) //이외
            {
                RtwListRow.DeActivate();// if (BeforeClickedRow > 0) Rows[BeforeClickedRow].Unfocus();
            }
            else //rows
            {
                MouseBtnClicked(this, e);
            }
            
            //if (e.Count == 1) if (E_ListRowClicked != null) E_ListRowClicked(this, new ListRowClickEventArgs());
        }

        void ListTable_E_ListRowClicked(object sender, ListRowClickEventArgs e)
        {

            //TestData();
        }

        Boolean _isPropertyChecking = false;
        protected void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                if (_isPropertyChecking) return;
                _isPropertyChecking = true;//무한반복을 막기 위함.
                PropertyChanged(this, new PropertyChangedEventArgs(info));
                _isPropertyChecking = false;
            }
        }




        public void HideTitleBar()
        {
            TitleBar.Height = new GridLength(0);
            /*
            this.ContentTemplate = this.Resources["noTitleTemplate"] as DataTemplate;
            UIElementCollection uilist = ((this.ContentTemplate as DataTemplate).LoadContent() as Grid).Children;

            foreach (UIElement uie in uilist)
            {
                if (uie is ListView)
                {
                    _listView = uie as ListView;
                    break;
                }
            }
            _listView.ItemsSource = Rows;
             */
        }

        public void ShowTitleBar()
        {
            TitleBar.Height = new GridLength(25);
            /*
            this.ContentTemplate = this.Resources["dTemplate"] as DataTemplate;
            UIElementCollection uilist = ((this.ContentTemplate as DataTemplate).LoadContent() as Grid).Children;

            foreach (UIElement uie in uilist)
            {
                if (uie is ListView)
                {
                    _listView = uie as ListView;
                    break;
                }
            }
            _listView.ItemsSource = Rows;
             */
        }

        public void ClearTitles()
        {
            Header.Clear();
            _titles.Clear();
            _titleDic.Clear();
            _wid.Clear();
            
        }

        public void ClearData()
        {
            //_listView.ItemsSource = Rows;
            
            _closeButtons.Clear();
            CheckedRows.Clear();
            ReleaseSelection();

            Rows = new List<RtwListRow>();
            //_listView.ItemsSource = Rows;
        }

        public void AddTitleColumn(Object key, Object obj, double width, Boolean editable = true)
        {
            _titleDic.Add(key, obj);
            _titles.Add(obj);
            AddToTitle(obj, width,editable);
            
        }

        public void AddTitleColumn(Object obj, double width, Boolean editable=true)
        {
            _titleDic.Add(obj, obj);
            _titles.Add(obj);
            
            AddToTitle(obj, width,editable);
        }

        

        void AddToTitle(Object obj, double wid, Boolean editable)
        {
            UIElement item = WpfFinder.getUiElement(obj);
            if (item is CheckBox)
            {
                CheckBox box = getStyledCheckBox(((item as CheckBox).IsChecked));
                item = box; 
                box.Checked += new RoutedEventHandler(Header_Checked);
                box.Unchecked += new RoutedEventHandler(Header_Checked);
            }

            Header.addAnItem(true, item, wid);
            Header.ColName = _titleDic.Keys.ToList();
            
            _wid.Add(wid);
            _editables.Add(editable);
            NotifyPropertyChanged("Header");
        }

        void Header_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox box = sender as CheckBox;
            int index = Header.IndexOf(box);
            if (index < 0) return;
            
                foreach (RtwListRow row in Rows)
                {
                    if (row[index] is CheckBox) (row[index] as CheckBox).IsChecked = box.IsChecked; //같은 열에 체크박스가 있으면 그 체크박스를 체크한다.
                    else //아니면 다른열들을 찾아서체크한다.
                    {
                        if (CheckIfOtherCheckBoxExist)
                        {
                            for (int i = 0; i < row.Count; i++)
                            {
                                if (row[i] is CheckBox)
                                {
                                    (row[i] as CheckBox).IsChecked = box.IsChecked;
                                    if (CheckAllCheckBox == false) break;
                                }
                            }
                        }
                    }
                }
        }

        public void AddDataRowNow(Boolean refreshNow, String RelativeObjName, Object relativeObject, object[] values)
        {
            
            AddDataRow(-1, RelativeObjName, relativeObject, values);
            if (refreshNow) RefreshList(false);
            
        }

        public void AddDataRowNow(Boolean refreshNow, double height, params object[] values)
        {
            AddDataRow(height, null, null, values);
            if (refreshNow) RefreshList(false);
        }

        List<RtwCloseButton> _closeButtons = new List<RtwCloseButton>();
        public void AddDataRow(double height, String relativeObjName, object relativeObj, params object[] values)
        {
            if (values == null || values.Length == 0) return;
            if (values != null && values[0] is object[]) values = values[0] as object[];
            if (values != null && values[0] is List<object>) values = (values[0] as List<object>).ToArray();
            if (height < 0) height = RowHeight;

            // try

            List<UIElement> elList = new List<UIElement>();
            UIElement element = null;

            //try
            RtwCloseButton closeBtn = new RtwCloseButton(Rows.Count, -1, null);
            
            _closeButtons.Add(closeBtn); //한 줄에 closeButton은 default로 생성되고 관리된다.
            closeBtn.Events.E_Click += new MouseButtonEventHandler(CloseBtnClicked);

            for (int i = 0; i < values.Length; i++)
            {
                #region object -> UIElement...
                if (values[i] is RtwListChildControl)
                {
                    
                    if ((RtwListChildControl)values[i] == RtwListChildControl.CloseButton)
                    {
                        element = closeBtn;
                        closeBtn.ColIndex = i;
                    }
                    else
                    {
                        element = (values[i] is UIElement) ? values[i] as UIElement : WpfFinder.getUiElement(values[i]);
                    }
                    
                }
                else
                {
                    element = (values[i] is UIElement) ? values[i] as UIElement : WpfFinder.getUiElement(values[i]);
                }
                #endregion
                
                if (element is CheckBox)
                {
                    element = getStyledCheckBox(((element as CheckBox).IsChecked));
                }
                else if (element is TextBlock)
                {
                    (element as TextBlock).ToolTip = (element as TextBlock).Text;
                    element.SetValue(FontFamilyProperty, this.FontFamily);
                    element.SetValue(FontSizeProperty, this.FontSize);
                    element.SetValue(ForegroundProperty, this.Foreground);

                }
                else if (element is Image)
                {

                }

                if ((element is FrameworkElement))
                {
                    if(_wid.Count>i) (element as FrameworkElement).Width = _wid[i];
                    else (element as FrameworkElement).Width = height;
                    (element as FrameworkElement).Height = height;
                }

                elList.Add(element);
                /*
                if (element is CheckBox)
                {
                    (element as CheckBox).Checked += new RoutedEventHandler(OnCheckBoxClicked);
                    (element as CheckBox).Unchecked += new RoutedEventHandler(OnCheckBoxClicked);
                }
                */

            }
            RtwListRow row = new RtwListRow(_wid, height, true, elList);
            row.ColName = Header.ColName;
            if (relativeObjName != null)
            {
                row.RelativeObject[relativeObjName] = relativeObj;
            }
            row.ItemHorizontalAlignment = DataHorizonAlignment;
            row.Index = Rows.Count;
            row.E_TextChanged += new ListRowTextChangedEventHandler(row_E_TextChanged);
            row.E_CheckBoxChanged += new ListCheckedEventHandler(OnCheckBoxClicked);
            row.E_ComboBoxChanged += new ListComboBoxEventHandler(row_E_ComboBoxChanged);
            row.E_CellWidthChanged += new ListRowCellWidthChangedHandler(OnCellWidthChanged);
            row.E_CellClicked += new ListRowCellClickedHandler(row_E_CellClicked);
            row.E_CellMouseDown += new ListRowCellClickedHandler(row_E_CellMouseDown);
            row.Editables = _editables;

            _rows.Add(row);
            
        }

        void row_E_CellMouseDown(object sender, ListRowCellClickedArgs e)
        {
            ClickedCol = e.ColIndex;
            ClickedRow = e.RowIndex;
        }

        void CloseBtnClicked(object sender, MouseButtonEventArgs e)
        {
            isMouseClickHandled = true;
            RtwCloseButton btn = sender as RtwCloseButton;
            RtwListRow deletedRow = Rows[btn.RowIndex];
            ListRowClickEventArgs args = new ListRowClickEventArgs(btn.RowIndex, btn.ColIndex, deletedRow, sender as UIElement, MyListBox, sender as UIElement, MyListBox);
            if (E_ListRowRemoving != null) E_ListRowRemoving(deletedRow, args);

            if (args.Returns != null)
            {
                if (args.Returns is bool && (bool)(args.Returns)==false)
                {
                    e.Handled = true;
                    return;
                }
            }
            RemoveARow(btn.RowIndex);
            
            
            if (E_ListRowRemoved != null) E_ListRowRemoved(deletedRow, new ListRowClickEventArgs(btn.RowIndex, btn.ColIndex, deletedRow, sender as UIElement, MyListBox, sender as UIElement, MyListBox));

            RefreshList();

            e.Handled = true;
        }

        public void RemoveARow(int rowIndex, bool refreshNow=true)
        {
            try
            {
                _rows.RemoveAt(rowIndex);
            }
            catch { }
            if (_closeButtons.Count > 0) _closeButtons.RemoveAt(rowIndex);

            if (refreshNow)
            {
                RefreshList();
            }
            
        }

        //List<RtwListRow> _tempNewList = new List<RtwListRow>();
        /*
        double ad_height;
        object ad_relativeObj;
        object[] ad_values;
        String ad_relativeObjName;
        
        void AddDataRowInvoke()
        {

            //if (lv == null)
            {
              //  return;
            }
            //else
            {
                double height = ad_height;
                object relativeObj = ad_relativeObj;
                object[] values = ad_values;
                List<UIElement> elList = new List<UIElement>();
                UIElement element;

                _tempNewList = this.Rows;
                //try
                {
                    for (int i = 0; i < values.Length; i++)
                    {
                        element = (values[i] is UIElement) ? values[i] as UIElement : WpfFinder.getUiElement(values[i]);
                        if (element is CheckBox)
                        {
                            element = getStyledCheckBox(((element as CheckBox).IsChecked));
                        }
                        else if (element is TextBlock)
                        {
                            (element as TextBlock).ToolTip = (element as TextBlock).Text;
                            element.SetValue(FontFamilyProperty, this.FontFamily);
                            element.SetValue(FontSizeProperty, this.FontSize);
                            element.SetValue(ForegroundProperty, this.Foreground);
                            
                        }
                        else if (element is Image)
                        {

                        }
                        
                        
                        
                        elList.Add(element);
                        

                    }
                    RtwListRow row = new RtwListRow(_wid, RowHeight, true, elList);
                    row.ColName = Header.ColName;
                    if (ad_relativeObjName != null)
                    {
                        row.RelativeObject[ad_relativeObjName] = relativeObj;
                    }
                    row.ItemHorizontalAlignment = DataHorizonAlignment;
                    row.Index = Rows.Count;
                    row.E_TextChanged += new ListRowTextChangedEventHandler(row_E_TextChanged);
                    row.E_CheckBoxChanged += new ListCheckedEventHandler(OnCheckBoxClicked);
                    row.E_ComboBoxChanged += new ListComboBoxEventHandler(row_E_ComboBoxChanged);
                    row.E_CellWidthChanged += new ListRowCellWidthChangedHandler(OnCellWidthChanged);
                    row.E_CellClicked += new ListRowCellClickedHandler(row_E_CellClicked);
                    row.Editables = _editables;
                    _tempNewList.Add(row);
                    
                   
                    
                   // if(_listView!=null) 
                       // _listView.Items.Add(row);
                }
                //catch (Exception e)
                {
                    //throw e;
                }
                //NotifyPropertyChanged("Rows");
            }
            
        }
        */
        object _cellClickedSender;
        ListRowCellClickedArgs _cellClickedArgs;
        void row_E_CellClicked(object sender, ListRowCellClickedArgs e)
        {
            _cellClickedSender = sender;
            _cellClickedArgs = e;
            _clickCounter.CountClick(sender, e);
        }

        public void RefreshList(bool releaseSelection = false)
        {
            if(releaseSelection) ReleaseSelection();
            //List<RtwListRow> newList = new List<RtwListRow>(Rows);
            Rows = Rows;
            resetIndex();
        }
        
        void row_E_ComboBoxChanged(object sender, ListComboBoxEventArgs e)
        {
            if (E_ComboBoxChanged != null)
            {
                e.RowIndex = (Rows.IndexOf(sender as RtwListRow));
                E_ComboBoxChanged(this, e);
            }
        }




       
        public List<RtwListRow> SelectedItems
        {
            get
            {
                _selectedItems.Clear();
                foreach (RtwListRow row in Rows)
                {
                    if (row.Selected) _selectedItems.Add(row);
                }
                return _selectedItems;
            }
        }
        public void ClearBackSelection()
        {
            foreach (RtwListRow row in Rows)
            {
                row.BackSelected = false;
            }
        }
        public void ReleaseSelection()
        {
            
            /*
            UIElement aChild = null;
            if (Rows.Count > 0 && Rows[0].Count > 0) aChild = Rows[0][0] as UIElement;
            if (aChild == null) return;
            ListBoxItem item = WpfFinder.getParentFromTemplate(aChild, DependencyObjectType.FromSystemType(typeof(ListBoxItem))) as ListBoxItem;
            if (item == null) return;
            item.IsSelected = false;

               
            ListBox view = LogicalTreeHelper.GetParent(item) as ListBox;
            DependencyObject obj = VisualTreeHelper.GetParent(item);
            while (obj != null)
            {
                if (obj is ListBox || obj is ListView)
                {
                    view = obj as ListView;
                    break;
                }
                obj = VisualTreeHelper.GetParent(obj);
            }
            if (view == null) return;
                */
            //DataTemplate template = this.ContentTemplateSelector.SelectTemplate(this, this);
            ListBox view = MyListBox;

            view.SelectionMode = System.Windows.Controls.SelectionMode.Single;
            view.SelectionMode = System.Windows.Controls.SelectionMode.Extended;

            view.SelectedItem = null;
            view.SelectedItems.Clear();
            view.SelectedIndex = -1;
            view.SelectedValue = null;


            IsSelected = false;
            
        }
        
        Boolean _isSelected = false;
        public Boolean IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                if (value == false)
                {
                    foreach (RtwListRow row in _rows)
                    {
                        row.Selected = false;
                    }
                    _selectedItems.Clear();
                }
                NotifyPropertyChanged("IsSelected");
            }
        }
        /*
        int _startDragRowIndex = -1;
        ListRow _beforeLeave = null;

        void row_MouseEnter(object sender, RoutedEventArgs e)
        {
            
            if (System.Windows.Input.Mouse.LeftButton == MouseButtonState.Pressed)
            {
                ListRow row = sender as ListRow;
                if (SelectedItems.Contains(row) == false) SelectedItems.Add(row);
                if (_beforeLeave != null)
                {
                    if (SelectedItems.Contains(row))
                    {
                        SelectedItems.Add(row);
                        (row.TemplatedParent as ListViewItem).IsSelected = true;
                    }
                    else
                    {
                        SelectedItems.Remove(_beforeLeave);
                        (_beforeLeave.TemplatedParent as ListViewItem).IsSelected = false;
                    }
                }
                _beforeLeave = row;
            }
        }

        void row_MouseLeave(object sender, RoutedEventArgs e)
        {
            if (System.Windows.Input.Mouse.LeftButton == MouseButtonState.Pressed)
            {
                ListRow row = sender as ListRow;

                if (SelectedItems.Contains(row) == false)
                {
                    SelectedItems.Add(row);
                    (row.TemplatedParent as ListViewItem).IsSelected = true;
                }

                _startDragRowIndex = row.Index;
            }
        }

        */


        void row_E_TextChanged(object sender, ListRowTextChangedEventArgs e)
        {
            e.RowIndex = (Rows.IndexOf(sender as RtwListRow));
            if (E_TextChanged != null) E_TextChanged(this, e);
        }



        protected void OnCheckBoxClicked(object sender, ListCheckedEventArgs e)
        {
            if (_checkBoxActivated == false) return;
            _checkBoxActivated = false;
            //try
            {
                CheckBox cb = e.OriginalSource as CheckBox;
                /*
                ListBoxItem item = WpfFinder.getParentFromTemplate(cb, DependencyObjectType.FromSystemType(typeof(ListBoxItem))) as ListBoxItem;
                //ListBox view = WpfFinder.getParentFromTemplate(item, DependencyObjectType.FromSystemType(typeof(ListBox))) as ListBox;
                if (item == null) return;
                ListRow row = item.Content as ListRow;
                */
                e.RowIndex = (Rows.IndexOf(sender as RtwListRow));

                int row_index = e.RowIndex;
                int col_index = e.ColumnIndex;


                if ((System.Windows.Input.Keyboard.GetKeyStates(Key.LeftShift) & KeyStates.Down) == KeyStates.Down)
                {
                    if (BeforeClickedRow >= 0)
                    {
                        int min = (BeforeClickedRow < row_index) ? BeforeClickedRow : row_index;
                        int max = (BeforeClickedRow < row_index) ? row_index : BeforeClickedRow;

                        for (int i = min; i <= max; i++)
                        {
                            rowCheckboxClicked(Rows[i], true);
                            try
                            {
                                if (E_CheckBoxChanged != null) E_CheckBoxChanged(sender, new ListCheckedEventArgs((Boolean)cb.IsChecked, row_index, col_index));
                            }
                            catch {
                                _checkBoxActivated = true;
                                throw;
                            }
                        }


                    }
                }
                else if ((System.Windows.Input.Keyboard.GetKeyStates(Key.LeftCtrl) & KeyStates.Down) == KeyStates.Down)
                {
                    if (BeforeClickedRow >= 0)
                    {
                        int min = (BeforeClickedRow < row_index) ? BeforeClickedRow : row_index;
                        int max = (BeforeClickedRow < row_index) ? row_index : BeforeClickedRow;

                        for (int i = min; i <= max; i++)
                        {
                            rowCheckboxClicked(Rows[i], false);
                            try
                            {
                                if (E_CheckBoxChanged != null) E_CheckBoxChanged(sender, new ListCheckedEventArgs((Boolean)cb.IsChecked, row_index, col_index));
                            }
                            catch {
                                _checkBoxActivated = true;
                                throw;
                            }
                        }
                    }
                }
                else
                {
                    try
                    {
                        if (E_CheckBoxChanged != null) E_CheckBoxChanged(sender, new ListCheckedEventArgs((Boolean)cb.IsChecked, row_index, col_index));
                    }
                    catch {
                        _checkBoxActivated = true;
                        throw;
                    }
                    ClickedRow = row_index;

                    rowCheckboxClicked(Rows[row_index], (Boolean)cb.IsChecked);
                }
                _checkBoxActivated = true;
                isMouseClickHandled = true; //체크박스 체크 후에 다른 반응하지 않도록..
            }
            //catch
            {
                _checkBoxActivated = true;
                isMouseClickHandled = true; //체크박스 체크 후에 다른 반응하지 않도록..
            //    throw;
            }
        }

        int ClickedCol = -1;
        int ClickedRow = -1;
        int BeforeClickedRow = -1;
        int BeforeClickedCol = -1;


        public bool isMouseClickHandled = false;
        protected void MouseBtnClicked(object sender, CountedClickEventArgs e)
        {
            if (isMouseClickHandled) //이전에 마우스 이벤트를 처리하고 더이상 처리하고 싶지않다면..
            {
                isMouseClickHandled = false;
                return;
            }
            //CountedClickEventArgs e = _clickCounter.Args;// BtnArgs;
            /*
            DependencyObject obj = e.MouseBtnArgs.OriginalSource as DependencyObject;
            
            if (obj.DependencyObjectType.IsSubclassOf(DependencyObjectType.FromSystemType(typeof(UIElement))))
            {
                //    if (obj.DependencyObjectType.IsSubclassOf(DependencyObjectType.FromSystemType(typeof(Shape)))) { return; }
            }
            else return;


            MouseButtonEventArgs mbe = e.MouseBtnArgs as MouseButtonEventArgs;
            ListBox view = (e.Sender as ListBox);
            RtwListRow row = view.SelectedItem as RtwListRow;
            if (row == null) return;
             */

            RtwListRow row = Rows[_cellClickedArgs.RowIndex];

            
            ListRowClickEventArgs arg = new ListRowClickEventArgs(
                _cellClickedArgs.RowIndex,
                _cellClickedArgs.ColIndex,
                row,
                sender as UIElement ,MyListBox, sender as UIElement, MyListBox
                );
            
            try
            {
                arg.ListObj = MyListBox;
                arg.ListRowItem = row;
                arg.RowIndex = _cellClickedArgs.RowIndex;
                arg.ColIndex = _cellClickedArgs.ColIndex;
                if (arg.RowIndex < 0 || arg.ColIndex < 0)
                {
                    if (RtwListRow.ActivatedRow != null) RtwListRow.DeActivate();
                    return;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            if (row == null) return;
            
            //try
            {
                if (e.Count == 1)
                {
                    if (e.PressedButton == MouseButton.Left)
                    {
                        if (ActionOnClicked == Actions.CheckBoxChecked)
                        {

                            rowCheckboxClicked(row);
                        }
                        else if (ActionOnClicked == Actions.Modify)
                        {
                            if (_editables[arg.ColIndex])
                            {
                                row.setModifyMode(arg.ColIndex);
                            }
                        }
                        else if (ActionOnClicked == Actions.ContextMenu)
                        {
                            U_ContextMenu.HorizontalOffset = 10;
                            U_ContextMenu.VerticalOffset = 10;

                            U_ContextMenu.IsOpen = true;
                        }
                    }
                    else if (e.PressedButton == MouseButton.Right)
                    {
                        if (ActionOnRightClicked == Actions.CheckBoxChecked)
                        {
                            rowCheckboxClicked(row);
                        }
                        else if (ActionOnRightClicked == Actions.Modify)
                        {
                            if (_editables[arg.ColIndex])
                            {
                                row.setModifyMode(arg.ColIndex);
                            }
                        }
                        else if (ActionOnRightClicked == Actions.ContextMenu)
                        {
                            U_ContextMenu.HorizontalOffset = 10;
                            U_ContextMenu.VerticalOffset = 10;

                            U_ContextMenu.IsOpen = true;
                            
                        }
                    }
                    if (E_ListRowClicked != null) E_ListRowClicked(e.Sender, arg);

                }
                else if (e.Count == 2)
                {

                    if (ActionOnDoubleClicked == Actions.CheckBoxChecked)
                    {
                        rowCheckboxClicked(row);
                    }
                    else if (ActionOnDoubleClicked == Actions.Modify)
                    {
                        if (_editables[arg.ColIndex])
                        {
                            row.setModifyMode(arg.ColIndex);
                        }
                    }

                    if (E_ListRowDoubleClicked != null) E_ListRowDoubleClicked(e.Sender, arg);

                }
            }
            //catch (Exception ex)
            {
            //    throw ex;
            }
        }
        void rowCheckboxClicked(RtwListRow aRow, bool? check=null)
        {
            
            for (int i = 0; i < aRow.Count; i++)
            {
                UIElement col = aRow[i];
                if (col is CheckBox)
                {
                    Boolean isChecked = (Boolean)((col as CheckBox).IsChecked);
                    (col as CheckBox).IsChecked =  (check!=null)? check : !isChecked;
                    if (check == true)
                    {
                        if(CheckedRows.Contains(aRow)==false) CheckedRows.Add(aRow);
                        
                    }
                    else if (check == false)
                    {
                        CheckedRows.Remove(aRow);
                    }
                }
            }
            
            
        }

        public List<Object> FindInCol(int index)
        {
            List<Object> aCol = new List<object>();
            foreach (RtwListRow row in Rows)
            {
                aCol.Add(row.getValues()[index]);
            }
            return aCol;
        }
        public List<Object> FindInCol(String name)
        {
            int index = _titles.IndexOf(name);
            return FindInCol(index);
        }


        public CheckBox getStyledCheckBox(bool? isChecked = false)
        {
            CheckBox cb = new CheckBox();
            cb.Style = this.Resources["CheckBoxTemplate"] as Style;
            cb.Padding = new Thickness(0, 0, 0, 0);
            cb.IsChecked = isChecked;
            return cb;
        }

        public ComboBox getStyledComboBox(params object[] items)
        {
            ComboBox cb = new ComboBox();
            for (int i = 0; i < items.Length; i++)
            {
                cb.Items.Add(items[i]);
            }
            return cb;
        }

        public ComboBox getStyledComboBox(List<String> items)
        {
            ComboBox cb = new ComboBox();
            for (int i = 0; i < items.Count; i++)
            {
                cb.Items.Add(items[i]);
            }
            return cb;
        }

        public ComboBox getStyledComboBox(List<Object> items)
        {
            ComboBox cb = new ComboBox();
            for (int i = 0; i < items.Count; i++)
            {
                cb.Items.Add(items[i]);
            }
            return cb;
        }

        SelectionMode _selMode = SelectionMode.Extended;
        public SelectionMode SelectionMode
        {
            get
            {
                return _selMode;
            }
            set
            {
                _selMode = value;
                NotifyPropertyChanged("SelectionMode");
            }
        }
        /*
        private void List_Check_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox c = sender as CheckBox;
            Object obj = c.TemplatedParent;
            //L_Title.Content = c.Name;
            //MessageBox.Show(c.Name);
        }
        private void List_Check_UnChecked(object sender, RoutedEventArgs e)
        {
            CheckBox c = sender as CheckBox;
            //L_Title.Content = c.Name;
            //MessageBox.Show(c.Name);
        }

        private void cb1_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer sv = e.OriginalSource as ScrollViewer;

            //sv.ScrollToVerticalOffset(e.VerticalOffset - 3);
        }

        private void ScrollViewer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer sv = e.OriginalSource as ScrollViewer;
            sv.ScrollToVerticalOffset(e.Delta);
            
        }

        private void ListBox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            ListBox lb = e.OriginalSource as ListBox;
            ScrollViewer sv = lb.Parent as ScrollViewer;
            sv.ScrollToVerticalOffset(e.Delta);
        }

        */


        protected void MouseWheelEventOccured(object sender, RoutedEventArgs e)
        {

            /*
            DataTemplate dt = this.Resources["dTemplate"] as DataTemplate;//.ContentTemplate as DataTemplate;

            DependencyObject current = e.OriginalSource as DependencyObject;
            DependencyObject result = e.OriginalSource as DependencyObject;

            while (current != null)
            {
                result = current;

                if (current is Visual)
                {
                    current = VisualTreeHelper.GetParent(current);
                }
                else
                {
                    // If we're in Logical Land then we must walk 
                    // up the logical tree until we find a 
                    // Visual/Visual3D to get us back to Visual Land.
                    current = LogicalTreeHelper.GetParent(current);
                }

                ListView lv=null;
                if (current is ListView) lv = current as ListView;
                if (current is ScrollViewer)
                {
                    ScrollViewer sv = current as ScrollViewer;
                    if (e is MouseWheelEventArgs)
                    {
                        MouseWheelEventArgs we = e as MouseWheelEventArgs;
                        double offset = sv.VerticalOffset - we.Delta;
                        sv.ScrollToEnd();
                        sv.ScrollToVerticalOffset(offset);
                        if (lv != null) lv.LayoutTransform.Transform(new Point(0, offset));
                    }
                    break;
                }
            }*/
        }

        protected void MyListBox_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            ScrollBar scrollbar = e.OriginalSource as ScrollBar;
            ListBox view = e.Source as ListBox;
            if (scrollbar.Orientation == Orientation.Horizontal)
            {
                double value = scrollbar.Value;
                double max = scrollbar.Maximum;
                double rate = value / max;
                double size = Header.TotalWidth - view.ActualWidth;
                double move = size*rate;
                Header.Margin = new Thickness(-move,0,0,0);
            }
            if (e.ScrollEventType == System.Windows.Controls.Primitives.ScrollEventType.EndScroll)
            {

            }
        }

        public void CheckBoxActivate(bool active)
        {
            _checkBoxActivated = active;
        }

        private void MyListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IList<object> removeList = e.RemovedItems as IList<object>;
            IList<object> addList = e.AddedItems as IList<object>;
            object item = (removeList.Count > 0) ? removeList[0] : (addList.Count > 0) ? addList[0] : null;
            if (item!=null)
            {
                if (item is RtwListRow)
                {
                    //SelectedItems.Clear();
                    foreach (RtwListRow aRow in removeList)
                    {
                        _selectedItems.Remove(aRow);
                    }

                    foreach (RtwListRow aRow in addList)
                    {
                        _selectedItems.Add(aRow);
                    }
                }
                else if(item is ListBoxItem)
                {
                    foreach (ListBoxItem aRow in removeList)
                    {
                        _selectedItems.Remove(aRow.Content as RtwListRow);
                    }

                    foreach (ListBoxItem aRow in addList)
                    {
                        _selectedItems.Add(aRow.Content as RtwListRow);
                    }
                }
            }

        }
    }

}
