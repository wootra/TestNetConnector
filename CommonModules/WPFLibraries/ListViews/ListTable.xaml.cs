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
namespace ListViews
{
    public partial class ListTable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected List<double> _wid = new List<double>();
        protected List<Boolean> _editables = new List<bool>();
        protected ClickCounter _clickCounter;
        protected Rectangle _sep = new Rectangle();
        protected ResourceHandler _rh;
        protected double _height = 20;
        protected HorizontalAlignment _dataHorizonAlign = HorizontalAlignment.Left;
        protected bool _checkBoxActivated = true;
        protected List<ListRow> _selectedItems = new List<ListRow>();

        public List<Object> _titles = new List<Object>();
        public Dictionary<Object, Object> _titleDic = new Dictionary<object, object>();
        
        
        public event ListRowClickEventHandler E_ListRowClicked;
        public event ListRowClickEventHandler E_ListRowDoubleClicked;
        public event ListCheckedEventHandler E_CheckBoxChanged;
        public event ListRowTextChangedEventHandler E_TextChanged;
        public event ListComboBoxEventHandler E_ComboBoxChanged;
        
        public enum Actions { Modify = 0, CheckBoxChecked, Nothing };

        public Actions ActionOnDoubleClicked { get; set; }
        public Actions ActionOnClicked { get; set; }
        public List<double> Wid { get { return _wid; } set { _wid = value; } }
        public List<int> CheckedIndices = new List<int>();
        public List<ListRow> CheckedRows = new List<ListRow>();
        List<ListRow> _rows = new List<ListRow>();
        ListView _listView;

        public bool CheckIfOtherCheckBoxExist = true;
        public bool CheckAllCheckBox = true;
        
        public List<ListRow> Rows {
            get{ return _rows;}
            set { 
                _rows = value;
                NotifyPropertyChanged("Rows");
            }
        }

        public ListRow Header { get; set; }
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

        public ListTable()
            : base()
        {
            InitializeComponent();

            Rows = new List<ListRow>();
            Header = new ListRow();
            //DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Normal, Dispatcher);
            //timer.Interval = new TimeSpan(2000);
            _clickCounter = new ClickCounter(200, this);
            _clickCounter.OnCountedClick += new CountedClickEventHandler(_clickCounter_OnCountedClick);
            ActionOnDoubleClicked = Actions.CheckBoxChecked;
            ActionOnClicked = Actions.Modify;
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

            //E_ListRowClicked += new ListRowClickEventHandler(ListTable_E_ListRowClicked);
            //TestData();
            //this.ContentTemplate = this.Resources["dTemplate"] as DataTemplate;
            //this.ContentTemplate = this.Resources["noTitleTemplate"] as DataTemplate;
            UIElementCollection uilist = ((this.ContentTemplate as DataTemplate).LoadContent() as Grid).Children;
            
            foreach (UIElement uie in uilist)
            {
                if (uie is ListView)
                {
                    _listView = uie as ListView;
                    break;
                }
            }
            
            //_listView.ItemsSource = Rows;
        }

        void _clickCounter_OnCountedClick(CountedClickEventArgs e)
        {
            MouseBtnClicked(this, e);
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
        }

        public void ShowTitleBar()
        {
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
            _tempNewList.Clear();
            ReleaseSelection();
            Rows = new List<ListRow>();
            _listView.ItemsSource = Rows;
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
            
                foreach (ListRow row in Rows)
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

        public void AddDataRow(Boolean refreshNow, Object relativeObject, object[] values)
        {
            
            AddDataRow(refreshNow, -1, true, relativeObject, values);
            
        }

        public void AddDataRow(Boolean refreshNow, double height = -1, params object[] values)
        {
            AddDataRow(refreshNow, height, false, null, values);
        }
        
        delegate void DelFunc();
        public void AddDataRow(Boolean refreshNow, double height, Boolean relativeObjInclude, object relativeObj, params object[] values)
        {
            if (values == null || values.Length == 0) return;
            if (values != null && values[0] is object[]) values = values[0] as object[];
            if (values != null && values[0] is List<object>) values = (values[0] as List<object>).ToArray();

            ad_height = height;
            ad_relativeObj = relativeObj;
            ad_values = values;

            DelFunc func = AddDataRowInvoke;
           // try
            {
                VerifyAccess();
                //if(refreshNow) _tempNewList.Clear();
                AddDataRowInvoke();
                if (refreshNow) RefreshList();
            }
           // catch 
            {
           //     this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(func));
            }
            
            
        }

        double ad_height;
        object ad_relativeObj;
        object[] ad_values;
        List<ListRow> _tempNewList = new List<ListRow>();
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
                        /*
                        if (element is CheckBox)
                        {
                            (element as CheckBox).Checked += new RoutedEventHandler(OnCheckBoxClicked);
                            (element as CheckBox).Unchecked += new RoutedEventHandler(OnCheckBoxClicked);
                        }
                        */

                    }
                    ListRow row = new ListRow(_wid, RowHeight, true, elList);
                    row.ColName = Header.ColName;
                    row.RelativeObject = relativeObj;
                    row.ItemHorizontalAlignment = DataHorizonAlignment;
                    row.Index = Rows.Count;
                    row.E_TextChanged += new ListRowTextChangedEventHandler(row_E_TextChanged);
                    row.E_CheckBoxChanged += new ListCheckedEventHandler(OnCheckBoxClicked);
                    row.E_ComboBoxChanged += new ListComboBoxEventHandler(row_E_ComboBoxChanged);
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
        public void RefreshList()
        {
            Rows = new List<ListRow>( _tempNewList);
        }
        

        void row_E_ComboBoxChanged(object sender, ListComboBoxEventArgs e)
        {
            if (E_ComboBoxChanged != null)
            {
                e.RowIndex = (Rows.IndexOf(sender as ListRow));
                E_ComboBoxChanged(this, e);
            }
        }




       
        public List<ListRow> SelectedItems
        {
            get
            {
                _selectedItems.Clear();
                foreach (ListRow row in Rows)
                {
                    if (row.Selected) _selectedItems.Add(row);
                }
                return _selectedItems;
            }
        }

        public void ReleaseSelection()
        {
            //try
            {
                foreach (ListRow row in Rows)
                {
                    row.Selected = false;
                }
                IsSelected = false;
                _selectedItems.Clear();

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
                //DataTemplate template = this.ContentTemplateSelector.SelectTemplate(this, this);
                
                view.SelectionMode = System.Windows.Controls.SelectionMode.Single;
                view.SelectionMode = System.Windows.Controls.SelectionMode.Extended;

                view.SelectedItem = null;
                view.SelectedItems.Clear();
                view.SelectedIndex = -1;
                view.SelectedValue = null;

                foreach (ListRow row in view.Items)
                {
                    row.Selected = false;
                }
                IsSelected = false;
            }
            //catch(Exception e) {
            //    throw e;
            //}
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


        void row_E_TextChanged(object sender, ListRowTextChangedEventArg e)
        {
            e.RowIndex = (Rows.IndexOf(sender as ListRow));
            if (E_TextChanged != null) E_TextChanged(this, e);
        }



        protected void OnCheckBoxClicked(object sender, ListCheckedEventArgs e)
        {
            if (_checkBoxActivated == false) return;
            _checkBoxActivated = false;
            CheckBox cb = e.OriginalSource as CheckBox;
            /*
            ListBoxItem item = WpfFinder.getParentFromTemplate(cb, DependencyObjectType.FromSystemType(typeof(ListBoxItem))) as ListBoxItem;
            //ListBox view = WpfFinder.getParentFromTemplate(item, DependencyObjectType.FromSystemType(typeof(ListBox))) as ListBox;
            if (item == null) return;
            ListRow row = item.Content as ListRow;
            */
            e.RowIndex = (Rows.IndexOf(sender as ListRow));
            
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
                        CheckedIndices.Add(i);
                        CheckedRows.Add(Rows[i]);
                        if (E_CheckBoxChanged != null) E_CheckBoxChanged(sender, new ListCheckedEventArgs((Boolean)cb.IsChecked, row_index, col_index));
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
                        if (CheckedIndices.IndexOf(i) >= 0)
                        {
                            CheckedIndices.Remove(i);
                            CheckedRows.Remove(Rows[i]);
                        }
                        if (E_CheckBoxChanged != null) E_CheckBoxChanged(sender, new ListCheckedEventArgs((Boolean)cb.IsChecked, row_index, col_index));
                    }
                }
            }
            else
            {
                if (E_CheckBoxChanged != null) E_CheckBoxChanged(sender, new ListCheckedEventArgs((Boolean)cb.IsChecked, row_index, col_index));
                BeforeClickedRow = row_index;
                if (cb.IsChecked == true && CheckedIndices.IndexOf(row_index) < 0)
                {
                    CheckedIndices.Add(row_index);
                    CheckedRows.Add(Rows[row_index]);
                }
                else if (CheckedIndices.IndexOf(row_index) >= 0)
                {
                    CheckedIndices.Remove(row_index);
                    CheckedRows.Remove(Rows[row_index]);
                }
            }
            _checkBoxActivated = true;
        }

        int BeforeClickedRow = -1;
        protected void MouseBtnClicked(object sender, CountedClickEventArgs e)
        {
            //CountedClickEventArgs e = _clickCounter.Args;// BtnArgs;
            DependencyObject obj = e.MouseBtnArgs.OriginalSource as DependencyObject;

            if (obj.DependencyObjectType.IsSubclassOf(DependencyObjectType.FromSystemType(typeof(UIElement))))
            {
                //    if (obj.DependencyObjectType.IsSubclassOf(DependencyObjectType.FromSystemType(typeof(Shape)))) { return; }
            }
            else return;


            MouseButtonEventArgs mbe = e.MouseBtnArgs as MouseButtonEventArgs;
            ListBox view = (e.Sender as ListBox);
            ListRow row = view.SelectedItem as ListRow;
            ListRowClickEventArgs arg = new ListRowClickEventArgs();
            try
            {
                arg.ListObj = view;
                arg.ListRowItem = view.SelectedItem as ListRow;
                arg.rowIndex = Rows.IndexOf(arg.ListRowItem);
                arg.colIndex = (arg.ListRowItem == null) ? -1 : (arg.ListRowItem.IndexOf(e.MouseBtnArgs.OriginalSource as UIElement));
                if (arg.rowIndex < 0 || arg.colIndex < 0)
                {
                    if (ListRow.ActivatedRow != null) ListRow.DeActivate();
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

                    if (ActionOnClicked == Actions.CheckBoxChecked)
                    {

                        rowCheckboxClicked(row);
                    }
                    else if (ActionOnClicked == Actions.Modify)
                    {
                        if(_editables[arg.colIndex]) row.setModifyMode(arg.colIndex);
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
                        if (_editables[arg.colIndex]) row.setModifyMode(arg.colIndex);
                    }

                    if (E_ListRowDoubleClicked != null) E_ListRowDoubleClicked(e.Sender, arg);

                }
            }
            //catch (Exception ex)
            {
            //    throw ex;
            }
            BeforeClickedRow = row.Index;
        }
        void rowCheckboxClicked(ListRow row, bool? check=null)
        {
            for (int i = 0; i < row.Count; i++)
            {
                if (row[i] is CheckBox)
                {
                    Boolean isChecked = (Boolean)((row[i] as CheckBox).IsChecked);
                    (row[i] as CheckBox).IsChecked =  (check!=null)? check : !isChecked;
                }
            }
        }

        public List<Object> FindInCol(int index)
        {
            List<Object> aCol = new List<object>();
            foreach (ListRow row in Rows)
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

        protected void MouseUpEventOccured(Object sender, RoutedEventArgs e)
        {
            _clickCounter.CountClick(sender, e);
            //TestData();
            //Console.Write("{0}", Rows.Count);
        }

        public void TestData()
        {
            /*
            AddTitleColumn(1, 40);
            AddTitleColumn(2, 40);
            AddTitleColumn(3, 40);
            AddTitleColumn(4, 40);
            */
            
            for (int i = 0; i < 10; i++)
            {
                List<object> uis = new List<object>();
                CheckBox cb = getStyledCheckBox();
                uis.Add(cb);
                for (int j = 0; j < 3; j++)
                {
                    uis.Add(WPF_Handler.WpfFinder.getUiElement("val" + j + "djkldjls", HorizontalAlignment.Left));
                }
                AddDataRow(false, -1, uis);

            }
            RefreshList();
            NotifyPropertyChanged("Rows");
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
            ListView view = e.Source as ListView;
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
                if (item is ListRow)
                {
                    //SelectedItems.Clear();
                    foreach (ListRow aRow in removeList)
                    {
                        _selectedItems.Remove(aRow);
                    }

                    foreach (ListRow aRow in addList)
                    {
                        _selectedItems.Add(aRow);
                    }
                }
                else if(item is ListViewItem)
                {
                    foreach (ListViewItem aRow in removeList)
                    {
                        _selectedItems.Remove(aRow.Content as ListRow);
                    }

                    foreach (ListViewItem aRow in addList)
                    {
                        _selectedItems.Add(aRow.Content as ListRow);
                    }
                }
            }

        }
    }

}
