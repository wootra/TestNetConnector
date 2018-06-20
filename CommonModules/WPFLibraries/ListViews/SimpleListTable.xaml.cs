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
using WPF_Handler;

namespace ListViews
{
    /// <summary>
    /// UserControl1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SimpleListTable:UserControl, INotifyPropertyChanged
    {
        List<double> _wid = new List<double>();
        public List<Object> _titles = new List<Object>();
        public Dictionary<Object, Object> _titleDic = new Dictionary<object, object>();
        public event PropertyChangedEventHandler PropertyChanged;
        public List<ListRow> _rows = new List<ListRow>();
        Boolean _isChecking = false;
        public ListRow Header { get; set; }

        public SimpleListTable():base()
        {
            InitializeComponent();
            Header = new ListRow();
            /*
            _header.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            _header.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            _header.Orientation = Orientation.Horizontal;
             */
            this.PropertyChanged += new PropertyChangedEventHandler(SimpleListTable_PropertyChanged);
            
            for (int i = 0; i < 4; i++)
            {
                AddTitleColumn(i, "test"+i,50);
            }

            AddValue(1, 2, 3, 4);
            AddValue(1, 2, 3, 4);
            AddValue(1, 2, 3, 4);
            AddValue(1, 2, 3, 4);
            AddValue(1, 2, 3, 4);

            /*
            this.DataList.Add(new Object[] { "name1", 0, true });
            this.DataList.Add(new Object[] { "name2", 0, true });
            this.DataList.Add(new Object[] { "name3", 0, true });
            this.DataList.Add(new Object[] { "name4", 0, true });
            
            
            Category = "cat1";
            this.ContentTemplate = this.Resources["dTemplate"] as DataTemplate;
            */
        }

        void SimpleListTable_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //L_Title.Text = e.PropertyName;
        }

        protected void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                if (_isChecking) return;
                _isChecking = true;//무한반복을 막기 위함.
                PropertyChanged(this, new PropertyChangedEventArgs(info));
                _isChecking = false;
            }
        }
        public List<ListRow> Rows
        {
            get { return _rows; }
        }

        public new Object Tag
        {
            get { return base.Tag; }
            set
            {
                base.Tag = value;

                //L_Title.Text = value as String;
                NotifyPropertyChanged("Tag");
            }
        }

        public List<double> Wid { get { return _wid; } set { _wid = value; } }
        
        public List<Object> Titles
        {
            get { return _titles; }
            set
            {
                _titles = value;
            }
        }

        public void AddTitleColumn(Object key, Object obj, double width)
        {
            
            Grid grid = new Grid();
            grid.Height = 15;
            grid.Width = width;

            AddToTitle(obj,width);

            UIElement element = WpfFinder.getUiElement(obj);
            
            element.SetValue(VerticalAlignmentProperty , VerticalAlignment.Center);
            element.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);

            
            grid.Children.Add(element);

            Rectangle rect = new Rectangle();
            

            rect.Style = this.FindResource("SeperatorLine") as Style;
            rect.Margin = new Thickness(width-3, 3, 0, 3);
            rect.Width = 1;
            rect.Height = 15;
            rect.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            grid.Children.Add(rect);
            
                
            _titles.Add(obj);
            _titleDic.Add(key, element);
            //_wid.Add(width);
            
        }

        void AddToTitle(Object obj,double wid)
        {
            Header.addAnItem(true, WpfFinder.getUiElement(obj), wid);
            _wid.Add(wid);
        }
        
        public void AddValue(params object[] values)
        {
            if (values == null || values.Length == 0) return;
            if (values != null && values[0] is object[]) values = values[0] as object[];

            List<UIElement> elList = new List<UIElement>();
            UIElement element;
            for (int i = 0; i < values.Length; i++)
            {
                element = WpfFinder.getUiElement(values[i]);
                elList.Add(element);
            }
            ListRow row = new ListRow(_wid, 20, true, elList);
            _rows.Add(row);
            DataList.Items.Add(row);
            
        }
       
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


        private void L_Title_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Object s = sender;

        }



    }

}
