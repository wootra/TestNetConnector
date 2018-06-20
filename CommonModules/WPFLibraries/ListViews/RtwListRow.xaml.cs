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
using System.ComponentModel;
using System.Runtime.Serialization;

namespace RtwWpfControls
{
    /// <summary>
    /// ListRow.xaml에 대한 상호 작용 논리
    /// </summary>
    [Serializable]
    public partial class RtwListRow : UserControl,INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event ListRowTextChangedEventHandler E_TextChanged;
        public event ListComboBoxEventHandler E_ComboBoxChanged;
        public event ListCheckedEventHandler E_CheckBoxChanged;
        public event ListRowCellWidthChangedHandler E_CellWidthChanged;
        public event ListRowCellClickedHandler E_CellClicked;
        public event ListRowCellClickedHandler E_CellMouseDown;

        public static RtwListRow ActivatedRow = null;
        StackPanel _panel = new StackPanel();
        Grid _mainGrid = new Grid();
        Grid _indexGrid = new Grid();
        List<Grid> _contentGrids = new List<Grid>();
        TextBlock _indexText = new TextBlock();
        List<UIElement> _items = new List<UIElement>();
        List<double> _wid = new List<double>();
        List<bool> _editables = new List<bool>();
        List<Object> _colName = new List<Object>();
        Rectangle _seperator = null;
        static Boolean _isModifying = false;
        HorizontalAlignment _itemHorizonAlign = HorizontalAlignment.Left;

        public Dictionary<String, Object> RelativeObject = new Dictionary<string, object>();

        public Grid MainGrid
        {
            get { return _mainGrid; }
            set {
                _mainGrid = value;
                NotifyPropertyChanged("MainGrid");
            }
        }
        public List<Object> ColName
        {
            get { return _colName; }
            set { _colName = value; }
        }

        bool _showIndex = true;
        public Boolean ShowIndex{
            get { return _showIndex; }
            set {
                _showIndex = value;
                _indexGrid.Visibility = (value) ? Visibility.Visible : Visibility.Collapsed; 
            }
        }

        double _indexWidth = 30;
        public double IndexWidth
        {
            get { return _indexWidth; }
            set
            {
                _indexWidth = value;
                _indexGrid.Width = value;
                NotifyPropertyChanged("IndexWidth");
            }
        }


        public Color IndexBackColor
        {
            get { return (_indexBack.Fill as SolidColorBrush).Color; }
            set { (_indexBack.Fill as SolidColorBrush).Color = value; }
        }

        public HorizontalAlignment ItemHorizontalAlignment
        {
            get { return _itemHorizonAlign; }
            set
            {
                _itemHorizonAlign = value;
                FrameworkElement fe;
                for (int i = 0; i < _items.Count; i++)
                {
                    fe = _items[i] as FrameworkElement;
                    if ((fe is CheckBox) == false) fe.HorizontalAlignment = value;
                    else fe.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                }
            }
        }
        public List<Boolean> Editables
        {
            get { return _editables; }
            set {
                _editables = value;
                
            }
        }
        void ListTree_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //this.RaiseEvent(new RoutedEventArgs());
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

        public RtwListRow()
            : base()
        {
            init((new double[] { }).ToList(), 30);
        }

        public RtwListRow(List<double> width, double height, Boolean wrap = false, Rectangle sep = null, params UIElement[] items)
            : base()
        {
            setSeperator(sep);
            init(width, height);
            AddItems(wrap, items);
        }

        public RtwListRow(List<double> width, double height, Boolean wrap = false, params UIElement[] items)
            : base()
        {
            init(width, height);
            AddItems(wrap, items);
        }

        public RtwListRow(List<double> width, double height, Boolean wrap = false, List<UIElement> items = null)
            : base()
        {
            init(width, height);
            AddItems(wrap, items);
        }

        public RtwListRow(List<double> width, double height, Boolean wrap = false, Rectangle sep = null, List<UIElement> items = null)
            : base()
        {
            setSeperator(sep);
            init(width, height);
            AddItems(wrap, items);
        }

        public void setSeperator(Rectangle rect)
        {
            _seperator = rect;
            IndexSepStyle = rect.Style;
        }

        public Style IndexSepStyle
        {
            get { return _indexSep.Style; }
            set{
                _indexSep.Style = value;
                NotifyPropertyChanged("IndexSepStyle");
            }
        }

        Color _selectedColor = Color.FromArgb(150,255,100,100);
        public Color SelectedColor { get { return _selectedColor; } set { _selectedColor = value; } }

        bool _backSelected = false;
        public bool BackSelected
        {
            get { return _backSelected; }
            set
            {
                if (value) _selectBack.Fill = new SolidColorBrush(_selectedColor);
                else _selectBack.Fill = Brushes.Transparent;
            }
        }
        
        public void setEnabled(bool isEnabled, String exception, params String[] exceptions)
        {
            
            List<int> index = new List<int>();
            int no = _colName.IndexOf(exception);
            if (no >= 0) index.Add(no);

            for (int i = 0; i < exceptions.Length; i++)
            {
                no = _colName.IndexOf(exceptions[i]);
                if (no >= 0) index.Add(no);
            }

            if (index.Count > 0) setEnabled(isEnabled, index.ToArray());
            else setEnabled(isEnabled);
        }

        public void setEnabled(bool isEnabled, params int[] exceptions)
        {
            for (int i = 0; i < _contentGrids.Count; i++)
            {
                if (exceptions==null || exceptions.Length==0 || exceptions.Contains(i) == false) _contentGrids[i].IsEnabled = isEnabled;
            }
        }
        public void setEnabled(int index, bool isEnabled)
        {
            _contentGrids[index].IsEnabled = isEnabled;
        }
        public void setEnabled(String name, bool isEnabled)
        {
            int index = _colName.IndexOf(name);
            setEnabled(index, isEnabled);
        }

        public void Clear()
        {
            _panel.Children.Clear();
            _panel.Children.Add(_indexGrid);
            _items.Clear();
        }

        Rectangle _selectBack = new Rectangle();
        public Rectangle SelectRect { get { return _selectBack; } } 

        Rectangle _indexSep = new Rectangle();
        MouseButtonEventHandler _sepPress;
        MouseButtonEventHandler _sepRelease;
        MouseEventHandler _sepMove;
        Rectangle _indexBack = new Rectangle();
        public void init(List<double> width, double height)
        {
            this.Content = MainGrid;

            _selectBack.Fill = new SolidColorBrush(Colors.Transparent);

            MainGrid.Children.Add(_selectBack);
            MainGrid.Children.Add(_panel);
            _panel.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            _panel.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            _panel.Margin = new Thickness(3, 0, 0, 0);
            _panel.Orientation = Orientation.Horizontal;
            
            _indexText.Text = "";
            _indexText.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            _indexText.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            _indexText.Margin = new Thickness(0,0,5,0);

            if (_seperator == null)
            {
                _indexSep.Width = 1;
            }
            else
            {
                _indexSep.Style = _seperator.Style;
            }
            _indexSep.Margin = new Thickness(2, 0, 2, 0);
            _indexSep.Height = height - 6;
            _indexSep.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            _indexSep.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            
            _sepPress = new MouseButtonEventHandler(sepPress);
            _sepMove = new MouseEventHandler(sepMove);
            _sepRelease = new MouseButtonEventHandler(sepRelease);


            _indexGrid.MouseDown += _sepPress;
            _indexGrid.MouseMove += _sepMove;
            _indexGrid.MouseUp += _sepRelease;
            _panel.MouseLeave += new MouseEventHandler(_panel_MouseLeave);
            

            
            _indexBack.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            _indexBack.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            _indexBack.Fill = (new SolidColorBrush(Color.FromArgb(100,200,200,200))); //index back
            
            _indexGrid.Children.Add(_indexBack);
            _indexGrid.Children.Add(_indexSep);
            _indexGrid.Children.Add(_indexText);
            _indexGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            _indexGrid.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            _indexGrid.Width = IndexWidth;
            _indexGrid.Visibility = (ShowIndex)? Visibility.Visible : System.Windows.Visibility.Collapsed ;
            _indexGrid.Tag = -1; //cell index

            _panel.Children.Add(_indexGrid);
            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            this.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            this.Height = height;
            this._wid = width;
            

            //this.Children.Add(panel);
            //this.AddLogicalChild(panel);
            //this.Content = panel;
            //this.AddVisualChild(panel);
            //ItemHorizontalAlignment = System.Windows.HorizontalAlignment.Left;
        }

      

        public void setCellWidth(int cellIndex, double width)
        {
            Grid grid = (cellIndex<0)? _indexGrid as Grid : _contentGrids[cellIndex] as Grid;
            

            if (width < 2) width = 2;
            //double afterWidth = _sepControlGrid.TranslatePoint(pt, _sepControlGrid).X;
            //if (E_CellWidthChanged != null) E_CellWidthChanged(this, new ListRowCellWidthChangedArgs(_initWidth, afterWidth, cellIndex, Index));
            
            if (cellIndex >= 0)
            {
                grid.Width = width;
                _wid[cellIndex] = width;

                try
                {
                    (_items[cellIndex] as FrameworkElement).Width = width;
                }
                catch { }
            }
            else IndexWidth = width;
        }



        void _panel_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        void sepRelease(object sender, MouseButtonEventArgs e)
        {
            if (_sepControlGrid == null) //경계가 아닌 부분을 눌렀을 때..
            {
                Grid col = sender as Grid;
                int colIndex = (int)(col.Tag);
                if (E_CellClicked != null)
                {
                    if (colIndex < 0)
                    {
                        E_CellClicked(_indexText, new ListRowCellClickedArgs(colIndex, Index, _indexText.Text,e));
                    }
                    else
                    {
                        E_CellClicked(_items[colIndex], new ListRowCellClickedArgs(colIndex, Index, _items[colIndex],e));
                    }
                }
            }
            else //경계선 눌렀다 놨을때
            {
                Grid gr = (sender as Grid);
                //Point pt = e.GetPosition(gr);
                Point pt = e.GetPosition(_panel);
                Point grPt = e.GetPosition(gr);

                int cellIndex = (int)(_sepControlGrid.Tag);
                double left = 0;
                if (cellIndex >= 0) left += IndexWidth;
                for (int i = 0; i < cellIndex; i++)
                {
                    left += _wid[i];
                }
                double afterWidth = _initWidth + (pt.X - _initX) - left;
                if (afterWidth < 0) afterWidth = 2;
                //double afterWidth = _sepControlGrid.TranslatePoint(pt, _sepControlGrid).X;
                
                _sepControlGrid.Width = afterWidth;
                
                if (cellIndex >= 0)
                {
                    _wid[cellIndex] = afterWidth;
                    _contentGrids[cellIndex].Width = afterWidth;
                    
                    try
                    {
                        (_items[cellIndex] as FrameworkElement).Width = afterWidth;
                    }
                    catch { }

                }
                else IndexWidth = afterWidth;

                _sepControlGrid = null;
                Mouse.OverrideCursor = Cursors.Arrow;

                _indexText.Text = (Index < 0) ? "no" : (Index + StartIndex).ToString();
                if (E_CellWidthChanged != null) E_CellWidthChanged(this, new ListRowCellWidthChangedArgs(_initWidth, afterWidth, cellIndex, Index));
            }
            e.Handled = true;
        }


        
        void sepMove(object sender, MouseEventArgs e)
        {
            //if (e.LeftButton == MouseButtonState.Pressed)
            //{
            Grid gr = (sender as Grid);
            //Point pt = e.GetPosition(gr);
            Point pt = e.GetPosition(_panel);
            Point grPt = e.GetPosition(gr);

                if (_sepControlGrid != null)
                {
                    int cellIndex = (int)(_sepControlGrid.Tag);
                    double left = 0;
                    if(cellIndex>=0) left+=IndexWidth;
                    for (int i = 0; i < cellIndex; i++)
                    {
                        left += _wid[i];
                    }
                    double afterWidth = _initWidth + (pt.X - _initX) - left;
                    if (afterWidth < 0) afterWidth = 2;
                    //double afterWidth = _sepControlGrid.TranslatePoint(pt, _sepControlGrid).X;
                    _sepControlGrid.Width = afterWidth;
                    if (cellIndex >= 0) _wid[cellIndex] = afterWidth;
                    else IndexWidth = afterWidth;

                    if (e.LeftButton == MouseButtonState.Released)
                    {
                        if (_sepControlGrid != null)
                        {
                            _sepControlGrid = null;
                            Mouse.OverrideCursor = Cursors.Arrow;

                            _indexText.Text = (Index < 0) ? "no" : (Index + StartIndex).ToString();
                            if (E_CellWidthChanged != null) E_CellWidthChanged(this, new ListRowCellWidthChangedArgs(_initWidth, afterWidth, cellIndex, Index));

                        }
                    }
                    else
                    {
                        if (_sepControlGrid != null && Math.Abs(afterWidth-_initWidth)>3) _indexText.Text = pt.X.ToString();
                    }
                }
                
            //}
            //else
                if (e.LeftButton == MouseButtonState.Released)
                {
                    if (_sepControlGrid == null)
                    
                    {
                        if (grPt.X >= (gr.Width - 10) && grPt.X < gr.Width) //우측 10pt에서 마우스 눌렀을 때.
                        {
                            if (Mouse.OverrideCursor != Cursors.SizeWE) Mouse.OverrideCursor = Cursors.SizeWE;
                        }
                        else
                        {
                            if (Mouse.OverrideCursor != Cursors.Arrow) Mouse.OverrideCursor = Cursors.Arrow;
                        }
                    }
                }
                else
                {
                    
                }
        }

        Grid _sepControlGrid = null;
        double _initWidth = 0;
        double _initX = 0;
        void sepPress(object sender, MouseButtonEventArgs e)
        {
            Grid gr = (sender as Grid);
            Point pt = e.GetPosition(gr);

            if (pt.X >= (gr.Width - 10)) //우측 10pt에서 마우스 눌렀을 때.
            {
                _sepControlGrid = gr;
                _initX = pt.X;
                _initWidth = _sepControlGrid.Width;
            }
            else
            {
                int col = _contentGrids.IndexOf(gr);
                if(E_CellMouseDown!=null) E_CellMouseDown(this, new ListRowCellClickedArgs(col, Index, _items[col], e));
            }
            
            //Mouse.OverrideCursor = Cursors.SizeWE;
        }

        public double TotalWidth
        {
            get
            {
                double wid = 0;
                for (int i = 0; i < _wid.Count; i++)
                {
                    wid += _wid[i];
                }
                if (ShowIndex) wid += IndexWidth;
                return wid;
            }
        }

        public void AddItems(Boolean wrap = false, List<UIElement> items = null)
        {
            if (items == null) return;

            UIElement element;

            for (int i = 0; i < items.Count; i++)
            {
                element = items[i];

                addAnItem(wrap, element, (_wid.Count > i) ? _wid[i] : (double)(element.GetValue(WidthProperty)));
            }
        }

        public void AddItems(Boolean wrap = false, params UIElement[] items)
        {
            if (items == null) return;

            UIElement element;

            for (int i = 0; i < items.Length; i++)
            {
                element = items[i];
                addAnItem(wrap, element, (_wid.Count > i) ? _wid[i] : (double)(element.GetValue(WidthProperty)));
            }
        }

        public void setModifyingText(String text)
        {
            tempBox.Text = text;
            tempBox.Select(text.Length, 0);
        }

        //int modifingIndex;
        //UIElement modifingTemp;
        TextBlock modifyingText;
        TextBox tempBox = new TextBox();
        String _savedText = "";
        public void setModifyMode(int colIndex)
        {
            
            if (colIndex < 0) return;
            try
            {
                if (_isModifying == false && _items[colIndex] is TextBlock)
                {
                    _isModifying = true;
                    //modifingIndex = colIndex;
                    TextBlock textBlock = (_items[colIndex] as TextBlock);
                    String text = textBlock.Text;
                    textBlock.Visibility = System.Windows.Visibility.Hidden;
                    double wid = (_wid.Count>colIndex && _wid[colIndex]>=0)? _wid[colIndex]: textBlock.Width;
                    TextBox box = tempBox;
                    box.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    box.BorderBrush = null;
                    box.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
                    box.PreviewKeyUp += new KeyEventHandler(box_KeyUp);
                    box.LostFocus += new RoutedEventHandler(box_LostFocus);
                    box.Width = textBlock.Width;
                    Point pt = _panel.PointFromScreen(_panel.Children[colIndex+1].PointToScreen(new Point(0, 0)));
                    box.Margin = new Thickness(pt.X, 0, 0, 0);
                    box.Text = text;
                    box.SelectAll();
                    //modifingTemp = panel.Children[modifingIndex];
                    modifyingText = textBlock;
                    _savedText = text;
                    this.MainGrid.Children.Add(box);

                    box.Focus();
                    ActivatedRow = this;
                }
            }
            catch(Exception e) {
                throw e;
            }
        }

        void box_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                /*
                if (e.Key == Key.Enter || e.Key == Key.Return)
                {
                    Unfocus(sender);
                }
                 */
                if (e.Key == Key.Escape)
                {
            
                }
            }
            
            catch(Exception ex) {
                throw ex;
            }
            e.Handled = true;
        }

        void box_LostFocus(object sender, RoutedEventArgs e)
        {
            
            Unfocus(sender);
            
        }
        public void CancelModify()
        {
            tempBox.Text = _savedText;
            Unfocus();
        }
        
        public void Unfocus(object sender=null)
        {
            if (tempBox == null) return;
            if (sender == null) sender = tempBox;
            if (modifyingText == null) return;
            TextBox box = sender as TextBox;
            String beforeText = modifyingText.Text;
            string text = box.Text;
            modifyingText.Text = text;
            modifyingText.ToolTip = text;
            MainGrid.Children.Remove(box);
            modifyingText.Visibility = System.Windows.Visibility.Visible;
            _isModifying = false;
            ActivatedRow = null;
            tempBox.ReleaseMouseCapture();
            int col = _items.IndexOf(modifyingText);
            if (E_TextChanged != null) E_TextChanged(this, new ListRowTextChangedEventArgs(this, modifyingText, beforeText, text,col));
        }

        public static void DeActivate()
        {
            if (ActivatedRow != null)
            {
                ActivatedRow.Unfocus();
            }
        }
        

        public void addAnItem(Boolean wrap, UIElement element, double width)
        {
            if(width>=0) _wid.Add(width);
            if (wrap == true)
            {
                Grid col;
                
                element.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
                element.SetValue(HorizontalAlignmentProperty, ItemHorizontalAlignment);
                element.SetValue(MarginProperty, new Thickness(0,0,0,0));
                //Grid colBack = new Grid();

                col = new Grid();
                Rectangle back = new Rectangle();
                col.Children.Add(back);
                col.Children.Add(element);//[0] = element;//.Add(innerGrid);

                

                _panel.Children.Add(col); //StackPanel에 들어가는 것은 실제로는 Grid이다.
                col.Width = width;
                col.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                
                back.Fill = Brushes.Transparent;
                back.Stretch = Stretch.Fill;
                back.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                back.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                

                if (width < 0 || width.ToString().Equals("NaN"))
                {
                    
                    width = (double)element.GetValue(WidthProperty)+5;
                    if (width < 0 || width.ToString().Equals("NaN"))
                    {
                        width = -1;
                    }
                }
                if (width > 0) col.Width = width;




                /*
                ColumnDefinition coldef = new ColumnDefinition();
                    
                if(width > 0) coldef.Width = new GridLength(width - 5);
                col.ColumnDefinitions.Add(coldef);
                coldef = new ColumnDefinition();
                coldef.Width = new GridLength(5);
                
                col.ColumnDefinitions.Add(coldef);
                
                if (width > 0)
                {
                    if (((element is TextBlock) == false)) element.SetValue(WidthProperty, width - 5);
                    else if (ItemHorizontalAlignment != System.Windows.HorizontalAlignment.Center) element.SetValue(WidthProperty, width - 5);

                    
                }
                */

                //col.Children[0].SetValue(Grid.ColumnProperty,0);

                //이벤트 삽입
                col.MouseDown += _sepPress;
                col.MouseMove += _sepMove;
                col.MouseUp += _sepRelease;

                if (element is ComboBox)
                {
                    (element as ComboBox).SelectionChanged += new SelectionChangedEventHandler(ListRow_SelectionChanged);
                    (element as ComboBox).VerticalContentAlignment = System.Windows.VerticalAlignment.Top;
                    (element as ComboBox).VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    (element as ComboBox).Padding = new Thickness(0, 0, 0, 0);
                    (element as ComboBox).Margin = new Thickness(0, 0, 0, 0);

                }
                else if (element is CheckBox)
                {
                    (element as CheckBox).Checked += new RoutedEventHandler(ListRow_Checked);
                    (element as CheckBox).Unchecked += new RoutedEventHandler(ListRow_Checked);

                }
                else if (element is TextBlock)
                {
                    (element as TextBlock).HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                }


                //innerGrid.Children.Add(element);

                #region 경계선 삽입
                Rectangle rect;
                if (_seperator == null)
                {
                    rect = new Rectangle();
                    rect.Fill = Brushes.LightGray ;
                    rect.Width = 1;
                    _indexSep.Fill = rect.Fill;
                }
                else
                {
                    rect = new Rectangle();
                    rect.Style = _seperator.Style;
                    
                    IndexSepStyle = rect.Style;
                }

                rect.Height = this.Height - 6;
                rect.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                rect.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                rect.Margin = new Thickness(0, 0, 0, 0);

                col.Children.Add(rect);
                //col.Children[1].SetValue(Grid.ColumnProperty, 1);
                
                col.Tag = _items.Count; //cell index
                #endregion

                _items.Add(element);//관리되는 item은 element이다.
                _contentGrids.Add(col);
            }
            else
            {
                Grid col = new Grid();
                col.Width = width;
                col.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                col.Children.Add(element);
                _items.Add(element);
                _panel.Children.Add(col);
                _contentGrids.Add(col);
            }
        }

        void ListRow_Checked(object sender, RoutedEventArgs e)
        {
            if (E_CheckBoxChanged != null)
            {
                CheckBox c = sender as CheckBox;

                int col_index = -1;
                for (int i = 0; i < _items.Count; i++)
                {
                    if (_items[i].Equals(c))
                    {
                        col_index = i;
                        ListCheckedEventArgs arg = new ListCheckedEventArgs(c.IsChecked, -1,col_index);
                        arg.Checked = c.IsChecked;
                        arg.ColumnIndex = col_index;
                        arg.RowIndex = -1;
                        arg.Source = this;
                        arg.OriginalSource = c;
                        E_CheckBoxChanged(this, arg);
                        break;
                    }
                }

            }
        }

        void ListRow_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (E_ComboBoxChanged != null)
            {
                ComboBox c = sender as ComboBox;
                
                int col_index = -1;
                for (int i = 0; i < _items.Count; i++)
                {
                    if (_items[i].Equals(c))
                    {
                        col_index = i;
                        ListComboBoxEventArgs arg = new ListComboBoxEventArgs();
                        arg.ColIndex = col_index;
                        arg.SelectedObject = c.Items[c.SelectedIndex];
                        arg.SelectedIndex = c.SelectedIndex;
                        arg.Source = this;
                        arg.OriginalSource = c;
                        E_ComboBoxChanged(this, arg);
                        break;
                    }
                }
                
            }
        }
        
        public void SetValue(int index, object value)
        {
            UIElement item = _items[index];
            if (item is TextBlock) (item as TextBlock).Text = value.ToString();
            else if (item is RtwComboBox)
            {
                RtwComboBox c = item as RtwComboBox;
                if (value is String)
                {
                    if (value is String) c.U_SelectedValue = value.ToString();
                }
                else c.U_SelectedIndex = (int)value;
            }
            else if (item is ComboBox)
            {
                ComboBox c = item as ComboBox;
                if (value is String)
                {
                    if (c.Items.Count > 0)
                    {
                        if (c.Items[0] is String)
                        {
                            c.SelectedIndex = c.Items.IndexOf(value);
                        }
                        else if (c.Items[0] is TextBlock)
                        {
                            for (int i = 0; i < c.Items.Count; i++)
                            {
                                if ((c.Items[i] as TextBlock).Text.Equals(value))
                                {
                                    c.SelectedIndex = i;
                                    break;
                                }
                            }
                        }
                    }
                }
                else (item as ComboBox).SelectedIndex = (int)value;
            }
            else if (item is CheckBox)
            {
                CheckBox c = item as CheckBox;
                if (value is bool? || value is Boolean)
                {
                    c.IsChecked = (bool?)value;
                }
                else if (value is String)
                {
                    String boolstr = value.ToString().ToLower();
                    c.IsChecked = (boolstr.Equals("true") || boolstr.Equals("t")) ? true : (boolstr.Equals("false") || boolstr.Equals("f")) ? false : (bool?)null;
                }
            }
        }

        public void SetValue(Object col_name, object value)
        {
            int num = _colName.IndexOf(col_name);
            SetValue(num, value);
        }

        public UIElement this[int num]
        {
            set
            {
                _items[num] = value;
                _panel.Children[num] = value;
            }
            get
            {
                return _items[num];
                //return Children[num];
            }
        }

        public UIElement this[Object name]
        {
            set
            {
                int num = _colName.IndexOf(name);
                _items[num] = value;
                _panel.Children[num] = value;
            }
            get
            {
                int num = _colName.IndexOf(name);
                if (num >= 0) return _items[num];
                else return null;
                //return Children[num];
            }
        }
        Boolean _isSelected = false;
        public Boolean Selected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
               
                NotifyPropertyChanged("Selected");
            }
        }

        int _index = -1;
        public int Index {
            get { return _index; }
            set { 
                _index = value;
                _indexText.Text = (value + StartIndex).ToString();
            } 
        }

        int _startIndex = 1;
        public int StartIndex {
            get { return _startIndex; }
            set { _startIndex = value;} 
        }
        
        public int Count { get { return _items.Count; } }

        public int IndexOf(UIElement item)
        {
            return _items.IndexOf(item);
        }

        public void setRealItem(int id, UIElement item)
        {
            _items[id] = item;
        }

        public List<Object> getValues()
        {
            List<object> values = new List<object>();
            for (int i = 0; i < _items.Count; i++)
            {
                if(_items[i] is TextBox){
                    values.Add((_items[i] as TextBox).Text);
                }
                else if (_items[i] is TextBlock)
                {
                    values.Add((_items[i] as TextBlock).Text);
                }
                else if (_items[i] is CheckBox)
                {
                    values.Add((_items[i] as CheckBox).IsChecked);
                }
                else if (_items[i] is ComboBox)
                {
                    values.Add((_items[i] as ComboBox).SelectedValue);
                }
                else
                {
                    values.Add(_items[i]);
                }
            }
            return values;
        }

        public Dictionary<Object, Object> getValuesDic()
        {
            Dictionary<Object, object> values = new Dictionary<Object, object>();
            
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i] is TextBox)
                {
                    values.Add(_colName[i],(_items[i] as TextBox).Text);
                }
                else if (_items[i] is TextBlock)
                {
                    values.Add( _colName[i], (_items[i] as TextBlock).Text);
                }
                else if (_items[i] is CheckBox)
                {
                    values.Add(_colName[i], (_items[i] as CheckBox).IsChecked);
                }
                else if (_items[i] is ComboBox)
                {
                    values.Add(_colName[i], (_items[i] as ComboBox).SelectedIndex);
                }
                else
                {
                    values.Add(_colName[i], _items[i]);
                }
            }
            return values;
        }
    }
    public enum ListRowItemKind{ TextBox=0, TextBlock, CheckBox, ComboBox};
    public delegate void ListRowTextChangedEventHandler(object sender, ListRowTextChangedEventArgs e);

    public class ListRowTextChangedEventArgs : EventArgs
    {
        public int RowIndex { get; set; }
        public int ColIndex { get; set; }
        public TextBlock OriginalSource { get; set; }
        public RtwListRow OriginalRow { get; set; }
        public String Text { get; set; }
        public String BeforeText { get; set; }
        public ListRowTextChangedEventArgs(RtwListRow rowObj, TextBlock txtObj, String beforeText, String afterText,int col, int row = -1)
        {
            RowIndex = row;
            ColIndex = col;
            OriginalRow = rowObj;
            OriginalSource = txtObj;
            Text = afterText;
            BeforeText = beforeText;
        }
    }

    public delegate void ListRowCellWidthChangedHandler(object sender, ListRowCellWidthChangedArgs e);
    public class ListRowCellWidthChangedArgs : EventArgs
    {
        public double BeforeWidth { get; set; }
        public double AfterWidth { get; set; }
        public int CellIndex { get; set; }
        public int RowIndex { get; set; }
        public ListRowCellWidthChangedArgs(double beforeWidth, double afterWidth, int cellIndex, int rowIndex)
        {
            BeforeWidth = beforeWidth;
            AfterWidth = afterWidth;
            CellIndex = cellIndex;
            RowIndex = rowIndex;
        }
    }

    public delegate void ListRowCellClickedHandler(object sender, ListRowCellClickedArgs e);
    public class ListRowCellClickedArgs : MouseButtonEventArgs
    {
        public int ColIndex { get; set; }
        public int RowIndex { get; set; }
        public object Item { get; set; }
        public ListRowCellClickedArgs(int col, int row, object item, MouseButtonEventArgs e):base(e.MouseDevice, e.Timestamp, e.ChangedButton, e.StylusDevice)
        {
            ColIndex = col;
            RowIndex = row;
            Item = item;
        }
    }

}
