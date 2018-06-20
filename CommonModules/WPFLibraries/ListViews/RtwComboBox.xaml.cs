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
using Drawing = System.Drawing;
namespace RtwWpfControls
{
    /// <summary>
    /// RtwComboBox.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class RtwComboBox : UserControl
    {
        public event RtwComboBoxSelectedEventHandler E_SelectionChanged;
        public event RtwComboBoxSelectedEventHandler E_KeyDown;
        public event RtwComboBoxSelectedEventHandler E_MouseDown;
        public RtwComboBox()
        {
            InitializeComponent();
            comboBox1.SelectionChanged += new SelectionChangedEventHandler(comboBox1_SelectionChanged);
            comboBox1.MouseDown += new MouseButtonEventHandler(comboBox1_MouseDown);
            comboBox1.KeyDown += new KeyEventHandler(comboBox1_KeyDown);
            
        }

        void comboBox1_KeyDown(object sender, KeyEventArgs e)
        {
            ComboBox c = sender as ComboBox;
            String selText = _items[c.SelectedIndex].Text;
            if (E_SelectionChanged != null) E_SelectionChanged(c, new RtwComboBoxItemSelectedArgs(c.SelectedIndex, selText, _items[c.SelectedIndex]));
        }

        void comboBox1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ComboBox c = sender as ComboBox;
            String selText = _items[c.SelectedIndex].Text;
            if (E_SelectionChanged != null) E_SelectionChanged(c, new RtwComboBoxItemSelectedArgs(c.SelectedIndex, selText, _items[c.SelectedIndex]));
        }

        void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox c = sender as ComboBox;
            try
            {
                String selText = _items[c.SelectedIndex].Text;
                if (E_SelectionChanged != null) E_SelectionChanged(c, new RtwComboBoxItemSelectedArgs(c.SelectedIndex, selText, _items[c.SelectedIndex]));
            }
            catch { }
        }

        List<RtwComboBoxItem> _items = new List<RtwComboBoxItem>();
        public List<RtwComboBoxItem> Items { get { return _items; } }

        public String this[int index]
        {
            get
            {
                return _items[index].Text;
            }
            set
            {
                _items[index].Text = value;
            }
        }

        public String U_Text
        {
            get { return comboBox1.Text; }
            set { comboBox1.Text = value; }
        }

        public int U_SelectedIndex
        {
            get
            {
                return comboBox1.SelectedIndex;
            }
            set
            {
                comboBox1.SelectedIndex = value;
            }
        }

        public RtwComboBoxItem U_SelectedItem
        {
            get
            {
                return comboBox1.SelectedItem as RtwComboBoxItem;
            }
            
        }

        public String U_SelectedValue
        {
            get
            {
                return (comboBox1.SelectedItem as RtwComboBoxItem).Text;
            }
            set
            {
                foreach (RtwComboBoxItem item in comboBox1.Items)
                {
                    if (item.Text.Equals(value))
                    {
                        comboBox1.SelectedItem = item;
                    }
                }
            }
        }

        public void Add(String text, double fontSize=-1)
        {
            Add(text, Colors.Black, SystemFonts.MessageFontFamily, FontStyles.Normal, fontSize);
        }
        public void Add(String text, Color color, double fontSize = -1)
        {
            Add(text,color, SystemFonts.MessageFontFamily, FontStyles.Normal, fontSize);
        }
        public void Add(String text, Drawing.Color iColor, double fontSize = -1)
        {
            Color color = new Color();
            color.A = iColor.A;
            color.R = iColor.R;
            color.G = iColor.G;
            color.B = iColor.B;
            Add(text, color, SystemFonts.MessageFontFamily, FontStyles.Normal, fontSize);
        }
        public void Add(String text, Color color, FontFamily font, double fontSize = -1)
        {
            Add(text, Colors.Black, font, FontStyles.Normal);
        }
        public void Add(String text, Color color, FontFamily font, FontStyle fontStyle, double fontSize = -1)
        {
            RtwComboBoxItem item = new RtwComboBoxItem(text, color, font, fontStyle, fontSize);
            _items.Add(item);
            comboBox1.Items.Add(item);
        }

        public void RemoveAll()
        {
            _items.Clear();
        }
        public void Remove(int index)
        {
            _items.RemoveAt(index);
        }
        public void Remove(RtwComboBoxItem item)
        {
            _items.Remove(item);
        }
        public void SetColor(int index, Color color)
        {
            _items[index].TextColor = color;
        }
    }

    public delegate void RtwComboBoxSelectedEventHandler(Object sender, RtwComboBoxItemSelectedArgs arg);
    public class RtwComboBoxItemSelectedArgs : EventArgs
    {
        public int Index;
        public String SelectedText;
        public RtwComboBoxItem SelectedItem;
        public RtwComboBoxItemSelectedArgs(int index, String selText, RtwComboBoxItem item)
        {
            Index = index;
            SelectedItem = item;
            SelectedText = selText;
        }
    }


    public class RtwComboBoxItem:TextBlock
    {
        public Color _textColor = Colors.Black;
        
        public RtwComboBoxItem(String text, double fontSize=-1)
        {
            init(text, Colors.Black, SystemFonts.MenuFontFamily, SystemFonts.MenuFontStyle, (fontSize<0)? SystemFonts.MenuFontSize: fontSize);
        }
        public RtwComboBoxItem(String text, Color color, double fontSize=-1)
        {
            init(text, color, SystemFonts.MenuFontFamily, SystemFonts.MenuFontStyle, (fontSize < 0) ? SystemFonts.MenuFontSize : fontSize);
            
        }
        public RtwComboBoxItem(String text, Color color, FontFamily fontFamily, double fontSize = -1)
        {
            init(text, color, fontFamily, SystemFonts.MenuFontStyle, (fontSize < 0) ? SystemFonts.MenuFontSize : fontSize);
        }
        public RtwComboBoxItem(String text, Color color, FontFamily fontFamily, FontStyle fontStyle, double fontSize=12)
        {
            init(text, color, fontFamily, fontStyle, (fontSize < 0) ? SystemFonts.MenuFontSize : fontSize);
        }
        void init(String text, Color color, FontFamily fontFamily, FontStyle fontStyle, double fontSize)
        {
            TextColor = color;
            base.Text = text;
            base.FontSize = fontSize;
            //base.Foreground = new SolidColorBrush(color);
            base.FontStyle = fontStyle;
            base.FontFamily = fontFamily;
        }

        public Color TextColor
        {
            get { return _textColor; }
            set
            {
                _textColor = value;
                base.Foreground = new SolidColorBrush(value);
            }
        }
    }
}
