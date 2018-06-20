using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FormAdders
{
    public partial class RtwComboBox : UserControl
    {
        public RtwComboBox()
        {
            InitializeComponent();
            B_TextArea.Click += new EventHandler(B_TextArea_Click);
            B_Expand.Click += new EventHandler(B_Expand_Click);
            base.BackColor = Color.Transparent;
        }

        void B_Expand_Click(object sender, EventArgs e)
        {
            _contextMenu.Show(this, new Point(0, this.Height));
        }

        void B_TextArea_Click(object sender, EventArgs e)
        {
            if (E_ComboBoxClick != null) E_ComboBoxClick(this, B_TextArea.Text, _selectedIndex);

        }

        
        public new Color BackColor
        {
            get
            {
                return _backColor;
                //return base.BackColor;
            }
            set
            {
                _backColor = value;
                base.BackColor = Color.Transparent;
                //base.BackColor = value;
            }
        }

        
        public Color TextBackColor
        {
            get
            {
                return B_TextArea.BackColor;
            }
            set
            {
                B_TextArea.BackColor = value;
            }
        }

        ContextMenu _contextMenu = new ContextMenu();
        #region contextMenu

        //ContextMenu _contextMenuParentNode = new ContextMenu();
        /*
        TriStateTreeNode _selectedTreeNode; //ContextMenu를 Open할때 기준이 되는 Node
        
        public ContextMenu U_ContextMenuParentNode
        {
            get { return _contextMenuParentNode; }
        }
         *
        ContextMenu _contextMenuEndNode = new ContextMenu();
        public ContextMenu U_ContextMenuEndNode
        {
            get { return _contextMenuEndNode; }
        }
        */
        public delegate void ContextMenuClickHandler(Object sender, String text, int index);
        public event ContextMenuClickHandler E_ItemClicked = null;
        public event ContextMenuClickHandler E_ComboBoxClick = null;
        #endregion

        int _selectedIndex = -1;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set { 
                _selectedIndex = value;
                if (value >= 0 && _contextMenu.MenuItems.Count>value)
                {
                    if (_contextMenu.MenuItems[value] is ColorMenuItem)
                    {
                        B_TextArea.ForeColor = (_contextMenu.MenuItems[value] as ColorMenuItem).FontColor;
                    }
                    B_TextArea.Text = _contextMenu.MenuItems[value].Text;
                }
                else
                {
                    B_TextArea.Text = "";
                }
            }
        }

        Color _backColor = Color.FromArgb(255, 0, 0, 64);

        public String SelectedItem
        {
            get
            {
                if (_selectedIndex >= 0) return _contextMenu.MenuItems[_selectedIndex].Text;
                else return "";
            }
            set
            {
                for(int i=0; i<_contextMenu.MenuItems.Count; i++)
                {
                    if (_contextMenu.MenuItems[i].Text.Equals(value))
                    {
                        _selectedIndex = i;
                        
                        B_TextArea.Text = value;
                        if (_contextMenu.MenuItems[i] is ColorMenuItem)
                        {
                            B_TextArea.ForeColor = (_contextMenu.MenuItems[i] as ColorMenuItem).FontColor;
                        }
                        return;
                    }
                }
                B_TextArea.Text = value;
                
            }
        }

        List<String> _items = new List<string>();
        public String[] Items
        {
            get { return _items.ToArray(); }
            set { 
                _items = value.ToList();
                _contextMenu.MenuItems.Clear();
                
                    for (int i = 0; i < _items.Count; i++)
                    {
                        if (_colors.Count > i) AddAnItem(_colors[i], _items[i]);
                        else AddAnItem(_items[i]);
                    }
                
            }
        }

        List<Color> _colors = new List<Color>();
        public Color[] Colors
        {
            get { return _colors.ToArray(); }
            set { 
                _colors = value.ToList();
                _contextMenu.MenuItems.Clear();
                
                    for (int i = 0; i < _items.Count; i++)
                    {
                        if (_colors.Count > i) AddAnItem(_colors[i], _items[i]);
                        else AddAnItem(_items[i]);
                    }
                
            }

        }

        public void AddAnItem(String text)
        {
            ColorMenuItem item = new ColorMenuItem(this.ForeColor, this.BackColor);
            //ContextMenuItems.Add(item);
            item.Text = text;
            item.Click += new EventHandler(ItemClicked);
            //item.OwnerDraw = true;
            _items.Add(text);
            _colors.Add(Color.Black);
            _contextMenu.MenuItems.Add(item);
            
        }
        public void AddItems(params String[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                AddAnItem(items[i]);
            }
        }
        
        public void AddAnItem(Color fontColor, String text)
        {
            MenuItem item = new ColorMenuItem(fontColor, this.BackColor);
            item.OwnerDraw = true;

            //ContextMenuItems.Add(item);
            item.Text = text;
            item.Click += new EventHandler(ItemClicked);
            _items.Add(text);
            _colors.Add(fontColor);
            _contextMenu.MenuItems.Add(item);

        }
        public void AddItems(Color fontColor, params String[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                AddAnItem(fontColor,items[i]);
            }
        }

  
        public void AddAnItem(Color fontColor, Color backColor, String text)
        {
            MenuItem item = new ColorMenuItem(fontColor, backColor);
            //ContextMenuItems.Add(item);
            item.Text = text;
            item.Click += new EventHandler(ItemClicked);
            _items.Add(text);
            _colors.Add(fontColor);
            _contextMenu.MenuItems.Add(item);

        }
        public void AddItems(Color fontColor, Color backColor, params String[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                AddAnItem(fontColor, backColor, items[i]);
            }
        }

        void ItemClicked(object sender, EventArgs e)
        {
            MenuItem item = sender as MenuItem;
            int index = _contextMenu.MenuItems.IndexOf(item);
            //_selectedIndex = index;
            //B_TextArea.Text = _contextMenu.MenuItems[index].Text;
            SelectedIndex = index;
            if (E_ItemClicked != null) E_ItemClicked(this, item.Text.ToString(), index);
        }

        public void ClearAll()
        {
            _items.Clear();
            _contextMenu.MenuItems.Clear();
        }

    }
    
    public class ColorMenuItem : MenuItem
    {
        #region constructor
        public ColorMenuItem(Color fontColor)
            : base()
        {
            _fontBrush = new SolidBrush(fontColor);
            this.OwnerDraw = true;
        }
        public ColorMenuItem(String Text, Color fontColor)
            : base(Text)
        {
            _fontBrush = new SolidBrush(fontColor);
            this.OwnerDraw = true;
        }
        public ColorMenuItem(Color fontColor, Color backColor)
            : base()
        {
            _fontBrush = new SolidBrush(fontColor);
            _backBrush = new SolidBrush(backColor);
            this.OwnerDraw = true;
        }
        public ColorMenuItem(String Text, Color fontColor, Color backColor)
            : base(Text)
        {
            _fontBrush = new SolidBrush(fontColor);
            _backBrush = new SolidBrush(backColor);
            this.OwnerDraw = true;
        }
        #endregion

        SolidBrush _fontBrush = new SolidBrush(Color.Red);
        public Color FontColor { 
            get { return _fontBrush.Color; }
            set { 
                _fontBrush = new SolidBrush(value);
            }
        }
        SolidBrush _backBrush = new SolidBrush(SystemColors.Control);
        public Color BackColor { 
            get { return _backBrush.Color; }
            set {
                _backBrush = new SolidBrush(value);
            }
        }

        SolidBrush _selectedBackBrush = new SolidBrush(SystemColors.ButtonShadow);
        public Color SelectedBackColor
        {
            get { return _selectedBackBrush.Color; }
            set {
                _selectedBackBrush.Color = value;
                _selectedBackPen.Color = value;
            }
        }

        SolidBrush _selectedFontBrush = new SolidBrush(Color.White);
        public Color SelectedFontColor
        {
            get { return _selectedFontBrush.Color; }
            set { _selectedFontBrush.Color = value; }
        }

        Pen _selectedBackPen = new Pen(SystemColors.MenuHighlight, 1);
        Font _font = new Font("Gulim", 9, FontStyle.Regular, GraphicsUnit.Point);

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            try
            {
                _font = e.Font;
                
                e.Graphics.FillRectangle(_backBrush, e.Bounds.X,
                           e.Bounds.Y, e.Bounds.Width + 1, e.Bounds.Height + 1);
                e.Graphics.FillRectangle(_backBrush, e.Bounds.X,
                           e.Bounds.Y, 20, e.Bounds.Height + 1);
                if (base.Text == "-")
                {
                    e.Graphics.FillRectangle(_backBrush, e.Bounds.X + 20,
                               e.Bounds.Y + 2, e.Bounds.Width - 20, e.Bounds.Height - 2);
                }
                else
                {
                    
                    
                    if ((e.State & DrawItemState.Selected) != 0) //selected
                    {
                        e.Graphics.FillRectangle(_selectedBackBrush, e.Bounds);
                        e.Graphics.DrawRectangle(_selectedBackPen, e.Bounds.X,
                                   e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1);
                        e.Graphics.DrawString(base.Text, e.Font, _selectedFontBrush,
                                         e.Bounds.X + 10, e.Bounds.Y + 2);
                        
                    }
                    else //normal
                    {
                        //e.Graphics.FillRectangle(_selectedBackBrush, e.Bounds);
                        //e.Graphics.DrawRectangle(_selectedBackPen, e.Bounds.X,
                          //         e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1);
                        e.Graphics.DrawString(base.Text, e.Font, _fontBrush,
                                         e.Bounds.X + 10, e.Bounds.Y + 2);
                    }
                    
                }
            }
            catch (Exception x)
            {
                System.Diagnostics.Trace.WriteLine(x.ToString(),
                                   "WpmMenuItem (" + this.Text + "): Drawing ");
            }
        }

        protected override void OnMeasureItem(MeasureItemEventArgs e)
        {
            try
            {
                if (base.Text == "-")
                {
                    e.ItemHeight = 4;
                }
                else
                {
                    e.ItemHeight = 18;
                    e.ItemWidth = Convert.ToInt32(e.Graphics.MeasureString(this.Text, _font).Width) + 30;
                }
            }
            catch (Exception x)
            {
                System.Diagnostics.Trace.WriteLine(x.ToString(),
                       "WpmMenuItem (" + this.Text + "): Measuring ");
            }
        }
    }
}
